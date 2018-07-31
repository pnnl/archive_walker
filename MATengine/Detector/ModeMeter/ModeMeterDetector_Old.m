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

function [DetectionResults, AdditionalOutput] = ModeMeterDetector_Old(PMUstruct,Parameters,PastAdditionalOutput)

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
    ExtractedParameters = ExtractedParametersAll{ModeIdx};
    TimeStringDN = datenum(TimeString{ModeIdx});
    NumMethods = length(ExtractedParameters.AlgorithmSpecificParameters);    
    % Initialize Result structure
    % Set up result matrices
    Mode = NaN(size(Data{ModeIdx},2),NumMethods);
%     DetectionResults(ModeIdx).FOfreq = cell(1,size(Data{ModeIdx},2));
%     DetectionResults(ModeIdx).FOfreq = {{}};
    AdditionalOutput(ModeIdx).ModeOfInterest= ExtractedParameters.ModeName;    
    AdditionalOutput(ModeIdx).Data = Data{ModeIdx}(end-size(Data{ModeIdx},1)+1:end,:);
    AdditionalOutput(ModeIdx).DataPMU = DataPMU{ModeIdx};
    AdditionalOutput(ModeIdx).DataChannel = DataChannel{ModeIdx};
    AdditionalOutput(ModeIdx).DataType = DataType{ModeIdx};
    AdditionalOutput(ModeIdx).DataUnit = DataUnit{ModeIdx};
    AdditionalOutput(ModeIdx).TimeString = TimeString{ModeIdx}(end-size(Data{ModeIdx},1)+1:end);
    if isempty(PastAdditionalOutput)
        AdditionalOutput(ModeIdx).Modetrack = cell(NumMethods,size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).Modetrack(:) = {{}};
%         AdditionalOutput(ModeIdx).OperatingValues = [];%Data{ModeIdx}(end,:);
        AdditionalOutput(ModeIdx).ModeOriginal = cell(1,size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeHistory = cell(1,size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).t = TimeStringDN(end);
        AdditionalOutput(ModeIdx).MpathRemainder = cell(NumMethods,size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).MpathRemainder(:) = {{}};
    else
        AdditionalOutput(ModeIdx).Modetrack = PastAdditionalOutput(ModeIdx).Modetrack;
        AdditionalOutput(ModeIdx).ModeOriginal = PastAdditionalOutput(ModeIdx).ModeOriginal;
        AdditionalOutput(ModeIdx).ModeHistory = PastAdditionalOutput(ModeIdx).ModeHistory;
        AdditionalOutput(ModeIdx).t = [PastAdditionalOutput(ModeIdx).t ;TimeStringDN(end); ];
%         AdditionalOutput(ModeIdx).OperatingValue = [PastAdditionalOutput(ModeIdx).OperatingValues;]; %Data{ModeIdx}(end,:)
%         AdditionalOutput(ModeIdx).OperatingPMU = [];%[PastAdditionalOutput(ModeIdx).OperatingPMU; DataPMU{ModeIdx}];
%         AdditionalOutput(ModeIdx).OperatingDataType =[];%[PastAdditionalOutput(ModeIdx).OperatingDataType; DataType{ModeIdx}];
%         AdditionalOutput(ModeIdx).OperatingDataUnit = [];%[PastAdditionalOutput(ModeIdx).OperatingDataUnit; DataUnit{ModeIdx}];
%         AdditionalOutput(ModeIdx).t = [PastAdditionalOutput(ModeIdx).t; TimeStringDN(end)];
    end
    % AdditionalOutput(1).MpathRemainder = cell(NumMethods,size(Data,2));
    AdditionalOutput(ModeIdx).fs = fs{ModeIdx};
%     AdditionalOutput(ModeIdx).Tstart = TimeStringDN(1);
%     AdditionalOutput(ModeIdx).Tend = TimeStringDN(end);
    % Only analyze channels with no NaN values
    KeepIdx = sum(isnan(Data{ModeIdx}))==0;
    % Indexing the occurrence of events separately for each method and
    % data channel
    
    for ChanIdx = 1:size(Data{ModeIdx},2)
        EventOccurenceIdx = 1;
        DetectionResults(ModeIdx).LowDampingRatio{EventOccurenceIdx} = cell(0);
        DetectionResults(ModeIdx).FreqLowDampingRatio{EventOccurenceIdx} = cell(0);
        DetectionResults(ModeIdx).ChannelsWithLowDampedMode{EventOccurenceIdx} = cell(0);
        DetectionResults(ModeIdx).PMUWithLowDampedMode{EventOccurenceIdx} = cell(0);
        DetectionResults(ModeIdx).MethodsWithLowDampedMode{EventOccurenceIdx} = cell(0);
        LowDampingRatio = [];        
        FreqLowDampingRatio = [];        
        DetectionResults(ModeIdx).ChannelsWithLowDampedMode = [];
        if ~KeepIdx(ChanIdx)
            AdditionalOutput(ModeIdx).ModeHistory{ChanIdx} = [AdditionalOutput(ModeIdx).ModeHistory{ChanIdx};NaN(1,length(NumMethods));];
            continue
        end
        if ~isempty(ExtractedParameters.FOdetectorPara)
            % Run FO detection parameter: %the first output contains all detected FO frequencies and the second one contains refined FO frequencies
            [DetectionResults(ModeIdx).FOfreqAll{ChanIdx}, DetectionResults(ModeIdx).FOfreqRefined{ChanIdx}]...
                = FOdetectionForModeMeter(Data{ModeIdx}(:,ChanIdx),ExtractedParameters.FOdetectorPara,fs{ModeIdx},ExtractedParameters.AnalysisLength);
        else
            DetectionResults(ModeIdx).FOfreqAll{ChanIdx} = [];
            DetectionResults(ModeIdx).FOfreqRefined{ChanIdx} = [];
        end
        for MethodIdx = 1:NumMethods
            [Mode(ChanIdx,MethodIdx), AdditionalOutput(ModeIdx).Modetrack{MethodIdx,ChanIdx}]...
                = eval([ExtractedParameters.MethodName{MethodIdx}...
                '(Data{ModeIdx}(:,ChanIdx),ExtractedParameters.AlgorithmSpecificParameters(MethodIdx), ExtractedParameters.DesiredModes,fs{ModeIdx},AdditionalOutput(ModeIdx).Modetrack{MethodIdx,ChanIdx},DetectionResults(ModeIdx).FOfreqAll{ChanIdx},DetectionResults(ModeIdx).FOfreqRefined{ChanIdx})']);
            DampingRatio = -real(Mode(ChanIdx,MethodIdx))/abs(Mode(ChanIdx,MethodIdx));
            Frequency = abs(imag(Mode(ChanIdx,MethodIdx)))/2/pi;
            if DampingRatio < ExtractedParameters.DampRatioThreshold                
                LowDampingRatio= [LowDampingRatio DampingRatio];   
                FreqLowDampingRatio= [FreqLowDampingRatio Frequency];   
                DetectionResults(ModeIdx).MethodsWithLowDampedMode{EventOccurenceIdx} = [DetectionResults(ModeIdx).MethodsWithLowDampedMode{EventOccurenceIdx} ExtractedParameters.MethodName{MethodIdx}];        
            end
               
            %         if strcmp(ExtractModeMeterParams.RetConTracking, 'ON') && ~isempty(PastAdditionalOutput)
            %             [AdditionalOutput(1).ModeHistory{MethodIdx,ChanIdx},AdditionalOutput(1).Modetrack{MethodIdx,ChanIdx},...
            %                 AdditionalOutput(1).MpathRemainder{MethodIdx,ChanIdx}]...
            %                 = GetRetCon(Mode(MethodIdx,ChanIdx), AdditionalOutput(1).ModeHistory{MethodIdx,ChanIdx},...
            %                 AdditionalOutput(1).Modetrack{MethodIdx,ChanIdx},ExtractedParameters.MaxRetConLength, ExtractedParameters.ResultUpdateInterval);
            %         else
            %             % Reset the tracking cell
            %AdditionalOutput(ModeIdx).Modetrack{MethodIdx,ChanIdx} = {Na};
            %         end
            %             if isempty(PastAdditionalOutput)
            %                 AdditionalOutput(ModeIdx).ModeOriginal{MethodIdx,ChanIdx} = Mode(MethodIdx,ChanIdx);
            %                 AdditionalOutput(ModeIdx).ModeHistory{MethodIdx,ChanIdx} = Mode(MethodIdx,ChanIdx);
            %             else
            
            %             end
            
            %        [AdditionalOutput, DetectionResults] = CheckModeHistoryLength(AdditionalOutput, DetectionResults, ResultPath);
        end
%         AdditionalOutput(ModeIdx).ModeOriginal{ChanIdx} = [AdditionalOutput(ModeIdx).ModeOriginal{ChanIdx};Mode(ChanIdx,:);];
        AdditionalOutput(ModeIdx).ModeHistory{ChanIdx} = [AdditionalOutput(ModeIdx).ModeHistory{ChanIdx};Mode(ChanIdx,:);];
        if ~isempty(DetectionResults(ModeIdx).MethodsWithLowDampedMode{EventOccurenceIdx})
            DetectionResults(ModeIdx).LowDampingRatio{EventOccurenceIdx} = LowDampingRatio;
            DetectionResults(ModeIdx).ChannelsWithLowDampedMode{EventOccurenceIdx} = DataChannel{ModeIdx}(ChanIdx);
            DetectionResults(ModeIdx).PMUWithLowDampedMode{EventOccurenceIdx} = DataPMU{ModeIdx}(ChanIdx);
            DetectionResults(ModeIdx).FreqLowDampingRatio{EventOccurenceIdx}=FreqLowDampingRatio;
            EventOccurenceIdx = EventOccurenceIdx + 1;
        end
        
    end
end

% Get power system operating condition values
OperatingValues = [];
OperatingNames = {};
OperatingUnits = {};
OperatingType = {};
if isfield(Parameters,'PowerSignals')
    if length(Parameters.PowerSignals) == 1
        Parameters.PowerSignals = {Parameters.PowerSignals};
    end
    
    for idx = 1:length(Parameters.PowerSignals)
        [Data, ~, DataChannelOp, DataType, DataUnit, ~, ~, ~] = ExtractData(PMUstruct,Parameters.PowerSignals{idx});
        OperatingValues = [OperatingValues Data(end,:)];
        OperatingNames = [OperatingNames DataChannelOp];
        OperatingUnits = [OperatingUnits DataUnit];
        OperatingType = [OperatingType DataType];
    end
end
if isfield(Parameters,'AngleSignals')
    if length(Parameters.AngleSignals) == 1
        Parameters.AngleSignals = {Parameters.AngleSignals};
    end
    
    for idx = 1:length(Parameters.AngleSignals)
        [Data, ~, DataChannelOp, DataType, DataUnit, ~, ~, ~] = ExtractData(PMUstruct,Parameters.AngleSignals{idx});
        OperatingValues = [OperatingValues Data(end,:)];
        OperatingNames = [OperatingNames DataChannelOp];
        OperatingUnits = [OperatingUnits DataUnit];
        OperatingType = [OperatingType DataType];
    end
end

if isempty(PastAdditionalOutput)
    AdditionalOutput(1).OperatingValues = OperatingValues;
else
    AdditionalOutput(1).OperatingValues = [PastAdditionalOutput(1).OperatingValues; OperatingValues;];
end

% % I think something is wrong here. Why is OperatingValues the only one that
% % gets added onto?
% AdditionalOutput(1).OperatingValues = [PastAdditionalOutput(1).OperatingValues; OperatingValues];

AdditionalOutput(1).OperatingNames = OperatingNames;
AdditionalOutput(1).OperatingUnits = OperatingUnits;
AdditionalOutput(1).OperatingType = OperatingType;
if isempty(PastAdditionalOutput)
    AdditionalOutput(1).Mode_n_SysCondList = WriteOperatingConditionsAndModeValues(AdditionalOutput,ExtractedParameters.ResultPath,PastAdditionalOutput(1).Mode_n_SysCondList);

else
    AdditionalOutput(1).Mode_n_SysCondList = WriteOperatingConditionsAndModeValues(AdditionalOutput,ExtractedParameters.ResultPath,[]);

end
% [AdditionalOutput, DetectionResult] = CheckModeHistoryLength(AdditionalOutput, DetectionResults, ResultPath]



