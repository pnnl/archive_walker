function EventList = UpdateRingOverEvents(EventList,PathEventXML,DetectorXML,EventType,TimeStamp)

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
    Hist = str2double(DetectorXML.(EventType).RMSlength)/2;
else
    % Several detectors all contribute to the same list of events, so only
    % use the maximum Hist
    Hist = [];
    for DetectorIndex = 1:NumDetectors
        Hist = [Hist str2double(DetectorXML.(EventType){DetectorIndex}.RMSlength)/2];
    end
    Hist = max(Hist);
end

% Events ending before this time are considered over. Those
% ending after this time may still be active.
EventEndLimit = TimeStamp - Hist/(60*60*24);

% Check each event in the list. Indices of events that are over
% are stored in OverEvents.
NumEvents = length(EventList.(EventType));
OverEvents = [];
for EventIdx = 1:NumEvents
    if EventList.(EventType)(EventIdx).End < EventEndLimit
        OverEvents = [OverEvents EventIdx];
    end
end

% Iterate through each event, then through the days that the event was
% active. Read the XML associated with the day into a struct, then Update
% the struct with the event info. Save the struct as an XML, replacing the
% original.
for EventIdx = OverEvents
    st = datetime(datevec(EventList.(EventType)(EventIdx).Start));
    en = datetime(datevec(EventList.(EventType)(EventIdx).End));
    for t = unique(cellstr(datestr([(st:1:en) en],'yymmdd'))).'
        EventXMLfileName = [PathEventXML '\EventList_' t{1} '.XML'];
        UpdateEventXML(EventList,EventXMLfileName,EventType,EventType,EventIdx);
    end
end

% Remove the events that are over from the current EventList
EventList.(EventType)(OverEvents) = [];
% Clean up so that it is [] rather than a 1x0 struct
if isempty(EventList.(EventType))
    EventList.(EventType) = [];
end