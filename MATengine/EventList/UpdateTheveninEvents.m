% function EventList = EventList = UpdateOutOfRangeEvents(DetectionResults, ~, EventList, ~, ~)
%
% This function updates a list of out-of-range events based on
% new detection results. For example, if an event is detected in two
% adjacent sets of  data, this function judges whether they are the same
% event and lists them as such.
%
% Inputs:
%   DetectionResults = Detection results from the out-of-range and ringdown 
%                      detectors. See OutOfRangeGeneralDetector.m, for 
%                      specifications. 
%   AdditionalOutput - Unused -> allows multiple functions to be called from UpdateEvents.m
%   EventList = A structure array containing the current list of events.
%               Each entry in the array contains a separate event and has
%               the fields:
%                   Start = datenum value for the start of the event
%                   End = datenum value for the end of the event
%                   Channel = cell array of strings specifying channels
%                             involved in the event
%                   PMU = cell array of strings specifying the PMUs
%                         associated with the channels listed in Channel
%   Params - Unused -> allows multiple functions to be called from UpdateEvents.m
%   AlarmParams - Unused -> allows multiple functions to be called from UpdateEvents.m
%   
% Outputs:
%   EventList = updated event list. See EventList in the list of Inputs
%               for specifications.
%
% Created by Jim Follum (james.follum@pnnl.gov) in November, 2016.


function EventList = UpdateTheveninEvents(DetectionResults, ~, EventList, Params, ~)

% Convert to days for use with datenum values
EventMergeWindow = str2num(Params.EventMergeWindow)/60/60/24;

% Separate the detections from all the different channels into groups that
% overlap. The start and end of each of these groups will be compared to
% the start and end points of previously detected events. 
%
% Get the start and end of detections from each channel. Convert from time
% strings to date number. ChannelIdx is the same size
% and indicates which channel the boundary points came from.
NewStarts = [];
NewEnds = [];
SubIdx = [];

if iscell(DetectionResults(1).StartTime)
    for Sidx = 1:length(DetectionResults)
        NewStarts = [NewStarts cellfun(@datenum,DetectionResults(Sidx).StartTime)];
        NewEnds = [NewEnds cellfun(@datenum,DetectionResults(Sidx).EndTime)];
        SubIdx = [SubIdx Sidx*ones(1,length(DetectionResults(Sidx).StartTime))];
    end
end
% 
% NewEdges contains all the starts and ends. 
NewEdges = [NewStarts NewEnds];
SubIdx = [SubIdx SubIdx];
%
% Starts are coded as 1, Ends are coded as -1. This coding is useful for
% identifying groups after sorting.
BoundTypeCode = [ones(size(NewStarts)) -1*ones(size(NewEnds))];
%
% Sort all of the boundary points. Use the sorting index to sort the coded
% boundary type and the index for the channel.
[NewEdges,idx] = sort(NewEdges);
BoundTypeCode = BoundTypeCode(idx);
SubIdx = SubIdx(idx);
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
Group = struct('SubIdx',[],'Start',[],'End',[]);
for GrpIdx = 1:length(startIndex)
    ThisGroupIdx = startIndex(GrpIdx):endIndex(GrpIdx);
    Group(GrpIdx).SubIdx = unique(SubIdx(ThisGroupIdx));
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
        EventIdx = find((Group(GrpIdx).Start - EventMergeWindow) < EventEnds);
        
        if isempty(EventIdx)
            % This is a newly detected event, add it to the list
            EventList(end+1).ID = AssignEventID();
            EventList(end).Start = Group(GrpIdx).Start;
            EventList(end).End = Group(GrpIdx).End;
            EventList(end).Sub = [EventList(end).Sub {DetectionResults(Group(GrpIdx).SubIdx).SubName}];

            % Remove duplicate substation listings
            EventList(end).Sub = unique(EventList(end).Sub);
        else
            % The currently considered event overlaps with a previously
            % listed event.
            
            % EventIdx will normally only contain one value, but it is
            % placed in a for loop for unusual circumstances where it
            % contains multiple values
            for ThisEvent = EventIdx
                EventList(ThisEvent).Start = min([EventList(ThisEvent).Start Group(GrpIdx).Start]);
                EventList(ThisEvent).End = max([EventList(ThisEvent).End Group(GrpIdx).End]);
                EventList(ThisEvent).Sub = [EventList(ThisEvent).Sub {DetectionResults(Group(GrpIdx).SubIdx).SubName}];
                
                % Remove duplicate substation listings
                EventList(ThisEvent).Sub = unique(EventList(ThisEvent).Sub);
            end
        end
        
        % If this event overlapped with more than one previously listed
        % event those events need to be combined.
        if length(EventIdx) > 1
            EventList(EventIdx(1)).Start = min([EventList(EventIdx).Start]);
            EventList(EventIdx(1)).End = max([EventList(EventIdx).End]);
            EventList(EventIdx(1)).Sub = EventList(EventIdx).Sub;
            
            % Remove duplicate substation listings
            EventList(EventIdx(1)).Sub = unique(EventList(EventIdx(1)).Sub);
            
            % Remove all events in EventIdx except EventIdx(1)
            EventList(EventIdx(2:end)) = [];
        end
    else
        % Events have not yet been detected, so those in Group definitely
        % need to be added
        EventList(1).ID = AssignEventID();
        EventList(1).Start = Group(GrpIdx).Start;
        EventList(1).End = Group(GrpIdx).End;
        EventList(1).Sub = {DetectionResults(Group(GrpIdx).SubIdx).SubName};
        
        % Remove duplicate substation listings
        EventList(1).Sub = unique(EventList(1).Sub);
    end
end