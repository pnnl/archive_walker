%           PastAdditionalOutput: past AdditionalOutput outputs from this
%                                 function that can be used for various
%                                 purposes.

function [DetectionResults, AdditionalOutput] = OutOfRangeGeneralDetector(PMUstruct,Parameters,PastAdditionalOutput)

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs] = ExtractData(PMUstruct,Parameters);
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
ExtractedParameters = ExtractParameters(Parameters);

% Store the parameters in variables for easier access
Max = ExtractedParameters.Max;
Min = ExtractedParameters.Min;
Duration = ExtractedParameters.Duration;

%% Based on the specified parameters, initialize useful variables



%% Perform detection

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'Max',[],'Min',[],'Duration',[],'Response',[]);

% % counter for the number of out of range signal detected
% signalDetected = 0;
% % Loop throught each signal
% for index = 1:size(Data,2)
%     currentSignal = Data(:,index);
%     aboveUpperLimitIndices = currentSignal > Max;
%     belowLowerLimitIndices = currentSignal < Min;
%     outOfRangeIndices = aboveUpperLimitIndices | belowLowerLimitIndices;
%     % find upper and lower extreme
%     upperExtreme = max(currentSignal(aboveUpperLimitIndices));
%     lowerExtreme = min(currentSignal(belowLowerLimitIndices));
%     totalTimeOutsideRange = size(currentSignal(outOfRangeIndices),1) * diff(t(1:2));
%     % if total time exceeds the Duration threshold, detected!
%     if totalTimeOutsideRange > Duration
%         signalDetected = signalDetected + 1;
%         DetectionResults(signalDetected).PMU = DataPMU(index);
%         DetectionResults(signalDetected).Channel = DataChannel(index);
%         DetectionResults(signalDetected).Max = upperExtreme;
%         DetectionResults(signalDetected).Min = lowerExtreme;
%         DetectionResults(signalDetected).Duration = totalTimeOutsideRange;
%     end
% end

% Loop throught each signal
for index = 1:size(Data,2)
    currentSignal = Data(:,index);
    aboveUpperLimitIndices = currentSignal > Max;
    belowLowerLimitIndices = currentSignal < Min;
    outOfRangeIndices = aboveUpperLimitIndices | belowLowerLimitIndices;
    % find upper and lower extreme
    upperExtreme = max(currentSignal(aboveUpperLimitIndices));
    lowerExtreme = min(currentSignal(belowLowerLimitIndices));
    totalTimeOutsideRange = size(currentSignal(outOfRangeIndices),1) * diff(t(1:2));
    % if total time exceeds the Duration threshold, detected!
    DetectionResults(index).PMU = DataPMU(index);
    DetectionResults(index).Channel = DataChannel(index);
    if isempty(upperExtreme)
        DetectionResults(index).Max = NaN;
    else
        DetectionResults(index).Max = upperExtreme;
    end
    if isempty(lowerExtreme)
        DetectionResults(index).Min = NaN;
    else
        DetectionResults(index).Min = lowerExtreme;
    end
    if totalTimeOutsideRange > Duration
        DetectionResults(index).Duration = totalTimeOutsideRange;
    else        
        DetectionResults(index).Duration = NaN;
    end
end

% Initialize structure for additional outputs
AdditionalOutput = struct([]);

end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters)
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
    Duration = str2double(Parameters.Duration);
else
    % Duration threshold is set to zero 
    Duration = 0;
end
ExtractedParameters = struct('Max',Max,'Min',Min,'Duration',Duration);

end