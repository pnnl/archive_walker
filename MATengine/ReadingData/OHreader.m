function [PMU,tPMU,fs] = OHreader(StartTime,Num_Flags,FileLength,preset,PresetFile)

EndTime = StartTime + FileLength/(60*60*24);

time_start = datestr(StartTime,'yyyy-mm-dd HH:MM:SS');
time_end = datestr(EndTime,'yyyy-mm-dd HH:MM:SS');

%% Read the PI presets

[PresetList,Server,Instance,GEPPort,SystemXML,ID] = ReadOHpresets(PresetFile);

%% Choose a preset

% Reduce to just the selected preset
KeepIdx = strcmp(PresetList,preset);
Server = Server{KeepIdx};
Instance = Instance{KeepIdx};
GEPPort = GEPPort{KeepIdx};
SystemXML = SystemXML{KeepIdx};
ID = ID{KeepIdx};

%% Load GetHistorianData from C# dll.
% Note that The ability to unload an assembly is not available in MATLAB,
% restart MATLAB to release the assembly.
if isdeployed
    % The GUI is calling the function, so point to the matlabDLLs folder
    % within the GUI folder
    [~,result] = system('path');
    SourceDir = char(regexpi(result, 'Path=(.*?);', 'tokens', 'once'));
    dllpath  = [SourceDir '\matlabDLLs\OH\ReadHistorian.dll']; % Full pathname is required
else
    % The function is being called from a Matlab session, so the path must
    % be specified.
    dllpath  = 'C:\Users\foll154\Documents\BPAoscillationApp\AWrepository\MATengine\matlabDLLs\OH\ReadHistorian.dll'; % Full pathname is required
end
try
    asmInfo  = NET.addAssembly(dllpath); % Make .NET assembly visible to MATLAB
catch
    warning('Path to ReadHistorian.dll may not be accurate in OHreader.m, leading to error.');
end

%% Setup rules to query data from OpenHistorian
Configuration                 = struct;
% "historianServer": Historian server host IP or DNS name. Can be optionally suffixed with port number, e.g.: historian:38402.
% "instanceName":    Instance name of the historian, e.g. PPA
% "GEPPort":         Historian Getaway Port Number, e.g. 6175
% "startTime":       Start time of desired data range in GMT
% "stopTime":        End time of desired data range in GMT
% "measurementIDs":  Comma separated list of measurement ID values; set to null ('') to retrieve values for all measurements
% "datacsv":         filename of a csv storing Historian data; set to null ('') to skip it
% "SystemXML":       a defaul xml file in OpenHistorian software folder contains all the details of active measurements

Configuration.historianServer = Server;
Configuration.instanceName    = Instance;
Configuration.GEPPort         = GEPPort;
Configuration.startTime       = time_start;
Configuration.stopTime        = time_end;
Configuration.measurementIDs  = ID;
Configuration.datacsv         = '';
Configuration.SystemXML       = SystemXML;

%%

DataStruct = ReadHistorian(Configuration);

fs = unique([DataStruct.FramesPerSecond]);
if length(fs) > 1
    error('All signals in a preset must have the same reporting rate.');
end

% Determine the offset between the requested and retrieved times in hours
time_offset = [];
for idx = 1:length(DataStruct)
    if isempty(DataStruct(idx).TimeSeries)
        % The time series is empty for this signal, so skip to the next one
        continue
    end
    
    time_offset = round((datenum(DataStruct(idx).TimeSeries{1,1}{1}) - StartTime)*24);
    break
end
if isempty(time_offset)
    error('No data was returned.');
end

PMU = ConvertHistorianStructToPMU(DataStruct,StartTime,EndTime,fs,time_offset,preset);

tPMU = PMU(1).Signal_Time.Signal_datenum;

%% Build final PMU structure

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