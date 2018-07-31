function UpdateEventXML(EventList,EventXMLfileName,EventType,FieldName,EventIdx)

if exist(EventXMLfileName,'file') == 2
    % The Event XML already exists for this day - update it

    % Read the XML
    EventXML = fun_xmlread_comments(EventXMLfileName);
    % Convert to the EventList format used in Archive Walker
    EventXML = EventListXML2MAT(EventXML);
    
    if isfield(EventXML,EventType)
        % Find the event that needs to be updated in EventXML
        IDidx = find(strcmp({EventXML.(EventType).ID}, EventList.(FieldName)(EventIdx).ID));
    else
        % The EventXML struct doesn't yet have a field for this type of
        % event
        EventXML.(EventType) = [];
        IDidx = [];
    end

    % If more than one record was found, throw a warning
    if length(IDidx) > 1
        warning('Non-unique event IDs are present');
    end

    if isempty(IDidx)
        % This event hasn't been added yet, add it to EventXML
        EventXML.(EventType) = [EventXML.(EventType) EventList.(FieldName)(EventIdx)];
    else
        % Update the existing event record
        EventXML.(EventType)(IDidx) = EventList.(FieldName)(EventIdx);
    end

    % Save the updated list of events that are over to an XML
    WriteEventListXML(EventXML,EventXMLfileName,0);
else
    % The Event XML for this day does not yet exist - create it
    OverEventList.(FieldName) = EventList.(FieldName)(EventIdx);
    WriteEventListXML(OverEventList,EventXMLfileName,0);
end