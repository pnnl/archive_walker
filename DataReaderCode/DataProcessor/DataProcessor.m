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

