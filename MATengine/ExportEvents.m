function ExportEvents(Config,EventPath,TimeStamp,done)

% Only proceed if event export is desired
if Config.Flag == 0
    return;
end

TimeStamp = datetime(TimeStamp,'ConvertFrom','datenum');
EndTime = dateshift(TimeStamp,'end','day');


% Trim off Event folder to get the path to this task
TaskPath = EventPath(1:end-6);

ConfigFile = fullfile(TaskPath,'Config.xml');
if exist(ConfigFile,'file') == 0
    msgbox(['Configuration file could not be found at ' TaskPath],'Error');
    return;
end

ConfigAll = fun_xmlread_comments(ConfigFile);
DetectorXML = ConfigAll.Config.DetectorConfig.Configuration;
if isfield(DetectorXML,'DataWriter')
    DW = DetectorXML.DataWriter;
    if length(DW) == 1
        DW = {DW};
    end

    % Extract the parameters for each data writer
    DWparams = cell(1,length(DW));
    for DWidx = 1:length(DW)
        DWparams{DWidx} = ExtractParameters(DW{DWidx});
    end
else
    msgbox('This task did not include a data writer.','Error');
    return;
end

if exist(EventPath,'dir') == 0
    msgbox(['Event folder could not be found at ' EventPath],'Error');
    return;
end

OutputPath = Config.ExportPath;
if exist(OutputPath,'dir') == 0
    msgbox('Specified output path does not exist','Error');
    return;
end
% Create Ringdown and OutOfRange folders in the output directory if they don't exist
DetNames = {'Ringdown','OutOfRange'};
for DetIdx = 1:length(DetNames)
    if exist(fullfile(OutputPath,DetNames{DetIdx}),'dir') == 0
        try
            mkdir(fullfile(OutputPath,DetNames{DetIdx}))
        catch
            msgbox('Unable to create subfolders for output data.','Error');
        end
    end
end


%**************************

ELring = LoadEventLists(TimeStamp,EventPath,'Ringdown',done);
ELoor = LoadEventLists(TimeStamp,EventPath,'OutOfRangeGeneral',done);

EL = {ELring,ELoor};

% The DataWriterDetector function is used to export the data. 
% The SeparatePMUs parameter should always be FALSE when used in this way.
% The NoTimeSubfolders parameter is only used when DataWriterDetector is
% called from this function. The extra parameter tells DataWriterDetector
% to omit yyyy and yymmdd subfolders when storing data.
DataWriterParams.SeparatePMUs = 'FALSE';
DataWriterParams.NoTimeSubfolders = 1;

% For each detector go through events
for DetIdx = 1:length(DetNames)
    if isempty(EL{DetIdx})
        % No events detected by this detector, skip to next
        continue;
    end

    % For each event
    for Eidx = 1:length(EL{DetIdx})
        Eid = strrep(EL{DetIdx}(Eidx).ID,'.','_');
        Estart = datetime(EL{DetIdx}(Eidx).Start,'ConvertFrom','datenum');
        Eend = datetime(EL{DetIdx}(Eidx).End,'ConvertFrom','datenum');

        % Add minutes before and after the event based on the user input
        Estart = Estart - Config.SurroundingMinutes/1440;
        Eend = Eend + Config.SurroundingMinutes/1440;
        
        % Store start and end times in the parameter structure for the
        % DataWriterDetector function
        DataWriterParams.Estart = Estart;
        DataWriterParams.Eend = Eend;

        % For each data writer
        for DWidx = 1:length(DWparams)
            if DWparams{DWidx}.SeparatePMUs
                DWstorage = dir(DWparams{DWidx}.SavePath);
                DWstorage = {DWstorage.name};

                % Remove the non-PMU directories
                DWstorage(strcmp(DWstorage,'.')) = [];
                DWstorage(strcmp(DWstorage,'..')) = [];
            else
                % All PMUs placed together in the main folder
                DWstorage = {[]};
            end

            % For each folder containing data (could be just main folder or several PMU folders)
            for StorageIdx = 1:length(DWstorage)
                DWstorageFull = fullfile(DWparams{DWidx}.SavePath,DWstorage{StorageIdx});

                % Get a list of the full filepaths for every day between Estart and Eend
                FilePaths = {};
                FileNames = {};
                FileTimes = [];
                for t = dateshift(Estart,'start','day'):min([EndTime-1/86400000 dateshift(Eend,'start','day')])
                    yyyy = datestr(t,'yyyy');
                    yymmdd = datestr(t,'yymmdd');
                    FileListDay = dir(fullfile(DWstorageFull,yyyy,yymmdd));
                    FileListDay = FileListDay([FileListDay.isdir] == 0);

                    FileListDayName = {FileListDay.name};
                    FileListDayFolder = {FileListDay.folder};

                    FileNames = [FileNames FileListDayName];

                    FileListDayPaths = strcat(FileListDayFolder,'\',FileListDayName);
                    FilePaths = [FilePaths FileListDayPaths];

                    FileListDayTimes = FileListDayName;
                    StrStart = FileListDayTimes{1}(1:end-19);
                    FileListDayTimes = extractBetween(FileListDayTimes,StrStart,'.csv');
                    FileListDayTimes = datetime(FileListDayTimes,'InputFormat','yyyyMMdd_HHmmSS');
                    FileTimes = [FileTimes FileListDayTimes];
                end

                KeepIdx = find(FileTimes<Estart,1,'last'):find(FileTimes<Eend,1,'last');
                FilePaths = FilePaths(KeepIdx);
                FileNames = FileNames(KeepIdx);
                FileTimes = FileTimes(KeepIdx);

                NewPath = fullfile(OutputPath,DetNames{DetIdx},datestr(FileTimes(1),'yyyy'),datestr(FileTimes(1),'yymmdd'),Eid,['DataWriter_' num2str(DWidx)]);
                if exist(NewPath,'dir') == 0
                    mkdir(NewPath);
                end
                
                % Concatenate together data from all files
                PMUconcat = [];
                for FileIdx = 1:length(FilePaths)
                    PMU = JSIS_CSV_2_Mat(FilePaths{FileIdx},0);
                    PMUconcat = ConcatenatePMU(PMUconcat,PMU);
                end
                % Set up parameters needed by DataWriterDetector function
                DataWriterParams.SavePath = NewPath;
                DataWriterParams.Mnemonic = FileNames{1}(1:end-20);
                % Export the data by repurposing the DataWriterDetector
                % function.
                DataWriterDetector(PMUconcat,DataWriterParams);
            end
        end
    end
end

% Delete old files written by data writer
%
% Check flag indicating if old files should be deleted
if Config.DeletePastFlag == 1
    % For each data writer
    for DWidx = 1:length(DWparams)
        if DWparams{DWidx}.SeparatePMUs
            DWstorage = dir(DWparams{DWidx}.SavePath);
            DWstorage = {DWstorage.name};

            % Remove the non-PMU directories
            DWstorage(strcmp(DWstorage,'.')) = [];
            DWstorage(strcmp(DWstorage,'..')) = [];
        else
            % All PMUs placed together in the main folder
            DWstorage = {[]};
        end

        % For each folder containing data (could be just main folder or several PMU folders)
        for StorageIdx = 1:length(DWstorage)
            DWstorageFull = fullfile(DWparams{DWidx}.SavePath,DWstorage{StorageIdx});

            % Delete all files from this day and before
            % Minimum allowed value of DeletePastDays is 2
            t = EndTime - Config.DeletePastDays;


            % Get a list of day folders within the year folder
            yyyy = datestr(t,'yyyy');
            FileListDay = dir(fullfile(DWstorageFull,yyyy));
            FileListDay = {FileListDay.name};
            % Remove the non-PMU directories
            FileListDay(strcmp(FileListDay,'.')) = [];
            FileListDay(strcmp(FileListDay,'..')) = [];
            % Convert to number
            FileListDayNum = str2double(FileListDay);
            t = str2double(datestr(t,'yymmdd'));

            KillIdx = find(FileListDayNum <= t);
            for k = KillIdx
                rmdir(fullfile(DWstorageFull,yyyy,FileListDay{k}),'s');
            end
        end
    end
end

end

function ExtractedParameters = ExtractParameters(Parameters)

% Path to save output files
if isfield(Parameters,'SavePath')
    SavePath = Parameters.SavePath;
    if exist(SavePath,'dir') == 0
        error(['Save path ' SavePath ' does not exist.']);
    end
else
    error(['Save path ' SavePath ' does not exist.']);
end

% Flag indicating whether each PMU should be stored in its own folder
if isfield(Parameters,'SeparatePMUs')
    if strcmp('TRUE',Parameters.SeparatePMUs)
        SeparatePMUs = true;
    else
        SeparatePMUs = false;
    end
else
    SeparatePMUs = false;
end

if SeparatePMUs
    % PMUs are to be placed in separate folders. The PMU name is used as
    % the mnemonic for file naming
    Mnemonic = '';
else
    % PMUs are all going in one folder. If the user specified a mnemonic,
    % use it. Otherwise, use the generic 'ExportedData'
    if isfield(Parameters,'Mnemonic')
        Mnemonic = Parameters.Mnemonic;
    else
        Mnemonic = 'ExportedData';
    end
end

ExtractedParameters = struct('SavePath',SavePath,...
    'SeparatePMUs',SeparatePMUs,'Mnemonic',Mnemonic);
end


function EL = LoadEventLists(TimeStamp,EventPath,Detector,done)
if (exist(fullfile(EventPath,'EventList_Current.XML'),'file')==2) && min(done)
    EL = EventListXML2MAT(fun_xmlread_comments(fullfile(EventPath,'EventList_Current.XML')));
    % Set to empty if the file was completely empty or if there were no Out-of-Range events
    if isstruct(EL)
        if isfield(EL,Detector)
            EL = EL.(Detector);
        else
            EL = [];
        end
    else
        EL = [];
    end
else
    EL = [];
end

% Load the event list and add it
if exist(fullfile(EventPath,['EventList_' datestr(TimeStamp,'yymmdd') '.XML']),'file')
    ELday = EventListXML2MAT(fun_xmlread_comments(fullfile(EventPath,['EventList_' datestr(TimeStamp,'yymmdd') '.XML'])));
    % Set to empty if the file was completely empty or if there were no Out-of-Range events
    if isstruct(ELday)
        if isfield(ELday,Detector)
            ELday = ELday.(Detector);
        else
            ELday = [];
        end
    else
        ELday = [];
    end
else
    ELday = [];
end

EL = [EL ELday];
end