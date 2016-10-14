%           PastAdditionalOutput: past AdditionalOutput outputs from this
%                                 function that can be used for various
%                                 purposes.

function [DetectionResults, AdditionalOutput] = RingdownDetector(PMUstruct,Parameters,PastAdditionalOutput)

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the periodogram detector could not be used.');
    DetectionResults = struct([]);
    AdditionalOutput = struct([]);
    return
end

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

RMSlength = ExtractedParameters.RMSlength;
ForgetFactor = ExtractedParameters.ForgetFactor;
RingThresholdScale = ExtractedParameters.RingThresholdScale;
MaxDuration = ExtractedParameters.MaxDuration;

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

N = RMSlength*fs;

%% New RMS energy based detector

% NaN values in a channel are unacceptable. If a channel has any NaN
% values, it is discarded from analysis.
DiscardChannelIdx = find(sum(isnan(Data),1) > 0);

% Only active power, frequency, and OTHER signal types are acceptable. If a
% channel has any other type, it is discarded from analysis.
DiscardChannelIdx = sort([DiscardChannelIdx find((~strcmp(DataType,'P')) & (~strcmp(DataType,'F')) & (~strcmp(DataType,'OTHER')))]);

Data2 = Data.^2;

DetectionResults = struct('PMU',[],'Channel',[],'RingStart',[],'RingEnd',[]);
AdditionalOutput = struct('threshold',[],'RMS',[],'TimePoints',[],'NextThreshold',[],'FilterConditions',[]);
% loop through each signal
for index = 1:size(Data2,2)
    % Store the PMU and channel name
    DetectionResults(index).PMU = DataPMU(index);
    DetectionResults(index).Channel = DataChannel(index);

    % Check if channel is to be included
    if ismember(index,DiscardChannelIdx)
        % This channel is not to be included - set to NaN
        DetectionResults(index).RingStart = {NaN};
        DetectionResults(index).RingEnd = {NaN};
    else
        % This channel is okay to be included
        if isempty(PastAdditionalOutput)
            % PastAdditionalOutput isn't available
            InitConditions = [];
        else
            InitConditions = PastAdditionalOutput(index).FilterConditions;
        end
        
        [RMS, AdditionalOutput(index).FilterConditions] = filter(ones(1,N)/N,1,Data2(:,index),InitConditions);
        RMS = sqrt(RMS);
        
        if isempty(PastAdditionalOutput)
            % PastAdditionalOutput isn't available
            ThisThreshold = RingThresholdScale*median(RMS);
        else
            if isempty(PastAdditionalOutput(index).NextThreshold)
                % This specific next threshold isn't available
                ThisThreshold = RingThresholdScale*median(RMS);
            else
                ThisThreshold = PastAdditionalOutput(index).NextThreshold;
            end
        end
        
        DetectionIdx = RMS > ones(size(Data2,1),1)*ThisThreshold;
        
        RingStart = {};
        RingEnd = {};
        DataRing = {};
        if sum(DetectionIdx) > 0
            % Ringdown was detected
            
            % Start and end of ringdown. Probably want to adjust this by
            % accounting for the filter delay
            Starts = find(diff([0; DetectionIdx]) == 1);
            Ends = find(diff(DetectionIdx) == -1);
            
            if length(Starts) > length(Ends)
                Ends = [Ends; length(DetectionIdx)];
            end
            
            % Adjust for filter delay, but don't let them be less than 0
            Starts = Starts - floor((N-1)/2);
            Ends = Ends - floor((N-1)/2);
            Starts(Starts<1) = 1;
            Ends(Ends<1) = 1;
            
            % Remove any detections where the duration is longer than the
            % maximum. This helps make the detector specific to ringdowns
            Starts(Ends-Starts+1 > MaxDuration*fs) = [];
            
            for RingIdx = 1:length(Starts)
                RingStart{RingIdx} = TimeString{Starts(RingIdx)};
                RingEnd{RingIdx} = TimeString{Ends(RingIdx)};
                DataRing{RingIdx} = Data(Starts(RingIdx):Ends(RingIdx),index);
            end
            
            % Ringdown was detected - do not update threshold
            AdditionalOutput(index).NextThreshold = ThisThreshold;
        else
            % Ringdown was not detected - update threshold
            AdditionalOutput(index).NextThreshold = ThisThreshold*ForgetFactor + RingThresholdScale*median(RMS)*(1-ForgetFactor);
        end
        
        % add signal to detected results
        DetectionResults(index).RingStart = RingStart;
        DetectionResults(index).RingEnd = RingEnd;

        AdditionalOutput(index).threshold = ThisThreshold;
        AdditionalOutput(index).RMS = RMS;
        AdditionalOutput(index).TimePoints = TimeString;
        AdditionalOutput(index).DataRing = DataRing;
    end
end


end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters,SignalLength,fs)
% Maximum duration
if isfield(Parameters,'MaxDuration')
    % Use specified maximum duration
    MaxDuration = str2double(Parameters.MaxDuration);
else
    % Use default maximum duration
    MaxDuration = 90;
end

% Length for RMS calculation
if isfield(Parameters,'RMSlength')
    % Use specified length
    RMSlength = str2double(Parameters.RMSlength);
else
    % Use default length
    RMSlength = 15;
end

% Forgetting factor for threshold
if isfield(Parameters,'ForgetFactor')
    % Use specified forgetting factor
    ForgetFactor = str2double(Parameters.ForgetFactor);
else
    % Use default forgetting factor
    ForgetFactor = 0.9;
end

% Scaling term for threshold
if isfield(Parameters,'RingThresholdScale')
    % Use specified scaling term
    RingThresholdScale = str2double(Parameters.RingThresholdScale);
else
    % Use default scaling term
    RingThresholdScale = 3;
end

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
    ThresholdScale = str2double(Parameters.ThresholdScale);
else
    % Use default value, 3.
    ThresholdScale = 3;
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
    'FrequencyTolerance',FrequencyTolerance,'RingThresholdScale',RingThresholdScale,...
    'RMSlength',RMSlength,'ForgetFactor',ForgetFactor,'MaxDuration',MaxDuration);

end