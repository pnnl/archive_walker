%% 
% get the next focus file
% this function handles different reading modes
% 
% Input: 
%   DataInfo: a data structure that contains all the information from the XML input file
%   flog: file handler of the log file
% 
% Output:
%   focusFile: the next focus file name
%   done: flag to indicate if data reader should be terminated 
%         it will be set to 1 in the case: 
%           a)all files are processed in the archive mode 
%           b) no more future files available in real time mode or hybrid mode
%   DataInfo: input DataInfo data structure with flagAvailableFiles being updated
%
% Created June 21, 2016 by Tao Fu
% 
%%
function [focusFile,done,DataInfo] = getNextFocusFile(DataInfo,flog)
done = 0; % used as a flag to identify the prcessing should be ended
foundFile = 0; % flag to identify the focus file is found from availableFileList;

%% process the next file on the availableFileList
availableFiles = DataInfo.availableFiles; % list of available files
if(~isempty(availableFiles))    
    % has available files
    flagAvailableFiles = DataInfo.flagAvailableFiles;
    % need to find files on the list that have not been processed
    k = find(flagAvailableFiles == 0);
    if(~isempty(k))
        % has files that is available
        focusFileIdx = k(1);
        focusFile = availableFiles{focusFileIdx};
        flagAvailableFiles(focusFileIdx) = 1; % mark the file as being checked
        DataInfo.flagAvailableFiles = flagAvailableFiles;
        foundFile = 1; %found the next foucus file
    else
        % all files on the list have been processed.
        if(strcmp(DataInfo.mode, 'Archive'))
            %in the archive mode, we are done            
            done = 1;
            focusFile = '';
        else
            % switch to real time mode
        end
    end
end

%%
if(~foundFile)
    % not found the next focus file
    if(~done)
        % this would be in the real time mode or hybrid mode
        % get the next focus file time
        tPMU = DataInfo.tPMU;
        if(tPMU ~= 0)
            focusFileTime = ceil(tPMU(end)*24*3600)/24/3600; % need to figure out what to do when the last PMU data file is not readable
        else
            focusFileTime = datenum(DataInfo.DateTimeStart); 
        end
        
        % find the name and folder of the focus file
        [focusFileFolder,focusFile] = getPdatFileFolder(DataInfo.FileDirectory,DataInfo.FileMnemonic,focusFileTime);
        fprintf(flog, 'The next focus file should be: %s\n',focusFile);
        fprintf(flog, 'The next focus file folder should be %s\n', focusFileFolder);
        % defaultFileTimeDiff = 1/60/24;  % default time for each PMU
        
        checking = 1;
        NoFutureCount = 0;
        FutureCount = 0;
        %FutureFileAvailable = 0;
        FutureWait = DataInfo.FutureWait;
        NoFutureWait = DataInfo.NoFutureWait;
        MaxFutureCount = DataInfo.MaxFutureCount;
        MaxNoFutureCount = DataInfo.MaxNoFutureCount;
        
        %% loop to find the next file
        while(checking == 1)
            if(exist(focusFile,'file'))
                % the focusFile is available, stop checking
                checking = 0;                
                % reset counters
                NoFutureCount = 0;
                FutureCount = 0;
            else
                % current focus file doesn't exist
                if(FutureCount > 0)
                    % already trying to detect future file
                    % wait one more cycle
                    %printf(flog, 'FutureCount = %d\n', FutureCount);
                    pause(FutureWait);
                    fprintf(flog,'paused %f secondes\n',FutureWait);
                    FutureCount = FutureCount+1;
                    fprintf(flog,'FutureCount = %d\n',FutureCount);
                else
                    % check if future files exist
                    % files in the focus folder
                    if(exist(focusFileFolder,'dir'))
                        files1 = dir([focusFileFolder,'*.pdat']);
                        if(~isempty(files1))
                            files1 = {files1.name};
                            for i = 1:length(files1)
                                files1{i} = [focusFileFolder,files1{i}];
                            end
                        else
                            files1 = {};
                        end
                        % files in the next day folder
                        nextDayTime = focusFileTime+1;
                        nextFileFolder = getPdatFileFolder(DataInfo.FileDirectory,DataInfo.FileMnemonic,nextDayTime);
                        files2 = {};
                        if(exist(nextFileFolder,'dir'))
                            files2 = dir([nextFileFolder,'*.pdat']);
                            if(~isempty(files2))
                                files2 = {files2.name};
                                for i = 1:length(files2)
                                    files2{i} = [nextFileFolder,files2{i}];
                                end
                            else
                                files2 = {};
                            end
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
                                fprintf(flog,'\nFound Future Files - Start FutureCount\n');
                                pause(FutureWait);
                                fprintf(flog, 'paused %f secondes\n',FutureWait);
                                FutureCount = 1;
                                NoFutureCount = 0;  % set NoFutureCount to 0
                                fprintf(flog,'FutureCount = %d\n',FutureCount);
                            end
                        end
                    end
                end
                
                %% no future files available, start no future count
                if(FutureCount == 0)
                    % both focus file and future files don't exist, noFutureCountStart
                    if(NoFutureCount > 0)
                        fprintf(flog,'NoFutureCount\n');
                    else
                        fprintf(flog,'\nNo Future Files Available - Start NoFutureCount\n');
                    end
                    pause(NoFutureWait);
                    fprintf(flog,'paused %f seconds\n',NoFutureWait);
                    NoFutureCount = NoFutureCount+1;
                    fprintf(flog,'NoFutureCount = %d\n',NoFutureCount);
                end
            end
            
            %% reached MaxFutureCount, skip the focus file and move to the next file
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
            
            %% check if MaxNoFutureCount is reached and procesing should be terminated 
            if(NoFutureCount >= MaxNoFutureCount)
                checking = 0;
                done = 1;
                fprintf(flog,'\nStopped Becuase No More Future Files Available\n');
            end
        end
    end
end




end

