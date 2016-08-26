% function ConcateStruct = ConcatenatePMU(DataProcessorStruct)
%This function concatenats different PMU structure  sent. Number of Channel
%in PMU data file to be concatenated should be same.
%Inputs:
        % DataProcessStruct: Strucutre consisting of different PMU
        % structure to be concatenated Dimension: 1 by number of PMU
        % strucutres to be concatenated
%       % secondsNeeds: time duration of needed PMU data in seconds
% Output:
        % ConcateStruct: Concatenated PMU Strucutre consisting of one PMU
        % structure
        % FlagContinue: Incase, concatenated data length is less than
        % SecondsNeeded, then thes value of FlagContinue is set to 1
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 06/22/2016
%
% Modified on June 24, 2016 by Tao Fu
%   modified to use a PMU strucuture cell list as input 
%
% Modified on July 11, 2016 by Tao Fu
%   deleted the added part when NUMPMUstrucutre = 1 to match output data
%   strucuter format
%Modified on Jul 22 by Urmila Agrawal
%   Flags are copied from source to destination PMU as it is.
% Modified on July 26, 2016 by Tao Fu
%   Added secondsNeeded as an input
% Modified on July 27 , 2016 by Urmila Agrawal
%   Fixed a minor bug. When checking for needed time length , Flags were
%   copied as 2-dimensional quantity, corrected it.
%
% Modified on August 24, 2016 by Urmila Agrawal
%     Files of different data length can be concatenated(needed to deal data
%     file containing multiple minutes of data, and also implement moving
%     window for updating results after certain time frame)
% 
%     Also, now data are not discarded if the length of concatenated data
%     exceeeds secondsNeeded.
%
%     Introduced a new output, flagcontinue. Incase, concatenated data length
%     is less than SecondsNeeded, then thes value of FlagContinue is set to
%     one. In mainscript file, if FlagContinue = 1, then it does not implement
%     any processing or quality check, and continues to get new data file to
%     concatenate.

function [PMU,FlagContinue] = ConcatenatePMU(DataProcessorStruct,secondsNeeded)
NumPMUstruct = length(DataProcessorStruct);
FlagContinue = 0;

%It's added to accomodate changes for implmenting data processing and
%detection algorithm with moving window, for case when PMURem has no data
%available.
for PMUstructIdx = 1:NumPMUstruct
    NChan= size(DataProcessorStruct{PMUstructIdx}(1).Data,2);
    if NChan ==0
        DataProcessorStruct{PMUstructIdx} = {};
    end
end
%removes empty cell array for PMU struct with no data
DataProcessorStruct = DataProcessorStruct(~cellfun('isempty',DataProcessorStruct));
NumPMUstruct = length(DataProcessorStruct); 
NumPMU = length(DataProcessorStruct{1});
%% Concatenating PMU structure
for PMUind = 1:NumPMU
    %Number of data points in the new PMU strucuture is initialized to zero
    NewData = 0; 
    %gives number of data channels
    NChan= size(DataProcessorStruct{1}(PMUind).Data,2); 
    % Initializing concatenated PMU
    PMU(PMUind).PMU_Name = DataProcessorStruct{1}(PMUind).PMU_Name;   % PMU name
    PMU(PMUind).Time_Zone = DataProcessorStruct{1}(PMUind).Time_Zone;         % time zone; for now this is just the PST time
    PMU(PMUind).Signal_Name = DataProcessorStruct{1}(PMUind).Signal_Name;
    PMU(PMUind).Signal_Type = DataProcessorStruct{1}(PMUind).Signal_Type;
    PMU(PMUind).Signal_Unit = DataProcessorStruct{1}(PMUind).Signal_Unit;
    for PMUStructInd = 1:NumPMUstruct
        % Gives number of data points of the PMU whose data is to concatenated
        NData= size(DataProcessorStruct{PMUStructInd}(PMUind).Data,1); 
        % If the number of channels in a data file to be concatenated does
        % not match for different PMU strucure, throws error and program exits
        if size(DataProcessorStruct{PMUStructInd}(PMUind).Data,2) ~= NChan
            error('Structures to be concatenated are of different sizes.');
        end
        PMU(PMUind).Signal_Time.Time_String(NewData+1:NewData+NData,1) = DataProcessorStruct{PMUStructInd}(PMUind).Signal_Time.Time_String;
        PMU(PMUind).Signal_Time.Signal_datenum(NewData+1:NewData+NData,1) = DataProcessorStruct{PMUStructInd}(PMUind).Signal_Time.Signal_datenum;
        PMU(PMUind).Stat(NewData+1:NewData+NData,1) = DataProcessorStruct{PMUStructInd}(PMUind).Stat;
        PMU(PMUind).Data(NewData+1:NewData+NData,:) = DataProcessorStruct{PMUStructInd}(PMUind).Data;
        PMU(PMUind).Flag(NewData+1:NewData+NData,:,:) = DataProcessorStruct{PMUStructInd}(PMUind).Flag;
        % Updates number of data points after concatenating each PMU
        % structure
        NewData= size(PMU(PMUind).Data,1); 
    end
    %Sorts PMU data based on datenum value and also removes duplicate value
    %(needed for CSV files)
    [PMU(PMUind).Signal_Time.Signal_datenum,Ind] = unique(sort(PMU(PMUind).Signal_Time.Signal_datenum,'ascend')); 
    PMU(PMUind).Signal_Time.Time_String = PMU(PMUind).Signal_Time.Time_String(Ind);
    PMU(PMUind).Stat = PMU(PMUind).Stat(Ind);
    PMU(PMUind).Data = PMU(PMUind).Data(Ind,:);
    PMU(PMUind).Flag = PMU(PMUind).Flag(Ind,:,:);    
end

%Checks if the concatenated data has enough data or not corresponding to
%userdefined input SecondsToConcat. Incase, concatenated data do not have
%enough data, sets FlagContinue to 1.
t = PMU(end).Signal_Time.Signal_datenum; % signal time
deltaT = t(end)-t(end-1); % time interval
T_end = t(end)+deltaT; % ending time of PMU;
dt = T_end-t;
dt = dt*24*3600; % day to seconds
dt = round(dt*1000)/1000;
if (dt(1) < secondsNeeded)
    FlagContinue =1;
end
end



