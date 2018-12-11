% function PMU = RetrieveData(RerunStartTime,RerunEndTime,ConfigFile,ControlPath,EventPath,InitializationPath,FileDirectory)
%
% This is one of the top-level functions intended to be called by the GUI.
% It reruns a portion of a previously run task and returns all of the
% signals generated. It does not include detection results, only signals
% that the user creates in the configuration are returned. This function is
% a wrapper for BAWS_main9.
%
% Called by: 
%   The AW GUI
%
% Calls: 
%   BAWS_main9
%
% Inputs:
%   RerunStartTime - String specifying the start time for the run in the
%       format MM/DD/YYYY HH:MM:SS.SSS
%   RerunEndTime - String specifying the end time for the run in the format
%       MM/DD/YYYY HH:MM:SS.SSS
%   ConfigFile - Path to the configuration XML used to configure the AW
%       engine for a run.
%   ControlPath - Path to folders containing Run.txt and Pause.txt files
%       written by the GUI to control the AW engine. A string.
%   EventPath - Path to the folder where results from detectors are to be
%       stored. A string.
%   InitializationPath - Path to the folder where initialization files
%       (used in rerun mode to recreate detailed results) and sparse data
%       (max and min of analyzed signals) are stored. A string.
%   FileDirectory - Paths to where PMU data that is to be analyzed is
%       stored. Cell array of strings.
%
% Outputs:
%   PMU - a structure array matching the configuration of the PMU
%       structures used within the tool.

function PMU = RetrieveData(RerunStartTime,RerunEndTime,ConfigFile,ControlPath,EventPath,InitializationPath,FileDirectory)

RerunStartTimeDT = datetime(RerunStartTime,'InputFormat','MM/dd/yyyy HH:mm:ss.SSS');
RerunEndTimeDT = datetime(RerunEndTime,'InputFormat','MM/dd/yyyy HH:mm:ss.SSS');

RerunStartTimeRound = datestr(dateshift(RerunStartTimeDT,'start','second'),'mm/dd/yyyy HH:MM:SS');
RerunEndTimeRound = datestr(dateshift(RerunEndTimeDT,'end','second'),'mm/dd/yyyy HH:MM:SS');

RerunDetector = 'RetrieveMode';
[~, ~, PMU] = BAWS_main9(ControlPath,EventPath,InitializationPath,FileDirectory,ConfigFile, RerunStartTimeRound, RerunEndTimeRound, RerunDetector);

PMU = rmfield(PMU,{'File_Name','Time_Zone'});
for idx = 1:length(PMU)
    PMU(idx).PMU_Name = {PMU(idx).PMU_Name};
    
    PMU(idx).fs = round((length(PMU(idx).Signal_Time.datetime)-1)/seconds(diff(PMU(idx).Signal_Time.datetime([1 end]))));
    
    KeepIdx = find(PMU(idx).Signal_Time.datetime >= RerunStartTimeDT, 1) : find(PMU(idx).Signal_Time.datetime <= RerunEndTimeDT, 1, 'last');
    %
    PMU(idx).Data = PMU(idx).Data(KeepIdx,:);
    PMU(idx).Stat = PMU(idx).Stat(KeepIdx,:);
    PMU(idx).Flag = PMU(idx).Flag(KeepIdx,:,:);
    PMU(idx).Signal_Time.Time_String = PMU(idx).Signal_Time.Time_String(KeepIdx);
    PMU(idx).Signal_Time.Signal_datenum = PMU(idx).Signal_Time.Signal_datenum(KeepIdx);
    PMU(idx).Signal_Time.datetime = PMU(idx).Signal_Time.datetime(KeepIdx);
    
    PMU(idx).Signal_Time = PMU(idx).Signal_Time.Signal_datenum;
end