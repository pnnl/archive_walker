function PMU = DQandCustomization(PMU,DataXML,NumStages, Num_Flags)

% Initialize the custom PMU sub-structure and add it to the PMU structure
% using some of the fields from an existing PMU sub-structure.
PMU = InitCustomPMU(PMU,PMU(1).File_Name,PMU(1).Time_Zone,PMU(1).Signal_Time,size(PMU(1).Data,1), Num_Flags);
custPMUidx = length(PMU);

for StageIdx = 1:NumStages
    % Data Quality Filtering step (if included in this stage)
    if isfield(DataXML.Configuration.Stages{StageIdx},'Filter')
        PMU = DQfilterStep(PMU,DataXML.Configuration.Stages{StageIdx});
    end
    
    % Signal Customization step (if included in this stage)
    if isfield(DataXML.Configuration.Stages{StageIdx},'Customization')
        PMU = CustomizationStep(PMU,custPMUidx,DataXML.Configuration.Stages{StageIdx});
    end
end