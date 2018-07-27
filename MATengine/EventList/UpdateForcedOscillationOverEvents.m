function EventList = UpdateForcedOscillationOverEvents(EventList,PathEventXML,EventType,TimeStamp)
% Go through each event. If OverallEnd is from more than a day before
% TimeStamp, the entire event is over. Otherwise, remove and store all of the
% older occurrences. The list of events for a given period
% contains only the occurrences overlapping with that period.
OverIdx = [];
OngoingIdx = [];
for EventIdx = 1:length(EventList.(EventType))
    if EventList.(EventType)(EventIdx).OverallEnd < TimeStamp-1
        % The last occurrence of this event was more than a day ago, so the
        % entire event is over.
        OverIdx = [OverIdx EventIdx];
    elseif length(EventList.(EventType)(EventIdx).Frequency) > 1
        % This event may be ongoing and, as checked in the elseif
        % statement, it has more than one occurrence. Remove and store all 
        % but the most recent occurrence.
        OngoingIdx = [OngoingIdx EventIdx];
    end
end

% Store events that are over in OverEventList
OverEventList.(EventType) = EventList.(EventType)(OverIdx);

for EventIdx = OngoingIdx
    % Start by adding the entire event
    OverEventList.(EventType) = [OverEventList.(EventType) EventList.(EventType)(EventIdx)];
    
    % For each field with an entry for each occurrence:
    %   Keep all but the final occurrence in OverEventList
    %   Keep only the final occurrence in EventList
    for FN = setdiff(fieldnames(OverEventList.(EventType)(end)),{'ID','OverallStart','OverallEnd'}).'
        OverEventList.(EventType)(end).(FN{1}) = OverEventList.(EventType)(end).(FN{1})(1:end-1);
        EventList.(EventType)(EventIdx).(FN{1}) = EventList.(EventType)(EventIdx).(FN{1})(end);
    end
end

% Remove events that are over from EventList
EventList.(EventType)(OverIdx) = [];

% Clean up so that the outputs are [] rather than 1x0 structs
if isempty(EventList.(EventType))
    EventList.(EventType) = [];
end
if isempty(OverEventList.(EventType))
    OverEventList.(EventType) = [];
end






% Iterate through each event, then through the days that the event was
% active. Read the XML associated with the day into a struct, then Update
% the struct with the event info. Save the struct as an XML, replacing the
% original.
for EventIdx = 1:length(OverEventList.(EventType))
    % Find the earliest start time and the lastest end time for the current
    % set of occurrences. Note that st is not the same as the OverallStart
    % field because the older occurrences may have been removed.
    st = datetime(datevec(min(OverEventList.(EventType)(EventIdx).Start)));
    en = datetime(datevec(max(OverEventList.(EventType)(EventIdx).End)));
    
    for t = unique(cellstr(datestr([(st:1:en) en],'yymmdd'))).'
        EventXMLfileName = [PathEventXML '\EventList_' t{1} '.XML'];
        
        % Find the occurrences associated with this day
        KeepOccurrence = [];
        for OccurrenceIdx = 1:length(OverEventList.(EventType)(EventIdx).Frequency)
            st2 = datetime(datevec(OverEventList.(EventType)(EventIdx).Start(OccurrenceIdx)));
            en2 = datetime(datevec(OverEventList.(EventType)(EventIdx).End(OccurrenceIdx)));
            t2 = unique(cellstr(datestr([(st2:1:en2) en2],'yymmdd')));
            if sum(strcmp(t2,t{1})) > 0
                % This occurrence is associated with the currently
                % evaluated day
                KeepOccurrence = [KeepOccurrence OccurrenceIdx];
            end
        end

        if exist(EventXMLfileName,'file') == 2
            % The Event XML already exists for this day - update it

            % Read the XML
            EventXML = fun_xmlread_comments(EventXMLfileName);
            % Convert to the EventList format used in Archive Walker
            EventXML = EventListXML2MAT(EventXML);

            if isfield(EventXML,EventType)
                % Find the event that needs to be updated in EventXML
                IDidx = find(strcmp({EventXML.(EventType).ID}, OverEventList.(EventType)(EventIdx).ID));
            else
                % The EventXML struct doesn't yet have a field for this type of
                % event, so add it
                EventXML.(EventType) = [];
                IDidx = [];
            end

            % If more than one record was found, throw a warning
            if length(IDidx) > 1
                warning('Non-unique event IDs are present');
            end

            if isempty(IDidx)
                % This event hasn't been added yet, add it to EventXML
                EventXML.(EventType) = [EventXML.(EventType) OverEventList.(EventType)(EventIdx)];
                
                % Keep only the occurrences for this day
                for FN = setdiff(fieldnames(EventXML.(EventType)(end)),{'ID','OverallStart','OverallEnd'}).'
                    EventXML.(EventType)(end).(FN{1}) = EventXML.(EventType)(end).(FN{1})(KeepOccurrence);
                end
            else
                % Update the existing event record
                EventXML.(EventType)(IDidx).OverallStart = OverEventList.(EventType)(EventIdx).OverallStart;
                EventXML.(EventType)(IDidx).OverallEnd = OverEventList.(EventType)(EventIdx).OverallEnd;
                for FN = setdiff(fieldnames(EventXML.(EventType)(IDidx)),{'ID','OverallStart','OverallEnd'}).'
                    EventXML.(EventType)(IDidx).(FN{1}) = [EventXML.(EventType)(IDidx).(FN{1}) OverEventList.(EventType)(EventIdx).(FN{1})(KeepOccurrence)];
                end
            end

            % Save the updated list of events that are over to an XML
            WriteEventListXML(EventXML,EventXMLfileName,0);
        else
            % The Event XML for this day does not yet exist - create it
            OverEventListTemp.(EventType) = OverEventList.(EventType)(EventIdx);
            
            % Keep only the occurrences for this day
            for FN = setdiff(fieldnames(OverEventListTemp.(EventType)(1)),{'ID','OverallStart','OverallEnd'}).'
                OverEventListTemp.(EventType)(1).(FN{1}) = OverEventListTemp.(EventType)(1).(FN{1})(KeepOccurrence);
            end
            
            WriteEventListXML(OverEventListTemp,EventXMLfileName,0);
        end
    end
end