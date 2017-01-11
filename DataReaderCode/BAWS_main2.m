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
%   would be used by DataProcessor()
%
% Updated July 12, 2016 by Tao Fu
%   moved counting the maximum number of flags that will be needed
%   (Flag_Bit) from createPdatStruct() and JSIS_CSV_2_Mat() to this script
%
% Updated on July 26, 2016 by Tao Fu
%   added secondsNeeded as an input to ConcatenatePMU() function
%
% Updated on August 23, 2016 by Urmila Agrawal
%   1. added new parameter FlagContinue as an output from ConcatenatePMU
%   function. If FlagContinue is set to 1, the codes that follow afterward
%   for processing data is skipped for that loop, and continue to get
%   additional data file
%   2. Changed code such that the results can be updated after every
%   specific time interval, which is a user-defined input. This is achieved
%   by shifting data frame by the time given by user-defined time interval.
%   3. If a data file contains data for time exceeding SecondsToConcat, the
%   data in the beginning are no more discarded. The processor and
%   detection algorithm is applied to the data from beginning, and then
%   next segment of data is obtained by shifting data frame for further
%   processing.
%

%prepare workspace
% close all;
% clear all;
% clc;
clear;

debugMode = 1;

%XML file

XMLFile = 'ReaderConfig_Test_CSV.xml';
% Parse XML file to MATLAB structure
DataXML = fun_xmlread_comments(XMLFile);

%XML file
XMLFile='ProcessorConfig_Test_CSV.xml';
% Parse XML file to MATLAB structure
ProcessXML = fun_xmlread_comments(XMLFile);

%XML file
XMLFile='DetectorConfig_Test.xml';
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

%% identify the maximum number of flags that will be needed

% set the counter
count = 0;
% get the number of filtering stages
NumStages = length(DataXML.Configuration.Stages);
for StageId = 1:NumStages
    if isfield(DataXML.Configuration.Stages{StageId},'Filter')
        % number of filters used in this stage
        NumFilters = length(DataXML.Configuration.Stages{StageId}.Filter);
        if NumFilters ==1
            % By default, the contents of StageStruct.Customization
            % would not be in a cell array because length is one. This
            % makes it so the same indexing can be used in the following for loop.
            DataXML.Configuration.Stages{StageId}.Filter = {DataXML.Configuration.Stages{StageId}.Filter};
        end
        for FilterIdx = 1:NumFilters
            % count filters that used FlagBit as a parameter
            if isfield(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBit')
                Flag_Bit(count+1) = str2num(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBit);
                count = count + 1;
            end
            % counts filter that used FlagBitChan as a parameter
            if isfield(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitChan')
                Flag_Bit(count+1) = str2num(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitChan);
                count = count + 1;
            end
            % count filter that used FlagBitSamp as a parameter
            if isfield(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitSamp')
                Flag_Bit(count+1) = str2num(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitSamp);
                count = count + 1;
            end
            % count filter that used FlagBitFreq as a parameter
            if isfield(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitFreq')
                Flag_Bit(count+1) = str2num(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitFreq);
                count = count + 1;
            end
        end
    end
end
% save Flag_Bit information into DataXML structure
DataXML.Flag_Bit = Flag_Bit;

%% put some DATAXML information into a strucutre, which will be used in looking for the next focus file
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
DataInfo.secondsToConcat = str2double(ProcessXML.Configuration.Processing.SecondsToConcat);    % seconds of PMUs that need to be concatenated
PMUall = {}; % used to hold PMUs for concatenation
oneMinuteEmptyPMU = []; % a one minute empty PMU used for missing minutes
emptyPMUexist = 0;   % a flag used to identify if oneMinuteEmptyPMU exists
FlagBitInput = 1; %Bit used to indicate flagged input data to be processed
FlagBitInterpo = 2; %Bit used to indicate data is interpolated
NumFlagsProcessor = 2; % Number of bits used to indicate processed data that has been flagged for different cases
ResultUpdateInterval = [];
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
                [PMU,tPMU,Num_Flags] = createPdatStruct(focusFile,DataXML);
            elseif(strcmpi(DataInfo.FileType, 'csv'))
                % JSIS_CSV format
                [PMU,tPMU,Num_Flags] = JSIS_CSV_2_Mat(focusFile,DataXML);
            end
            DataInfo.tPMU = tPMU;
            %            Apply data quality filters and signal customizations
            PMU = DQandCustomization(PMU,DataXML,NumDQandCustomStages,Num_Flags, DataInfo.FileType);
            %            Return only the desired PMUs and signals
            PMU = GetOutputSignals(PMU,DataXML);
            
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
            if isempty(ResultUpdateInterval)
                % This if statement corresponds to the first few seconds
                % (or minutes) until enough files are concatenated for
                % further processing
                PMUall = preparePMUList(PMUall,PMU,oneMinuteEmptyPMU);
            else
                % Once the processing starts, PMURem contains remaining data from previous processed data
                % file(data corresponds to time duration(SecondsToConcat-ResultUpdateInterval)
                PMU_rem{1} = PMURem;
                PMUall = preparePMUList(PMU_rem,PMU,oneMinuteEmptyPMU);
            end
            
            % Concatenate all the PMU structures on the list into one PMU structure for processing
            [PMURem,FlagContinue] = ConcatenatePMU(PMUall,DataInfo.secondsToConcat);
            if FlagContinue ==1
                %Incase concatenated data file do not have enough data, then
                %it skips other processes and continues with next iteration
                continue;
            end
            %User-defined input for updating results after certain interval
            ResultUpdateInterval = str2num(DetectorXML.Configuration.ResultUpdateInterval);
            if ResultUpdateInterval > DataInfo.secondsToConcat
                error('Result update interval cannot exceed seconds to concatenate');
            end
            while(true)
                %This function extracts data segment corresponding to
                %SecondsToConcat for further processing
                [PMUsegment, PMURem,FlagBreak] = ExtractPMUsegment(PMURem,DataInfo.secondsToConcat,ResultUpdateInterval);
                
                % **********
                % Processing
                % **********
                
                %Data is processed for each data segment separately
                PMU_ProcessorOutput = DataProcessor(PMUsegment, ProcessXML, NumProcessingStages, FlagBitInterpo,FlagBitInput,NumFlagsProcessor);
                % Return only the desired PMUs and signals
                PMU_ProcessorOutput = GetOutputSignals(PMU_ProcessorOutput,ProcessXML);
                
                
                
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
                [DetectionResults, AdditionalOutput] = RunDetection(PMU_ProcessorOutput,DetectorXML);
                
                
                if FlagBreak==1
                    % When PMURem does not have enough data points, this
                    %while loop is skipped entirely
                    break
                end
            end
            
            
            %% update some information
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