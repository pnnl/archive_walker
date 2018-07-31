% function PMU = InitCustomPMU(PMU,FileName,Time_Zone,Signal_Time,N,Num_Flags)
% This function initializes the custom PMU sub-structure and adds it to the PMU structure
% using some of the fields from an existing PMU sub-structure.
% 
% Inputs:
	% PMU: struct array of dimension 1 by Number of PMUs
        % PMU(i).File_Name: a string specifying file name containing i^th PMU data        
        % PMU(i).PMU_Name: a string specifying name of i^th PMU
        % PMU(i).Time_zone:  a string specifying time-zone from where data are recorded
        % PMU(i).Signal_Time: a string specifying time stamp of i^th PMU data
        % PMU(i).Signal_Name: a cell array of strings specifying name of Signals in i^th PMU
        % PMU(i).Signal_Type: a cell array of strings specifying type of Signals in i^th PMU
        % PMU(i).Signal_Unit: a cell array of strings specifying unit of Signals in i^th PMU
        % PMU(i).Stat: Numerical array specifying status of i^th PMU
        % PMU(i).Data: Numerical matrix containing data measured by the i^th PMU
        % PMU(i).Flag: 3-dimensional matrix providing information on the
        % i^th PMU data that are flagged by different filters
    % N: Number of data points for each signal in i^th PMU
    % Num_Flags: Number of flag bits
% 
% Outputs:
    % PMU: 
%     
%Created by: 
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov): Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix

function PMU = InitCustomPMU(PMU,PMUname,Signal_Time,Num_Flags)

next = length(PMU)+1;

N = length(Signal_Time.Signal_datenum);

PMU(next).File_Name = 'Custom';   % file name
PMU(next).PMU_Name = PMUname;   % PMU name
PMU(next).Time_Zone = 'Custom';         % time zone; for now this is just the PST time 
PMU(next).Signal_Time = Signal_Time;
PMU(next).Signal_Name = cell(1,0);
PMU(next).Signal_Type = cell(1,0);
PMU(next).Signal_Unit = cell(1,0);
PMU(next).Stat = zeros(N,1);
PMU(next).Data = zeros(N,0);
PMU(next).Flag = false(N,0,Num_Flags); %flag matrix