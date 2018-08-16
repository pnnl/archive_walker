%   function [DetectionResults, AdditionalOutput] = ModeMeterDetector(PMUstruct,Parameters,PastAdditionalOutput)
%
%   Inputs:
%           PMUstruct: PMU structure in a common format for all PMUs
%           Parameters:
%           PastAdditionalOutput:
%
%   Outputs:
%           DetectionResults:
%           AdditionalOutput:
%
% Created by Urmila Agrawal(urmila.agrawal@pnnl.gov) on 07/09/2018

function [DetectionResults, AdditionalOutput] = ModeMeterDetector(PMUstruct,Parameters,PastAdditionalOutput)

%#function LS_ARMA
%#function LS_ARMApS
%#function YW_ARMA
%#function YW_ARMApS

%%
% ResultUpdateInterval = Parameters.ResultUpdateInterval;

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs. The sampling rate is needed for some default
% parameter values.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, ~, fs, TimeString] = ExtractDataModemeter(PMUstruct,Parameters);
catch
    warning('Input data for the mode-meter could not be used.');
    DetectionResults = struct('FOfreq',cell(1,length(Parameters.Mode)),'LowDampedMode',[],'ChannelName',[],'MethodName',[]);%'MpathRemainder',[],'Name',Parameters.Name
    AdditionalOutput = struct('ModeOriginal',[],...
        'ModeTrack',[],'OperatingValues',[], 'OperatingNames',[], 'OperatingUnits',[],...
        'Data',[],'DataPMU',[],'DataChannel',[],'DataType',[],'DataUnit',[],'TimeString',[],...
        'ModeOriginal',[],'ModeHistory',[],'ModeFreqHistory',[],'ModeDRHistory',[],'MethodName',[],'ChannelName',[]);
    return
end
DetectionResults = struct([]);
AdditionalOutput = struct([]);
ExtractedParametersAll = ExtractModeMeterParams(Parameters,fs);
AdditionalOutput(1).OperatingValues = [];

for ModeIdx = 1:length(Parameters.Mode)
    ModeEstimateCalcIdx = 1;
    ExtractedParameters = ExtractedParametersAll{ModeIdx};
    TimeStringDN = datenum(TimeString{ModeIdx});
    NumMethods = length(ExtractedParameters.AlgorithmSpecificParameters);
    AdditionalOutput(ModeIdx).Data = Data{ModeIdx}(end-size(Data{ModeIdx},1)+1:end,:);
    AdditionalOutput(ModeIdx).DataPMU = DataPMU{ModeIdx};
    AdditionalOutput(ModeIdx).DataChannel = DataChannel{ModeIdx};
    AdditionalOutput(ModeIdx).DataType = DataType{ModeIdx};
    AdditionalOutput(ModeIdx).DataUnit = DataUnit{ModeIdx};
    AdditionalOutput(ModeIdx).TimeString = TimeString{ModeIdx}(end-size(Data{ModeIdx},1)+1:end);
    if isempty(PastAdditionalOutput)
        AdditionalOutput(ModeIdx).Modetrack = cell(1,NumMethods*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).Modetrack(:) = {{}};
        AdditionalOutput(ModeIdx).t = TimeStringDN(end);
        AdditionalOutput(ModeIdx).ModeOriginal = cell(1,NumMethods*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeDRHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeFreqHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
        TimeString1 = '0';
        TimeString2 = '0';
    else
        AdditionalOutput(ModeIdx).Modetrack = PastAdditionalOutput(ModeIdx).Modetrack;
        AdditionalOutput(ModeIdx).ModeOriginal = PastAdditionalOutput(ModeIdx).ModeOriginal;
        TimeString1 = datestr(AdditionalOutput(ModeIdx).TimeString{end},'yymmdd');
        TimeString2 = datestr(PastAdditionalOutput(ModeIdx).TimeString{end},'yymmdd');
        %Check if there is a transition to the next day, if true then clear
        %additionalouput for mode history, original and other parameters
        if strcmp(TimeString1,TimeString2)
            AdditionalOutput(ModeIdx).ModeHistory = PastAdditionalOutput(ModeIdx).ModeHistory;
            AdditionalOutput(ModeIdx).ModeDRHistory = PastAdditionalOutput(ModeIdx).ModeDRHistory;
            AdditionalOutput(ModeIdx).ModeFreqHistory = PastAdditionalOutput(ModeIdx).ModeFreqHistory;
            AdditionalOutput(ModeIdx).t = [PastAdditionalOutput(ModeIdx).t; TimeStringDN(end);];
        else
            AdditionalOutput(ModeIdx).ModeHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
            AdditionalOutput(ModeIdx).ModeDRHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
            AdditionalOutput(ModeIdx).ModeFreqHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
            AdditionalOutput(ModeIdx).t = TimeStringDN(end);
            PastAdditionalOutput(1).OperatingValues = [];
        end
    end
    
    AdditionalOutput(ModeIdx).fs = fs{ModeIdx};
    % Only analyze channels with no NaN values
    KeepIdx = sum(isnan(Data{ModeIdx}))==0;
    % Indexing the occurrence of events separately for each method and
    % data channel
    
    for ChanIdx = 1:size(Data{ModeIdx},2)
        if ~KeepIdx(ChanIdx)
            for MethodIdx = 1:NumMethods
                Mode=NaN;
                DampingRatio=NaN;
                Frequency=NaN;
                AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx};Mode;];
                AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx};DampingRatio;];
                AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx};Frequency;];
                AdditionalOutput(ModeIdx).ChannelsName{ModeEstimateCalcIdx} = DataChannel{ModeIdx}(ChanIdx);
                AdditionalOutput(ModeIdx).ModeOfInterest{ModeEstimateCalcIdx}= ExtractedParameters.ModeName;
                AdditionalOutput(ModeIdx).MethodName{ModeEstimateCalcIdx}= ExtractedParameters.MethodName{MethodIdx};
                ModeEstimateCalcIdx = ModeEstimateCalcIdx + 1;
            end
        else
            if ~isempty(ExtractedParameters.FOdetectorPara)
                % Run FO detection algorithm
                DetectionResults(ModeIdx).FOfreq{ChanIdx}...
                    = FOdetectionForModeMeter(Data{ModeIdx}(:,ChanIdx),ExtractedParameters.FOdetectorPara,fs{ModeIdx},ExtractedParameters.AnalysisLength);
            else
                DetectionResults(ModeIdx).FOfreq{ChanIdx} = [];
            end
            for MethodIdx = 1:NumMethods
                [Mode, AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx}]...
                    = eval([ExtractedParameters.MethodName{MethodIdx}...
                    '(Data{ModeIdx}(:,ChanIdx),ExtractedParameters.AlgorithmSpecificParameters(MethodIdx), ExtractedParameters.DesiredModes,fs{ModeIdx},AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx},DetectionResults(ModeIdx).FOfreq{ChanIdx})']);
                DampingRatio = -real(Mode)/abs(Mode);
                Frequency = abs(imag(Mode))/2/pi;
                Frequency(isnan(DampingRatio))= NaN;
                if strcmp(ExtractedParameters.RetConTrackingStatus, 'ON')
                    [AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx},AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx},...
                        AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx},AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx},ModeRem]...
                            = RunRetCon(Mode, AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx},AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx},...
                            AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx}, AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx},...
                            ExtractedParameters.MaxRetConLength, Parameters.ResultUpdateInterval);
                        %         If ModeRem is not empty, need to assess .csv for previous days and update
                        %         the mode estimates
                        if ~isempty(ModeRem) && ~isempty(PastAdditionalOutput)
                            N = length(ModeRem);
                            ModeRem = repmat([-100+200*1i],5,1);
                            UpdatePreviousDayModeEst(ModeRem, ExtractedParameters.ResultPathFinal,PastAdditionalOutput(1).Mode_n_SysCondList,TimeStringDN(end),ModeEstimateCalcIdx);
                        end
                else
                    % Reset the tracking cell
                    AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx} = {};
                end
                AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx};Mode;];
                AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx};DampingRatio;];
                AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx};Frequency;];
                AdditionalOutput(ModeIdx).ChannelsName{ModeEstimateCalcIdx} = DataChannel{ModeIdx}(ChanIdx);
                AdditionalOutput(ModeIdx).ModeOfInterest{ModeEstimateCalcIdx}= ExtractedParameters.ModeName;
                AdditionalOutput(ModeIdx).MethodName{ModeEstimateCalcIdx}= ExtractedParameters.MethodName{MethodIdx};
                AdditionalOutput(ModeIdx).ModeOriginal{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeOriginal{ModeEstimateCalcIdx};Mode;];
                ModeEstimateCalcIdx = ModeEstimateCalcIdx + 1;
            end
        end
    end
end

% Get power system operating condition values
if isfield(Parameters,'BaseliningSignals')
    [Data, ~, OperatingNames, OperatingType, OperatingUnits, ~, ~, ~] = ExtractData(PMUstruct,Parameters.BaseliningSignals);
    OperatingValues = Data(end,:);
    
    if isempty(PastAdditionalOutput)
        AdditionalOutput(1).OperatingValues = OperatingValues;
    else
        AdditionalOutput(1).OperatingValues = [PastAdditionalOutput(1).OperatingValues; OperatingValues;];
    end
    AdditionalOutput(1).OperatingNames = OperatingNames;
    AdditionalOutput(1).OperatingUnits = OperatingUnits;
    AdditionalOutput(1).OperatingType = OperatingType;
else
    AdditionalOutput(1).OperatingNames = [];
    AdditionalOutput(1).OperatingUnits = [];
    AdditionalOutput(1).OperatingType = [];
end
%Write modeestimates and system operating points to a .csv file
if ~isempty(PastAdditionalOutput)
    AdditionalOutput(1).Mode_n_SysCondList = WriteOperatingConditionsAndModeValues(AdditionalOutput,ExtractedParameters.ResultPathFinal,PastAdditionalOutput(1).Mode_n_SysCondList);
else
    AdditionalOutput(1).Mode_n_SysCondList = WriteOperatingConditionsAndModeValues(AdditionalOutput,ExtractedParameters.ResultPathFinal,[]);
    
end


