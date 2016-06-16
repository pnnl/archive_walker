function PMUstruct = SpecTypeUnitCustomization(PMUstruct,custPMUidx,Parameters)

% Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals

[~,NcustSigs,NFlags] = size(PMUstruct(custPMUidx).Flag);

if isfield(Parameters,'CustName')
    % Make sure the specified signal type and unit are compatible
    if ~CheckTypeAndUnits(Parameters.SigType,Parameters.SigUnit)
        % Not compatible
        warning([Parameters.SigType ' and ' Parameters.SigUnit ' are incompatible. Values were set to NaN and Flags set.']);
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = true; %flags is set for customized signal for error in user input
        return
    end
    
    AvailablePMU = {PMUstruct.PMU_Name};
    
    PMUidx = find(strcmp(Parameters.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, issue warning and do nothing
    if isempty(PMUidx)
        warning(['PMU ' Parameters.PMU ' could not be found. Values were set to NaN and Flags set.']);
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = true; %flags is set for customized signal for error in user input
        return
    end
    
    SigIdx = find(strcmp(Parameters.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, issue warning and do nothing
    if isempty(SigIdx)
        warning(['Signal ' Parameters.signal{SigCount}.Channel ' could not be found. Values were set to NaN and Flags set.']);
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = true; %flags is set for customized signal for error in user input
        return
    end
    
    % All checks passed, set type and unit for input signal or create a new
    % custom signal with the type and unit
    % A custom signal name was provided, so instead of replacing the input
    % signal add a new signal to the custom signal PMU
    PMUstruct(custPMUidx).Data(:,NcustSigs+1) = PMUstruct(PMUidx).Data(:,SigIdx);
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,:) = PMUstruct(PMUidx).Flag(:,SigIdx,:);
    FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags-1) = FlagVec; %flags is set for customized signal obtained from input signal with flagged data
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = false; %flags is set for customized signal obtained from input signal with flagged data
    PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = Parameters.SigType;
    PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = Parameters.SigUnit;
    PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = Parameters.CustName;
else
    % A custom signal name was not provided, so replace the input signal's type
    % and unit
    % Make sure the specified signal type and unit are compatible
    if ~CheckTypeAndUnits(Parameters.SigType,Parameters.SigUnit)
        % Not compatible
        warning([Parameters.SigType ' and ' Parameters.SigUnit ' are incompatible. No changes made.']);
        return
    end
    
    AvailablePMU = {PMUstruct.PMU_Name};
    
    PMUidx = find(strcmp(Parameters.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, issue warning and do nothing
    if isempty(PMUidx)
        warning(['PMU ' Parameters.PMU ' could not be found. No changes made.']);
        return
    end
    
    SigIdx = find(strcmp(Parameters.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, issue warning and do nothing
    if isempty(SigIdx)
        warning(['Signal ' Parameters.signal{SigCount}.Channel ' could not be found. No changes made.']);
        return
    end
    PMUstruct(PMUidx).Signal_Type{SigIdx} = Parameters.SigType;
    PMUstruct(PMUidx).Signal_Unit{SigIdx} = Parameters.SigUnit;
end