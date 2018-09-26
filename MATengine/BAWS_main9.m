% [DetectionResultsRerun, AdditionalOutputRerun] = BAWS_main9(varargin)
%
% This is the main function used to generate results (normal mode when
% called by RunNormalMode) or to rerun analyses to retrieve detailed
% operation information (rerun mode when called by RerunThevenin,
% RerunForcedOscillation, RerunOutOfRange, or RerunRingdown). In Rerun
% mode, additional inputs are required.
%
% Called by: 
%   RunNormalMode
%   RerunThevenin
%   RerunForcedOscillation
%   RerunOutOfRange
%   RerunRingdown
%
% Calls: 
%   AddMissingToSparsePMU
%   AddToSparsePMU
%   CleanAdditionalOutput
%   ConcatenatePMU
%   CreatePMUsegment
%   DataProcessor
%   DisableDetectors
%   DQandCustomization
%   fun_xmlread_comments
%   GenerateWindAppReport
%   getFocusFiles
%   GetOutputSignals
%   GetSparseData
%   InitializeBAWS
%   LoadFocusFiles
%   RetrieveInitializationFile
%   RunDetection
%   StoreEventList
%   UpdateEvents
%   UpdatePMUconcat
%   WindApplication
%   WriteEventListXML
%
% Inputs:
%   ControlPath - Path to folders containing Run.txt and Pause.txt files
%       written by the GUI to control the AW engine. A string.
%   EventPath - Path to the folder where results from detectors are to be
%       stored. A string.
%   InitializationPath - Path to the folder where initialization files
%       (used in rerun mode to recreate detailed results) and sparse data
%       (max and min of analyzed signals) are stored. A string.
%   FileDirectory - Paths to where PMU data that is to be analyzed is
%       stored. Cell array of strings.
%   ConfigFile - Path to the configuration XML used to configure the AW
%       engine for a run.
%   Additional inputs required for Rerun mode:
%       RerunStartTime - String specifying the start time for the run in the
%           format MM/DD/YYYY HH:MM:SS 
%      RerunEndTime - String specifying the end time for the run in the format
%           MM/DD/YYYY HH:MM:SS
%       RerunDetector - Specifies which detector to be rerun. String.
%           Acceptable values: 'Periodogram', 'SpectralCoherence', 
%           'Thevenin','ModeMeter', 'Ringdown', 'OutOfRangeGeneral','WindRamp'
%
% Outputs:
%   DetectionResultsRerun - Cell array containing the DetectionResults
%       output of the RunDetection function for each time the function was
%       called. Only used in rerun mode; set to {} in normal mode.
%   AdditionalOutputRerun - Cell array containing the AdditionalOutput
%       output of the RunDetection function for each time the function was
%       called. Only used in rerun mode; set to {} in normal mode.

function [DetectionResultsRerun, AdditionalOutputRerun] = BAWS_main9(varargin)

% Collect file paths
%
% Path (possibly) containing the RunFlag, PauseFlag, and PauseData files.
ControlPath = varargin{1};
%
% Event path - this is where results are stored in XML files
EventPath = varargin{2};
if exist(EventPath,'dir') ~= 7; mkdir(EventPath); end
%
% Initialization path - this is where files are stored that allow rerun and
% sparse capabilities
InitializationPath = varargin{3};
if exist(InitializationPath,'dir') ~= 7; mkdir(InitializationPath); end
%
% File directory - this is where the files to be analyzed are stored
% For now, assume that this is a cell array, even if only one directory is
% used. Need to verify this with Heng.
FileDirectory = varargin{4};
if isempty(FileDirectory)
    error('At least one file directory must be specified.');
end
for idx = 1:length(FileDirectory)
    if exist(FileDirectory{idx},'dir') ~= 7
        error(['The following directory for input data does not exist: ' FileDirectory{idx}])
    end
end

% Check if PauseData.mat exists. If it does, this call is being used to
% un-pause a run of the tool.
if exist([ControlPath '\PauseData.mat'],'file') == 0
    % PauseData.mat does not exist - this is an initial run, so no need to
    % unpause
    Unpause = false;
else
    % PauseData.mat does exist - need to unpause a previous run, so skip
    % everything until PauseData.mat is loaded.
    Unpause = true;
end

if (nargin == 5) && (~Unpause)
    RunMode = 'Normal';
    
    ConfigFile = varargin{5};
    if exist(ConfigFile,'file')
        ConfigAll = fun_xmlread_comments(ConfigFile);
    else
        error(['File ' ConfigFile ' does not exist.']);
    end
    
    [DataXML,ProcessXML,PostProcessCustXML,DetectorXML,WindAppXML,...
        BlockDetectors,FileDetectors,NumDQandCustomStages,NumPostProcessCustomStages,...
        NumProcessingStages,DataInfo,FileInfo,...
        ResultUpdateInterval,SecondsToConcat,AlarmingParams,Num_Flags] = InitializeBAWS(ConfigAll,EventPath);
%     ConfigSignalSelection = GetPMU_SignalList(DetectorXML, [FileDetectors BlockDetectors],WindAppXML);
    InitialCondosFilter = [];
    InitialCondosMultiRate = [];
    FinalAngles = [];
    AdditionalOutput = [];
    AdditionalOutputCondos = [];

    EventList = struct();

    PMUconcat = [];
    
    FileLength = [];
    
    SparsePMU = struct();
    
    PMUbyFile = cell(1,length(FileInfo));
elseif ~Unpause
    RunMode = 'Rerun';
    
    % Configuration file tied to results
    ConfigFile = varargin{5};
    % Start and end times specified by the user
    RerunStartTime = varargin{6};
    % Check formatting of start time. Also stores the start time as a
    % datetime variable.
    try
        RerunStartTimeDT = datetime(RerunStartTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
    catch
        error(['The rerun start time ' RerunStartTime ' is not in the following format: MM/dd/yyyy HH:mm:ss']);
    end
    %
    RerunEndTime = varargin{7};
    % Check formatting of end time
    try
        datetime(RerunEndTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
    catch
        error(['The rerun end time ' RerunEndTime ' is not in the following format: MM/dd/yyyy HH:mm:ss']);
    end
    % Detector of interest for the rerun
    RerunDetector = varargin{8};
    
    if exist(ConfigFile,'file')
        ConfigAll = fun_xmlread_comments(ConfigFile);
    else
        error(['Configuration file ' ConfigFile ' does not exist.']);
    end
    
    [~,~,~,DetectorXML,~,BlockDetectors,FileDetectors,~,~,~,~,~,~,SecondsToConcat,~,~] = InitializeBAWS(ConfigAll,EventPath);    
%     ConfigSignalSelection = GetPMU_SignalList(DetectorXML, [FileDetectors BlockDetectors],[]);
    
    % Error check on RerunDetector entry
    if sum(strcmp(RerunDetector,[{'ForcedOscillation'} BlockDetectors FileDetectors])) == 0
        error([RerunDetector ' is not a valid detector name.']);
    end
    
    % If forced oscillation or Thevenin application results are of interest, the window size must
    % be accounted for in the start time. The other detectors operate file-by-file.
    % To do this, an initialization file corresponding to RerunStartTime is
    % loaded and SecondsToConcat is retrieved. RerunStartTime is then
    % adjusted to account for the window size, and the new initialization
    % file is loaded.
    if strcmp(RerunDetector,'ForcedOscillation') || strcmp(RerunDetector,'Thevenin') %|| strcmp(RerunDetector,'ModeMeter')
        % Need to account for the sizes of the windows used in analysis

        % The SparsePMU has timestamps every ResultUpdateInterval
        % seconds. These correspond the the last sample in the window
        % that was evaluated.
        %
        % The first of these timestamps between RerunStartTime and
        % RerunEndTime should be the last sample in the first analysis
        % window that is evaluated.
        
        if strcmp(RerunDetector,'ForcedOscillation')
            % Load the SparsePMU corresponding to the periodogram or
            % spectral coherence detectors. The timestamps will be
            % identical.
            if isfield(DetectorXML,'Periodogram')
                SparseOut = GetSparseData(RerunStartTime,RerunEndTime,InitializationPath,'Periodogram');
            elseif isfield(DetectorXML,'SpectralCoherence')
                SparseOut = GetSparseData(RerunStartTime,RerunEndTime,InitializationPath,'SpectralCoherence');
            else
                warning('Forced oscillation detectors were not running during this time.');
                DetectionResultsRerun = {};
                AdditionalOutputRerun = {};
                return
            end
        else
            strcmp(RerunDetector,'Thevenin')
            % Load the SparsePMU corresponding to the Thevenin application.
            if isfield(DetectorXML,'Thevenin')
                SparseOut = GetSparseData(RerunStartTime,RerunEndTime,InitializationPath,'Thevenin');
            else
                warning('The Thevenin application was not running during this time.');
                DetectionResultsRerun = {};
                AdditionalOutputRerun = {};
                return
            end
%         else
%             % Load the SparsePMU corresponding to the ModeMeter algorithm.
%             if isfield(DetectorXML,'ModeMeter')
%                 SparseOut = GetSparseData(RerunStartTime,RerunEndTime,InitializationPath,'ModeMeter');
%             else
%                 warning('The modemeter application was not running during this time.');
%                 DetectionResultsRerun = {};
%                 AdditionalOutputRerun = {};
%                 return
%             end
%             
        end
        %
        % Find the first timestamp after RerunStartTime
        FirstWindowEndTime = datetime(datestr(SparseOut(1).t(find(SparseOut(1).t >= datenum(RerunStartTimeDT),1)),'yyyy/mm/dd HH:MM:SS.FFF'),'InputFormat','yyyy/MM/dd HH:mm:ss.SSS');
        if isnat(FirstWindowEndTime) || (datenum(FirstWindowEndTime) > datenum(RerunEndTime))
            if strcmp(RerunDetector,'ForcedOscillation')
                warning('The forced oscillation detection algorithms did not execute during the specified time range.');
            else
                strcmp(RerunDetector,'Thevenin')
                warning('The Thevenin application did not execute during the specified time range.');
%             else
%                 warning('The modemeter application did not execute during the specified time range.');
            end
            DetectionResultsRerun = {};
            AdditionalOutputRerun = {};
            return
        end
        FirstWindowStartTime = FirstWindowEndTime - SecondsToConcat/60/60/24;
        % If the start time has fractional seconds, shift to the next
        % whole second to prevent being off by one sample.
        if FirstWindowStartTime.Second ~= round(FirstWindowStartTime.Second)
            FirstWindowStartTime = dateshift(FirstWindowEndTime - SecondsToConcat/60/60/24, 'end', 'second');
        end

        % RerunStartTimeDT will be used to select the proper
        % initialization file. 
        % WindowStartTime will be used to trim the first PMU file that
        % is loaded so that the analyzed windows are identical to those
        % in the original run.
        RerunStartTimeDT = FirstWindowStartTime;
    else
        FirstWindowStartTime = [];
    end
    
    
    % Find the initialization file corresponding to RerunStartTime
    [StarterInitializationFile, RerunStartTimeDTadj] = RetrieveInitializationFile(RerunStartTimeDT,InitializationPath);
    % Load the selected initialization file
    % Provides: ConfigAll, InitialCondosFilter, InitialCondosMultiRate,
    % FinalAngles, AdditionalOutputCondos
    load(StarterInitializationFile);
    %
    RerunStartTime = datestr(RerunStartTimeDTadj,'mm/dd/yyyy HH:MM:SS');

    % Adjust ConfigAll.Config.DataConfig.Configuration.ReaderProperties.Mode 
    % to select archive mode and the start and end times given by
    % RerunStartTime and RerunEndTime
    Mode.Name = 'Archive';
    Mode.Params.DateTimeStart = RerunStartTime;
    Mode.Params.DateTimeEnd = RerunEndTime;
    ConfigAll.Config.DataConfig.Configuration.ReaderProperties.Mode = Mode;

    % Get everything set up based on the configuration structure stored in
    % the initialization file.
    [DataXML,ProcessXML,PostProcessCustXML,DetectorXML,WindAppXML,...
        BlockDetectors,FileDetectors,NumDQandCustomStages,NumPostProcessCustomStages,...
        NumProcessingStages,DataInfo,FileInfo,...
        ResultUpdateInterval,SecondsToConcat,AlarmingParams,Num_Flags] = InitializeBAWS(ConfigAll,EventPath);
    
    % Disable all but the desired detector
    DetectorXML = DisableDetectors(DetectorXML,RerunDetector);
    % If neither the forced oscillation detectors or the Thevenin
    % application or the modemeter applications are of interest, set 
    % SecondsToConcat to empty so that the loop doesn't execute
    if (~strcmp(RerunDetector,'ForcedOscillation')) && (~strcmp(RerunDetector,'Thevenin')) %&& (~strcmp(RerunDetector,'ModeMeter'))
        SecondsToConcat = [];
    end
    
    AdditionalOutput = AdditionalOutputCondos;
    PMUconcat = [];
    
    % Load PMUbyFile from where it was stored when normal mode was run.
    % This way, there is an example of the PMU structures from each
    % directory, even if the first file in rerun mode is missing.
    if exist([InitializationPath '\PMUtemplate.mat'],'file') > 0
        load([InitializationPath '\PMUtemplate.mat']);
    else
        % Normal mode never made it far enough to load a file from each
        % directory, so there is no hope for rerun mode to run.
        warning('Files from each directory were not available during the initial run, so results cannot be returned.');
        DetectionResultsRerun = {};
        AdditionalOutputRerun = {};
        return
    end
    % Adjust the timestamps in each of the PMU structures to just before
    % the time when this rerun is to start. This mimics the use of
    % PMUbyFile in normal mode.
    for FileIdx = 1:length(PMUbyFile)
        % Current timestamps in the template PMU structure (assumes all 
        % PMUs in the file have the same timestamps) 
        tt = PMUbyFile{FileIdx}(1).Signal_Time.Signal_datenum;
        % Shift to the specified start time minus the length of the file
        % tt = datenum(RerunStartTime) + (tt - tt(1)) - ((tt(end)-tt(1)) + (tt(2)-tt(1)));
        tt = datenum(RerunStartTime) + (tt-tt(end)-tt(2)+tt(1));
        % Convert to string
        ttStr = cellstr(datestr(tt,'mm/dd/yy HH:MM:SS.FFF'));
        for pmuIdx = 1:length(PMUbyFile{FileIdx})
            PMUbyFile{FileIdx}(pmuIdx).Signal_Time.Signal_datenum = tt;
            PMUbyFile{FileIdx}(pmuIdx).Signal_Time.Time_String = ttStr;
        end
    end
    
    clear RerunStartTime RerunEndTime RerunStartTimeDT RerunDetector InitializationPathUser IDX StarterInitializationFile Mode
end

FlagBitInput = 1; %Bit used to indicate flagged input data to be processed
FlagBitInterpo = 2; %Bit used to indicate data is interpolated
NumFlagsProcessor = 4; % Number of bits used to indicate processed data that has been flagged for different cases
% The first 2 are needed for processing flags, the second two are used for
% the post-processing customization step that follows.

% These are only used beyond this point in Rerun mode, but they are 
% returned either way.
DetectionResultsRerun = {};
AdditionalOutputRerun = {};

LastShortcutOverEmpty = 0;


%% processing files

done = 0;
while(~min(done))
    if Unpause
        % The tool is being unpaused - load PauseData.mat so that
        % processing can continue        
        try
            load([ControlPath '\PauseData.mat']) 
        catch
            warning(['Attempt to unpause failed because ' ControlPath '\PauseData.mat could not be loaded.']);
            return
        end
        [~,~,~,DetectorXML,~,~,~,~,~,~,~,~,~,~,~] = InitializeBAWS(ConfigAll,EventPath);
        try
            delete([ControlPath '\PauseData.mat'])
        catch
            warning([ControlPath '\PauseData.mat could not be deleted, which may cause logic problems later.']);
        end
        
        % The tool is now unpaused, so reset the Unpause flag so that
        % processing will continue as usual.
        Unpause = false;
    end
    
    
    % Check if the RunFlag file exists
    if (exist([ControlPath '\RunFlag.txt'],'file') == 0) && (~isempty(ControlPath))
        % The RunFlag file does not exist, so processing is to
        % pause/terminate
        
        % Check if the PauseFlag file exists
        if exist([ControlPath '\PauseFlag.txt'],'file') ~= 0
            % The PauseFlag file does exist - save the workspace so that
            % processing can resume later.
            
            % Clear filepaths so that results and data can be moved before
            % the task is unpaused
            ControlPathHold = ControlPath;
            clear ControlPath EventPath InitializationPath FileDirectory
            
            save([ControlPathHold '\PauseData.mat']);
        end
        
        return
    else
        % The RunFlag file does exist, so processing is to continue as
        % usual
        
        % Get the list of files to be loaded and the number of files that were
        % skipped over to get to those files.
        focusFile = cell(1,length(FileInfo));
        done = zeros(1,length(FileInfo));
        SkippedFiles = zeros(1,length(FileInfo));
        FocusFileTime = zeros(1,length(FileInfo));
        for FileIdx = 1:length(FileInfo)
            [focusFile{FileIdx},done(FileIdx),SkippedFiles(FileIdx),FocusFileTime(FileIdx),DataInfo] = getFocusFiles(FileInfo(FileIdx),FileDirectory{FileIdx},DataInfo,FileLength);
            FileInfo(FileIdx).lastFocusFile = focusFile{FileIdx};
        end
        %
        % Find the directories with files corresponding to the earliest
        % FocusFileTime
        idx = FocusFileTime == min(FocusFileTime);
        FocusFileTime = min(FocusFileTime);
        %
        % Remove any directories where a focusFile (even a future one) could
        % not be found.
        idx = idx & ~done;
        %
        % Number of skipped files is identical for all directories with the
        % same FocusFileTime - assumes files from different directories
        % correspond to the same time frames.
        SkippedFiles = SkippedFiles(find(idx,1));
    end
    
    
    % If running in normal mode:
    if strcmp(RunMode,'Normal')
        % Store the sparse PMU structure for the previous file
        % Make sure the last focus file time is available first
        if ~isempty(DataInfo.LastFocusFileTime)
            SparsePath = [InitializationPath '\SparsePMU\' datestr(DataInfo.LastFocusFileTime,'yyyy')];
            if exist(SparsePath,'dir') == 0
                mkdir(SparsePath);
            end
            save([SparsePath '\SparsePMU_' datestr(DataInfo.LastFocusFileTime,'yyyymmdd')],'SparsePMU');
        end
        
        % If the last focus file time is available
        %   AND
        % If the day of the last focus file is different than the day of the
        % current focus file
        %           OR
        % Processing is done (done==1)
        %   THEN
        % Generate a wind report
        % Update the event list and store events that are over
        % Reset the sparse PMU structure
        if ~isempty(DataInfo.LastFocusFileTime) && (~strcmp(datestr(FocusFileTime,'yyyymmdd'),datestr(DataInfo.LastFocusFileTime,'yyyymmdd')) || min(done))
            % If the wind app is configured, generate a report
            if isfield(WindAppXML,'PMU')
                StartTime = datestr(DataInfo.LastFocusFileTime,'yyyy-mm-dd 00:00:00');
                EndTime = datestr(DataInfo.LastFocusFileTime,'yyyy-mm-dd 23:59:00');
                
                % Wind application report path - this is where reports from the wind
                % application are stored
                WindReportPath = WindAppXML.ReportPath;
                if exist(WindReportPath,'dir') ~= 7; mkdir(WindReportPath); end
                
                WindReportPathFull = [WindReportPath '\WindEventReport_' datestr(DataInfo.LastFocusFileTime,'yymmdd')];
                GenerateWindAppReport(ConfigFile,StartTime,EndTime,'NewestLast',WindReportPathFull,DataXML,FileDirectory,NumDQandCustomStages,InitializationPath,ProcessXML,NumProcessingStages,FlagBitInterpo,FlagBitInput,NumFlagsProcessor,PostProcessCustXML,NumPostProcessCustomStages,PMUbyFile,FileLength,Num_Flags,EventPath);
            end

            % Update the event list and store events that are over
            EventList = StoreEventList(EventPath,EventList,PMU,DetectorXML,AdditionalOutput);

            % Reset the sparse PMU structure
            SparsePMU = struct();
        end
        WriteEventListXML(EventList,[EventPath '\EventList_Current.XML'],0);
    end
    %
    % This entry will be used in the next loop, so it will be the previous
    % FocusFileTime.
    DataInfo.LastFocusFileTime = FocusFileTime;
    %
    % If ALL entries in 'done' indicate that processing should stop, break
    % from the while loop
    if(done)
        break
    end
    
    for FileIdx = 1:(SkippedFiles+1)
        [PMU,PMUbyFile,DataInfo,FileInfo,FileLength] = LoadFocusFiles(FileIdx,SkippedFiles,FileInfo,PMUbyFile,idx,DataInfo,focusFile,FileLength,Num_Flags);
        
        if isempty(PMU)
            % Files from each directory have not yet been successfully
            % loaded, so further processing can't be completed yet. Jump
            % to next FileIdx.
            continue
        end
        
        % If enough files are missing, it will save processing time to skip
        % over them rather than treating them like bad data that is processed 
        % normally. 
        % Check if missing files should be skipped over.
        if (FileIdx < 3) || (FileIdx == SkippedFiles+1) || ~strcmp(RunMode,'Normal')
            % Files are either not being skipped over, this is only the 
            % first or second file to be skipped over, this is the first
            % available file after skipping over missing files, or the run
            % mode is not normal mode. Do not take the shortcut.
            ShortcutOverEmpty = 0;
        elseif (FileIdx-1)*FileLength > SecondsToConcat
            % Files are being skipped over and enough files have been
            % skipped to surpass SecondsToConcat. Take the shortcut.
            ShortcutOverEmpty = 1;
        elseif isempty(SecondsToConcat) || isnan(SecondsToConcat)
            % More than 1 file is being skipped over and data is not being
            % concatenated together, only file-by-file processing is taking
            % place. Take the shortcut.
            ShortcutOverEmpty = 1;
        else
            % Files are being skipped over, but not enough files have been
            % skipped to surpass SecondsToConcat. Do not take the shortcut.
            ShortcutOverEmpty = 0;
        end
        % Before taking the shortcut, make additional checks related to the
        % ringdown detector's median filter, which requires a history of
        % RMS calculations.
        if (ShortcutOverEmpty == 1) && isfield(DetectorXML,'Ringdown')
            % If enough files will be skipped to fill the RMS history
            % variable RMShist with NaNs, then fill it with NaNs and
            % continue with the shortcut. If not, do not take the shortcut.
            MaxHist = 0;
            for idx1 = 1:length(AdditionalOutput)
                for idx2 = 1:length(AdditionalOutput(idx1).Ringdown)
                    MaxHist = max([MaxHist length(AdditionalOutput(idx1).Ringdown(idx2).RMShist)/AdditionalOutput(idx1).Ringdown(1).fs]);
                end
            end
            if SkippedFiles*FileLength >= MaxHist
                % Enough files will be skipped. Set all RMShist fields to
                % NaNs and continue with shortcut.
                for idx1 = 1:length(AdditionalOutput)
                    for idx2 = 1:length(AdditionalOutput(idx1).Ringdown)
                        AdditionalOutput(idx1).Ringdown(idx2).RMShist(:) = NaN;
                        AdditionalOutputCondos(idx1).Ringdown(idx2).RMShist(:) = NaN;
                    end
                end
            else
                % Not enough files will be skipped. Do not take shortcut
                ShortcutOverEmpty = 0;
            end
        end
        if ShortcutOverEmpty == 1
            % Take the shortcut through the missing files.
            
            % Except for the timestamp in AdditionalOutputCondos, nothing
            % in the initialization file will have changed. Update
            % AdditionalOutputCondos and then save the initialization file.
            AdditionalOutputCondos(1).TimeStamp = datestr(FileInfo(1).tPMU(end),'yyyy-mm-dd HH:MM:SS.FFF');
            yyyymmdd = datestr(FileInfo(1).tPMU(1),'yyyymmdd');
            hhmmss = datestr(FileInfo(1).tPMU(1),'HHMMSS');
            InitializationFilePath = [InitializationPath '\' yyyymmdd(1:4) '\' yyyymmdd(3:8) '\'];
            InitializationFile = [InitializationFilePath 'Initialization_' yyyymmdd '_' hhmmss '.mat'];
            save(InitializationFile,...
                'AdditionalOutputCondos','InitialCondosFilter','InitialCondosMultiRate','FinalAngles','ConfigAll','FileLength');
            
            % Add NaNs to SparsePMU for event detectors
            SparsePMU = AddMissingToSparsePMU(SparsePMU,datestr(FileInfo(1).tPMU(end),'yyyy-mm-dd HH:MM:SS.FFF'),DetectorXML,FileDetectors);
            
            % If necessary, handle steps that are specific to detectors
            % that operate on an interval, rather than file-by-file
            if ~isempty(SecondsToConcat) && ~isnan(SecondsToConcat)
                % Increment timestamps in PMUconcat by ResultUpdateInterval
                % seconds. Do this in a loop until the final timestamp is 
                % within ResultUpdateInterval seconds of the last sample of
                % loaded data. This mimics the sliding process used in the
                % normal code.
                %
                % Note that I know PMUconcat is at least SecondsToConcat in
                % length and filled with NaNs because ShortcutOverEmpty is 
                % only set to 1 when SecondsToConcat is non-empty if enough
                % files have been skipped to reach SecondsToConcat.
                while 1
                    % I need to do this with PMUsegment and then just add
                    % the total skipped time to PMUconcat (one time, not in
                    % a loop).
                    PMUsegment(1).Signal_Time.Signal_datenum = PMUsegment(1).Signal_Time.Signal_datenum(end);
                    if PMUsegment(1).Signal_Time.Signal_datenum+(ResultUpdateInterval/60/60/24) <= FileInfo(1).tPMU(end)
                        PMUsegment(1).Signal_Time.Signal_datenum = PMUsegment(1).Signal_Time.Signal_datenum + (ResultUpdateInterval/60/60/24);

                        % Add NaNs to SparsePMU for FO detectors - each time they would
                        % have been implemented using ResultUpdateInterval
                        SparsePMU = AddMissingToSparsePMU(SparsePMU,datestr(PMUsegment(1).Signal_Time.Signal_datenum,'yyyy-mm-dd HH:MM:SS.FFF'),DetectorXML,BlockDetectors);
                    else
                        break
                    end
                end
                for PMUidx = 1:length(PMUconcat)
                    PMUconcat(PMUidx).Signal_Time.Signal_datenum = PMUconcat(PMUidx).Signal_Time.Signal_datenum + (FileLength/60/60/24);
                end
            end
            
            % Jump to next FileIdx
            continue
            
            % skip = 0
                % Good file 1 loaded
                % Good initialization from file 0 saved
                % Good file 1 processed
            % skip = 0
                % Good file 2 loaded
                % Good initialization from file 1 saved
                % Good file 2 processed
            % skip = 3
                % FileIdx = 1
                % Bad file 3 loaded
                % Good initialization from file 2 saved
                % Bad file 3 processed
                % FileIdx = 2
                % Bad file 4 loaded
                % Bad initialization from file 3 saved
                % Bad file 4 processed
                % FileIdx = 3
                % Bad file 5 loaded
                    % This is where you can take the shortcut (you need the
                    % bad initialization file).
                % Bad initialization from file 4 saved
                % Bad file 5 processed
                % FileIdx = 4 (= skip+1)
                % Good file 6 loaded
                % Bad initialization from file 5 saved
                % Good file 6 processed
            % skip = 0
                % Good file 7 loaded
                % Good initialization from file 6 saved
                % Good file 7 processed
        end
        % If adjustments to the datenum value in PMUconcat were made for
        % missing files, update the datestr value as well. This only needs
        % to be done at the first file after missing files. 
        if (LastShortcutOverEmpty == 1) && (ShortcutOverEmpty == 0) && ~isempty(SecondsToConcat) && ~isnan(SecondsToConcat)
            for PMUidx = 1:length(PMUconcat)
                PMUconcat(PMUidx).Signal_Time.Time_String = cellstr(datestr(PMUconcat(PMUidx).Signal_Time.Signal_datenum,'yyyy-mm-dd HH:MM:SS.FFF'));
            end
        end
        LastShortcutOverEmpty = ShortcutOverEmpty;
        
        if strcmp(RunMode,'Normal') && ~exist([InitializationPath '\PMUtemplate.mat'],'file')
            % Files from each directory have been loaded, the tool is in
            % normal mode, and PMUtemplate.mat does not yet exist. Save
            % PMUbyFile, which contains examples of each file to a mat file
            % called PMUtemplate.mat. It is used in rerun mode if a file in
            % a directory is missing. First, set all of its entries to NaN.
            for DirIdx = 1:length(PMUbyFile)
                for pmuIdx = 1:length(PMUbyFile{DirIdx})
                    PMUbyFile{DirIdx}(pmuIdx).Flag(:) = 1;
                    PMUbyFile{DirIdx}(pmuIdx).Data(:) = NaN;
                    PMUbyFile{DirIdx}(pmuIdx).Stat(:) = NaN;
                end
            end
            save([InitializationPath '\PMUtemplate.mat'], 'PMUbyFile');
        elseif strcmp(RunMode,'Rerun')
            FileProgress = round((FileInfo(1).tPMU(end)-datenum(DataInfo.DateTimeStart))/(datenum(DataInfo.DateTimeEnd)-datenum(DataInfo.DateTimeStart))*100);
            if FileProgress > 100
                FileProgress = 100;
            end
            disp(['Progress through files: ' num2str(FileProgress) '%']);
        end
        
        % Apply data quality filters and signal customizations
        PMU = DQandCustomization(PMU,DataXML,NumDQandCustomStages,Num_Flags);
        % Return only the desired PMUs and signals
        PMU = GetOutputSignals(PMU,DataXML);
        
        disp(datestr(FileInfo(1).tPMU(1)))
        
        % If running in normal mode:
        if strcmp(RunMode,'Normal')
            % Save initialization information - filter conditions, etc.
            % that can be used to pick up the analysis at this minute.
            yyyymmdd = datestr(FileInfo(1).tPMU(1),'yyyymmdd');
            hhmmss = datestr(FileInfo(1).tPMU(1),'HHMMSS');
            InitializationFilePath = [InitializationPath '\' yyyymmdd(1:4) '\' yyyymmdd(3:8) '\'];
            InitializationFile = [InitializationFilePath 'Initialization_' yyyymmdd '_' hhmmss '.mat'];
            % If the directory for the files hasn't been established
            % yet, add it.
            if exist(InitializationFilePath,'dir') == 0
                mkdir(InitializationFilePath);
            end
            save(InitializationFile,...
                'AdditionalOutputCondos','InitialCondosFilter','InitialCondosMultiRate','FinalAngles','ConfigAll','FileLength');
        end


        % **********
        % Processing
        % **********
        [PMU, InitialCondosFilter, InitialCondosMultiRate, FinalAngles] = DataProcessor(PMU, ProcessXML, NumProcessingStages, FlagBitInterpo,FlagBitInput,NumFlagsProcessor,InitialCondosFilter,InitialCondosMultiRate,FinalAngles);
        % Return only the desired PMUs and signals
        PMU = GetOutputSignals(PMU, DataXML);
        
        % *****************************
        % Post-Processing Customization
        % *****************************
        % Num_Flags is hard coded as 4 to account for the 2 processor flags
        % and the 2 customization flags.
        PMU = DQandCustomization(PMU,PostProcessCustXML,NumPostProcessCustomStages,4);
%         PMU = GetOutputSignalsRev(PMU,ConfigSignalSelection);
        
        % *********
        % Detection
        % *********
        % Perform event detection on data from most recently loaded
        % file
        [DetectionResults, AdditionalOutput] = RunDetection(PMU,DetectorXML,FileDetectors,AdditionalOutput);
        
        
        
        if strcmp(RunMode,'Normal')
            % Running in normal mode
            
            EventList = UpdateEvents(DetectionResults,AdditionalOutput,DetectorXML,AlarmingParams,FileDetectors,EventList);

            EventList = WindApplication(PMU,WindAppXML,EventList,DetectorXML,FileLength);
            
            SparsePMU = AddToSparsePMU(SparsePMU,AdditionalOutput,DetectorXML,FileDetectors);
        elseif ~isempty(FirstWindowStartTime)
            % Running in rerun mode with forced oscillation detectors
            
            % Trim the PMU structure so that it starts at WindowStartTime.
            % This will ensure that the windows of data evaluated in rerun
            % mode match those from the original run
            for TrimIdx = 1:length(PMU)
                % Subtraction institutes 1 millisecond precision. Without
                % it, negligible differences in time cause problems.
                KeepIdx = PMU(TrimIdx).Signal_Time.Signal_datenum >= datenum(FirstWindowStartTime)-0.001/60/60/24;
                PMU(TrimIdx).Stat = PMU(TrimIdx).Stat(KeepIdx);
                PMU(TrimIdx).Data = PMU(TrimIdx).Data(KeepIdx,:);
                PMU(TrimIdx).Flag = PMU(TrimIdx).Flag(KeepIdx,:,:);
                PMU(TrimIdx).Signal_Time.Signal_datenum = PMU(TrimIdx).Signal_Time.Signal_datenum(KeepIdx);
                PMU(TrimIdx).Signal_Time.Time_String = PMU(TrimIdx).Signal_Time.Time_String(KeepIdx);
            end
            
            % Only need to do this once, so set WindowStartTime to empty so
            % that this elseif no longer evaluates to true.
            FirstWindowStartTime = [];
        end

        % A window of length SecondsToConcat slides
        % ResultUpdateInterval seconds at each step.

        if ~isempty(SecondsToConcat) && ~isnan(SecondsToConcat)
            % PMUconcat holds enough data in memory to apply the FO
            % detection algorithms (SecondsToConcat).
            % Add the current PMU to PMUconcat.
            PMUconcat = ConcatenatePMU(PMUconcat,PMU);
            % Length of PMUconcat in seconds
            PMUconcatLength = round((PMUconcat(1).Signal_Time.Signal_datenum(end)-PMUconcat(1).Signal_Time.Signal_datenum(1)+PMUconcat(1).Signal_Time.Signal_datenum(2)-PMUconcat(1).Signal_Time.Signal_datenum(1))*24*60*60);
            % If PMUconcat has enough data (SecondsToConcat) to apply the
            % FO detection algorithms, continue. If not, skip detection and
            % continue with the loop.
            if PMUconcatLength >= SecondsToConcat
                % Number of times the analysis window can slide through the
                % data stored in PMUconcat
                NumSlide = floor((PMUconcatLength-SecondsToConcat)/ResultUpdateInterval)+1;
                % Slide through the data in PMUconcat. A window of length
                % SecondsToConcat is updated every ResultUpdateInterval
                % seconds.
                for SlideIdx = 1:NumSlide
                    % PMUsegment stores the current analysis window. It is
                    % of length SecondsToConcat.
                    PMUsegment = CreatePMUsegment(PMUconcat,SecondsToConcat,PMUconcatLength,ResultUpdateInterval,SlideIdx);

                    % Apply detection algorithms.
                    [DetectionResults, AdditionalOutput] = RunDetection(PMUsegment,DetectorXML,BlockDetectors,AdditionalOutput);
                    % If running in normal mode:
                    if strcmp(RunMode,'Normal')
                        EventList = UpdateEvents(DetectionResults,AdditionalOutput,DetectorXML,AlarmingParams,BlockDetectors,EventList);
                        
                        SparsePMU = AddToSparsePMU(SparsePMU,AdditionalOutput,DetectorXML,BlockDetectors);
                    else
                        % Rerun mode 
                        
                        % If the timestamp in AdditionalOutput is beyond
                        % the specified end time don't add it to the Rerun
                        % cell arrays. Return them as-is. This happens if
                        % ResultUpdateInterval is such that multiple
                        % evaluations fall within a single loaded file of
                        % data.
                        if datenum(AdditionalOutput(1).TimeStamp) > datenum(ConfigAll.Config.DataConfig.Configuration.ReaderProperties.Mode.Params.DateTimeEnd)
                            return
                        end
                        
                        % Store detection results from block
                        % detectors.
                        %
                        % Note that block detectors OR file
                        % detectors are applied in Rerun mode, not both.
                        %
                        % After the first one, only store
                        % ResultUpdateInterval seconds of Data
                        DetectionResultsRerun{length(DetectionResultsRerun) + 1} = DetectionResults;
                        if isempty(AdditionalOutputRerun)
                            AdditionalOutputRerun{length(AdditionalOutputRerun) + 1} = AdditionalOutput;
                        else
                            if isfield(AdditionalOutput,'Periodogram')
                                for DetIdx = 1:length(AdditionalOutput)
                                    if ~isempty(AdditionalOutput(DetIdx).Periodogram)
                                        KeepSamp = ResultUpdateInterval*AdditionalOutput(DetIdx).Periodogram(1).fs;
                                        AdditionalOutput(DetIdx).Periodogram(1).Data = AdditionalOutput(DetIdx).Periodogram(1).Data(end-KeepSamp+1:end,:);
                                        AdditionalOutput(DetIdx).Periodogram(1).TimeString = AdditionalOutput(DetIdx).Periodogram(1).TimeString(end-KeepSamp+1:end);
                                    end
                                end
                            end
                            if isfield(AdditionalOutput,'SpectralCoherence')
                                for DetIdx = 1:length(AdditionalOutput)
                                    if ~isempty(AdditionalOutput(DetIdx).SpectralCoherence)
                                        KeepSamp = ResultUpdateInterval*AdditionalOutput(DetIdx).SpectralCoherence(1).fs;
                                        AdditionalOutput(DetIdx).SpectralCoherence(1).Data = AdditionalOutput(DetIdx).SpectralCoherence(1).Data(end-KeepSamp+1:end,:);
                                        AdditionalOutput(DetIdx).SpectralCoherence(1).TimeString = AdditionalOutput(DetIdx).SpectralCoherence(1).TimeString(end-KeepSamp+1:end);
                                    end
                                end
                            end
                            % This is done inside the TheveninDetector code
%                             if isfield(AdditionalOutput,'Thevenin')
%                                 for DetIdx = 1:length(AdditionalOutput)
%                                     if ~isempty(AdditionalOutput(DetIdx).Thevenin)
%                                         KeepSamp = ResultUpdateInterval*AdditionalOutput(DetIdx).Thevenin(1).fs;
%                                         AdditionalOutput(DetIdx).Thevenin(1).Data = AdditionalOutput(DetIdx).Thevenin(1).Data(end-KeepSamp+1:end,:);
%                                         
%                                         for SubIdx = 1:length(AdditionalOutput(DetIdx).Thevenin)
%                                             AdditionalOutput(DetIdx).Thevenin(SubIdx).Vmeas = AdditionalOutput(DetIdx).Thevenin(SubIdx).Vmeas(end-KeepSamp+1:end,:);
%                                             AdditionalOutput(DetIdx).Thevenin(SubIdx).TimeString = AdditionalOutput(DetIdx).Thevenin(SubIdx).TimeString(end-KeepSamp+1:end);
%                                             AdditionalOutput(DetIdx).Thevenin(SubIdx).ShuntQ = AdditionalOutput(DetIdx).Thevenin(SubIdx).ShuntQ(end-KeepSamp+1:end,:);
%                                             AdditionalOutput(DetIdx).Thevenin(SubIdx).SinkQ = AdditionalOutput(DetIdx).Thevenin(SubIdx).SinkQ(end-KeepSamp+1:end,:);
%                                         end
%                                     end
%                                 end
%                             end
                            AdditionalOutputRerun{length(AdditionalOutputRerun) + 1} = AdditionalOutput;
                        end
                    end
                end

                PMUconcat = UpdatePMUconcat(PMUconcat,ResultUpdateInterval,PMUconcatLength,NumSlide);
            end
        else
            % If in Rerun mode, store detection results from file-by-file
            % detectors and SparesePMU points (max and min)
            %
            % Note that forced oscillation detectors OR event
            % detectors are applied in Rerun mode, not both.
            if strcmp(RunMode,'Rerun')
                DetectionResultsRerun{length(DetectionResultsRerun) + 1} = DetectionResults;
                AdditionalOutputRerun{length(AdditionalOutputRerun) + 1} = AdditionalOutput;
            end
        end

        %% Clean AdditionalOutput to contain only initialization info
        AdditionalOutputCondos = CleanAdditionalOutput(AdditionalOutput);
        
        %% If running in rerun mode save DetectionResults and AdditionalOutput 
        % Note that in rerun mode RunDetection is only called once because
        % only the detectors of interest are run. Thus, it's okay that
        % DetectionResults is normally overwritten when the FO detectors
        % are applied.
        % If running in rerun mode:
    end
end