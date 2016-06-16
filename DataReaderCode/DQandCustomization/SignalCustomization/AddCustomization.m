function PMUstruct = AddCustomization(PMUstruct,custPMUidx,Parameters)

SignalName = Parameters.SignalName;

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
[N,NumSig,NFlags] = size(PMUstruct(custPMUidx).Flag);

AvailablePMU = {PMUstruct.PMU_Name};

NumTerms = length(Parameters.term);
if NumTerms == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.term = {Parameters.term};
end

% Add each term
CustSig = [];
SignalUnit = 'O';   % Default in case no signals are added
SignalType = 'OTHER';   % Default in case no signals are added
ErrFlag = 0; % Error flag 
for TermIdx = 1:NumTerms
    PMUidx = find(strcmp(Parameters.term{TermIdx}.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, return NaNs for the sum
    if isempty(PMUidx)
        warning(['PMU ' Parameters.term{TermIdx}.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlag = 1;
        break
    end
    
    SigIdx = find(strcmp(Parameters.term{TermIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, return NaNs for the sum
    if isempty(SigIdx)
        warning(['Signal ' Parameters.term{TermIdx}.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlag = 1;
        break
    end
    
    % Make sure dimensions match the data matrix. If not, return NaN and
    % set Flags
    if ~size(PMUstruct(PMUidx).Data(:,SigIdx),1) == N
        warning([Parameters.term{TermIdx}.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
        ErrFlag = 1;
        break
    end
    
    if isempty(CustSig)
        % This is the first term. Other terms must have the same units to 
        % be included.
        CustSig = PMUstruct(PMUidx).Data(:,SigIdx);
        FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
        SignalUnit = PMUstruct(PMUidx).Signal_Unit{SigIdx};
        SignalType = PMUstruct(PMUidx).Signal_Type{SigIdx};
    else
        % If there is disagreement between signal types, set to OTHER
        if ~strcmp(SignalType,PMUstruct(PMUidx).Signal_Type{SigIdx})
            SignalType = 'OTHER';
        end
        
        % Make sure units are consistent with the other terms. If not,
        % return NaN and set Flags
        if ~strcmp(SignalUnit,PMUstruct(PMUidx).Signal_Unit{SigIdx})
            warning(['Signal ' Parameters.term{TermIdx}.Channel ' does not have the correct units, returning NaN and setting Flags']);
            ErrFlag = 1;
            break
        end
        
        % Dimensions and units are okay, so add the signal in
        CustSig = CustSig + PMUstruct(PMUidx).Data(:,SigIdx);
        % Track flags
        FlagVec = FlagVec | (sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0);
    end
end


PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
if ErrFlag
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;    
    PMUstruct(custPMUidx).Flag(:,NumSig+1,NFlags) = true; %flagged for error in user input
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
else
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnit;
    if CheckTypeAndUnits(SignalType,SignalUnit)
        % SignalType is okay for the SignalUnit
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
    else
        % Specified SignalType was not acceptable, so set to OTHER
        warning([SignalType ' is not an acceptable signal type, setting to OTHER.']);
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
    end

    % Add the custom signal and set associated flags
    PMUstruct(custPMUidx).Data(:,NumSig+1) = CustSig;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,NFlags-1) = FlagVec;
end

