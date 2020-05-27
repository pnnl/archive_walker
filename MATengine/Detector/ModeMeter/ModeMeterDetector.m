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

% These make functions that are called using eval() visible when the tool
% is compiled as a DLL. DO NOT REMOVE!
%
%#function LS_ARMApS
%#function YW_ARMApS
%#function STLS

NumMode = length(Parameters.Mode); % gives number of modes of interest
Data = cell(1,NumMode);
DataPMU = cell(1,NumMode);
DataChannel = cell(1,NumMode);
DataType = cell(1,NumMode);
DataUnit = cell(1,NumMode);
TimeString = cell(1,NumMode);
fs = cell(1,NumMode);
NumMethods = NaN(NumMode,1);
if NumMode==1
    Parameters.Mode = {Parameters.Mode};
end

DataFOdet = cell(1,NumMode);
fsFOdet = cell(1,NumMode);
DataEVENTdet = cell(1,NumMode);
fsEVENTdet = cell(1,NumMode);

try
    for ModeIdx = 1:NumMode
        [Data{ModeIdx}, DataPMU{ModeIdx}, DataChannel{ModeIdx}, DataType{ModeIdx}, DataUnit{ModeIdx}, ~, fs{ModeIdx}, TimeString{ModeIdx}] = ExtractData(PMUstruct,Parameters.Mode{ModeIdx});
        
        AnalysisLength = str2double(Parameters.Mode{ModeIdx}.AnalysisLength)*fs{ModeIdx};
        Data{ModeIdx} = Data{ModeIdx}(end-AnalysisLength+1:end,:);
        TimeString{ModeIdx} = TimeString{ModeIdx}(end-AnalysisLength+1:end);
        
        
        if isfield(Parameters.Mode{ModeIdx},'FOdetectorParam')
            [DataFOdet{ModeIdx}, ~, ~, ~, ~, ~, fsFOdet{ModeIdx}, ~] = ExtractData(PMUstruct,Parameters.Mode{ModeIdx}.FOdetectorParam);
        end
        if isfield(Parameters.Mode{ModeIdx},'EventDetectorParam')
            [DataEVENTdet{ModeIdx}, ~, ~, ~, ~, ~, fsEVENTdet{ModeIdx}, ~] = ExtractData(PMUstruct,Parameters.Mode{ModeIdx}.EventDetectorParam);
            if fs{ModeIdx} ~= fsEVENTdet{ModeIdx}
                error('Sample rate of input to mode meter must match input to transient event detection for the mode meter.');
            end
        end
    end
catch
    warning('Input data for the mode-meter could not be used.');
    DetectionResults = struct([]);
    AdditionalOutput = struct('ModeTrack',[],'OperatingValues',[], 'OperatingNames',[], 'OperatingUnits',[],...
        'Data',[],'DataPMU',[],'DataChannel',[],'DataType',[],'DataUnit',[],'TimeString',[],...
        'ModeOriginal',[],'ModeHistory',[],'ModeFreqHistory',[],'ModeDRHistory',[],'MethodName',[],'ChannelName',[],...
        'ExtraOutput',[]);
    return
end
DetectionResults = struct([]);
AdditionalOutput = struct([]);
ExtractedParametersAll = ExtractModeMeterParams(Parameters,fs,fsFOdet,fsEVENTdet);
AdditionalOutput(1).OperatingValues = [];

if isempty(PastAdditionalOutput)
    FirstCall = true;
else
    FirstCall = false;
end

if isfield(Parameters,'CalcDEF')
    if strcmp(Parameters.CalcDEF,'TRUE')
        NumDEFpaths = length(Parameters.DEF.PathIdx);
        DEFparams = Parameters.DEF;
    else
        DEFparams = [];
        NumDEFpaths = 0;
    end
else
    DEFparams = [];
    NumDEFpaths = 0;
end

for ModeIdx = 1:NumMode
    ModeEstimateCalcIdx = 1;
    ExtractedParameters = ExtractedParametersAll{ModeIdx};
    TimeStringDN = datenum(TimeString{ModeIdx});
    NumMethods(ModeIdx) = length(ExtractedParameters.AlgorithmSpecificParameters);
    AdditionalOutput(ModeIdx).Data = Data{ModeIdx}(end-size(Data{ModeIdx},1)+1:end,:);
    AdditionalOutput(ModeIdx).DataPMU = DataPMU{ModeIdx};
    AdditionalOutput(ModeIdx).DataChannel = DataChannel{ModeIdx};
    AdditionalOutput(ModeIdx).DataType = DataType{ModeIdx};
    AdditionalOutput(ModeIdx).DataUnit = DataUnit{ModeIdx};
    AdditionalOutput(ModeIdx).TimeString = TimeString{ModeIdx}(end-size(Data{ModeIdx},1)+1:end);
    if FirstCall
        AdditionalOutput(ModeIdx).Modetrack = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).Modetrack(:) = {{}};
        AdditionalOutput(ModeIdx).t = TimeStringDN(end);
        AdditionalOutput(ModeIdx).ModeOriginal = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeHistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeDRHistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).ModeFreqHistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
        AdditionalOutput(ModeIdx).DEFhistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
        
        AdditionalOutput(ModeIdx).EventDet = cell(1,size(Data{ModeIdx},2));
        PastAdditionalOutput(ModeIdx).EventDet = cell(1,size(Data{ModeIdx},2));
        
        AdditionalOutput(ModeIdx).FOdet = cell(1,size(Data{ModeIdx},2));
        PastAdditionalOutput(ModeIdx).FOdet = cell(1,size(Data{ModeIdx},2));
        
        AdditionalOutput(ModeIdx).ExtraOutput = cell(size(Data{ModeIdx},2),NumMethods(ModeIdx));
        PastAdditionalOutput(ModeIdx).ExtraOutput = cell(size(Data{ModeIdx},2),NumMethods(ModeIdx));
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
            AdditionalOutput(ModeIdx).DEFhistory = PastAdditionalOutput(ModeIdx).DEFhistory;
            AdditionalOutput(ModeIdx).t = [PastAdditionalOutput(ModeIdx).t; TimeStringDN(end);];
        else
            AdditionalOutput(ModeIdx).ModeHistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
            AdditionalOutput(ModeIdx).ModeDRHistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
            AdditionalOutput(ModeIdx).ModeFreqHistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
            AdditionalOutput(ModeIdx).DEFhistory = cell(1,NumMethods(ModeIdx)*size(Data{ModeIdx},2));
            AdditionalOutput(ModeIdx).t = TimeStringDN(end);
            PastAdditionalOutput(1).OperatingValues = [];
        end
        AdditionalOutput(ModeIdx).ExtraOutput = cell(size(Data{ModeIdx},2),NumMethods(ModeIdx));
    end
    
    AdditionalOutput(ModeIdx).fs = fs{ModeIdx};
    
    for ChanIdx = 1:size(Data{ModeIdx},2)
        % Forced oscillation detection
        if isempty(ExtractedParameters.FOdetectorPara)
            % Forced oscillation detection was not desired
            FOfreq = [];
            TimeLoc = [];
            FOfreqRefined = [];
            TimeLocRefined = [];
        elseif sum(isnan(DataFOdet{ModeIdx}(:,ChanIdx))) > 0
            % There are NaN values in the input signal which prevent 
            % detection from being run
            FOfreq = [];
            TimeLoc = [];
            FOfreqRefined = [];
            TimeLocRefined = [];
        else
            % Run FO detection algorithm
            AdditionalOutput(ModeIdx).FOdet{ChanIdx} = FOdetectionForModeMeter(DataFOdet{ModeIdx}(:,ChanIdx),ExtractedParameters.FOdetectorPara,ExtractedParameters.TimeLocParams,Parameters.ResultUpdateInterval,fsFOdet{ModeIdx},PastAdditionalOutput(ModeIdx).FOdet{ChanIdx});
            
            FOfreq = AdditionalOutput(ModeIdx).FOdet{ChanIdx}.FOfreq;
            FOfreqRefined = AdditionalOutput(ModeIdx).FOdet{ChanIdx}.FOfreqRefined;
            
            % Adjust the values in TimeLoc to account for the difference
            % between sampling rates: fsFOdet{ModeIdx} and fs{ModeIdx}
            TimeLoc = round((AdditionalOutput(ModeIdx).FOdet{ChanIdx}.TimeLoc-1)*fs{ModeIdx}/fsFOdet{ModeIdx} + 1);
            TimeLocRefined = round((AdditionalOutput(ModeIdx).FOdet{ChanIdx}.TimeLocRefined-1)*fs{ModeIdx}/fsFOdet{ModeIdx} + 1);
            % Values at the very end can be assigned to a downsampled index
            % beyond the current window. This corrects for it.
            TimeLoc(TimeLoc > length(Data{ModeIdx}(:,ChanIdx))) = length(Data{ModeIdx}(:,ChanIdx));
            TimeLocRefined(TimeLocRefined > length(Data{ModeIdx}(:,ChanIdx))) = length(Data{ModeIdx}(:,ChanIdx));
        end

        % High-energy event detection
        if ~isempty(ExtractedParameters.EventDetectorPara)
            % Run event detection algorithm
            AdditionalOutput(ModeIdx).EventDet{ChanIdx} = ...
                EventDetectionForModeMeter(Data{ModeIdx}(:,ChanIdx),DataEVENTdet{ModeIdx}(:,ChanIdx),ExtractedParameters.EventDetectorPara,Parameters.ResultUpdateInterval,fsEVENTdet{ModeIdx},PastAdditionalOutput(ModeIdx).EventDet{ChanIdx},PastAdditionalOutput(ModeIdx).ExtraOutput(ChanIdx,:));

            win = AdditionalOutput(ModeIdx).EventDet{ChanIdx}.win;
        else
            win = cell(1,NumMethods(ModeIdx));
            win(:) = {ones(size(Data{ModeIdx}(:,ChanIdx)))};
        end

        for MethodIdx = 1:NumMethods(ModeIdx)
            [Mode, AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx}, AdditionalOutput(ModeIdx).ExtraOutput{ChanIdx,MethodIdx}]...
                = eval([ExtractedParameters.AlgorithmSpecificParameters{MethodIdx}.FunctionName...
                '(Data{ModeIdx}(:,ChanIdx),win{MethodIdx},ExtractedParameters.AlgorithmSpecificParameters{MethodIdx}, ExtractedParameters.DesiredModes,fs{ModeIdx},AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx},FOfreq,TimeLoc,FOfreqRefined,TimeLocRefined)']);
            DampingRatio = -real(Mode)/abs(Mode);
            Frequency = abs(imag(Mode))/2/pi;
            if isnan(DampingRatio)
                Frequency = NaN;
                DEF = NaN(1,NumDEFpaths);
            elseif ~isempty(DEFparams)
                DEF = CalculateDEF(PMUstruct,Parameters.DEF,Frequency);
                DEF = DEF';
            else
                % Good mode estimates, but the DEF is not desired.
                DEF = NaN(1,NumDEFpaths);
            end
            if strcmp(ExtractedParameters.RetConTrackingStatus, 'ON')
                [AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx},AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx},...
                    AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx},AdditionalOutput(ModeIdx).DEFhistory{ModeEstimateCalcIdx},...
                    AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx},ModeRem]...
                    = RunRetCon(Mode, AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx},AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx},...
                    AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx}, AdditionalOutput(ModeIdx).DEFhistory{ModeEstimateCalcIdx},...
                    AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx}, ExtractedParameters.MaxRetConLength, Parameters.ResultUpdateInterval);

                if ~isempty(ModeRem) && ~isempty(PastAdditionalOutput)
                    ChanIdxUpdated = 0;
                    for ModeIdxToUndateChanIdx = 1:(ModeIdx-1)
                        ChanIdxUpdated = ChanIdxUpdated + NumMethods(ModeIdxToUndateChanIdx)*size(Data{ModeIdxToUndateChanIdx},2);
                    end
                    UpdatePreviousDayModeEst(ModeRem, ExtractedParameters.ResultPathFinal,TimeStringDN(end),ChanIdxUpdated + ModeEstimateCalcIdx);
                end
            else
                % Reset the tracking cell
                AdditionalOutput(ModeIdx).Modetrack{ModeEstimateCalcIdx} = {};
            end
            AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeHistory{ModeEstimateCalcIdx};Mode;];
            AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeDRHistory{ModeEstimateCalcIdx};DampingRatio;];
            AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeFreqHistory{ModeEstimateCalcIdx};Frequency;];
            AdditionalOutput(ModeIdx).DEFhistory{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).DEFhistory{ModeEstimateCalcIdx};DEF];
            AdditionalOutput(ModeIdx).ChannelsName{ModeEstimateCalcIdx} = DataChannel{ModeIdx}(ChanIdx);
            AdditionalOutput(ModeIdx).ModeOfInterest{ModeEstimateCalcIdx}= ExtractedParameters.ModeName;
            AdditionalOutput(ModeIdx).MethodName{ModeEstimateCalcIdx}= ExtractedParameters.MethodName{MethodIdx};
            AdditionalOutput(ModeIdx).ModeOriginal{ModeEstimateCalcIdx} = [AdditionalOutput(ModeIdx).ModeOriginal{ModeEstimateCalcIdx};Mode;];
            ModeEstimateCalcIdx = ModeEstimateCalcIdx + 1;
        end
    end
end
% Get power system operating condition values
if isfield(Parameters,'BaseliningSignals')
    [Data, ~, OperatingNames, OperatingType, OperatingUnits, ~, ~, ~] = ExtractData(PMUstruct,Parameters.BaseliningSignals);
    OperatingValues = Data(end,:);
    
    if FirstCall
        AdditionalOutput(1).OperatingValues = OperatingValues;
    else
        AdditionalOutput(1).OperatingValues = [PastAdditionalOutput(1).OperatingValues; OperatingValues];
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
WriteOperatingConditionsAndModeValues(AdditionalOutput,ExtractedParameters.ResultPathFinal,DEFparams);



