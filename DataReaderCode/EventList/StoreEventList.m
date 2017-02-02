function EventList = StoreEventList(EventList,PMU,DetectorXML,AdditionalOutput)

PathEventXML = DetectorXML.Configuration.EventPath;

% Find the time of the next expected sample
t = PMU(1).Signal_Time.Signal_datenum;
TimeStamp = interp1(1:length(t),t,length(t)+1,'linear','extrap');

% Index through each event type and update the list of events that are
% over.
for EventType = fieldnames(EventList).'
    k = strfind(EventType{1}, '_');
    if ~isempty(k)
        DetectorIndex = str2double(EventType{1}(k+1:end));
        EventType = EventType{1}(1:k-1);
    else
        EventType = EventType{1};
    end
    
    switch EventType
        case 'ForcedOscillation'
            EventList = UpdateForcedOscillationOverEvents(EventList,PathEventXML,EventType,TimeStamp);
        case 'Ringdown'
            EventList = UpdateRingOverEvents(EventList,PathEventXML,DetectorXML,EventType,TimeStamp);
        case 'OutOfRangeGeneral'
            EventList = UpdateOutOfRangeOverEvents(EventList,PathEventXML,DetectorXML,EventType,DetectorIndex,TimeStamp);
        case 'OutOfRangeFrequency'
            EventList = UpdateOutOfRangeOverEvents(EventList,PathEventXML,DetectorXML,EventType,DetectorIndex,TimeStamp);
        case 'WindRamp'
            EventList = UpdateWindRampOverEvents(EventList,PathEventXML,DetectorXML,EventType,DetectorIndex,TimeStamp,AdditionalOutput);
    end
end