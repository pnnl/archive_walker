% function PMUStruct = DataProcessor(DataProcessorStruct, ProcessXML)
% This function carries out all the data processing.
%
% Inputs:
	% DataProcessorStruct: struct array of dimension 1 by Number of PMU
	% structures to be concatenated
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
            % information on angle unraping operation
            % ProcessXML.Configuration.Processing.MultiRate: struct array containing
            % information on upsampling and downsampling operation
% Outputs:
    % PMUStruct: struct array of PMUs containing processed data
    
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov) on 06/22/2016

function PMUStruct = DataProcessor(DataProcessorStruct, ProcessXML)

if length(DataProcessorStruct) >1
    PMUStruct = ConcatenatePMU(DataProcessorStruct);
end
    
PMUStruct = interpo(PMUStruct,ProcessXML.Configuration.Processing.Interpolate);

if isfield(ProcessXML.Configuration.Processing,'Filter')
    PMUStruct = DPfilterStep(PMUStruct,ProcessXML.Configuration.Processing.Filter);
end

% if isfield(ProcessXML.Configuration.Processing,'MultiRate')
%     PMUStruct = DPMultiRate(PMUStruct,ProcessXML.Configuration.Processing.MultiRate);
% end

if isfield(ProcessXML.Configuration.Processing,'Unwrap')
    PMUStruct = DPUnwrap(PMUStruct,ProcessXML.Configuration.Processing.Unwrap);
end

if isfield(ProcessXML.Configuration.Processing,'Wrap')
    PMUStruct = DPWrap(PMUStruct,ProcessXML.Configuration.Processing.Wrap);
end

