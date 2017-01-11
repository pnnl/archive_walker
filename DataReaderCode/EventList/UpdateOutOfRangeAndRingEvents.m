function EventList = UpdateOutOfRangeAndRingEvents(DetectionResults, ~, EventList, ~, ~)

% Separate the detections from all the different channels into groups that
% overlap. The start and end of each of these groups will be compared to
% the start and end points of previously detected events. 
%
% Get the start and end of detections from each channel. Convert from time
% strings to date number. ChannelIdx is the same size
% and indicates which channel the boundary points came from.
NewStarts = [];
NewEnds = [];
ChannelIdx = [];
% The general detector has an OutStart field, the frequency detector has a
% DurationOutStart field, and the ringdown detector has a RingStart field.
% The if-else statements below determine which detector the
% DetectionResults structure is from. This allows events from all three
% detectors to be listed using this one function.
if isfield(DetectionResults,'OutStart')
    % These detection results are for the general out-of-range detector
    
    % If the OutStart field is NaN (not a cell) it indicates that the detector
    % could not be implemented. No need to update events, just return the
    % current event list. (If the first entry is NaN, they will all be NaN).
    if ~iscell(DetectionResults(1).OutStart)
        return
    end

    for ChanIdx = 1:length(DetectionResults)
        NewStarts = [NewStarts cellfun(@datenum,DetectionResults(ChanIdx).OutStart)];
        NewEnds = [NewEnds cellfun(@datenum,DetectionResults(ChanIdx).OutEnd)];
        ChannelIdx = [ChannelIdx ChanIdx*ones(1,length(DetectionResults(ChanIdx).OutStart))];
    end
elseif isfield(DetectionResults,'DurationOutStart')
    % These detection results are for the frequency out-of-range detector
    
    % If the DurationOutStart and RateOfChangeOutStart fields are NaN (not 
    % cells) it indicates that the detectors could not be implemented. No 
    % need to update events, just return the current event list. (If the 
    % first entry is NaN, they will all be NaN).
    if ~iscell(DetectionResults(1).DurationOutStart) && ~iscell(DetectionResults(1).RateOfChangeOutStart)
        return
    end
    
    if iscell(DetectionResults(1).DurationOutStart)
        for ChanIdx = 1:length(DetectionResults)
            NewStarts = [NewStarts cellfun(@datenum,DetectionResults(ChanIdx).DurationOutStart)];
            NewEnds = [NewEnds cellfun(@datenum,DetectionResults(ChanIdx).DurationOutEnd)];
            ChannelIdx = [ChannelIdx ChanIdx*ones(1,length(DetectionResults(ChanIdx).DurationOutStart))];
        end
    end
    
    if iscell(DetectionResults(1).RateOfChangeOutStart)
        for ChanIdx = 1:length(DetectionResults)
            NewStarts = [NewStarts cellfun(@datenum,DetectionResults(ChanIdx).RateOfChangeOutStart)];
            NewEnds = [NewEnds cellfun(@datenum,DetectionResults(ChanIdx).RateOfChangeOutEnd)];
            ChannelIdx = [ChannelIdx ChanIdx*ones(1,length(DetectionResults(ChanIdx).RateOfChangeOutStart))];
        end
    end
elseif isfield(DetectionResults,'RingStart')
    % These detection results are for the ringdown detector
    
    for ChanIdx = 1:length(DetectionResults)
        % RingStart is set to NaN (not a cell) when the channel is 
        % discarded. Ignore these channels.
        if iscell(DetectionResults(ChanIdx).RingStart)
            NewStarts = [NewStarts cellfun(@datenum,DetectionResults(ChanIdx).RingStart)];
            NewEnds = [NewEnds cellfun(@datenum,DetectionResults(ChanIdx).RingEnd)];
            ChannelIdx = [ChannelIdx ChanIdx*ones(1,length(DetectionResults(ChanIdx).RingStart))];
        end
    end
else
    error('The detection results do not contain a necessary field.');
end
% 
% NewEdges contains all the starts and ends. 
NewEdges = [NewStarts NewEnds];
ChannelIdx = [ChannelIdx ChannelIdx];
%
% Starts are coded as 1, Ends are coded as -1. This coding is useful for
% identifying groups after sorting.
BoundTypeCode = [ones(size(NewStarts)) -1*ones(size(NewEnds))];
%
% Sort all of the boundary points. Use the sorting index to sort the coded
% boundary type and the index for the channel.
[NewEdges,idx] = sort(NewEdges);
BoundTypeCode = BoundTypeCode(idx);
ChannelIdx = ChannelIdx(idx);
%
% BoundaryTypeCode contains 1 for starts and -1 for ends. A cumulative sum
% of 0 indicates the number of starts equals the number of ends and thus
% a group.
endIndex = find(cumsum(BoundTypeCode)==0);
startIndex = [1 (endIndex+1)];
startIndex = startIndex(1:end-1);
%
% For each group, store the index of the channel, the overall start time, and
% the overall end time.
Group = struct('ChannelIdx',[],'Start',[],'End',[]);
for GrpIdx = 1:length(startIndex)
    Group(GrpIdx).ChannelIdx = unique(ChannelIdx(startIndex(GrpIdx):endIndex(GrpIdx)));
    Group(GrpIdx).Start = NewEdges(startIndex(GrpIdx));
    Group(GrpIdx).End = NewEdges(endIndex(GrpIdx));
end

% If no events were detected return the current EventList without any
% changes
if isempty(Group(1).Start)
    return
end
        
% For each group of detections
for GrpIdx = 1:length(Group)
    if isfield(EventList,'Start')
        % Events have already been detected. Update them based on the
        % contents of Group
        
        % Cell arrays containing datenum values for the start and stop 
        % times of all the currently stored events
        EventStarts = [EventList.Start];
        EventEnds = [EventList.End];
        
        % Indices of events in EventList where the the currently
        % considered event overlaps with a previously listed event.
        % In practice, this will almost always contain a maximum of 1
        % event. The code is written to handle multiple entries just in
        % case.
        EventIdx = find(((Group(GrpIdx).Start > EventStarts) & (Group(GrpIdx).Start < EventEnds)) | ...
            ((Group(GrpIdx).End > EventStarts) & (Group(GrpIdx).End < EventEnds)) | ...
            ((Group(GrpIdx).Start < EventStarts) & (Group(GrpIdx).End > EventEnds)));
        
        if isempty(EventIdx)
            % This is a newly detected event, add it to the list
            EventList(end+1).Start = Group(GrpIdx).Start;
            EventList(end).End = Group(GrpIdx).End;
            EventList(end).Channel = [EventList(end).Channel DetectionResults(Group(GrpIdx).ChannelIdx).Channel];
            EventList(end).PMU = [EventList(end).PMU DetectionResults(Group(GrpIdx).ChannelIdx).PMU];
            
            % Remove duplicate channel/PMU listings
            ChannelPMU = strcat(EventList(end).Channel,EventList(end).PMU);
            [~,Uidx] = unique(ChannelPMU);
            EventList(end).Channel = EventList(end).Channel(Uidx);
            EventList(end).PMU = EventList(end).PMU(Uidx);
        else
            % The currently considered event overlaps with a previously
            % listed event.
            
            % EventIdx will normally only contain one value, but it is
            % placed in a for loop for unusual circumstances where it
            % contains multiple values
            for ThisEvent = EventIdx
                EventList(ThisEvent).Start = min([EventList(ThisEvent).Start Group(GrpIdx).Start]);
                EventList(ThisEvent).End = max([EventList(ThisEvent).End Group(GrpIdx).End]);
                EventList(ThisEvent).Channel = [EventList(ThisEvent).Channel DetectionResults(Group(GrpIdx).ChannelIdx).Channel];
                EventList(ThisEvent).PMU = [EventList(ThisEvent).PMU DetectionResults(Group(GrpIdx).ChannelIdx).PMU];

                % Remove duplicate channel/PMU listings
                ChannelPMU = strcat(EventList(ThisEvent).Channel,EventList(ThisEvent).PMU);
                [~,Uidx] = unique(ChannelPMU);
                EventList(ThisEvent).Channel = EventList(ThisEvent).Channel(Uidx);
                EventList(ThisEvent).PMU = EventList(ThisEvent).PMU(Uidx);
            end
        end
        
        % If this event overlapped with more than one previously listed
        % event those events need to be combined.
        if length(EventIdx) > 1
            EventList(EventIdx(1)).Start = min([EventList(EventIdx).Start]);
            EventList(EventIdx(1)).End = max([EventList(EventIdx).End]);
            EventList(EventIdx(1)).Channel = [EventList(EventIdx).Channel];
            EventList(EventIdx(1)).PMU = [EventList(EventIdx).PMU];
            
            % Remove duplicate channel/PMU listings
            ChannelPMU = strcat(EventList(EventIdx(1)).Channel,EventList(EventIdx(1)).PMU);
            [~,Uidx] = unique(ChannelPMU);
            EventList(EventIdx(1)).Channel = EventList(EventIdx(1)).Channel(Uidx);
            EventList(EventIdx(1)).PMU = EventList(EventIdx(1)).PMU(Uidx);
            
            % Remove all events in EventIdx except EventIdx(1)
            EventList(EventIdx(2:end)) = [];
        end
    else
        % Events have not yet been detected, so those in Group definitely
        % need to be added
        EventList(1).Start = Group(GrpIdx).Start;
        EventList(1).End = Group(GrpIdx).End;
        EventList(1).Channel = [DetectionResults(Group(GrpIdx).ChannelIdx).Channel];
        EventList(1).PMU = [DetectionResults(Group(GrpIdx).ChannelIdx).PMU];

        % Remove duplicate channel/PMU listings
        ChannelPMU = strcat(EventList(1).Channel,EventList(1).PMU);
        [~,Uidx] = unique(ChannelPMU);
        EventList(1).Channel = EventList(1).Channel(Uidx);
        EventList(1).PMU = EventList(1).PMU(Uidx);
    end
end