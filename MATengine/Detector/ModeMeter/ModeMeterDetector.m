%   function [DetectionResults, AdditionalOutput] = ModeMeterDetector(PMUstruct,Parameters,PastAdditionalOutput)
%   This function implemements the Thevenin application
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

% ResultUpdateInterval = Parameters.ResultUpdateInterval;

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs. The sampling rate is needed for some default
% parameter values.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString] = ExtractDataModemeter(PMUstruct,Parameters);
catch
    warning('Input data for the mode-meter could not be used.');
    DetectionResults = struct('FOfreq',cell(1,length(Parameters.Mode)),'ModeName',cell(1,length(Parameters.Mode)), 'ChannelName',cell(1,length(Parameters.Mode)), 'DampingRatio',cell(1,length(Parameters.Mode)));%'MpathRemainder',[],'Name',Parameters.Name
    AdditionalOutput = struct('MpathRemainder',[],'ModeOriginal',[],...
        'ModeTrack',[],'OperatingValues',[], 'OperatingNames',[], 'OperatingUnits',[],...
        'Data',[],'DataPMU',[],'DataChannel',[],'DataType',[],'DataUnit',[],'TimeString',[]);
    return
end
DetectionResults = struct([]);
AdditionalOutput = struct([]);
ExtractedParametersAll = ExtractModeMeterParams(Parameters,fs);
AdditionalOutput(1).OperatingValues = [];

for ModeIdx = 1:length(Parameters.Mode)
    EventOccurenceIdx = 1;
    ExtractedParameters = ExtractedParametersAll{ModeIdx};
    TimeStringDN = datenum(TimeString{ModeIdx});
    NumMethods = length(ExtractedParameters.AlgorithmSpecificParameters);
    % Initialize Result structure
    % Set up result matrices
    %     Mode = NaN(size(Data{ModeIdx},2),NumMethods);    
    AdditionalOutput(ModeIdx).Data = Data{ModeIdx}(end-size(Data{ModeIdx},1)+1:end,:);
    AdditionalOutput(ModeIdx).DataPMU = DataPMU{ModeIdx};
    AdditionalOutput(ModeIdx).DataChannel = DataChannel{ModeIdx};
    AdditionalOutput(ModeIdx).DataType = DataType{ModeIdx};
    AdditionalOutput(ModeIdx).DataUnit = DataUnit{ModeIdx};
    AdditionalOutput(ModeIdx).TimeString = TimeString{ModeIdx}(end-size(Data{ModeIdx},1)+1:end);
    if isempty(PastAdditionalOutput)
        AdditionalOutput(ModeIdx).Modetrack = cell(NumMethods,size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).Modetrack(:) = {{}};
        AdditionalOutput(ModeIdx).t = TimeStringDN(end);
        AdditionalOutput(ModeIdx).ModeHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeDRHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeFreqHistory = cell(1,NumMethods*size(Data{ModeIdx},2));
    else
        AdditionalOutput(ModeIdx).Modetrack = PastAdditionalOutput(ModeIdx).Modetrack;
        AdditionalOutput(ModeIdx).ModeHistory = PastAdditionalOutput(ModeIdx).ModeHistory;
        AdditionalOutput(ModeIdx).ModeDRHistory = PastAdditionalOutput(ModeIdx).ModeDRHistory;
        AdditionalOutput(ModeIdx).ModeFreqHistory = PastAdditionalOutput(ModeIdx).ModeFreqHistory;
        AdditionalOutput(ModeIdx).t = [PastAdditionalOutput(ModeIdx).t ;TimeStringDN(end); ];
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
                AdditionalOutput(ModeIdx).ModeHistory{EventOccurenceIdx} = [AdditionalOutput(ModeIdx).ModeHistory{EventOccurenceIdx};Mode;];
                AdditionalOutput(ModeIdx).ModeDRHistory{EventOccurenceIdx} = [AdditionalOutput(ModeIdx).ModeDRHistory{EventOccurenceIdx};DampingRatio;];
                AdditionalOutput(ModeIdx).ModeFreqHistory{EventOccurenceIdx} = [AdditionalOutput(ModeIdx).ModeFreqHistory{EventOccurenceIdx};Frequency;];
                AdditionalOutput(ModeIdx).ChannelsName{EventOccurenceIdx} = DataChannel{ModeIdx}(ChanIdx);
                AdditionalOutput(ModeIdx).ModeOfInterest{EventOccurenceIdx}= ExtractedParameters.ModeName;                              
                AdditionalOutput(ModeIdx).MethodName{EventOccurenceIdx}= ExtractedParameters.MethodName{MethodIdx};
                EventOccurenceIdx = EventOccurenceIdx + 1;
            end
        else
            if ~isempty(ExtractedParameters.FOdetectorPara)
                % Run FO detection parameter: %the first output contains all detected FO frequencies and the second one contains refined FO frequencies
                [DetectionResults(ModeIdx).FOfreqAll{ChanIdx}, DetectionResults(ModeIdx).FOfreqRefined{ChanIdx}]...
                    = FOdetectionForModeMeter(Data{ModeIdx}(:,ChanIdx),ExtractedParameters.FOdetectorPara,fs{ModeIdx},ExtractedParameters.AnalysisLength);
            else
                DetectionResults(ModeIdx).FOfreqAll{ChanIdx} = [];
                DetectionResults(ModeIdx).FOfreqRefined{ChanIdx} = [];
            end
            for MethodIdx = 1:NumMethods
                [Mode, AdditionalOutput(ModeIdx).Modetrack{MethodIdx,ChanIdx}]...
                    = eval([ExtractedParameters.MethodName{MethodIdx}...
                    '(Data{ModeIdx}(:,ChanIdx),ExtractedParameters.AlgorithmSpecificParameters(MethodIdx), ExtractedParameters.DesiredModes,fs{ModeIdx},AdditionalOutput(ModeIdx).Modetrack{MethodIdx,ChanIdx},DetectionResults(ModeIdx).FOfreqAll{ChanIdx},DetectionResults(ModeIdx).FOfreqRefined{ChanIdx})']);
                DampingRatio = -real(Mode)/abs(Mode);
                Frequency = abs(imag(Mode))/2/pi;
                if Frequency==0 && isnan(DampingRatio)
                    Frequency = NaN;
                end
                AdditionalOutput(ModeIdx).ModeHistory{EventOccurenceIdx} = [AdditionalOutput(ModeIdx).ModeHistory{EventOccurenceIdx};Mode;];
                AdditionalOutput(ModeIdx).ModeDRHistory{EventOccurenceIdx} = [AdditionalOutput(ModeIdx).ModeDRHistory{EventOccurenceIdx};DampingRatio;];
                AdditionalOutput(ModeIdx).ModeFreqHistory{EventOccurenceIdx} = [AdditionalOutput(ModeIdx).ModeFreqHistory{EventOccurenceIdx};Frequency;];
                AdditionalOutput(ModeIdx).ChannelsName{EventOccurenceIdx} = DataChannel{ModeIdx}(ChanIdx);
                AdditionalOutput(ModeIdx).ModeOfInterest{EventOccurenceIdx}= ExtractedParameters.ModeName;                              
                AdditionalOutput(ModeIdx).MethodName{EventOccurenceIdx}= ExtractedParameters.MethodName{MethodIdx};
                EventOccurenceIdx = EventOccurenceIdx + 1;
            end
        end        
    end
end

% Get power system operating condition values
% OperatingValues = [];
% OperatingNames = {};
% OperatingUnits = {};
% OperatingType = {};
if isfield(Parameters,'BaseliningSignals')
%     if length(Parameters.BaseliningSignals) == 1
%         Parameters.BaseliningSignals = {Parameters.BaseliningSignals};
%     end
%     
%     for idx = 1:length(Parameters.BaseliningSignals)
        [Data, ~, OperatingNames, OperatingType, OperatingUnits, ~, ~, ~] = ExtractData(PMUstruct,Parameters.BaseliningSignals);
        OperatingValues = Data(end,:);%[OperatingValues Data(end,:)];
%         OperatingNames = [OperatingNames DataChannelOp];
%         OperatingUnits = [OperatingUnits DataUnit];
%         OperatingType = [OperatingType DataType];
%     end
end
% if isfield(Parameters,'AngleSignals')
%     if length(Parameters.AngleSignals) == 1
%         Parameters.AngleSignals = {Parameters.AngleSignals};
%     end
%     
%     for idx = 1:length(Parameters.AngleSignals)
%         [Data, ~, DataChannelOp, DataType, DataUnit, ~, ~, ~] = ExtractData(PMUstruct,Parameters.AngleSignals{idx});
%         OperatingValues = [OperatingValues Data(end,:)];
%         OperatingNames = [OperatingNames DataChannelOp];
%         OperatingUnits = [OperatingUnits DataUnit];
%         OperatingType = [OperatingType DataType];
%     end
% end

if isempty(PastAdditionalOutput)
    AdditionalOutput(1).OperatingValues = OperatingValues;
else
    AdditionalOutput(1).OperatingValues = [PastAdditionalOutput(1).OperatingValues; OperatingValues;];
end

AdditionalOutput(1).OperatingNames = OperatingNames;
AdditionalOutput(1).OperatingUnits = OperatingUnits;
AdditionalOutput(1).OperatingType = OperatingType;
if ~isempty(PastAdditionalOutput)
    AdditionalOutput(1).Mode_n_SysCondList = WriteOperatingConditionsAndModeValues(AdditionalOutput,ExtractedParameters.ResultPath,PastAdditionalOutput(1).Mode_n_SysCondList);
else
    AdditionalOutput(1).Mode_n_SysCondList = WriteOperatingConditionsAndModeValues(AdditionalOutput,ExtractedParameters.ResultPath,[]);
    
end
% [AdditionalOutput, DetectionResult] = CheckModeHistoryLength(AdditionalOutput, DetectionResults, ResultPath]



