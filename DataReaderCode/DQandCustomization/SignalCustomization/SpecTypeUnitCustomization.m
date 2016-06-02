function PMUstruct = SpecTypeUnitCustomization(PMUstruct,custPMUidx,Parameters)

% Make sure the specified signal type and unit are compatible
if ~CheckTypeAndUnits(Parameters.SigType,Parameters.SigUnit)
    % Not compatible
    warning([Parameters.SigType ' and ' Parameters.SigUnit ' are incompatible, no change made.']);
    return
end

AvailablePMU = {PMUstruct.PMU_Name};

PMUidx = find(strcmp(Parameters.PMU,AvailablePMU));
% If the specified PMU is not in PMUstruct, issue warning and do nothing
if isempty(PMUidx)
    warning(['PMU ' Parameters.PMU ' could not be found, no change made.']);
    return
end

SigIdx = find(strcmp(Parameters.Channel,PMUstruct(PMUidx).Signal_Name));
% If the specified signal is not in PMUstruct, issue warning and do nothing
if isempty(SigIdx)
    warning(['Signal ' Parameters.signal{SigCount}.Channel ' could not be found, no change made.']);
    return
end

% All checks passed, set type and unit for input signal or create a new
% custom signal with the type and unit
if isfield(Parameters,'CustName')
    % A custom signal name was provided, so instead of replacing the input
    % signal add a new signal to the custom signal PMU
    
    % Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
    [~,NcustSigs] = size(PMUstruct(custPMUidx).Data);

    PMUstruct(custPMUidx).Data(:,NcustSigs+1) = PMUstruct(PMUidx).Data(:,SigIdx);
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1) = PMUstruct(PMUidx).Flag(:,SigIdx);
    PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = Parameters.SigType;
    PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = Parameters.SigUnit;
    PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = Parameters.CustName;
else
    % A custom signal name was not provided, so replace the input signal's type
    % and unit
    PMUstruct(PMUidx).Signal_Type{SigIdx} = Parameters.SigType;
    PMUstruct(PMUidx).Signal_Unit{SigIdx} = Parameters.SigUnit;
end