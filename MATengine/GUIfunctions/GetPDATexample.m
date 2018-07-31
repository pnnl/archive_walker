function PMU = GetPDATexample(pdatFile)

[PMU,~,fs] = createPdatStruct(pdatFile,1,[]);

PMU = rmfield(PMU,{'Stat','Data','Flag','File_Name','Time_Zone','Signal_Time'});
PMU(1).fs = fs;

for idx = 1:length(PMU)
    PMU(idx).PMU_Name = {PMU(idx).PMU_Name};
end