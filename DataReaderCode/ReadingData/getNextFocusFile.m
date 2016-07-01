%% 
% get the next focus file
% this function handles different reading modes
% 
% Input: 
%   DataInfo: a data structure that contains all the information from the XML input file
%   flog: file handler of the log file
%   debugMode: indicate if the code is running in the debug mode
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
%   2. added debugMode as input
%   3. modifed the code after DataInfo doesn't have a list of files that should be processed in the archive mode
%
%%
function [focusFile,done,DataInfo] = getNextFocusFile(DataInfo,flog,debugMode)
done = 0; % used as a flag to identify the prcessing should be ended

% file type
if(strcmpi(DataInfo.FileType,'pdat'))
    fileType = 1;
elseif(strcmpi(DataInfo.FileType,'csv'))
    fileType = 2;
end

% get the next focus file time
tPMU = DataInfo.tPMU;
if(tPMU ~= 0)
    % tPMU is from the last processed focus file
    focusFileTime = ceil(tPMU(end)*24*3600)/24/3600; 
else
    if(isempty(DataInfo.lastFocusFile))
        % no last focus file, this is at the beginning of processing
        focusFileTime = datenum(DataInfo.DateTimeStart);
    else
        % last focus file was found.
        lastFocusFileTime = getPdatFileTime(DataInfo.lastFocusFile);
        defaultFileInt = 1/60/24;  % default file interval is 1 minute
        focusFileTime = lastFocusFileTime+defaultFileInt;
    end
end

% check focus file time
if(~strcmp(DataInfo.mode, 'RealTime'))
    % in archive mode or hybrid mode
    DateTimeEnd = datenum(DataInfo.DateTimeEnd);
    if(focusFileTime > DateTimeEnd)
        if(strcmp(DataInfo.mode, 'Archive'))
            % focus file time is later than the specified DateTimeEnd
            % finish processing data, no need to continue
            done = 1;
        elseif(strcmp(DataInfo.mode, 'Hybrid'))
            % switch to real time mode
            DataInfo.DateTimeEnd = [];
        end
    end
end



     
% find the name and folder of the focus file
if(fileType == 1)
    % pdat files
    [focusFileFolder,focusFile] = getPdatFileFolder(DataInfo.FileDirectory,DataInfo.FileMnemonic,focusFileTime);
elseif(fileType == 2)
    % csv files
    [focusFileFolder,focusFile] = getCSVFileFolder(DataInfo.FileDirectory,DataInfo.FileMnemonic,focusFileTime);
end
if(debugMode)
    fprintf(flog, 'The next focus file should be: %s\n',focusFile);
    fprintf(flog, 'The next focus file folder should be %s\n', focusFileFolder);
end

checking = 1;
NoFutureCount = 0;
FutureCount = 0;
%FutureFileAvailable = 0;
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
            fprintf(flog,'\nStopped Becuase No More Future Files Available\n');
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
            %printf(flog, 'FutureCount = %d\n', FutureCount);
            if(isempty(DataInfo.DateTimeEnd))
                pause(FutureWait); % pause in real time mode
                if(debugMode)
                    fprintf(flog, 'in real time mode, paused %f secondes\n',FutureWait);
                end
            else
                % still in archive mode, no pause
                if(debugMode)
                    fprintf(flog,'In Archive Mode, No pause...\n');
                end
            end
            FutureCount = FutureCount+1;
            if(debugMode)
                fprintf(flog,'FutureCount = %d\n',FutureCount);
            end
        else
            % check if future files exist
            % files in the focus folder
            if(exist(focusFileFolder,'dir'))
                if(fileType == 1)
                    files1 = dir([focusFileFolder,'*.pdat']);
                elseif(fileType == 2)
                    files1 = dir([focusFileFolder,'*.csv']);
                end
                if(~isempty(files1))
                    files1 = {files1.name};
                    %for i = 1:length(files1)
                    %    files1{i} = [focusFileFolder,files1{i}];
                    %end
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
                end
                if(~isempty(files2))
                    files2 = {files2.name};
                    %for i = 1:length(files2)
                    %    files2{i} = [nextFileFolder,files2{i}];
                    %end
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
                k = find(fileTimes >= focusFileTime);
                if(~isempty(k))
                    % future files availabe
                    futureFileList = files(k);
                    % start future file count
                    if(debugMode)
                        fprintf(flog,'\nFound Future Files - Start FutureCount\n');
                    end
                    if(isempty(DataInfo.DateTimeEnd))
                        pause(FutureWait); % pause in real time mode
                        if(debugMode)
                            fprintf(flog, 'in real time mode, paused %f secondes\n',FutureWait);
                        end
                    else
                        % still in archive mode, no pause
                        if(debugMode)
                            fprintf(flog,'In Archive Mode, No pause...\n');
                        end
                    end
                    FutureCount = 1;
                    NoFutureCount = 0;  % set NoFutureCount to 0
                    if(debugMode)
                        fprintf(flog,'FutureCount = %d\n',FutureCount);
                    end
                end
            end            
        end
        
        %% no future files available
        if(FutureCount == 0)
            % both focus file and future files don't exist,
            NoFutureCount = NoFutureCount+1;        
            if(isempty(DataInfo.DateTimeEnd))                
                % in real time mode, start noFutureCount
                if(NoFutureCount > 0)
                    if(debugMode)
                        fprintf(flog,'NoFutureCount\n');
                    end
                else
                    if(debugMode)
                        fprintf(flog,'\nNo Future Files Available - Start NoFutureCount\n');
                    end
                end
                
                pause(NoFutureWait);                        
                if(debugMode)
                    fprintf(flog,'paused %f seconds\n',NoFutureWait);
                    fprintf(flog,'NoFutureCount = %d\n',NoFutureCount);                    
                end
            else
                % in archive mode, no need to wait, end the process
                if(debugMode)
                    fprintf(flog,'No future files available. In archive mode, no need to wait\n');
                    fprintf(flog,'NoFutureCount = %d\n',NoFutureCount);   
                end
            end
        end
    end
    

end





end

