% function EventList = UpdateForcedOscillationEvents(DetectionResults, AdditionalOutput, EventList, Params, AlarmParams)
%
% This function updates a list of forced oscillation events based on new detection
% results. For example, if an oscillation is detected in two adjacent sets of
% data, this function judges whether they are the same oscillation and
% lists them as such.
%
% Inputs:
%   DetectionResults = Detection results from the forced oscillation 
%                      detectors. See PeriodogramDetector.m and
%                      SpectralCoherenceDetector.m for specifications.
%   AdditionalOutput = additional output from the forced oscillation 
%                      detectors. See PeriodogramDetector.m and
%                      SpectralCoherenceDetector.m for specifications. The
%                      sampling rate, start time of the analysis window,
%                      and end time of the analysis window are used in this
%                      function.
%   EventList = A structure array containing the current list of events.
%               Each entry in the array contains a separate event. Each
%               field is a vector (with the exception of Channel, which is
%               a cell array) with length equal to the number of
%               occurrences of the forced oscillation. These fields are: 
%                   ID = a unique numeric identifier for each occurrence
%                   Frequency = the mean of the frequency estimates
%                               obtained during detection for each
%                               occurrence.
%                   Start = the datenum value corresponding to the earliest
%                           sample where the forced oscillation was
%                           detected for each occurrence
%                   End = the datenum value corresponding to the latest
%                         sample where the forced oscillation was
%                         detected for each occurrence
%                   Persistence = the number of seconds that the forced
%                                 oscillation has persisted during each
%                                 occurrence.
%                   Channel = a cell array of strings specifying the
%                             channels where the forced oscillation was
%                             detected for each occurrence.
%                   Amplitude = the largest estimated amplitude of the
%                               oscillation for each occurrence.
%                   SNR = the largest estimated SNR of the oscillation for
%                         each occurrence.
%                   Coherence = the largest coherence calculated during
%                               detection for each occurrence.
%   Params = parameters specified in the detection XML. In this function,
%            the frequency tolerance used to distinguish between forced
%            oscillations in each detector is needed.
%   AlarmParams = structure array of parameters specified in the detector
%                 XML to govern when an alarm is set for a forced
%                 oscillation. The fields that are common for the
%                 periodogram and spectral coherence methods are:
%                       TimeMin = minimum time in seconds that an
%                                 oscillation must persist for an alarm to
%                                 be set, unless it exceeds
%                                 SNRalarm (CoherenceAlarm).
%                       TimeCorner = Along with SNRcorner
%                                    (CoherenceCorner), specifies a point
%                                    on the alarm curve to adjust the shape
%                                    of the curve.
%                 The fields are different for the periodogram and spectral
%                 coherence methods are:
%                   Periodogram:
%                       SNRalarm = SNR at which an alarm will be set,
%                                  regardless of duration
%                       SNRmin = minimum SNR required for an alarm to be
%                                set, regardless of duration
%                       SNRcorner = Along with TimeCorner, specifies a
%                                   point on the alarm curve to adjust the
%                                   shape of the curve
%                   Spectral Coherence:
%                       CoherenceAlarm = Coherence at which an alarm will 
%                                        be set, regardless of duration
%                       CoherenceMin = minimum Coherence required for an
%                                      alarm to be set, regardless of
%                                      duration 
%                       CoherenceCorner = Along with TimeCorner, specifies
%                                   a point on the alarm curve to adjust
%                                   the shape of the curve
%   
% Outputs:
%   EventList = updated event list. See EventList in the list of Inputs
%               for specifications.
%
% Created by Jim Follum (james.follum@pnnl.gov) in November, 2016.

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
                EventList(EventIdx).ID = AssignEventID();
                EventList(EventIdx).Frequency = DetectionResults(DetIdx).Frequency(FreqIdx);
                EventList(EventIdx).Start = datenum(AdditionalOutput(1).Start);
                EventList(EventIdx).End = datenum(AdditionalOutput(1).End);
                EventList(EventIdx).Persistence = 0;
                EventList(EventIdx).OccurrenceID = {AssignEventID()};
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
                    EventList(EventIdx).OccurrenceID = [EventList(EventIdx).OccurrenceID, AssignEventID()];
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
            EventList(1).ID = AssignEventID();
            EventList(1).Frequency = DetectionResults(DetIdx).Frequency(FreqIdx);
            EventList(1).Start = datenum(AdditionalOutput(1).Start);
            EventList(1).End = datenum(AdditionalOutput(1).End);
            EventList(1).Persistence = 0;
            EventList(1).OccurrenceID = {AssignEventID()};
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

% Update the OverallStart and OverallEnd fields of the EventList, which
% track the earliest and latest the FO was detected across all occurrences
for EventIdx = 1:length(EventList)
    if isfield(EventList(EventIdx),'OverallStart')
        EventList(EventIdx).OverallStart = min([EventList(EventIdx).OverallStart min(EventList(EventIdx).Start)]);
    else
        EventList(EventIdx).OverallStart = min(EventList(EventIdx).Start);
    end
    
    if isfield(EventList(EventIdx),'OverallEnd')
        EventList(EventIdx).OverallEnd = max([EventList(EventIdx).OverallEnd max(EventList(EventIdx).End)]);
    else
        EventList(EventIdx).OverallEnd = max(EventList(EventIdx).End);
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