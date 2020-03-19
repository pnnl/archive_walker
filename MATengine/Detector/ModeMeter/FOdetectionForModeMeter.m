% Created by Urmila Agrawal


function AdditionalOutput = FOdetectionForModeMeter(y,FOdetParam,FOlocParam,ResultUpdateInterval,fs,PastAdditionalOutput)

%Extract parameters from the structure
WindowType = FOdetParam.WindowType;
ZeroPadding = FOdetParam.ZeroPadding;
WindowLength = FOdetParam.WindowLength;
WindowOverlap = FOdetParam.WindowOverlap;
MedianFilterOrder = FOdetParam.MedianFilterOrder;
Pfa = FOdetParam.Pfa;
FrequencyMin = FOdetParam.FrequencyMin;
FrequencyMax = FOdetParam.FrequencyMax;
FrequencyTolerance = FOdetParam.FrequencyTolerance;
MinTestStatWinLength = FOdetParam.MinTestStatWinLength;

% Window for the GMSC and Daniell-Welch periodogram (PSD estimate)
GMSCandPSDwindow = eval([WindowType '(WindowLength)']);

ResultUpdateInterval = ResultUpdateInterval*fs;

%% Window design
if isempty(PastAdditionalOutput)
    % Perform initial window design
    SLL = 50;
    PdThresh = 0.9;
    [WinStruct,D,D0,U,OmegaB,FreqOfInterest] = WindowDesign(MinTestStatWinLength,length(y),SLL,fs,Pfa,FrequencyMin,FrequencyMax,PdThresh);
    
    % Store window design in AdditionalOutput
    AdditionalOutput.WinStruct = WinStruct;
    AdditionalOutput.D = D;
    AdditionalOutput.D0 = D0;
    AdditionalOutput.U = U;
    AdditionalOutput.OmegaB = OmegaB;
    AdditionalOutput.FreqOfInterest = FreqOfInterest;
    
    % Set PastFOfreq and PastTimeLoc to [] - later in the code this will signify
    % that there were no detected oscillations from the previous iteration
    % to consider
    PastFOfreq = [];
    PastTimeLoc = [];
else
    % Retrieve window design from PastAdditionalOutput
    WinStruct = PastAdditionalOutput.WinStruct;
    D = PastAdditionalOutput.D;
    D0 = PastAdditionalOutput.D0;
    U = PastAdditionalOutput.U;
    OmegaB = PastAdditionalOutput.OmegaB;
    FreqOfInterest = PastAdditionalOutput.FreqOfInterest;
    
    AdditionalOutput = PastAdditionalOutput;
    
    PastFOfreq = PastAdditionalOutput.FOfreq;
    PastTimeLoc = PastAdditionalOutput.TimeLoc;
end

%% Handle the case where y has NaN values (detection is not run)

if sum(isnan(y)) > 0
    AdditionalOutput.FOfreq = [];
    AdditionalOutput.TimeLoc = [];
    return
end

%% Perform detection

%estimates power spectrum of ambient noise using power spectrum of selected signal
freqAll = fs*(0:ZeroPadding-1)/ZeroPadding;
OmegaBpsd = find(freqAll>FrequencyMin & freqAll <FrequencyMax); %Frequency bins of interest;
AmbientNoisePSD = CalcPSD_OmegaB(y, ZeroPadding, WindowOverlap, GMSCandPSDwindow,MedianFilterOrder,fs,OmegaBpsd);

% Resample AmbientNoisePSD to create the ambient spectral estimates for the
% shorter windows.
PhiV = struct('PSD',cell(1,length(D)));
for d = 1:length(D)
    PhiV(d).PSD = interp1(freqAll(OmegaBpsd),AmbientNoisePSD,FreqOfInterest(d).f);
end

FOfreq = Detect(y,Pfa,FrequencyTolerance,D,WinStruct,U,PhiV,D0,OmegaB,PhiV(end).PSD,D0(end),OmegaB(end).bins,fs);

if FOlocParam.PerformTimeLoc
    % Run time localization
    TimeLoc = RunTimeLocalization(y,FOfreq,FOlocParam,fs);
else
    % Time localization is disabled - assume that all FOs are present for
    % the entire analysis window
    TimeLoc = [ones(length(FOfreq),1) length(y)*ones(length(FOfreq),1)];
end


% For each of the FOs detected in the previous iteration, check if it is
% still being detected. If not add it to the list of detected FOs and
% adjust its location in time. 
% This gives the detector "memory" so that FOs detected with shorter
% windows aren't lost as they pass out of the short windows, leading to
% bias.
FOadd = [];
LOCadd = [];
for fidx = 1:length(PastFOfreq)
    if (min(abs(PastFOfreq(fidx) - FOfreq)) > FrequencyTolerance)
        TooFar = true;
    else
        TooFar = false;
    end
    if TooFar || isempty(FOfreq)
        % Update the FO location based on the estimate from the
        % previous iteration
        LOCtemp = PastTimeLoc(fidx,:)-ResultUpdateInterval;
        if LOCtemp(2) < 1
            continue
        end
        if LOCtemp(1) < 1
            LOCtemp(1) = 1;
        end
        LOCadd = [LOCadd; LOCtemp];
        FOadd = [FOadd PastFOfreq(fidx)];
    end
end
FOfreq = [FOfreq FOadd];
TimeLoc = [TimeLoc; LOCadd];


AdditionalOutput.FOfreq = FOfreq;
AdditionalOutput.TimeLoc = TimeLoc;
end

% [FOfreqPool,FOdiffPool,Dpool,FOfreqAll,DpoolAll] = Detect(y,PfaMax,D,WinStruct,U,PhiV,D0,OmegaB,PhiVzp,D0zp,OmegaB_ZP,fs)
% 
% This function performs the detection algorithm on the signal contained in
% y. After the initial frequencies are found, they are refined in two ways.
% First, the bins are broken up such that each group is separated from the
% others by at least 5 frequency bins. A single frequency is then
% selected from each group based on the scaled periodogram. The second
% refining step is to remove duplicate frequencies coming from the multiple
% detection segments. Preference is given to larger segments. See Section 4.2
% for details on frequency refining.
%
% INPUTS:
% y = signal to be analyzed
% PfaMax = maximum probability of false alarm
% D = vector of detection segment lengths. max(D)<=length(y)
% WinStruct = structure containing windows with lengths corresponding to D
% U = vector of scaling terms for periodogram associated with the windows in WinStruct (see eq 4.8)
% PhiV = structure of PSDs to be used to calculate threshold. Truncated to  
%        frequency range of interest
% D0 = vector of zero padded periodogram lengths (used for detection). 
%      Also match lengths of PSDs in PhiV before they were truncated to
%      frequency range of interest.
% OmegaB = structure containing indices for each periodogram with length given by D0.
%          These indices correspond to frequency range of interest and make
%          the periodograms match the PSDs in PhiV.
% PhiVzp = zero padded PSD used in frequency refinement. Truncated to frequency range of interest
% D0zp = length of PhiVzp before being truncated to frequency range of
%        interest.
% OmegaB_ZP = indices of the full PhiVzp corresponding to frequency range
%             of interest
% fs = sampling rate of y
%
% OUTPUTS:
% FOfreqPool = vector of detected frequencies. These frequencies are
%              refined as described in Section 4.2.
% FOdiffPool = vector of differences between periodogram and PSD at 
%              the detected frequencies in FOfreqPool. These values are 
%              used in amplitude estimation.
% Dpool = vector of detection segment lengths corresponding to detected
%         frequencies in FOfreqPool.
% FOfreqAll = vector of all unrefined detected frequencies (minus
%              duplicates from the multiple detection segments)
% DpoolAll = vector of detection segment lengths corresponding to detected
%            frequencies in FOfreqAll.

function [FOfreqPool,FOdiffPool,Dpool,FOfreqAll,DpoolAll] = Detect(y,PfaMax,FrequencyTolerance,D,WinStruct,U,PhiV,D0,OmegaB,PhiVzp,D0zp,OmegaB_ZP,fs)

% Preliminaries

fZP = fs*(0:D0zp-1)/D0zp;   % Frequency vector for zero padded periodogram used in frequency refinement
fZP = fZP(OmegaB_ZP);       % Select frequency range of interest

% Result vectors
FOfreqPool = [];    % Frequencies of detected FOs
FOdiffPool = [];    % Difference between periodogram and PSD at detected frequencies. Used in amplitude estimation
Dpool = [];         % Detection segment lengths responsible for each detected frequency
FOfreqAll = [];     % Unrefined frequency estimates from all detection segments (no duplicates)
DpoolAll = [];      % Detection segment lengths corresponding to FOfreqAll

% Run detection algorithm for each detection segment length
for d = 1:length(D)
    % Preliminaries

    OmegaBseg = OmegaB(d).bins; % Indices of bins to be examined
    B = length(OmegaBseg);      % Number of bins to be examined

    % Calculate detection threshold
    gam = -PhiV(d).PSD*log(PfaMax/B);    % Detection threshold (see eq 4.35)

    % Calculate periodogram
    PhiYhat = 1/(D(d)*U(d))*abs(fft(y(end-D(d)+1:end).*WinStruct(d).win,D0(d))).^2;    % (see eq 2.34)

    % Perform hypothesis test
    OmegaFO = OmegaBseg(PhiYhat(OmegaBseg) > gam);    % FO detected when periodogram exceeds threshold
    
    f = fs*(0:D0(d)-1)'/D0(d);              % Frequency vector accompanying periodogram          
    FOfreqAll = [FOfreqAll; f(OmegaFO)];    % Contains all detected frequencies
    DpoolAll = [DpoolAll; D(d)*ones(size(f(OmegaFO)))];

    % Refine frequency estimates (leakage filter and further zero padding)
    % Also finds the difference between PhiYhat and PhiVzp to pass along to
    % periodogram-based amplitude estimation algorithm.
    if isempty(OmegaFO) == 0    % Only continue if an FO was detected
        PhiYhatZP = 1/(D(d)*U(d))*abs(fft(y(end-D(d)+1:end).*WinStruct(d).win,D0zp)).^2;    % Periodogram with extra zero padding for frequency refinement
        PhiT = 2*PhiYhatZP(OmegaB_ZP)./PhiVzp;   % Scaled periodogram (see eq. 4.24)
        PhiDiff = PhiYhatZP(OmegaB_ZP) - PhiVzp; % Used in amplitude estimation. See eq. (4.95)
        
        Loc = [0 find(diff(f(OmegaFO)')>FrequencyTolerance) length(OmegaFO)];    % Breaks frequency bins with detections 
                                                            % into groups separated by at least FrequencyTolerance Hz
                                                            
        FOfreq = zeros(1,length(Loc)-1);    % Final frequency vector
        FOdiff = zeros(1,length(Loc)-1);    % Vector of differences associated with FOfreq to be used in amplitude estimation
        Group(d).IdxZP = zeros(2,length(Loc)-1);
        for L = 1:length(Loc)-1             % For each group
            % Range of frequencies corresponding to PhiVzp that match range of
            % current group of frequency bins
            OmegaGroup = ceil(D0zp/D0(d)*(OmegaFO(Loc(L)+1)-2)+1):floor(D0zp/D0(d)*OmegaFO(Loc(L+1))+1);    

            % Limit frequency bins under consideration to those in the
            % frequency range of interest.
            OmegaGroup = intersect(OmegaGroup,OmegaB_ZP);

            % Transform to indices of PhiT
            OmegaGroup = find(OmegaB_ZP == OmegaGroup(1)):find(OmegaB_ZP == OmegaGroup(end));

            % Refined frequency estimate in Hz (see Sec. 4.2)
            FOfreq(L) = fZP(OmegaGroup(PhiT(OmegaGroup) == max(PhiT(OmegaGroup))));   

            % Will be used by amplitude estimation algorithm (see eq. (4.95))
            FOdiff(L) = PhiDiff(OmegaGroup(PhiT(OmegaGroup) == max(PhiT(OmegaGroup))));

            Group(d).IdxZP(:,L) = [OmegaGroup(1); OmegaGroup(end)];
%             Group(d).IdxZP(:,L) = [1; length(fZP)];
        end
    else
        FOfreq = [];
        FOdiff = [];
        Group(d).IdxZP = [];
    end
    
    FOfreqPool = [FOfreqPool FOfreq];
    FOdiffPool = [FOdiffPool FOdiff];
    Dpool = [Dpool D(d)*ones(size(FOfreq))];
end
[FOfreqAll,Uidx] = unique(FOfreqAll);
DpoolAll = DpoolAll(Uidx);

% Further frequency refinement (see Section 4.2)
% Remove (approximately) duplicate frequencies coming from multiple
% detection segments. Give preference to larger detection segments. This
% algorithm uses the grouping established in the leakage filter above.
if isempty(FOfreqPool) == 0
    DD = unique(Dpool);     % Lengths of detection sements with a detection
    for d = 1:length(DD)    % For each of these detection segments
        Idx = find(Dpool == DD(d)); % Indices of FOfreqPool with detections from the current detection segment
        GrpIdx = find(D==DD(d),1);  % Index of the current detection segment in D
        KillIdx = [];               % Keeps track of detected frequencies that should be removed
                                    % to give preference to larger detection segments
        for ii = Idx    % For each of the frequencies detected by the current detection segment
            % This is the group index that the current detected frequency represents (established above)
            GrpFreqIdx = ii-Idx(1)+1;
            
            % Find frequencies from all other detection segments that fall
            % within the range of the group that the current detected
            % frequency represents.
            FreqTemp = FOfreqPool(setdiff(Idx(1):length(Dpool),Idx));   % Having Idx(1) instead of 1 keeps the alg from killing frequencies from big windows because smaller windows have a similar frequency
            FreqTemp = FreqTemp(FreqTemp >= fZP(Group(GrpIdx).IdxZP(1,GrpFreqIdx)));
            FreqTemp = FreqTemp(FreqTemp <= fZP(Group(GrpIdx).IdxZP(2,GrpFreqIdx)));

            % If detected frequencies from larger detection segments are
            % duplicates of this one, remove it.
            if isempty(FreqTemp) == 0
                KillIdx = [KillIdx ii];
            end
        end
        % Remove frequencies and associated values to give preference to
        % larger detection segments.
        FOfreqPool(KillIdx) = [];
        Dpool(KillIdx) = [];
        FOdiffPool(KillIdx) = [];
    end

    % Sort final results by frequency
    [FOfreqPool,SortIdx] = sort(FOfreqPool);
    Dpool = Dpool(SortIdx);
    FOdiffPool = FOdiffPool(SortIdx);
end

end