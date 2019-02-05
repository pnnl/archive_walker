% function PMU = GetPDATexample(pdatFile)
%
% This is one of the top-level functions intended to be called by the GUI.
% It returns a structure containing the names of PMUs and their signals
% contained in the example pdat file that is passed as an input. This
% allows the GUI to display the PMUs and signals when setting up a task.
% When the input MetaOnly~=1, it also returns the measurements,
% statuses, and flags for each PMU.
%
% Called by: 
%   The AW GUI
%
% Calls: 
%   pdatReadnCreateStruct
%
% Inputs:
%   InputFile - Path to the file that is to be read. String.
%   FileType - 1 for pdat, 2 for JSIS CSV, or 3 for BPA HQ POW
%   MetaOnly - If 1 then only meta data is returned, i.e., no measurements,
%       time stamps, or statuses
%
% Outputs:
%   PMU - Stripped down structure that is the output of the
%       pdatReadnCreateStruct function. 

function PMU = GetFileExampleDB(StartTime,preset,PresetFile,MetaOnly,DBtype)

FileLength = 60;
StartTime = datenum(StartTime,'mm/dd/yyyy HH:MM:SS');
if strcmp(DBtype,'PI')
    [PMU,~,fs] = PIreader(StartTime,0,FileLength,preset,PresetFile);
elseif strcmp(DBtype,'OpenHistorian')
    [PMU,~,fs] = OHreader(StartTime,0,FileLength,preset,PresetFile);
end

if MetaOnly == 1
    PMU = rmfield(PMU,{'Stat','Data','Flag','File_Name','Time_Zone','Signal_Time'});
else
    PMU = rmfield(PMU,{'File_Name','Time_Zone'});
end

fs = num2cell(fs*ones(1,length(PMU)));
[PMU.fs] = fs{:};

for idx = 1:length(PMU)
    PMU(idx).PMU_Name = {PMU(idx).PMU_Name};
    
    if MetaOnly ~= 1
        PMU(idx).Signal_Time = PMU(idx).Signal_Time.Signal_datenum;
    end
end