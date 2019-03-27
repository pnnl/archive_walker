% Created by Urmila Agrawal


function Freq_FO_Refined = FOdetectionForModeMeter(y,Parameters,fs)

%Extract parameters from the structure
WindowType = Parameters.WindowType;
ZeroPadding = Parameters.ZeroPadding;
WindowLength = Parameters.WindowLength;
WindowOverlap = Parameters.WindowOverlap;
MedianFilterOrder = Parameters.MedianFilterOrder;
Pfa = Parameters.Pfa;
FrequencyMin = Parameters.FrequencyMin;
FrequencyMax = Parameters.FrequencyMax;
FrequencyTolerance = Parameters.FrequencyTolerance;
% Window for test statistic periodogram
PeriodogramWindow = eval([WindowType '(length(y))']);

% Window for the GMSC and Daniell-Welch periodogram (PSD estimate)
GMSCandPSDwindow = eval([WindowType '(WindowLength)']);

%% Perform detection

% Frequency of interest
freqAll = fs*(0:ZeroPadding-1)/ZeroPadding;
OmegaB = find(freqAll>FrequencyMin & freqAll <FrequencyMax); %Frequency bins of interest;
FreqInterest = freqAll(OmegaB); % Frequency of interest
LengthFreqInterest = length(FreqInterest); %Number of frequency bins of interest

%calculates power spectrum of selected signals
SignalPSD = CalcPSD_OmegaB(y, ZeroPadding, 0, PeriodogramWindow, [], fs,OmegaB);
%estimates power spectrum of ambient noise using power spectrum of selected signal
AmbientNoisePSD = CalcPSD_OmegaB(y, ZeroPadding, WindowOverlap, GMSCandPSDwindow,MedianFilterOrder,fs,OmegaB);
%analyses signal for single channel mode
%gives test statistic for detecting FO
TestStatistic = SignalPSD;%./AmbientNoisePSD(OmegaB,ChannelIdx);
%gives threshold for detecting FO
Threshold = -AmbientNoisePSD*log(Pfa/LengthFreqInterest);

% % gives estimates of frequency and amplitude of forced oscillations
% FOfreq = DetectFO(TestStatistic, Threshold, SignalPSD, AmbientNoisePSD, FrequencyTolerance, PeriodogramWindow, FreqInterest);

%finds indices of frequency for which TestStatistic exceeds Threshold
Freq_FOind = find(TestStatistic > Threshold);
%Gives frequency for which TestStatistic exceeds Threshold
Freq_FO = FreqInterest(Freq_FOind);

% if TestStatistic does not exceed Threshold for any frequency, then
% rerurns NaN value as estimates
if isempty(Freq_FO)
    Frequency_est = NaN;
    Freq_FO_Refined = NaN;
    return;
else    
    % Breaks frequency bins with detections into groups separated by at least tol Hz
    Loc = [0, find(diff(Freq_FO) > FrequencyTolerance), length(Freq_FO)];

    Frequency_est = zeros(1,length(Loc)-1); % Refined frequency vector
    Freq_FO_Refined = zeros(1,length(Loc)-1); % indices of the refined frequencies

    for L = 1:(length(Loc)-1) % For each group
        Lidx = (Loc(L)+1):Loc(L+1);
        MaxIdx = find(TestStatistic(Freq_FOind(Lidx)) == max(TestStatistic(Freq_FOind(Lidx))));
        Frequency_est(L) = Freq_FO(Lidx(MaxIdx));
        Freq_FO_Refined(L) = Freq_FOind(Lidx(MaxIdx));  % Indices of f that correspond to refined frequency estimates
    end
    
end

