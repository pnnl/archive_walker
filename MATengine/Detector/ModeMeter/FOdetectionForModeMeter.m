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
    PastDdet = [];
    PastFOfreqRefined = [];
    PastTimeLocRefined = [];
    PastDdetRefined = [];
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
    PastDdet = PastAdditionalOutput.Ddet;
    PastFOfreqRefined = PastAdditionalOutput.FOfreqRefined;
    PastTimeLocRefined = PastAdditionalOutput.TimeLocRefined;
    PastDdetRefined = PastAdditionalOutput.DdetRefined;
end

%% Handle the case where y has NaN values (detection is not run)

if sum(isnan(y)) > 0
    AdditionalOutput.FOfreq = [];
    AdditionalOutput.TimeLoc = [];
    AdditionalOutput.Ddet = [];
    AdditionalOutput.FOfreqRefined = [];
    AdditionalOutput.TimeLocRefined = [];
    AdditionalOutput.DdetRefined = [];
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

[FOfreq,Ddet,FOfreqRefined,DdetRefined] = Detect(y,Pfa,FrequencyTolerance,D,WinStruct,U,PhiV,D0,OmegaB,fs);

if FOlocParam.PerformTimeLoc
    % Run time localization
    TimeLoc = [];
    for fidx = 1:length(FOfreq)
        TimeLocTemp = RunTimeLocalization(y(end-Ddet(fidx)+1:end),FOfreq(fidx),FOlocParam,fs);
        % Store FO location and adjust for difference between localization
        % window and the detection window
        TimeLoc = [TimeLoc; TimeLocTemp + length(y)-Ddet(fidx)];
    end
else
    % Time localization is disabled - assume that FO is present for entire
    % detection window 
    TimeLoc = [length(y)-Ddet'+1 length(y)*ones(length(Ddet),1)];
end

% Get the subset of TimeLoc that corresponds to FOfreqRefined
RefinedIdx = zeros(1,length(FOfreqRefined));
for k = 1:length(FOfreqRefined)
    RefinedIdx(k) = find(FOfreqRefined(k) == FOfreq);
end
TimeLocRefined = TimeLoc(RefinedIdx,:);

% For each of the FOs detected in the previous iteration, check if it is
% still being detected. If not, add it to the list of detected FOs and
% adjust its location in time. 
% This gives the detector "memory" so that FOs detected with shorter
% windows aren't lost as they pass out of the short windows, leading to
% bias.
% If the FO was detected before, update the current frequency estimate
% based on prior information.
[FOfreq,Ddet,TimeLoc] = FOmemory(FOfreq,Ddet,TimeLoc,PastFOfreq,PastTimeLoc,PastDdet,FrequencyTolerance,ResultUpdateInterval);
[FOfreqRefined,DdetRefined,TimeLocRefined] = FOmemory(FOfreqRefined,DdetRefined,TimeLocRefined,PastFOfreqRefined,PastTimeLocRefined,PastDdetRefined,FrequencyTolerance,ResultUpdateInterval);

% For unrefined frequencies: Make sure there are no duplicates.
%
% Start by sorting according to Ddet so that the largest Ddet is retained
% after duplicates are removed.
[Ddet,SortIdx] = sort(Ddet,'descend');
FOfreq = FOfreq(SortIdx);
TimeLoc = TimeLoc(SortIdx,:);
% Keep unique frequencies. Use the 'stable' set order so that the largest
% Ddet are retained.
[FOfreq,uidx] = unique(FOfreq,'stable');
Ddet = Ddet(uidx);
TimeLoc = TimeLoc(uidx,:);
% Finally, sort by frequency
[FOfreq,SortIdx] = sort(FOfreq,'ascend');
Ddet = Ddet(SortIdx);
TimeLoc = TimeLoc(SortIdx,:);

% For refined frequencies:
% Additional check to make sure FOs are far apart in frequency. Combine
% multiples into one by giving preference to frequency estimates from
% longer detection segments, then to estimates associated with longer
% durations. If multiple frequency estimates remain for a group, they are
% averaged.
if ~isempty(FOfreqRefined)
    Loc = [0 find(diff(FOfreqRefined)>FrequencyTolerance) length(FOfreqRefined)];    % Breaks frequency bins with detections into groups separated by at least FrequencyTolerance Hz
    
    Dur = diff(TimeLocRefined,[],2);
    
    FOfreqFinal = zeros(1,length(Loc)-1);    % Final frequency vector
    TimeLocFinal = zeros(length(Loc)-1,2);
    DdetFinal = zeros(1,length(Loc)-1);
    for L = 1:length(Loc)-1             % For each group
        Lidx = Loc(L)+1:Loc(L+1);
        % First based on the length of detection segment
        MaxIdxD = find(max(DdetRefined(Lidx)) == DdetRefined(Lidx));
        % If multiple, then base it on the duration of the FO
        MaxIdxDur = find(max(Dur(Lidx(MaxIdxD))) == Dur(Lidx(MaxIdxD)));
        % Indices (still could be multiple) that should be included
        KeepIdx = Lidx(MaxIdxD(MaxIdxDur));
        % Final frequency is the average of remaining (hopefully just one)
        FOfreqFinal(L) = mean(FOfreqRefined(KeepIdx));
        % Store the detection segment length (all the same, so just need
        % the first)
        DdetFinal(L) = DdetRefined(KeepIdx(1));

        TimeLocFinal(L,1) = min(TimeLocRefined(Lidx,1));
        TimeLocFinal(L,2) = max(TimeLocRefined(Lidx,2));
    end
    FOfreqRefined = FOfreqFinal;
    TimeLocRefined = TimeLocFinal;
    DdetRefined = DdetFinal;
end


AdditionalOutput.FOfreq = FOfreq;
AdditionalOutput.TimeLoc = TimeLoc;
AdditionalOutput.Ddet = Ddet;
AdditionalOutput.FOfreqRefined = FOfreqRefined;
AdditionalOutput.TimeLocRefined = TimeLocRefined;
AdditionalOutput.DdetRefined = DdetRefined;
end

% [FOfreqPool,FOdiffPool,Dpool,FOfreqAll,DpoolAll] = Detect(y,PfaMax,D,WinStruct,U,PhiV,D0,OmegaB,fs)
% 
% This function performs the detection algorithm on the signal contained in
% y. After the initial frequencies are found, they are refined in two ways.
% First, the bins are broken up such that each group is separated from the
% others by a certain tolerance. A single frequency is then
% selected from each group using a frequency estimation method. The second
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

function [FOfreqAll,Dall,FOfreqRefined,Drefined] = Detect(y,PfaMax,FrequencyTolerance,D,WinStruct,U,PhiV,D0,OmegaB,fs)

% Result vectors
FOfreqRefined = [];    % Frequencies of detected FOs
Drefined = [];         % Detection segment lengths responsible for each detected frequency
GroupRangeRefined = [];
FOfreqAll = [];
Dall = [];

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
    DetIdx = find(PhiYhat(OmegaBseg) > gam);
    OmegaFO = OmegaBseg(DetIdx);    % FO detected when periodogram exceeds threshold
    Evidence = PhiYhat(OmegaBseg(DetIdx)) - gam(DetIdx);
    
    f = fs*(0:D0(d)-1)'/D0(d);              % Frequency vector accompanying periodogram   
    
    FOfreq = zeros(1,length(OmegaFO));
    s = [y(end-D(d)+1:end); zeros(D0(d)-D(d)+1,1)];
    for k = 1:length(OmegaFO)
        mhat = OmegaFO(k);
        FOfreq(k) = EstimateFrequency(s,mhat,fs);
    end
    FOfreqAll = [FOfreqAll FOfreq];
    Dall = [Dall D(d)*ones(size(FOfreq))];

    % Refine frequency estimates (leakage filter and iterative estimation)
    if isempty(OmegaFO) == 0    % Only continue if an FO was detected
        Loc = [0 find(diff(f(OmegaFO)')>FrequencyTolerance) length(OmegaFO)];    % Breaks frequency bins with detections 
                                                            % into groups separated by at least FrequencyTolerance Hz
                                                            
        FOfreqRefTemp = zeros(1,length(Loc)-1);    % Final frequency vector
        GroupRange = zeros(2,length(Loc)-1);
        for L = 1:length(Loc)-1             % For each group
            Lidx = Loc(L)+1:Loc(L+1);
            % Find index that provides most evidence of a FO
            [~,mhat] = max(Evidence(Lidx));
            FOfreqRefTemp(L) = FOfreq(Lidx(mhat));
            
            GroupRange(1,L) = f(OmegaFO(Lidx(1))) - fs/D0(d);
            GroupRange(2,L) = f(OmegaFO(Lidx(end))) + fs/D0(d);
        end
    else
        FOfreqRefTemp = [];
        GroupRange = [];
    end
    
    FOfreqRefined = [FOfreqRefined FOfreqRefTemp];
    Drefined = [Drefined D(d)*ones(size(FOfreqRefTemp))];
    GroupRangeRefined = [GroupRangeRefined GroupRange];
end

% Remove duplicates from the list of unrefined frequency estimates.
%
% Start by sorting according to Dall so that the largest Dall is retained
% after duplicates are removed.
[Dall,SortIdx] = sort(Dall,'descend');
FOfreqAll = FOfreqAll(SortIdx);
% Keep unique frequencies. Use the 'stable' set order so that the largest
% Dall are retained.
[FOfreqAll,uidx] = unique(FOfreqAll,'stable');
Dall = Dall(uidx);

% Further frequency refinement (see Section 4.2)
% Remove (approximately) duplicate frequencies coming from multiple
% detection segments. Give preference to larger detection segments. This
% algorithm uses the grouping established in the leakage filter above.
if isempty(FOfreqRefined) == 0
    DD = unique(Drefined);     % Lengths of detection sements with a detection
    for d = 1:length(DD)    % For each of these detection segments
        Idx = find(Drefined == DD(d)); % Indices of FOfreqPool with detections from the current detection segment
        KillIdx = [];               % Keeps track of detected frequencies that should be removed
                                    % to give preference to larger detection segments
        for ii = Idx    % For each of the frequencies detected by the current detection segment
            % Find frequencies from all other detection segments that fall
            % within the range of the group that the current detected
            % frequency represents.
            GrpIdx = setdiff(Idx(1):length(Drefined),Idx);
            FreqTemp = FOfreqRefined(GrpIdx);   % Having Idx(1) instead of 1 keeps the alg from killing frequencies from big windows because smaller windows have a similar frequency
            RangeTemp = GroupRangeRefined(:,GrpIdx);
            FreqTemp = FreqTemp(FreqTemp >= min(RangeTemp(1,:)));
            FreqTemp = FreqTemp(FreqTemp <= max(RangeTemp(2,:)));

            % If detected frequencies from larger detection segments are
            % duplicates of this one, remove it.
            if ~isempty(FreqTemp)
                KillIdx = [KillIdx ii];
            end
        end
        % Remove frequencies and associated values to give preference to
        % larger detection segments.
        FOfreqRefined(KillIdx) = [];
        Drefined(KillIdx) = [];
        GroupRangeRefined(:,KillIdx) = [];
    end

    % Sort final results by frequency
    [FOfreqRefined,SortIdx] = sort(FOfreqRefined);
    Drefined = Drefined(SortIdx);
end

end


% This function estimates the iterative frequency estimation algorithm from
% "Iterative Frequency Estimation by Interpolation on Fourier
% Coefficients". This is the paper that Luke Dosiek referenced in "The 
% Effects of Forced Oscillation Frequency Estimation Error on the LS-ARMA+S
% Mode Meter".
function fhat = EstimateFrequency(s,mhat,fs)

N = length(s);

d0 = 0;
k = 0:N-1;
for q = 1:3
    p = 0.5;
    Xp = s.' * exp(-1i*2*pi*k*(mhat + d0 + p)/N).';
    p = -0.5;
    Xn = s.' * exp(-1i*2*pi*k*(mhat + d0 + p)/N).';
    h = 1/2*real((Xp + Xn)/(Xp - Xn));
%     h = 1/2*(abs(Xp) - abs(Xn))/(abs(Xp) + abs(Xn));
    d0 = d0 + h;
end

fhat = (mhat + d0)/N*fs;

end




% For each of the FOs detected in the previous iteration, check if it is
% still being detected. If not, add it to the list of detected FOs and
% adjust its location in time. 
% This gives the detector "memory" so that FOs detected with shorter
% windows aren't lost as they pass out of the short windows, leading to
% bias.
% If the FO was detected before, update the current frequency estimate
% based on prior information.
function [FOfreq,Ddet,TimeLoc] = FOmemory(FOfreq,Ddet,TimeLoc,PastFOfreq,PastTimeLoc,PastDdet,FrequencyTolerance,ResultUpdateInterval);

FOadd = [];
LOCadd = [];
DdetAdd = [];
for fidx = 1:length(PastFOfreq)
    if (min(abs(PastFOfreq(fidx) - FOfreq)) > FrequencyTolerance)
        TooFar = true;
    else
        TooFar = false;
    end
    if TooFar || isempty(FOfreq)
        % Add FO detected previously
        
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
        DdetAdd = [DdetAdd PastDdet(fidx)];
    elseif ~TooFar
        % Modify start time based on previous run

        % Update the FO location based on the estimate from the
        % previous iteration
        LOCtemp = PastTimeLoc(fidx,:)-ResultUpdateInterval;
        if LOCtemp(2) < 1
            continue
        end
        if LOCtemp(1) < 1
            LOCtemp(1) = 1;
        end

        CurrIdx = near(FOfreq,PastFOfreq(fidx));
        if isempty(CurrIdx)
            continue;
        end
        TimeLoc(CurrIdx,1) = min([TimeLoc(CurrIdx,1) LOCtemp(1)]);
        
        % If the past detection segment was longer than the current one,
        % use the frequency estimate from that one
        if PastDdet(fidx) > Ddet(CurrIdx)
            FOfreq(CurrIdx) = PastFOfreq(fidx);
        end
    end
end
FOfreq = [FOfreq FOadd];
TimeLoc = [TimeLoc; LOCadd];
Ddet = [Ddet DdetAdd];

[FOfreq,SortIdx] = sort(FOfreq,'ascend');
TimeLoc = TimeLoc(SortIdx,:);
Ddet = Ddet(SortIdx);

end