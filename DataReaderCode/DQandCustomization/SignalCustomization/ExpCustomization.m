function PMUstruct = ExpCustomization(PMUstruct,custPMUidx,Parameters)

FlagVal = str2num(Parameters.FlagVal);
if isempty(FlagVal)
    warning(['Flag ' Parameters.FlagVal ' could not be converted to a number. Flags will be set to NaN.']);
    FlagVal = NaN;
end

exponent = str2num(Parameters.exponent);
if isempty(exponent)
    warning(['Flag ' Parameters.exponent ' could not be converted to a number, returning NaN and setting Flags.']);
    exponent = NaN;
end

% Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
[N,NcustSigs] = size(PMUstruct(custPMUidx).Data);

AvailablePMU = {PMUstruct.PMU_Name};

NumSigs = length(Parameters.signal);
if NumSigs == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.signal = {Parameters.signal};
end

SigMat = NaN*ones(N,NumSigs);
FlagMat = FlagVal*ones(N,NumSigs);
SignalType = cell(1,NumSigs);
SignalUnit = cell(1,NumSigs);
SignalName = cell(1,NumSigs);
for SigCount = 1:NumSigs
    PMUidx = find(strcmp(Parameters.signal{SigCount}.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidx)
        warning(['PMU ' Parameters.signal{SigCount}.PMU ' could not be found, returning NaN and setting Flags.']);
        continue
    end
    
    SigIdx = find(strcmp(Parameters.signal{SigCount}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdx)
        warning(['Signal ' Parameters.signal{SigCount}.Channel ' could not be found, returning NaN and setting Flags']);
        continue
    end
    
    % Make sure dimensions match the data matrix. If not, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if ~size(PMUstruct(PMUidx).Data(:,SigIdx),1) == N
        warning([Parameters.signal{SigCount}.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
        continue
    end
    
    % Assign data and set flags to zero
    SigMat(:,SigCount) = PMUstruct(PMUidx).Data(:,SigIdx).^exponent;
    if isnan(exponent)
        FlagMat(:,SigCount) = FlagVal;
    else
        FlagMat(:,SigCount) = PMUstruct(PMUidx).Flag(:,SigIdx);
    end
    % Store SignalType
    SignalType{SigCount} = PMUstruct(PMUidx).Signal_Type{SigIdx};
    % Store SignalUnit
    SignalUnit{SigCount} = PMUstruct(PMUidx).Signal_Unit{SigIdx};
    % Store SignalName
    SignalName{SigCount} = Parameters.signal{SigCount}.CustName;
end

if isnan(exponent)
    NotScalarIdx = 1:NumSigs;
else
    NotScalarIdx = find(~((strcmp('SC',SignalType)) & (strcmp('SC',SignalUnit))));
end
SignalType(NotScalarIdx) = {'OTHER'};
SignalUnit(NotScalarIdx) = {'O'};

% Store results in custom PMU
PMUstruct(custPMUidx).Signal_Name(NcustSigs+(1:NumSigs)) = SignalName;
PMUstruct(custPMUidx).Signal_Type(NcustSigs+(1:NumSigs)) = SignalType;
PMUstruct(custPMUidx).Signal_Unit(NcustSigs+(1:NumSigs)) = SignalUnit;
PMUstruct(custPMUidx).Data(:,NcustSigs+(1:NumSigs)) = SigMat;
PMUstruct(custPMUidx).Flag(:,NcustSigs+(1:NumSigs)) = FlagMat;