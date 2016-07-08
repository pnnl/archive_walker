% Simple XML read of the simple XML demo file
% for Archive Walker reading
%
% This is the main script for the project. 
%
% Created April 29, 2016 by Frank Tuffner
%
% Updated June 21, 2016 by Tao Fu
%   update it to be the BAWS main script
%
% Updated June 24, 2016 by Tao Fu
%   1. added debugMode to write the output log file
%   2. removed fields that keeps processed data information in the DataInfo struture; 
%   3. added functions to put output PMU structure in a cell array
%   


%prepare workspace
close all;
clear all;
clc;

debugMode = 1; 

% add several file paths
addpath('..\ConfigXML');
addpath('..\DataProcessor');
addpath('..\DQandCustomization');
addpath('..\DQandCustomization\DQfilters');
addpath('..\');
%XML file
%XMLFile='ConfigXML2_Hybrid.xml';
% XMLFile = 'ConfigXML2_RealTime.xml';
%XMLFile = 'D:\BAWS\codes\New folder\ConfigXML2_Archive.xml';
% Parse XML file to MATLAB structure
XMLFile = 'ConfigXML_CSV.xml';
DataXML = fun_xmlread_comments(XMLFile);

%XML file
XMLFile='ProcessXML.xml';
% Parse XML file to MATLAB structure
ProcessXML = fun_xmlread_comments(XMLFile);

%XML file
XMLFile='DetectorConfig.xml';
% Parse XML file to MATLAB structure
DetectorXML = fun_xmlread_comments(XMLFile);

% DQ and customization are done in stages. Each stage is composed of a DQ
% step and a customization step.
if isfield(DataXML.Configuration,'Stages')
    NumDQandCustomStages = length(DataXML.Configuration.Stages);
    if NumDQandCustomStages == 1
        % By default, the contents of DataXML.Configuration.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        DataXML.Configuration.Stages = {DataXML.Configuration.Stages};
    end
else
    NumDQandCustomStages = 0;
end

% Processing is done in stages. Each stage is composed of a filtering step
% and a multirate step.
if isfield(ProcessXML.Configuration.Processing,'Stages')
    NumProcessingStages = length(ProcessXML.Configuration.Processing.Stages);
    if NumProcessingStages == 1
        % By default, the contents of ProcessXML.Configuration.Processing.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        ProcessXML.Configuration.Processing.Stages = {ProcessXML.Configuration.Processing.Stages};
    end
else
    NumProcessingStages = 0;
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
    if(strcmp(DataXML.Configuration.ReaderProperties.Mode.Name,'Hybrid'))
        DateTimeStart = DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
    else
       % we will use the current time as the start time; still need to consider time zone
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
% FilePath = [FileDirectory '\' FileMnemonic];

% FileDate = datestr(DateTimeStart(1:19),'_yyyymmdd_HHMMSS');
% FileName = [FilePath FileDate '.pdat'];

%% put data into a strucutre
flog = fopen('BAWS_processing_log.txt','w');
fprintf(flog, '********************************************************\n');
DataInfo.mode = DataXML.Configuration.ReaderProperties.Mode.Name;
DataInfo.FileDirectory = FileDirectory;
DataInfo.FileMnemonic = FileMnemonic;
DataInfo.FileType = DataXML.Configuration.ReaderProperties.FileType;

if(strcmp(DataInfo.mode, 'Archive'))
    fprintf(flog,'Mode = Archive\n');
    DataInfo.DateTimeStart = DateTimeStart(1:19);
    DataInfo.DateTimeEnd = DateTimeEnd(1:19);
    fprintf(flog, 'DateTimeStart =  %s\n',DataInfo.DateTimeStart);
    fprintf(flog, 'DateTimeEnd  =   %s\n',DataInfo.DateTimeEnd);
    
    % set some wait parameters for archive mode
    DataInfo.NoFutureWait = 0;
    DataInfo.MaxNoFutureCount = 1;
    
    DataInfo.FutureWait = 0;
    DataInfo.MaxFutureCount = 1;
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


%%
done = 0;

%% set DateTimeEnd for real time mode and hybrid mode
if(strcmp(DataInfo.mode, 'Hybrid'))
    % Hybrid Mode
    currT = now;    % current time
    DataInfo.DateTimeEnd = datestr(currT-DataInfo.RealTimeRange/60/24);   %RealTimeRang is converted from minutes to day
elseif(strcmp(DataInfo.mode, 'RealTime'))
    % realtime mode
    DataInfo.DateTimeEnd = [];
end


%% flag for processed files
DataInfo.tPMU = 0;  % time of the last measurement in the last processed file
DataInfo.lastFocusFile = ''; % last focus file
DataInfo.secondesToConcat = str2double(ProcessXML.Configuration.Processing.SecondsToConcat);
PMUall = {};
oneMinuteEmptyPMU = [];
emptyPMUexist = 0;

%% processing files
while(~done)
   [focusFile,done,outDataInfo] = getNextFocusFile(DataInfo,flog,debugMode);
   DataInfo = outDataInfo;
   if(~done)
       % focusFile is available
       if(debugMode)
           fprintf(flog,'\nThe current PMU file is: %s\n', focusFile);
       end
       try
           % make sure focusFile is written
           [stat,attr] = fileattrib(focusFile);
           while(~attr.UserWrite)
               % pause 0.5 seconds when the file is still being written
               pause(0.5);
               [stat,attr] = fileattrib(focusFile);
           end
           
           % ***********
           % Data Reader
           % ***********
           % Create the PMU structure
           DataInfo.lastFocusFile = focusFile;
           DataInfo.tPMU = 0;
           if(strcmpi(DataInfo.FileType, 'pdat'))
               % pdat format                  
               [PMU,tPMU,Num_Flags] = createPdatStruct(focusFile,DataXML);
           elseif(strcmpi(DataInfo.FileType, 'csv'))
               [PMU,tPMU,Num_Flags] = JSIS_CSV_2_Mat(focusFile,DataXML);
           end
           % Apply data quality filters and signal customizations
            PMU = DQandCustomization(PMU,DataXML,NumDQandCustomStages,Num_Flags);
           % Return only the desired PMUs and signals
           PMU = GetOutputSignals(PMU,DataXML);
           
           % Create an empty one minute PMU data structure for later use
           % after processing the first file
           % this only need to be called onece
           if(~emptyPMUexist)                  
               oneMinuteEmptyPMU = createOneMinuteEmptyPMU(PMU);
               emptyPMUexist = 1;
           end
           
           % **********************
           % Collect PMU Structures according to specified seconds 
           % **********************
            PMUall = preparePMUList(PMUall,PMU,oneMinuteEmptyPMU,DataInfo.secondesToConcat);               
            
            % Concatenate all the PMU structures on the list into one PMU structure for prcessing
            concatPMU = ConcatenatePMU(PMUall);            
            
           % **********
           % Processing
           % **********
           concatPMU = DataProcessor(concatPMU, ProcessXML, NumProcessingStages, FlagBitInterpo);
           % Return only the desired PMUs and signals
           concatPMU = GetOutputSignals(concatPMU,ProcessXML);
           
           % *********
           % Detection
           % *********
%            DetectionResults = RunDetection(concatPMU,DetectorXML);
           
           %% update some information
           DataInfo.tPMU = tPMU;
           if(debugMode)
               fprintf(flog, 'Number of PMUs in the file: %f\n',length(PMU));
               fprintf(flog, 'PMU start Time:  %s\n', datestr(tPMU(1),'mm/dd/yyyy HH:MM:SS:FFF'));
               fprintf(flog, 'PMU end Time:    %s\n', datestr(tPMU(end),'mm/dd/yyyy HH:MM:SS:FFF'));
           end
       catch msg1
           % failed to process the focus file           
           fprintf(flog, 'Could not process the current PMU dta file: %s\n',focusFile);
           fprintf(flog, '    %s\n',msg1.message);           
           
           % stop processing files
           done = 1;
           
           % throw the error message
           str = ['Could not process the current PMU dta file: ',focusFile];
           disp(str);           
           error(msg1.message);
           
       end
   end   
end

fprintf(flog, '\nFinished processing files\n');
fprintf(flog, '********************************************************\n');
fprintf(flog,'\n');