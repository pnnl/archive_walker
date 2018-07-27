% function RerunOut = RerunOutOfRange(RerunStartTime,RerunEndTime,InitializationPath)
%
% This function reruns the general out-of-range detector so that detailed
% results can be displayed in the GUI.
%
% Inputs:
% RerunStartTime = start time for the run in the format MM/DD/YYYY HH:MM:SS
% RerunEndTime = end time for the run in the format MM/DD/YYYY HH:MM:SS
% RerunDetector = type of detector. Acceptable values: 'OutOfRangeGeneral'
% InitializationPath = path to the folder containing initialization files
%
% Example inputs:
% RerunStartTime = '08/21/2016 23:10:30';
% RerunEndTime = '08/21/2016 23:11:52';
% ConfigFile = 'C:\Users\foll154\Documents\BPAoscillationApp\CodeForProject2\DataReaderCode\ConfigXML\RerunTest.xml'
% RerunDetector = 'OutOfRangeGeneral';

function RerunOut = RerunOutOfRange(RerunStartTime,RerunEndTime,ConfigFile,ControlPath,EventPath,InitializationPath,FileDirectory)

% Rerun the general out-of-range detector for the specified time period
[~, AdditionalOutputRerun] = BAWS_main9(ControlPath,EventPath,InitializationPath,FileDirectory,ConfigFile, RerunStartTime, RerunEndTime, 'OutOfRangeGeneral');

% If the rerun did not return any results return an empty structure.
if isempty(AdditionalOutputRerun)
    init = cell(1,0);
    RerunOut = struct('t',init,'Data',init,'DurationMaxMat',init, 'DurationMinMat',init, 'OutsideCount',init,...
        'RateOfChangeMaxMat',init, 'RateOfChangeMinMat',init, 'Rate',init, 'Duration',init, 'RateOfChange',init,...
        'DataPMU',init, 'DataChannel',init,'DataType',init, 'DataUnit',init);
    
    return
end
%
% Initialize the output structure
% Find out the number of general out-of-range detectors
Ndet = 0;
for DetIdx = 1:length(AdditionalOutputRerun{1})
    if isfield(AdditionalOutputRerun{1},'OutOfRangeGeneral')
        if ~isempty(AdditionalOutputRerun{1}(DetIdx).OutOfRangeGeneral)
            Ndet = Ndet + 1;
        else
            break;
        end
    else
        break;
    end
end
init = cell(1,Ndet);
RerunOut = struct('t',init,'Data',init,'DurationMaxMat',init, 'DurationMinMat',init, 'OutsideCount',init,...
    'RateOfChangeMaxMat',init, 'RateOfChangeMinMat',init, 'Rate',init, 'Duration',init, 'RateOfChange',init,...
    'DataPMU',init, 'DataChannel',init);

RerunStartTimeDT = datetime(RerunStartTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
RerunEndTimeDT = datetime(RerunEndTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
% For each general out-of-range detector
for DetIdx = 1:Ndet
    % For each entry in AdditionalOutputRerun, which correspond to 
    % files of PMU data, store the relevant values
    for FileIdx = 1:length(AdditionalOutputRerun)
        RerunOut(DetIdx).t = [RerunOut(DetIdx).t; datetime(AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).TimeString,'InputFormat','yyyy-MM-dd HH:mm:ss.SSS')];

        RerunOut(DetIdx).Data = [RerunOut(DetIdx).Data; AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).Data];

        RerunOut(DetIdx).DurationMaxMat = [RerunOut(DetIdx).DurationMaxMat; AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).DurationMaxMat];
        RerunOut(DetIdx).DurationMinMat = [RerunOut(DetIdx).DurationMinMat; AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).DurationMinMat];
        RerunOut(DetIdx).OutsideCount = [RerunOut(DetIdx).OutsideCount; AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).OutsideCount];

        RerunOut(DetIdx).RateOfChangeMaxMat = [RerunOut(DetIdx).RateOfChangeMaxMat; AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).RateOfChangeMaxMat];
        RerunOut(DetIdx).RateOfChangeMinMat = [RerunOut(DetIdx).RateOfChangeMinMat; AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).RateOfChangeMinMat];

        RateTemp = NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).RateOfChangeMaxMat));
        if iscell(AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).Rate)
            for chan = 1:length(AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).DataChannel)
                RateTemp(AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).ExtremeLocs{chan}(2:end),chan) = AdditionalOutputRerun{FileIdx}(DetIdx).OutOfRangeGeneral(1).Rate{chan};
            end
            RerunOut(DetIdx).Rate = [RerunOut(DetIdx).Rate; RateTemp];
        else
            RerunOut(DetIdx).Rate = NaN;
        end
    end

    % Limit times to those specified by the inputs
    KeepIdx = (RerunStartTimeDT <= RerunOut(DetIdx).t) & (RerunOut(DetIdx).t <= RerunEndTimeDT);
    RerunOut(DetIdx).t = datenum(RerunOut(DetIdx).t(KeepIdx));
    RerunOut(DetIdx).Data = RerunOut(DetIdx).Data(KeepIdx,:);
    RerunOut(DetIdx).DurationMaxMat = RerunOut(DetIdx).DurationMaxMat(KeepIdx,:);
    RerunOut(DetIdx).DurationMinMat = RerunOut(DetIdx).DurationMinMat(KeepIdx,:);
    if size(RerunOut(DetIdx).OutsideCount,1) == length(KeepIdx)
        RerunOut(DetIdx).OutsideCount = RerunOut(DetIdx).OutsideCount(KeepIdx,:);
    end
    RerunOut(DetIdx).RateOfChangeMaxMat = RerunOut(DetIdx).RateOfChangeMaxMat(KeepIdx,:);
    RerunOut(DetIdx).RateOfChangeMinMat = RerunOut(DetIdx).RateOfChangeMinMat(KeepIdx,:);
    if size(RerunOut(DetIdx).Rate,1) == length(KeepIdx)
        RerunOut(DetIdx).Rate = RerunOut(DetIdx).Rate(KeepIdx,:);
    end

    % Add parameters that are constant across time
    RerunOut(DetIdx).Duration = AdditionalOutputRerun{1}(DetIdx).OutOfRangeGeneral(1).Duration;
    RerunOut(DetIdx).RateOfChange = AdditionalOutputRerun{1}(DetIdx).OutOfRangeGeneral(1).RateOfChange;
    RerunOut(DetIdx).DataPMU = AdditionalOutputRerun{1}(DetIdx).OutOfRangeGeneral(1).DataPMU;
    RerunOut(DetIdx).DataChannel = AdditionalOutputRerun{1}(DetIdx).OutOfRangeGeneral(1).DataChannel;
    RerunOut(DetIdx).DataType = AdditionalOutputRerun{1}(DetIdx).OutOfRangeGeneral(1).DataType;
    RerunOut(DetIdx).DataUnit = AdditionalOutputRerun{1}(DetIdx).OutOfRangeGeneral(1).DataUnit;
end