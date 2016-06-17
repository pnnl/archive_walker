% function PMU = DQandCustomization(PMU,DataXML,NumStages, Num_Flags)
% This function carries out all the filter and customization operation for
% all the stages
%
% Inputs:
	% PMU: struct array of dimension 1 by Number of PMUs
        % PMU(i).File_Name: a string specifying file name containing i^th PMU data        
        % PMU(i).PMU_Name: a string specifying name of i^th PMU
        % PMU(i).Time_zone:  a string specifying time-zone from where data are recorded
        % PMU(i).Signal_Time: a string specifying time stamp of i^th PMU data
        % PMU(i).Signal_Name: a cell array of strings specifying name of Signals in i^th PMU
        % PMU(i).Signal_Type: a cell array of strings specifying type of Signals in i^th PMU
        % PMU(i).Signal_Unit: a cell array of strings specifying unit of Signals in i^th PMU
        % PMU(i).Stat: Numerical array specifying status of i^th PMU
        % PMU(i).Data: Numerical matrix containing data measured by the i^th PMU
        % PMU(i).Flag: 3-dimensional matrix providing information on the
        % i^th PMU data that are flagged by different filters
    % Stages: array of struct containing information on DQ filter and 
        % customization operation in each stage. This array is part of
        % the DataXML structure accessed as DataXML.Configuration.Stages.
        % (dimension is 1 by number of stages)
    % NumStages: Number of stages
    % Num_Flags: Total number of flag bits
%
% Outputs:
    % PMU
    
function PMU = DQandCustomization(PMU,Stages,NumStages, Num_Flags)

% Initialize the custom PMU sub-structure and add it to the PMU structure
% using some of the fields from an existing PMU sub-structure.
PMU = InitCustomPMU(PMU,PMU(1).File_Name,PMU(1).Time_Zone,PMU(1).Signal_Time,size(PMU(1).Data,1), Num_Flags);
custPMUidx = length(PMU);

for StageIdx = 1:NumStages
    % Data Quality Filtering step (if included in this stage)
    if isfield(Stages{StageIdx},'Filter')
        PMU = DQfilterStep(PMU,Stages{StageIdx});
    end
    
    % Signal Customization step (if included in this stage)
    if isfield(Stages{StageIdx},'Customization')
        PMU = CustomizationStep(PMU,custPMUidx,Stages{StageIdx});
    end
end