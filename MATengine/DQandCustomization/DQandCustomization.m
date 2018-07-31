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
    % DataXML: structure containing configuration from the input XML file
        % DataXML.Stages: struct array containing
        % information on filter and customization operation in each stage
        % (dimension is 1 by number of stages)
        % DataXML.Flag_Bit: Cotains information on flag bits used by
        % different data quality check filters
    % NumStages: Number of stages
    % Num_Flags: Total number of flag bits
    % FileType: Data file type (csv or PDAT)
%
% Outputs:
    % PMU
% Modified July 28, 2016 by Urmila Agrawal:
% Added filetype input parameter to be used by voltage quality check filter
% as voltage quantity given in pdat file is per phase and that in csv file
% is line-to-line
%
% Modified March 31, 2017 by Jim Follum
    % The FileType is no longer needed by DQfilterStep() (see the 
    % VoltPhasorFilt() function for details) so it was removed as an input.

function PMU = DQandCustomization(PMU,DataXML,NumStages, Num_Flags)

for StageIdx = 1:NumStages
    % Data Quality Filtering step (if included in this stage)
    if isfield(DataXML.Stages{StageIdx},'Filter')
        PMU = DQfilterStep(PMU,DataXML.Stages{StageIdx});
    end
    
    % Signal Customization step (if included in this stage)
    if isfield(DataXML.Stages{StageIdx},'Customization')
        PMU = CustomizationStep(PMU,Num_Flags,DataXML.Stages{StageIdx});
    end
end