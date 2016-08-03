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
% Updated July 11, 2016 by Urmila Agrawal
%   added 3 variables: flagBitInput, FlagBitInterpo, and NumFlags, which
%   would be used by concatenatePMU() and DataProcessor()
%
% Updated July 12, 2016 by Tao Fu
%   moved counting the maximum number of flags that will be needed
%   (Flag_Bit) from createPdatStruct() and JSIS_CSV_2_Mat() to this script
%
% Updated on July 26, 2016 by Tao Fu
%   added secondsNeeded as an input to ConcatenatePMU() function
%
% Updated on July 28, 2016 by Urmila Agrawal
%   added DataInfo.FileTypeas an an input to DQandCustomization() function
%
%prepare workspace
% close all;
% clear all;
% clc;
clear;

debugMode = 1;

% add several file paths
addpath('..\ConfigXML');
addpath('..\DataProcessor');
addpath('..\DQandCustomization');
addpath('..\DQandCustomization\DQfilters');
addpath('..\DataProcessor\Filter');
addpath('..\DQandCustomization\SignalCustomization');
addpath('..\Detector');
% addpath('..\Detector\PeriodogramDetector');
% addpath('..\Detector\SpectralCoherenceDetector');
addpath('..\');

%XML file
ConfigFile = 'DataConfig_JSIS.xml';
%XMLFile = 'DataConfig_CSV.xml';

% Parse XML file to MATLAB structure
DataConfig = fun_xmlread_comments(ConfigFile);

%XML file
ConfigFile='ProcessConfig_JSIS.xml';
%XMLFile='ProcessConfig_CSV.xml';

% Parse XML file to MATLAB structure
ProcessConfig = fun_xmlread_comments(ConfigFile);

%XML file
ConfigFile='DetectorConfig_JSIS.xml';
% Parse XML file to MATLAB structure
DetectorConfig = fun_xmlread_comments(ConfigFile);

% DQ and customization are done in stages. Each stage is composed of a DQ
% step and a customization step.
if isfield(DataConfig.Configuration,'Stages')
    NumDQandCustomStages = length(DataConfig.Configuration.Stages);
    if NumDQandCustomStages == 1
        % By default, the contents of DataXML.Configuration.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        DataConfig.Configuration.Stages = {DataConfig.Configuration.Stages};
    end
else
    NumDQandCustomStages = 0;
end

% Processing is done in stages. Each stage is composed of a filtering step
% and a multirate step.
if isfield(ProcessConfig.Configuration.Processing,'Stages')
    NumProcessingStages = length(ProcessConfig.Configuration.Processing.Stages);
    if NumProcessingStages == 1
        % By default, the contents of ProcessXML.Configuration.Processing.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        ProcessConfig.Configuration.Processing.Stages = {ProcessConfig.Configuration.Processing.Stages};
    end
else
    NumProcessingStages = 0;
end

% Get parameters for the operation mode
if strcmp(DataConfig.Configuration.ReaderProperties.Mode.Name, 'Archive')
    % Archive-walker mode
    
    % Start time for processing
    DateTimeStart = DataConfig.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
    % End time for processing
    DateTimeEnd = DataConfig.Configuration.ReaderProperties.Mode.Params.DateTimeEnd;
elseif (strcmp(DataConfig.Configuration.ReaderProperties.Mode.Name, 'RealTime') || ...
        strcmp(DataConfig.Configuration.ReaderProperties.Mode.Name, 'Hybrid'))
    % Real-time and Archiver mode parameters
    
    % Start time for processing
    if(strcmp(DataConfig.Configuration.ReaderProperties.Mode.Name,'Hybrid'))
        DateTimeStart = DataConfig.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
    else
        % we will use the current time as the start time; still need to consider time zone
        currT = now;
        DateTimeStart = datestr(currT,'yyyy-mm-dd HH:MM:SS');
    end
    
    % Wait time when no future data is available (seconds)
    NoFutureWait = str2num(DataConfig.Configuration.ReaderProperties.Mode.Params.NoFutureWait);
    % Number of times to wait NoFutureWait seconds before stopping
    % execution because no data is available.
    MaxNoFutureCount = str2num(DataConfig.Configuration.ReaderProperties.Mode.Params.MaxNoFutureCount);
    
    % Wait time when future data is available (seconds)
    FutureWait = str2num(DataConfig.Configuration.ReaderProperties.Mode.Params.FutureWait);
    % Number of times to wait FutureWait seconds before jumping to the
    % next available file of data
    MaxFutureCount = str2num(DataConfig.Configuration.ReaderProperties.Mode.Params.MaxFutureCount);
    
    if strcmp(DataConfig.Configuration.ReaderProperties.Mode.Name, 'Hybrid')
        % Hybrid mode has an additional parameter
        
        % When the current file comes within RealTimeRange minutes of the
        % current system time it switches from archive to real-time modes.
        RealTimeRange = str2num(DataConfig.Configuration.ReaderProperties.Mode.Params.RealTimeRange);
    end
else
    error('The mode of operation is not specified properly. Options: Archive, RealTime, and Hybrid.');
end

FileDirectory = DataConfig.Configuration.ReaderProperties.FileDirectory;
FileMnemonic = DataConfig.Configuration.ReaderProperties.Mnemonic;
% FilePath = [FileDirectory '\' FileMnemonic];

% FileDate = datestr(DateTimeStart(1:19),'_yyyymmdd_HHMMSS');
% FileName = [FilePath FileDate '.pdat'];

%% identify the maximum number of flags that will be needed

% set the counter
count = 0;
% get the number of filtering stages
NumStages = length(DataConfig.Configuration.Stages);
for StageId = 1:NumStages
    if isfield(DataConfig.Configuration.Stages{StageId},'Filter')
        % number of filters used in this stage
        NumFilters = length(DataConfig.Configuration.Stages{StageId}.Filter);
        if NumFilters ==1
            % By default, the contents of StageStruct.Customization
            % would not be in a cell array because length is one. This
            % makes it so the same indexing can be used in the following for loop.
            DataConfig.Configuration.Stages{StageId}.Filter = {DataConfig.Configuration.Stages{StageId}.Filter};
        end
        for FilterIdx = 1:NumFilters
            % count filters that used FlagBit as a parameter
            if isfield(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBit')
                Flag_Bit(count+1) = str2num(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBit);
                count = count + 1;
            end
            % counts filter that used FlagBitChan as a parameter
            if isfield(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitChan')
                Flag_Bit(count+1) = str2num(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitChan);
                count = count + 1;
            end
            % count filter that used FlagBitSamp as a parameter
            if isfield(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitSamp')
                Flag_Bit(count+1) = str2num(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitSamp);
                count = count + 1;
            end
            % count filter that used FlagBitFreq as a parameter
            if isfield(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitFreq')
                Flag_Bit(count+1) = str2num(DataConfig.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitFreq);
                count = count + 1;
            end
        end
    end
end
% save Flag_Bit information into DataXML structure
DataConfig.Flag_Bit = Flag_Bit;

%% put some DATAXML information into a strucutre, which will be used in looking for the next focus file
flog = fopen('BAWS_processing_log.txt','w');
fprintf(flog, '********************************************************\n');
DataInfo.mode = DataConfig.Configuration.ReaderProperties.Mode.Name;
DataInfo.FileDirectory = FileDirectory;
DataInfo.FileMnemonic = FileMnemonic;
DataInfo.FileType = DataConfig.Configuration.ReaderProperties.FileType;

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
DataInfo.secondesToConcat = str2double(ProcessConfig.Configuration.Processing.SecondsToConcat);    % seconds of PMUs that need to be concatenated
PMUall = {}; % used to hold PMUs for concatenation
oneMinuteEmptyPMU = []; % a one minute empty PMU used for missing minutes
emptyPMUexist = 0;   % a flag used to identify if oneMinuteEmptyPMU exists
FlagBitInput = 1; %Bit used to indicate flagged input data to be processed
FlagBitInterpo = 2; %Bit used to indicate data is interpolated
NumFlagsProcessor = 2; % Number of bits used to indicate processed data that has been flagged

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
                % check if the file is ready for reading
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
                [PMU,tPMU,Num_Flags] = createPdatStruct(focusFile,DataConfig);
            elseif(strcmpi(DataInfo.FileType, 'csv'))
                % JSIS_CSV format
                [PMU,tPMU,Num_Flags] = JSIS_CSV_2_Mat(focusFile,DataConfig);
            end
            
%             PMU(1).Data(2:11,1) = 200000 + randn(10,1);
%             PMU(1).Data(21:100,1) = 100000;
            
%           Apply data quality filters and signal customizations
            PMU = DQandCustomization(PMU,DataConfig,NumDQandCustomStages,Num_Flags,DataInfo.FileType);
            %            Return only the desired PMUs and signals
            PMU = GetOutputSignals(PMU,DataConfig);
            
            % Create an empty one minute PMU data structure for later use
            % after processing the first file
            % this only needs to be called once
            if(~emptyPMUexist)
                oneMinuteEmptyPMU = createOneMinuteEmptyPMU(PMU);
                emptyPMUexist = 1;
            end
            
            % **********************
            % Collect PMU Structures according to specified seconds
            % **********************
            PMUall = preparePMUList(PMUall,PMU,oneMinuteEmptyPMU,DataInfo.secondesToConcat);
            
            % Concatenate all the PMU structures on the list into one PMU structure for prcessing
            concatPMU = ConcatenatePMU(PMUall,DataInfo.secondesToConcat);
            
            % **********
            % Processing
            % **********
            PMU_ProcessorOutput = DataProcessor(concatPMU, ProcessConfig, NumProcessingStages, FlagBitInterpo,FlagBitInput,NumFlagsProcessor);
            % Return only the desired PMUs and signals
            PMU_ProcessorOutput = GetOutputSignals(PMU_ProcessorOutput,ProcessConfig);
            
            ttt = (1:size(PMU_ProcessorOutput.Data,1))/60;
            figure, plot(ttt,PMU_ProcessorOutput.Data);
            
            % *********
            % Detection
            % *********
            % Implementation Note:
            % The ringdown detector slides a window across the data and
            % calculates energy. After adding a new minute of data, many of
            % these energy calculations will be redone for certain (but
            % common) parameter setups. To avoid this, the energies and
            % other necessary information could be stored in
            % AdditionalOutput. After being returned here, they could be
            % stored as extra information in DetectorXML for use by the
            % detector the next time it is called to reduce computations. A
            % similar strategy could be useful when implementing the other
            % detectors too.
            [DetectionResults, AdditionalOutput] = RunDetection(PMU_ProcessorOutput,DetectorConfig);
            
            %% update some information
            DataInfo.tPMU = tPMU;
            if(debugMode)
                fprintf(flog, 'Number of PMUs in the file: %f\n',length(PMU));
                fprintf(flog, 'PMU start Time:  %s\n', datestr(tPMU(1),'mm/dd/yyyy HH:MM:SS:FFF'));
                fprintf(flog, 'PMU end Time:    %s\n', datestr(tPMU(end),'mm/dd/yyyy HH:MM:SS:FFF'));
            end
        catch msg1
            % failed to process the focus file
            fprintf(flog, 'Could not process the current PMU data file: %s\n',focusFile);
            fprintf(flog, '    %s\n',msg1.message);
            
            % stop processing files
            done = 1;
            
            % throw the error message
            str = ['Could not process the current PMU data file: ',focusFile];
            disp(str);
            error(msg1.message);
            
        end
    end
end

fprintf(flog, '\nFinished processing files\n');
fprintf(flog, '********************************************************\n');
fprintf(flog,'\n');