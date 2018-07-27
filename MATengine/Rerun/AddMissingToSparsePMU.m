function SparsePMU = AddMissingToSparsePMU(SparsePMU,TimeStamp,DetectorXML,DetectorTypes)

for DetectorType = DetectorTypes
    
    % Note that the {1} following each DetectorType is necessary.
    % DetectorType on its own is a cell. Adding the {1} accesses the string
    % within the cell.
    
    % Check if the current DetectorType was included in the detector's
    % configuration file. If so, run the detector.
    if isfield(DetectorXML,DetectorType{1})
        % Find the number of separate instances of this detector type.
        NumDetectors = length(DetectorXML.(DetectorType{1}));
        if NumDetectors == 1
            % By default, the contents of DetectorXML.(DetectorType{1}) would not be
            % in a cell array because length is one. This makes it so the same
            % indexing can be used in the following for loop.
            DetectorXML.(DetectorType{1}) = {DetectorXML.(DetectorType{1})};
        end
        
        % Implement each instance of this detector type
        for DetectorIndex = 1:NumDetectors
            SparsePMU(DetectorIndex).(DetectorType{1}).DataMin = [SparsePMU(DetectorIndex).(DetectorType{1}).DataMin; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataMin,2))];
            SparsePMU(DetectorIndex).(DetectorType{1}).DataMax = [SparsePMU(DetectorIndex).(DetectorType{1}).DataMax; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataMax,2))];
%             if strcmp(DetectorType{1},'WindRamp')
%                 SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin = [SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin,2))];
%                 SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax = [SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax,2))];
%             end
            
            SparsePMU(DetectorIndex).(DetectorType{1}).TimeStamp = [SparsePMU(DetectorIndex).(DetectorType{1}).TimeStamp; TimeStamp];
        end
    end
end