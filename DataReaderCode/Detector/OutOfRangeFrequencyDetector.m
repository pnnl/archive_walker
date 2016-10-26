%           PastAdditionalOutput: past AdditionalOutput outputs from this
%                                 function that can be used for various
%                                 purposes.

function [DetectionResults, AdditionalOutput] = OutOfRangeFrequencyDetector(PMUstruct,Parameters,PastAdditionalOutput)

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the periodogram detector could not be used.');
    DetectionResults = struct([]);
    AdditionalOutput = struct([]);
    return
end

% Using the outputs from ExtractData(), make sure that NaN values are
% handled appropriately and that signal types and units are appropriate.
% This comment is general for all detectors, write code specific to the
% detector you're working on.
KeepIdx = strcmp(DataType,'F') | strcmp(DataType,'OTHER');
Data = Data(:,KeepIdx);
DataPMU = DataPMU(KeepIdx);
DataChannel = DataChannel(KeepIdx);

%% Get detector parameters

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. 
% Additional inputs, such as the length of the input data or the sampling 
% rate, can be added as necessary. 
ExtractedParameters = ExtractParameters(Parameters,fs);

% Store the parameters in variables for easier access
DurationMax = ExtractedParameters.DurationMax;
DurationMin = ExtractedParameters.DurationMin;
Duration = ExtractedParameters.Duration;
AnalysisWindow = ExtractedParameters.AnalysisWindow;
%
RateOfChangeMax = ExtractedParameters.RateOfChangeMax;
RateOfChangeMin = ExtractedParameters.RateOfChangeMin;
RateOfChange = ExtractedParameters.RateOfChange;


%% Error checks
if Duration > AnalysisWindow
    warning('Duration cannot be greater than AnalysisWindow. Default of 0 will be used.');
    Duration = 0;
end

if DurationMax < DurationMin
    warning('DurationMax is less than DurationMin. Duration-based frequency event detector will not be implemented.')
    DurationMax = NaN;
    DurationMin = NaN;
    Duration = NaN;
    AnalysisWindow = NaN;
end

if RateOfChangeMax < RateOfChangeMin
    warning('RateOfChangeMax is less than RateOfChangeMin. Rate-of-change-based frequency event detector will not be implemented.')
    RateOfChangeMax = NaN;
    RateOfChangeMin = NaN;
    RateOfChange = NaN;
end


%% Perform detection

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'OutStart',[],'OutEnd',[]);

% Initialize structure for additional outputs
AdditionalOutput = struct('FilterConditions',[]);


if ~isnan(AnalysisWindow)
    % Logical matrix to mark locations where Data goes outside of the detection
    % thresholds for the duration-based detector. The matrix remains false 
    % where Data is NaN so that these values are ignored, as specified.
    OutOfBoundsDuration = false(size(Data));
    % Set the matrix to true where Data goes above its upper detection
    % threshold. DurationMax is only NaN when it is not to be included.
    if ~isnan(DurationMax)
        OutOfBoundsDuration(Data > DurationMax) = true;
    end
    % Set the matrix to true where Data goes below its lower detection
    % threshold. DurationMin is only NaN when it is not to be included.
    if ~isnan(DurationMin)
        OutOfBoundsDuration(Data < DurationMin) = true;
    end

    % Loop throught each signal
    for index = 1:size(Data,2)
        % Store the PMU and channel name
        DetectionResults(index).PMU = DataPMU(index);
        DetectionResults(index).Channel = DataChannel(index);

        if isempty(PastAdditionalOutput)
            % PastAdditionalOutput isn't available
            InitConditions = [];
        else
            InitConditions = PastAdditionalOutput(index).FilterConditions;
        end

        [OutsideCount, AdditionalOutput(index).FilterConditions] = filter(ones(1,AnalysisWindow),1,OutOfBoundsDuration(:,index),InitConditions);

        DetectionIdx = OutsideCount > Duration;

        OutStart = {};
        OutEnd = {};
        if sum(DetectionIdx) > 0
            % Out-of-range data was detected

            % Start and end of out-of-range data (window length is accounted
            % for later)
            Starts = find(diff([0; DetectionIdx]) == 1);
            Ends = find(diff(DetectionIdx) == -1);

            if length(Starts) > length(Ends)
                Ends = [Ends; length(DetectionIdx)];
            end

            for OutIdx = 1:length(Starts)
                OutStart{OutIdx} = datestr(datenum(TimeString{Starts(OutIdx)})-(AnalysisWindow/fs)/(60*60*24),'yyyy-mm-dd HH:MM:SS.FFF');
                OutEnd{OutIdx} = TimeString{Ends(OutIdx)};
            end
        end

        DetectionResults(index).DurationOutStart = OutStart;
        DetectionResults(index).DurationOutEnd = OutEnd;
    end
else
    % Detector cannot be implemented
    
    % Loop throught each signal
    for index = 1:size(Data,2)
        % Store the PMU and channel name
        DetectionResults(index).PMU = DataPMU(index);
        DetectionResults(index).Channel = DataChannel(index);
        DetectionResults(index).DurationOutStart = NaN;
        DetectionResults(index).DurationOutEnd = NaN;
    end
end



if ~isnan(RateOfChange)
    % Logical matrix to mark locations where Data goes outside of the detection
    % thresholds for the rate-of-change-based detector. The matrix remains false 
    % where Data is NaN so that these values are ignored, as specified.
    OutOfBoundsRateOfChange = false(size(Data));
    % Set the matrix to true where Data goes above its upper detection
    % threshold. RateOfChangeMax is only NaN when it is not to be included.
    if ~isnan(DurationMax)
        OutOfBoundsRateOfChange(Data > RateOfChangeMax) = true;
    end
    % Set the matrix to true where Data goes below its lower detection
    % threshold. RateOfChangeMin is only NaN when it is not to be included.
    if ~isnan(DurationMin)
        OutOfBoundsRateOfChange(Data < RateOfChangeMin) = true;
    end

    % Find the rate of change - loop through each signal
    for index = 1:size(Data,2)
        [PeakVals, PeakLocs] = findpeaks(Data(:,index));
        [ValleyVals, ValleyLocs] = findpeaks(-Data(:,index));
        ValleyVals = -ValleyVals;

        % Create a list of alternating maxima and minima
        ExtremeVals = [PeakVals; ValleyVals];
        ExtremeLocs = [PeakLocs; ValleyLocs];
        [ExtremeLocs, SortIdx] = sort(ExtremeLocs);
        ExtremeVals = ExtremeVals(SortIdx);

        % Add start and end points
        ExtremeLocs = [1; ExtremeLocs; size(Data,1)];
        ExtremeVals = [Data(1,index); ExtremeVals; Data(end,index)];

        SampChange = diff(ExtremeLocs);
        FreqChange = abs(diff(ExtremeVals));

        Rate = FreqChange./SampChange;
        OverRate = find(Rate > RateOfChange);

        ExtremeIdx = false(size(Data,1),1);
        for OverRateIdx = OverRate.'
            ExtremeIdx(ExtremeLocs(OverRateIdx):ExtremeLocs(OverRateIdx+1)) = true;
        end
    %     DetectionIdx = OutOfBoundsRateOfChange(:,index) & ExtremeIdx;

        DetectionIdx = OutOfBoundsRateOfChange(:,index);

        OutStart = {};
        OutEnd = {};
        if sum(DetectionIdx) > 0
            % Out-of-range data was detected

            % Start and end of out-of-range data
            Starts = find(diff([0; DetectionIdx]) == 1);
            Ends = find(diff(DetectionIdx) == -1);

            if length(Starts) > length(Ends)
                Ends = [Ends; length(DetectionIdx)];
            end

            for OutIdx = 1:length(Starts)
                if sum(ExtremeIdx(Starts(OutIdx):Ends(OutIdx))) > 0          
                    OutStart{OutIdx} = TimeString{Starts(OutIdx)};
                    OutEnd{OutIdx} = TimeString{Ends(OutIdx)};
                end
            end
        end

        DetectionResults(index).RateOfChangeOutStart = OutStart;
        DetectionResults(index).RateOfChangeOutEnd = OutEnd;
    end
else
    % Detector cannot be implemented
    
    % Loop through each signal
    for index = 1:size(Data,2)
        DetectionResults(index).RateOfChangeOutStart = NaN;
        DetectionResults(index).RateOfChangeOutEnd = NaN;
    end
end

end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters,fs)

% % Return frequency response measures?
% if isfield(Parameters,'FrequencyResponse')
%     % User has made a selection
%     if ~strcmp(Parameters.FrequencyResponse,'TRUE')
%         % Frequency response measures are to be returned
%         FrequencyResponse = true;
%     elseif ~strcmp(Parameters.FrequencyResponse,'FALSE')
%         % Frequency response measures are not to be returned
%         FrequencyResponse = false;
%     else
%         % User entry is unacceptable, use default (not returned)
%         FrequencyResponse = false;
%     end
% else
%     % User did not specify, use default (not returned)
%     FrequencyResponse = false;
% end

% Upper threshold for duration-based detector
if isfield(Parameters,'DurationMax')
    % Use specified DurationMax, upper threshold
    DurationMax = str2double(Parameters.DurationMax);
    
    if isnan(DurationMax)
        % str2double sets the value to NaN when it can't make it a number
        warning('DurationMax is not a number and will be ignored in duration-based frequency event detector.');
    end
else
    % Upper threshold is not considered 
    DurationMax = NaN;
end

% Lower threshold for duration-based detector
if isfield(Parameters,'DurationMin')
    % Use specified DurationMin, lower threshold
    DurationMin = str2double(Parameters.DurationMin);
    
    if isnan(DurationMin)
        % str2double sets the value to NaN when it can't make it a number
        warning('DurationMin is not a number and will be ignored in duration-based frequency event detector.');
    end
else
    % Lower threshold is not considered 
    DurationMin = NaN;
end

% Duration threshold
if isfield(Parameters,'Duration')
    % Use specified Duration threshold
    % Multiplication by fs produces units of samples
    Duration = round(str2double(Parameters.Duration)*fs);
    
    if isnan(Duration)
        % str2double sets the value to NaN when it can't make it a number
        warning('Duration is not a number. Default of 0 will be used');
        Duration = 0;
    end
else
    % Duration threshold is set to zero 
    Duration = 0;
end

% Analysis window size
if isfield(Parameters,'AnalysisWindow')
    % Use specified analysis window length
    % Multiplication by fs produces units of samples
    AnalysisWindow = str2double(Parameters.AnalysisWindow)*fs;
    
    if isnan(AnalysisWindow)
        % str2double sets the value to NaN when it can't make it a number
        warning('AnalysisWindow is not a number. Duration based frequency event detector will not be implemented.');
    end
else
    warning('Analysis window length was not specified. The duration based frequency event detector will not be implemented.');
    AnalysisWindow = NaN;
end

% Upper threshold for rate-of-change-based detector
if isfield(Parameters,'RateOfChangeMax')
    % Use specified RateOfChangeMax, upper threshold
    RateOfChangeMax = str2double(Parameters.RateOfChangeMax);
    
    if isnan(RateOfChangeMax)
        % str2double sets the value to NaN when it can't make it a number
        warning('RateOfChangeMax is not a number and will be ignored');
    end
else
    % Upper threshold is not considered 
    RateOfChangeMax = NaN;
end

% Lower threshold for rate-of-change-based detector
if isfield(Parameters,'RateOfChangeMin')
    % Use specified DurationMin, lower threshold
    RateOfChangeMin = str2double(Parameters.RateOfChangeMin);
    
    if isnan(RateOfChangeMin)
        % str2double sets the value to NaN when it can't make it a number
        warning('RateOfChangeMin is not a number and will be ignored');
    end
else
    % Lower threshold is not considered 
    RateOfChangeMin = NaN;
end

% Rate of change threshold
if isfield(Parameters,'RateOfChange')
    % Use specified RateOfChange threshold
    % Division by fs produces units of Hz/sample
    RateOfChange = str2double(Parameters.RateOfChange)/fs;
    
    if isnan(RateOfChange)
        % str2double sets the value to NaN when it can't make it a number
        warning('RateOfChange is not a number. Rate-of-change based frequency detector will not be implemented.');
    end
else
    % RateOfChange threshold is not considered 
    RateOfChange = NaN;
end

ExtractedParameters = struct('DurationMax',DurationMax,...
    'DurationMin',DurationMin,'Duration',Duration,...
    'RateOfChangeMax',RateOfChangeMax,'RateOfChangeMin',RateOfChangeMin,...
    'RateOfChange',RateOfChange,'AnalysisWindow',AnalysisWindow);

end