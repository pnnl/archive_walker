% function PMU = DataProcessor(DataProcessorStruct, ProcessXML)
% This function carries out all the data processing operation.
%
% Inputs:
	% DataProcessorStruct: struct array for a single PMU
    % ProcessXML: structure containing configuration from the input XML
    % file for data processing
        % ProcessXML.Configuration.Processing: struct array containing
        % information on data processing
            % ProcessXML.Configuration.Processing.Filter: struct array containing
            % information on filtering operation
            % ProcessXML.Configuration.Processing.Interpolate: struct array containing
            % information on missing data interpolation
            % ProcessXML.Configuration.Processing.Wrap: struct array containing
            % information on angle wraping operation
            % ProcessXML.Configuration.Processing.Unwrap: struct array containing
            % information on angle unwraping operation
            % ProcessXML.Configuration.Processing.MultiRate: struct array containing
            % information on datarate change operation
% Outputs:
    % PMU: struct array of PMUs containing processed data
%        
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov) on 06/22/2016

function PMUStruct = DataProcessor(PMUStruct, ProcessXML,NumStages,FlagBitInterpo)


if isfield(ProcessXML.Configuration.Processing,'Unwrap')
    PMUStruct.PMU = DPUnwrap(PMUStruct.PMU,ProcessXML.Configuration.Processing.Unwrap);
end

if isfield(ProcessXML.Configuration.Processing,'Interpolate')
    PMUStruct.PMU = DPinterpolation(PMUStruct.PMU,ProcessXML.Configuration.Processing.Interpolate,FlagBitInterpo);
end

for StageIdx = 1:NumStages
    
    if isfield(ProcessXML.Configuration.Processing.Stages{StageIdx},'Filter')
        PMUStruct.PMU = DPfilterStep(PMUStruct.PMU,ProcessXML.Configuration.Processing.Stages{StageIdx}.Filter);
    end
    
    if isfield(ProcessXML.Configuration.Processing.Stages{StageIdx},'Multirate')
        PMUStruct.PMU= DPMultiRate(PMUStruct.PMU,ProcessXML.Configuration.Processing.Stages{StageIdx}.Multirate);
    end
end

if isfield(ProcessXML.Configuration.Processing,'Wrap')
    PMUStruct.PMU = DPWrap(PMUStruct.PMU,ProcessXML.Configuration.Processing.Wrap);
end