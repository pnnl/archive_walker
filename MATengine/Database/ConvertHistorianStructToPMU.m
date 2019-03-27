function PMU = ConvertHistorianStructToPMU(DataStruct,tStart,tEnd,fs,time_offset,preset)

PMUlist = {DataStruct.Device};
PMUu = unique(PMUlist);

SigName = {DataStruct.PointTag};
SigType = ConvertSigType({DataStruct.SignalType});
SigUnit = ConvertSigUnit({DataStruct.EngineeringUnits});

tIdeal = tStart:(1/fs)/(60*60*24):tEnd;
tIdeal = [tStart-(1/fs)/(60*60*24); tIdeal.'; tEnd+(1/fs)/(60*60*24)];

tDatenum = tIdeal(2:end-2);
tString = cellstr(datestr(tDatenum,'yyyy-mm-dd HH:MM:SS.FFF'));
tDatetime = datetime(tDatenum,'ConvertFrom','datenum');

MT = cell(1,length(PMUu));
PMU = struct('PMU_Name',MT,'Signal_Name',MT,'Signal_Type',MT,'Signal_Unit',MT,'Signal_Time',MT,'Data',MT);
for idx = 1:length(PMUu)
    PMU(idx).PMU_Name = [PMUu{idx} '_' preset];
    
    for SigIdx = find(strcmp(PMUlist,PMUu{idx}))
        if isfield(DataStruct,'TimeSeries')
            t = datenum(DataStruct(SigIdx).TimeSeries{:,1},'yyyy-mm-dd HH:MM:SS.FFF');
            % Adjust for offset from specified time due to time zone
            t = t - time_offset/24;
            t = [tStart-(1/fs)/(60*60*24); t; tEnd+(1/fs)/(60*60*24)];
            % If there are jumps larger than 1.5*(1/fs) seconds, fill in 
            % data as NaN
            JumpLoc = find(diff(t) > 1.5*(1/fs)/(60*60*24));
            if ~isempty(JumpLoc)
                JumpLoc = [JumpLoc; length(t)];
                dFrom = [NaN; DataStruct(SigIdx).TimeSeries{:,2}; NaN];
                
                dTo = nan(length(tIdeal),1);
                
                % Start with samples leading up to the first jump
                dTo(1:JumpLoc(1)) = dFrom(1:JumpLoc(1));
                
                
                for jIdx = 1:length(JumpLoc)-1
                    FromIdx = JumpLoc(jIdx)+1:JumpLoc(jIdx+1);
                    ToIdx = near(t(JumpLoc(jIdx)+1),tIdeal) + (0:length(FromIdx)-1);
                    dTo(ToIdx) = dFrom(FromIdx);
                end
                
                % Trim the the first and last because they were added for
                % checking time stamps.
                dTo = dTo(2:end-1);
            else
                % Time stamps are okay
                dTo = DataStruct(SigIdx).TimeSeries{:,2};
            end
            
            % Trim the last because the end time is one sample too far.
            dTo = dTo(1:end-1);
            
            
            if isempty(PMU(idx).Signal_Name)
                PMU(idx).Signal_Time.Time_String = tString;
                PMU(idx).Signal_Time.Signal_datenum = tDatenum;
                PMU(idx).Signal_Time.datetime = tDatetime;
            end
            
            PMU(idx).Data = [PMU(idx).Data dTo];
        end
        PMU(idx).Signal_Name = [PMU(idx).Signal_Name SigName(SigIdx)];
        PMU(idx).Signal_Type = [PMU(idx).Signal_Type SigType(SigIdx)];
        PMU(idx).Signal_Unit = [PMU(idx).Signal_Unit SigUnit(SigIdx)];
    end
end

end

function SigType = ConvertSigType(SigType)
    for idx = 1:length(SigType)
        switch SigType{idx}
            case 'FREQ'
                SigType{idx} = 'F';
            case 'DFDT'
                SigType{idx} = 'RCF';
            case 'VPHM'
                SigType{idx} = 'VMP';
            case 'VPHA'
                SigType{idx} = 'VAP';
            case 'IPHM'
                SigType{idx} = 'IMP';
            case 'IPHA'
                SigType{idx} = 'IAP';
            otherwise
                SigType{idx} = 'OTHER';
        end
    end
end

function SigUnit = ConvertSigUnit(SigUnit)
    for idx = 1:length(SigUnit)
        switch SigUnit{idx}
            case 'Hz'
            case 'Volts'
                SigUnit{idx} = 'V';
            case 'Degrees'
                SigUnit{idx} = 'DEG';
            case 'Amps'
                SigUnit{idx} = 'A';
            otherwise
                SigUnit{idx} = 'O';
        end
    end
end