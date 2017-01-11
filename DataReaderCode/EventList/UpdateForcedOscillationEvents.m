function EventList = UpdateForcedOscillationEvents(DetectionResults, AdditionalOutput, EventList, Params, AlarmParams)


% Retrieve and use values specified in the detector's configuration XML
% The frequency tolerance specifies how far apart two forced oscillations
% must be to be considered separate events.
if isfield(DetectionResults,'Amplitude')
    % Periodogram
    ExtractedParameters = ExtractFOdetectionParamsPer(Params,AdditionalOutput(1).fs);
    FrequencyTolerance = ExtractedParameters.FrequencyTolerance;
    
    AlarmField = 'SNR';
    ValAlarm = AlarmParams.SNRalarm;
    ValMin = AlarmParams.SNRmin;
    ValCorner = AlarmParams.SNRcorner;
    % TimeMin and TimeCorner are the same for Periodogram and Spectral
    % Coherence
else
    % Spectral Coherence
    ExtractedParameters = ExtractFOdetectionParamsSC(Params,AdditionalOutput(1).fs);
    FrequencyTolerance = ExtractedParameters.FrequencyTolerance;
    
    AlarmField = 'Coherence';
    ValAlarm = AlarmParams.CoherenceAlarm;
    ValMin = AlarmParams.CoherenceMin;
    ValCorner = AlarmParams.CoherenceCorner;
    % TimeMin and TimeCorner are the same for Periodogram and Spectral
    % Coherence
end

FrequenciesToAdd = {};
UpdatedEventList = [];
for DetIdx = 1:length(DetectionResults)
    % If no FOs were detected, return the existing event list
    if (length(DetectionResults(DetIdx).Frequency) == 1)
        if isnan(DetectionResults(DetIdx).Frequency)
            return
        end
    end

    for FreqIdx = 1:length(DetectionResults(DetIdx).Frequency)
        if isfield(EventList,'Frequency')
            % FOs have been detected already
            FrequencyMeans = cellfun(@mean,{EventList.Frequency});
            EventIdx = near(FrequencyMeans,DetectionResults(DetIdx).Frequency(FreqIdx),FrequencyTolerance);

            if isempty(EventIdx)
                % Not close to any recorded frequencies - a new event
                EventIdx = length(EventList) + 1;
                UpdatedEventList = [UpdatedEventList EventIdx];
                EventList(EventIdx).Frequency = DetectionResults(DetIdx).Frequency(FreqIdx);
                EventList(EventIdx).Start = datenum(AdditionalOutput(1).Start);
                EventList(EventIdx).End = datenum(AdditionalOutput(1).End);
                EventList(EventIdx).Persistence = 0;
                EventList(EventIdx).Channel = DetectionResults(DetIdx).Channel;
                if length(DetectionResults(DetIdx).Channel) == 1
                    EventList(EventIdx).Channel = {DetectionResults(DetIdx).Channel};
                else
                    EventList(EventIdx).Channel = DetectionResults(DetIdx).Channel;
                end
                
                if isfield(DetectionResults(DetIdx),'Amplitude')
                    % Periodogram
                    EventList(EventIdx).Amplitude = {DetectionResults(DetIdx).Amplitude(FreqIdx,:)};
                    EventList(EventIdx).SNR = {DetectionResults(DetIdx).SNR(FreqIdx,:)};
                    EventList(EventIdx).Coherence = {NaN*ones(1,length(DetectionResults(DetIdx).Channel))};
                else
                    % Spectral Coherence
                    EventList(EventIdx).Coherence = {DetectionResults(DetIdx).Coherence(FreqIdx,:)};
                    EventList(EventIdx).Amplitude = {NaN*ones(1,length(DetectionResults(DetIdx).Channel))};
                    EventList(EventIdx).SNR = {NaN*ones(1,length(DetectionResults(DetIdx).Channel))};
                end
            else
                % Close enough to previously detected frequency to be
                % considered the same - update the existing event
                
                UpdatedEventList = [UpdatedEventList EventIdx];

                % If there is overlap in the start and end times, consider it
                % the same occurrence. If not, list as a new occurrence
                if datenum(AdditionalOutput(1).Start) < EventList(EventIdx).End(end)
                    % This FO started before the end of the most recent
                    % occurrence, so it is the same occurrence. Update the
                    % frequency and end time. The frequency is updated by
                    % averaging the frequency from this FO with the previous
                    % frequency. This puts a lot more weight on the most recent
                    % detections.
                    if length(FrequenciesToAdd) < EventIdx
                        FrequenciesToAdd{EventIdx} = [];
                    end
                    FrequenciesToAdd{EventIdx} = [FrequenciesToAdd{EventIdx} DetectionResults(DetIdx).Frequency(FreqIdx)];
                    
                    EventList(EventIdx).Persistence(end) = EventList(EventIdx).Persistence(end) +  (datenum(AdditionalOutput(1).End) - EventList(EventIdx).End(end))*24*60*60;
                    EventList(EventIdx).End(end) = datenum(AdditionalOutput(1).End);
                    % If this start time is earlier than the recorded one,
                    % update it. This could happen if two detection algorithms
                    % or implementations detect the same FO but have different
                    % analysis window lengths
                    if datenum(AdditionalOutput(1).Start) < EventList(EventIdx).Start(end)
                        EventList(EventIdx).Start(end) = datenum(AdditionalOutput(1).Start);
                    end
                    
                    % If a channel involved in detection was not previously
                    % listed, add it.
                    for ChanIdx = 1:length(DetectionResults(DetIdx).Channel)
                        ChanLoc = strcmp(DetectionResults(DetIdx).Channel{ChanIdx}, EventList(EventIdx).Channel{end});
                        if sum(ChanLoc) == 0
                            % This channel hasn't been added yet, so add it
                            EventList(EventIdx).Channel{end} = [EventList(EventIdx).Channel{end}, {DetectionResults(DetIdx).Channel{ChanIdx}}];
                            
                            if isfield(DetectionResults(DetIdx),'Amplitude')
                                % Periodogram
                                EventList(EventIdx).Amplitude{end} = [EventList(EventIdx).Amplitude{end}, DetectionResults(DetIdx).Amplitude(FreqIdx,ChanIdx)];
                                EventList(EventIdx).SNR{end} = [EventList(EventIdx).SNR{end}, DetectionResults(DetIdx).SNR(FreqIdx,ChanIdx)];
                                EventList(EventIdx).Coherence{end} = [EventList(EventIdx).Coherence{end}, NaN];
                            else
                                % Spectral coherence
                                EventList(EventIdx).Coherence{end} = [EventList(EventIdx).Coherence{end}, DetectionResults(DetIdx).Coherence(FreqIdx,ChanIdx)];
                                EventList(EventIdx).Amplitude{end} = [EventList(EventIdx).Amplitude{end}, NaN];
                                EventList(EventIdx).SNR{end} = [EventList(EventIdx).SNR{end}, NaN];
                            end
                        else
                            % This channel has already been added, so just
                            % update the amplitude/coherence if the
                            % currently stored value is smaller or NaN
                            if isfield(DetectionResults(DetIdx),'Amplitude')
                                % Periodogram
                                if (isnan(EventList(EventIdx).Amplitude{end}(ChanLoc)) || (EventList(EventIdx).Amplitude{end}(ChanLoc) < DetectionResults(DetIdx).Amplitude(FreqIdx,ChanIdx)))
                                    EventList(EventIdx).Amplitude{end}(ChanLoc) = DetectionResults(DetIdx).Amplitude(FreqIdx,ChanIdx);
                                end
                                if (isnan(EventList(EventIdx).SNR{end}(ChanLoc)) || (EventList(EventIdx).SNR{end}(ChanLoc) < DetectionResults(DetIdx).SNR(FreqIdx,ChanIdx)))
                                    EventList(EventIdx).SNR{end}(ChanLoc) = DetectionResults(DetIdx).SNR(FreqIdx,ChanIdx);
                                end
                            else
                                % Spectral coherence
                                if (isnan(EventList(EventIdx).Coherence{end}(ChanLoc)) || (EventList(EventIdx).Coherence{end}(ChanLoc) < DetectionResults(DetIdx).Coherence(FreqIdx,ChanIdx)))
                                    EventList(EventIdx).Coherence{end}(ChanLoc) = DetectionResults(DetIdx).Coherence(FreqIdx,ChanIdx);
                                end
                            end
                        end
                    end
                else
                    % This FO started after the end of the most recent
                    % occurrence, so it is a new occurrence
                    EventList(EventIdx).Frequency = [EventList(EventIdx).Frequency, DetectionResults(DetIdx).Frequency(FreqIdx)];
                    EventList(EventIdx).Start = [EventList(EventIdx).Start, datenum(AdditionalOutput(1).Start)];
                    EventList(EventIdx).End = [EventList(EventIdx).End, datenum(AdditionalOutput(1).End)];
                    EventList(EventIdx).Persistence = [EventList(EventIdx).Persistence, 0];
                    if length(DetectionResults(DetIdx).Channel) == 1
                        EventList(EventIdx).Channel = [EventList(EventIdx).Channel, {DetectionResults(DetIdx).Channel}];
                    else
                        EventList(EventIdx).Channel = [EventList(EventIdx).Channel, DetectionResults(DetIdx).Channel];
                    end
                    
                    if isfield(DetectionResults(DetIdx),'Amplitude')
                        % Periodogram
                        EventList(EventIdx).Amplitude = [EventList(EventIdx).Amplitude DetectionResults(DetIdx).Amplitude(FreqIdx,:)];
                        EventList(EventIdx).SNR = [EventList(EventIdx).SNR DetectionResults(DetIdx).SNR(FreqIdx,:)];
                        EventList(EventIdx).Coherence = [EventList(EventIdx).Coherence NaN*ones(1,length(DetectionResults(DetIdx).Channel))];
                    else
                        % Spectral Coherence
                        EventList(EventIdx).Coherence = [EventList(EventIdx).Coherence DetectionResults(DetIdx).Coherence(FreqIdx,:)];
                        EventList(EventIdx).Amplitude = [EventList(EventIdx).Amplitude NaN*ones(1,length(DetectionResults(DetIdx).Channel))];
                        EventList(EventIdx).SNR = [EventList(EventIdx).SNR NaN*ones(1,length(DetectionResults(DetIdx).Channel))];
                    end
                end
            end
        else
            % No FOs have yet been detected - begin the list with this one
            UpdatedEventList = [UpdatedEventList 1];
            EventList(1).Frequency = DetectionResults(DetIdx).Frequency(FreqIdx);
            EventList(1).Start = datenum(AdditionalOutput(1).Start);
            EventList(1).End = datenum(AdditionalOutput(1).End);
            EventList(1).Persistence = 0;
            if length(DetectionResults(DetIdx).Channel) == 1
                EventList(1).Channel = {DetectionResults(DetIdx).Channel};
            else
                EventList(1).Channel = DetectionResults(DetIdx).Channel;
            end
            
            if isfield(DetectionResults(DetIdx),'Amplitude')
                % Periodogram
                EventList(1).Amplitude = {DetectionResults(DetIdx).Amplitude(FreqIdx,:)};
                EventList(1).SNR = {DetectionResults(DetIdx).SNR(FreqIdx,:)};
                EventList(1).Coherence = {NaN*ones(1,length(DetectionResults(DetIdx).Channel))};
            else
                % Spectral Coherence
                EventList(1).Coherence = {DetectionResults(DetIdx).Coherence(FreqIdx,:)};
                EventList(1).Amplitude = {NaN*ones(1,length(DetectionResults(DetIdx).Channel))};
                EventList(1).SNR = {NaN*ones(1,length(DetectionResults(DetIdx).Channel))};
            end
        end
    end
end

for EventIdx = 1:length(FrequenciesToAdd)
    if ~isempty(FrequenciesToAdd{EventIdx})
        EventList(EventIdx).Frequency(end) = mean([EventList(EventIdx).Frequency(end) mean(FrequenciesToAdd{EventIdx})]);
    end
end

UpdatedEventList = unique(UpdatedEventList);
for EventIdx = UpdatedEventList
    MaxVal = cellfun(@max,EventList(EventIdx).(AlarmField));
    AlarmFlag = false(1,length(MaxVal));
    
    AlarmFlag(MaxVal > ValAlarm) = true;
    N = (AlarmParams.TimeCorner-AlarmParams.TimeMin)*(ValCorner-ValMin);
    TimeThresh = N./(MaxVal-ValMin) + AlarmParams.TimeMin;
    AlarmFlag((EventList(EventIdx).Persistence > TimeThresh) & (MaxVal > ValMin)) = true;
    if isfield(EventList,'AlarmFlag')
        if isempty(EventList(EventIdx).AlarmFlag)
            EventList(EventIdx).AlarmFlag = AlarmFlag;
        else
            EventList(EventIdx).AlarmFlag = [EventList(EventIdx).AlarmFlag false(length(AlarmFlag)-length(EventList(EventIdx).AlarmFlag))] | AlarmFlag;
        end
    else
        EventList(EventIdx).AlarmFlag = AlarmFlag;
    end
end

% if isfield(DetectionResults,'SNR')
%     % Periodogram
%     for EventIdx = UpdatedEventList
%         MaxSNR = cellfun(@max,EventList(EventIdx).SNR);
%         AlarmFlag = false(1,length(MaxSNR));
%         AlarmFlag(MaxSNR > AlarmParams.SNRalarm) = true;
%         N = (AlarmParams.TimeCorner-AlarmParams.TimeMin)*(AlarmParams.SNRcorner-AlarmParams.SNRmin);
%         TimeThresh = N./(MaxSNR-AlarmParams.SNRmin) + AlarmParams.TimeMin;
%         AlarmFlag((EventList(EventIdx).Persistence > TimeThresh) & (MaxSNR > AlarmParams.SNRmin)) = true;
%         if isfield(EventList,'AlarmFlag')
%             if isempty(EventList(EventIdx).AlarmFlag)
%                 EventList(EventIdx).AlarmFlag = AlarmFlag;
%             else
%                 EventList(EventIdx).AlarmFlag = [EventList(EventIdx).AlarmFlag false(length(AlarmFlag)-length(EventList(EventIdx).AlarmFlag))] | AlarmFlag;
%             end
%         else
%             EventList(EventIdx).AlarmFlag = AlarmFlag;
%         end
%     end
% else
%     % Spectral Coherence
%     for EventIdx = UpdatedEventList
%         MaxCoherence = cellfun(@max,EventList(EventIdx).Coherence);
%         AlarmFlag = false(1,length(MaxCoherence));
%         AlarmFlag(MaxCoherence > AlarmParams.CoherenceAlarm) = true;
%         N = (AlarmParams.TimeCorner-AlarmParams.TimeMin)*(AlarmParams.CoherenceCorner-AlarmParams.CoherenceMin);
%         TimeThresh = N./(MaxCoherence-AlarmParams.CoherenceMin) + AlarmParams.TimeMin;
%         AlarmFlag((EventList(EventIdx).Persistence > TimeThresh) & (MaxCoherence > AlarmParams.CoherenceMin)) = true;
%         if isfield(EventList,'AlarmFlag')
%             if isempty(EventList(EventIdx).AlarmFlag)
%                 EventList(EventIdx).AlarmFlag = AlarmFlag;
%             else
%                 EventList(EventIdx).AlarmFlag = [EventList(EventIdx).AlarmFlag false(length(AlarmFlag)-length(EventList(EventIdx).AlarmFlag))] | AlarmFlag;
%             end
%         else
%             EventList(EventIdx).AlarmFlag = AlarmFlag;
%         end
%     end
% end