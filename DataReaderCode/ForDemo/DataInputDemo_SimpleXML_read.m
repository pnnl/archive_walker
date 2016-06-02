% Simple XML read of the simple XML demo file
% for Archive Walker reading
%
% Created April 29, 2016 by Frank Tuffner

%prepare workspace
% close all;
clear all;
% clc;

ThisFig = figure;

%XML file
XMLFile='XML4DEMO.xml';
% XMLFile='ConfigXML2_Hybrid.xml';
%XMLFile = 'ConfigXML2_RealTime.xml';
%XMLFile = 'ConfigXML2_Archive.xml';


% Parse XML file to MATLAB structure
DataXML = fun_xmlread_comments(XMLFile);

% DQ and customization are done in stages. Each stage is composed of a DQ
% step and a customization step.
NumStages = length(DataXML.Configuration.Stages);
if NumStages == 1
    % By default, the contents of DataXML.Configuration.Stages would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    DataXML.Configuration.Stages = {DataXML.Configuration.Stages};
end

% Get parameters for the operation mode
if strcmp(DataXML.Configuration.ReaderProperties.Mode.Name, 'Archive')
    % Archive-walker mode
        
    % Start time for processing
    DateTimeStart = DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
    % End time for processing
    DateTimeEnd = DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeEnd;
elseif (strcmp(DataXML.Configuration.ReaderProperties.Mode.Name, 'RealTime') || ...
        strcmp(DataXML.Configuration.ReaderProperties.Mode.Name, 'Hybrid'))
    % Real-time and Archiver mode parameters
    
    % Start time for processing
    DateTimeStart = DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeStart;

    % Wait time when no future data is available (seconds)
    NoFutureWait = str2num(DataXML.Configuration.ReaderProperties.Mode.Params.NoFutureWait);
    % Number of times to wait NoFutureWait seconds before stopping
    % execution because no data is available.
    MaxNoFutureCount = str2num(DataXML.Configuration.ReaderProperties.Mode.Params.MaxNoFutureCount);

    % Wait time when future data is available (seconds)
    FutureWait = str2num(DataXML.Configuration.ReaderProperties.Mode.Params.FutureWait);
    % Number of times to wait FutureWait seconds before jumping to the
    % next available file of data
    MaxFutureCount = str2num(DataXML.Configuration.ReaderProperties.Mode.Params.MaxFutureCount);
    
    if strcmp(DataXML.Configuration.ReaderProperties.Mode.Name, 'Hybrid')
        % Hybrid mode has an additional parameter
        
        % When the current file comes within RealTimeRange minutes of the
        % current system time it switches from archive to real-time modes.
        RealTimeRange = str2num(DataXML.Configuration.ReaderProperties.Mode.Params.RealTimeRange);
    end
else
    error('The mode of operation is not specified properly. Options: Archive, RealTime, and Hybrid.');
end

FileDirectory = DataXML.Configuration.ReaderProperties.FileDirectory;
FileMnemonic = DataXML.Configuration.ReaderProperties.Mnemonic;
FilePath = [FileDirectory '\' FileMnemonic];

FileDate = datestr(DateTimeStart(1:19),'_yyyymmdd_HHMMSS');
FileName = [FilePath FileDate '.pdat'];

%% put data into a strucutre, we may not need this for now, just in case we want to convert codes into functions
flog = fopen('BAWS_processing_log.txt','w');
fprintf(flog, '********************************************************\n');
DataInfo.mode = DataXML.Configuration.ReaderProperties.Mode.Name;
if(strcmp(DataInfo.mode, 'Archive'))
    fprintf(flog,'Mode = Archive\n');
    DataInfo.DateTimeStart = DateTimeStart(1:19);
    DataInfo.DateTimeEnd = DateTimeEnd(1:19);
    fprintf(flog, 'DateTimeStart =  %s\n',DataInfo.DateTimeStart);
    fprintf(flog, 'DateTimeEnd  =   %s\n',DataInfo.DateTimeEnd);
else
    % RealTime or Hybrid
    if strcmp(DataInfo.mode, 'Hybrid')
        fprintf(flog,'Mode  = Hybrid\n');
    else
        fprintf(flog,'Mode  = Real Time\n');
    end
    DataInfo.DateTimeStart = DateTimeStart(1:19);
    DataInfo.NoFutureWait = NoFutureWait;
    DataInfo.MaxNoFutureCount = MaxNoFutureCount;
    
    DataInfo.FutureWait = FutureWait;
    DataInfo.MaxFutureCount = MaxFutureCount;
    
    fprintf(flog, 'DateTimeStart    = %s\n', DataInfo.DateTimeStart);
    fprintf(flog, 'NoFutureWait     = %f\n', DataInfo.NoFutureWait);
    fprintf(flog, 'MaxNoFutureCount = %f\n', DataInfo.MaxNoFutureCount);
    fprintf(flog, 'FutureWait       = %f\n', DataInfo.FutureWait);
    fprintf(flog, 'MaxFutureCount   = %f\n', DataInfo.MaxFutureCount);
    
    if strcmp(DataInfo.mode, 'Hybrid')
        DataInfo.RealTimeRange = RealTimeRange;
        fprintf(flog, 'RealTimeRange    = %f\n,',DataInfo.RealTimeRange);
    end
end
fprintf(flog, '********************************************************\n');
fprintf(flog,'\n');

%% Setup for demo

MovePDAT4DEMO;

%% process files in Archive mode
% this is only for Archive mode and Hybrid mode
done = 0;   % flag to indicate if archive mode is done
prevFileList = {};
tPMU = 0;
if(strcmp(DataInfo.mode, 'Archive') || strcmp(DataInfo.mode, 'Hybrid'))
    fprintf(flog, 'processing archived files ....\n');
    while(~done)
        DateTimeStart = DataInfo.DateTimeStart;
        if(strcmp(DataInfo.mode, 'Archive'))
            % Archive mode
            DateTimeEnd = DataInfo.DateTimeEnd;
            done = 1;   % only get data file list once
        else
            % Hybrid Mode
            currT = now;    % current time
            DateTimeEnd = datestr(currT-DataInfo.RealTimeRange/60/24);   %RealTimeRang is converted from minutes to day
        end
        fileList = getArchivedFiles(FileDirectory, DateTimeStart, DateTimeEnd);
        newFiles = setdiff(fileList,prevFileList);
        prevFileList = fileList;
        if(isempty(newFiles))
            % no new files
            done = 1;
        else
            % process each file
            fileFlag = zeros(1,length(newFiles));   % a flag for file in the current day to indicate if the file is processed            
            for i = 1:length(newFiles)
                currFile = newFiles{i};
                fprintf(flog,'\nThe current PMU file is: %s\n', currFile);
                disp(['The current PMU file is: ', currFile]);
                try
                    [PMU,tPMU] = createPdatStructDemo(currFile,DataXML);      
                    PMU = DQandCustomization(PMU,DataXML,NumStages);
                    
                    t = datetime(PMU(3).Signal_Time.Time_String,'InputFormat','yyyy-MM-dd HH:mm:ss.SSS');
                    figure(ThisFig), hold on;
%                     plot(t,PMU(3).Data(:,5),'b--',...
%                         t,PMU(3).Data(:,6),'g--',...
%                         t,PMU(3).Data(:,7),'r')
                    x = PMU(3).Data(:,7);
                    x(x<-180) = x(x<-180)+360;
                    plot(t,x,'b')
                    title('Archive-Walker Mode');
                    hold off;
                    
                    fileFlag(i) = 1;
                    fprintf(flog, 'Number of PMUs in the file: %f\n',length(PMU));
                    fprintf(flog, 'PMU start Time:  %s\n', datestr(tPMU(1),'mm/dd/yyyy HH:MM:SS:FFF'));
                    fprintf(flog, 'PMU end Time:    %s\n', datestr(tPMU(end),'mm/dd/yyyy HH:MM:SS:FFF'));                    
                catch
                    fprintf(flog, 'Could not process the current PMU dta file: %s\n',currFile);
                end
            end
        end
    end   
    fprintf(flog, '\nFinished processing archived files\n');
    fprintf(flog, '********************************************************\n');
    fprintf(flog,'\n');

end





%% now process data in real time mode
if(strcmp(DataInfo.mode, 'RealTime') || strcmp(DataInfo.mode, 'Hybrid'))
    fprintf(flog, 'processing files in real time ....\n');
    % the file that will be processed next
    if(tPMU ~= 0)
        focusFileTime = ceil(tPMU(end)*24*3600)/24/3600; % need to figure out what to do when the last PMU data file is not readable
    else
        focusFileTime = datenum(DataInfo.DateTimeStart);
    end
    % find the name and folder of the focus file
    [focusFileFolder,focusFile] = getPdatFileFolder(FileDirectory,FileMnemonic,focusFileTime);
    fprintf(flog, 'The next focus file should be: %s\n',focusFile);
    fprintf(flog, 'The next focus file folder should be %s\n', focusFileFolder);
    defaultFileTimeDiff = 1/60/24;  % default time for each PMU
    
    running = 1;
    NoFutureCount = 0;
    FutureCount = 0;
    FutureFileAvailable = 0;
    while(running == 1)
        
        if(exist(focusFile,'file'))
            % check if the file is done written
            [stat,attr] = fileattrib(focusFile);
            while(~attr.UserWrite)
                % pause 1 seconde when the file is still being written
                pause(1);
            end
            %process the next file and move to the one after the next file
            fprintf(flog,'\nThe current PMU file is: %s\n', focusFile);
            
            try
                [PMU,tPMU] = createPdatStructDemo(focusFile,DataXML);
                PMU = DQandCustomization(PMU,DataXML,NumStages);
                
                t = datetime(PMU(3).Signal_Time.Time_String,'InputFormat','yyyy-MM-dd HH:mm:ss.SSS');
                figure(ThisFig), hold on;
                title('Real-Time Mode');
%                     plot(t,PMU(3).Data(:,5),'b--',...
%                         t,PMU(3).Data(:,6),'g--',...
%                         t,PMU(3).Data(:,7),'r')
                x = PMU(3).Data(:,7);
                x(x<-180) = x(x<-180)+360;
                plot(t,x,'b')
                hold off;
                    
                % update for the next focus file
                focusFileTime = ceil(tPMU(end)*24*3600)/24/3600;
                fprintf(flog, 'Number of PMUs in the file: %f\n',length(PMU));
                fprintf(flog, 'PMU start Time:  %s\n', datestr(tPMU(1),'mm/dd/yyyy HH:MM:SS:FFF'));
                fprintf(flog, 'PMU end Time:    %s\n', datestr(tPMU(end),'mm/dd/yyyy HH:MM:SS:FFF'));

            catch
                % failed to process the current focus file, move to the next file
                focusFileTime = focusFileTime+defaultFileTimeDiff;
                fprintf(flog, 'Could not process the current PMU dta file: %s\n',currFile);
                fprintf(flog, 'Move to the next file using the default file time difference\n');
            end
            [focusFileFolder,focusFile] = getPdatFileFolder(FileDirectory,FileMnemonic,focusFileTime);
            fprintf(flog, 'The next focus file should be: %s\n',focusFile);
            fprintf(flog, 'The next focus file folder should be %s\n', focusFileFolder);
            
            % reset counters
            NoFutureCount = 0;
            FutureCount = 0;
        else
            if(FutureCount > 0)
                % already detected future file
                % wait one more cycle
                %printf(flog, 'FutureCount = %d\n', FutureCount);
                
                figure(ThisFig)
                title('Waiting - future file is available');
                
                
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
                    nextFileFolder = getPdatFileFolder(FileDirectory,FileMnemonic,nextDayTime);
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
                            
                            figure(ThisFig)
                            title('Waiting - future file is available');
                            
                            
                            % start future file count                            
                            fprintf(flog,'\nstart FutureCount\n');
                            pause(FutureWait);
                            fprintf(flog, 'paused %f secondes\n',FutureWait);
                            FutureCount = 1;
                            NoFutureCount = 0;  % set NoFutureCount to 0
                            fprintf(flog,'FutureCount = %d\n',FutureCount);
                        end
                    end
                end
            end
            if(FutureCount == 0)
                % both focus file and future files don't exist, noFutureCountStart
                if(NoFutureCount > 0)
                    fprintf(flog,'NoFutureCount\n');
                else
                    fprintf(flog,'\nStart NoFutureCount\n');
                end
                
                figure(ThisFig)
                title('Waiting - future file is not available');
                
                pause(NoFutureWait);
                fprintf(flog,'paused %f seconds\n',NoFutureWait);
                NoFutureCount = NoFutureCount+1;
                fprintf(flog,'NoFutureCount = %d\n',NoFutureCount);
            end
        end
        
        if(FutureCount > MaxFutureCount)
            % pass this file
            %focusFileTime = focusFileTime+defaultFileTimeDiff; % may change to use the first future file
            %[focusFileFolder,focusFile] = getPdatFileFolder(FileDirectory,FileMnemonic,focusFileTime);
            
            % process the 1st fugure file
            futureFileList = sort(futureFileList);
            focusFile = futureFileList{1};
            fprintf(flog,'\nThe current PMU file is: %s\n', focusFile);
            try
                [PMU,tPMU] = createPdatStructDemo(focusFile,DataXML);
                PMU = DQandCustomization(PMU,DataXML,NumStages);
                
                t = datetime(PMU(3).Signal_Time.Time_String,'InputFormat','yyyy-MM-dd HH:mm:ss.SSS');
                figure(ThisFig), hold on;
%                     plot(t,PMU(3).Data(:,5),'b--',...
%                         t,PMU(3).Data(:,6),'g--',...
%                         t,PMU(3).Data(:,7),'r')
                x = PMU(3).Data(:,7);
                x(x<-180) = x(x<-180)+360;
                plot(t,x,'b')
                title('Real-Time Mode');
                hold off;
                    
                % update for the next focus file
                focusFileTime = ceil(tPMU(end)*24*3600)/24/3600;
                fprintf(flog, 'Number of PMUs in the file: %f\n',length(PMU));
                fprintf(flog, 'PMU start Time:  %s\n', datestr(tPMU(1),'mm/dd/yyyy HH:MM:SS:FFF'));
                fprintf(flog, 'PMU end Time:    %s\n', datestr(tPMU(end),'mm/dd/yyyy HH:MM:SS:FFF'));
            catch
                % failed to process the current focus file, move to the next file
                focusFileTime = focusFileTime+defaultFileTimeDiff;
                fprintf(flog, 'Could not process the current PMU dta file: %s\n',currFile);
                fprintf(flog, 'Move to the next file using the default file time difference\n');
            end
            [focusFileFolder,focusFile] = getPdatFileFolder(FileDirectory,FileMnemonic,focusFileTime);
            fprintf(flog, 'The next focus file should be: %s\n',focusFile);
            fprintf(flog, 'The next focus file folder should be %s\n', focusFileFolder);

            % reset counters
            NoFutureCount = 0;
            FutureCount = 0;
        end
        
        % check if procesing should be terminated
        if(NoFutureCount >= MaxNoFutureCount)
            running = 0;
            fprintf(flog,'\nStopped Becuase No More Future Files Available\n');
        end
    end   
end


    
%PMU = createPdatStruct(FileName,DataXML);