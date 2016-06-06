function PMUstruct = AngleUnitCustomization(PMUstruct,custPMUidx,Parameters)

AvailablePMU = {PMUstruct.PMU_Name};

NumToConvert = length(Parameters.ToConvert);
if NumToConvert == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.ToConvert = {Parameters.ToConvert};
end

for ToConvertIdx = 1:NumToConvert
    % Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
    [~,NcustSigs,NFlags] = size(PMUstruct(custPMUidx).Flag);
    
    if isfield(Parameters.ToConvert{ToConvertIdx},'CustName')
        % A custom signal name was provided, so instead of replacing the input
        % signal add a new signal to the custom signal PMU
        PMUidx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.PMU,AvailablePMU));
        % If the specified PMU is not in PMUstruct, issue warning
        if isempty(PMUidx)
            warning(['PMU ' Parameters.ToConvert{ToConvertIdx}.PMU ' could not be found. Values were set to NaN and Flags set.']);
            PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
            PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = true; %flags is set for customized signal for error in user input
            continue
        end
        
        SigIdx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
        % If the specified signal is not in PMUstruct, issue warning and do nothing
        if isempty(SigIdx)
            warning(['Signal ' Parameters.ToConvert{ToConvertIdx}.Channel ' could not be found. Values were set to NaN and Flags set.']);
            PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
            PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = true; %flags is set for customized signal for error in user input
            continue
        end
        switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
            case 'DEG'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*pi/180;
                NewUnit = 'RAD';
            case 'RAD'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*180/pi;
                NewUnit = 'DEG';
            otherwise
                warning([PMUstruct(PMUidx).Signal_Unit{SigIdx} ' is not an appropriate unit. Values were set to NaN and Flags set.']);
                PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
                PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = true; %flags is set for customized signal for error in user input
                continue
        end
        
        % A custom signal name was provided, so instead of replacing the input
        % signal add a new signal to the custom signal PMU
        
        % Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
        
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NewAng;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,:) = PMUstruct(PMUidx).Flag(:,SigIdx,:);
        FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags-1) = FlagVec; %flags is set for customized signal obtained from input signal with flagged data
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = false;
        PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = PMUstruct(PMUidx).Signal_Type{SigIdx};
        PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = NewUnit;
        PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = Parameters.ToConvert{ToConvertIdx}.CustName;
    else
        
        PMUidx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.PMU,AvailablePMU));
         if isempty(PMUidx)
            warning(['PMU ' Parameters.ToConvert{ToConvertIdx}.PMU ' could not be found. No changes made']);
            continue
        end
        
        SigIdx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
        % If the specified signal is not in PMUstruct, issue warning and do nothing
        if isempty(SigIdx)
            warning(['Signal ' Parameters.ToConvert{ToConvertIdx}.Channel ' could not be found. No changes made']);
            continue
        end
        switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
            case 'DEG'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*pi/180;
                NewUnit = 'RAD';
            case 'RAD'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*180/pi;
                NewUnit = 'DEG';
            otherwise
                warning([PMUstruct(PMUidx).Signal_Unit{SigIdx} ' is not an appropriate unit. No changes made']);
            continue
        end
        % A custom signal name was not provided, so replace the input signal and its unit
        PMUstruct(PMUidx).Data(:,SigIdx) = NewAng;
        PMUstruct(PMUidx).Signal_Unit{SigIdx} = NewUnit;
    end
end