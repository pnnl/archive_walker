function [PMU,tPMU,SampleRate] = POWreadHQ(inFile,Num_Flags)

[signalData,SampleRate,signalNames,signalTypes,signalUnits,~,t] = GetHQdata(inFile);

tPMU = t/24/3600 + getPdatFileTime(inFile);
timeStr = datestr(tPMU, 'yyyy-mm-dd HH:MM:SS.FFF');

PMU.File_Name = inFile;   % file name

% used file name as the PMU name
k = strfind(inFile,'\');
if(isempty(k))
    pmuName = inFile(1:end-4);
else
    idx = k(end);
    pmuName = inFile(idx+1:end-4);
end

k = strfind(pmuName,'_');
if(~isempty(k))
    currPMUName = pmuName(1:k(1)-1);
end
PMU.PMU_Name = currPMUName;   % PMU name
PMU.Time_Zone = '-08:00';         % time zone; for now this is just the PST time

% signal time
PMU.Signal_Time.Time_String = cellstr(timeStr);
PMU.Signal_Time.Signal_datenum = tPMU;
PMU.Signal_Time.datetime = datetime(tPMU,'ConvertFrom','datenum','Format','MM/dd/yy HH:mm:ss.SSSSSS');

% variable names
PMU.Signal_Name = signalNames;

% Signal_Type and Signal_Unit
PMU.Signal_Type = signalTypes;
PMU.Signal_Unit = signalUnits;



% flag
[m,n] = size(signalData);
Flag = false(m,n,Num_Flags);
PMU.Flag = Flag;

% PMU data and Stat
PMU.Data = signalData;
PMU.Stat = zeros(m,1);