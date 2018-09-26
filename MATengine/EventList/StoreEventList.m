function EventList = StoreEventList(EventPath,EventList,PMU,DetectorXML,AdditionalOutput)

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
            UpdateOverallStartAndEndTimes(EventList,EventPath,EventType);
            EventList = UpdateForcedOscillationOverEvents(EventList,EventPath,EventType,TimeStamp);
        case 'Ringdown'
            EventList = UpdateRingOverEvents(EventList,EventPath,DetectorXML,EventType,TimeStamp);
        case 'OutOfRangeGeneral'
            EventList = UpdateOutOfRangeOverEvents(EventList,EventPath,DetectorXML,EventType,DetectorIndex,TimeStamp);
        case 'WindRamp'
            EventList = UpdateWindRampOverEvents(EventList,EventPath,DetectorXML,EventType,DetectorIndex,TimeStamp,AdditionalOutput);
        case 'WindApp'
            EventList = UpdateWindAppOverEvents(EventList,EventPath,EventType);
        case 'Thevenin'
            EventList = UpdateTheveninOverEvents(EventList,EventPath,DetectorXML,EventType,DetectorIndex,TimeStamp);
        case 'Modemeter'
            EventList = UpdateModeMeterOverEvents(EventList,EventPath,DetectorXML,EventType,DetectorIndex,TimeStamp,AdditionalOutput);
    end
end