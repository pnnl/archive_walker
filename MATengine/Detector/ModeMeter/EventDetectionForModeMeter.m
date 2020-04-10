function AdditionalOutput = EventDetectionForModeMeter(y,RMS,Parameters,ResultUpdateInterval,fs,PastAdditionalOutput,ExtraOutput)

%% Store detector parameters in variables for easier access
Threshold = Parameters.Threshold;
RingdownID = Parameters.RingdownID;

% Analysis length
N1 = size(RMS,1);

%% RMS energy based detector

ResultUpdateInterval = ResultUpdateInterval*fs;

AdditionalOutput = struct('GrpDelay',[],'EventIndicator',[],'win',[]);

if isempty(PastAdditionalOutput)
    [bBP,bLP] = DesignRMSfilters('Band 2',fs);
    GrpDelay = round((length(bBP)-1)/2 + (length(bLP)-1)/2);
    AdditionalOutput.GrpDelay = GrpDelay;
    
    RMSpast = [];
    RMSpresent = RMS;
else
    % Initial conditions are available
    GrpDelay = PastAdditionalOutput.GrpDelay;
    AdditionalOutput.GrpDelay = GrpDelay;
    % Only process the newly available measurements
    RMSpast = RMS(1:end-ResultUpdateInterval);
    RMSpresent = RMS(end-ResultUpdateInterval+1:end);
end

% Perform detection by comparing RMS to Threshold
DetLgc = RMSpresent > Threshold;




% If event is too long, don't perform the check. We want to be very
    % targeted with the ringdown check. We're trying to distinguish between
    % a true ringdown and nonlinear measurements following a disturbance.
    % No other excursions should be removed.
% If an event is timed such that not enough data is available to compare
    % against the step response, remove the event. This will happen for
    % events taking place at the end of the analysis window. 
% If an event is at the end of the window:
    % If it is longer than Emax, do not remove it
    % If it is shorter than Emax:
        % If it is long enough to perform the check, do so
        % If it is too short to perform the check, window it out
% If an event is at the beginning of the window:
    % Compare RMSpast to the Threshold and add the event to ThisDetLgc
    % The past can't be corrected, but it can inform what to do with the current data
    % If the event is longer than Emax, do not window out the portion in the current data
    % If it is shorter than Emax:
        % If it is long enough to perform the check, do so
        % If it is too short to perform the check, window it out


% Perform check to distinguish good ringdowns from unreliable measurements
% right after a disturbance
% Only perform check if the user requested (specified by RingdownID flag)
% and if events were detected
if RingdownID && (sum(DetLgc) > 0)
    % Maximum length for an event to be considered - make sure that Emax >
    % RMSlength, otherwise you will consider essentially zero events
    Emax = 20*fs;
    % Length of simulations used to evaluate the events
    Nsim = 10*fs;
    
    % Number of different mode meter algorithms applied to this channel of data. The
    % determination about the event is made separately for each algorithm
    % because each algorithm produces a different system model.
    NumMethods = length(ExtraOutput);
    % For each of the mode meter algorithms
    for MethodIdx = 1:NumMethods
        % Detection vector for this method
        ThisDetLgc = DetLgc;
        
        % Behavior is different depending on whether this is the first time
        % the code has been run - checked by evaluating if RMSpast is empty
        if isempty(RMSpast)
            % This is the first time the code has been run. The mode meter has
            % not been run, so all events shorter than Emax must be
            % windowed out. To prevent longer events from being windowed
            % out, set ThisDetLgc to false.
            ThisDetLgc = NoModelCase(ThisDetLgc,Emax,RMSpast,Threshold);
        else
            % Only perform the check if the ExtraOutput structure indicates
            % that the mode meter algorithm produces the necessary ARMA
            % model. If the check can't be completed, window out any event
            % shorter than Emax.
            if isfield(ExtraOutput{MethodIdx},'a') && isfield(ExtraOutput{MethodIdx},'b')
                % The ARMA model is available
                % Events shorter than Emax are windowed out. Events for 
                % which Nsim samples are not available are windowed out.
                % Events with at least Nsim samples and no more than Emax
                % samples are evaluated. ThisDetLgc is set to false for 
                % events identified as ringdowns
                ThisDetLgc = ModelCase(ThisDetLgc,Emax,Nsim,RMSpast,Threshold,ResultUpdateInterval,y,GrpDelay,ExtraOutput{MethodIdx}.a,ExtraOutput{MethodIdx}.b,fs);
            else
                % An ARMA model is not available, either because the method
                % does not create one or because of a problem, such as data
                % quality.
                % All events shorter than Emax should be windowed out. To
                % prevent longer events from being windowed out, set 
                % ThisDetLgc to false.
                ThisDetLgc = NoModelCase(ThisDetLgc,Emax,RMSpast,Threshold);
            end
        end
        
        % Use ThisDetLgc to update the window
        [win, EventIndicator] = UpdateWindow(y,ThisDetLgc,Parameters,N1,ResultUpdateInterval,GrpDelay,PastAdditionalOutput,MethodIdx);
        AdditionalOutput.win{MethodIdx} = win;
        AdditionalOutput.EventIndicator{MethodIdx} = EventIndicator;
    end
else
    % Number of different mode meter algorithms applied to this channel of data. The
    % determination about the event is made separately for each algorithm
    % because each algorithm produces a different system model.
    NumMethods = length(ExtraOutput);
    % For each of the mode meter algorithms
    for MethodIdx = 1:NumMethods
        % Use ThisDetLgc to update the window
        [win, EventIndicator] = UpdateWindow(y,DetLgc,Parameters,N1,ResultUpdateInterval,GrpDelay,PastAdditionalOutput,MethodIdx);
        AdditionalOutput.win{MethodIdx} = win;
        AdditionalOutput.EventIndicator{MethodIdx} = EventIndicator;
    end
end

end

%% Embedded functions

%% NoModelCase
function ThisDetLgc = NoModelCase(ThisDetLgc,Emax,RMSpast,Threshold)

% If the first sample is part of an event, add previous values 
if ThisDetLgc(1)
    % Retrieve previous values
    PastDetLgc = RMSpast > Threshold;
    % Find the last non-event sample
    LastNonEidx = find(~PastDetLgc,1,'last');
    % Number of previous samples containing the event.
    % These will be added to DetLgc
    if ~isempty(LastNonEidx)
        Nadd = length(RMSpast)-LastNonEidx;
        ThisDetLgc = [ones(Nadd,1); ThisDetLgc];
    else
        Nadd = 0;
    end
else
    Nadd = 0;
end

% Calculate the duration of each event
DetIdx = find(ThisDetLgc);
StIdx = [1; find(diff(DetIdx) > 1)+1];
EnIdx = [find(diff(DetIdx) > 1); length(DetIdx)];
Dur = EnIdx-StIdx+1;

% Retain only events longer than Emax
StIdx = StIdx(Dur > Emax);
EnIdx = EnIdx(Dur > Emax);

% Turn off the detection flag for events longer than Emax
for e = 1:length(StIdx)
    ThisDetLgc(DetIdx(StIdx(e)):DetIdx(EnIdx(e))) = 0;
end

% Remove extra samples
ThisDetLgc = ThisDetLgc(Nadd+1:end);

end


%% ModelCase
function ThisDetLgc = ModelCase(ThisDetLgc,Emax,Nsim,RMSpast,Threshold,ResultUpdateInterval,y,GrpDelay,a,b,fs)

% If the first sample is part of an event, add previous values 
if ThisDetLgc(1)
    % Retrieve previous values
    PastDetLgc = RMSpast > Threshold;
    % Find the last non-event sample
    LastNonEidx = find(~PastDetLgc,1,'last');
    % Number of previous samples containing the event.
    % These will be added to DetLgc
    if ~isempty(LastNonEidx)
        Nadd = length(RMSpast)-LastNonEidx;
        ThisDetLgc = [ones(Nadd,1); ThisDetLgc];
    else
        Nadd = 0;
    end
else
    Nadd = 0;
end

% Calculate the duration of each event
DetIdx = find(ThisDetLgc);
StIdx = [1; find(diff(DetIdx) > 1)+1];
EnIdx = [find(diff(DetIdx) > 1); length(DetIdx)];
Dur = EnIdx-StIdx+1;

% Start and end of events longer than Emax (will not be
% windowed)
StIdxTooLong = StIdx(Dur > Emax);
EnIdxTooLong = EnIdx(Dur > Emax);
% Turn off the detection flag for events longer than Emax
for e = 1:length(StIdxTooLong)
    ThisDetLgc(DetIdx(StIdxTooLong(e)):DetIdx(EnIdxTooLong(e))) = 0;
end

% Start and end of events shorter than Emax - will be evaluated
StIdxEval = StIdx(Dur <= Emax);
EnIdxEval = EnIdx(Dur <= Emax);

for e = 1:length(StIdxEval)
    eIdx1 = DetIdx(StIdxEval(e)):DetIdx(EnIdxEval(e));
    eIdx2 = eIdx1 - Nadd + (length(y) - ResultUpdateInterval);
    [IsRingdown,RingKeepIdx] = CheckIfRingdown(y,eIdx2,a,b,Nsim,fs,GrpDelay);
    if IsRingdown
        % This is a ringdown, so set ThisDetLgc to false
        ThisDetLgc(eIdx1(RingKeepIdx)) = 0;
    end
end

% Remove extra samples
ThisDetLgc = ThisDetLgc(Nadd+1:end);

end


%% CheckIfRingdown
function [IsRingdown,RingKeepIdx] = CheckIfRingdown(y,eIdx,a,b,Nsim,fs,GrpDelay)

% You don't need to worry about whether the event took place in the past or
% not. Get the event (no matter where it is), evaluate it, and return the
% result. The ModelCase (and NoModelCase) functions only deal with indices
% corresponding to the RMS signal. The delay between the mode meter signal
% and the RMS is addressed in UpdateWindow. 

% Remember that eIdx(1) is important for finding the event in y, but 
% length(eIdx) is not. In this function the length of the event in the RMS
% doesn't matter. Use Nsim samples for the comparison no matter what.

% If Nsim samples are not available, then IsRingdown = false so that the
% event is removed. The rest of the event may later be determined to be a
% good ringdown, but changes to the window prior to the current
% ResultUpdateInterval samples are impossible. 
if eIdx(1) - GrpDelay + Nsim-1 > length(y)
    IsRingdown = false;
    RingKeepIdx = [];
    return
end

% Retrieve step response based on the a and b coefficients
% The try-catch is used in case there's a problem with a or b
try
    h = stepz(b,a,Nsim,fs);
catch
    IsRingdown = false;
    RingKeepIdx = [];
    return
end

% Retrieve transient
transient = y(eIdx(1) - GrpDelay + (0:Nsim-1));

% Scale
h = h/max(abs(h))*max(abs(transient));

% Calculate dynamic time warping distance
d = dtw(h,transient);

if d < 0.45
    IsRingdown = true;

    % Even if it is a ringdown, the initial part of the disturbance
    % should be removed. Find the maximum and minimum in the signal.
    % Then find the zero crossing following the later of the two.
    yy = y(eIdx-GrpDelay);
    [yyMax,MaxIdx] = max(yy);
    [yyMin,MinIdx] = min(yy);
    if (yyMin < 0) && (0 < yyMax)
        if MinIdx < MaxIdx
            % Find the first negative value following the maximum, which
            % followed the minimum
            zIdx = find(yy(MaxIdx:end) < 0,1) + MaxIdx-1;
        else % MaxIdx < MinIdx
            % Find the first positive value following the minimum, which
            % followed the maximum
            zIdx = find(yy(MinIdx:end) > 0,1) + MinIdx-1;
        end
    else
        zIdx = [];
    end

    RingKeepIdx = zIdx:length(yy);
else
    IsRingdown = false;
    RingKeepIdx = [];
end

end


%% UpdateWindow
function [win, EventIndicator] = UpdateWindow(y,DetLgc,Parameters,N1,ResultUpdateInterval,GrpDelay,PastAdditionalOutput,MethodIdx)

lam1 = Parameters.lam1;
lam2 = Parameters.lam2;
N2 = Parameters.N2;
PostEventWinAdj = Parameters.PostEventWinAdj;

if isempty(PastAdditionalOutput)
    % PastAdditionalOutput isn't available
    
    % Initialize the event indicator 
    EventIndicator = zeros(1,N1);
    
    % Initialize the window
    % Initialize the window with zeros during events
    win = lam1.^(N1-1:-1:0)';
    
    DetIdx = find(DetLgc);
    
    % If an event was detected, get the initial EventIndicator setup, then
    % initialize the window
    if ~isempty(DetIdx)
        DetDiff = find(diff(DetIdx)>1);
        
        % Retrieve indices of the start and end of each event. 
        GrpSt = DetIdx([1; DetDiff+1]);
        GrpEn = [DetIdx(DetDiff); DetIdx(end)];
        
        % Shift the event forward by GrpDelay samples to account for the
        % filter delays introduced when calculating the RMS
        GrpSt = GrpSt - GrpDelay;
        GrpEn = GrpEn - GrpDelay;
        
        % Make sure that the start and end are within the window.
        GrpSt(GrpSt < 1) = 1;
        GrpEn(GrpEn < 1) = 1;
        
        % For each detected event, mark the event
        for Eidx = 1:length(GrpSt)
            EventIndicator(GrpSt(Eidx):GrpEn(Eidx)) = 1;
        end
        
        % Remove the event from the window
        win(EventIndicator ~= 0) = 0;
    end
    
    return
end

EventIndicator = PastAdditionalOutput.EventIndicator{MethodIdx};

% Adjust the indices of samples with a detection based on the group delay
% of the filters used to calculate the RMS
DetLgcAdj = false(size(DetLgc));
% DetLgc is just for the most recent ResultUpdateInterval samples. Create
% an index that corresponds to the EventIndicator vector, which corresponds
% to the full analysis window of length N1. Simultaneously, adjust for the
% group delay.
DetIdxEI = find(DetLgc) + N1 - GrpDelay;
% In many cases, DetIdxEI will contain indices beyond N1 because DetLgc
% corresponds to the ResultUpdtateInterval samples following
% EventIndicator. Overlap is caused by the group delay. Remove indices
% beyond N1 (the window corresponding to EventIndicator).
DetIdxEI(DetIdxEI > N1) = [];
% Set EventIndicator to 1 at DetIdxEI. This is detecting an event in
% samples that are already passed. 
EventIndicator(DetIdxEI) = 1;
% Adjust DetLgc to account for the group delay of the filters used to
% calculate the RMS.
DetIdx = find(DetLgc) - GrpDelay;
DetIdx(DetIdx<1) = [];  % These can be dropped off because EventIndicator was adjusted above
DetLgcAdj(DetIdx) = true;
DetLgc = DetLgcAdj;

win = PastAdditionalOutput.win{MethodIdx};
for k = 1:ResultUpdateInterval
    if DetLgc(k)
        % Newest sample is part of an event
        % Shift the event tracker forward and mark the event
        EventIndicator = [EventIndicator(2:end) 1];
    else
        % Newest sample is NOT part of an event
        % Shift the event tracker forward and mark the non-event
        EventIndicator = [EventIndicator(2:end) 0];
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
        win = [lam*win(2:end); 0];

        % Because an event can be detected in past samples, make sure
        % all event samples are set to zero in the window
        win(EventIndicator ~= 0) = 0;
    else
        % New sample is not part of an event

        % Update window
        win = [lam*win(2:end); 1];
        
        % Because an event can be detected in past samples, make sure
        % all event samples are set to zero in the window
        win(EventIndicator ~= 0) = 0;

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

% Due to the filter delay when calculating the RMS, the most recent GrpDelay
% samples of y can't be effectively evaluated with RMS. 
% After removing samples as specified by the window, if the maximum value
% of y occurs within the final GrpDelay samples, remove all the samples.
% This will prevent events that just occurred from biasing the mode meter.
% Downside: In this implementation the window will be permanently set to
% zero for these samples, even if they don't trigger the RMS-energy
% detector.
y(win == 0) = 0;
[~,MaxIdx] = max(y);
if MaxIdx >= length(y)-GrpDelay
    win(end-GrpDelay:end) = 0;
end

end