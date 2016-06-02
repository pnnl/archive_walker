function PMUstruct = PrefixCustomization(PMUstruct,custPMUidx,Parameters)

AvailablePMU = {PMUstruct.PMU_Name};

NumToConvert = length(Parameters.ToConvert);
if NumToConvert == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.ToConvert = {Parameters.ToConvert};
end

for ToConvertIdx = 1:NumToConvert
    PMUidx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, issue warning and do nothing
    if isempty(PMUidx)
        warning(['PMU ' Parameters.ToConvert{ToConvertIdx}.PMU ' could not be found, no change made.']);
        continue
    end
    
    SigIdx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdx)
        warning(['Signal ' Parameters.ToConvert{ToConvertIdx}.Channel ' could not be found, no change made.']);
        continue
    end
    
    % This coding approach is bulky, but it will allow for expansion to
    % more than two unit options for each type.
    NewUnit = Parameters.ToConvert{ToConvertIdx}.NewUnit;
    ErrFlagIdentical = 0;
    ErrFlagIncompatible = 0;
    switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
        case 'V'
            % From V to NewUnit
            if strcmp(NewUnit,'kV')
                PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
            elseif strcmp(NewUnit,'V')
                ErrFlagIdentical = 1;
            else
                ErrFlagIncompatible = 1;
            end
        case 'kV'
            % From kV to NewUnit
            if strcmp(NewUnit,'V')
                PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
            elseif strcmp(NewUnit,'kV')
                ErrFlagIdentical = 1;
            else
                ErrFlagIncompatible = 1;
            end
        case 'A'
            % From A to NewUnit
            if strcmp(NewUnit,'kA')
                PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
            elseif strcmp(NewUnit,'A')
                ErrFlagIdentical = 1;
            else
                ErrFlagIncompatible = 1;
            end
        case 'kA'
            % From kA to NewUnit
            if strcmp(NewUnit,'A')
                PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
            elseif strcmp(NewUnit,'kA')
                ErrFlagIdentical = 1;
            else
                ErrFlagIncompatible = 1;
            end
        case 'mHz/sec'
            % From Hz/sec to NewUnit
            if strcmp(NewUnit,'Hz/sec')
                PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
            elseif strcmp(NewUnit,'mHz/sec')
                ErrFlagIdentical = 1;
            else
                ErrFlagIncompatible = 1;
            end
        case 'Hz/sec'
            % From mHz/sec to NewUnit
            if strcmp(NewUnit,'mHz/sec')
                PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
            elseif strcmp(NewUnit,'Hz/sec')
                ErrFlagIdentical = 1;
            else
                ErrFlagIncompatible = 1;
            end
        otherwise
            warning([PMUstruct(PMUidx).Signal_Unit{SigIdx} ' is an unacceptable input unit, no change made.']);
            continue
    end
    
    if ErrFlagIdentical
        warning(['Old and new unit for ' Parameters.ToConvert{ToConvertIdx}.Channel ' are identical.']);
        continue
    end
    
    if ErrFlagIncompatible
        warning(['Old and new unit for ' Parameters.ToConvert{ToConvertIdx}.Channel ' are incompatible, no change made.']);
        continue
    end
    
    % Assign the new unit
    
    
    % All checks passed, set type and unit for input signal or create a new
    % custom signal with the type and unit
    if isfield(Parameters.ToConvert{ToConvertIdx},'CustName')
        % A custom signal name was provided, so instead of replacing the input
        % signal add a new signal to the custom signal PMU
        
        % Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
        [~,NcustSigs] = size(PMUstruct(custPMUidx).Data);
        
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = PMUstruct(PMUidx).Data(:,SigIdx);
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1) = PMUstruct(PMUidx).Flag(:,SigIdx);
        PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = PMUstruct(PMUidx).Signal_Type{SigIdx};
        PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = Parameters.ToConvert{ToConvertIdx}.NewUnit;
        PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = Parameters.ToConvert{ToConvertIdx}.CustName;
    else
        % A custom signal name was not provided, so replace the input signal's unit
        PMUstruct(PMUidx).Signal_Unit{SigIdx} = Parameters.ToConvert{ToConvertIdx}.NewUnit;
    end
end