function EventList = UpdateWindRampOverEvents(EventList,PathEventXML,DetectorXML,EventType,DetectorIndex,TimeStamp,AdditionalOutput)

% Hist is the number of seconds of the most recently
% available data that may be found to include an event when the next file
% is analyzed. 
%
% EventEndLimit = TimeStamp - Hist/(60*60*24) is then the threshold for
% events that are over. If a recorded event ended before EventEndLimit it
% is over and future detections cannot be considered to be from the same
% event. 
%
% Example: The start of the next file is TimeStamp = 14:12:00 and the
% detector uses an analysis window of 10 seconds. An event with an end time
% of 14:11:45 is over and won't be associated with future detections. An
% event with an end time of 14:11:55 may or may not be over, so it needs to
% stay in the current EventList. 

% Find the number of separate instances of this detector type.
NumDetectors = length(DetectorXML.(EventType));
% Find Hist, the number of seconds of data from previous files
% that this detector utilizes (new events can be added in this
% time window)
if NumDetectors == 1
    Hist = -(min(AdditionalOutput(1).(EventType).extremaLoc) - AdditionalOutput(1).(EventType).gd);
else
    Hist = -(min(AdditionalOutput(DetectorIndex).(EventType).extremaLoc) - AdditionalOutput(DetectorIndex).(EventType).gd);
end

% Events ending before this time are considered over. Those
% ending after this time may still be active.
EventEndLimit = TimeStamp - Hist/(60*60*24);

% Convert to string for convenience
DetectorIndex = num2str(DetectorIndex);

% Check each event in the list. Indices of events that are over
% are stored in OverEvents.
NumEvents = length(EventList.([EventType '_' DetectorIndex]));
OverEvents = [];
for EventIdx = 1:NumEvents
    if datenum(EventList.([EventType '_' DetectorIndex])(EventIdx).TrendEnd) < EventEndLimit
        OverEvents = [OverEvents EventIdx];
    end
end

% Iterate through each event, then through the days that the event was
% active. Read the XML associated with the day into a struct, then Update
% the struct with the event info. Save the struct as an XML, replacing the
% original.
for EventIdx = OverEvents
    st = datetime(EventList.([EventType '_' DetectorIndex])(EventIdx).TrendStart,'InputFormat','MM/dd/yy HH:mm:ss.SSS');
    en = datetime(EventList.([EventType '_' DetectorIndex])(EventIdx).TrendEnd,'InputFormat','MM/dd/yy HH:mm:ss.SSS');
    for t = unique(cellstr(datestr([(st:1:en) en],'yymmdd'))).'
        EventXMLfileName = [PathEventXML '\EventList_' t{1} '.XML'];
        UpdateEventXML(EventList,EventXMLfileName,EventType,[EventType '_' DetectorIndex],EventIdx);
    end
end

% Remove the events that are over from the current EventList
EventList.([EventType '_' DetectorIndex])(OverEvents) = [];
% Clean up so that it is [] rather than a 1x0 struct
if isempty(EventList.([EventType '_' DetectorIndex]))
    EventList.([EventType '_' DetectorIndex]) = [];
end