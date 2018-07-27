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

function Mode_n_SysCondListID = WriteOperatingConditionsAndModeValues(AdditionalOutput,ResultPath, Mode_n_SysCondListID)
%
% EventList = {};
% return;
%
if isempty(Mode_n_SysCondListID)
    Mode_n_SysCondListID = AssignEventID();
    FileName = [ResultPath Mode_n_SysCondListID '.csv'];
    % DataMat(:,1) = datestr(AdditionalOutput.t);
    H1 = {};
    H2 = {};
    H3 = {};
    H4 = {};
    for SysCondIdx = 1:length(AdditionalOutput(1).OperatingNames)
        H1 = [H1, 'OperatingValue'];
        H2= [H2, AdditionalOutput(1).OperatingNames{SysCondIdx}];
        H3 = [H3, AdditionalOutput(1).OperatingType{SysCondIdx}];
        H4 = [H4, AdditionalOutput(1).OperatingUnits{SysCondIdx}];
    end
    for ModeIdx = 1:length(AdditionalOutput)
        for EventIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
            H1 = [H1, 'DampingRatio'];
            H2 = [H2, AdditionalOutput(ModeIdx).ModeOfInterest{EventIdx}];
            H3 = [H3, AdditionalOutput(ModeIdx).ChannelsName{EventIdx}];
            H4 = [H4, AdditionalOutput(ModeIdx).MethodName{EventIdx}];
        end
    end
    for ModeIdx = 1:length(AdditionalOutput)
        for EventIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
            H1 = [H1, 'Frequency'];
            H2 = [H2, AdditionalOutput(ModeIdx).ModeOfInterest{EventIdx}];
            H3 = [H3, AdditionalOutput(ModeIdx).ChannelsName{EventIdx}];
            H4 = [H4, AdditionalOutput(ModeIdx).MethodName{EventIdx}];
        end
    end
else
    FileName = [ResultPath Mode_n_SysCondListID '.csv'];
     Habc = readtable(FileName);
    H1 = Habc.Properties.VariableNames;%textscan(fid,'%s','delimiter', '\n');
%     H1(1) = {''};
    H2 = Habc{1,:}; %textscan(fid,'%s','delimiter', '\n');
    H3 = Habc{2,:}; %textscan(fid,'%s','delimiter', '\n');
    H4 = Habc{3,:}; %textscan(fid,'%s','delimiter', '\n');
%     delete(FileName);
    %fclose(fid);
end
% if length(AdditionalOutput(1).t)==1
%     Tstr{1} = datestr(AdditionalOutput(1).t);
% else
%     for TIdx = 1:length(AdditionalOutput(1).t)
%         Tstr{TIdx} = datestr(AdditionalOutput(1).t(TIdx));
%     end
% end

for SysCondIdx = 1:length(AdditionalOutput(1).OperatingNames)
    T1 = cell2mat({AdditionalOutput(1).OperatingValues});
end
T2 = [];
for ModeIdx = 1:length(AdditionalOutput)
        T2 = [T2 cell2mat(AdditionalOutput(ModeIdx).ModeDRHistory)];
end
for ModeIdx = 1:length(AdditionalOutput)
        T2 = [T2 cell2mat(AdditionalOutput(ModeIdx).ModeFreqHistory)];
end
% T3 = [];
% for ModeIdx = 1:length(AdditionalOutput)
%     for EventIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
%         T3 = [T3 cell2mat(AdditionalOutput(ModeIdx).ModeFreqHistory)];
%     end
% end

T = [T1 T2];
H = {H1,H2,H3,H4};
fid = fopen(FileName,'w');
for idx = 1:4
    commaHeader = [H{idx};repmat({','},1,numel(H{idx}))]; %insert commaas
    commaHeader = commaHeader(:)';
    commaHeader = commaHeader(1:end-1);
    textHeader = cell2mat(commaHeader); %cHeader in text with commas
    fprintf(fid,'%s\n',textHeader);
end
% %fprintf(fid,'%s,%f',T{:,1});
% for k=1:size(T,1)
% %     fprintf(fid,'%s',Tstr{k});   
%     fprintf(fid,'%f,',T(k,:});   
%     fprintf(fid,'\n');   
% end
fclose(fid);
dlmwrite(FileName,T,'-append');

% DataMat = [];
% Data(1,:)
% for SysCondIdx = 1:length(AdditionalOutput(1).OperatingNames)
%
% end
% for ModeIdx = 1:length(AdditionalOutput)
%     for EventIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
%         H1 = {H1, 'DampingRatio'};
%         H2 = {H2, AdditionalOutput(ModeIdx).ModeOfInterest{EventIdx}};
%         H3 = {H3, AdditionalOutput(ModeIdx).ChannelsName{EventIdx}};
%         H4 = {H4, AdditionalOutput(ModeIdx).MethodName{EventIdx}};
%     end
% end
% for ModeIdx = 1:length(AdditionalOutput)
%     for EventIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
%         H1 = {H1, 'Frequency'};
%         H2 = {H2, AdditionalOutput(ModeIdx).ModeOfInterest{EventIdx}};
%         H3 = {H3, AdditionalOutput(ModeIdx).ChannelsName{EventIdx}};
%         H4 = {H4, AdditionalOutput(ModeIdx).MethodName{EventIdx}};
%     end
% end
