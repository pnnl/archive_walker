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
    warning('Input data for the ringdown detector could not be used.');
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
ExtractedParameters = ExtractParameters(Parameters,fs);

% Store the parameters in variables for easier access
RMSlength = ExtractedParameters.RMSlength;
RMSmedianFilterOrder = ExtractedParameters.RMSmedianFilterOrder;
RingThresholdScale = ExtractedParameters.RingThresholdScale;


%% New RMS energy based detector

DetectionResults = struct('PMU',[],'Channel',[],'RingStart',[],'RingEnd',[]);
MT = cell(1,size(Data,2)); % Get it? Empty!
AdditionalOutput = struct('threshold',MT,'RMS',MT,'RMShist',MT,'FilterConditions',MT);
% AdditionalOutput(1).DataRaw = Data;
AdditionalOutput(1).DataPMU = DataPMU;
AdditionalOutput(1).DataChannel = DataChannel;
AdditionalOutput(1).DataType = DataType;
AdditionalOutput(1).DataUnit = DataUnit;
AdditionalOutput(1).TimeString = TimeString;

% Only active power, frequency, and OTHER signal types are acceptable. If a
% channel has any other type, it is discarded from analysis.
DiscardChannelIdx = find((~strcmp(DataType,'P')) & (~strcmp(DataType,'F')) & (~strcmp(DataType,'OTHER')));
Data(:,DiscardChannelIdx) = NaN;

% Add remaining data after channels discarded
AdditionalOutput(1).Data = Data;

Data2 = Data.^2;

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
            
            % Apply median filter to RMS to establish the threshold
            RMSmed = medfilt1(RMS,RMSmedianFilterOrder,'truncate');
            
            % Use NaNs to pad the front end of the history. This makes the
            % history longer than necessary. Additional samples are removed
            % in the next line of code.
            AdditionalOutput(index).RMShist = [NaN(RMSmedianFilterOrder,1); RMS];
            
            % Keep only the most recent RMSmedianFilterOrder-1 values
            AdditionalOutput(index).RMShist = AdditionalOutput(index).RMShist(max([1 (length(AdditionalOutput(index).RMShist)-(RMSmedianFilterOrder-1)+1)]):end);
        else
            % PastAdditionalOutput is available
            
            % Add previous RMS values to the front end to make the median
            % filter continuous
            RMSmed = medfilt1([PastAdditionalOutput(index).RMShist; RMS],RMSmedianFilterOrder);
            % Remove extra samples
            RMSmed = RMSmed((RMSmedianFilterOrder+1)/2:end-(RMSmedianFilterOrder-1)/2);
            
            % Replace the RMS values in PastAdditionalOutput for next time
            AdditionalOutput(index).RMShist = [PastAdditionalOutput(index).RMShist; RMS];
            % Keep only the most recent RMSmedianFilterOrder-1 values
            AdditionalOutput(index).RMShist = AdditionalOutput(index).RMShist(max([1 (length(AdditionalOutput(index).RMShist)-(RMSmedianFilterOrder-1)+1)]):end);
        end
        % Calculate the threshold
        ThisThreshold = RMSmed*RingThresholdScale;
        
        DetectionIdx = RMS > ThisThreshold;
        
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
        end
        
        % add signal to detected results
        DetectionResults(index).RingStart = RingStart;
        DetectionResults(index).RingEnd = RingEnd;

        AdditionalOutput(index).threshold = ThisThreshold;
        AdditionalOutput(index).RMS = RMS;
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
if isfield(Parameters,'RMSmedianFilterTime')
    % Use specified filter order, converted to samples from time
    RMSmedianFilterOrder = round(str2double(Parameters.RMSmedianFilterTime)*fs);
    
    if isnan(RMSmedianFilterOrder)
        % str2double sets the value to NaN when it can't make it a number
        warning('RMSmedianFilterTime is not a number. Default of 120 seconds will be used.');
        RMSmedianFilterOrder = 120*fs;
    end
    
    if (RMSmedianFilterOrder<=0)
        warning('RMSmedianFilterTime must be positive. Default of 120 seconds will be used.');
        RMSmedianFilterOrder = 120*fs;
    end
else
    % Use default filter time
    RMSmedianFilterOrder = 120*fs;
end
% Ensure that the median filter order is odd
if mod(RMSmedianFilterOrder,2) == 0
    RMSmedianFilterOrder = RMSmedianFilterOrder + 1;
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
    'RMSlength',RMSlength,'RMSmedianFilterOrder',RMSmedianFilterOrder);
end