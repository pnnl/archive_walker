%           PastAdditionalOutput: past AdditionalOutput outputs from this
%                                 function that can be used for various
%                                 purposes.

function [DetectionResults, AdditionalOutput] = RingdownDetector(PMUstruct,Parameters,PastAdditionalOutput)

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
RMSlength = ExtractedParameters.RMSlength;
ForgetFactor = ExtractedParameters.ForgetFactor;
RingThresholdScale = ExtractedParameters.RingThresholdScale;


%% New RMS energy based detector

% NaN values in a channel are unacceptable. If a channel has any NaN
% values, it is discarded from analysis.
DiscardChannelIdx = find(sum(isnan(Data),1) > 0);

% Only active power, frequency, and OTHER signal types are acceptable. If a
% channel has any other type, it is discarded from analysis.
DiscardChannelIdx = sort([DiscardChannelIdx find((~strcmp(DataType,'P')) & (~strcmp(DataType,'F')) & (~strcmp(DataType,'OTHER')))]);

Data2 = Data.^2;

DetectionResults = struct('PMU',[],'Channel',[],'RingStart',[],'RingEnd',[]);
AdditionalOutput = struct('threshold',[],'RMS',[],'TimePoints',[],'NextThreshold',[],'FilterConditions',[]);
% loop through each signal
for index = 1:size(Data2,2)
    % Store the PMU and channel name
    DetectionResults(index).PMU = DataPMU(index);
    DetectionResults(index).Channel = DataChannel(index);

    % Check if channel is to be included
    if ismember(index,DiscardChannelIdx)
        % This channel is not to be included - set to NaN
        DetectionResults(index).RingStart = NaN;
        DetectionResults(index).RingEnd = NaN;
    else
        % This channel is okay to be included
        if isempty(PastAdditionalOutput)
            % PastAdditionalOutput isn't available
            % Get initial conditions by filtering data with a constant 
            % value equal to the first sample of Data.
            [~, InitConditions] = filter(ones(1,RMSlength)/RMSlength,1,Data2(1,index)*ones(ceil(RMSlength/2),1));
        else
            InitConditions = PastAdditionalOutput(index).FilterConditions;
        end
        
        [RMS, AdditionalOutput(index).FilterConditions] = filter(ones(1,RMSlength)/RMSlength,1,Data2(:,index),InitConditions);
        RMS = sqrt(RMS);
        
        if isempty(PastAdditionalOutput)
            % PastAdditionalOutput isn't available
            ThisThreshold = RingThresholdScale*median(RMS);
        else
            if isempty(PastAdditionalOutput(index).NextThreshold)
                % This specific next threshold isn't available
                ThisThreshold = RingThresholdScale*median(RMS);
            else
                ThisThreshold = PastAdditionalOutput(index).NextThreshold;
            end
        end
        
        DetectionIdx = RMS > ones(size(Data2,1),1)*ThisThreshold;
        
        RingStart = {};
        RingEnd = {};
        if sum(DetectionIdx) > 0
            % Ringdown was detected
            
            % Start and end of ringdown (later adjusted for filter delay)
            Starts = find(diff([0; DetectionIdx]) == 1);
            Ends = find(diff(DetectionIdx) == -1);
            
            if length(Starts) > length(Ends)
                Ends = [Ends; length(DetectionIdx)];
            end
            
            for RingIdx = 1:length(Starts)
                % Store ring start and end points while accounting for
                % filter delay in the starting point. Not accounting for
                % the delay in the end point allows overlap in the
                % detection results from adjacent files, which allows a
                % single event occurring across files to be listed as a
                % single event.
                RingStart{RingIdx} = datestr(datenum(TimeString{Starts(RingIdx)})-((RMSlength-1)/2/fs)/(60*60*24),'yyyy-mm-dd HH:MM:SS.FFF');
                RingEnd{RingIdx} = TimeString{Ends(RingIdx)};
            end
            
            % Ringdown was detected - do not update threshold
            AdditionalOutput(index).NextThreshold = ThisThreshold;
        else
            % Ringdown was not detected - update threshold
            AdditionalOutput(index).NextThreshold = ThisThreshold*ForgetFactor + RingThresholdScale*median(RMS)*(1-ForgetFactor);
        end
        
        % add signal to detected results
        DetectionResults(index).RingStart = RingStart;
        DetectionResults(index).RingEnd = RingEnd;

        AdditionalOutput(index).threshold = ThisThreshold;
        AdditionalOutput(index).RMS = RMS;
        AdditionalOutput(index).TimePoints = TimeString;
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

% Length for RMS calculation
if isfield(Parameters,'RMSlength')
    % Use specified length
    RMSlength = str2double(Parameters.RMSlength)*fs;
    
    if isnan(RMSlength)
        % str2double sets the value to NaN when it can't make it a number
        warning('RMSlength is not a number. Default of 15 will be used.');
        RMSlength = 15*fs;
    end
else
    % Use default length
    RMSlength = 15*fs;
end

% Forgetting factor for threshold
if isfield(Parameters,'ForgetFactor')
    % Use specified forgetting factor
    ForgetFactor = str2double(Parameters.ForgetFactor);
    
    if isnan(ForgetFactor)
        % str2double sets the value to NaN when it can't make it a number
        warning('ForgetFactor is not a number. Default of 0.9 will be used.');
        ForgetFactor = 0.9;
    end
    
    if (ForgetFactor<0) || (ForgetFactor>1)
        warning('ForgetFactor is not between 0 and 1. Default of 0.9 will be used.');
        ForgetFactor = 0.9;
    end
else
    % Use default forgetting factor
    ForgetFactor = 0.9;
end

% Scaling term for threshold
if isfield(Parameters,'RingThresholdScale')
    % Use specified scaling term
    RingThresholdScale = str2double(Parameters.RingThresholdScale);
    
    if isnan(RingThresholdScale)
        % str2double sets the value to NaN when it can't make it a number
        warning('RingThresholdScale is not a number. Default of 3 will be used.');
        RingThresholdScale = 3;
    end
    
    if RingThresholdScale < 0
        warning('RingThresholdScale cannot be negative. Default of 3 will be used.');
        RingThresholdScale = 3;
    end
else
    % Use default scaling term
    RingThresholdScale = 3;
end


ExtractedParameters = struct('RingThresholdScale',RingThresholdScale,...
    'RMSlength',RMSlength,'ForgetFactor',ForgetFactor);
end