% [PMUsegment,UpdatedPMU, FlagBreak] = ExtractPMUsegment(PMU,secondsNeeded,ResultUpdateInterval)
%This function breaks PMU data into two structure, one containing data to
%be processed given by PMUsegment, and other containing data available for
%next processing
%Inputs:
        % PMU: Strucutre consisting of different PMU
        % structure to be concatenated Dimension: 1 by number of PMU
        % secondsNeeded: time duration of needed PMU data in seconds
        % ResultUpdatInterval: User-defined input for time interval for
        % updating results
% Output:
        % PMUsegment: Extracted segement of PMU structure for further processing and applying detector
        % UpdatedPMU: Segement of PMU structure obtained after removing data
        % corresponding to time ResultUpdateInterval. It contains data
        % available for next processing        
        % FlagBreak: Incase UpdatedPMU do not have enough data for next
        % processing, its value is set to one. In main script file, it
        % indicates that more data files is needed.
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 08/24/2016

function [PMUsegment, UpdatedPMU, FlagBreak] = ExtractPMUsegment(PMU,secondsNeeded,ResultUpdateInterval)

%% check and extract needed time length
for i = 1:length(PMU)
    % for each PMU
    currPMU = PMU(i);
    PMUsegment(i)= currPMU;
    UpdatedPMU(i) = currPMU;
    
    t = currPMU.Signal_Time.Signal_datenum; % signal time
    t = t - t(1); %start time is changed to zero
    t = round(t*24*3600*1000)/1000; % day to seconds

    PMUsegmentIndex = find(t < secondsNeeded);
    PMUupdatedIndex = find(t >= ResultUpdateInterval);
    if ~isempty(PMUupdatedIndex)
        PMUupdatedStartingIndex = PMUupdatedIndex(1); % starting index
        UpdatedPMU(i).Signal_Time.Signal_datenum = currPMU.Signal_Time.Signal_datenum(PMUupdatedStartingIndex:end);
        UpdatedPMU(i).Signal_Time.Time_String = currPMU.Signal_Time.Time_String(PMUupdatedStartingIndex:end);
        UpdatedPMU(i).Stat = currPMU.Stat(PMUupdatedStartingIndex:end,:);
        UpdatedPMU(i).Data = currPMU.Data(PMUupdatedStartingIndex:end,:);
        UpdatedPMU(i).Flag = currPMU.Flag(PMUupdatedStartingIndex:end,:,:);
    else
        %This section is for the case when no data is available for next
        %processing
        UpdatedPMU(i).Signal_Time.Signal_datenum = [];
        UpdatedPMU(i).Signal_Time.Time_String = cell(0);
        UpdatedPMU(i).Stat = [];
        UpdatedPMU(i).Data = [];
        UpdatedPMU(i).Flag = [];
    end    
    
    PMUsegment(i).Signal_Time.Signal_datenum = currPMU.Signal_Time.Signal_datenum(PMUsegmentIndex);
    PMUsegment(i).Signal_Time.Time_String = currPMU.Signal_Time.Time_String(PMUsegmentIndex);
    PMUsegment(i).Stat = currPMU.Stat(PMUsegmentIndex,:);
    PMUsegment(i).Data = currPMU.Data(PMUsegmentIndex,:);
    PMUsegment(i).Flag = currPMU.Flag(PMUsegmentIndex,:,:);    
    
end

%Checks if UpdatedPMU has enough data for next processing or not
t = UpdatedPMU(1).Signal_Time.Signal_datenum; % signal time
if length(t) >=2 % update interval should contain at least 2 samples
    deltaT = t(end)-t(end-1); % time interval
    T_end = t(end)+deltaT; % ending time of PMU;
    dt = T_end-t;
    dt = dt*24*3600; % day to seconds
    dt = round(dt*1000)/1000;
    if (dt(1) < secondsNeeded)
        FlagBreak =1;
    else
        FlagBreak = 0;
    end
else
    FlagBreak =1;
end



