%   [DetectionResults, AdditionalOutput] = SpectralCoherenceDetector(PMUstruct,Parameters)
%   This function implemements spectral coherence method for forced oscillation detection 
%   Inputs:
%           PMUstruct: PMU structure in a common format for all PMUs
%           Parameters: User specified values for carrying out detection.
%           If any field is empty, uses default values
%               Mode = Mode of detection, either singlechannel or
%               multi-channel
%               AnalysisLength = Number of samples for signal
%               analysis
%               WindowType = Type of window used for GMSC calculation
%               ZeroPadding = Zero padded length of GMSC
%               WindowLength = Length of the sections for the GMSC
%               WindowOverlap = Amount of overlap between sections
%               for the GMSC
%               FrequencyMin = Minimum frequency to be considered
%               FrequencyMax = Maximum frequency to be considered
%               FrequencyTolerance = Tolerance used to refine the
%               frequency estimate
%               Delay = delay in samples used to calculate the self-GMSC
%               NumberDelays =  Number of delays in the self-GMSC
%               ThresholdScale = Scaling factor to establish the detection threshold
%           PastAdditionalOutput: past AdditionalOutput outputs from this
%                                 function that can be used for various
%                                 purposes.
%
%   Outputs:
%           DetectionResults: Struct Array containing information on PMU and Channel of
%           signal used for analysis along with estimates of frequency and
%           amplitude of FO 
%                   (MultiChannel Case: Dimension 1 by 1)
%                   (SingleChannel Case: Dimension 1 by number of channels)
%           AdditionalOutput: Contains estimates of power spectrum of
%           signal, ambient noise spectrum estimates, test statistics, and
%           threshold 
%                   (MultiChannel Case: Dimension 1 by 1)
%                   (SingleChannel Case: Dimension 1 by number of channels)
%
% Created by Jim Follum (james.follum@pnnl.gov)
% Modified by: Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/20/2016
% wrote code under section: i. Get detector parameters ii. Frequency of
% interest iii. Check Data quality and Signal type iv. Initialization v.
% Perform detection

function [DetectionResults, AdditionalOutput] = SpectralCoherenceDetector(PMUstruct,Parameters,PastAdditionalOutput)

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the spectral coherence detector could not be used.');
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
ExtractedParameters = ExtractParameters(Parameters,fs);

% Store the parameters in variables for easier access
Mode = ExtractedParameters.Mode;
AnalysisLength = ExtractedParameters.AnalysisLength;
Delay = ExtractedParameters.Delay;
NumberDelays = ExtractedParameters.NumberDelays;
ThresholdScale = ExtractedParameters.ThresholdScale;
WindowType = ExtractedParameters.WindowType;
ZeroPadding = ExtractedParameters.ZeroPadding;
WindowLength =ExtractedParameters.WindowLength;
WindowOverlap = ExtractedParameters.WindowOverlap;
FrequencyMin = ExtractedParameters.FrequencyMin;
FrequencyMax = ExtractedParameters.FrequencyMax;
FrequencyTolerance = ExtractedParameters.FrequencyTolerance;
% WindowLength = floor((AnalysisLength)/8);
% WindowOverlap = floor(WindowLength/2);

Data = Data(end-(AnalysisLength+Delay*(NumberDelays-1))+1:end,:);
%% Based on the specified parameters, initialize useful variables

Window = eval([WindowType '(WindowLength)']);
% Window = hann(WindowLength);

%% Frequency of interest
freqAll = fs*(0:ZeroPadding-1)/ZeroPadding;
OmegaB = find(freqAll>FrequencyMin & freqAll <FrequencyMax); %Frequency bins of interest;
FreqInterest = freqAll(OmegaB); % Frequency of interest
LengthFreqInterest = length(FreqInterest); %Number of frequency bins of interest

%% Check Data quality and Signal type

%Removes data channel containing even one NaN value
%Signals of type Active Power, Reactive Power, Apparent Power, Frequency
%and Others are selected for analysis purpose
[SelectedData, SelectedDataTypeInd] = CheckDataTypeQuality(Data, DataType);

%% Initialization

% Initialize structure to output detection results
if strcmp(Mode,'SingleChannel')
    DetectionResults = struct('PMU',[],'Channel',[],'Frequency',[],'Coherence',[]);
    AdditionalOutput = struct('SignalCoherenceSpectrum',[],'Threshold',[],'Frequency',[],'Mode',[],'fs',fs,'Start',TimeString{end-size(Data,1)+1},'End',TimeString{end});
else
    DetectionResults = struct('PMU',[],'Channel',[],'Frequency',[],'Coherence',[],'TestStatistic',[]);
    AdditionalOutput = struct('SignalCoherenceSpectrum',[],'TestStatistic',[],'Threshold',[],'Frequency',[],'Mode',[],'fs',fs,'Start',TimeString{end-size(Data,1)+1},'End',TimeString{end});
end

% Initialize structure for additional outputs


%all appropriate fields are assigned NaN value so that NaN value is returned if none of the signal is selected for signal analysis
if strcmp(Mode,'MultiChannel')
    DetectionResults.PMU = DataPMU;
    DetectionResults.Channel = DataChannel;
    DetectionResults.Frequency = NaN;
    DetectionResults.Coherence = NaN*ones(1,length(DataChannel));
    DetectionResults.TestStatistic = NaN;
    AdditionalOutput.Threshold = NaN;
    AdditionalOutput.SignalCoherenceSpectrum = NaN*ones(LengthFreqInterest,length(DataChannel));
    AdditionalOutput.TestStatistic = NaN*ones(LengthFreqInterest,1);
    AdditionalOutput.Frequency = FreqInterest;
    AdditionalOutput.Mode = Mode;    
else
    for ChannelIdx = 1:length(DataChannel)
        DetectionResults(ChannelIdx).PMU = DataPMU(ChannelIdx);
        DetectionResults(ChannelIdx).Channel = DataChannel(ChannelIdx);
        DetectionResults(ChannelIdx).Frequency = NaN;
        DetectionResults(ChannelIdx).Coherence = NaN; 
        AdditionalOutput(ChannelIdx).Threshold = NaN;
        AdditionalOutput(ChannelIdx).SignalCoherenceSpectrum = NaN*ones(LengthFreqInterest,1);  
        AdditionalOutput(ChannelIdx).Frequency = FreqInterest;
        AdditionalOutput(ChannelIdx).Mode = Mode;    
    end
end
if isempty(SelectedDataTypeInd)
    % returns if none of the singals are apprpriate for further analysis
    return;
end
for ChannelIdx = 1:length(SelectedDataTypeInd)
    SelectedData(:,ChannelIdx) = SelectedData(:,ChannelIdx) - mean(SelectedData(:,ChannelIdx));
end

%% Perform detection

%gives estimates of self-generalized magnitude squared coherence for each
%signal
[GMSC_est, FlagIndicator]= calcSelfGMSC(SelectedData, ZeroPadding, WindowOverlap, Window, OmegaB, Delay, NumberDelays,AnalysisLength);

if FlagIndicator
    warning('Not enough data points.');
    return;
end

if strcmp(Mode,'MultiChannel')
   
    %gives test statistic for detecting FO
    TestStatistic = mean(GMSC_est,2);  % sum of the PSDs calculated for all channels    
    %gives threshold for detecting FO
    Threshold = ThresholdScale*median(TestStatistic);
    % gives estimates of frequency and amplitude of forced oscillations
    [FrequencyEst, FrequencyInd] = DetectFO_SC(TestStatistic, Threshold, FrequencyTolerance, FreqInterest);
    %assignes calculated value to struct array containing detection results
    DetectionResults.Frequency = FrequencyEst(:);
    DetectionResults.Coherence = NaN*ones(length(FrequencyEst),length(DataChannel));
    DetectionResults.TestStatistic = NaN*ones(length(FrequencyEst),1);
    
    if ~isempty(FrequencyInd)
        DetectionResults.TestStatistic = TestStatistic(FrequencyInd);
        DetectionResults.Coherence(:,SelectedDataTypeInd) = GMSC_est(FrequencyInd,:);
    end
    AdditionalOutput.Threshold = Threshold;
    AdditionalOutput.SignalCoherenceSpectrum(:,SelectedDataTypeInd) = GMSC_est;
    AdditionalOutput.TestStatistic = TestStatistic;
    AdditionalOutput.Data = SelectedData; 
else %analyses signal for single channel mode
    for ChannelIdx = 1:length(SelectedDataTypeInd)
        %gives test statistic for detecting FO
        TestStatistic = GMSC_est(:,ChannelIdx);
        %gives threshold for detecting FO
        Threshold = ThresholdScale*median(TestStatistic);
        % gives estimates of frequency and amplitude of forced oscillations
        [FrequencyEst, FrequencyInd] = DetectFO_SC(TestStatistic, Threshold, FrequencyTolerance, FreqInterest);
        %assignes calculated value to struct array containing detection results
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Frequency = FrequencyEst(:);
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Coherence = NaN*ones(length(FrequencyEst),1);     
        if ~isempty(FrequencyInd)
            DetectionResults(SelectedDataTypeInd(ChannelIdx)).Coherence = GMSC_est(FrequencyInd,ChannelIdx);
        end
        AdditionalOutput(SelectedDataTypeInd(ChannelIdx)).Threshold = Threshold;
        AdditionalOutput(SelectedDataTypeInd(ChannelIdx)).SignalCoherenceSpectrum = GMSC_est(:,ChannelIdx);
    end
end
end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
% Created by: Jim Follum (james.follum@pnnl.gov)
function ExtractedParameters = ExtractParameters(Parameters,fs)

% Mode of operation - 'SingleChannel' or 'MultiChannel'
if isfield(Parameters,'Mode')
    % Use specified mode
    Mode = Parameters.Mode;
else
    % Use default mode: single channel
    Mode = 'SingleChannel';
end

% Number of samples to use in the analysis
if isfield(Parameters,'AnalysisLength')
    % Use specified value
    AnalysisLength = str2double(Parameters.AnalysisLength)*fs;
else
    error('AnalysisLength must be specified for the spectral coherence based forced oscillation detector.');
end

% delay in samples used to calculate self-GMC
if isfield(Parameters,'Delay')
    % Use specified value
    Delay = ceil(str2double(Parameters.Delay))*fs;
else
    % Use default value (length of the input signals)
    Delay = floor(AnalysisLength/5);
end

% Number of delays in the self-GMSC
if isfield(Parameters,'NumberDelays')
    % Use specified value
    NumberDelays = ceil(str2double(Parameters.NumberDelays));
    if NumberDelays < 2
        NumberDelays = 2;
    end
else
    % Use default value (length of the input signals)
    NumberDelays = 2;
end

% Number of delays in the self-GMSC
if isfield(Parameters,'ThresholdScale')
    % Use specified value
    ThresholdScale = str2double(Parameters.ThresholdScale);
    if ThresholdScale  < 2
        ThresholdScale  = 2;
    end
else
    % Use default value 
    ThresholdScale = 3;
end

%Type of window used for the self-GMSC. Options are rectangular, bartlett,
%hann, hamming, and blackman. If omitted, default is hann
if isfield(Parameters,'WindowType')
    % Use specified window
    WindowType = Parameters.WindowType;
else
    % Use default window
    WindowType = 'hann';
end

% Zero padded length of the test statistic periodogram, Daniell-Welch
% periodogram, and GMSC. If omitted, no zero padding is implemented.
if isfield(Parameters,'FrequencyInterval')
    % Use specified zero padding
    FrequencyInterval = str2double(Parameters.FrequencyInterval);
    ZeroPadding = round(fs/FrequencyInterval);
else
    % Use default zero padding (none for the test statistic periodogram)
    ZeroPadding = AnalysisLength*fs;
end

% Length of the sections for the self-GMSC. If 
% omitted, default is 1/8 of K (AnalyisLength).
if isfield(Parameters,'WindowLength')
    % Use specified window length
    WindowLength = str2double(Parameters.WindowLength)*fs;
else
    % Use default window length
    WindowLength = floor(AnalysisLength/8);
end

% Amount of overlap between sections for the self-GMSC. If omitted, default
% is half of WindowLength.
if isfield(Parameters,'WindowOverlap')
    % Use specified window overlap
    WindowOverlap = str2double(Parameters.WindowOverlap)*fs;
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

ExtractedParameters = struct('Mode',Mode,'AnalysisLength',AnalysisLength,...
    'Delay',Delay,'NumberDelays',NumberDelays,'ThresholdScale',ThresholdScale,...
    'WindowType',WindowType,'ZeroPadding',ZeroPadding,...
    'WindowLength',WindowLength,'WindowOverlap',WindowOverlap,...    
    'FrequencyMin',FrequencyMin,'FrequencyMax',FrequencyMax,...
    'FrequencyTolerance',FrequencyTolerance);

end

%% Nested function

% function [Data, AppropriateDataTypeInd] = CheckDataTypeQuality(Data,DataType,AnalysisLength)
% This function selects signals with appropriate data types, and also
% discards channel containing NaN data point(s) for forced oscillation
% detection purpose using periodogram method. 
%
% Inputs: 
%     Data: PMU measurements
%     DataType: Signal type 
%     AnalysisLength: Length of signal for analysis
%             
% Outputs:
%     Data: PMU measurements that meets the requirements for analysis
%     AppropriateDataTypeInd: Indices of data that meets the requirements for analysis
%             
% Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov) on 07/19/2016

function [Data, AppropriateDataTypeInd] = CheckDataTypeQuality(Data,DataType)

%finds indices of appropriate signal type for FO detection using periodogram method
AppropriateDataTypeInd = find(strcmp(DataType,'F') | strcmp(DataType,'P') | strcmp(DataType,'Q') | strcmp(DataType,'S') | strcmp(DataType,'OTHER'));
NaNSignalInd = [];
for ChannelIdx = 1:length(AppropriateDataTypeInd)
    if ~isempty(find(isnan(Data(:,AppropriateDataTypeInd(ChannelIdx))), 1))
        %gives indices of signal with NaN data point(s)
        NaNSignalInd = [NaNSignalInd ChannelIdx];
    end
end
AppropriateDataTypeInd(NaNSignalInd) = [];
%matrix consisting of appropriate data segment and signal type
Data = Data(:,AppropriateDataTypeInd);
end