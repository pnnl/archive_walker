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

%#function rectwin
%#function bartlett
%#function hann
%#function hamming
%#function blackman

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs. The sampling rate is needed for some default
% parameter values.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString] = ExtractData(PMUstruct,Parameters);
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
ExtractedParameters = ExtractFOdetectionParamsPer(Parameters,fs);

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
CalcDEF = ExtractedParameters.CalcDEF;

if CalcDEF
    NumDEFpaths = size(Parameters.DEF.PathDescription,2);
else
    try
        NumDEFpaths = size(Parameters.DEF.PathDescription,2);
    catch
        NumDEFpaths = 1;
    end
end

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

% Initialize structures to output detection results and additional outputs

%all appropriate fields are assigned NaN value so that NaN value is returned if none of the signal is selected for signal analysis
if strcmp(Mode,'MultiChannel')
    DetectionResults.PMU = DataPMU;
    DetectionResults.Channel = DataChannel;
    DetectionResults.Unit = DataUnit;
    DetectionResults.Frequency = NaN;
    DetectionResults.Amplitude = NaN*ones(1,length(DataChannel));
    DetectionResults.SNR = NaN*ones(1,length(DataChannel));
    DetectionResults.DEF = NaN(NumDEFpaths,1);
    AdditionalOutput.SignalPSD = NaN*ones(LengthFreqInterest,length(DataChannel));
    AdditionalOutput.AmbientNoiseSpectrum = NaN*ones(LengthFreqInterest,length(DataChannel));
    AdditionalOutput.TestStatistic = NaN*ones(LengthFreqInterest,1);
    AdditionalOutput.Threshold = NaN*ones(LengthFreqInterest,1);
    AdditionalOutput.Frequency = FreqInterest;
    AdditionalOutput.Mode = Mode;   
    AdditionalOutput.fs = fs;
    AdditionalOutput.Start = TimeString{end-size(SelectedData,1)+1};
    AdditionalOutput.End = TimeString{end};
else
    DetectionResults = struct('PMU',[],'Channel',[],'Frequency',[],'Amplitude',[],'SNR',[],'DEF',[]);
    AdditionalOutput = struct('SignalPSD',[], 'AmbientNoiseSpectrum', [], 'TestStatistic', [],'Threshold',[],'Frequency',[],'Mode',[],'fs',fs,'Start',TimeString{end-size(SelectedData,1)+1},'End',TimeString{end});

    for ChannelIdx = 1:length(DataChannel)
        DetectionResults(ChannelIdx).PMU = DataPMU(ChannelIdx);
        DetectionResults(ChannelIdx).Channel = DataChannel(ChannelIdx);
        DetectionResults(ChannelIdx).Unit = DataUnit(ChannelIdx);
        DetectionResults(ChannelIdx).Frequency = NaN;
        DetectionResults(ChannelIdx).Amplitude = NaN;
        DetectionResults(ChannelIdx).SNR = NaN;
        DetectionResults(ChannelIdx).DEF = NaN;
        AdditionalOutput(ChannelIdx).SignalPSD = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).AmbientNoiseSpectrum = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).TestStatistic = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).Threshold = NaN*ones(LengthFreqInterest,1);
        AdditionalOutput(ChannelIdx).Frequency = FreqInterest;
        AdditionalOutput(ChannelIdx).Mode = Mode;   
        AdditionalOutput(ChannelIdx).fs = fs;
        AdditionalOutput(ChannelIdx).Start = TimeString{end-size(SelectedData,1)+1};
        AdditionalOutput(ChannelIdx).End = TimeString{end};
    end
end

% AdditionalOutput(1).DataRaw = Data(end-size(SelectedData,1)+1:end,:);
AdditionalOutput(1).Data = Data(end-size(SelectedData,1)+1:end,:);
AdditionalOutput(1).Data(:,setdiff(1:size(Data,2),SelectedDataTypeInd)) = NaN;
AdditionalOutput(1).DataPMU = DataPMU;
AdditionalOutput(1).DataChannel = DataChannel;
AdditionalOutput(1).DataType = DataType;
AdditionalOutput(1).DataUnit = DataUnit;
AdditionalOutput(1).TimeString = TimeString(end-size(SelectedData,1)+1:end);

if isempty(SelectedDataTypeInd)
    % returns if none of the singals are apprpriate for further analysis, 
    return;
end

% for ChannelIdx = 1:length(SelectedDataTypeInd)
%     SelectedData(:,ChannelIdx) = SelectedData(:,ChannelIdx) - mean(SelectedData(:,ChannelIdx));
% end
% % SelectedData(:,3) = SelectedData(:,3) + 5*sin(2*pi*0.5*t');
% % SelectedData(:,5) = SelectedData(:,5) + 5*sin(2*pi*0.75*t');
% % SelectedData(:,7) = SelectedData(:,7) + 6*sin(2*pi*1*t');

%% Perform detection

%calculates power spectrum of selected signals
SignalPSD = CalcPSD_OmegaB(SelectedData, ZeroPadding, 0, PeriodogramWindow, [], fs,OmegaB);
%estimates power spectrum of ambient noise using power spectrum of selected signal
AmbientNoisePSD = CalcPSD_OmegaB(SelectedData, ZeroPadding, WindowOverlap, GMSCandPSDwindow,MedianFilterOrder,fs,OmegaB);
if strcmp(Mode,'MultiChannel')
    %gives test statistic for detecting FO
    TestStatistic = sum(2*SignalPSD./AmbientNoisePSD,2);  % sum of the PSDs calculated for all channels
    %gives estimates of generalized magnitude squared coherence
    GMSC_est = calcGMSC(SelectedData, ZeroPadding, WindowOverlap, GMSCandPSDwindow,OmegaB,MedianFilterOrder);
    %gives threshold for detecting FO
    Threshold = CalcThreshold(GMSC_est, Pfa, LengthFreqInterest, length(SelectedDataTypeInd));
    % gives estimates of frequency and amplitude of forced oscillations
    [Frequency_est, Amplitude_est, SNR_est] = DetectFO(TestStatistic, Threshold, SignalPSD, AmbientNoisePSD, FrequencyTolerance, PeriodogramWindow, FreqInterest);
    %assignes calculated value to struct array containing detection results
    DetectionResults.Frequency = Frequency_est(:);
    DetectionResults.Amplitude = NaN*ones(length(Frequency_est),length(DataChannel));
    DetectionResults.Amplitude(:,SelectedDataTypeInd) = Amplitude_est;
    DetectionResults.SNR = NaN*ones(length(Frequency_est),length(DataChannel));
    DetectionResults.SNR(:,SelectedDataTypeInd) = SNR_est;
    if CalcDEF
        if ~isnan(Frequency_est)
            DetectionResults.DEF = CalculateDEF(PMUstruct,Parameters.DEF,Frequency_est);
        else
            DetectionResults.DEF = NaN(NumDEFpaths,length(Frequency_est));
        end
    else
        DetectionResults.DEF = NaN(NumDEFpaths,length(Frequency_est));
    end
    AdditionalOutput.SignalPSD(:,SelectedDataTypeInd) = SignalPSD;
    AdditionalOutput.AmbientNoiseSpectrum(:,SelectedDataTypeInd) = AmbientNoisePSD;
    AdditionalOutput.TestStatistic = TestStatistic;
    AdditionalOutput.Threshold = Threshold;
    
else %analyses signal for single channel mode
    for ChannelIdx = 1:length(SelectedDataTypeInd)
        %gives test statistic for detecting FO
        TestStatistic = SignalPSD(:,ChannelIdx);%./AmbientNoisePSD(OmegaB,ChannelIdx);
        %gives threshold for detecting FO
        Threshold = -AmbientNoisePSD(:,ChannelIdx)*log(Pfa/LengthFreqInterest);
%         figure;
%         plot(Threshold); hold all; plot(TestStatistic);
%         pfa = 1-(1-exp(-Threshold))^LengthFreqInterest;
        % gives estimates of frequency and amplitude of forced oscillations
        [Frequency_est, Amplitude_est, SNR_est] = DetectFO(TestStatistic, Threshold, SignalPSD(:,ChannelIdx), AmbientNoisePSD(:,ChannelIdx), FrequencyTolerance, PeriodogramWindow, FreqInterest);
        %assignes calculated value to struct array containing detection results
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Frequency = Frequency_est(:);
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Amplitude = NaN*ones(length(Frequency_est),1);
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).Amplitude = Amplitude_est;
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).SNR = NaN*ones(length(Frequency_est),1);
        DetectionResults(SelectedDataTypeInd(ChannelIdx)).SNR = SNR_est;
        if CalcDEF
            if ~isnan(Frequency_est)
                DetectionResults(SelectedDataTypeInd(ChannelIdx)).DEF = CalculateDEF(PMUstruct,Parameters.DEF,Frequency_est);
            else
                DetectionResults(SelectedDataTypeInd(ChannelIdx)).DEF = NaN(NumDEFpaths,length(Frequency_est));
            end
        else
            DetectionResults(SelectedDataTypeInd(ChannelIdx)).DEF = NaN(NumDEFpaths,length(Frequency_est));
        end
        AdditionalOutput(:,SelectedDataTypeInd(ChannelIdx)).SignalPSD = SignalPSD(:,ChannelIdx);
        AdditionalOutput(:,SelectedDataTypeInd(ChannelIdx)).AmbientNoiseSpectrum = AmbientNoisePSD(:,ChannelIdx);
        AdditionalOutput(SelectedDataTypeInd(ChannelIdx)).TestStatistic = TestStatistic;   
        AdditionalOutput(SelectedDataTypeInd(ChannelIdx)).Threshold = Threshold;
    end    
end
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