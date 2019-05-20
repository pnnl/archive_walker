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

function PMU = GetFileExample(InputFile,FileType,MetaOnly)

try
    Unsupported = 0;
    if FileType == 1
        [PMU,~,fs] = pdatReadnCreateStruct(InputFile,0,[]);
    elseif FileType == 2
        [PMU,~,fs] = JSIS_CSV_2_Mat(InputFile,0);
    elseif FileType == 3
        [PMU,~,fs] = POWreadHQ(InputFile,0);
    else
        Unsupported = 1;
        error(['FileType = ' num2str(FileType) ' is not a supported value.']);
    end
catch e
    if Unsupported == 1
        % Repeat error so that the message is captured by the GUI
        throw(MException('Engine:Error',['FileType = ' num2str(FileType) ' is not a supported value.']));
    else
        throw(MException('Engine:Error','Attempt to read the file failed. It may be corrupt or the wrong format. Error message: \n%s',e.message));
    end
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