%function [DataXML,ProcessXML,PostProcessCustXML,DetectorXML,WindAppXML,...
%     BlockDetectors,FileDetectors,NumDQandCustomStages,NumPostProcessCustomStages,...
%     NumProcessingStages,DataInfo,FileInfo,...
%     ResultUpdateInterval,SecondsToConcat,AlarmingParams,Num_Flags] = InitializeBAWS(ConfigAll)

function [DataXML,ProcessXML,PostProcessCustXML,DetectorXML,WindAppXML,...
    BlockDetectors,FileDetectors,NumDQandCustomStages,NumPostProcessCustomStages,...
    NumProcessingStages,DataInfo,FileInfo,...
    ResultUpdateInterval,SecondsToConcat,AlarmingParams,Num_Flags] = InitializeBAWS(ConfigAll)

DataXML = ConfigAll.Config.DataConfig.Configuration;
ProcessXML = ConfigAll.Config.ProcessConfig.Configuration;
PostProcessCustXML = ConfigAll.Config.PostProcessCustomizationConfig.Configuration;
DetectorXML = ConfigAll.Config.DetectorConfig.Configuration;
WindAppXML = ConfigAll.Config.WindAppConfig.Configuration;
%ModeMeterXML = ConfigAll.Config.ModeMeterConfig.Configuration;
clear ConfigAll

% These lists are passed to the RunDetection function to tell it which
% detectors to implement. The calls are separated because the block detectors
% operate on a block of data sliding through time, while the file
% detectors operate on data as if it were streaming.
BlockDetectors = {'Periodogram', 'SpectralCoherence', 'Thevenin','ModeMeter'};
FileDetectors = {'Ringdown', 'OutOfRangeGeneral','WindRamp'};

if length(DataXML.ReaderProperties.FilePath) == 1
    % Places the contents in a cell array so that indexing is the same as
    % if there were multiple FilePath entries in DataXML.
    DataXML.ReaderProperties.FilePath = {DataXML.ReaderProperties.FilePath};
end

% DQ and customization are done in stages. Each stage is composed of a DQ
% step and a customization step.
if isfield(DataXML,'Stages')
    NumDQandCustomStages = length(DataXML.Stages);
    if NumDQandCustomStages == 1
        % By default, the contents of DataXML.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        DataXML.Stages = {DataXML.Stages};
    end
else
    NumDQandCustomStages = 0;
end

% Post processing stages
if isfield(PostProcessCustXML,'Stages')
    NumPostProcessCustomStages = length(PostProcessCustXML.Stages);
    if NumPostProcessCustomStages == 1
        % By default, the contents of PostProcessCustXML.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used.
        PostProcessCustXML.Stages = {PostProcessCustXML.Stages};
    end
else
    NumPostProcessCustomStages = 0;
end

% Processing is done in stages. Each stage is composed of a filtering step
% and a multirate step.
if isfield(ProcessXML.Processing,'Stages')
    NumProcessingStages = length(ProcessXML.Processing.Stages);
    if NumProcessingStages == 1
        % By default, the contents of ProcessXML.Processing.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        ProcessXML.Processing.Stages = {ProcessXML.Processing.Stages};
    end
else
    NumProcessingStages = 0;
end

% Wind application
if isfield(WindAppXML,'PMU')
    if length(WindAppXML.PMU) == 1
        % Places the contents in a cell array so that indexing is the same as
        % if there were multiple PMU entries in WindAppXML.
        WindAppXML.PMU = {WindAppXML.PMU};
    end
    for PMUidx = 1:length(WindAppXML.PMU)
        if length(WindAppXML.PMU{PMUidx}.PowerChannel) == 1
            % Places the contents in a cell array so that indexing is the same as
            % if there were multiple PowerChannel entries for the PMU.
            WindAppXML.PMU{PMUidx}.PowerChannel = {WindAppXML.PMU{PMUidx}.PowerChannel};
        end
    end
    WindAppXML.WindValues = {};
end


% Get parameters for the operation mode
if strcmp(DataXML.ReaderProperties.Mode.Name, 'Archive')
    % Archive-walker mode
    
    % Start time for processing
    DateTimeStart = DataXML.ReaderProperties.Mode.Params.DateTimeStart;
    % End time for processing
    DateTimeEnd = DataXML.ReaderProperties.Mode.Params.DateTimeEnd;
elseif (strcmp(DataXML.ReaderProperties.Mode.Name, 'RealTime') || ...
        strcmp(DataXML.ReaderProperties.Mode.Name, 'Hybrid'))
    % Real-time and Archiver mode parameters
    
    % Start time for processing
    if(strcmp(DataXML.ReaderProperties.Mode.Name,'Hybrid'))
        DateTimeStart = DataXML.ReaderProperties.Mode.Params.DateTimeStart;
    else
        % we will use the current time as the start time; still need to consider time zone
        DateTimeStart = datestr(datetime('now','TimeZone',DataXML.ReaderProperties.Mode.Params.TimeZone),'yyyy-mm-dd HH:MM:00');
    end
    
    % Wait time when no future data is available (seconds)
    NoFutureWait = str2num(DataXML.ReaderProperties.Mode.Params.NoFutureWait);
    % Number of times to wait NoFutureWait seconds before stopping
    % execution because no data is available.
    MaxNoFutureCount = str2num(DataXML.ReaderProperties.Mode.Params.MaxNoFutureCount);
    
    % Wait time when future data is available (seconds)
    FutureWait = str2num(DataXML.ReaderProperties.Mode.Params.FutureWait);
    % Number of times to wait FutureWait seconds before jumping to the
    % next available file of data
    MaxFutureCount = str2num(DataXML.ReaderProperties.Mode.Params.MaxFutureCount);
    
    if strcmp(DataXML.ReaderProperties.Mode.Name, 'Hybrid')
        % Hybrid mode has an additional parameter
        
        % When the current file comes within RealTimeRange minutes of the
        % current system time it switches from archive to real-time modes.
        RealTimeRange = str2num(DataXML.ReaderProperties.Mode.Params.RealTimeRange);
    end
else
    error('The mode of operation is not specified properly. Options: Archive, RealTime, and Hybrid.');
end

%% identify the maximum number of flags that will be needed

% set the counter
count = 0;
% get the number of filtering stages
NumStages = length(DataXML.Stages);
NumFilters = 0;
for StageId = 1:NumStages
    if isfield(DataXML.Stages{StageId},'Filter')
        % number of filters used in this stage
        NumFilters = length(DataXML.Stages{StageId}.Filter);
        if NumFilters ==1
            % By default, the contents of StageStruct.Customization
            % would not be in a cell array because length is one. This
            % makes it so the same indexing can be used in the following for loop.
            DataXML.Stages{StageId}.Filter = {DataXML.Stages{StageId}.Filter};
        end
        for FilterIdx = 1:NumFilters
            % count filters that used FlagBit as a parameter
            if isfield(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBit')
                Flag_Bit(count+1) = str2num(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBit);
                count = count + 1;
            end
            % counts filter that used FlagBitChan as a parameter
            if isfield(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitChan')
                Flag_Bit(count+1) = str2num(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitChan);
                count = count + 1;
            end
            % count filter that used FlagBitSamp as a parameter
            if isfield(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitSamp')
                Flag_Bit(count+1) = str2num(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitSamp);
                count = count + 1;
            end
            % count filter that used FlagBitFreq as a parameter
            if isfield(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBitFreq')
                Flag_Bit(count+1) = str2num(DataXML.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBitFreq);
                count = count + 1;
            end
        end
    end
end
% save Flag_Bit information into DataXML structure
if ((NumStages == 0) || (NumFilters == 0))
    DataXML.Flag_Bit = [];
    PostProcessCustXML.Flag_Bit = [];
else
    DataXML.Flag_Bit = Flag_Bit;
    PostProcessCustXML.Flag_Bit = Flag_Bit;
end

% Add 2 extra flags
% the first additional bit is flagged when the customized signal uses flagged input signal
% the second additional input is if the customized signal was not created because of some error in user input.
if isempty(DataXML.Flag_Bit)
    Num_Flags = 2;
else
    Num_Flags = max(DataXML.Flag_Bit)+2; 
end

clear NumFilters

%% put some DATAXML information into a strucutre, which will be used in looking for the next focus file
DataInfo.mode = DataXML.ReaderProperties.Mode.Name;

EmptyCell = cell(1,length(DataXML.ReaderProperties.FilePath));
FileInfo = struct('FileMnemonic',EmptyCell,'FileType',EmptyCell,'lastFocusFile',EmptyCell,'tPMU',EmptyCell);
for idx = 1:length(FileInfo)
    FileInfo(idx).FileMnemonic = DataXML.ReaderProperties.FilePath{idx}.Mnemonic;
    FileInfo(idx).FileType = DataXML.ReaderProperties.FilePath{idx}.FileType;
end

if(strcmp(DataInfo.mode, 'Archive'))
    DataInfo.DateTimeStart = DateTimeStart(1:19);
    DataInfo.DateTimeEnd = DateTimeEnd(1:19);
    
    % set some wait parameters for archive mode
    DataInfo.NoFutureWait = 0;
    DataInfo.MaxNoFutureCount = 1;
    
    DataInfo.FutureWait = 0;
    DataInfo.MaxFutureCount = 1;
else
    % RealTime or Hybrid
    DataInfo.DateTimeStart = DateTimeStart(1:19);
    DataInfo.NoFutureWait = NoFutureWait;
    DataInfo.MaxNoFutureCount = MaxNoFutureCount;
    
    DataInfo.FutureWait = FutureWait;
    DataInfo.MaxFutureCount = MaxFutureCount;
    
    if strcmp(DataInfo.mode, 'Hybrid')
        DataInfo.RealTimeRange = RealTimeRange;
        DataInfo.TimeZone = DateTimeStart(21:end);
    end
end

%% set DateTimeEnd for real time mode (in Hybrid mode it is updated after
% each file  in getFocusFiles.
if(strcmp(DataInfo.mode, 'RealTime'))
    % realtime mode
    DataInfo.DateTimeEnd = [];
end

DataInfo.LastFocusFileTime = [];    % Stores the datenum associated with the last focus file

%%

%User-defined input for updating results after certain interval
if isfield(DetectorXML,'ResultUpdateInterval')
    ResultUpdateInterval = str2double(DetectorXML.ResultUpdateInterval);
    SecondsToConcat = ResultUpdateInterval;
end
temp = DetectorXML;

if isfield(temp,'Periodogram')
    if length(temp.Periodogram) == 1
        temp.Periodogram = {temp.Periodogram};
    end
    for idx = 1:length(temp.Periodogram)
        % Run the function that extracts parameters. Normally it takes
        % the sampling rate as an input and returns lengths in samples.
        % By setting fs=1 at the input, it returns lengths in seconds,
        % as required.
        ExtractedParameters = ExtractFOdetectionParamsPer(temp.Periodogram{idx},1);
        SecondsToConcat = max([SecondsToConcat ExtractedParameters.AnalysisLength]);
    end
end

if isfield(temp,'SpectralCoherence')
    if length(temp.SpectralCoherence) == 1
        temp.SpectralCoherence = {temp.SpectralCoherence};
    end
    for idx = 1:length(temp.SpectralCoherence)
        % Run the function that extracts parameters. Normally it takes
        % the sampling rate as an input and returns lengths in samples.
        % By setting fs=1 at the input, it returns lengths in seconds,
        % as required.
        ExtractedParameters = ExtractFOdetectionParamsSC(temp.SpectralCoherence{idx},1);
        SecondsToConcat = max([SecondsToConcat ExtractedParameters.AnalysisLength+ExtractedParameters.Delay*(ExtractedParameters.NumberDelays-1)]);
    end
end

if isfield(temp,'Thevenin')
    if length(temp.Thevenin) == 1
        temp.Thevenin = {temp.Thevenin};
        
        % The Thevenin detectors need ResultUpdateInterval (most other
        % detectors do not), so add it to the Thevenin portion of the
        % configuration structure. (single detector)
        DetectorXML.Thevenin.ResultUpdateInterval = ResultUpdateInterval;
    else
        % The Thevenin detectors need ResultUpdateInterval (most other
        % detectors do not), so add it to the Thevenin portion of the
        % configuration structure. (multiple detectors)
        for idx = 1:length(temp.Thevenin)
            DetectorXML.Thevenin{idx}.ResultUpdateInterval = ResultUpdateInterval;
        end
    end
    for idx = 1:length(temp.Thevenin)
        if isfield(temp.Thevenin{idx}, 'AnalysisLength')
            SecondsToConcat = max([SecondsToConcat str2double(temp.Thevenin{idx}.AnalysisLength)]);
        end
    end
end

%Calculating SecondsToConcat using user-specified input for
%analysislength for modemeter algorithm
if isfield(temp,'ModeMeter')
    if length(temp.ModeMeter) == 1
        % The modal analysis algorithm needs ResultUpdateInterval, so
        %add it to the Thevenin portion of the configuration structure.
        temp.ModeMeter = {temp.ModeMeter};
        DetectorXML.ModeMeter.ResultUpdateInterval = ResultUpdateInterval;
    else
        for idx = 1:length(temp.ModeMeter)
            % The modal analysis algorithm needs ResultUpdateInterval, so
            %add it to the Thevenin portion of the configuration structure.
            DetectorXML.ModeMeter{idx}.ResultUpdateInterval = ResultUpdateInterval;
        end
    end
    for idx = 1:length(temp.ModeMeter)
        if isfield(temp.ModeMeter{idx}, 'AnalysisLength')
            SecondsToConcat = max([SecondsToConcat str2double(temp.ModeMeter{idx}.AnalysisLength)]);
        end
    end
end


%% Extract alarming parameters
AlarmingParams = struct('Periodogram',[],'SpectralCoherence',[],...
    'Ringdown',[],'OutOfRangeGeneral',[],'WindRamp',[],'Thevenin',[],'ModeMeter',[]);
if isfield(DetectorXML.Alarming,'Periodogram')
    AlarmingParams.Periodogram = ExtractAlarmingParamsPer(DetectorXML.Alarming.Periodogram);
else
    AlarmingParams.Periodogram = ExtractAlarmingParamsPer(struct());
end
if isfield(DetectorXML.Alarming,'SpectralCoherence')
    AlarmingParams.SpectralCoherence = ExtractAlarmingParamsSC(DetectorXML.Alarming.SpectralCoherence);
else
    AlarmingParams.SpectralCoherence = ExtractAlarmingParamsSC(struct());
end
if isfield(DetectorXML.Alarming,'Ringdown')
    AlarmingParams.Ringdown = ExtractAlarmingParamsRingdown(DetectorXML.Alarming.Ringdown);
else
    AlarmingParams.Ringdown = ExtractAlarmingParamsRingdown(struct());
end
% if isfield(DetectorXML.Alarming,'ModeMeter')
%     AlarmingParams.ModeMeter = ExtractAlarmingParamsModeMeter(DetectorXML.Alarming.Ringdown);
% else
%     AlarmingParams.Modemeter = ExtractAlarmingParamsModeMeter(struct());
% end