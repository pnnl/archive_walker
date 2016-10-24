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
ExtractedParameters = ExtractParameters(Parameters,fs);

% Store the parameters in variables for easier access
Max = ExtractedParameters.Max;
Min = ExtractedParameters.Min;
AnalysisWindow = ExtractedParameters.AnalysisWindow;
Duration = ExtractedParameters.Duration;

%% Based on the specified parameters, initialize useful variables



%% Perform detection

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'OutStart',[],'OutEnd',[]);

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
else
    % Upper threshold is not considered 
    Max = NaN;
end
% Lower threshold
if isfield(Parameters,'Min')
    % Use specified Min, lower threshold
    Min = str2double(Parameters.Min);
else
    % Lower threshold is not considered 
    Min = NaN;
end
% Duration threshold
if isfield(Parameters,'Duration')
    % Use specified Duration threshold
    Duration = str2double(Parameters.Duration)*fs;
else
    % Duration threshold is set to zero 
    Duration = 0;
end
% Analysis window size
if isfield(Parameters,'AnalysisWindow')
    % Use specified analysis window length
    AnalysisWindow = str2double(Parameters.AnalysisWindow)*fs;
else
    error('Analysis window length must be specified for general out-of-range detector.');
end
ExtractedParameters = struct('Max',Max,'Min',Min,'Duration',Duration,'AnalysisWindow',AnalysisWindow);

end