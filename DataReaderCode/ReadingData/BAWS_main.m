% Simple XML read of the simple XML demo file
% for Archive Walker reading
%
% Created April 29, 2016 by Frank Tuffner
%
% Updated June 21, 2016 by Tao Fu
%   update it to be the BAWS main script
%


%prepare workspace
close all;
clear all;
clc;


%XML file
%XMLFile='ConfigXML2_Hybrid.xml';
XMLFile = 'ConfigXML2_RealTime.xml';
%XMLFile = 'ConfigXML2_Archive.xml';


% Parse XML file to MATLAB structure
DataXML = fun_xmlread(XMLFile);

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
    if(strcmp(DataXML.Configuration.ReaderProperties.Mode.Name,'Hybrid'))
        DateTimeStart = DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
    else
       % we will use the current time as the start time
       currT = now;
       DateTimeStart = datestr(currT,'yyyy-mm-dd HH:MM:SS');        
    end

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
DataInfo.FileDirectory = FileDirectory;
DataInfo.FileMnemonic = FileMnemonic;

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
    DataInfo.DateTimeStart = DateTimeStart(1:19);  % why do we have a DataTimeStart in the real time mode?
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


%%
done = 0;

%% get the list of available file.
% these files will be processed in archive mode
if(strcmp(DataInfo.mode, 'Archive'))
    %archive node
    DateTimeStart = DataInfo.DateTimeStart;
    DateTimeEnd = DataInfo.DateTimeEnd;
elseif(strcmp(DataInfo.mode, 'Hybrid'))
    % Hybrid Mode
    DateTimeStart = DataInfo.DateTimeStart;
    currT = now;    % current time
    DateTimeEnd = datestr(currT-DataInfo.RealTimeRange/60/24);   %RealTimeRang is converted from minutes to day
elseif(strcmp(DataInfo.mode, 'RealTime'))
    % realtime mode
    DateTimeStart = DataInfo.DateTimeStart;
    DateTimeEnd = [];
end

if(~isempty(DateTimeEnd))
    %get current available files in archive mode and hybrid mode
    availableFiles = getArchivedFiles(FileDirectory,DateTimeStart,DateTimeEnd);
    if(~isempty(availableFiles))
        flagAvailableFiles = zeros(length(availableFiles),1);
    else
        flagAvailableFiles = [];
    end
else
    availableFiles = {};
    flagAvailableFiles = [];
end
DataInfo.availableFiles = availableFiles;
DataInfo.flagAvailableFiles = flagAvailableFiles;

% flag for processed files
DataInfo.processedFileList = {};
DataInfo.processedFileFlag = [];
DataInfo.lastFileTime = '';
DataInfo.tPMU = 0;  % time of the last measurement in the last processed file

%% processing files
while(~done)
   [focusFile,done,outDataInfo] = getNextFocusFile(DataInfo,flog);
   DataInfo = outDataInfo;
   if(~done)
       % focusFile is available
       fprintf(flog,'\nThe current PMU file is: %s\n', focusFile);
       try
           % make sure focusFile is written
           [stat,attr] = fileattrib(focusFile);
           while(~attr.UserWrite)
               % pause 0.5 seconds when the file is still being written
               pause(0.5);
           end
           [PMU,tPMU] = createPdatStruct(focusFile,DataXML);
           % update some information
           DataInfo.tPMU = tPMU;
           timeNum = getPdatFileTime(focusFile);
           DataInfo.lastFileTime = datestr(timeNum,'yyyy-mm-dd HH:MM:SS');
           DataInfo.processedFileList = [DataInfo.processedFileList;focusFile];
           DataInfo.processedFileFlag = [DataInfo.processedFileFlag;1];
           fprintf(flog, 'Number of PMUs in the file: %f\n',length(PMU));
           fprintf(flog, 'PMU start Time:  %s\n', datestr(tPMU(1),'mm/dd/yyyy HH:MM:SS:FFF'));
           fprintf(flog, 'PMU end Time:    %s\n', datestr(tPMU(end),'mm/dd/yyyy HH:MM:SS:FFF'));
       catch
           % failed to process the focus file
           DataInfo.processedFileList = [DataInfo.processedFileList;focusFile];
           DataInfo.processedFileFlag = [DataInfo.processedFileFlag;-1];                     
           fprintf(flog, 'Could not process the current PMU dta file: %s\n',currFile);
       end
   end   
end

fprintf(flog, '\nFinished processing files\n');
fprintf(flog, '********************************************************\n');
fprintf(flog,'\n');










