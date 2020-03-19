function AdditionalOutput = EventDetectionForModeMeter(RMS,Parameters,ResultUpdateInterval,fs,PastAdditionalOutput)

%% Store detector parameters in variables for easier access
Threshold = Parameters.Threshold;
lam1 = Parameters.lam1;
lam2 = Parameters.lam2;
N2 = Parameters.N2;
PostEventWinAdj = Parameters.PostEventWinAdj;

% Analysis length
N1 = size(RMS,1);

%% RMS energy based detector

ResultUpdateInterval = ResultUpdateInterval*fs;

AdditionalOutput = struct('RMSlength',[],'EventIndicator',[],'win',[],'Eadditional',[]);

if isempty(PastAdditionalOutput)
    [~,bLP] = DesignRMSfilters('Band 2',fs);
    RMSlength = length(bLP);
    AdditionalOutput.RMSlength = RMSlength;
    
    % Initialize the event indicator 
    EventIndicator = zeros(1,N1);
else
    % Initial conditions are available
    RMSlength = PastAdditionalOutput.RMSlength;
    AdditionalOutput.RMSlength = RMSlength;
    % Only process the newly available measurements
    RMS = RMS(end-ResultUpdateInterval+1:end);
    % Retrieve the event indicator from the previous call
    EventIndicator = PastAdditionalOutput.EventIndicator;
end

% Perform detection by comparing RMS to Threshold
DetLgc = RMS > Threshold;

if isempty(PastAdditionalOutput)
    % PastAdditionalOutput isn't available
    
    % Initialize the window
    % Initialize the window with zeros during events
    win = lam1.^(N1-1:-1:0);
    
    DetIdx = find(DetLgc);
    
    % If an event was detected, get the initial EventIndicator setup, then
    % initialize the window
    if ~isempty(DetIdx)
        DetDiff = find(diff(DetIdx)>1);
        
        % Retrieve indices of the start and end of each event. 
        GrpSt = DetIdx([1; DetDiff+1]);
        GrpEn = [DetIdx(DetDiff); DetIdx(end)];
        
        % The start is moved forward by RMSlength samples and the end is moved back
        % RMSlength samples to ensure that the entire event is removed.
        GrpStExtra = GrpSt - RMSlength + 1;
        GrpEnExtra = GrpEn + RMSlength;
        
        % Make sure that the start and end are within the window.
        % Additional samples that should be removed are counted with
        % Eadditional
        Eadditional = max([max(GrpEnExtra)-N1, 0]);
        GrpStExtra(GrpStExtra < 1) = 1;
        GrpEnExtra(GrpEnExtra > N1) = N1;
        
        % For each detected event, mark the event
        for Eidx = 1:length(GrpSt)
            EventIndicator(GrpStExtra(Eidx):GrpEnExtra(Eidx)) = 2;
            EventIndicator(GrpSt(Eidx):GrpEn(Eidx)) = 1;
        end
        
        % Remove the event from the window
        win(EventIndicator ~= 0) = 0;
    else
        Eadditional = 0;
    end
    
    AdditionalOutput.win = win';
    AdditionalOutput.EventIndicator = EventIndicator;
    AdditionalOutput.Eadditional = Eadditional;
    return
end

win = PastAdditionalOutput.win';
Eadditional = PastAdditionalOutput.Eadditional;
for k = 1:ResultUpdateInterval
    if DetLgc(k)
        % Newest sample is part of an event
        % Shift the event tracker forward and mark the event
        EventIndicator = [EventIndicator(2:end) 1];
        
        % If this is the first sample where the event has not been detected,
        % Add the past RMSlength samples to the event to make sure that
        % it is completely removed
        if EventIndicator(end-1) == 0
            EventIndicator(end-RMSlength+1:end-1) = 2;
        end
    else
        % Newest sample is NOT part of an event
        % Shift the event tracker forward and mark the non-event
        EventIndicator = [EventIndicator(2:end) 0];
        
        % If there was an event detected in the previous sample, add an 
        % additional RMSlength samples to the event to make sure
        % that it is completely removed
        if EventIndicator(end-1) == 1
            Eadditional = RMSlength;
        end
        
        if Eadditional > 0
            % Continue removing samples for the detected event
            EventIndicator(end) = 2;
            Eadditional = Eadditional - 1;
        end
    end
    
    if isempty(find(EventIndicator(end-N2+1:end)==1,1))
        % There is NOT an event in the past N2 samples, use lam1
        lam = lam1;
    else
        % There is an event in the past N2 samples, use lam2
        lam = lam2;
    end
    
    if EventIndicator(end) ~= 0
        % New sample is part of an event
        win = [lam*win(2:end) 0];

        % Because an event can be detected in past samples, make sure
        % all event samples are set to zero in the window
        win(EventIndicator ~= 0) = 0;
    else
        % New sample is not part of an event

        % Update window
        win = [lam*win(2:end) 1];

        % Find the last zero in the window
        Zidx = find(win==0,1,'last');

        % If a PreEventKill flag is set to 1 AND there are zeros in the 
        % analysis window:
        % Reduce the window before the most recent event, either by
        % diminishing the value of the window before the event or by
        % shortening the window
        if strcmp(PostEventWinAdj,'DIMINISH') && ~isempty(Zidx)
            % Calculate the change in P that will cause the window before
            % the most recent event to be zero by the time N2 post-event
            % samples have been added to the analysis window
            deltaP = sum(win(1:Zidx))/(N2 - (N1-Zidx));
            
            if deltaP < 0
                deltaP = 0;
            end

            % Indices of non-event weights
            NEidx = win(1:Zidx) > 0;
            % Number of weights before the most recent non-event data
            Nw = sum(NEidx);
            % Reduce the weights by deltaP/Nw, for a total reduction in P of deltaP
            win(NEidx) = win(NEidx) - deltaP/Nw;

            % If any weights fell below zero, set them equal to zero.
            win(win<0) = 0;
        elseif strcmp(PostEventWinAdj,'SHORTEN') && ~isempty(Zidx)
            % Remove the oldest samples at a rate such that they are all
            % zero when N2 post-event samples have been added to the
            % analysis window
            
            if sum(win(1:Zidx) > 0) > 0
                Vidx = find(win(1:Zidx) > 0,1,'last');
                win(1:round((N1-N2)/N2*(N1-Zidx-(Zidx-Vidx)))) = 0;
            end
        end
    end
end
AdditionalOutput.win = win';
AdditionalOutput.EventIndicator = EventIndicator;
AdditionalOutput.Eadditional = Eadditional;