% function RerunOut = RerunForcedOscillation(RerunStartTime,RerunEndTime,InitializationPath)
%
% This function reruns the periodogram and spectral-coherence forced
% oscillation detectors so that detailed results can be displayed in the GUI.
%
% Inputs:
% RerunStartTime = start time for the run in the format MM/DD/YYYY HH:MM:SS
% RerunEndTime = end time for the run in the format MM/DD/YYYY HH:MM:SS
% InitializationPath = path to the folder containing initialization files
%
% Example inputs:
% RerunStartTime = '08/21/2016 23:58:05';
% RerunEndTime = '08/22/2016 00:02:35';
% ConfigFile = 'C:\Users\foll154\Documents\BPAoscillationApp\CodeForProject2\DataReaderCode\ConfigXML\RerunTest.xml'

function RerunOut = RerunForcedOscillation(RerunStartTime,RerunEndTime,ConfigFile,ControlPath,EventPath,InitializationPath,FileDirectory)

RerunDetector = 'ForcedOscillation';

% Rerun the general out-of-range detector for the specified time period
[~, AdditionalOutputRerun] = BAWS_main9(ControlPath,EventPath,InitializationPath,FileDirectory,ConfigFile, RerunStartTime, RerunEndTime, RerunDetector);

% If the rerun did not return any results return an empty structure.
if isempty(AdditionalOutputRerun)
    init = cell(1,0);
    RerunOut.Per = struct('t',init,'Data',init,'EvalTimeStamps',init,'TestStatistic',init, 'Threshold',init,'Frequency',init,'DataPMU',init, 'DataChannel',init,'DataType',init, 'DataUnit',init);
    RerunOut.SC = struct('t',init,'Data',init,'EvalTimeStamps',init,'TestStatistic',init, 'Threshold',init,'Frequency',init,'DataPMU',init, 'DataChannel',init,'DataType',init, 'DataUnit',init);
    
    return
end

RerunDetector = 'Periodogram';
% Initialize the output structure
% Find out the number of periodogram detectors
NdetPer = 0;
for DetIdx = 1:length(AdditionalOutputRerun{1})
    if isfield(AdditionalOutputRerun{1},RerunDetector)
        if ~isempty(AdditionalOutputRerun{1}(DetIdx).(RerunDetector))
            NdetPer = NdetPer + 1;
        else
            break;
        end
    else
        break;
    end
end
init = cell(1,NdetPer);
RerunOutPer = struct('t',init,'Data',init,'EvalTimeStamps',init,'TestStatistic',init, 'Threshold',init,'Frequency',init,'DataPMU',init, 'DataChannel',init,'DataType',init, 'DataUnit',init);

% For each general out-of-range detector
for DetIdx = 1:NdetPer
    % Add parameters that are constant across time
    RerunOutPer(DetIdx).Frequency = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).Frequency;
    RerunOutPer(DetIdx).DataPMU = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataPMU;
    RerunOutPer(DetIdx).DataChannel = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataChannel;
    RerunOutPer(DetIdx).DataType = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataType;
    RerunOutPer(DetIdx).DataUnit = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataUnit;
    
    % For each entry in AdditionalOutputRerun, which correspond to 
    % files of PMU data, store the relevant values
    RerunOutPer(DetIdx).TestStatistic = NaN(0,length(AdditionalOutputRerun),length(AdditionalOutputRerun{1}(DetIdx).(RerunDetector)));
    RerunOutPer(DetIdx).Threshold = NaN(0,length(AdditionalOutputRerun),length(AdditionalOutputRerun{1}(DetIdx).(RerunDetector)));
    for UpdateIdx = 1:length(AdditionalOutputRerun)
        RerunOutPer(DetIdx).t = [RerunOutPer(DetIdx).t; datenum(AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(1).TimeString)];
        RerunOutPer(DetIdx).Data = [RerunOutPer(DetIdx).Data; AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(1).Data];
        RerunOutPer(DetIdx).EvalTimeStamps = [RerunOutPer(DetIdx).EvalTimeStamps; datenum(AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(1).TimeString{end})];
        
        for ChanIdx = 1:length(AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector))
            RerunOutPer(DetIdx).TestStatistic(1:length(RerunOutPer(DetIdx).Frequency),UpdateIdx,ChanIdx) = AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(ChanIdx).TestStatistic;
            RerunOutPer(DetIdx).Threshold(1:length(RerunOutPer(DetIdx).Frequency),UpdateIdx,ChanIdx) = AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(ChanIdx).Threshold;
        end
    end
end



RerunDetector = 'SpectralCoherence';
% Initialize the output structure
% Find out the number of Spectral Coherence detectors
NdetSC = 0;
for DetIdx = 1:length(AdditionalOutputRerun{1})
    if isfield(AdditionalOutputRerun{1},RerunDetector)
        if ~isempty(AdditionalOutputRerun{1}(DetIdx).(RerunDetector))
            NdetSC = NdetSC + 1;
        else
            break;
        end
    else
        break;
    end
end
init = cell(1,NdetSC);
RerunOutSC = struct('t',init,'Data',init,'EvalTimeStamps',init,'TestStatistic',init, 'Threshold',init,'Frequency',init,'DataPMU',init, 'DataChannel',init,'DataType',init, 'DataUnit',init);

% For each general out-of-range detector
for DetIdx = 1:NdetSC
    % Add parameters that are constant across time
    RerunOutSC(DetIdx).Frequency = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).Frequency;
    RerunOutSC(DetIdx).DataPMU = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataPMU;
    RerunOutSC(DetIdx).DataChannel = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataChannel;
    RerunOutSC(DetIdx).DataType = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataType;
    RerunOutSC(DetIdx).DataUnit = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataUnit;
    
    % For each entry in AdditionalOutputRerun, which correspond to 
    % files of PMU data, store the relevant values
    RerunOutSC(DetIdx).TestStatistic = NaN(0,length(AdditionalOutputRerun),length(AdditionalOutputRerun{1}(DetIdx).(RerunDetector)));
    RerunOutSC(DetIdx).Threshold = NaN(0,length(AdditionalOutputRerun),length(AdditionalOutputRerun{1}(DetIdx).(RerunDetector)));
    for UpdateIdx = 1:length(AdditionalOutputRerun)
        RerunOutSC(DetIdx).t = [RerunOutSC(DetIdx).t; datenum(AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(1).TimeString)];
        RerunOutSC(DetIdx).Data = [RerunOutSC(DetIdx).Data; AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(1).Data];
        RerunOutSC(DetIdx).EvalTimeStamps = [RerunOutSC(DetIdx).EvalTimeStamps; datenum(AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(1).TimeString{end})];
        
        for ChanIdx = 1:length(AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector))
            RerunOutSC(DetIdx).TestStatistic(1:length(RerunOutSC(DetIdx).Frequency),UpdateIdx,ChanIdx) = AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(ChanIdx).TestStatistic;
            RerunOutSC(DetIdx).Threshold(1:length(RerunOutSC(DetIdx).Frequency),UpdateIdx,ChanIdx) = AdditionalOutputRerun{UpdateIdx}(DetIdx).(RerunDetector)(ChanIdx).Threshold;
        end
    end
end

RerunOut.Per = RerunOutPer;
RerunOut.SC = RerunOutSC;