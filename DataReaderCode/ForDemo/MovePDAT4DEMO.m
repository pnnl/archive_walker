OldDataDir = 'C:\Users\foll154\Documents\BPA Oscillation App\ToGit\OriginalData';
DataDir = 'C:\Users\foll154\Documents\BPA Oscillation App\ToGit\ForDemo\PseudoNewData';

OldYearFolder = dir(OldDataDir);
OldYearFolder = OldYearFolder([OldYearFolder.isdir] == 1);
OldYearFolder = OldYearFolder(cellfun(@isempty,regexp({OldYearFolder.name},'20'))==0);
OldYearFolder = OldYearFolder.name;

OldDayFolder = dir([OldDataDir '\' OldYearFolder]);
OldDayFolder = OldDayFolder(end).name;

OldFiles = dir([OldDataDir '\' OldYearFolder '\' OldDayFolder '\WISPDITT*']);
OldFiles = {OldFiles.name};

NowTime = now();
RightNow = datevec(datestr(NowTime));
NewYearFolder = num2str(RightNow(1));
NewDayFolder = [num2str(RightNow(1)) '0' num2str(RightNow(2)) num2str(RightNow(3))];
NewDayFolder = NewDayFolder(3:end);

try
    rmdir([DataDir '\' NewYearFolder],'s');
end
copyfile([OldDataDir '\' OldYearFolder '\' OldDayFolder '\WISPDITT*'],...
    [DataDir '\' NewYearFolder '\' NewDayFolder]);

CopiedFiles = dir([DataDir '\' NewYearFolder '\' NewDayFolder '\WISPDITT*']);
CopiedFiles = {CopiedFiles.name};

mkdir([DataDir '\' NewYearFolder '\' NewDayFolder '\Future']);

StartBack = 20;  % First file will be from StartBack minutes before right now
for idx = 1:length(CopiedFiles)
    ThisCopiedFile = CopiedFiles{idx};
    N = length(ThisCopiedFile);
    
    NewName = ThisCopiedFile;
    NewName(N-19:N-5) = datestr(NowTime+((idx-1)-StartBack)/60/24,'yyyymmdd_HHMM00');
    
    if ((idx-1)-StartBack == -15) || ((idx-1)-StartBack == -5)
        delete([DataDir '\' NewYearFolder '\' NewDayFolder '\' ThisCopiedFile]);
        continue
    end
    
    if (idx-1)-StartBack > 0
        movefile([DataDir '\' NewYearFolder '\' NewDayFolder '\' ThisCopiedFile],...
            [DataDir '\' NewYearFolder '\' NewDayFolder '\Future\' NewName]);
    else
        movefile([DataDir '\' NewYearFolder '\' NewDayFolder '\' ThisCopiedFile],...
            [DataDir '\' NewYearFolder '\' NewDayFolder '\' NewName]);
    end
end