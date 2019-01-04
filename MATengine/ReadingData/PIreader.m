function [PMU,tPMU,fs] = PIreader(StartTime,Num_Flags,FileLength,preset,PresetFile)

EndTime = StartTime + FileLength/(60*60*24);

time_start = datestr(StartTime,'dd-mmm-yyyy HH:MM:SS');
time_end = datestr(EndTime,'dd-mmm-yyyy HH:MM:SS');
time_offset = -8;

% PresetFile = 'C:\Users\foll154\Documents\Central America FY17\PI_Reader_package\PI_presets.xml';

%% Read the PI presets

[PMU,Server] = ReadPIpresets(PresetFile);

%% Choose a preset

% Reduce PMU to just that preset
PMU = PMU(strcmp({PMU.PMU_Name},preset));

%% Setup for talking to PI

% pisdk = NET.addAssembly('OSIsoft.PISDK');
% pisdksrv = NET.addAssembly('OSIsoft.PITimeServer');
% pisdkcom = NET.addAssembly('OSIsoft.PISDKCommon');
NET.addAssembly('OSIsoft.PISDK');
NET.addAssembly('OSIsoft.PITimeServer');
NET.addAssembly('OSIsoft.PISDKCommon');
import PISDK.*

pi_sdk =  PISDK.PISDKClass();

%%

time_start = System.String(time_start); 
time_end = System.String(time_end);

% Iterate through signals and retrieve them from server
v = cell(1,length(PMU.Signal_Name));
tPMU = [];
fs = [];
for idx = 1:length(PMU.Signal_Name)
    pi_point1 = pi_sdk.GetPoint(['\\' Server '\' PMU.Signal_Name{idx}]);
    pi_data1 = pi_point1.Data.RecordedValues(time_start,time_end);
    
    v{idx} = zeros(pi_data1.Count()-1,1);
    if idx == 1
        tPMU = zeros(pi_data1.Count()-1,1);
    end
    for i = 1:pi_data1.Count()-1
        v{idx}(i) = pi_data1.Item(i).Value;
        
        if idx == 1
            tPMU(i) = pi_data1.Item(i).TimeStamp.UTCSeconds/60/60/24 + datenum(1970,0,1,0,0,0) + time_offset/24;
        end
    end
    if idx == 1
        fs = (length(tPMU)-1)/(diff(tPMU([1 end]))*24*60*60);
        if fs > 1
            fs = round(fs);
        end
    else
        if length(v{idx}) ~= length(v{idx-1})
            error('All signals grouped by a preset must have the same time stamps.');
        end
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