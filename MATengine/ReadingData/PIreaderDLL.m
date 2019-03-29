function [PMU,tPMU,fs] = PIreaderDLL(StartTime,Num_Flags,FileLength,preset,PresetFile)

%% Load assemblies developed by PNNL to talk to PI
if isdeployed
    % The GUI is calling the function, so point to the matlabDLLs folder
    % within the GUI folder
%     [~,result] = system('path');
%     SourceDir = char(regexpi(result, 'Path=(.*?);', 'tokens', 'once'));
%     dllpath  = [SourceDir '\matlabDLLs\PI\OpenPI.dll']; % Full pathname is required
    
    dllpath = [pwd '\matlabDLLs\PI\OpenPI.dll'];
else
    % The function is being called from a Matlab session, so the path must
    % be specified.
    dllpath  = 'C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\matlabDLLs\PI\OpenPI.dll'; % Full pathname is required
end
try
    asmInfo  = NET.addAssembly(dllpath); % Make .NET assembly visible to MATLAB
catch
    error(['Adding OpenPI.dll unsuccessful. Ensure ' dllpath ' is valid.']);
end

%%

% StartTime and EndTime
EndTime = StartTime + FileLength/(60*60*24);
time_start = datestr(StartTime,'dd-mmm-yyyy HH:MM:SS');
time_end = datestr(EndTime,'dd-mmm-yyyy HH:MM:SS');

%% Read the PI presets and Choose a preset
[PMU,Server] = ReadPIpresets(PresetFile);
PresetIdx = find(strcmp({PMU.PMU_Name},preset));
if isempty(PresetIdx)
    error(['Preset ' preset ' was not found in ' PresetFile]);
end
Server = Server{PresetIdx};
PMU = PMU(strcmp({PMU.PMU_Name},preset));

%% Get the PIData of all signals
tagnames = NET.createArray('System.String',length(PMU.Signal_Name));
for idx = 1:length(PMU.Signal_Name)
    tagnames(idx) = ['\\' Server '\' PMU.Signal_Name{idx}];
end
% Use OpenPI to query data from server
interval = 0;   % No interpolation of points
OpenPI.Model.GetPIPoints(tagnames, time_start, time_end, interval);
ALLDataTable = readtable('Data.csv','Delimiter',',','Format','%d %f %f'); delete('Data.csv');

%% Setup Tag Names
v = cell(1,length(PMU.Signal_Name));
tPMUsig = cell(1,length(PMU.Signal_Name));
fs = zeros(1,length(PMU.Signal_Name));
for idx = 1:length(PMU.Signal_Name)
    DataTable  = ALLDataTable(ALLDataTable.Signal == idx,:);
    
    if size(DataTable,1) == 2
        % Only two points were returned, indicating that the measurements
        % are not available (PI creates the values based on interpolation
        % of the nearest available measurements)
        fs(idx) = NaN;
        v{idx} = NaN;
        continue;
    end
    
    % One extra sample is always retrieved, so remove it
    DataTable(end,:) = [];
    
    % TimeStamp and Value from PI for each signal
    if idx == 1
        time_offset = round((StartTime - (DataTable.Time(1)/86400 + datenum(1970,0,1,0,0,0)))*24);
    end
    tPMUsig{idx} = DataTable.Time/86400 + datenum(1970,0,1,0,0,0) + time_offset/24;
    v{idx} = DataTable.Value;
    
    fsTemp = 1./diff(DataTable.Time);
    fsTempMed = median(fsTemp);
    fsTemp = fsTemp((2/3*fsTempMed < fsTemp) & (fsTemp < 2*fsTempMed));
    fs(idx) = round(mean(fsTemp));
end
fs = median(fs,'omitnan');
if isnan(fs)
    error('Time period does not appear to contain any of the selected signals.');
end
tPMU = StartTime + (0:1/fs:FileLength)'/86400;
tPMU = tPMU(1:end-1);
N = length(tPMU);

for idx = 1:length(v)
    if N == length(v{idx})
        % Has the right number of samples, skip to the next one
        continue;
    elseif 1 == length(v{idx})
        % Signal was not returned from PI database (data is missing), so
        % set the signal to a vector of NaNs and skip to next signal
        v{idx} = NaN(N,1);
        continue;
    end
    
    % Wrong number of samples - need to identify missing with NaN
    try
        % Identify missing samples at the beginning
        NumNanToAdd = near(tPMUsig{idx}(1),tPMU)-1;
        tPMUsig{idx} = [tPMU(1:NumNanToAdd); tPMUsig{idx}];
        v{idx} = [NaN(NumNanToAdd,1); v{idx}];

        % Look for jumps larger than 1.5 times the sampling interval
        JumpIdx = find(diff(tPMUsig{idx})*86400 > 1.5*1/fs);
        while ~isempty(JumpIdx)
            NewIdx = near(tPMUsig{idx}(JumpIdx(1)+1),tPMU);

            tPMUsig{idx} = [tPMUsig{idx}(1:JumpIdx(1)); tPMU(JumpIdx(1)+1:NewIdx-1); tPMUsig{idx}(JumpIdx(1)+1:end)];
            v{idx} = [v{idx}(1:JumpIdx(1)); NaN(length(JumpIdx(1)+1:NewIdx-1),1); v{idx}(JumpIdx(1)+1:end)];

            JumpIdx = find(diff(tPMUsig{idx})*86400 > 1.5*1/fs);
        end

        % Identify missing samples at the end
        NumNanToAdd = N - near(tPMUsig{idx}(end),tPMU);
        tPMUsig{idx} = [tPMUsig{idx}; tPMU(end-NumNanToAdd+1:end)];
        v{idx} = [v{idx}; NaN(NumNanToAdd,1)];
    catch
        % Attempt to identify missing data failed, so set entire signal to
        % NaN
        warning(['Attempt to set missing data in ' PMU.Signal_Name{idx} ' to NaN failed. Setting all values to NaN.']);
        v{idx} = NaN(N,1);
    end
end

%% Build final PMU structure
PMU.Signal_Time.Time_String = cellstr(datestr(tPMU,'yyyy-mm-dd HH:MM:SS.FFF'));
PMU.Signal_Time.Signal_datenum = tPMU;
PMU.Signal_Time.datetime = datetime(tPMU,'ConvertFrom','datenum','Format','MM/dd/yy HH:mm:ss.SSSSSS');
PMU.Data = zeros(length(v{1}),length(v));
for idx = 1:length(v)
    PMU.Data(:,idx) = v{idx};
end

time_offsetSign = sign(time_offset);
time_offset = time(caldays(0) + hours(abs(time_offset)));
time_offset.Format = 'hh:mm';
time_offset = char(time_offset);
if time_offsetSign == -1
    time_offset = ['-' time_offset];
end
for idx = 1:length(PMU)
    PMU(idx).File_Name = PresetFile;
    PMU(idx).Stat = zeros(size(PMU(idx).Data,1),1);
    PMU(idx).Flag = false(size(PMU(idx).Data,1),size(PMU(idx).Data,2),Num_Flags);
    PMU(idx).Time_Zone = time_offset;
end