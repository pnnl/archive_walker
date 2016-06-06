function PMU = InitCustomPMU(PMU,FileName,Time_Zone,Signal_Time,N,Num_Flags)
next = length(PMU)+1;

PMU(next).File_Name = FileName;   % file name
PMU(next).PMU_Name = 'Custom Signals';   % PMU name
PMU(next).Time_Zone = Time_Zone;         % time zone; for now this is just the PST time 
PMU(next).Signal_Time = Signal_Time;
PMU(next).Signal_Name = cell(1,0);
PMU(next).Signal_Type = cell(1,0);
PMU(next).Signal_Unit = cell(1,0);
PMU(next).Stat = zeros(N,1);
PMU(next).Data = zeros(N,0);
PMU(next).Flag = false(N,0,Num_Flags);