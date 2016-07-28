function [DetectionResults, AdditionalOutput] = OutOfRangeFrequencyDetector(PMUstruct,Parameters)

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

FrequencyResponse = ExtractedParameters.FrequencyResponse;
DurationMax = ExtractedParameters.DurationMax;
DurationMin = ExtractedParameters.DurationMin;
Duration = ExtractedParameters.Duration;
RateOfChangeMax = ExtractedParameters.RateOfChangeMax;
RateOfChangeMin = ExtractedParameters.RateOfChangeMin;
RateOfChange = ExtractedParameters.RateOfChange;




%% Based on the specified parameters, initialize useful variables

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'Max',[],'Min',[],'Duration',[],'Response',[]);

upperExtreme = [];
lowerExtreme = [];
totalTimeOutsideRange = [];
upperTightExtreme = [];
lowerTightExtreme = [];
totalTimeOutsideTightRange=[];

% find the limits
MaxLoose = DurationMax * 60;
MinLoose = DurationMin * 60;
MaxTight = RateOfChangeMax * 60;
MinTight = RateOfChangeMin * 60;
deltaT = diff(t(1:2));

%% Perform detection
% loop through each signal
for index = 1:size(Data,2)    
    if strcmp(DataUnit(index),'Hz')
        % check the loose limits first
        currentSignal = Data(:,index);
        aboveUpperLimitIndices = currentSignal > MaxLoose;
        belowLowerLimitIndices = currentSignal < MinLoose;
        outOfRangeIndices = aboveUpperLimitIndices | belowLowerLimitIndices;
        % find upper and lower extreme
        upperExtreme = max(currentSignal(aboveUpperLimitIndices));
        lowerExtreme = min(currentSignal(belowLowerLimitIndices));
        totalTimeOutsideRange = size(currentSignal(outOfRangeIndices),1) * deltaT;


        % then check the tight limits
        aboveUpperTightLimitIndices = currentSignal > MaxTight;
        belowLowerTightLimitIndices = currentSignal < MinTight;
        outOfTightRangeIndicesLogic = aboveUpperTightLimitIndices | belowLowerTightLimitIndices

        totalTimeOutsideTightRange = size(currentSignal(outOfTightRangeIndicesLogic),1) * deltaT;

        outOfTightRangeIndices = find(outOfTightRangeIndicesLogic);
        % find the all the out of tight range sections in signal
        rangeStartingIndices = [0; find(diff(outOfTightRangeIndices)>1); length(outOfTightRangeIndices)];
        numberOfOutofrangeRanges = length(rangeStartingIndices)-1;

        nFrequencyResponse = 0;
        % for each out of tight range section
        for nRange = 1:numberOfOutofrangeRanges
            % find the starting index of the section
            startingIndex = outOfTightRangeIndices(rangeStartingIndices(nRange) + 1);
            % find the ending index of the section
            endingIndex = outOfTightRangeIndices(rangeStartingIndices(nRange + 1));
            % calculate all the rate of change in this section
            deltaFInThisRange = diff(currentSignal(startingIndex-1:endingIndex+1));
            % find the value and location of the maximum rate of change in this
            % section
            [peakValue,peakIndex]=max(abs(deltaFInThisRange));
            % if the above found maximum rate of change is larger than
            % threshold, then figure out the frequency response of A,B,C points
            if peakValue> RateOfChange
                nFrequencyResponse = nFrequencyResponse + 1;
                tZeroIndex = peakIndex+startingIndex-1;
                timeBeforePointA = 16/deltaT;
                transientPeriod = 20/deltaT;
                TimeAfterPointB = 32/deltaT;
                % find partial array end in the index where the max rate change
                % is
                partialDeltaFInThisRange = deltaFInThisRange(1:peakIndex);
                if deltaFInThisRange(peakIndex)<0
                    % sudden drop, go back from the maximum rate change up to
                    % the top point where the drop started
                    pointAIndex = find(partialDeltaFInThisRange>0,1,'last')+startingIndex-1;
                else
                    % sudden increase, go back from the maximum rate change
                    % down to the lowest point where the increase started
                    pointAIndex = find(partialDeltaFInThisRange<0,1,'last')+startingIndex-1;
                end
                if timeBeforePointA < pointAIndex
                    startingTimeIndex = pointAIndex - timeBeforePointA+1;
                else
                    % edge case where A is less than 16 seconds from the start
                    % of the signal
                    startingTimeIndex = 0;
                end
                response(nFrequencyResponse).A = mean(currentSignal(startingTimeIndex:pointAIndex));
                if pointAIndex+transientPeriod > length(currentSignal)
                    % edge case where transient period is less than 20 seconds
                    pointBIndex = length(currentSignal);
                else
                    pointBIndex = pointAIndex+transientPeriod;
                end
                if pointBIndex+TimeAfterPointB > length(currentSignal)
                    % edge case where B point is less than 32 seconds till the
                    % end of the signal
                    response(nFrequencyResponse).B = mean(currentSignal(pointBIndex+1:end));
                else
                    response(nFrequencyResponse).B = mean(currentSignal(pointBIndex+1:pointBIndex+TimeAfterPointB));
                end
                if deltaFInThisRange(peakIndex)<0
                    % sudden drop, C is the lowest point during transient
                    % period
                    response(nFrequencyResponse).C = min(currentSignal(pointAIndex+1:pointBIndex));
                    lowerTightExtreme(nFrequencyResponse) = response(nFrequencyResponse).C;
                else
                    % sudden increase, C is the highest point during transient
                    % period
                    response(nFrequencyResponse).C = max(currentSignal(pointAIndex+1:pointBIndex));
                    upperTightExtreme(nFrequencyResponse) = response(nFrequencyResponse).C;
                end
            end
        end
        % if out of loose range, reporting the one peak out of loose range and all
        % frequency response (if requested)
        if totalTimeOutsideRange > Duration
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
            DetectionResults(index).Duration = totalTimeOutsideRange;       
        else
            % if not out of loose range, reporting the all peaks that is out of
            % tight range and over the rate of change threshold, plus all the
            % frequency response (if requested)
            DetectionResults(index).PMU = DataPMU(index);
            DetectionResults(index).Channel = DataChannel(index);
            if isempty(upperTightExtreme)
                DetectionResults(index).Max = NaN;
            else
                DetectionResults(index).Max = upperTightExtreme;
            end
            if isemtpy(lowerTightExtreme)
                DetectionResults(index).Min = NaN;
            else
                DetectionResults(index).Min = lowerTightExtreme;
            end
            if isemtpy(totalTimeOutsideTightRange)
                DetectionResults(index).Duration = NaN;
            else
                DetectionResults(index).Duration = totalTimeOutsideTightRange;
            end
        end
        if FrequencyResponse
            DetectionResults(index).Response = response;
        end
    else
        waring(['Wrong signal type for the out of range frequency detector!'...
            'Only frequency signal allowed, but input is'...
            'type %s of PMU %s, channel %s.',DataType(index),DataPMU(index),DataChannel(index)]);
        DetectionResults(index).PMU = DataPMU(index);
        DetectionResults(index).Channel = DataChannel(index);
        DetectionResults(index).Max = NaN;
        DetectionResults(index).Min = NaN;
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
% If TRUE, the frequency response measures are returned. If FALSE or omitted, the measures are not returned
if isfield(Parameters,'FrequencyResponse')
    % Use specified frequency response
    FrequencyResponse = Parameters.FrequencyResponse;
else
    FrequencyResponse = False;
end

% Upper threshold for the normal range as percentage of nominal. If omitted, it is not considered.
if isfield(Parameters,'DurationMax')
    DurationMax = str2double(Parameters.DurationMax);
else
    DurationMax = NaN;
end

% Lower threshold for the normal range as percentage of nominal. If omitted, it is not considered.
if isfield(Parameters, 'DurationMin')
    DurationMin = str2double(Parameters.DurationMin);
else
    DurationMin = NaN;
end

% Duration threshold in samples. If omitted, it is set to zero.
if isfield(Parameters, 'Duration')
    Duration = str2double(Parameters.Duration);
else
    Duration = 0;
end

% Upper threshold for the normal range as percentage of nominal. If omitted, it is not considered.
if isfield(Parameters, 'RateOfChangeMax')
    RateOfChangeMax = str2double(Parameters.RateOfChangeMax);
else
    RateOfChangeMax = NaN;
end

% Lower threshold for the normal range as percentage of nominal. If omitted, it is not considered.
if isfield(Parameters, 'RateOfChangeMin')
    RateOfChangeMin = str2double(Parameters.RateOfChangeMin);
else
    RateOfChangeMin = NaN;
end

% Rate-of-change threshold in Hz/sample. If omitted, this part of the detector is not implemented.
if isfield(Parameters, 'RateOfChange')
    RateOfChange = str2double(Parameters.RateOfChange);
else
    RateOfChange = NaN;
end

ExtractedParameters = struct('FrequencyResponse',FrequencyResponse,'DurationMax',DurationMax,...
    'DurationMin',DurationMin,'Duration',Duration,'RateOfChangeMax',RateOfChangeMax,...
    'RateOfChangeMin',RateOfChangeMin,'RateOfChange',RateOfChange);

end