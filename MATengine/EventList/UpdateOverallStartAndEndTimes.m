function UpdateOverallStartAndEndTimes(EventList,PathEventXML,EventType)

% OverallStart and OverallEnd times for each event
OS = zeros(1,length(EventList.(EventType)));
OE = zeros(1,length(EventList.(EventType)));
ID = cell(1,length(EventList.(EventType)));
% For each forced oscillation event in the current list
for EventIdx = 1:length(EventList.(EventType))
    OS(EventIdx) = EventList.(EventType)(EventIdx).OverallStart;
    OE(EventIdx) = EventList.(EventType)(EventIdx).OverallEnd;
    ID{EventIdx} = EventList.(EventType)(EventIdx).ID;
end

% For each day during which an event was active
for t = floor(min(OS)):floor(max(OE))
    EventXMLfileName = [PathEventXML '\EventList_' datestr(t,'yymmdd') '.XML'];
    
    % Find events that were active during this day
    EventIdx = find(~((OE < t) | (OS >= t+1)));

    % Existing files are updated. If the file hasn't been written yet,
    % the OverallStart and OverallEnd times will be correct when it is
    % written for the first time by UpdateForcedOscillationOverEvents.
    %
    % Also, there must be an event that was active during this day
    % (EventIdx cannot be empty). Otherwise, there will be nothing to
    % update.
    if (exist(EventXMLfileName,'file') == 2) && (~isempty(EventIdx))
        % The Event XML already exists for this day - update it

        % Read the XML
        EventXML = fun_xmlread_comments(EventXMLfileName);
        % Convert to the EventList format used in Archive Walker
        EventXML = EventListXML2MAT(EventXML);
        
        % Existing entries are updated. If the file doesn't have an
        % entry for Forced Oscillations yet, the OverallStart and
        % OverallEnd times will be correct when the entry for this
        % event is written for the first time by
        % UpdateForcedOscillationOverEvents.
        if isfield(EventXML,EventType)
            % For each of the events active during this day
            for ThisEventIdx = EventIdx
                % Find the event that needs to be updated in EventXML
                IDidx = find(strcmp({EventXML.(EventType).ID}, ID{ThisEventIdx}));

                % If more than one record was found, throw a warning
                if length(IDidx) > 1
                    warning('Non-unique event IDs are present');
                end

                if ~isempty(IDidx)
                    % Update the existing event record
                    EventXML.(EventType)(IDidx).OverallStart = OS(ThisEventIdx);
                    EventXML.(EventType)(IDidx).OverallEnd = OE(ThisEventIdx);
                end
            end
            
            % Save the updated list of events that are over to an XML
            WriteEventListXML(EventXML,EventXMLfileName,0);
        end
    end
end