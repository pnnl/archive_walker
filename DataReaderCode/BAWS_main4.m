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
% ConfigAll = fun_xmlread_comments('Config_JSIS_RingCSV.XML');
% ConfigAll = fun_xmlread_comments('Config_JSIS_FO.XML');
% ConfigAll = fun_xmlread_comments('Config_Wind.XML');
% ConfigAll = fun_xmlread_comments('Config_Wind_LongTrend.XML');
% ConfigAll = fun_xmlread_comments('Config_BigEdRing.XML');
% ConfigAll = fun_xmlread_comments('Config_AB_in_out.XML');
% ConfigAll = fun_xmlread_comments('Config_AB_in_out_ModeEst_NSB_LikeBPA.XML');
% ConfigAll = fun_xmlread_comments('Config_TimeErrorHunt.XML');
ConfigAll = fun_xmlread_comments('C:\Users\wang690\Desktop\projects\ArchiveWalker\detector\AWS_GUI_Dev\BAWGUI\bin\Debug\ConfigFiles\Config_MultFileTest_Customizations_2custPMU.XML');
DataXML = ConfigAll.Config.DataConfig;
ProcessXML = ConfigAll.Config.ProcessConfig;
DetectorXML = ConfigAll.Config.DetectorConfig;
clear Config

% These lists are passed to the RunDetection function to tell it which
% detectors to implement. The calls are separated because the FO detectors
% operate on a block of data sliding through time, while the event
% detectors operate on data as if it were streaming.
FOdetectors = {'Periodogram', 'SpectralCoherence'};
EventDetectors = {'Ringdown', 'OutOfRangeGeneral',...
    'OutOfRangeFrequency', 'WindRamp'};

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
        DateTimeStart = datestr(datetime('now','TimeZone','UTC'),'yyyy-mm-dd HH:MM:00');
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

%% Store filepaths from the configuration XML

try
    PathEventXML = DetectorXML.Configuration.EventPath;
catch
    error('The event XML path must be specified in the configuration file.');
end
% If the directory doesn't yet exist, add it.
if exist(PathEventXML,'dir') ~= 7
%     mkdir(PathEventXML);
end

try
    InitializationPath = ProcessXML.Configuration.InitializationPath;
catch
    error('The initialization path must be specified in the configuration file.');
end
% If the directory doesn't yet exist, add it.
if exist(InitializationPath,'dir') ~= 7
%     mkdir(InitializationPath);
end

%% identify the maximum number of flags that will be needed

% set the counter
count = 0;
Flag_Bit=[];
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
    %RealTimeRang is converted from minutes to day
    DataInfo.DateTimeEnd = datestr(datetime('now','TimeZone','UTC')-DataInfo.RealTimeRange/60/24,'yyyy-mm-dd HH:MM:SS');
elseif(strcmp(DataInfo.mode, 'RealTime'))
    % realtime mode
    DataInfo.DateTimeEnd = [];
end

%% flag for processed files
DataInfo.tPMU = 0;  % time of the last measurement in the last processed file
DataInfo.lastFocusFile = ''; % last focus file
DataInfo.LastFocusFileTime = [];    % Stores the datenum associated with the last focus file
DataInfo.FocusFileTime = [];    % Stores the datenum of the current focus file
PMUall = {}; % used to hold PMUs for concatenation
PMURem = [];
FlagBitInput = 1; %Bit used to indicate flagged input data to be processed
FlagBitInterpo = 2; %Bit used to indicate data is interpolated
NumFlagsProcessor = 2; % Number of bits used to indicate processed data that has been flagged for different cases

%% 

%User-defined input for updating results after certain interval
if isfield(DetectorXML.Configuration,'ResultUpdateInterval')
    ResultUpdateInterval = str2double(DetectorXML.Configuration.ResultUpdateInterval);
    SecondsToConcat = ResultUpdateInterval;
    
    temp = DetectorXML.Configuration;
    
    if isfield(temp,'Periodogram')
        if length(temp.Periodogram) == 1
            temp.Periodogram = {temp.Periodogram};
        end
        for idx = 1:length(temp.Periodogram)
            if isfield(temp.Periodogram{idx},'AnalysisLength')
                SecondsToConcat = max([SecondsToConcat str2double(temp.Periodogram{idx}.AnalysisLength)]);
            else
                error('AnalysisLength must be specified for the Periodogram-based forced oscillation detector.');
            end
        end
    end
    
    if isfield(temp,'SpectralCoherence')
        if length(temp.Periodogram) == 1
            temp.SpectralCoherence = {temp.SpectralCoherence};
        end
        for idx = 1:length(temp.SpectralCoherence)
            if isfield(temp.SpectralCoherence{idx},'AnalysisLength')
                SecondsToConcat = max([SecondsToConcat str2double(temp.SpectralCoherence{idx}.AnalysisLength)]);
            else
                error('AnalysisLength must be specified for the spectral coherence based forced oscillation detector.');
            end
        end
    end
else
    ResultUpdateInterval = [];
    SecondsToConcat = [];
end


%% Extract alarming parameters
AlarmingParams = struct('Periodogram',[],'SpectralCoherence',[],...
    'Ringdown',[],'OutOfRangeGeneral',[],'OutOfRangeFrequency',[],...
    'WindRamp',[]);
if isfield(DetectorXML.Configuration.Alarming,'Periodogram')
    AlarmingParams.Periodogram = ExtractAlarmingParamsPer(DetectorXML.Configuration.Alarming.Periodogram);
else
    AlarmingParams.Periodogram = ExtractAlarmingParamsPer(struct());
end
if isfield(DetectorXML.Configuration.Alarming,'SpectralCoherence')
    AlarmingParams.SpectralCoherence = ExtractAlarmingParamsSC(DetectorXML.Configuration.Alarming.SpectralCoherence);
else
    AlarmingParams.SpectralCoherence = ExtractAlarmingParamsSC(struct());
end
if isfield(DetectorXML.Configuration.Alarming,'Ringdown')
    AlarmingParams.Ringdown = ExtractAlarmingParamsRingdown(DetectorXML.Configuration.Alarming.Ringdown);
else
    AlarmingParams.Ringdown = ExtractAlarmingParamsRingdown(struct());
end


%% processing files

InitialCondosFilter = [];
InitialCondosMultiRate = [];
FinalAngles = [];
AdditionalOutput = [];
AdditionalOutputCondos = [];

EventList = struct();

PMUStruct = []; % used to store constant fields for PMU data structure after reading the 1st file
signalCounts = []; % used to store the number of signals in each PMU;
while(~done)
    [focusFile,done,outDataInfo,SkippedFiles] = getNextFocusFile(DataInfo,flog,debugMode);
    DataInfo = outDataInfo;
    
    % If the last focus file time is available
    %   AND
    % If the day of the last focus file is different than the day of the
    % current focus file
    %   THEN
    % Update the event list and store events that are over
    if (~isempty(DataInfo.LastFocusFileTime)) && ~strcmp(datestr(DataInfo.FocusFileTime,'yyyymmdd'),datestr(DataInfo.LastFocusFileTime,'yyyymmdd'))
        EventList = StoreEventList(EventList,PMU,DetectorXML,AdditionalOutput);
    end
%     WriteEventListXML(EventList,[PathEventXML '\EventList_Current.XML'],0);
%     WriteEventListXML(EventList,[PathEventXML '\EventList_Current_Bkup.XML'],0);
%     
%     if (~isempty(DataInfo.LastFocusFileTime))
%         EventList = StoreEventList(EventList,PMU,DetectorXML,AdditionalOutput);
%     end
    
    if(~done)
        % focusFile is available
        if(debugMode)
            fprintf(flog,'\nThe current PMU file is: %s\n', focusFile);
        end
%         try
            
            for FileIdx = 1:(SkippedFiles+1)
                if FileIdx < SkippedFiles+1
                    % Must generate a PMU structure filled with NaNs to
                    % represent the skipped file
                    PMU = basePMU;
                    for pmuIdx = 1:length(PMU)
                        PMU(pmuIdx).File_Name = 'Missing';
                        PMU(pmuIdx).Flag(:) = 1;
                        PMU(pmuIdx).Data(:) = NaN;
                        PMU(pmuIdx).Stat(:) = NaN;
                        tt = PMU(pmuIdx).Signal_Time.Signal_datenum;
                        PMU(pmuIdx).Signal_Time.Signal_datenum = tt + tt(end) + tt(2)-tt(1);
                        PMU(pmuIdx).Signal_Time.Time_String = {datestr(PMU(pmuIdx).Signal_Time.Signal_datenum,'mm/dd/yy HH:MM:SS.FFF')};
                    end
                    tPMU = PMU(pmuIdx).Signal_Time.Signal_datenum;
                else
                    % Proceed like normal and load the focus file
                    
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
                        [PMU,tPMU,Num_Flags,outStruct,outSignalCounts] = createPdatStruct(focusFile,DataXML,PMUStruct,signalCounts);
                        if(isempty(PMUStruct))
                            % initialize PMU structure fixed fields after reading the 1st file
                            PMUStruct = outStruct;
                            signalCounts = outSignalCounts;
                        end
                    elseif(strcmpi(DataInfo.FileType, 'csv'))
                        % JSIS_CSV format
                        [PMU,tPMU,Num_Flags] = JSIS_CSV_2_Mat(focusFile,DataXML);
                    end
                    basePMU = PMU;
                end
                DataInfo.tPMU = tPMU;
                %            Apply data quality filters and signal customizations
                PMU = DQandCustomization(PMU,DataXML,NumDQandCustomStages,Num_Flags, DataInfo.FileType);
                %            Return only the desired PMUs and signals
                PMU = GetOutputSignals(PMU,DataXML);
                
                
                % Save initialization information - filter conditions, etc.
                % that can be used to pick up the analysis at this minute.
                yyyymmdd = datestr(DataInfo.FocusFileTime,'yyyymmdd');
                hhmmss = datestr(DataInfo.FocusFileTime,'HHMMSS');
                InitializationFilePath = [InitializationPath yyyymmdd(1:4) '\' yyyymmdd(3:8) '\'];
                InitializationFile = [InitializationFilePath 'Initialization_' yyyymmdd '_' hhmmss '.mat'];
                % Only load an initialization file if this is the first
                % file loaded (indicated by an empty AdditionalOutput
                % structure)
                if isempty(AdditionalOutput)
                    if exist(InitializationFile,'file') == 2
                        load(InitializationFile)
                        AdditionalOutput = AdditionalOutputCondos;
                    end
                end
                % If the directory for the files hasn't been established
                % yet, add it.
                if exist(InitializationFilePath,'dir') == 0
                    mkdir(InitializationFilePath);
                end
                save(InitializationFile,...
                    'AdditionalOutputCondos','InitialCondosFilter','InitialCondosMultiRate','FinalAngles','DataXML','ProcessXML','DetectorXML');
                
                
                % **********
                % Processing
                % **********
                
                [PMU, InitialCondosFilter, InitialCondosMultiRate, FinalAngles] = DataProcessor(PMU, ProcessXML, NumProcessingStages, FlagBitInterpo,FlagBitInput,NumFlagsProcessor,InitialCondosFilter,InitialCondosMultiRate,FinalAngles);
                % Return only the desired PMUs and signals
                PMU = GetOutputSignals(PMU, ProcessXML);

                


                % *********
                % Detection
                % *********
                
                % Create an empty one minute PMU data structure for later use
                % after processing the first file
                % this only needs to be called once
                if exist('oneMinuteEmptyPMU','var') == 0
                    oneMinuteEmptyPMU = createOneMinuteEmptyPMU(PMU);
                end

                % Perform event detection on data from most recently loaded
                % file
                [DetectionResults, AdditionalOutput] = RunDetection(PMU,DetectorXML,EventDetectors,AdditionalOutput);
                EventList = UpdateEvents(DetectionResults,AdditionalOutput,DetectorXML,AlarmingParams,EventDetectors,EventList);
                                
                % Retrieve the segment of data that will be examined for forced
                % oscillations. A window of length SecondsToConcat slides
                % across this segment, advancing ResultUpdateInterval seconds
                % each step.
                [PMUall,PMURem,PMUsegmentAll,n] = RetrieveBlock(PMU,oneMinuteEmptyPMU,PMUall,PMURem,ResultUpdateInterval,SecondsToConcat);
                % Perform forced oscillation detection for each time that the
                % block can slide within the current segment of data
                for nn = 1:n+1
                    %This function extracts data segment corresponding to
                    %SecondsToConcat for further processing
                    [PMUsegment, PMUsegmentAll] = ExtractPMUsegment(PMUsegmentAll,SecondsToConcat,ResultUpdateInterval);

                    [DetectionResults, AdditionalOutput] = RunDetection(PMUsegment,DetectorXML,FOdetectors,AdditionalOutput);
                    EventList = UpdateEvents(DetectionResults,AdditionalOutput,DetectorXML,AlarmingParams,FOdetectors,EventList);
                end
                
                
                %% Clean AdditionalOutput to contain only initialization info
                AdditionalOutputCondos = AdditionalOutput;
                %
                if isfield(AdditionalOutputCondos,'Ringdown')
                    for DetIdx = 1:length(AdditionalOutputCondos)
                        if ~isempty(AdditionalOutputCondos(DetIdx).Ringdown)
                            FN = fieldnames(AdditionalOutputCondos(DetIdx).Ringdown);
                            AdditionalOutputCondos(DetIdx).Ringdown = rmfield(AdditionalOutputCondos(DetIdx).Ringdown,FN(~(strcmp(FN,'FilterConditions') | strcmp(FN,'NextThreshold'))));
                        end
                    end
                end

                %% update some information
                if(debugMode)
                    fprintf(flog, 'Number of PMUs in the file: %f\n',length(PMU));
                    fprintf(flog, 'PMU start Time:  %s\n', datestr(tPMU(1),'mm/dd/yyyy HH:MM:SS:FFF'));
                    fprintf(flog, 'PMU end Time:    %s\n', datestr(tPMU(end),'mm/dd/yyyy HH:MM:SS:FFF'));
                end
            end
%         catch msg1
%             % failed to process the focus file
%             fprintf(flog, 'Could not process the current PMU data file: %s\n',focusFile);
%             fprintf(flog, '    %s\n',msg1.message);
%             
%             % stop processing files
%             done = 1;
%             
%             % throw the error message
%             str = ['Could not process the current PMU data file: ',focusFile];
%             disp(str);
%             error(msg1.message);
%             
%         end
    end
end

fprintf(flog, '\nFinished processing files\n');
fprintf(flog, '********************************************************\n');
fprintf(flog,'\n');