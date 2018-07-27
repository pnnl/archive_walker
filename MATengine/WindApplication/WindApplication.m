function EventList = WindApplication(PMU,WindAppXML,EventList,DetectorXML,FileLength)

% If the wind application isn't configured, return the current EventList
if ~isfield(WindAppXML,'PMU')
    return
end

% Find the time of the next expected sample
t = PMU(1).Signal_Time.Signal_datenum;
TimeStamp = interp1(1:length(t),t,length(t)+1,'linear','extrap');

%% Combine event lists from all out of range (OOR) detectors
% Discard all events that are active

% Get the current list of IDs with stored wind values
[IDcell{1:length(WindAppXML.WindValues)}] = deal('ID');
EventIDs = cellfun(@getfield,WindAppXML.WindValues,IDcell,'UniformOutput',false);
clear IDcell

EventListTemp = EventList;
WindEvents.OOR = [];
    
% Check if an out-of-range detector was included in the detector's
% configuration file. If so, update the event list for the detector.
if isfield(DetectorXML,'OutOfRangeGeneral')
    % Find the number of separate instances of this detector type.
    NumDetectors = length(DetectorXML.('OutOfRangeGeneral'));

    % Update events from each instance of this detector type
    for DetectorIndex = 1:NumDetectors
        % Find Hist, the number of seconds of data from previous files
        % that this detector utilizes (new events can be added in this
        % time window)
        if NumDetectors == 1
            Hist = str2double(DetectorXML.('OutOfRangeGeneral').AnalysisWindow);
        else
            Hist = str2double(DetectorXML.('OutOfRangeGeneral'){DetectorIndex}.AnalysisWindow);
        end
        % Events ending before this time are considered over. Those
        % ending after this time may still be active.
        EventEndLimitHigh = TimeStamp - Hist/(60*60*24);
        EventEndLimitLow = TimeStamp - (Hist+FileLength)/(60*60*24);

        FieldName = ['OutOfRangeGeneral' '_' num2str(DetectorIndex)];

        if ~isfield(EventListTemp, FieldName)
            continue
        end

        % Check each event in the list. Events must have ended between
        % Hist+FileLength and Hist seconds ago. More recent, and they
        % are still active. Older, and they have already been
        % considered.
        NumEvents = length(EventListTemp.(FieldName));
        IgnoreEvents = [];
        for EventIdx = 1:NumEvents
            if ((EventListTemp.(FieldName)(EventIdx).End < EventEndLimitLow) || (EventListTemp.(FieldName)(EventIdx).End >= EventEndLimitHigh))
                % This is an active event. It won't be added to the
                % wind events until it is over.
                IgnoreEvents = [IgnoreEvents EventIdx];
            end

            % Find the power for signals associated
            % with the wind application (listed in the
            % configuration XML). These will be used when the event
            % ends.

            % Check if the power values have already been set for
            % this event. 
            if sum(strcmp(EventIDs,EventListTemp.(FieldName)(EventIdx).ID)) > 0
                % Power values have already been set for this event
                continue
            end

            % Power values must be set for this event
            WindAppXML.WindValues{end+1}.ID = EventListTemp.(FieldName)(EventIdx).ID;
            WindAppXML.WindValues{end}.PMU = WindAppXML.PMU;
            % Iterate through each PMU that needs power values
            % stored
            for PMUidx = 1:length(WindAppXML.WindValues{end}.PMU)
                % Find the index of the PMU in the data structure
                PMUdataIdx = strcmp({PMU.PMU_Name},WindAppXML.WindValues{end}.PMU{PMUidx}.Name);

                % Make sure a PMU structure was found. If not, 
                % issue a warning and jump to next index.
                if sum(PMUdataIdx) == 0
                    warning(['The PMU ' WindAppXML.WindValues{end}.PMU{PMUidx}.Name ' was not found and will not be considered in the wind application.']);
                    continue
                end
                % Make sure only 1 PMU structure was found. If more, then
                % issue a warning and jump to next index.
                if sum(PMUdataIdx) > 1
                    warning(['There is more than one PMU with the name ' WindAppXML.WindValues{end}.PMU{PMUidx}.Name '. It will not be considered in the wind application.']);
                    continue
                end


                % Iterate through the channels listed in the
                % configuration XML
                for ChanIdx = 1:length(WindAppXML.WindValues{end}.PMU{PMUidx}.PowerChannel)
                    % Find the index of the channel in the data
                    % matrix
                    ChanDataIdx = strcmp(PMU(PMUdataIdx).Signal_Name,WindAppXML.WindValues{end}.PMU{PMUidx}.PowerChannel{ChanIdx}.Name);

                    % Make sure the channel was found. If not, 
                    % issue a warning and jump to next index.
                    if sum(ChanDataIdx) == 0
                        warning(['The channel ' WindAppXML.WindValues{end}.PMU{PMUidx}.PowerChannel{ChanIdx}.Name ' in the ' WindAppXML.WindValues{end}.PMU{PMUidx}.Name ' PMU was not found and will not be considered in the wind application.']);
                        continue
                    end
                    % Make sure only 1 channel was found. If more, then
                    % issue a warning and jump to next index.
                    if sum(ChanDataIdx) > 1
                        warning(['There is more than one channel with the name ' WindAppXML.WindValues{end}.PMU{PMUidx}.PowerChannel{ChanIdx}.Name ' in the ' WindAppXML.WindValues{end}.PMU{PMUidx}.Name ' PMU. It will not be considered in the wind application.']);
                        continue
                    end

                    % Store the first non-NaN value of the channel
                    PowChan = PMU(PMUdataIdx).Data(:,ChanDataIdx);
                    val = PowChan(find(~isnan(PowChan),1));
                    if isempty(val)
                        WindAppXML.WindValues{end}.PMU{PMUidx}.PowerChannel{ChanIdx}.Value = -Inf;
                    else
                        WindAppXML.WindValues{end}.PMU{PMUidx}.PowerChannel{ChanIdx}.Value = val;
                    end
                end
            end
        end
        % Remove the events that are too old or new to be considered from the list
        EventListTemp.(FieldName)(IgnoreEvents) = [];
        % Clean up so that it is [] rather than a 1x0 struct
        if isempty(EventListTemp.(FieldName))
            EventListTemp.(FieldName) = [];
        end

        WindEvents.OOR = [WindEvents.OOR EventListTemp.(FieldName)];
    end
end

if ~isfield(EventList, 'WindApp')
    EventList.WindApp = [];
end

[NameCell{1:length(WindAppXML.PMU)}] = deal('Name');
WindPMU = cellfun(@getfield,WindAppXML.PMU,NameCell,'UniformOutput',false);


KillIdx = [];
% Iterate through the events stored in WindEvents.OOR
for EventIdx = 1:length(WindEvents.OOR)
    % Search EventList.WindApp for events with this ID
    if isfield(EventList.WindApp,'ID')
        if sum(strcmp({EventList.WindApp.ID},WindEvents.OOR(EventIdx).ID)) > 0
            % This event is already in EventList.WindApp, so move to next
            % event
            KillIdx = [KillIdx EventIdx];
            continue
        end
    end
    
    % This event is newly over (not in EventList.WindApp)
        
    % Get the current list of IDs with stored wind values
    [IDcell{1:length(WindAppXML.WindValues)}] = deal('ID');
    EventIDs = cellfun(@getfield,WindAppXML.WindValues,IDcell,'UniformOutput',false);
    clear IDcell

    % Find the index of WindAppXML.WindValues
    % corresponding to this event
    EventValueIdx = strcmp(EventIDs,WindEvents.OOR(EventIdx).ID);
    
    % Initialize the WindPower field
    WindEvents.OOR(EventIdx).WindPower = {};

    % Iterate through each of the wind application PMUs
    KeepEvent = false;
    for PMUidx = 1:length(WindAppXML.WindValues{EventValueIdx}.PMU)
        % Check if this wind application PMU was listed under the
        % detected event. If not, move to next event. If it was, check
        % the value of the wind power value.
        if sum(strcmp(WindEvents.OOR(EventIdx).PMU,WindAppXML.WindValues{EventValueIdx}.PMU{PMUidx}.Name)) > 0
            % This wind application PMU was listed under the detected
            % event.

            % Iterate through the wind power channels, and check their
            % value against the associated threshold.
            for PowerChanIdx = 1:length(WindAppXML.WindValues{EventValueIdx}.PMU{PMUidx}.PowerChannel)
                WindEvents.OOR(EventIdx).WindPower{end+1} = [WindAppXML.WindValues{EventValueIdx}.PMU{PMUidx}.Name ',' WindAppXML.WindValues{EventValueIdx}.PMU{PMUidx}.PowerChannel{PowerChanIdx}.Name ',' num2str(WindAppXML.WindValues{EventValueIdx}.PMU{PMUidx}.PowerChannel{PowerChanIdx}.Value)];

                if abs(WindAppXML.WindValues{EventValueIdx}.PMU{PMUidx}.PowerChannel{PowerChanIdx}.Value) > abs(str2double(WindAppXML.WindValues{EventValueIdx}.PMU{PMUidx}.PowerChannel{PowerChanIdx}.Threshold))
                    % This event qualifies to be recorded by the wind
                    % application
                    KeepEvent = true;
                end
            end
        else
            % This wind application PMU was not listed under the
            % detected event, so move to next PMU.
            continue
        end
    end
    
    if ~KeepEvent
        KillIdx = [KillIdx EventIdx];
    end
end

WindEvents.OOR(KillIdx) = [];
% Clean up so that it is [] rather than a 1x0 struct
if isempty(WindEvents.OOR)
    WindEvents.OOR = [];
end

EventList.WindApp = [EventList.WindApp WindEvents.OOR];
