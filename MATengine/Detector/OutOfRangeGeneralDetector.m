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
    warning('Input data for the out-of-range detector could not be used.');
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
Nominal = ExtractedParameters.Nominal;
AverageWindow = ExtractedParameters.AverageWindow;
%
DurationMax = ExtractedParameters.DurationMax;
DurationMin = ExtractedParameters.DurationMin;
Duration = ExtractedParameters.Duration;
AnalysisWindow = ExtractedParameters.AnalysisWindow;
%
RateOfChangeMax = ExtractedParameters.RateOfChangeMax;
RateOfChangeMin = ExtractedParameters.RateOfChangeMin;
RateOfChange = ExtractedParameters.RateOfChange;
EventMergeWindow = ExtractedParameters.EventMergeWindow;


%% Error checks
if Duration > AnalysisWindow
    warning('Duration cannot be greater than AnalysisWindow. Default of 0 will be used.');
    Duration = 0;
end

if DurationMax < DurationMin
    warning('DurationMax is less than DurationMin. Duration-based out-of-range event detector will not be implemented.')
    DurationMax = NaN;
    DurationMin = NaN;
    Duration = NaN;
    AnalysisWindow = NaN;
end

if RateOfChangeMax < RateOfChangeMin
    warning('RateOfChangeMax is less than RateOfChangeMin. Rate-of-change-based out-of-range event detector will not be implemented.')
    RateOfChangeMax = NaN;
    RateOfChangeMin = NaN;
    RateOfChange = NaN;
    EventMergeWindow = NaN;
end


%% Perform detection

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'DurationOutStart',[],'DurationOutEnd',[],'DurationExtrema',[],'DurationExtremaFactor',[],'RateOfChangeOutStart',[],'RateOfChangeOutEnd',[],'RateOfChangeExtrema',[],'RateOfChangeExtremaFactor',[]);

% Initialize structure for additional outputs
AdditionalOutput = struct('FilterConditions',cell(1,size(Data,2)),'AverageFilterConditions',cell(1,size(Data,2)));
% AdditionalOutput(1).DataRaw = Data;
AdditionalOutput(1).Data = Data;
AdditionalOutput(1).DataPMU = DataPMU;
AdditionalOutput(1).DataChannel = DataChannel;
AdditionalOutput(1).DataType = DataType;
AdditionalOutput(1).DataUnit = DataUnit;
AdditionalOutput(1).TimeString = TimeString;

if isempty(PastAdditionalOutput)
    % PastAdditionalOutput isn't available
    PastAdditionalOutput = struct('FilterConditions',cell(1,size(Data,2)),'AverageFilterConditions',cell(1,size(Data,2)),'ExtremeLocs',cell(1,size(Data,2)),'ExtremeVals',cell(1,size(Data,2)));
end

if ~isnan(AverageWindow)
    % Use a running average to set the base for the detector
    % The filtering could be done in one call to filter(), but this way
    % provides consistency within AdditionalOutput
    DataAvg = NaN(size(Data));
    for index = 1:size(Data,2)
        if isempty(PastAdditionalOutput(index).AverageFilterConditions)
            [~,PastAdditionalOutput(index).AverageFilterConditions] = filter(ones(1,AverageWindow)/AverageWindow,1,Data(1,index)*ones(AverageWindow,1));
        end
        
        [DataAvg(:,index),AdditionalOutput(index).AverageFilterConditions] = filter(ones(1,AverageWindow)/AverageWindow,1,Data(:,index),PastAdditionalOutput(index).AverageFilterConditions);
    end
    
    Base = DataAvg;
else
    % Use a nominal value to set the base for the detector
    Base = Nominal*ones(size(Data));
end

DurationMaxMat = Base + DurationMax;
DurationMinMat = Base + DurationMin;
RateOfChangeMaxMat = Base + RateOfChangeMax;
RateOfChangeMinMat = Base + RateOfChangeMin;

% figure(101); hold on;
% xlims = xlim;
% idx = floor(xlims(2)/3600)*3600;
% idx = idx+1:idx+3600;
% plot(idx,Data(:,1),'b');
% plot(idx,DurationMaxMat(:,1),'r');
% plot(idx,DurationMinMat(:,1),'r');
% plot(idx,RateOfChangeMaxMat(:,1),'g');
% plot(idx,RateOfChangeMinMat(:,1),'g');
% hold off;
% xlim([0 idx(end)]);
% propedit;
% drawnow

if ~isnan(AnalysisWindow)
    % Logical matrix to mark locations where Data goes outside of the detection
    % thresholds for the duration-based detector. The matrix remains false 
    % where Data is NaN so that these values are ignored, as specified.
    OutOfBoundsDuration = false(size(Data));
    % Set the matrix to true where Data goes above its upper detection
    % threshold. DurationMax is only NaN when it is not to be included.
    if ~isnan(DurationMax)
        OutOfBoundsDuration(Data > DurationMaxMat) = true;
    end
    % Set the matrix to true where Data goes below its lower detection
    % threshold. DurationMin is only NaN when it is not to be included.
    if ~isnan(DurationMin)
        OutOfBoundsDuration(Data < DurationMinMat) = true;
    end

    % Loop throught each signal
    OutsideCount = zeros(size(Data));
    for index = 1:size(Data,2)
        if isempty(PastAdditionalOutput)
            % PastAdditionalOutput isn't available
            InitConditions = [];
        else
            InitConditions = PastAdditionalOutput(index).FilterConditions;
        end

        [OutsideCount(:,index), AdditionalOutput(index).FilterConditions] = filter(ones(1,AnalysisWindow),1,OutOfBoundsDuration(:,index),InitConditions);

        
%         figure(102); hold on;
%         xlims = xlim;
%         idx = floor(xlims(2)/3600)*3600;
%         idx = idx+1:idx+3600;
%         plot(idx,OutsideCount,'b');
%         plot(idx,Duration*ones(size(idx)),'r');
%         hold off;
%         xlim([0 idx(end)]);
%         propedit;
%         drawnow



        DetectionIdx = OutsideCount(:,index) > Duration;

        OutStart = {};
        OutEnd = {};
        Extrema = {};
        ExtremaFactor = {};
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
                
                % Adjust the start time by the length of the analysis
                % window so that the recorded extrema is for the entirety
                % of the analysis window, rather than just the part where
                % the duration was reached. Only part of the analysis 
                % window can be considered if it includes part of a
                % previous file - the index cannot fall below 1.
                % Thus, this step is done after the time is recorded in the
                % first line of the for loop.
                Starts(OutIdx) = max([1, Starts(OutIdx)-AnalysisWindow]);
                
                WinIdx = Starts(OutIdx):Ends(OutIdx);
                DiffHigh = Data(WinIdx,index) - DurationMaxMat(WinIdx,index);
                DiffHigh(DiffHigh < 0) = 0;
                DiffLow = DurationMinMat(WinIdx,index) - Data(WinIdx,index);
                DiffLow(DiffLow < 0) = 0;
                [~,DiffIdx] = max(max(DiffHigh,DiffLow));
                Extrema{OutIdx} = Data(WinIdx(DiffIdx(1)),index);
                ExtremaFactor{OutIdx} = abs((Base(WinIdx(DiffIdx(1)),index) - Extrema{OutIdx})/Base(WinIdx(DiffIdx(1)),index));
            end
        end

        % Store the PMU, channel name, and results
        DetectionResults(index).PMU = DataPMU(index);
        DetectionResults(index).Channel = DataChannel(index);
        DetectionResults(index).DurationOutStart = OutStart;
        DetectionResults(index).DurationOutEnd = OutEnd;
        DetectionResults(index).DurationExtrema = Extrema;
        DetectionResults(index).DurationExtremaFactor = ExtremaFactor;
    end
else
    % Detector cannot be implemented
    
    OutsideCount = NaN;
    
    % Loop throught each signal
    for index = 1:size(Data,2)
        % Store the PMU and channel name
        DetectionResults(index).PMU = DataPMU(index);
        DetectionResults(index).Channel = DataChannel(index);
        DetectionResults(index).DurationOutStart = NaN;
        DetectionResults(index).DurationOutEnd = NaN;
        DetectionResults(index).DurationExtrema = NaN;
        DetectionResults(index).DurationExtremaFactor = NaN;
    end
end

AdditionalOutput(1).DurationMaxMat = DurationMaxMat;
AdditionalOutput(1).DurationMinMat = DurationMinMat;
AdditionalOutput(1).OutsideCount = OutsideCount;
AdditionalOutput(1).Duration = Duration;




if ~isnan(EventMergeWindow)
    % Logical matrix to mark locations where Data goes outside of the detection
    % thresholds for the rate-of-change-based detector. The matrix remains false 
    % where Data is NaN so that these values are ignored, as specified.
    OutOfBoundsRateOfChange = false(size(Data));
    % Set the matrix to true where Data goes above its upper detection
    % threshold. RateOfChangeMax is only NaN when it is not to be included.
    if ~isnan(RateOfChangeMax)
        OutOfBoundsRateOfChange(Data > RateOfChangeMaxMat) = true;
    end
    % Set the matrix to true where Data goes below its lower detection
    % threshold. RateOfChangeMin is only NaN when it is not to be included.
    if ~isnan(RateOfChangeMin)
        OutOfBoundsRateOfChange(Data < RateOfChangeMinMat) = true;
    end

    % Find the rate of change - loop through each signal
    ExtremeLocs = cell(1,size(Data,2));
    ExtremeVals = cell(1,size(Data,2));
    Rate = cell(1,size(Data,2));
    for index = 1:size(Data,2)
        [PeakVals, PeakLocs] = findpeaks(Data(:,index));
        [ValleyVals, ValleyLocs] = findpeaks(-Data(:,index));
        ValleyVals = -ValleyVals;

        % Create a list of alternating maxima and minima
        ExtremeVals{index} = [PeakVals; ValleyVals];
        ExtremeLocs{index} = [PeakLocs; ValleyLocs];
        [ExtremeLocs{index}, SortIdx] = sort(ExtremeLocs{index});
        ExtremeVals{index} = ExtremeVals{index}(SortIdx);
        
        % In the following code, initial points are added to the locations
        % and values of extrema to provide continuity from file to file.
        if isempty(PastAdditionalOutput(1).ExtremeLocs)
            % Information about previous extrema is not available, either
            % because this is the first file processed or the previous file
            % was missing.
            ExtremeLocs{index} = [1; ExtremeLocs{index}; size(Data,1)];
            ExtremeVals{index} = [Data(1,index); ExtremeVals{index}; Data(end,index)];
        elseif isempty(ExtremeLocs{index})
            % This signal has no extreme values, probably because it is all
            % NaN (file was missing). Do nothing. No events will be
            % detected.
        elseif length(PastAdditionalOutput(1).ExtremeLocs{index}) < 2
            % Not enough previous values were recorded to be used. This
            % normally occurs because it was NaN or because the whole file
            % was missing.
            ExtremeLocs{index} = [1; ExtremeLocs{index}; size(Data,1)];
            ExtremeVals{index} = [Data(1,index); ExtremeVals{index}; Data(end,index)];
        elseif isnan(PastAdditionalOutput(1).ExtremeLocs{index}(end))
            % The last sample in the previous file was NaN there is only 
            % one entry in ExtremeLocs and ExtremeVals, so start the
            % process over.
            ExtremeLocs{index} = [1; ExtremeLocs{index}; size(Data,1)];
            ExtremeVals{index} = [Data(1,index); ExtremeVals{index}; Data(end,index)];
        else
            % Take information from the previous file.
            % The following convention is used:
            %   ex0 - the final extrema in the previous file
            %   en - the final point in the previous file
            %   st - the fist point in the current file
            %   ex1 - the first extrema in the current file
            ex0Loc = PastAdditionalOutput(1).ExtremeLocs{index}(end-1) - size(Data,1);
            enLoc = 0;  % = PastAdditionalOutput(1).ExtremeLocs{index}(end) - size(Data,1);
            stLoc = 1;
            ex1Loc = ExtremeLocs{index}(1);
            ex0Val = PastAdditionalOutput(1).ExtremeVals{index}(end-1);
            enVal = PastAdditionalOutput(1).ExtremeVals{index}(end);
            stVal = Data(1,index);
            ex1Val = ExtremeVals{index}(1);
            
            % The following if statements are used to determine how the
            % values of ex0, en, st, and ex1 are related to each other. It
            % determines which points are peaks, valleys, or inbetween.
            % Once determined, peaks and valleys are added to the front of
            % ExtremeLocs and ExtremeVals. 
            % In all cases, the final point of the file is also added.
            if ex0Val > enVal
                % ex0Val > enVal
                if enVal > stVal
                    % enVal > stVal
                    if stVal > ex1Val
                        % stVal > ex1Val

                        % ex0Val > enVal > stVal > ex1Val
                        % ex0: peak
                        % en:
                        % st:
                        % ex1: valley
                        ExtremeLocs{index} = [ex0Loc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [ex0Val; ExtremeVals{index}; Data(end,index)];
                    else
                        % stVal < ex1Val

                        % ex0Val > enVal > stVal < ex1Val
                        % ex0: peak
                        % en:
                        % st: valley
                        % ex1: peak
                        ExtremeLocs{index} = [ex0Loc; stLoc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [ex0Val; stVal; ExtremeVals{index}; Data(end,index)];
                    end
                else
                    % enVal < stVal
                    if stVal > ex1Val
                        % stVal > ex1Val

                        % ex0Val > enVal < stVal > ex1Val
                        % ex0: peak
                        % en: valley
                        % st: peak
                        % ex1: valley
                        ExtremeLocs{index} = [enLoc; stLoc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [enVal; stVal; ExtremeVals{index}; Data(end,index)];
                    else
                        % stVal < ex1Val

                        % ex0Val > enVal < stVal < ex1Val
                        % ex0: peak
                        % en: valley
                        % st: 
                        % ex1: peak
                        ExtremeLocs{index} = [enLoc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [enVal; ExtremeVals{index}; Data(end,index)];
                    end
                end
            else
                % ex0Val < enVal
                if enVal > stVal
                    % enVal > stVal
                    if stVal > ex1Val
                        % stVal > ex1Val

                        % ex0Val < enVal > stVal > ex1Val
                        % ex0: valley
                        % en: peak
                        % st: 
                        % ex1: valley
                        ExtremeLocs{index} = [enLoc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [enVal; ExtremeVals{index}; Data(end,index)];
                    else
                        % stVal < ex1Val

                        % ex0Val < enVal > stVal < ex1Val
                        % ex0: valley
                        % en: peak
                        % st: valley
                        % ex1: peak
                        ExtremeLocs{index} = [enLoc; stLoc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [enVal; stVal; ExtremeVals{index}; Data(end,index)];
                    end
                else
                    % enVal < stVal
                    if stVal > ex1Val
                        % stVal > ex1Val

                        % ex0Val < enVal < stVal > ex1Val
                        % ex0: valley
                        % en: 
                        % st: peak
                        % ex1: valley
                        ExtremeLocs{index} = [ex0Loc; stLoc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [ex0Val; stVal; ExtremeVals{index}; Data(end,index)];
                    else
                        % stVal < ex1Val

                        % ex0Val < enVal < stVal < ex1Val
                        % ex0: valley
                        % en: 
                        % st: 
                        % ex1: peak
                        ExtremeLocs{index} = [ex0Loc; ExtremeLocs{index}; size(Data,1)];
                        ExtremeVals{index} = [ex0Val; ExtremeVals{index}; Data(end,index)];
                    end
                end
            end
        end

        SampChange = diff(ExtremeLocs{index});
        FreqChange = abs(diff(ExtremeVals{index}));

        Rate{index} = FreqChange./SampChange;
        OverRate = find(Rate{index} > RateOfChange);

        ExtremeIdx = false(size(Data,1),1);
        for OverRateIdx = OverRate.'
            ExtremeIdx(max([ExtremeLocs{index}(OverRateIdx), 1]):ExtremeLocs{index}(OverRateIdx+1)) = true;
        end
        DetectionIdx = OutOfBoundsRateOfChange(:,index) & ExtremeIdx;

        OutStart = {};
        OutEnd = {};
        Extrema = {};
        ExtremaFactor = {};
        if sum(DetectionIdx) > 0
            % Out-of-range data was detected

            % Start and end of out-of-range data
            Starts = find(diff([0; DetectionIdx]) == 1);
            Ends = find(diff(DetectionIdx) == -1);

            if length(Starts) > length(Ends)
                Ends = [Ends; length(DetectionIdx)];
            end
            
            for OutIdx = 1:length(Ends)
                % Indices of inbounds points
                InBoundsIdx = find(~OutOfBoundsRateOfChange(:,index));
                % Find the first inbounds point after the current end point
                if isempty(InBoundsIdx)
                    NextInBoundsIdx = [];
                else
                    NextInBoundsIdx = InBoundsIdx(find(InBoundsIdx > Ends(OutIdx),1,'first'));
                end
                % If an inbound point could not be found, assume it is the
                % next sample beyond this file
                if isempty(NextInBoundsIdx)
                    NextInBoundsIdx = size(Data,1)+1;
                end
                % Subtract one to find the last out-of-bounds point in this
                % group of out-of-bounds points and set it as the end of
                % the group.
                Ends(OutIdx) = NextInBoundsIdx-1;
            end

            for OutIdx = 1:length(Starts)
                if sum(ExtremeIdx(Starts(OutIdx):Ends(OutIdx))) > 0          
%                     OutStart{OutIdx} = TimeString{Starts(OutIdx)};
                    OutStart{OutIdx} = datestr(datenum(TimeString{Starts(OutIdx)})-(EventMergeWindow/fs)/(60*60*24),'yyyy-mm-dd HH:MM:SS.FFF');
                    OutEnd{OutIdx} = TimeString{Ends(OutIdx)};
                    
                    % Adjust the start time by the EventMergeWindow 
                    % parameter so that the recorded extrema is for the entirety
                    % of the considered window, rather than just the part where
                    % the Rate was reached. Only part of the analysis 
                    % window can be considered if it includes part of a
                    % previous file - the index cannot fall below 1.
                    % Thus, this step is done after the time is recorded in the
                    % first line of the for loop.
                    Starts(OutIdx) = max([1, Starts(OutIdx)-EventMergeWindow]);
                    
                    WinIdx = Starts(OutIdx):Ends(OutIdx);
                    DiffHigh = Data(WinIdx,index) - RateOfChangeMaxMat(WinIdx,index);
                    DiffHigh(DiffHigh < 0) = 0;
                    DiffLow = RateOfChangeMinMat(WinIdx,index) - Data(WinIdx,index);
                    DiffLow(DiffLow < 0) = 0;
                    [~,DiffIdx] = max(max(DiffHigh,DiffLow));
                    Extrema{OutIdx} = Data(WinIdx(DiffIdx(1)),index);
                    ExtremaFactor{OutIdx} = abs((Base(WinIdx(DiffIdx(1)),index) - Extrema{OutIdx})/Base(WinIdx(DiffIdx(1)),index));
                end
            end
        end

        DetectionResults(index).RateOfChangeOutStart = OutStart;
        DetectionResults(index).RateOfChangeOutEnd = OutEnd;
        DetectionResults(index).RateOfChangeExtrema = Extrema;
        DetectionResults(index).RateOfChangeExtremaFactor = ExtremaFactor;
        
        % Only retain the final two entries, they're all that is necessary
        % when the next file is loaded
        % Commented out on 5/3/18 because it messes up the rerun, which
        % needs the entire vector.
%         if length(ExtremeLocs{index}) > 1
%             ExtremeLocs{index} = ExtremeLocs{index}(end-1:end);
%         end
    end
else
    % Detector cannot be implemented
    
    ExtremeLocs = NaN;
    ExtremeVals = NaN;
    Rate = NaN;
    
    % Loop through each signal
    for index = 1:size(Data,2)
        DetectionResults(index).RateOfChangeOutStart = NaN;
        DetectionResults(index).RateOfChangeOutEnd = NaN;
        DetectionResults(index).RateOfChangeExtrema = NaN;
        DetectionResults(index).RateOfChangeExtremaFactor = NaN;
    end
end

AdditionalOutput(1).RateOfChangeMaxMat = RateOfChangeMaxMat;
AdditionalOutput(1).RateOfChangeMinMat = RateOfChangeMinMat;
AdditionalOutput(1).ExtremeLocs = ExtremeLocs;
AdditionalOutput(1).ExtremeVals = ExtremeVals;
AdditionalOutput(1).Rate = Rate;
AdditionalOutput(1).RateOfChange = RateOfChange;

end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters,fs)

% Upper threshold for duration-based detector
if isfield(Parameters,'DurationMax')
    % Use specified DurationMax, upper threshold
    DurationMax = str2double(Parameters.DurationMax);
    
    if isnan(DurationMax)
        % str2double sets the value to NaN when it can't make it a number
        warning('DurationMax is not a number and will be ignored in duration-based out-of-range event detector.');
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
        warning('DurationMin is not a number and will be ignored in duration-based out-of-range event detector.');
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
        warning('AnalysisWindow is not a number. Duration based out-of-range event detector will not be implemented.');
    end
elseif (~isnan(DurationMin)) || (~isnan(DurationMax))
    warning('Analysis window length was not specified. The duration based out-of-range event detector will not be implemented.');
    AnalysisWindow = NaN;
else
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
        warning('RateOfChange is not a number. Rate-of-change based out-of-range detector will not be implemented.');
    end
else
    % RateOfChange threshold is not considered 
    RateOfChange = NaN;
end

% Window for combining events detected with the rate-of-change based
% detector. Rate of change events tend to happen very close together.
% Setting the start time back by EventMergeWindow seconds helps
% to group these events together.
if isfield(Parameters,'EventMergeWindow')
    % Use specified merge window length
    % Multiplication by fs produces units of samples
    EventMergeWindow = str2double(Parameters.EventMergeWindow)*fs;
    
    if isnan(EventMergeWindow)
        % str2double sets the value to NaN when it can't make it a number
        warning('The length of the merge window is not a number. Rate-of-change based out-of-range event detector will not be implemented.');
    end
elseif (~isnan(RateOfChangeMin)) || (~isnan(RateOfChangeMax))
    warning('The length of the merge window was not specified. The rate-of-change based out-of-range event detector will not be implemented.');
    EventMergeWindow = NaN;
else
    EventMergeWindow = NaN;
end


% Default length of averaging window
AverageWindowDefault = 60*fs;
%
if isfield(Parameters,'Nominal')
    % Use specified nominal value
    Nominal = str2double(Parameters.Nominal);
    
    % str2double sets the value to NaN when it can't make it a number.
    % Check to see if the user-input for Nominal was valid.
    if isnan(Nominal)
        % User-specified value was invalid.
        
        % Check if AverageWindow was specified
        if isfield(Parameters,'AverageWindow')
            % Use specified length for the averaging window. Convert to samples
            AverageWindow = str2double(Parameters.AverageWindow)*fs;
            
            % Check if the user-specified value for AverageWindow was valid
            if isnan(AverageWindow)
                % The user-specified value was invalid
                warning('The nominal value and length of the averaging window for an out-of-range detector were not numbers. The default averaging window will be used instead.');
                % Use default: 60 seconds converted to samples
                AverageWindow = AverageWindowDefault;
            else
                warning('The nominal value for an out-of-range detector is not a number. The specified averaging window will be used instead.');
            end
        else
            warning('The nominal value for an out-of-range detector is not a number. The default averaging window will be used instead.');
            % Use default: 60 seconds converted to samples
            AverageWindow = AverageWindowDefault;
        end
    elseif isfield(Parameters,'AverageWindow')
        % The AverageWindow was specified, but it is unneeded because the
        % default value was also specified.
        warning('Nominal value and length of averaging window were specified for an out-of-range detector. The nominal value will be used.');
        AverageWindow = NaN;
    else
        % Just the nominal value was specified and it was valid, so the
        % AverageWindow parameter is unneeded.
        AverageWindow = NaN;
    end
elseif isfield(Parameters,'AverageWindow')
    % Use specified length for the averaging window. Convert to samples
    AverageWindow = str2double(Parameters.AverageWindow)*fs;
    % Nominal value is unneeded
    Nominal = NaN;

    % Check if the user-specified value for AverageWindow was valid
    if isnan(AverageWindow)
        % The user-specified value was invalid
        warning('Averaging window length is not a number, using default value instead.');
        AverageWindow = AverageWindowDefault;
    end
else
    % Neither the nominal value or averaging window length were specified,
    % so use default value.
    AverageWindow = AverageWindowDefault;
    % Nominal value is unneeded
    Nominal = NaN;
end

ExtractedParameters = struct('DurationMax',DurationMax,...
    'DurationMin',DurationMin,'Duration',Duration,...
    'RateOfChangeMax',RateOfChangeMax,'RateOfChangeMin',RateOfChangeMin,...
    'RateOfChange',RateOfChange,'AnalysisWindow',AnalysisWindow,...
    'Nominal',Nominal,'AverageWindow',AverageWindow,'EventMergeWindow',EventMergeWindow);

end