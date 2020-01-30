function PlotInfo = ReadMMdata(StartTime,EndTime,EventPath)

StartTimeDT = datetime(StartTime);
EndTimeDT = datetime(EndTime);
Days = dateshift(StartTimeDT,'start','day'):dateshift(EndTimeDT,'start','day');

MMfolders = dir([EventPath '\MM']);
MMfolders = {MMfolders.name};
MMfolders(strcmp(MMfolders,'.')) = [];
MMfolders(strcmp(MMfolders,'..')) = [];

PlotInfo = struct('Data',{},'Time',{},'SignalNames',{},'Ylabel',{},'Title',{});
for MMidx = 1:length(MMfolders)
    MM = [];
    for DayIdx = 1:length(Days)
        FilePath = [fullfile(EventPath, 'MM', MMfolders{MMidx}, datestr(Days(DayIdx),'yymmdd')) '.csv'];
        if exist(FilePath,'file') == 0
            continue
        end
        
        if isempty(MM)
            H = readtable(FilePath,'ReadVariableNames',false);
            H = H(1:4,:);
            VarNames = H{1,:};

            MM = readtable(FilePath,'HeaderLines',4);
            MM.Properties.VariableNames = VarNames;
            
            t = Days(DayIdx) + MM.Time/24;
        else
            MMnew = readtable(FilePath,'HeaderLines',4);
            MMnew.Properties.VariableNames = VarNames;
            
            MM = [MM; MMnew];
            
            t = [t; Days(DayIdx) + MMnew.Time/24];
        end
    end
    
    KeepIdx = (StartTimeDT <= t) & (t <= EndTimeDT);
    MM = MM(KeepIdx,:);
    t = t(KeepIdx);

    FreqIdx = contains(VarNames,'Frequency');
    DampIdx = contains(VarNames,'DampingRatio');
    OpIdx = contains(VarNames,'OperatingValue');
    DEFidx = contains(VarNames,'DEF');

    ModeNames = unique(H{2,FreqIdx},'stable');
    ModeIdx = cell(1,length(ModeNames));
    for Midx = 1:length(ModeNames)
        ModeIdx{Midx} = strcmp(H{2,:},ModeNames{Midx}) & ~DEFidx;

        AlgNames = unique(H{4,ModeIdx{Midx}},'stable');
        AlgIdx = cell(1,length(AlgNames));
        for Aidx = 1:length(AlgNames)
            AlgIdx{Aidx} = strcmp(H{4,:},AlgNames{Aidx});

            F = MM{:,FreqIdx & ModeIdx{Midx} & AlgIdx{Aidx}};
            D = MM{:,DampIdx & ModeIdx{Midx} & AlgIdx{Aidx}}*100;
            SigNames = H{3,FreqIdx & ModeIdx{Midx} & AlgIdx{Aidx}};
            
            PlotInfo(end+1).Data = F;
            PlotInfo(end).Time = datenum(t);
            PlotInfo(end).SignalNames = SigNames;
            PlotInfo(end).Ylabel = 'Frequency (Hz)';
            PlotInfo(end).Title = [ModeNames{Midx} ' - ' AlgNames{Aidx}];
            
            PlotInfo(end+1).Data = D;
            PlotInfo(end).Time = datenum(t);
            PlotInfo(end).SignalNames = SigNames;
            PlotInfo(end).Ylabel = 'Damping Ratio (%)';
            PlotInfo(end).Title = [ModeNames{Midx} ' - ' AlgNames{Aidx}];
        end
    end

    UnitNames = unique(H{4,OpIdx},'stable');
    UnitIdx = cell(1,length(UnitNames));
    for Uidx = 1:length(UnitNames)
        UnitIdx{Uidx} = strcmp(H{4,:},UnitNames{Uidx});

        TypeNames = unique(H{3,UnitIdx{Uidx}},'stable');
        TypeIdx = cell(1,length(TypeNames));
        for Tidx = 1:length(TypeNames)
            TypeIdx{Tidx} = strcmp(H{3,:},TypeNames{Tidx});

            T = MM{:,OpIdx & UnitIdx{Uidx} & TypeIdx{Tidx}};
            SigNames = H{2,OpIdx & UnitIdx{Uidx} & TypeIdx{Tidx}};
            
            PlotInfo(end+1).Data = T;
            PlotInfo(end).Time = datenum(t);
            PlotInfo(end).SignalNames = SigNames;
            PlotInfo(end).Ylabel = [TypeNames{Tidx} ' (' UnitNames{Uidx} ')'];
            PlotInfo(end).Title = 'Baselining Signals';
        end
    end
end