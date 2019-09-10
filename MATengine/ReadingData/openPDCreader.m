function [PMU,tPMU,fs] = openPDCreader(StartTime,Num_Flags,FileLength,preset,PresetFile)

EndTime = StartTime + FileLength/(60*60*24);

time_start = datestr(StartTime,'yyyy-mm-dd HH:MM:SS');
time_end = datestr(EndTime,'yyyy-mm-dd HH:MM:SS');

%% Read the PI presets

PresetInfo = ReadOpenPDCpresets(PresetFile);

%% Choose a preset

% Reduce to just the selected preset
KeepIdx = strcmp({PresetInfo.name},preset);
PresetInfo = PresetInfo(KeepIdx);

if isempty(PresetInfo)
    error(['Preset ' preset ' not found in ' PresetFile]);
end

Archive = PresetInfo.Signal(1).Archive;
IDtemp = {PresetInfo.Signal.ID};
if length(IDtemp) > 1
    ID = cell(1,2*length(IDtemp)-1);
    ID(1:2:end) = IDtemp;
    ID(2:2:end) = {','};
    ID = [ID{:}];
else
    ID = IDtemp{1};
end

PMUlist = {PresetInfo.Signal.PMU};
fsList = str2double({PresetInfo.Signal.fs});

%% Load GetHistorianData from C# dll.
% Note that The ability to unload an assembly is not available in MATLAB,
% restart MATLAB to release the assembly.
if isdeployed
    % The GUI is calling the function, so point to the matlabDLLs folder
    % within the GUI folder
%     [~,result] = system('path');
%     SourceDir = char(regexpi(result, 'Path=(.*?);', 'tokens', 'once'));
%     dllpath  = [SourceDir '\matlabDLLs\OH\ReadHistorian.dll']; % Full pathname is required
    
    dllpath = [pwd '\matlabDLLs\openPDC\ReadPDC.dll'];
else
    % The function is being called from a Matlab session, so the path must
    % be specified.
    dllpath  = 'C:\Users\foll154\OneDrive - PNNL\Documents\BPAoscillationApp\AWrepository\MATengine\matlabDLLs\openPDC\ReadPDC.dll'; % Full pathname is required
end
try
    asmInfo  = NET.addAssembly(dllpath); % Make .NET assembly visible to MATLAB
catch
    error(['Adding ReadHistorian.dll unsuccessful. Ensure ' dllpath ' is valid.']);
end

%%

datacsv = '.\datalist.csv';
ReadPDC.GetPDCmeasurement.GetHistorianData(datacsv, Archive, ID, time_start, time_end);
DataTable = readtable(datacsv,'Delimiter',',','Format','%d %s %f');
delete datalist.csv datalist_RecordList.csv

%%

IDtemp = str2double(IDtemp);

PMUu = unique(PMUlist);
PMUuPreset = strcat(PMUu,['_' preset]);
MT = cell(1,length(PMUuPreset));
PMU = struct('PMU_Name',PMUuPreset,'Signal_Name',MT,'Signal_Type',MT,'Signal_Unit',MT,'Signal_Time',MT,'Data',MT);
PMUtemp = struct('Data',MT,'Time',MT,'fs',MT);
time_offset = [];
for PMUidx = 1:length(PMU)
    SigIdx = find(strcmp(PMUu{PMUidx},PMUlist));
    
    PMU(PMUidx).Signal_Name = {PresetInfo.Signal(SigIdx).Signal_Name};
    PMU(PMUidx).Signal_Type = {PresetInfo.Signal(SigIdx).Signal_Type};
    PMU(PMUidx).Signal_Unit = {PresetInfo.Signal(SigIdx).Signal_Unit};
    
    PMUtemp(PMUidx).fs = fsList(SigIdx);
    
    PMUtemp(PMUidx).Data = cell(1,length(SigIdx));
    PMUtemp(PMUidx).Time = cell(1,length(SigIdx));
    for k = 1:length(SigIdx)
        ThisSigIdx = SigIdx(k);
        
        ThisDataTable = DataTable(DataTable.HistorianID == IDtemp(ThisSigIdx),:);
        
        if size(ThisDataTable,1) < 2
            % No data was loaded
            
            PMUtemp(PMUidx).Data{k} = [];
            PMUtemp(PMUidx).Time{k} = NaN;
            
            continue;
        end
        
        % One extra sample is always retrieved, so remove it
        ThisDataTable(end,:) = [];
        
        PMUtemp(PMUidx).Data{k} = ThisDataTable.Value;
        
        PMUtemp(PMUidx).Time{k} = datenum(ThisDataTable.Time,'yyyy-mm-dd HH:MM:SS.FFF');
        
        if isempty(time_offset)
            time_offset = round((PMUtemp(PMUidx).Time{k}(1) - StartTime)*24);
        end
    end
end


fs = unique([PMUtemp.fs]);
if length(fs) > 1
    error('All signals in a preset must have the same sampling rate');
end
tPMU = StartTime + (0:1/fs:FileLength)'/86400;
tPMU = tPMU(1:end-1);
N = length(tPMU);

for PMUidx = 1:length(PMU)
    for idx = 1:length(PMU(PMUidx).Signal_Name)
        if N == length(PMUtemp(PMUidx).Data{idx})
            % Has the right number of samples, skip to the next one
            continue;
        elseif isempty(PMUtemp(PMUidx).Data{idx})
            % Signal was not returned from openPDC (data is missing), so
            % set the signal to a vector of NaNs and skip to next signal
            PMUtemp(PMUidx).Data{idx} = NaN(N,1);
            continue;
        end

        % Wrong number of samples - need to identify missing with NaN
        try
            % Remove any duplicate time stamps
            [~,uidx] = unique(PMUtemp(PMUidx).Time{idx});
            DupIdx = setdiff(1:length(PMUtemp(PMUidx).Time{idx}),uidx);
            PMUtemp(PMUidx).Time{idx}(DupIdx) = [];
            PMUtemp(PMUidx).Data{idx}(DupIdx) = [];
            
            % Identify missing samples at the beginning
            NumNanToAdd = near(PMUtemp(PMUidx).Time{idx}(1),tPMU)-1;
            PMUtemp(PMUidx).Time{idx} = [tPMU(1:NumNanToAdd); PMUtemp(PMUidx).Time{idx}];
            PMUtemp(PMUidx).Data{idx} = [NaN(NumNanToAdd,1); PMUtemp(PMUidx).Data{idx}];

            % Look for jumps larger than 1.5 times the sampling interval
            JumpIdx = find(diff(PMUtemp(PMUidx).Time{idx})*86400 > 1.5*1/fs);
            while ~isempty(JumpIdx)
                NewIdx = near(PMUtemp(PMUidx).Time{idx}(JumpIdx(1)+1),tPMU);

                PMUtemp(PMUidx).Time{idx} = [PMUtemp(PMUidx).Time{idx}(1:JumpIdx(1)); tPMU(JumpIdx(1)+1:NewIdx-1); PMUtemp(PMUidx).Time{idx}(JumpIdx(1)+1:end)];
                PMUtemp(PMUidx).Data{idx} = [PMUtemp(PMUidx).Data{idx}(1:JumpIdx(1)); NaN(length(JumpIdx(1)+1:NewIdx-1),1); PMUtemp(PMUidx).Data{idx}(JumpIdx(1)+1:end)];

                JumpIdx = find(diff(PMUtemp(PMUidx).Time{idx})*86400 > 1.5*1/fs);
            end

            % Identify missing samples at the end
            NumNanToAdd = N - near(PMUtemp(PMUidx).Time{idx}(end),tPMU);
            PMUtemp(PMUidx).Time{idx} = [PMUtemp(PMUidx).Time{idx}; tPMU(end-NumNanToAdd+1:end)];
            PMUtemp(PMUidx).Data{idx} = [PMUtemp(PMUidx).Data{idx}; NaN(NumNanToAdd,1)];
        catch
            % Attempt to identify missing data failed, so set entire signal to
            % NaN
            warning(['Attempt to set missing data in ' PMU(PMUidx).Signal_Name{idx} ' to NaN failed. Setting all values to NaN.']);
            PMUtemp(PMUidx).Data{idx} = NaN(N,1);
        end
    end
end

%% Build final PMU structure
time_offsetSign = sign(time_offset);
time_offset = time(caldays(0) + hours(abs(time_offset)));
time_offset.Format = 'hh:mm';
time_offset = char(time_offset);
if time_offsetSign == -1
    time_offset = ['-' time_offset];
end

for PMUidx = 1:length(PMU)
    PMU(PMUidx).Signal_Time.Time_String = cellstr(datestr(tPMU,'yyyy-mm-dd HH:MM:SS.FFF'));
    PMU(PMUidx).Signal_Time.Signal_datenum = tPMU;
    PMU(PMUidx).Signal_Time.datetime = datetime(tPMU,'ConvertFrom','datenum','Format','MM/dd/yy HH:mm:ss.SSSSSS');
    PMU(PMUidx).Data = zeros(length(PMUtemp(PMUidx).Data{1}),length(PMUtemp(PMUidx).Data));
    for idx = 1:length(PMUtemp(PMUidx).Data)
        PMU(PMUidx).Data(:,idx) = PMUtemp(PMUidx).Data{idx};
    end

    PMU(PMUidx).File_Name = PresetFile;
    PMU(PMUidx).Stat = zeros(size(PMU(PMUidx).Data,1),1);
    PMU(PMUidx).Flag = false(size(PMU(PMUidx).Data,1),size(PMU(PMUidx).Data,2),Num_Flags);
    PMU(PMUidx).Time_Zone = time_offset;
end