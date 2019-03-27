function Result = WriteMMreport(StartTime,EndTime,EventPath,ReportType,DampThresh,EventSepMinutes,ReportPath)

if exist(ReportPath,'file') ~= 0
    Result = 'Cannot overwrite existing file.';
    return
end

StartTimeDT = datetime(StartTime);
EndTimeDT = datetime(EndTime);
Days = dateshift(StartTimeDT,'start','day'):dateshift(EndTimeDT,'start','day');

MMfolders = dir([EventPath '\MM']);
MMfolders = {MMfolders.name};
MMfolders(strcmp(MMfolders,'.')) = [];
MMfolders(strcmp(MMfolders,'..')) = [];

OpConPlotInfo = struct('Data',{},'Time',{},'SignalNames',{},'Ylabel',{},'Title',{});
mmPlotInfo = struct('Data',{},'Time',{},'SignalNames',{},'Ylabel',{},'Title',{},'Mode',{});

ModeNameRep = {};
tSt = NaT(0);
tEn = NaT(0);
MinD = [];
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
    
    fs = round((length(t)-1)/minutes(t(end)-t(1)));
    EventSepSamp = EventSepMinutes*fs;
    
    KeepIdx = (StartTimeDT <= t) & (t <= EndTimeDT);
    MM = MM(KeepIdx,:);
    t = t(KeepIdx);

    FreqIdx = contains(VarNames,'Frequency');
    DampIdx = contains(VarNames,'DampingRatio');
    OpIdx = contains(VarNames,'OperatingValue');

    ModeNames = unique(H{2,FreqIdx},'stable');
    for Midx = 1:length(ModeNames)
        ModeIdx = strcmp(H{2,:},ModeNames{Midx});
        D = MM{:,DampIdx & ModeIdx}*100;
        
        DetIdx = D < DampThresh;
        DetIdxOR = sum(DetIdx,2) > 0;
        
        % Identify periods separated by at least EventSepSamp samples
        DetLoc = find(DetIdxOR);
        if ~isempty(DetLoc)
            SegEn = [DetLoc(diff(DetLoc) > EventSepSamp); DetLoc(end)];
            SegSt = [DetLoc(1); DetLoc(find(diff(DetLoc) > EventSepSamp)+1)];
        else
            SegEn = [];
            SegSt = [];
        end
        
        for SegIdx = 1:length(SegSt)
            RepInfo = struct('ModeName',{},'tSt',NaT,'tEn',NaT,'MinD',[]);
            
            ModeNameRep = [ModeNameRep; ModeNames{Midx}];
            MinD = [MinD; min(min(D(SegSt(SegIdx):SegEn(SegIdx),:),[],'omitnan'),[],'omitnan')];
            tSt = [tSt; t(SegSt(SegIdx))];
            tEn = [tEn; t(SegEn(SegIdx))];
        end
    end
    
    if strcmp(ReportType,'Graphical')
        
        for Midx = 1:length(ModeNames)
            ModeIdx = strcmp(H{2,:},ModeNames{Midx});
            AlgNames = unique(H{4,ModeIdx},'stable');
            
            for Aidx = 1:length(AlgNames)
                AlgIdx = strcmp(H{4,:},AlgNames{Aidx});

                F = MM{:,FreqIdx & ModeIdx & AlgIdx};
                D = MM{:,DampIdx & ModeIdx & AlgIdx}*100;
                SigNames = H{3,FreqIdx & ModeIdx & AlgIdx};

                mmPlotInfo(end+1).Data = F;
                mmPlotInfo(end).Time = t;
                mmPlotInfo(end).SignalNames = SigNames;
                mmPlotInfo(end).Ylabel = 'Frequency (Hz)';
                mmPlotInfo(end).Title = [ModeNames{Midx} ' - ' AlgNames{Aidx}];
                mmPlotInfo(end).Mode = ModeNames{Midx};

                mmPlotInfo(end+1).Data = D;
                mmPlotInfo(end).Time = t;
                mmPlotInfo(end).SignalNames = SigNames;
                mmPlotInfo(end).Ylabel = 'Damping Ratio (%)';
                mmPlotInfo(end).Title = [ModeNames{Midx} ' - ' AlgNames{Aidx}];
                mmPlotInfo(end).Mode = ModeNames{Midx};
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

                OpConPlotInfo(end+1).Data = T;
                OpConPlotInfo(end).Time = t;
                OpConPlotInfo(end).SignalNames = SigNames;
                OpConPlotInfo(end).Ylabel = [TypeNames{Tidx} ' (' UnitNames{Uidx} ')'];
                OpConPlotInfo(end).Title = 'Baselining Signals';
            end
        end
    end
end

Dur = round(hours(tEn-tSt),1);

T = table(tSt,tEn,Dur,ModeNameRep,MinD);
T.Properties.VariableNames = {'Start','End','DurationHours','Mode','MinDamping'};
T = sortrows(T,'Start');

if strcmp(ReportType,'Graphical')
    try
        WriteMMreportGraphical(StartTime,EndTime,DampThresh,T,mmPlotInfo,OpConPlotInfo,ReportPath);
        Result = 'Success';
    catch e
        Result = e.message;
    end
else
    try
        writetable(T,ReportPath);
        Result = 'Success';
    catch e
        Result = e.message;
    end
end