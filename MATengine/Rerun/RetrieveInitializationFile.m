function [InitializationFile, InitializationFileTime] = RetrieveInitializationFile(RerunStartTime,InitializationPath)

% Find the initialization file for RerunStartTime, or immediately
% preceeding it
yyyymmdd = datestr(RerunStartTime,'yyyymmdd');
InitializationFilePath = [InitializationPath '\' yyyymmdd(1:4) '\' yyyymmdd(3:8) '\'];
% List of initialization files for RerunStartTime date
InitializationFiles = dir(InitializationFilePath);
IsDir = [InitializationFiles.isdir];
InitializationFiles = {InitializationFiles.name};
InitializationFiles = InitializationFiles(~IsDir);
InitializationFileTimes = FileToTime(InitializationFiles,yyyymmdd);
[InitializationFileTimes, SortIdx] = sort(InitializationFileTimes,'ascend');
InitializationFiles = InitializationFiles(SortIdx);
% Find the first initialization file before or at RerunStartTime
InitializationFileIdx = find(InitializationFileTimes <= RerunStartTime, 1, 'last');
if isempty(InitializationFileIdx)
    % An initialization file for this time could not be found in the
    % folder of files for the day of RerunStartTime. Technically it may
    % be possible for the initialization file to be from the previous
    % day, but more likely there is a problem in what the user is
    % requesting. Throw a warning and select the first initialization
    % file from this day. If none exist, throw an error.
    if isempty(InitializationFiles)
        error(['Initialization files for day ' datestr(RerunStartTime,'yyyy-mm-dd') ' do not exist.']);
    else
        warning(['Desired initialization file could not be found for day ' datestr(RerunStartTime,'yyyy-mm-dd') '. First file for this day will be used.']);
        InitializationFileIdx = 1;
    end
end

InitializationFile = [InitializationFilePath InitializationFiles{InitializationFileIdx}];
InitializationFileTime = InitializationFileTimes(InitializationFileIdx);
end

function Times = FileToTime(Files,yyyymmdd)

yyyy = str2double(yyyymmdd(1:4));
mm = str2double(yyyymmdd(5:6));
dd = str2double(yyyymmdd(7:8));

if ~isempty(Files)
    Times(length(Files)) = datetime();
else
    Times = datetime();
    return
end
for idx = 1:length(Files)
    temp = strsplit(Files{idx},'_');
    temp = strsplit(temp{3},'.');
    HH = str2double(temp{1}(1:2));
    MM = str2double(temp{1}(3:4));
    SS = str2double(temp{1}(5:6));
    Times(idx) = datetime(yyyy,mm,dd,HH,MM,SS);
end

end