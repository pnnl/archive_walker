% function SparseOut = GetSparseData(SparseStartTime,SparseEndTime,SparseDetector,InitializationPath)
%
% This function returns the sparse data for a specified time frame so that
% it can be displayed in the GUI.
%
% Inputs:
% SparseStartTime = start time for the data in the format MM/DD/YYYY HH:MM:SS
% SparseEndTime = end time for the data in the format MM/DD/YYYY HH:MM:SS
% InitializationPath = path to the folder containing initialization files
% SparseDetector = detector of interest. Acceptable values: 'Periodogram', 'SpectralCoherence', 'Ringdown', 'OutOfRangeGeneral','WindRamp'
%
% Example inputs:
% SparseStartTime = '08/21/2016 23:57:12';
% SparseEndTime = '08/22/2016 00:01:56';
% ConfigFile = 'C:\Users\foll154\Documents\BPAoscillationApp\CodeForProject2\DataReaderCode\ConfigXML\RerunTest.xml'
% SparseDetector = 'SpectralCoherence';

function SparseOut = GetSparseData(SparseStartTime,SparseEndTime,InitializationPath,SparseDetector)

% Convert string times to datetime
SparseStartTime = datetime(SparseStartTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
SparseEndTime = datetime(SparseEndTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
% Shift to the end of the minute so that the specified time is captured
SparseEndTimeShift = dateshift(SparseEndTime,'end','minute');
% If the shift to the end of the minute moved it to another day, step back
% by one second so that an extra SparsePMU doesn't have to be loaded, which
% may not exist. 
if day(SparseEndTimeShift) ~= day(SparseEndTime)
    SparseEndTime = SparseEndTimeShift - seconds(1);
else
    SparseEndTime = SparseEndTimeShift;
end

% Get dates associated with the start and end times
SparseStartDay = dateshift(SparseStartTime,'start','day');
SparseEndDay = dateshift(SparseEndTime,'start','day');

% Initialize the structure
SparseOut = struct('DataMin',[],'DataMax',[],'DataPMU',[],'DataChannel',[],'DataType',[],'DataUnit',[],'t',{});

% For each day in the time range
for ThisDay = SparseStartDay:SparseEndDay
    % Load the SparsePMU structure for the day
    try
        load([InitializationPath '\SparsePMU\' datestr(ThisDay,'yyyy') '\SparsePMU_' datestr(ThisDay,'yyyymmdd') '.mat']);
    catch
        if exist([InitializationPath '\SparsePMU\' datestr(ThisDay,'yyyy') '\SparsePMU_' datestr(ThisDay,'yyyymmdd') '.mat'],'file') > 0
            % File exists but couldn't be loaded
            warning(['A required file exists but could not be loaded: ' InitializationPath '\SparsePMU\' datestr(ThisDay,'yyyy') '\SparsePMU_' datestr(ThisDay,'yyyymmdd') '.mat']);
        else
            % File does not exist
            warning(['A required file for the specified time range does not exist: ' InitializationPath '\SparsePMU\' datestr(ThisDay,'yyyy') '\SparsePMU_' datestr(ThisDay,'yyyymmdd') '.mat']);
        end
        continue
    end
    % For each detector (max across all detectors)
    for DetIdx = 1:length(SparsePMU)
        % length(SparsePMU) = the max number of detectors for any time of
        % detector. Thus, ensure that DetIdx applies for this detector.
        if isfield(SparsePMU,SparseDetector)
            if ~isempty(SparsePMU(DetIdx).(SparseDetector))
                if DetIdx > length(SparseOut)
                    % This index of SparseOut isn't available yet, initialize
                    % it
                    SparseOut(DetIdx).DataMin = SparsePMU(DetIdx).(SparseDetector).DataMin;
                    SparseOut(DetIdx).DataMax = SparsePMU(DetIdx).(SparseDetector).DataMax;
                    SparseOut(DetIdx).t = SparsePMU(DetIdx).(SparseDetector).TimeStamp;
                    SparseOut(DetIdx).DataPMU = SparsePMU(DetIdx).(SparseDetector).DataPMU;
                    SparseOut(DetIdx).DataChannel = SparsePMU(DetIdx).(SparseDetector).DataChannel;
                    SparseOut(DetIdx).DataType = SparsePMU(DetIdx).(SparseDetector).DataType;
                    SparseOut(DetIdx).DataUnit = SparsePMU(DetIdx).(SparseDetector).DataUnit;
                else
                    % This index of SparseOut is available, add to it
                    SparseOut(DetIdx).DataMin = [SparseOut(DetIdx).DataMin; SparsePMU(DetIdx).(SparseDetector).DataMin];
                    SparseOut(DetIdx).DataMax = [SparseOut(DetIdx).DataMax; SparsePMU(DetIdx).(SparseDetector).DataMax];
                    SparseOut(DetIdx).t = [SparseOut(DetIdx).t; SparsePMU(DetIdx).(SparseDetector).TimeStamp];
                end
            end
        end
    end
end

% Trim to the time range specified by the inputs
% Convert to datenum
for DetIdx = 1:length(SparseOut)
    SparseOut(DetIdx).t = datetime(SparseOut(DetIdx).t,'InputFormat','yyyy-MM-dd HH:mm:ss.SSS');
    KeepIdx = (SparseStartTime<=SparseOut(DetIdx).t) & (SparseOut(DetIdx).t<=SparseEndTime);
    SparseOut(DetIdx).DataMin = SparseOut(DetIdx).DataMin(KeepIdx,:);
    SparseOut(DetIdx).DataMax = SparseOut(DetIdx).DataMax(KeepIdx,:);
    SparseOut(DetIdx).t = datenum(SparseOut(DetIdx).t(KeepIdx));
end