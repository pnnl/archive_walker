function [DetectionResults, AdditionalOutput] = RingdownDetector(PMUstruct,Parameters)

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the periodogram detector could not be used.');
    DetectionResults = struct([]);
    AdditionalOutput = struct([]);
    return
end

% Using the outputs from ExtractData(), make sure that NaN values are
% handled appropriately and that signal types and units are appropriate.
% This comment is general for all detectors, write code specific to the
% detector you're working on.


%% Get detector parameters

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. 
% Additional inputs, such as the length of the input data or the sampling 
% rate, can be added as necessary. 
ExtractedParameters = ExtractParameters(Parameters,size(Data,1),fs);

% Store the parameters in variables for easier access
Mode = ExtractedParameters.Mode;
SegmentLength = ExtractedParameters.SegmentLength;
SegmentDelay = ExtractedParameters.SegmentDelay;
SegmentNumber = ExtractedParameters.SegmentNumber;
EnergyThresholdScale = ExtractedParameters.EnergyThresholdScale;
AnalysisLength = ExtractedParameters.AnalysisLength;
Delay = ExtractedParameters.Delay;
NumberDelays = ExtractedParameters.NumberDelays;
ThresholdScale = ExtractedParameters.ThresholdScale;
WindowType = ExtractedParameters.WindowType;
ZeroPadding = ExtractedParameters.ZeroPadding;
WindowLength = ExtractedParameters.WindowLength;
WindowOverlap = ExtractedParameters.WindowOverlap;
FrequencyMin = ExtractedParameters.FrequencyMin;
FrequencyMax = ExtractedParameters.FrequencyMax;
FrequencyTolerance = ExtractedParameters.FrequencyTolerance;


%% Based on the specified parameters, initialize useful variables

% should be SegmentLength + ( SegmentNumber - 1 ) * SegmentDelay
if SegmentLength+SegmentNumber*SegmentDelay > size(Data,1)
    throw(MException('','SegmentLength+SegmentNumber*SegmentDelay must not exceed the length of the input signal'));
end

if SegmentLength+SegmentNumber*SegmentDelay ~= AnalysisLength+NumberDelays*Delay
    warning('SegmentLength+SegmentNumber*SegmentDelay should equal AnalysisLength+NumberDelays*Delay');
end

%% Perform detection

% Initialize structure to output detection results

% calculate energy for all segments in all signals
for segment = 1:SegmentNumber
    currentSegment = Data(SegmentDelay*(segment - 1)+1:SegmentDelay*(segment - 1) + SegmentLength,:);
    yHat = mean(currentSegment);
    allEnergyChannels(segment,:) = sum((currentSegment-ones(size(currentSegment,1),1)*yHat).^2,1);
end


% A = allEnergyChannels > threshold;
% segmentDistance = SegmentLength / SegmentDelay;

% single channel
if strcmp(Mode,'SingleChannel')
    DetectionResults = struct('PMU',[],'Channel',[],'RingStart',[],'RingEnd',[],'EnergyChannel',[]);
    numberOfRingDownSignals = 0;
    % loop through each signal
    for index = 1:size(allEnergyChannels,2)
        currentSignal = allEnergyChannels(:,index);
        % ?_e=e×median{E_m[n]}
        threshold = EnergyThresholdScale * median(currentSignal);
        % find all peaks K samples apart and larger than threshold
        [peaks, locations] = findpeaks(currentSignal, 'MinPeakDistance',SegmentLength/SegmentDelay,'MinPeakHeight',threshold);
        RingStart = [];
        RingEnd = [];
        % find ringstart and ringend for all peaks
        for peakIndex = 1:length(peaks)
            startIndex = locations(peakIndex)*SegmentDelay+1;
            endIndex = locations(peakIndex)*SegmentDelay+SegmentLength;
            RingStart = [RingStart, t(startIndex)];
            RingEnd = [RingEnd, t(endIndex)];
        end
        % add signal to detected results
        if ~isempty(peaks)
            numberOfRingDownSignals = numberOfRingDownSignals + 1;
            DetectionResults(numberOfRingDownSignals).PMU = DataPMU(index);
            DetectionResults(numberOfRingDownSignals).Channel = DataChannel(index);
            DetectionResults(numberOfRingDownSignals).RingStart = RingStart;
            DetectionResults(numberOfRingDownSignals).RingEnd = RingEnd;
            DetectionResults(numberOfRingDownSignals).EnergyChannel = peaks;
        end
    end
% multiple channels
else
    DetectionResults = struct('PMU',[],'Channel',[],'RingStart',[],'RingEnd',[],'EnergyChannel',[],'EnergyTotal',[]);
    % E[n]
    totalEnergyAcrossChannels = sum(allEnergyChannels,2);
    % ?_e=e×median{E[n]}
    threshold = EnergyThresholdScale * median(totalEnergyAcrossChannels);
    % find all peaks K samples apart and larger than threshold
    [peaks, locations] = findpeaks(totalEnergyAcrossChannels, 'MinPeakDistance',SegmentLength/SegmentDelay,'MinPeakHeight',threshold);
    RingStart = [];
    RingEnd = [];
    EnergyChannel = [];
    % find ringstart and ringend for all peaks
    % find all peak energy for each channel
    for peakIndex = 1:length(peaks)
        startIndex = locations(peakIndex)*SegmentDelay+1;
        endIndex = locations(peakIndex)*SegmentDelay+SegmentLength;
        RingStart = [RingStart, t(startIndex)];
        RingEnd = [RingEnd, t(endIndex)];
        EnergyChannel = [EnergyChannel; allEnergyChannels(locations,:)];
    end
    % add signal to detected results
    if ~isempty(peaks)
        DetectionResults.PMU = DataPMU;
        DetectionResults.Channel = DataChannel;
        DetectionResults.RingStart = RingStart;
        DetectionResults.RingEnd = RingEnd;
        DetectionResults.EnergyChannel = EnergyChannel;
        DetectionResults.EnergyTotal = sum(EnergyChannel,2);
    end    
end
% Initialize structure for additional outputs
AdditionalOutput = struct([]);

end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters,SignalLength,fs)
% Mode of operation - 'SingleChannel' or 'MultiChannel'
if isfield(Parameters,'Mode')
    % Use specified mode
    Mode = Parameters.Mode;
else
    % Use default mode: single channel
    Mode = 'SingleChannel';
end

% Length of segments in samples for the energy detector
if isfield(Parameters,'SegmentLength')
    % Use specified segment length
    SegmentLength = str2double(Parameters.SegmentLength);
else
    % is there any default value?
    throw(MException('','User input for segment length required!'));
end

% Delay between evaluated segments in samples. This can be thought of as the time-resolution of the detector.
if isfield(Parameters,'SegmentDelay')
    % Use specified segment delay
    SegmentDelay = str2double(Parameters.SegmentDelay);
else
    % is there any default value?
    throw(MException('','User input for segment dalay required!'));
end

% Number of segments to evaluate. SegmentLength+SegmentNumber*SegmentDelay must not exceed the length of the input signal
if isfield(Parameters,'SegmentNumber')
    % Use specified segment number
    SegmentNumber = str2double(Parameters.SegmentNumber);
else
    % is there any default value?
    throw(MException('','User input  for segment number required'));
end

% Scaling factor to establish the detection threshold
if isfield(Parameters,'EnergyThresholdScale')
    % Use specified value
    EnergyThresholdScale = str2double(Parameters.EnergyThresholdScale);
else
    % use default
    throw(MException('','User input for energy threshold scaling factor is required.'));
end

% Number of samples to use in the analysis
if isfield(Parameters,'AnalysisLength')
    % Use specified value
    AnalysisLength = str2double(Parameters.AnalysisLength);
else
    % Use default value (length of the input signals)
    AnalysisLength = SignalLength;
end

% The delay in samples used to calculate the self-GMSC. If omitted, the default is floor(AnalysisLength/10)
if isfield(Parameters,'Delay')
    % Use specified value
    Delay = str2double(Parameters.Delay);
else
    % Use default, floor(AnalysisLength/10)
    Delay = floor(AnalysisLength/10);
end

% Number of delays in the self-GMSC. Must be an integer greater than or equal to 2. If omitted, default is 2
if isfield(Parameters,'NumberDelays')
    % Use specified value
    NumberDelays = str2double(Parameters.Delay);
else
    % Use default value 2.
    NumberDelays = 2;
end

% Scaling factor to establish the detection threshold. If omitted, default is 3
if isfield(Parameters,'ThresholdScale')
    % Use specified value
    ThresholdScale = Parameters.ThresholdScale;
else
    % Use default value, 3.
    ThresholdScale = 3.
end

% Type of window used for the test statistic periodogram, Daniell-Welch 
% periodogram, and GMSC. Options are rectwin, bartlett, hann, hamming, and 
% blackman. If omitted, default is hann.
if isfield(Parameters,'WindowType')
    % Use specified window
    WindowType = Parameters.WindowType;
else
    % Use default window
    WindowType = 'hann';
end

% Zero padded length of the test statistic periodogram, Daniell-Welch 
% periodogram, and GMSC. If omitted, no zero padding is implemented.
if isfield(Parameters,'ZeroPadding')
    % Use specified zero padding
    ZeroPadding = str2double(Parameters.ZeroPadding);
else
    % Use default zero padding (none)
    ZeroPadding = AnalysisLength;
end

% Length of the sections for the Daniell-Welch periodogram and GMSC. If 
% omitted, default is 1/5 of K (AnalyisLength).
if isfield(Parameters,'WindowLength')
    % Use specified window length
    WindowLength = str2double(Parameters.WindowLength);
else
    % Use default window length, default is 1/5 of AnalyisLength
    WindowLength = floor(AnalysisLength/5);
end

% Amount of overlap between sections for the Daniell-Welch periodogram and 
% GMSC. If omitted, default is half of WindowLength
if isfield(Parameters,'WindowOverlap')
    % Use specified window overlap
    WindowOverlap = str2double(Parameters.WindowOverlap);
else
    % Use default window overlap
    WindowOverlap = floor(WindowLength/2);
end

% Minimum frequency to be considered. If omitted, the default is zero, but 
% in many cases this will cause excessive false alarms.
if isfield(Parameters,'FrequencyMin')
    % Use specified minimum frequency
    FrequencyMin = str2num(Parameters.FrequencyMin);
else
    % Use default minimum frequency
    FrequencyMin = 0;
end

% Maximum frequency to be considered. If omitted, the default is the 
% Nyquist frequency
if isfield(Parameters,'FrequencyMax')
    % Use specified maximum frequency
    FrequencyMax = str2num(Parameters.FrequencyMax);
else
    % Use default maximum frequency
    FrequencyMax = fs/2;
end

% Tolerance used to refine the frequency estimate. If omitted, the default 
% is the greater of 1) the main lobe width of the test statistic 
% periodogram's window and 2) half of FrequencyMin.
if isfield(Parameters,'FrequencyTolerance')
    % Use specified frequency tolerance
    FrequencyTolerance = str2num(Parameters.FrequencyTolerance);
else
    % Use default frequency tolerance
    % First term in max() is the minimum frequency divided by 2. The second
    % term is the sampling rate times the main lobe width in radians per
    % sample. The result is the main lobe width in Hz. The full expression
    % is included in the comments, but a reduced expression is used to
    % avoid numerical errors introduced by multiplying and dividing by pi.
    switch WindowType
        case 'rectwin'
            FrequencyTolerance = max([FrequencyMin/2, (2*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 4*pi/WindowLength / (2*pi))]);
        case 'bartlett'
            FrequencyTolerance = max([FrequencyMin/2, (4*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 8*pi/WindowLength / (2*pi))]);
        case 'hann'
            FrequencyTolerance = max([FrequencyMin/2, (4*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 8*pi/WindowLength / (2*pi))]);
        case 'hamming'
            FrequencyTolerance = max([FrequencyMin/2, (4*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 8*pi/WindowLength / (2*pi))]);
        case 'blackman'
            FrequencyTolerance = max([FrequencyMin/2, (6*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 12*pi/WindowLength / (2*pi))]);
    end
end

ExtractedParameters = struct('Mode',Mode,'SegmentLength',SegmentLength,...
    'SegmentDelay',SegmentDelay, 'SegmentNumber',SegmentNumber,...
    'EnergyThresholdScale',EnergyThresholdScale,'AnalysisLength',AnalysisLength,...
    'Delay',Delay,'NumberDelays',NumberDelays,'ThresholdScale',ThresholdScale,...
    'WindowType',WindowType,'ZeroPadding',ZeroPadding,...
    'WindowLength',WindowLength,'WindowOverlap',WindowOverlap,...
    'FrequencyMin',FrequencyMin,'FrequencyMax',FrequencyMax,...
    'FrequencyTolerance',FrequencyTolerance);

end