% function EventList = UpdateWindRampEvents(DetectionResults, ~, EventList, ~, ~)
%
% This function updates a list of wind ramp events based on new detection
% results. For example, if a wind ramp is detected in two adjacent sets of
% data, this function judges whether they constitute a single event and
% lists them as such. Channels are considered separately, so overlap of
% events on different channels is ignored.
%
% Inputs:
%   DetectionResults = Detection results from the wind ramp detector. See
%                      WindRampDetector.m for specifications.
%   AdditionalOutput - Unused -> allows multiple functions to be called from UpdateEvents.m
%   EventList = A structure array containing the current list of events.
%               Each entry in the array contains a separate event. Fields are:
%                   PMU = a string specifying the PMU associated with the channel
%                   Channel = a string specifying the channel name
%                   TrendStart = the datenum value corresponding to the
%                                trend's start
%                   TrendEnd = the datenum value corresponding to the
%                              trend's end
%                   TrendValue = the trend's value (change during the
%                                trend)
%   Params - Unused -> allows multiple functions to be called from UpdateEvents.m
%   AlarmParams - Unused -> allows multiple functions to be called from UpdateEvents.m
%   
% Outputs:
%   EventList = updated event list. See EventList in the list of Inputs
%               for specifications.
%
% Created by Jim Follum (james.follum@pnnl.gov) in November, 2016.

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
                        EventList(end+1).ID = AssignEventID();
                        EventList(end).PMU = DetectionResults(chan).PMU;
                        EventList(end).Channel = DetectionResults(chan).Channel;
                        EventList(end).TrendStart = DetectionResults(chan).TrendStart{idx};
                        EventList(end).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                        EventList(end).TrendValue = DetectionResults(chan).TrendValue(idx);
                        EventList(end).ValueStart = DetectionResults(chan).ValueStart(idx);
                        EventList(end).ValueEnd = DetectionResults(chan).ValueEnd(idx);
                    elseif length(EventIdx) == 1
                        % This is an existing event. Update the event end 
                        % time and trend value.
                        EventList(EventIdx).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                        EventList(EventIdx).TrendValue = DetectionResults(chan).TrendValue(idx);
                        EventList(EventIdx).ValueEnd = DetectionResults(chan).ValueEnd(idx);
                    else
                        % It should not be possible to have more than 1
                        % event correspond to this detection result, so
                        % throw an error.
                        error('Multiple events were found that correspond to this detection result, UpdateWIndRampEvents.m needs to be corrected.');
                    end
                else
                    % There isn't an event for this channel yet, so add the
                    % one from these detection results
                    EventList(end+1).ID = AssignEventID();
                    EventList(end).PMU = DetectionResults(chan).PMU;
                    EventList(end).Channel = DetectionResults(chan).Channel;
                    EventList(end).TrendStart = DetectionResults(chan).TrendStart{idx};
                    EventList(end).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                    EventList(end).TrendValue = DetectionResults(chan).TrendValue(idx);
                    EventList(end).ValueStart = DetectionResults(chan).ValueStart(idx);
                    EventList(end).ValueEnd = DetectionResults(chan).ValueEnd(idx);
                end
                
            else
                % Events have not yet been detected, so the detection results
                % are definitely new events
                EventList(1).ID = AssignEventID();
                EventList(1).PMU = DetectionResults(chan).PMU;
                EventList(1).Channel = DetectionResults(chan).Channel;
                EventList(1).TrendStart = DetectionResults(chan).TrendStart{idx};
                EventList(1).TrendEnd = DetectionResults(chan).TrendEnd{idx};
                EventList(1).TrendValue = DetectionResults(chan).TrendValue(idx);
                EventList(1).ValueStart = DetectionResults(chan).ValueStart(idx);
                EventList(1).ValueEnd = DetectionResults(chan).ValueEnd(idx);
            end
        end
    end
end