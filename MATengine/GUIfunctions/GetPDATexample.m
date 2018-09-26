% function PMU = GetPDATexample(pdatFile)
%
% This is one of the top-level functions intended to be called by the GUI.
% It returns a structure containing the names of PMUs and their signals
% contained in the example pdat file that is passed as an input. This
% allows the GUI to display the PMUs and signals when setting up a task.
%
% Called by: 
%   The AW GUI
%
% Calls: 
%   pdatReadnCreateStruct
%
% Inputs:
%   pdatFile - Path to the pdat file that is to be read. String.
%
% Outputs:
%   PMU - Stripped down structure that is the output of the
%       pdatReadnCreateStruct function. 

function PMU = GetPDATexample(pdatFile)

[PMU,~,fs] = pdatReadnCreateStruct(pdatFile,1,[]);

PMU = rmfield(PMU,{'Stat','Data','Flag','File_Name','Time_Zone','Signal_Time'});
PMU(1).fs = fs;

for idx = 1:length(PMU)
    PMU(idx).PMU_Name = {PMU(idx).PMU_Name};
end