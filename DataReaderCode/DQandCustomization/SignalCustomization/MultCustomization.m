function PMUstruct = MultCustomization(PMUstruct,custPMUidx,Parameters)

SignalName = Parameters.SignalName;

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
[N,NumSig] = size(PMUstruct(custPMUidx).Data);

AvailablePMU = {PMUstruct.PMU_Name};

NumFactors = length(Parameters.factor);
if NumFactors == 1
    % By default, the contents of Parameters.factor would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.factor = {Parameters.factor};
end

% Multiply each factor
CustSig = [];
ErrFlag = 0;    % Error flag
for FactorIdx = 1:NumFactors
    PMUidx = find(strcmp(Parameters.factor{FactorIdx}.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, set Data to NaNs and set
    % Flags
    if isempty(PMUidx)
        warning(['PMU ' Parameters.factor{FactorIdx}.PMU ' could not be found, returning NaN and setting flags']);
        ErrFlag = 1;
        break
    end
    
    SigIdx = find(strcmp(Parameters.factor{FactorIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, return NaN and set Flags
    if isempty(SigIdx)
        warning(['Signal ' Parameters.factor{FactorIdx}.Channel ' could not be found, returning NaN and setting flags.']);
        ErrFlag = 1;
        break
    end
    
    % Make sure dimensions match the data matrix. If not, return NaN and
    % set Flags
    if ~size(PMUstruct(PMUidx).Data(:,SigIdx),1) == N
        warning([Parameters.factor{FactorIdx}.Channel ' has a different length than the custom PMU Data field, returning NaN and setting flags.']);
        ErrFlag = 1;
        break
    end
    
    if isempty(CustSig)
        % This is the first factor. Other factors must have the same units to 
        % be included.
        CustSig = PMUstruct(PMUidx).Data(:,SigIdx);
        FlagVec = PMUstruct(PMUidx).Flag(:,SigIdx) > 0;
        SignalUnit = {PMUstruct(PMUidx).Signal_Unit{SigIdx}};
        SignalType = {PMUstruct(PMUidx).Signal_Type{SigIdx}};
    else
        % Track signal units and types
        SignalUnit{length(SignalUnit)+1} = PMUstruct(PMUidx).Signal_Unit{SigIdx};
        SignalType{length(SignalType)+1} = PMUstruct(PMUidx).Signal_Type{SigIdx};
        
        % Multiply the signal in
        CustSig = CustSig .* PMUstruct(PMUidx).Data(:,SigIdx);
        
        % Check for flags
        FlagVec = FlagVec | (PMUstruct(PMUidx).Flag(:,SigIdx) > 0);
    end
end

FlagVal = str2num(Parameters.FlagVal);
if isempty(FlagVal)
    warning(['Flag ' Parameters.FlagVal ' could not be converted to a number. Flags will be set to NaN.']);
    FlagVal = NaN;
end

PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
if ErrFlag
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
    PMUstruct(custPMUidx).Flag(:,NumSig+1) = FlagVal;
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
else
    % Select the units and type for the custom signal
    SCidx = ~(strcmp(SignalUnit,'SC') & strcmp(SignalType,'SC')); % True where type and unit are not both scalar
    if sum(SCidx) == 1
        % Only one signal was not a scalar, so use those units and type
        SignalUnit = SignalUnit{SCidx};
        SignalType = SignalType{SCidx};
    else
        % More than one signal had non-scalar units, so set to OTHER
        SignalUnit = 'O';
        SignalType = 'OTHER';
    end

    if CheckTypeAndUnits(SignalType,SignalUnit)
        % SignalUnit and SignalType are acceptable
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnit;
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
    else
        % Specified SignalUnit and SignalType were not acceptable, so set to OTHER
        warning('Disagreement between signal units and type, setting to other.');
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
    end

    % Add the custom signal and flags
    PMUstruct(custPMUidx).Data(:,NumSig+1) = CustSig;
    
    FlagVec = double(FlagVec);
    FlagVec(FlagVec==1) = FlagVal;
    PMUstruct(custPMUidx).Flag(:,NumSig+1) = FlagVec;
end