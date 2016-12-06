function EventList = UpdateWindRampEvents(DetectionResults, ~, EventList, ~, ~)

for chan = 1:length(DetectionResults)
    if ~isempty(DetectionResults(chan).TrendStart) && iscell(DetectionResults(chan).TrendStart)
        % The results are not empty (no event detected) nor are they set to
        % NaN (TrendStart is a cell so it cannot be NaN, which would 
        % indicate that the channel was of inappropriate type). Add new 
        % results to the EventList.
        
        % For each event in this channel's detection results
        for idx = 1:length(DetectionResults(chan).TrendStart)
            if isfield(EventList,'TrendStart')
                % Events have already been detected. Update them based on the
                % new detection results
                
                % Get indices of events for the same PMU and channel
                PMUmatch = strcmp(DetectionResults(chan).PMU, {EventList.PMU});
                ChanMatch = strcmp(DetectionResults(chan).Channel, {EventList.Channel});
                MatchIdx = find(PMUmatch & ChanMatch);
                
                if ~isempty(MatchIdx)
                    % There is at least one event for this channel.
                    
                    % Get all event end times for events from the
                    % matching channel
                    EventEnds = {EventList(MatchIdx).TrendEnd};
                    
                    % If the detected event started before a previously
                    % recorded event ended, they are the same event.
                    EventIdx = MatchIdx(datenum(DetectionResults(chan).TrendStart{idx}) < datenum(EventEnds));
                    if isempty(EventIdx)
                        % This is a new event
                        EventList(end+1).PMU = DetectionResults(chan).PMU;
                        EventList(end).Channel = DetectionResults(chan).Channel;
                        EventList(end).TrendStart = DetectionResults(chan).TrendStart{idx};
                        EventList(end).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                        EventList(end).TrendValue = DetectionResults(chan).TrendValue(idx);
                    elseif length(EventIdx) == 1
                        % This is an existing event. Update the event end 
                        % time and trend value.
                        EventList(EventIdx).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                        EventList(EventIdx).TrendValue = DetectionResults(chan).TrendValue(idx);
                    else
                        % It should not be possible to have more than 1
                        % event correspond to this detection result, so
                        % throw an error.
                        error('Multiple events were found that correspond to this detection result, UpdateWIndRampEvents.m needs to be corrected.');
                    end
                else
                    % There isn't an event for this channel yet, so add the
                    % one from these detection results
                    EventList(end+1).PMU = DetectionResults(chan).PMU;
                    EventList(end).Channel = DetectionResults(chan).Channel;
                    EventList(end).TrendStart = DetectionResults(chan).TrendStart{idx};
                    EventList(end).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                    EventList(end).TrendValue = DetectionResults(chan).TrendValue(idx);
                end
                
            else
                % Events have not yet been detected, so the detection results
                % are definitely new events
                EventList(1).PMU = DetectionResults(chan).PMU;
                EventList(1).Channel = DetectionResults(chan).Channel;
                EventList(1).TrendStart = DetectionResults(chan).TrendStart{idx};
                EventList(1).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                EventList(1).TrendValue = DetectionResults(chan).TrendValue(idx);
            end
        end
    end
end