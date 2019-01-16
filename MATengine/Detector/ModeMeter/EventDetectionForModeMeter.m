function AdditionalOutput = EventDetectionForModeMeter(Data,Parameters,ResultUpdateInterval,fs,PastAdditionalOutput)

%% Store detector parameters in variables for easier access
RMSlength = Parameters.RMSlength;
RMSmedianFilterOrder = Parameters.RMSmedianFilterOrder;
RingThresholdScale = Parameters.RingThresholdScale;

% P is the sum of the window across time
%
% Pmax corresponds to w(t)=1 for all t
Pmax = length(Data);
% Pmin is specified by the user based on the amount of data needed to get
% an estimate with acceptable variance
Pmin = Parameters.MinAnalysisLength;

deltaP = Pmax/Pmin-1;

%% RMS energy based detector

AdditionalOutput = struct('threshold',[],'RMS',[],'RMShist',[],'FilterConditions',[]);

Data2 = Data.^2;

if isempty(PastAdditionalOutput)
    % PastAdditionalOutput isn't available
    % Get initial conditions by filtering data with a constant 
    % value equal to the first sample of Data.
    [~, InitConditions] = filter(ones(1,RMSlength)/RMSlength,1,Data2(1)*ones(ceil(RMSlength/2),1));
else
    % Initial conditions are available
    InitConditions = PastAdditionalOutput.FilterConditions;
    % Only process the newly available measurements
    Data2 = Data2(end-ResultUpdateInterval*fs+1:end);
end

[RMS, AdditionalOutput.FilterConditions] = filter(ones(1,RMSlength)/RMSlength,1,Data2,InitConditions);
RMS = sqrt(RMS);


if isempty(PastAdditionalOutput)
    % PastAdditionalOutput isn't available

    % Apply median filter to RMS to establish the threshold
    RMSmed = medfilt1(RMS,RMSmedianFilterOrder,'truncate');

    % Use NaNs to pad the front end of the history. This makes the
    % history longer than necessary. Additional samples are removed
    % in the next line of code.
    AdditionalOutput.RMShist = [NaN(RMSmedianFilterOrder,1); RMS];

    % Keep only the most recent RMSmedianFilterOrder-1 values
    AdditionalOutput.RMShist = AdditionalOutput.RMShist(max([1 (length(AdditionalOutput.RMShist)-(RMSmedianFilterOrder-1)+1)]):end);
    
    % Calculate the threshold
    Threshold = RMSmed*RingThresholdScale;
    
    DetectionIdx = RMS > Threshold;
else
    % PastAdditionalOutput is available

    % Add previous RMS values to the front end to make the median
    % filter continuous
    RMSmed = medfilt1([PastAdditionalOutput.RMShist; RMS],RMSmedianFilterOrder);
    % Remove extra samples
    RMSmed = RMSmed((RMSmedianFilterOrder+1)/2:end-(RMSmedianFilterOrder-1)/2);

    % Replace the RMS values in PastAdditionalOutput for next time
    AdditionalOutput.RMShist = [PastAdditionalOutput.RMShist; RMS];
    % Keep only the most recent RMSmedianFilterOrder-1 values
    AdditionalOutput.RMShist = AdditionalOutput.RMShist(max([1 (length(AdditionalOutput.RMShist)-(RMSmedianFilterOrder-1)+1)]):end);
    
    Threshold = RMSmed*RingThresholdScale;
    DetectionIdx = RMS > Threshold;
    
    
    
%     % Add previous test statistic values to the front end
%     TestStat = [PastAdditionalOutput.TestStat; RMS];
%     % Remove extra samples
%     TestStat = TestStat(end-length(Data)+1:end);
    
%     % Calculate the threshold
%     Threshold = RMSmed*RingThresholdScale;
%     % Add previous threshold to the front end
%     Threshold = [PastAdditionalOutput.Threshold; Threshold];
%     % Remove extra samples
%     Threshold = Threshold(end-length(Data)+1:end);
end
% % Store in AdditionalOutput for use next time
% AdditionalOutput.TestStat = TestStat;
% AdditionalOutput.Threshold = Threshold;

% DetectionIdx = TestStat > Threshold;

% Adjust the beginning of detections to account for the filter delay
if sum(DetectionIdx) > 0
    % Ringdown was detected

    % Start and end of ringdown (later adjusted for filter delay)
    Starts = find(diff([0; DetectionIdx]) == 1);
    Ends = find(diff(DetectionIdx) == -1);

    if length(Starts) > length(Ends)
        Ends = [Ends; length(DetectionIdx)];
    end
    
    [~,valLocs] = findpeaks(-RMS);
    
    BelowMedIdx = find(RMS < (Threshold/RingThresholdScale));
    
    for RingIdx = 1:length(Starts)
        % Find the first 
        PrevVal = valLocs(find(Starts(RingIdx) > valLocs,1,'last'));
        PrevMed = BelowMedIdx(find(Starts(RingIdx) > BelowMedIdx,1,'last'));
        NewStart = min([PrevVal PrevMed]);
        if isempty(NewStart)
            Starts(RingIdx) = 1;
        else
            Starts(RingIdx) = NewStart;
        end
        
        DetectionIdx(Starts(RingIdx):Ends(RingIdx)) = true;
    end
end

if isempty(PastAdditionalOutput)
    % Initialize the window with zeros during events
    win = ones(size(Data));
    win(DetectionIdx) = 0;
else
    win = PastAdditionalOutput.win;
    for nn = 1:(ResultUpdateInterval*fs)
        if DetectionIdx(nn)
            % New sample is part of an event
            win = [win(2:end); 0];
            
            % No further adjustments to window necessary
        else
            % New sample is not part of an event
            
            % This is the amount gained by dropping off the last sample in
            % the window
            wGain = 1-win(1);
            % Add a weight of 1 for the newest sample because it's not an
            % event
            win = [win(2:end); 1];

            P = sum(win);

            % Find the last zero in the window
            Zidx = find(win==0,1,'last');

            % If an event is present in the analysis window, reduce P by deltaP.
            % This is accomplished by reducing weights on all non-event data
            % before the most recent event.
            % Only continue if there is an event in the analysis window AND if
            % decreasing P by deltaP will not violate the Pmin constraint
            if (~isempty(Zidx)) && (P-deltaP > Pmin)
                % Indices of non-event weights
                NEidx = win(1:Zidx) > 0;
                % Number of weights before the most recent non-event data
                Nw = sum(NEidx);
                % Reduce the weights by (deltaP+wGain)/Nw, for a total 
                % reduction in P of deltaP
                win(NEidx) = win(NEidx) - (deltaP+wGain)/Nw;

                % If any weights fell below zero, set them equal to zero. This
                % will reduce the impact on P, but this situation shouldn't
                % occur.
                win(win<0) = 0;
            end
        end
    end
end
AdditionalOutput.win = win;