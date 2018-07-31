% function EventList = UpdateEvents(DetectionResults,AdditionalOutput,DetectorXML,AlarmParams,DetectorTypes,EventList)
%
% This function updates the event lists for each of the event and
% oscillation detectors. The functions corresponding to each event and
% oscillation detector are:
%   Periodogram - UpdateForcedOscillationEvents
%   Spectral Coherence - UpdateForcedOscillationEvents()
%   Ringdown - UpdateRingEvents()
%   General out-of-range - UpdateOutOfRangeEvents()
%   Wind ramp - UpdateWindRampEvents()
% These functions all have the same inputs and outputs. This function
% simply calls these functions in sequence to update the lists.
%
% Inputs: (see functions listed above for further details)
%   DetectionResults = Output from RunDetection() containing detection
%                      results
%   AdditionalOutput = Output from RunDetection() containing additional
%                      information from the detectors
%   DetectorXML = Structure array containing detection parameters specified
%                 in the detector XML
%   AlarmParams = Structure array of alarming parameters specified in the
%                 detector XML
%   DetectorTypes = Cell array containing strings describing which
%                   detectors need to be updated
%   EventList = Structure array containing a list of events from each
%               detector.
%
% Outputs:
%   EventList = Structure array containing the updated list of events.
%
% Created by: Jim Follum (james.follum@pnnl.gov) on 11/10/2016

function EventList = UpdateEvents(DetectionResults,AdditionalOutput,DetectorXML,AlarmParams,DetectorTypes,EventList)

%#function UpdateForcedOscillationEvents
%#function UpdateRingEvents
%#function UpdateOutOfRangeEvents
%#function UpdateWindRampEvents
%#function UpdateTheveninEvents

for DetectorType = DetectorTypes
    
    % Note that the {1} following each DetectorType is necessary.
    % DetectorType on its own is a cell. Adding the {1} accesses the string
    % within the cell.
    
    % Check if the current DetectorType was included in the detector's
    % configuration file. If so, update the event list for the detector.
    if isfield(DetectorXML,DetectorType{1})
        % Find the number of separate instances of this detector type.
        NumDetectors = length(DetectorXML.(DetectorType{1}));
        if NumDetectors == 1
            % By default, the contents of DetectorXML.(DetectorType{1}) would not be
            % in a cell array because length is one. This makes it so the same
            % indexing can be used in the following for loop.
            DetectorXML.(DetectorType{1}) = {DetectorXML.(DetectorType{1})};
        end
        
        % Update events from each instance of this detector type
        for DetectorIndex = 1:NumDetectors
            switch DetectorType{1}
                case 'Periodogram'
                    FieldName = 'ForcedOscillation';
                    FunctionName = 'UpdateForcedOscillationEvents';
                case 'SpectralCoherence'
                    FieldName = 'ForcedOscillation';
                    FunctionName = 'UpdateForcedOscillationEvents';
                case 'Ringdown'
                    FieldName = 'Ringdown';
                    FunctionName = 'UpdateRingEvents';
                case 'OutOfRangeGeneral'
                    FieldName = ['OutOfRangeGeneral_' num2str(DetectorIndex)];
                    FunctionName = 'UpdateOutOfRangeEvents';
                case 'WindRamp'
                    FieldName = ['WindRamp_' num2str(DetectorIndex)];
                    FunctionName = 'UpdateWindRampEvents';
                case 'Thevenin'
                    FieldName = ['Thevenin_' num2str(DetectorIndex)];
                    FunctionName = 'UpdateTheveninEvents';
                case 'ModeMeter'
                    continue;
%                     FieldName = 'ModeMeter';
%                     FunctionName = 'UpdateModeMeterEvents';
%                     AlarmParams.ModeMeter{1} = [];
            end
            
            if ~isfield(EventList, FieldName)
                EventList.(FieldName) = [];
            end
            
            if ~isempty(DetectionResults(DetectorIndex).(DetectorType{1}))
                eval(['EventList.' FieldName...
                    '=' FunctionName '(DetectionResults(DetectorIndex).(DetectorType{1}), AdditionalOutput(DetectorIndex).(DetectorType{1}), EventList.' FieldName ', DetectorXML.(DetectorType{1}){DetectorIndex}, AlarmParams.(DetectorType{1}) );'])
            end
        end
    end
end