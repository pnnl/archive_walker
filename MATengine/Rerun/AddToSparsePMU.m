function SparsePMU = AddToSparsePMU(SparsePMU,AdditionalOutput,DetectorXML,DetectorTypes)

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
            DataMin = min(AdditionalOutput(DetectorIndex).(DetectorType{1})(1).Data,[],1,'omitnan');
            DataMax = max(AdditionalOutput(DetectorIndex).(DetectorType{1})(1).Data,[],1,'omitnan');
            DataPMU = AdditionalOutput(DetectorIndex).(DetectorType{1})(1).DataPMU;
            DataChannel = AdditionalOutput(DetectorIndex).(DetectorType{1})(1).DataChannel;
            DataType = AdditionalOutput(DetectorIndex).(DetectorType{1})(1).DataType;
            DataUnit = AdditionalOutput(DetectorIndex).(DetectorType{1})(1).DataUnit;
%             if strcmp(DetectorType{1},'WindRamp')
%                 DataRawMin = min(AdditionalOutput(DetectorIndex).(DetectorType{1})(1).DataRaw,[],1,'omitnan');
%                 DataRawMax = max(AdditionalOutput(DetectorIndex).(DetectorType{1})(1).DataRaw,[],1,'omitnan');
%             end
            
            if isfield(SparsePMU,DetectorType{1}) && (length(SparsePMU) >= DetectorIndex)
                % Already has data or is empty
                
                % SparsePMU has been initialized. Only add these values if the PMU and
                % channel names are in agreement, otherwise throw a warning
                if isempty(SparsePMU(DetectorIndex).(DetectorType{1}))
                    % Empty - no entry yet, so add the entry
                    SparsePMU(DetectorIndex).(DetectorType{1}).DataPMU = DataPMU;
                    SparsePMU(DetectorIndex).(DetectorType{1}).DataChannel = DataChannel;
                    SparsePMU(DetectorIndex).(DetectorType{1}).DataType = DataType;
                    SparsePMU(DetectorIndex).(DetectorType{1}).DataUnit = DataUnit;
                    SparsePMU(DetectorIndex).(DetectorType{1}).DataMin = DataMin;
                    SparsePMU(DetectorIndex).(DetectorType{1}).DataMax = DataMax;
                    SparsePMU(DetectorIndex).(DetectorType{1}).TimeStamp = {AdditionalOutput(1).TimeStamp};
%                     if strcmp(DetectorType{1},'WindRamp')
%                         SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin = DataRawMin;
%                         SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax = DataRawMax;
%                     end
                else
                    % Data already present, only add to it if names are in
                    % agreement
                    if length(DataPMU) == length(SparsePMU(DetectorIndex).(DetectorType{1}).DataPMU)
                        cond2 = sum(strcmp(DataPMU,SparsePMU(DetectorIndex).(DetectorType{1}).DataPMU) & strcmp(DataChannel,SparsePMU(DetectorIndex).(DetectorType{1}).DataChannel)) == length(DataPMU);
                    else
                        cond2 = false;
                    end
                    
                    if cond2
                        SparsePMU(DetectorIndex).(DetectorType{1}).DataMin = [SparsePMU(DetectorIndex).(DetectorType{1}).DataMin; DataMin];
                        SparsePMU(DetectorIndex).(DetectorType{1}).DataMax = [SparsePMU(DetectorIndex).(DetectorType{1}).DataMax; DataMax];
%                         if strcmp(DetectorType{1},'WindRamp')
%                             SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin = [SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin; DataRawMin];
%                             SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax = [SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax; DataRawMax];
%                         end
                    else
                        warning('PMU or channel names have changed, so SparsePMU values are being set to NaN');
                        SparsePMU(DetectorIndex).(DetectorType{1}).DataMin = [SparsePMU(DetectorIndex).(DetectorType{1}).DataMin; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataMin,2))];
                        SparsePMU(DetectorIndex).(DetectorType{1}).DataMax = [SparsePMU(DetectorIndex).(DetectorType{1}).DataMax; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataMax,2))];
%                         if strcmp(DetectorType{1},'WindRamp')
%                             SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin = [SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin,2))];
%                             SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax = [SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax; NaN(1,size(SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax,2))];
%                         end
                    end

                    SparsePMU(DetectorIndex).(DetectorType{1}).TimeStamp = [SparsePMU(DetectorIndex).(DetectorType{1}).TimeStamp; AdditionalOutput(1).TimeStamp];
                end
            else
                % Add first entry
                SparsePMU(DetectorIndex).(DetectorType{1}).DataPMU = DataPMU;
                SparsePMU(DetectorIndex).(DetectorType{1}).DataChannel = DataChannel;
                SparsePMU(DetectorIndex).(DetectorType{1}).DataType = DataType;
                SparsePMU(DetectorIndex).(DetectorType{1}).DataUnit = DataUnit;
                SparsePMU(DetectorIndex).(DetectorType{1}).DataMin = DataMin;
                SparsePMU(DetectorIndex).(DetectorType{1}).DataMax = DataMax;
                SparsePMU(DetectorIndex).(DetectorType{1}).TimeStamp = {AdditionalOutput(1).TimeStamp};
%                 if strcmp(DetectorType{1},'WindRamp')
%                     SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMin = DataRawMin;
%                     SparsePMU(DetectorIndex).(DetectorType{1}).DataRawMax = DataRawMax;
%                 end
            end
        end
    end
end