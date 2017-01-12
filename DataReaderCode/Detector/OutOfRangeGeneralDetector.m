%           PastAdditionalOutput: past AdditionalOutput outputs from this
%                                 function that can be used for various
%                                 purposes.

function [DetectionResults, AdditionalOutput] = OutOfRangeGeneralDetector(PMUstruct,Parameters,PastAdditionalOutput)

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


%% Get detector parameters

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. 
% Additional inputs, such as the length of the input data or the sampling 
% rate, can be added as necessary. 
persistent ExtractedParameters
if isempty(ExtractedParameters)
    ExtractedParameters = ExtractParameters(Parameters,fs);
end

% Store the parameters in variables for easier access
Max = ExtractedParameters.Max;
Min = ExtractedParameters.Min;
AnalysisWindow = ExtractedParameters.AnalysisWindow;
Duration = ExtractedParameters.Duration;

%% Error checks
if Duration > AnalysisWindow
    warning('Duration cannot be greater than AnalysisWindow. Default of 0 will be used.');
    Duration = 0;
end

if Max < Min
    warning('Max is less than Min. General out-of-range event detector will not be implemented.')
    Max = NaN;
    Min = NaN;
    Duration = NaN;
    AnalysisWindow = NaN;
end


%% Perform detection

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'OutStart',[],'OutEnd',[]);

if ~isnan(AnalysisWindow)
    % Logical matrix to mark locations where Data goes outside of the detection
    % thresholds. The matrix remains false where Data is NaN so that these
    % values are ignored, as specified.
    OutOfBounds = false(size(Data));
    % Set the matrix to true where Data goes above its upper detection
    % threshold. Max is only NaN when it is not to be included.
    if ~isnan(Max)
        OutOfBounds(Data > Max) = true;
    end
    % Set the matrix to true where Data goes below its lower detection
    % threshold. Min is only NaN when it is not to be included.
    if ~isnan(Min)
        OutOfBounds(Data < Min) = true;
    end

    AdditionalOutput = struct('FilterConditions',[]);

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

        [OutsideCount, AdditionalOutput(index).FilterConditions] = filter(ones(1,AnalysisWindow),1,OutOfBounds(:,index),InitConditions);

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

        DetectionResults(index).OutStart = OutStart;
        DetectionResults(index).OutEnd = OutEnd;
    end
else
    % Detector cannot be implemented
    
    % Loop throught each signal
    for index = 1:size(Data,2)
        % Store the PMU and channel name
        DetectionResults(index).PMU = DataPMU(index);
        DetectionResults(index).Channel = DataChannel(index);
        DetectionResults(index).OutStart = NaN;
        DetectionResults(index).OutEnd = NaN;
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
% Upper threshold
if isfield(Parameters,'Max')
    % Use specified Max, upper threshold
    Max = str2double(Parameters.Max);
    
    if isnan(Max)
        % str2double sets the value to NaN when it can't make it a number
        warning('Max is not a number and will be ignored in the general out-of-range event detector.');
    end
else
    % Upper threshold is not considered 
    Max = NaN;
end
% Lower threshold
if isfield(Parameters,'Min')
    % Use specified Min, lower threshold
    Min = str2double(Parameters.Min);
    
    if isnan(Min)
        % str2double sets the value to NaN when it can't make it a number
        warning('Min is not a number and will be ignored in the general out-of-range event detector.');
    end
else
    % Lower threshold is not considered 
    Min = NaN;
end
% Duration threshold
if isfield(Parameters,'Duration')
    % Use specified Duration threshold
    Duration = round(str2double(Parameters.Duration)*fs);
    
    if isnan(Min)
        % str2double sets the value to NaN when it can't make it a number
        warning('Duration is not a number. Default of 0 will be used in the general out-of-range event detector.');
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
        warning('AnalysisWindow is not a number. General out-of-range event detector will not be implemented.');
    end
else
    warning('AnalysisWindow is not a number. General out-of-range event detector will not be implemented.');
    AnalysisWindow = NaN;
end

ExtractedParameters = struct('Max',Max,'Min',Min,'Duration',Duration,'AnalysisWindow',AnalysisWindow);

end