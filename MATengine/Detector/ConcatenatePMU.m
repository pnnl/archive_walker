function PMUconcat = ConcatenatePMU(PMUconcat,PMU)

% PMUconcat starts empty. If it is empty, initialize it as the
% current PMU structure. If it is not empty, add the current
% PMU structure to it.
if isempty(PMUconcat)
    PMUconcat = PMU;
    for PMUidx = 1:length(PMUconcat)
        PMUconcat(PMUidx).File_Name = [];
    end
else
    for PMUidx = 1:length(PMUconcat)
        PMUconcat(PMUidx).Stat = [PMUconcat(PMUidx).Stat; PMU(PMUidx).Stat];
        PMUconcat(PMUidx).Data = [PMUconcat(PMUidx).Data; PMU(PMUidx).Data];
        PMUconcat(PMUidx).Flag = [PMUconcat(PMUidx).Flag; PMU(PMUidx).Flag];
        PMUconcat(PMUidx).Signal_Time.Time_String = [PMUconcat(PMUidx).Signal_Time.Time_String; PMU(PMUidx).Signal_Time.Time_String];
        PMUconcat(PMUidx).Signal_Time.Signal_datenum = [PMUconcat(PMUidx).Signal_Time.Signal_datenum; PMU(PMUidx).Signal_Time.Signal_datenum];
    end
end