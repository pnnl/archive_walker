% function RerunOut = RerunRingdown(RerunStartTime,RerunEndTime,InitializationPath)
%
% This function reruns the ringdown detector so that detailed
% results can be displayed in the GUI.
%
% Inputs:
% RerunStartTime = start time for the run in the format MM/DD/YYYY HH:MM:SS
% RerunEndTime = end time for the run in the format MM/DD/YYYY HH:MM:SS
% InitializationPath = path to the folder containing initialization files
%
% Example inputs:
% RerunStartTime = '08/21/2016 23:55:30';
% RerunEndTime = '08/22/2016 00:03:52';
% ConfigFile = 'C:\Users\foll154\Documents\BPAoscillationApp\CodeForProject2\DataReaderCode\ConfigXML\RerunTest.xml'

function RerunOut = RerunRingdown(RerunStartTime,RerunEndTime,ConfigFile,ControlPath,EventPath,InitializationPath,FileDirectory)

RerunDetector = 'Ringdown';

% Rerun the general out-of-range detector for the specified time period
[~, AdditionalOutputRerun] = BAWS_main9(ControlPath,EventPath,InitializationPath,FileDirectory,ConfigFile, RerunStartTime, RerunEndTime, RerunDetector);

% If the rerun did not return any results return an empty structure.
if isempty(AdditionalOutputRerun)
    % The rerun did not return any results
    init = cell(1,0);
    RerunOut = struct('t',init,'Data',init,'RMS',init, 'Threshold',init,'DataPMU',init, 'DataChannel',init,'DataType',init, 'DataUnit',init);
    
    return
end
%
% Initialize the output structure
% Find out the number of general out-of-range detectors
Ndet = 0;
for DetIdx = 1:length(AdditionalOutputRerun{1})
    if isfield(AdditionalOutputRerun{1},RerunDetector)
        if ~isempty(AdditionalOutputRerun{1}(DetIdx).(RerunDetector))
            Ndet = Ndet + 1;
        else
            break;
        end
    else
        break;
    end
end
init = cell(1,Ndet);
RerunOut = struct('t',init,'Data',init,'RMS',init, 'Threshold',init,'DataPMU',init, 'DataChannel',init);

RerunStartTimeDT = datetime(RerunStartTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
RerunEndTimeDT = datetime(RerunEndTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
% For each general out-of-range detector
for DetIdx = 1:Ndet
    % For each entry in AdditionalOutputRerun, which correspond to 
    % files of PMU data, store the relevant values
    for FileIdx = 1:length(AdditionalOutputRerun)
        RerunOut(DetIdx).t = [RerunOut(DetIdx).t; datetime(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).TimeString,'InputFormat','yyyy-MM-dd HH:mm:ss.SSS')];

        RerunOut(DetIdx).Data = [RerunOut(DetIdx).Data; AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data];
        
        RMStemp = [];
        ThresholdTemp = [];
        for ChanIdx = 1:length(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector))      
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(ChanIdx).RMS)
                RMStemp = [RMStemp AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(ChanIdx).RMS];
            else
                RMStemp = [RMStemp NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
            
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(ChanIdx).threshold)
                ThresholdTemp = [ThresholdTemp AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(ChanIdx).threshold];
            else
                ThresholdTemp = [ThresholdTemp NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
        end
        RerunOut(DetIdx).RMS = [RerunOut(DetIdx).RMS; RMStemp];       
        RerunOut(DetIdx).Threshold = [RerunOut(DetIdx).Threshold; ThresholdTemp];
    end

    % Limit times to those specified by the inputs
    KeepIdx = (RerunStartTimeDT <= RerunOut(DetIdx).t) & (RerunOut(DetIdx).t <= RerunEndTimeDT);
    RerunOut(DetIdx).t = datenum(RerunOut(DetIdx).t(KeepIdx));
    RerunOut(DetIdx).Data = RerunOut(DetIdx).Data(KeepIdx,:);
    RerunOut(DetIdx).RMS = RerunOut(DetIdx).RMS(KeepIdx,:);
    RerunOut(DetIdx).Threshold = RerunOut(DetIdx).Threshold(KeepIdx,:);

    % Add parameters that are constant across time
    RerunOut(DetIdx).DataPMU = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataPMU;
    RerunOut(DetIdx).DataChannel = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataChannel;
    RerunOut(DetIdx).DataType = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataType;
    RerunOut(DetIdx).DataUnit = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataUnit;
end