function EventList = UpdateWindAppOverEvents(EventList,PathEventXML,EventType)

% Iterate through each event, then through the days that the event was
% active. Read the XML associated with the day into a struct, then Update
% the struct with the event info. Save the struct as an XML, replacing the
% original.
for EventIdx = 1:length(EventList.(EventType))
    st = datetime(datevec(EventList.(EventType)(EventIdx).Start));
    en = datetime(datevec(EventList.(EventType)(EventIdx).End));
    for t = unique(cellstr(datestr([(st:1:en) en],'yymmdd'))).'
        EventXMLfileName = [PathEventXML '\EventList_' t{1} '.XML'];
        UpdateEventXML(EventList,EventXMLfileName,EventType,EventType,EventIdx);
    end
end

% The wind application only considers events that are over, so all of them
% can be removed.
EventList.(EventType) = [];