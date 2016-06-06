function PMUstruct = AngleCustomization(PMUstruct,custPMUidx,Parameters)

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
[N,NcustSigs,NFlags] = size(PMUstruct(custPMUidx).Flag);

AvailablePMU = {PMUstruct.PMU_Name};

NumSigs = length(Parameters.signal);
if NumSigs == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.signal = {Parameters.signal};
end
SigMat = NaN*ones(N,NumSigs);
FlagMat = true(N,NumSigs);
SignalType = cell(1,NumSigs);
SignalName = cell(1,NumSigs);
ErrFlag = zeros(NumSigs,1);
for SigCount = 1:NumSigs
    PMUidx = find(strcmp(Parameters.signal{SigCount}.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidx)
        warning(['PMU ' Parameters.signal{SigCount}.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlag(SigCount) = 1;
        continue
    end
    
    SigIdx = find(strcmp(Parameters.signal{SigCount}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdx)
        warning(['Signal ' Parameters.signal{SigCount}.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlag(SigCount) = 1;
        continue
    end
    
    % Make sure dimensions match the data matrix. If not, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if ~size(PMUstruct(PMUidx).Data(:,SigIdx),1) == N
        warning([Parameters.signal{SigCount}.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
        ErrFlag(SigCount) = 1;
        continue
    end
    
    % Apply the operation and assign data
    SigMat(:,SigCount) = atan2(imag(PMUstruct(PMUidx).Data(:,SigIdx)),real(PMUstruct(PMUidx).Data(:,SigIdx)));
    
    % Set flags
    FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    FlagMat(:,SigCount) = FlagVec;
    
    % Store SignalType
    switch PMUstruct(PMUidx).Signal_Type{SigIdx}
        case 'VPP'
            SignalType{SigCount} = 'VAP';
        case 'VPA'
            SignalType{SigCount} = 'VAA';
        case 'VPB'
            SignalType{SigCount} = 'VAB';
        case 'VPC'
            SignalType{SigCount} = 'VAC';
        case 'IPP'
            SignalType{SigCount} = 'IAP';
        case 'IPA'
            SignalType{SigCount} = 'IAA';
        case 'IPB'
            SignalType{SigCount} = 'IAB';
        case 'IPC'
            SignalType{SigCount} = 'IAC';
        otherwise
            SignalType{SigCount} = 'OTHER';
    end
    % Store SignalName
    SignalName{SigCount} = Parameters.signal{SigCount}.CustName;
end

% Store results in custom PMU
PMUstruct(custPMUidx).Signal_Name(NcustSigs+(1:NumSigs)) = SignalName;
PMUstruct(custPMUidx).Signal_Type(NcustSigs+(1:NumSigs)) = SignalType;
SignalUnit = cell(NumSigs,1);
[SignalUnit{:}] = deal('RAD');
PMUstruct(custPMUidx).Signal_Unit(NcustSigs+(1:NumSigs)) = SignalUnit;
PMUstruct(custPMUidx).Data(:,NcustSigs+(1:NumSigs)) = SigMat;
for SigInd = 1:NumSigs
    if ErrFlag(SigInd)
        PMUstruct(custPMUidx).Flag(:,NcustSigs+SigInd,NFlags) = FlagMat(:,SigInd); %flagged for error in user input
    else
        PMUstruct(custPMUidx).Flag(:,NcustSigs+SigInd,NFlags-1) = FlagMat(:,SigInd);%flagged for flagged input signal
    end
end



