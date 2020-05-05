% [WinStruct,D,D0,U] = WindowDesign(Dmin,Dmax,SLL,fs,PfaMax,fMin,fMax,PdThresh)
%
% This function designs the detection segments and accompanying windows
% used by the detection algorithm in Detect.m. The Kaiser window is
% designed by calculating the design parameter based on the desired
% side-lobe level. Detection segment lengths and the amount of zero padding
% for each segment are then chosen as described in Section 4.1.5.
%
% INPUTS:
% Dmin = length of the shortest detection segment in samples
% Dmax = minimum length for the longest detection segment in samples
% SLL = desired side-lobe level of Kaiser window. This term was denoted as
%       a rho in the dissertation
% fs = sampling rate of signal that the detection algorithm will be applied
%      to
% PfaMax = maximum allowed probability of false alarm for detection
%          algorithm
% fMin = minimum frequency in detection algorithm's range
% fMax = maximum frequency in detection algorithm's range
% PdThresh = The amount of zero padding will be decided using the SNR_FO 
%            that produces Pd=PdThresh when no zero padding is used
%
% OUTPUTS:
% WinStruct = structure containing each window. WinStruct(1).win contains
%             the shortest window, WinStruct(end).win contains the longest
% D = vector containing detection segment lengths (match lengths of windows
%     in WinStruct)
% D0 = vector containing zero padded detection segment lengths
% U = vector containing scaling parameter for periodograms calculated using
%     the windows in WinStruct
% OmegaB = structure containing indices of periodogram that will correspond 
%          to frequency range of interest

function [WinStruct,D,D0,U,OmegaB,FreqOfInterest] = WindowDesign(Dmin,Dmax,SLL,fs,PfaMax,fMin,fMax,PdThresh)

%% Preliminaries  

Dshort = Dmin;      % Starting point for shorter window to start algorithm

SNRdb = -50:0.25:0;   % SNR range to search for Pd = 0.9 (part of the 
                      % process for choosing the amount of zero padding
SNRrange = 10.^(SNRdb./10);  % Convert from dB to linear scale

%% Find the Kaiser window's design parameter and U for D=Dmax (see eq. (4.49))
if SLL < 13.26
    alpha = 0;
elseif SLL < 60
    alpha = 0.76609*(SLL - 13.26)^0.4 + 0.09834*(SLL - 13.26);
elseif SLL < 120
    alpha = 0.12438*(SLL + 6.3);
else
    disp('SLL is too large (greater than 120). A rectangular window will be used.');
    alpha = 0;
end


%% Determine detection segment lengths

% Store window and length for shortest detection segment
WinStruct(1).win = kaiser(Dshort,alpha);
D = Dshort;
U = 1/(Dshort)*sum((WinStruct(1).win).^2); % Scaling parameter for periodogram

WinIdx = 1;     % Index to store window
while 1
    WinIdx = WinIdx + 1; % Increment index
    
    % Set initial values for the detection segment lengths. Dlong will be
    % incrementally increased.
    Dshort = D(end);
    Dlong = Dshort;
    
    winShort = kaiser(Dshort,alpha);    % Current short window 
    while 1
        Dlong = Dlong + fs;             % Increment the long detection segment's length
        
        % Check if the new length is greater than Dmax. If so, use Dmax as
        % the length and stop iterating 
        if Dlong >= Dmax
            WinStruct(WinIdx).win = kaiser(Dmax,alpha);
            D = [D Dmax];
            U = [U 1/Dmax*sum((WinStruct(WinIdx).win).^2)];
            break;
        end
        
        winLong = kaiser(Dlong,alpha);  % Calculate new long window
        
        % Check if the condition in eq. (4.52) has been passed. If so, back
        % up one step and take that detection segment length as the next
        % detection segment length.
        if 1/Dshort*sum(winShort)^2 > 1/Dlong*sum(winLong(end-Dshort+1:end))^2
            WinStruct(WinIdx).win = kaiser(Dlong-fs,alpha);
            D = [D Dlong-fs];
            U = [U 1/(Dlong-fs)*sum((WinStruct(WinIdx).win).^2)];
            break;
        end
    end
    
    % Check to see if the current detection segment is long enough to stop
    % iterating.
    if D(end) == Dmax
        break;
    end
end

%% Determine the amount of zero padding for each window (See Section 4.1.5)

D0 = [];
for WinNum = 1:length(D)
    % Window and associated values for this detection segment
    w = WinStruct(WinNum).win;
    DD = D(WinNum);
    UU = U(WinNum);

    % Find SNR_FO for which Probability of detection (Pd) = PdThresh. No zero padding is used.
    [~,B] = FindFreqRangeIdx(fs,DD,fMin,fMax);  % Find number of bins to be examined
    lam = SNRrange/(UU*DD)*abs(exp(1i*(0:DD-1)*pi/(2*DD))*w)^2;  % Noncentrality parameter
    Pd = 1 - ncx2cdf(-2*log(PfaMax/B),2,lam);           % Probability of detection as a function of lam
    SNR_FO = SNRrange(find(Pd>PdThresh,1,'first'));     % SNR_FO to use to find amount of zero padding
    PdThreshAct = Pd(find(Pd>PdThresh,1,'first'));      % Actual Pd for SNR_FO and no zero padding
    
    Bold = B;                       % To start iteration
    D0vec = DD:4*DD;                % Vector of D0 to check
    if length(D0vec) > 100000
        D0vec = round(linspace(DD,4*DD,100000));
    end
    Pd = 2*ones(1,length(D0vec));    % Stores Pd vs. D0
    Pd(1) = PdThreshAct;            % Just added for clarity
    for D0idx = 2:length(D0vec)
        % Determine how many bins must be examined for this level of zero
        % padding
        [~,Bnew] = FindFreqRangeIdx(fs,D0vec(D0idx),fMin,fMax);  % Find number of bins to be examined
        

        % For constant B, Pd will decrease as D0 increases. Thus, D0 for
        % which D0+1 results in a jump in B correspond to local maxima. If
        % Bnew > Bold, a jump in B occurs. So, Pd is found for D0-1 and
        % Bold. This picks out only local maxima and saves a lot of
        % computation time.
        if Bnew > Bold
            % Calculate noncentrality parameter and probability of
            % detection for D0-1 and Bold (one step back)
            lam = SNR_FO/(UU*DD)*abs(exp(1i*(0:DD-1)*pi/(2*D0vec(D0idx-1)))*w)^2;
            Pd(D0idx-1) = 1 - ncx2cdf(-2*log(PfaMax/Bold),2,lam);

            % If Pd with zero padding has dropped sufficiently below the Pd
            % without zero padding stop searching. The maximum can be
            % found.
            if Pd(D0idx-1) < PdThreshAct - 0.01
                break;
            end
        end
        
        Bold = Bnew;    % Update B
    end
    % The desired amount of zero padding corresponds to the maximum in the
    % probability of detection as a function of zero padding.
    Pd(Pd == 2) = 0;
    D0 = [D0 D0vec(Pd == max(Pd))];
    
    % Find indices of periodogram that will correspond to frequency range of interest
    [OmegaB(WinNum).bins,~,FreqOfInterest(WinNum).f] = FindFreqRangeIdx(fs,D0(end),fMin,fMax);
end

end

% function [OmegaB,B] = FindFreqRangeIdx(fs,D,fMin,fMax)
%
% This function calculates the frequency vector associated with a
% periodogram based on D samples of a signal with sampling rate fs. Then,
% it locates the indices of this frequency vector corresponding to fMin and
% fMax. The indices are chosen such that f(fMinLoc)<=fMin and
% f(fMaxLoc)>=fMax.
%
% INPUTS:
% fs = sampling rate associated with periodogram
% D = number of samples in frequency vector
% fMin = minimum frequency in range
% fMax = maximum frequency in range
%
% OUTPUTS:
% OmegaB = indices corresponding to frequency range of interest
% B = number of bins in the frequency range

function [OmegaB,B,FreqOfInterest] = FindFreqRangeIdx(fs,D,fMin,fMax)

f = fs*(0:D-1)/D;                       % Frequency vector for periodogram of length D0
fMinLoc = find(fMin-f <= 0, 1);         % Location of frequency bin nearest fMin
fMaxLoc = find(fMax-f < 0, 1)-1;        % Location of frequency bin nearest fMax
OmegaB = fMinLoc:fMaxLoc;               % Indices corresponding to frequency range of interest
B = length(OmegaB);                     % Number of bins to be examined
FreqOfInterest = f(OmegaB)';

end