%   function [DetectionResults, AdditionalOutput] = PeriodogramDetector(PMUstruct,Parameters)
%   This function implemements periodogram detection 
%   Inputs:
%           PMUstruct: PMU structure in a common format for all PMUs
%           Parameters: User specified values for carrying out detection.
%           If any field is empty, uses provided default values
%                       Mode = Mode of detection, either singlechannel or
%                       multi-channel
%                       AnalysisLength = Number of samples for signal
%                       analysis
%                       WindowType = ype of window used for the test
%                       statistic periodogram, Daniell-Welch periodogram,
%                       and GMSC calculation
%                       ZeroPadding = Zero padded length of the test
%                       statistic periodogram,Daniell-Welch periodogram, and GMSC
%                       WindowLength = Length of the sections for the
%                       Daniell-Welch periodogram and GMSC
%                       WindowOverlap = Amount of overlap between sections
%                       for the Daniell-Welch periodogram and GMSC
%                       MedianFilterOrder = Order for the median filter
%                       used in the Daniell-Welch periodogram
%                       Pfa = Probability of false alarm
%                       FrequencyMin = Minimum frequency to be considered
%                       FrequencyMax = Maximum frequency to be considered
%                       FrequencyTolerance = Tolerance used to refine the
%                       frequency estimate
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
% Modified by: Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/18/2016
% wrote code under section: i. Frequency of interest ii. Check Data quality
% and Signal type iii. Initialization iv. Perform detection

function [DetectionResults, AdditionalOutput] = PeriodogramDetector(PMUstruct,Parameters,PastAdditionalOutput)

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs. The sampling rate is needed for some default
% parameter values.
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
ExtractedParameters = ExtractParameters(Parameters,fs);

% Store the parameters in variables for easier access
Mode = ExtractedParameters.Mode;
AnalysisLength = ExtractedParameters.AnalysisLength;
WindowType = ExtractedParameters.WindowType;
ZeroPadding = ExtractedParameters.ZeroPadding;
WindowLength = ExtractedParameters.WindowLength;
WindowOverlap = ExtractedParameters.WindowOverlap;
MedianFilterOrder = ExtractedParameters.MedianFilterOrder;
Pfa = ExtractedParameters.Pfa;
FrequencyMin = ExtractedParameters.FrequencyMin;
FrequencyMax = ExtractedParameters.FrequencyMax;
FrequencyTolerance = ExtractedParameters.FrequencyTolerance;

%% Based on the specified parameters, initialize useful variables

% Window for test statistic periodogram
PeriodogramWindow = eval([WindowType '(AnalysisLength)']);

% Window for the GMSC and Daniell-Welch periodogram (PSD estimate)
GMSCandPSDwindow = eval([WindowType '(WindowLength)']);

%% Frequency of interest
freqAll = fs*(0:ZeroPadding-1)/ZeroPadding;
OmegaB = find(freqAll>FrequencyMin & freqAll <FrequencyMax); %Frequency bins of interest;
FreqInterest = freqAll(OmegaB); % Frequency of interest
LengthFreqInterest = length(FreqInterest); %Number of frequency bins of interest

%% Check Data quality and Signal type

%Removes data channel containing even one NaN value
%Signals of type Active Power, Reactive Power, Apparent Power, Frequency
%and Others are selected for analysis purpose
[SelectedData, SelectedDataTypeInd] = CheckDataTypeQuality(Data, DataType, AnalysisLength);

%% Initialization

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'Frequency',[],'Amplitude',[]);
% Initialize structure for additional outputs
AdditionalOutput = struct('SignalPSD',[], 'AmbientNoiseSpectrum', [], 'TestStatistic', [],'Threshold',[],'Frequency',[],'Mode',[]);

%all appropriate fields are assigned NaN value so that NaN value is returned if none of the signal is selected for signal analysis
if strcmp(Mode,'MultiChannel')
    DetectionResults.PMU = DataPMU;
    DetectionResults.Channel = DataChannel;
    DetectionResults.Frequency = NaN;
    DetectionResults.Amplitude = NaN*ones(1,length(DataChannel));
    AdditionalOutput.SignalPSD = NaN*ones(LengthFreqInterest,length(DataChannel));
    AdditionalOutput.AmbientNoiseSpectrum = NaN*ones(LengthFreqInterest,length(DataChannel));
    AdditionalOutput.TestStatistic = NaN*ones(LengthFreqInterest,1);
    AdditionalOutput.Threshold = NaN*ones(LengthFreqInterest,1);
    AdditionalOutput.Frequency = FreqInterest;
    AdditionalOutput.Mode = Mode;    
    
else
    for ChannelIdx = 1:length(DataChannel)
        DetectionResults(ChannelIdx).PMU = DataPMU(ChannelIdx);
        DetectionResults(ChannelIdx).Channel = DataChannel(ChannelIdx);
        DetectionResults(ChannelIdx).Frequency = NaN;
        DetectionResults(ChannelIdx).Amplitude = NaN;
        AdditionalOutput(ChannelIdx).SignalPSD = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).AmbientNoiseSpectrum = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).TestStatistic = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).Threshold = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).Frequency = FreqInterest;
        AdditionalOutput(ChannelIdx).Mode = Mode;   
    end
end

if isempty(SelectedDataTypeInd)
    % returns if none of the singals are apprpriate for further analysis, 
    return;
end

for ChannelIdx = 1:length(SelectedDataTypeInd)
    SelectedData(:,ChannelIdx) = SelectedData(:,ChannelIdx) - mean(SelectedData(:,ChannelIdx));
end
% SelectedData(:,3) = SelectedData(:,3) + 5*sin(2*pi*0.5*t');
% SelectedData(:,5) = SelectedData(:,5) + 5*sin(2*pi*0.75*t');
% SelectedData(:,7) = SelectedData(:,7) + 6*sin(2*pi*1*t');

%% Perform detection

%calculates power spectrum of selected signals
SignalPSD = CalcPSD(SelectedData, ZeroPadding, 0, PeriodogramWindow, [], fs);
%estimates power spectrum of ambient noise using power spectrum of selected signal
AmbientNoisePSD = CalcPSD(SelectedData, ZeroPadding, WindowOverlap, GMSCandPSDwindow,MedianFilterOrder,fs);
if strcmp(Mode,'MultiChannel')
    %gives test statistic for detecting FO
    TestStatistic = sum(2*SignalPSD(OmegaB,:)./AmbientNoisePSD(OmegaB,:),2);  % sum of the PSDs calculated for all channels
    %gives estimates of generalized magnitude squared coherence
    GMSC_est = calcGMSC(SelectedData, ZeroPadding, WindowOverlap, GMSCandPSDwindow,OmegaB);
    %gives threshold for detecting FO
    Threshold = CalcThreshold(GMSC_est, Pfa, LengthFreqInterest, length(SelectedDataTypeInd));
    % gives estimates of frequency and amplitude of forced oscillations
    [Frequency_est, Amplitude_est] = DetectFO(TestStatistic, Threshold, SignalPSD(OmegaB,:), AmbientNoisePSD(OmegaB,:), FrequencyTolerance, PeriodogramWindow, FreqInterest);
    %assignes calculated value to struct array containing detection results
    DetectionResults.Frequency = Frequency_est(:);
    DetectionResults.Amplitude = NaN*ones(length(Frequency_est),length(DataChannel));
    DetectionResults.Amplitude(:,SelectedDataTypeInd) = Amplitude_est;
    AdditionalOutput.SignalPSD(:,SelectedDataTypeInd) = SignalPSD(OmegaB,:);
    AdditionalOutput.AmbientNoiseSpectrum(:,SelectedDataTypeInd) = AmbientNoisePSD(OmegaB,:);
    AdditionalOutput.TestStatistic = TestStatistic;
    AdditionalOutput.Threshold = Threshold;
    
else %analyses signal for single channel mode
    for ChannelIdx = 1:length(SelectedDataTypeInd)
        %gives test statistic for detecting FO
        TestStatistic = SignalPSD(OmegaB,ChannelIdx);%./AmbientNoisePSD(OmegaB,ChannelIdx);
        %gives threshold for detecting FO
        Threshold = -AmbientNoisePSD(OmegaB,ChannelIdx)*log(Pfa/LengthFreqInterest);
%         figure;
%         plot(Threshold); hold all; plot(TestStatistic);
%         pfa = 1-(1-exp(-Threshold))^LengthFreqInterest;
        % gives estimates of frequency and amplitude of forced oscillations
        [Frequency_est, Amplitude_est] = DetectFO(TestStatistic, Threshold, SignalPSD(OmegaB,ChannelIdx), AmbientNoisePSD(OmegaB,ChannelIdx), FrequencyTolerance, PeriodogramWindow, FreqInterest);
        %assignes calculated value to struct array containing detection results
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Frequency = Frequency_est(:);
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Amplitude = NaN*ones(length(Frequency_est),1);
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Amplitude = Amplitude_est;
        AdditionalOutput(:,SelectedDataTypeInd(ChannelIdx)).SignalPSD = SignalPSD(OmegaB,ChannelIdx);
        AdditionalOutput(:,SelectedDataTypeInd(ChannelIdx)).AmbientNoiseSpectrum = AmbientNoisePSD(OmegaB,ChannelIdx);
        AdditionalOutput(SelectedDataTypeInd(ChannelIdx)).TestStatistic = TestStatistic;   
        AdditionalOutput(SelectedDataTypeInd(ChannelIdx)).Threshold = Threshold;
    end    
end
end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified.
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
    error('AnalysisLength must be specified for the Periodogram-based forced oscillation detector.');
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
if isfield(Parameters,'FrequencyInterval')
    % Use specified zero padding
    FrequencyInterval = str2double(Parameters.FrequencyInterval);
    ZeroPadding = round(fs/FrequencyInterval);
else
    % Use default zero padding (none for the test statistic periodogram)
    ZeroPadding = AnalysisLength*fs;
    FrequencyInterval = fs/ZeroPadding;
end

% Length of the sections for the Daniell-Welch periodogram and GMSC. If
% omitted, default is 1/3 of K (AnalyisLength).
if isfield(Parameters,'WindowLength')
    % Use specified window length
    WindowLength = str2double(Parameters.WindowLength)*fs;
else
    % Use default window length
    WindowLength = floor(AnalysisLength/3);
end

% Amount of overlap between sections for the Daniell-Welch periodogram and
% GMSC. If omitted, default is half of WindowLength
if isfield(Parameters,'WindowOverlap')
    % Use specified window overlap
    WindowOverlap = str2double(Parameters.WindowOverlap)*fs;
else
    % Use default window overlap
    WindowOverlap = floor(WindowLength/2);
end

% Order for the median filter used in the Daniell-Welch periodogram. If
% omitted, the default is the smallest odd integer greater than or equal to
% three times the main lobe width of the applied window. If an even number,
% a number less than 3, or a non-integer is specified, the smallest odd
% integer greater than or equal to 3 and the specified value will be used.
if isfield(Parameters,'MedianFilterFrequencyWidth')
    % Use specified median filter order
    MedianFilterFrequencyWidth = str2double(Parameters.MedianFilterFrequencyWidth);
    
    % Round to ensure that an integer is selected
    MedianFilterOrder = round(MedianFilterFrequencyWidth/FrequencyInterval);
    
    % If the median filter is less than 3, set it to 3
    if MedianFilterOrder < 3
        MedianFilterOrder = 3;
    end
else
    % Use default median filter order.
    % The term inside ceil() is 3 times the main lobe width in radians per
    % sample times the amount of zero padding divided by 2 times pi. The
    % result is 3 times the main lobe width in bins. The full expressions
    % are included in the comments, but the expressions have been reduced
    % to avoid numerical errors when multiplying and dividing by pi.
    switch WindowType
        case 'rectwin'
            MedianFilterOrder = ceil(6*ZeroPadding/WindowLength); % ceil(3 * 4*pi/WindowLength * K0/(2*pi));
        case 'bartlett'
            MedianFilterOrder = ceil(12*ZeroPadding/WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
        case 'hann'
            MedianFilterOrder = ceil(12*ZeroPadding/WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
        case 'hamming'
            MedianFilterOrder = ceil(12*ZeroPadding/WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
        case 'blackman'
            MedianFilterOrder = ceil(18*ZeroPadding/WindowLength); % ceil(3 * 12*pi/WindowLength * K0/(2*pi));
    end
end
% If the median filter order is even, add one to make it odd.
if mod(MedianFilterOrder,2) == 0
    MedianFilterOrder = MedianFilterOrder + 1;
end


% Probability of false alarm. With zero padding it becomes a maximum
if isfield(Parameters,'Pfa')
    % Use specified probability of false alarm
    Pfa = str2num(Parameters.Pfa);
else
    % Use default probability of false alarm
    Pfa = 0.01;
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
    'WindowType',WindowType,'ZeroPadding',ZeroPadding,...
    'WindowLength',WindowLength,'WindowOverlap',WindowOverlap,...
    'MedianFilterOrder',MedianFilterOrder,'Pfa',Pfa,...
    'FrequencyMin',FrequencyMin,'FrequencyMax',FrequencyMax,...
    'FrequencyTolerance',FrequencyTolerance);

end

%% Nested function

% function [Data, AppropriateDataTypeInd] = CheckDataTypeQuality(Data,DataType,AnalysisLength)
% This function selects signals with appropriate data types, and also
% discards channel containing NaN data point(s) for forced oscillation
% detection purpose using periodogram method. This function also selects
% appropriate data segment for each selected signal based on analysislength parameter. 
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

function [Data, AppropriateDataTypeInd] = CheckDataTypeQuality(Data,DataType,AnalysisLength)

%finds indices of appropriate signal type for FO detection using periodogram method
AppropriateDataTypeInd = find(strcmp(DataType,'P') | strcmp(DataType,'Q') | strcmp(DataType,'S') | strcmp(DataType,'F') | strcmp(DataType,'OTHER'));
NaNSignalInd = [];
for ChannelIdx = 1:length(AppropriateDataTypeInd)
    if ~isempty(find(isnan(Data(:,AppropriateDataTypeInd(ChannelIdx))), 1))
        %gives indices of signal with NaN data point(s)
        NaNSignalInd = [NaNSignalInd ChannelIdx];
    end
end
AppropriateDataTypeInd(NaNSignalInd) = [];
%gives starting point of the signal, ending point coincides with last
%sample of data point
DataIntervalStart = size(Data,1) - AnalysisLength +1;
if DataIntervalStart <1
    DataIntervalStart = 1;
end
%matrix consisting of appropriate data segment and signal type
Data = Data(DataIntervalStart:end,AppropriateDataTypeInd);
end











