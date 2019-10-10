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

% New function that will also record the PMU in EventList. I left off on line 284.

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
RevisitOccurrence = zeros(0,2);
for DetIdx = 1:length(DetectionResults)
    % If no FOs were detected, move to next detection result
    if (length(DetectionResults(DetIdx).Frequency) == 1)
        if isnan(DetectionResults(DetIdx).Frequency)
            continue
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
%                 if length(DetectionResults(DetIdx).Channel) == 1
%                     EventList(EventIdx).Channel = {DetectionResults(DetIdx).Channel};
%                 else
%                     EventList(EventIdx).Channel = DetectionResults(DetIdx).Channel;
%                 end
                EventList(EventIdx).Channel = {DetectionResults(DetIdx).Channel};
                EventList(EventIdx).PMU = {DetectionResults(DetIdx).PMU};
                EventList(EventIdx).Unit = {DetectionResults(DetIdx).Unit};
                
                EventList(EventIdx).DEF = DetectionResults(DetIdx).DEF(:,FreqIdx);
                EventList(EventIdx).PathDescription = Params.PathDescription;
                
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
                        
                        % This may cause overlap with an earlier
                        % occurrence, so add it to the list of occurrences
                        % that need to be checked at the end of this
                        % function
                        RevisitOccurrence = [RevisitOccurrence; EventIdx length(EventList(EventIdx).Start)];
                    end
                    
                    % If a channel involved in detection was not previously
                    % listed, add it.
                    for ChanIdx = 1:length(DetectionResults(DetIdx).Channel)
                        ChanLoc = strcmp(DetectionResults(DetIdx).Channel{ChanIdx}, EventList(EventIdx).Channel{end}) & strcmp(DetectionResults(DetIdx).PMU{ChanIdx}, EventList(EventIdx).PMU{end});
                        if sum(ChanLoc) == 0
                            % This channel hasn't been added yet, so add it
                            EventList(EventIdx).Channel{end} = [EventList(EventIdx).Channel{end}, {DetectionResults(DetIdx).Channel{ChanIdx}}];
                            EventList(EventIdx).PMU{end} = [EventList(EventIdx).PMU{end}, {DetectionResults(DetIdx).PMU{ChanIdx}}];
                            EventList(EventIdx).Unit{end} = [EventList(EventIdx).Unit{end}, {DetectionResults(DetIdx).Unit{ChanIdx}}];
                            
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
                    
                    % If the maximum DEF is now larger (or if it is currently listed as NaN), update the values
                    MaxDEF = max(EventList(EventIdx).DEF(:,end));
                    if (max(DetectionResults(DetIdx).DEF(:,FreqIdx)) > MaxDEF) || isnan(MaxDEF)
                        EventList(EventIdx).DEF(:,end) = DetectionResults(DetIdx).DEF(:,FreqIdx);
                    end
                else
                    % This FO started after the end of the most recent
                    % occurrence, so it is a new occurrence
                    EventList(EventIdx).Frequency = [EventList(EventIdx).Frequency, DetectionResults(DetIdx).Frequency(FreqIdx)];
                    EventList(EventIdx).Start = [EventList(EventIdx).Start, datenum(AdditionalOutput(1).Start)];
                    EventList(EventIdx).End = [EventList(EventIdx).End, datenum(AdditionalOutput(1).End)];
                    EventList(EventIdx).Persistence = [EventList(EventIdx).Persistence, 0];
                    EventList(EventIdx).OccurrenceID = [EventList(EventIdx).OccurrenceID, AssignEventID()];
%                     if length(DetectionResults(DetIdx).Channel) == 1
%                         EventList(EventIdx).Channel = [EventList(EventIdx).Channel, {DetectionResults(DetIdx).Channel}];
%                     else
%                         EventList(EventIdx).Channel = [EventList(EventIdx).Channel, DetectionResults(DetIdx).Channel];
%                     end
                    EventList(EventIdx).Channel = [EventList(EventIdx).Channel, {DetectionResults(DetIdx).Channel}];
                    EventList(EventIdx).PMU = [EventList(EventIdx).PMU, {DetectionResults(DetIdx).PMU}];
                    EventList(EventIdx).Unit = [EventList(EventIdx).Unit, {DetectionResults(DetIdx).Unit}];
                    
                    EventList(EventIdx).DEF = [EventList(EventIdx).DEF, DetectionResults(DetIdx).DEF(:,FreqIdx)];
                    
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
%             if length(DetectionResults(DetIdx).Channel) == 1
%                 EventList(1).Channel = {DetectionResults(DetIdx).Channel};
%             else
%                 EventList(1).Channel = DetectionResults(DetIdx).Channel;
%             end
            EventList(1).Channel = {DetectionResults(DetIdx).Channel};
            EventList(1).PMU = {DetectionResults(DetIdx).PMU};
            EventList(1).Unit = {DetectionResults(DetIdx).Unit};
            
            EventList(1).DEF = DetectionResults(DetIdx).DEF(:,FreqIdx);
            EventList(1).PathDescription = Params.PathDescription;
            
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

% For each occurrence that had its start point moved earlier, check to see
% if it now overlaps with another occurrence
%
% Structure array to contain all of the occurrences are merged and need to be
% removed. The structure array index corresponds to the EventIdx
KillList = struct('OverlapIdx',cell(1,max(RevisitOccurrence(:,1))));
for idx = 1:size(RevisitOccurrence,1)
    EventIdx = RevisitOccurrence(idx,1);
    OccurIdx = RevisitOccurrence(idx,2);
    
    % Find indices of occurrences that overlap with the one being
    % considered
    OverlapIdx = find(EventList(EventIdx).Start(OccurIdx) < EventList(EventIdx).End);
    % Remove the occurrence under consideration from the list
    OverlapIdx = setdiff(OverlapIdx,OccurIdx);
    % Add these occurrences, which are about to be merged, to the list of
    % those for this event that need to be removed
    KillList(EventIdx).OverlapIdx = [KillList(EventIdx).OverlapIdx OverlapIdx];
    % If there are more occurrences that this one overlaps with, merge them
    if ~isempty(OverlapIdx)
        % The OccurrenceID is not updated
        %
        % OverallStart and OverallEnd will be updated in the next block of
        % code
        
        % The new frequency is the average of the frequencies from each
        % occurrence
        EventList(EventIdx).Frequency(OccurIdx) = mean([EventList(EventIdx).Frequency(OccurIdx) EventList(EventIdx).Frequency(OverlapIdx)]);
        % The new start time is the earliest start time from each
        % occurrence
        EventList(EventIdx).Start(OccurIdx) = min([EventList(EventIdx).Start(OccurIdx) EventList(EventIdx).Start(OverlapIdx)]);
        % The new end time is the latest end time from each occurrence
        EventList(EventIdx).End(OccurIdx) = max([EventList(EventIdx).End(OccurIdx) EventList(EventIdx).End(OverlapIdx)]);
        % The new persistence is the maximum from each occurrence
        EventList(EventIdx).Persistence(OccurIdx) = max([EventList(EventIdx).Persistence(OccurIdx) EventList(EventIdx).Persistence(OverlapIdx)]);
        % The new alarm flag is an OR of the flags from each occurrence
        EventList(EventIdx).AlarmFlag(OccurIdx) = max([EventList(EventIdx).AlarmFlag(OccurIdx) EventList(EventIdx).AlarmFlag(OverlapIdx)]);
        % The new DEF is the entry with the largest value
        temp = [EventList(EventIdx).DEF(:,OccurIdx) EventList(EventIdx).DEF(:,OverlapIdx)];
        [~,MaxIdx] = max(max(temp));
        EventList(EventIdx).DEF(:,OccurIdx) = temp(:,MaxIdx);
        
        % Merge all the information related to individual channels - name,
        % amplitude, SNR, coherence
        CombinedChannel = {};
        CombinedPMU = {};
        CombinedUnit = {};
        CombinedAmp = [];
        CombinedSNR = [];
        CombinedCohere = [];
        for MergeIdx = OverlapIdx
            CombinedChannel = [CombinedChannel EventList(EventIdx).Channel{MergeIdx}];
            CombinedPMU = [CombinedPMU EventList(EventIdx).PMU{MergeIdx}];
            CombinedUnit = [CombinedUnit EventList(EventIdx).Unit{MergeIdx}];
            CombinedAmp = [CombinedAmp EventList(EventIdx).Amplitude{MergeIdx}];
            CombinedSNR = [CombinedSNR EventList(EventIdx).SNR{MergeIdx}];
            CombinedCohere = [CombinedCohere EventList(EventIdx).Coherence{MergeIdx}];
        end
        % Create a new list of all channels involved from any of the occurrences
        %
        % Combine channel and PMU names
        CombinedChannelPMU = strcat(CombinedChannel,CombinedPMU);
        % Find index of unique channel/PMU pairs
        [~,uidx] = unique(CombinedChannelPMU);
        % Reduce list of channels to unique channel/PMU pairs
        NewChannel = CombinedChannel(uidx);
        NewPMU = CombinedPMU(uidx);
        NewUnit = CombinedUnit(uidx);
        NewChannelPMU = CombinedChannelPMU(uidx);
        NewAmp = [];
        NewSNR = [];
        NewCohere = [];
        % For each unique channel
        for UniqueChannelPMU = NewChannelPMU
            % Find the indices of the matching channels
            ThisChannel = strcmp(UniqueChannelPMU,CombinedChannelPMU);
            NewAmp = [NewAmp max(CombinedAmp(ThisChannel))];
            NewSNR = [NewSNR max(CombinedSNR(ThisChannel))];
            NewCohere = [NewCohere max(CombinedCohere(ThisChannel))];
        end
        EventList(EventIdx).Channel{OccurIdx} = NewChannel;
        EventList(EventIdx).PMU{OccurIdx} = NewPMU;
        EventList(EventIdx).Unit{OccurIdx} = NewUnit;
        EventList(EventIdx).Amplitude{OccurIdx} = NewAmp;
        EventList(EventIdx).SNR{OccurIdx} = NewSNR;
        EventList(EventIdx).Coherence{OccurIdx} = NewCohere;
    end
end
% Remove the other occurrences that have been merged into the
% newest one
for EventIdx = 1:length(KillList)
    if ~isempty(KillList(EventIdx).OverlapIdx)
        EventList(EventIdx).Frequency(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).Start(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).End(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).Persistence(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).OccurrenceID(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).Channel(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).PMU(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).Unit(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).Amplitude(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).SNR(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).Coherence(KillList(EventIdx).OverlapIdx) = [];
        EventList(EventIdx).AlarmFlag(KillList(EventIdx).OverlapIdx) = [];
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


if ~isempty(UpdatedEventList)
    UpdatedEventList = unique(UpdatedEventList);
end
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