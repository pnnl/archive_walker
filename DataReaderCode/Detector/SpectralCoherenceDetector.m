function [DetectionResults, AdditionalOutput] = SpectralCoherenceDetector(PMUstruct,Parameters)

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, t, fs] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the periodogram detector could not be used.');
    DetectionResults = struct([]);
    AdditionalOutput = struct([]);
    return
end

%% Get detector parameters

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. 
% Additional inputs, such as the length of the input data or the sampling 
% rate, can be added as necessary. 
ExtractedParameters = ExtractParameters(Parameters);

% Store the parameters in variables for easier access
Mode = ExtractedParameters.Mode;


%% Based on the specified parameters, initialize useful variables



%% Perform detection

% Initialize structure to output detection results
if strcmp(Mode,'SingleChannel')
    DetectionResults = struct('PMU',[],'Channel',[],'Frequency',[],'Coherence',[]);
else
    DetectionResults = struct('PMU',[],'Channel',[],'Frequency',[],'Coherence',[],'TestStatistic',[]);
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

% Mode of operation - 'SingleChannel' or 'MultiChannel'
if isfield(Parameters,'Mode')
    % Use specified mode
    Mode = Parameters.Mode;
else
    % Use default mode: single channel
    Mode = 'SingleChannel';
end

ExtractedParameters = struct('Mode',Mode);

end