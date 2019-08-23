%% 
% get the next focus file
% this function handles different reading modes
% 
% Input: 
%   DataInfo: a data structure that contains all the information from the XML input file
% 
% Output:
%   focusFile: the next focus file name
%   done: flag to indicate if data reader should be terminated 
%         it will be set to 1 in the case: 
%           a)all files are processed in the archive mode 
%           b) no more future files available in real time mode or hybrid mode
%   DataInfo: input DataInfo data structure
%
% Created June 21, 2016 by Tao Fu
% 
% Modified June 23, 2016 by Tao Fu
%   1. reorganized the code for easy reading
%   2. added debugMode as input (removed on 5/10/2018 by Jim Follum)
%   3. modifed the code after DataInfo doesn't have a list of files that should be processed in the archive mode
%
%%
function [focusFile,done,SkippedFiles,FocusFileTime,DataInfo,FileLength] = getFocusFiles(FileInfo,FileDirectory,DataInfo,FileLength,ResultUpdateInterval)
done = 0; % used as a flag to identify the prcessing should be ended

if(isempty(FileInfo.lastFocusFile))
    % no last focus file, this is at the beginning of processing
    InitialFocusFileTime = datenum(DataInfo.DateTimeStart);
else
    % tPMU is from the last processed focus file
    InitialFocusFileTime = ceil(FileInfo.tPMU(end)*24*3600)/24/3600; 
    
%     while InitialFocusFileTime > datenum(datetime('now','TimeZone','UTC'))-110.6478
%         pause(10);
%     end
end

% Check focus file time: time to quit?, time to transition to real-time
% mode?
%
% If in Hybrid mode, update the end time based on the current system time
if(strcmp(DataInfo.mode, 'Hybrid'))
    DataInfo.DateTimeEnd = datestr(datetime('now','TimeZone','UTC')+DataInfo.UTCoffset-DataInfo.RealTimeRange/60/60/24,'yyyy-mm-dd HH:MM:SS');
end
DateTimeEnd = datenum(DataInfo.DateTimeEnd);
% If statement will evaluate to false if DateTimeEnd is empty (real-time
% mode)
if(InitialFocusFileTime > DateTimeEnd)
    if(strcmp(DataInfo.mode, 'Archive'))
        % focus file time is later than the specified DateTimeEnd
        % finish processing data, no need to continue
        done = 1;
    elseif(strcmp(DataInfo.mode, 'Hybrid'))
        % switch to real time mode
        DataInfo.DateTimeEnd = [];
        DataInfo.mode = 'RealTime';
    end
end

% file type
if(strcmpi(FileInfo.FileType,'pdat'))
    fileType = 1;
elseif(strcmpi(FileInfo.FileType,'csv'))
    fileType = 2;
elseif(strcmpi(FileInfo.FileType,'powHQ'))
    fileType = 3;
elseif(strcmpi(FileInfo.FileType,'PI'))
    SkippedFiles = 0;
    FocusFileTime = InitialFocusFileTime;
    focusFile = FocusFileTime;
    FileLength = ResultUpdateInterval;
    DataInfo.PresetFile = fullfile(FileDirectory, DataInfo.PresetFileInit);
    
    % Data from FocusFileTime to (FocusFileTime+FileLength) will be read.
    %
    % If FocusFileTime is in the future, throw an error to avoid being
    % essentially stuck in matlab during a very long wait. 
    %
    % If FocusFileTime+FileLength is in the future, pause until that time
    % is reached so that all data will be available.
    %
    % Only applicable in Real-Time and Hybrid modes (UTCoffset field not set
    % in Archive mode)
    if isfield(DataInfo,'UTCoffset')
        % Current time in data's time zone
        CurrentTime = datenum(datetime('now','TimeZone','UTC')+DataInfo.UTCoffset);
        
        if FocusFileTime > CurrentTime
            error('Requested data should not be in the future.');
        elseif (FocusFileTime + FileLength/(60*60*24)) > CurrentTime
            % CurrentTime is (FocusFileTime+FileLength-CurrentTime) into
            % the future. Pause for this amount of time so that all
            % requested data will be available.
%             disp(['Pausing for ' num2str((FocusFileTime + FileLength/(60*60*24) - CurrentTime)*24*60*60) ' seconds']);
            pause((FocusFileTime + FileLength/(60*60*24) - CurrentTime)*24*60*60);
        end
    end
    
    return
elseif(strcmpi(FileInfo.FileType,'OpenHistorian'))
    SkippedFiles = 0;
    FocusFileTime = InitialFocusFileTime;
    focusFile = FocusFileTime;
    FileLength = ResultUpdateInterval;  
    DataInfo.PresetFile = fullfile(FileDirectory, DataInfo.PresetFileInit);
    
    % Data from FocusFileTime to (FocusFileTime+FileLength) will be read.
    %
    % If FocusFileTime is in the future, throw an error to avoid being
    % essentially stuck in matlab during a very long wait. 
    %
    % If FocusFileTime+FileLength is in the future, pause until that time
    % is reached so that all data will be available.
    %
    % Only applicable in Real-Time and Hybrid modes (UTCoffset field not set
    % in Archive mode)
    if isfield(DataInfo,'UTCoffset')
        % Current time in data's time zone
        CurrentTime = datenum(datetime('now','TimeZone','UTC')+DataInfo.UTCoffset);
        
        if FocusFileTime > CurrentTime
            error('Requested data should not be in the future.');
        elseif (FocusFileTime + FileLength/(60*60*24)) > CurrentTime
            % CurrentTime is (FocusFileTime+FileLength-CurrentTime) into
            % the future. Pause for this amount of time so that all
            % requested data will be available.
%             disp(['Pausing for ' num2str((FocusFileTime + FileLength/(60*60*24) - CurrentTime)*24*60*60) ' seconds']);
            pause((FocusFileTime + FileLength/(60*60*24) - CurrentTime)*24*60*60);
        end
    end
    
    return
elseif(strcmpi(FileInfo.FileType,'openPDC'))
    SkippedFiles = 0;
    FocusFileTime = InitialFocusFileTime;
    focusFile = FocusFileTime;
    FileLength = ResultUpdateInterval;  
    DataInfo.PresetFile = fullfile(FileDirectory, DataInfo.PresetFileInit);
    
    % Data from FocusFileTime to (FocusFileTime+FileLength) will be read.
    %
    % If FocusFileTime is in the future, throw an error to avoid being
    % essentially stuck in matlab during a very long wait. 
    %
    % If FocusFileTime+FileLength is in the future, pause until that time
    % is reached so that all data will be available.
    %
    % Only applicable in Real-Time and Hybrid modes (UTCoffset field not set
    % in Archive mode)
    if isfield(DataInfo,'UTCoffset')
        % Current time in data's time zone
        CurrentTime = datenum(datetime('now','TimeZone','UTC')+DataInfo.UTCoffset);
        
        if FocusFileTime > CurrentTime
            error('Requested data should not be in the future.');
        elseif (FocusFileTime + FileLength/(60*60*24)) > CurrentTime
            % CurrentTime is (FocusFileTime+FileLength-CurrentTime) into
            % the future. Pause for this amount of time so that all
            % requested data will be available.
%             disp(['Pausing for ' num2str((FocusFileTime + FileLength/(60*60*24) - CurrentTime)*24*60*60) ' seconds']);
            pause((FocusFileTime + FileLength/(60*60*24) - CurrentTime)*24*60*60);
        end
    end
    
    return
end

     
% find the name and folder of the focus file
[focusFileFolder,focusFile] = getFileFolder(FileDirectory,FileInfo.FileMnemonic,InitialFocusFileTime,fileType);

checking = 1;
NoFutureCount = 0;
FutureCount = 0;
FutureWait = DataInfo.FutureWait;
NoFutureWait = DataInfo.NoFutureWait;
MaxFutureCount = DataInfo.MaxFutureCount;
MaxNoFutureCount = DataInfo.MaxNoFutureCount;

%% loop until the focus file exist
while(checking == 1)
    if(exist(focusFile,'file'))
        %% the focusFile is available, stop checking
        checking = 0;
        % reset counters
        NoFutureCount = 0;
        FutureCount = 0;
    end
    
    %% check if it is time to give up or skip the focus file
    if(checking == 1)        
        % reached MaxFutureCount, skip the focus file and move to the next file
        if(FutureCount >= MaxFutureCount)
            % pass this file
            % focusFileTime = focusFileTime+defaultFileTimeDiff; % may change to use the first future file
            % [focusFileFolder,focusFile] = getPdatFileFolder(DataInfo.FileDirectory,DataInfo.FileMnemonic,focusFileTime);
            
            % process the 1st future file, we probably need to check if the next file in line is available
            futureFileList = sort(futureFileList);
            focusFile = futureFileList{1};
            checking = 0;            
            % reset counters
            NoFutureCount = 0;
            FutureCount = 0;
        end
        
        % check if MaxNoFutureCount is reached and procesing should be terminated
        if(NoFutureCount >= MaxNoFutureCount)
            checking = 0;
            done = 1;
        end
        
    end
    
    %% current focus file doesn't exist
    if(checking == 1)
        % if(already in future wait)
        %   wait one more cycle
        % else
        %   check if future files exist
        %   if(future files exist)
        %       start future count
        %   end
        % end
        % 
        % if(not in future count mode) i.e. no future files exist
        %   start no future count
        % end
        %
        if(FutureCount > 0)
            % already trying to detect future file
            % wait one more cycle
            if(isempty(DataInfo.DateTimeEnd))
                pause(FutureWait); % pause in real time mode
            end
            FutureCount = FutureCount+1;
        else
            % check if future files exist
            % files in the focus folder
            if(exist(focusFileFolder,'dir'))
                if(fileType == 1)
                    files1 = dir([focusFileFolder,'*.pdat']);
                elseif(fileType == 2)
                    files1 = dir([focusFileFolder,'*.csv']);
                elseif(fileType == 3)
                    files1 = dir([focusFileFolder,'*.mat']);
                end
                if(~isempty(files1))
                    files1 = {files1.name};
                    
                    % Remove any files that do not have the right mnemonic
                    files1 = files1(startsWith(files1, [FileInfo.FileMnemonic '_']));
                    
                    files1 = strcat(focusFileFolder,'\',files1); % this is supposed to be faster than a loop
                else
                    files1 = {};
                end
            else
                files1 = {};
            end
            % files in the next available day folder
            % nextDayTime = focusFileTime+1;
            % nextFileFolder = getPdatFileFolder(DataInfo.FileDirectory,DataInfo.FileMnemonic,nextDayTime);
            nextFileFolder = getNextFutureDayFolder(focusFileFolder,fileType);
            files2 = {};
            if(exist(nextFileFolder,'dir'))
                if(fileType == 1)
                    files2 = dir([nextFileFolder,'*.pdat']);
                elseif(fileType == 2)
                    files2 = dir([nextFileFolder,'*.csv']);
                elseif(fileType == 3)
                    files2 = dir([nextFileFolder,'*.mat']);
                end
                if(~isempty(files2))
                    files2 = {files2.name};
                    
                    % Remove any files that do not have the right mnemonic
                    files2 = files2(startsWith(files2, [FileInfo.FileMnemonic '_']));
                    
                    files2 = strcat(nextFileFolder,'\',files2); % this is supposed to be faster than a loop
                else
                    files2 = {};
                end
            else
                files2 = {};
            end
            files = [files1,files2];
            if(~isempty(files))
                % has files in the folder
                % check if future files exist
                fileTimes = zeros(1,length(files));
                for i = 1:length(files)
                    fileTimes(i) = getPdatFileTime(files{i});
                end
                k = find(fileTimes >= InitialFocusFileTime);
                if(~isempty(k))
                    % future files availabe
                    futureFileList = files(k);
                    % start future file count
                    if(isempty(DataInfo.DateTimeEnd))
                        pause(FutureWait); % pause in real time mode
                    end
                    FutureCount = 1;
                    NoFutureCount = 0;  % set NoFutureCount to 0
                end
            end            
        end
        
        %% no future files available
        if(FutureCount == 0)
            % both focus file and future files don't exist,
            NoFutureCount = NoFutureCount+1;        
            if(isempty(DataInfo.DateTimeEnd))                
                % in real time mode, start noFutureCount
                pause(NoFutureWait);                        
            end
        end
    end
    

end

k1 = strfind(focusFile,'_');
k2 = strfind(focusFile,'.');

dayStr = focusFile(k1(end-1)+1:k1(end)-1);
timeStr = focusFile(k1(end)+1:k2(end)-1);

year = str2num(dayStr(1:4));
month = str2num(dayStr(5:6));
day = str2num(dayStr(7:8));

hour = str2num(timeStr(1:2));
minute = str2num(timeStr(3:4));
second = str2num(timeStr(5:6));

FocusFileTime = datenum([year month day hour minute second]);

if ~isempty(FileLength)
    SkippedFiles = round(((FocusFileTime - InitialFocusFileTime)*24*60*60)/FileLength);
else
    SkippedFiles = 0;
end

end

