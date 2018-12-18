clear;

%% User choices

time_start = '2-Jan-2015 10:00:00';
time_end = '2-Jan-2015 10:05:00';

PresetFile = 'C:\Users\foll154\Documents\Central America FY17\PI_Reader_package\PI_presets.xml';

%% Read the PI presets

[PMUinit,Server] = ReadPIpresets(PresetFile);

%% Choose a preset
preset = 'Preset1';

% Reduce PMU to just that preset
PMUinit = PMUinit(strcmp({PMUinit.PMU_Name},preset));

%% Setup for talking to PI

pisdk = NET.addAssembly('OSIsoft.PISDK')
pisdksrv = NET.addAssembly('OSIsoft.PITimeServer')
pisdkcom = NET.addAssembly('OSIsoft.PISDKCommon')
import PISDK.*

pi_sdk =  PISDK.PISDKClass();

%%

time_start = System.String(time_start); 
time_end = System.String(time_end);

% Iterate through signals and retrieve them from server
v = cell(1,length(PMUinit.Signal_Name));
t = cell(1,length(PMUinit.Signal_Name));
fs = zeros(1,length(PMUinit.Signal_Name));
for idx = 1:length(PMUinit.Signal_Name)
    pi_point1 = pi_sdk.GetPoint(['\\' Server '\' PMUinit.Signal_Name{idx}]);
    pi_data1 = pi_point1.Data.RecordedValues(time_start,time_end);
    
    v{idx} = zeros(1,pi_data1.Count());
    t{idx} = zeros(1,pi_data1.Count());
    for i = 1:pi_data1.Count()
        v{idx}(i) = pi_data1.Item(i).Value;
        t{idx}(i) = pi_data1.Item(i).TimeStamp.UTCSeconds/60/60/24 + datenum(1970,0,1,0,0,0) - 8/24;
    end
    fs(idx) = length(t{idx})/(diff(t{idx}([1 end]))*24*60*60);
    if fs(idx) > 1
        fs(idx) = round(fs(idx));
    else
%         fs(idx) = 1/round(1/fs(idx));
    end
end

%% Build final PMU structure

fsU = unique(fs);
MT = cell(1,length(fsU));
PMU = struct('PMU_Name',MT,'Signal_Name',MT,'Signal_Type',MT,'Signal_Unit',MT,'fs',MT);
for idx = 1:length(fsU)
    SigIdx = fsU(idx) == fs;
    PMU(idx).PMU_Name = [PMUinit.PMU_Name '_' num2str(fsU(idx))];
    PMU(idx).Signal_Name = PMUinit.Signal_Name(SigIdx);
    PMU(idx).Signal_Type = PMUinit.Signal_Type(SigIdx);
    PMU(idx).Signal_Unit = PMUinit.Signal_Unit(SigIdx);
    PMU(idx).fs = fsU(idx);
end