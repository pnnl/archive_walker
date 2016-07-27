% function ConcateStruct = ConcatenatePMU(DataProcessorStruct)
%This function concatenats different PMU structure  sent. The dimension of
%PMU structure to be concatenated should be same.
%Inputs:
        % DataProcessStruct: Strucutre consisting of different PMU
        % structure to be concatenated Dimension: 1 by number of PMU
        % strucutres to be concatenated
%       % secondsNeeds: time duration of needed PMU data in seconds
% Output:
        % ConcateStruct: Concatenated PMU Strucutre consisting of one PMU
        % structure
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 06/22/2016
%
% Modified on June 24, 2016 by Tao Fu
%   modified to use a PMU strucuture cell list as input 
%
% Modified on July 11, 2016 by Tao Fu
%   deleted the added part when NUMPMUstrucutre = 1 to match output data
%   strucuter format
% Modified on July 26, 2016 by Tao Fu
%   Added secondsNeeded as an input
%

function PMU= ConcatenatePMU(DataProcessorStruct,secondsNeeded)
NumPMUstruct = length(DataProcessorStruct);

NumPMU = length(DataProcessorStruct{1});
%% Concatenating PMU structure
for PMUind = 1:NumPMU
    [NData,NChan,NumFlags]= size(DataProcessorStruct{1}(PMUind).Flag); %gives number of data points, number of data channels and number of flags
    % Initializing concatenated PMU
    PMU(PMUind).PMU_Name = DataProcessorStruct{1}(PMUind).PMU_Name;   % PMU name
    PMU(PMUind).Time_Zone = DataProcessorStruct{1}(PMUind).Time_Zone;         % time zone; for now this is just the PST time
    PMU(PMUind).Signal_Name = DataProcessorStruct{1}(PMUind).Signal_Name;
    PMU(PMUind).Signal_Type = DataProcessorStruct{1}(PMUind).Signal_Type;
    PMU(PMUind).Signal_Unit = DataProcessorStruct{1}(PMUind).Signal_Unit;
    PMU(PMUind).Stat = zeros(NumPMUstruct*NData,1);
    PMU(PMUind).Data = zeros(NumPMUstruct*NData,NChan);
    PMU(PMUind).Flag = false(NumPMUstruct*NData,NChan,NumFlags); %flag matrix
    PMU(PMUind).Signal_Time.Time_String = cell(NumPMUstruct*NData,1);
    PMU(PMUind).Signal_Time.Signal_datenum = zeros(1,NumPMUstruct*NData);
    for PMUStructInd = 1:NumPMUstruct
        
        % If the size of data file to be concatenated is different, throws
        % error and program exits
        if size(DataProcessorStruct{PMUStructInd}(PMUind).Data) ~= [NData,NChan]
            error('Structures to be concatenated are of different sizes.');
        end
        PMU(PMUind).Signal_Time.Time_String((PMUStructInd-1)*NData+1:PMUStructInd*NData) = DataProcessorStruct{PMUStructInd}(PMUind).Signal_Time.Time_String;
        PMU(PMUind).Signal_Time.Signal_datenum((PMUStructInd-1)*NData+1:PMUStructInd*NData) = DataProcessorStruct{PMUStructInd}(PMUind).Signal_Time.Signal_datenum;
        PMU(PMUind).Stat((PMUStructInd-1)*NData+1:PMUStructInd*NData,1) = DataProcessorStruct{PMUStructInd}(PMUind).Stat;
        PMU(PMUind).Data((PMUStructInd-1)*NData+1:PMUStructInd*NData,:) = DataProcessorStruct{PMUStructInd}(PMUind).Data;
        PMU(PMUind).Flag((PMUStructInd-1)*NData+1:PMUStructInd*NData,:,:) = DataProcessorStruct{PMUStructInd}(PMUind).Flag;
    end
end
%Sorts PMU data based on datenum value and also removes duplicate value
[PMU(PMUind).Signal_Time.Signal_datenum,Ind] = unique(sort(PMU(PMUind).Signal_Time.Signal_datenum,'ascend')); %sorts in ascending order with respect to time array and then removes duplicate rows
PMU(PMUind).Signal_Time.Time_String = PMU(PMUind).Signal_Time.Time_String(Ind);
PMU(PMUind).Stat = PMU(PMUind).Stat(Ind);
PMU(PMUind).Data = PMU(PMUind).Data(Ind,:);
PMU(PMUind).Flag = PMU(PMUind).Flag(Ind,:,:);

%% check and extract needed time length
for i = 1:length(PMU)
    % for each PMU
    currPMU = PMU(i);
    t = currPMU.Signal_Time.Signal_datenum; % signal time
    deltaT = t(end)-t(end-1); % time interval
    T_end = t(end)+deltaT; % ending time of PMU;
    dt = T_end-t;
    dt = dt*24*3600; % day to seconds
    if(dt(1) > secondsNeeded)
        % has more data than we needed
        % remove some rounding errors
        dt = round(dt*1000)/1000;
        % select needed time 
        k = find(dt <= secondsNeeded);
        k1 = k(1); % starting index
        currPMU.Signal_Time.Signal_datenum = currPMU.Signal_Time.Signal_datenum(k1:end);
        currPMU.Signal_Time.Time_String = currPMU.Signal_Time.Time_String(k1:end);
        currPMU.Stat = currPMU.Stat(k1:end,:);
        currPMU.Data = currPMU.Data(k1:end,:);
        currPMU.Flag = currPMU.Flag(k1:end,:);
        PMU(i) = currPMU;
    end
    
end
    

end



