
% Created by: Jim Follum (james.follum@pnnl.gov) on 11/10/2016

function EventList = UpdateEvents(DetectionResults,AdditionalOutput,DetectorXML,DetectorTypes,EventList)

for DetectorType = DetectorTypes
    
    % Note that the {1} following each DetectorType is necessary.
    % DetectorType on its own is a cell. Adding the {1} accesses the string
    % within the cell.
    
    % Check if the current DetectorType was included in the detector's
    % configuration file. If so, update the event list for the detector.
    if isfield(DetectorXML.Configuration,DetectorType{1})
        % Find the number of separate instances of this detector type.
        NumDetectors = length(DetectorXML.Configuration.(DetectorType{1}));
        if NumDetectors == 1
            % By default, the contents of DetectorXML.Configuration.(DetectorType{1}) would not be
            % in a cell array because length is one. This makes it so the same
            % indexing can be used in the following for loop.
            DetectorXML.Configuration.(DetectorType{1}) = {DetectorXML.Configuration.(DetectorType{1})};
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
                    FunctionName = 'UpdateOutOfRangeAndRingEvents';
                case 'OutOfRangeGeneral'
                    FieldName = ['OutOfRangeGeneral_' num2str(DetectorIndex)];
                    FunctionName = 'UpdateOutOfRangeAndRingEvents';
                case 'OutOfRangeFrequency'
                    FieldName = ['OutOfRangeFrequency_' num2str(DetectorIndex)];
                    FunctionName = 'UpdateOutOfRangeAndRingEvents';
                case 'WindRamp'
                    FieldName = 'WindRamp';
                    FunctionName = 'UpdateWindRampEvents';
            end
            
            if ~isfield(EventList, FieldName)
                EventList.(FieldName) = [];
            end
            
            if ~isempty(DetectionResults(DetectorIndex).(DetectorType{1}))
                eval(['EventList.' FieldName...
                    '=' FunctionName '(DetectionResults(DetectorIndex).(DetectorType{1}), AdditionalOutput(DetectorIndex).(DetectorType{1}), EventList.' FieldName ', DetectorXML.Configuration.(DetectorType{1}){DetectorIndex});'])
            end
        end
    end
end