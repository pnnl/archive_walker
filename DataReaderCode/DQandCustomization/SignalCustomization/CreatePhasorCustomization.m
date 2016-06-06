function PMUstruct = CreatePhasorCustomization(PMUstruct,custPMUidx,Parameters)

% Size of the current Data matrix for the custom PMU - N samples by NumSig
% signals by NFlags flags
[N,NcustSigs,NFlags] = size(PMUstruct(custPMUidx).Flag);
AvailablePMU = {PMUstruct.PMU_Name};

NumPhasor = length(Parameters.phasor);
if NumPhasor == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.phasor = {Parameters.phasor};
end
% [~,~,NumFlag] = size(PMUstruct(custPMUidx).Flag);
SigMat = NaN*ones(N,NumPhasor);
FlagMat = true(N,NumPhasor);
SignalType = cell(1,NumPhasor);
SignalUnit = cell(1,NumPhasor);
SignalName = cell(1,NumPhasor);
ErrFlagUser = zeros(NumPhasor,1);
for PhasorIdx = 1:NumPhasor
    PMUidxMag = find(strcmp(Parameters.phasor{PhasorIdx}.mag.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidxMag)
        warning(['PMU ' Parameters.phasor{PhasorIdx}.mag.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    PMUidxAng = find(strcmp(Parameters.phasor{PhasorIdx}.ang.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidxAng)
        warning(['PMU ' Parameters.phasor{PhasorIdx}.ang.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    SigIdxMag = find(strcmp(Parameters.phasor{PhasorIdx}.mag.Channel,PMUstruct(PMUidxMag).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdxMag)
        warning(['Signal ' Parameters.phasor{PhasorIdx}.mag.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    SigIdxAng = find(strcmp(Parameters.phasor{PhasorIdx}.ang.Channel,PMUstruct(PMUidxMag).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdxAng)
        warning(['Signal ' Parameters.phasor{PhasorIdx}.ang.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    % Make sure dimensions match the data matrix. If not, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if ~size(PMUstruct(PMUidxMag).Data(:,SigIdxMag),1) == N
        warning([Parameters.phasor{PhasorIdx}.mag.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    % Make sure dimensions match the data matrix. If not, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if ~size(PMUstruct(PMUidxAng).Data(:,SigIdxAng),1) == N
        warning([Parameters.phasor{PhasorIdx}.ang.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    % Make sure types make sense
    ErrFlag = 0;
    switch PMUstruct(PMUidxMag).Signal_Type{SigIdxMag}
        case 'VMP'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAP')
                SignalType{PhasorIdx} = 'VPP';
            else
                ErrFlag = 1;
            end
        case 'VMA'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAA')
                SignalType{PhasorIdx} = 'VPA';
            else
                ErrFlag = 1;
            end
        case 'VMB'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAB')
                SignalType{PhasorIdx} = 'VPB';
            else
                ErrFlag = 1;
            end
        case 'VMC'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAC')
                SignalType{PhasorIdx} = 'VPC';
            else
                ErrFlag = 1;
            end
        case 'IMP'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAP')
                SignalType{PhasorIdx} = 'IPP';
            else
                ErrFlag = 1;
            end
        case 'IMA'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAA')
                SignalType{PhasorIdx} = 'IPA';
            else
                ErrFlag = 1;
            end
        case 'IMB'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAB')
                SignalType{PhasorIdx} = 'IPB';
            else
                ErrFlag = 1;
            end
        case 'IMC'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAC')
                SignalType{PhasorIdx} = 'IPC';
            else
                ErrFlag = 1;
            end
        otherwise
            % Magnitude type is not acceptable, skip the rest of the for
            % loop so that Data remains NaNs and Flags remain set.
            warning([Parameters.phasor{PhasorIdx}.mag.Channel ' is not a voltage or current magnitude, returning NaN and setting Flags']);
            ErrFlagUser(PhasorIdx) = 1;
            continue
    end
    if ErrFlag
        % Angle type is not acceptable, skip the rest of the for
        % loop so that Data remains NaNs and Flags remain set.
        warning([Parameters.phasor{PhasorIdx}.ang.Channel ' is not a voltage or current angle or does not match magnitude, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    AngleUnits = PMUstruct(PMUidxAng).Signal_Unit{SigIdxAng};
    switch AngleUnits
        case 'DEG'
            % Convert to radians
            SigAngle = (PMUstruct(PMUidxAng).Data(:,SigIdxAng))*pi/180;
            FlagVec = sum(PMUstruct(PMUidxAng).Flag(:,SigIdxAng,:),3);
        case 'RAD'
            % Angle is already in radians, as desired
            SigAngle = PMUstruct(PMUidxAng).Data(:,SigIdxAng);
            FlagVec = sum(PMUstruct(PMUidxAng).Flag(:,SigIdxAng,:),3);
        otherwise
            % Units are not correct, skip the rest of the for loop so that
            % the Data remains NaNs and Flags remain set.
            warning([Parameters.phasor{PhasorIdx}.ang.Channel ' does not have units of RAD or DEG, returning NaN and setting Flags.']);
            ErrFlagUser(PhasorIdx) = 1;
            continue
    end
    
    SigMat(:,PhasorIdx) = PMUstruct(PMUidxMag).Data(:,SigIdxMag) .* exp(1i*SigAngle);
    
    FlagMat(:,PhasorIdx) = (FlagVec>0) | (sum(PMUstruct(PMUidxMag).Flag(:,SigIdxMag,:),3) > 0);   % Set flags 

    % Store SignalUnit
    SignalUnit{PhasorIdx} = PMUstruct(PMUidxMag).Signal_Unit{SigIdxMag};
    % Store SignalName
    SignalName{PhasorIdx} = Parameters.phasor{PhasorIdx}.CustName;
end

% Store results in custom PMU
PMUstruct(custPMUidx).Signal_Name(NcustSigs+(1:NumPhasor)) = SignalName;
PMUstruct(custPMUidx).Signal_Type(NcustSigs+(1:NumPhasor)) = SignalType;
PMUstruct(custPMUidx).Signal_Unit(NcustSigs+(1:NumPhasor)) = SignalUnit;
PMUstruct(custPMUidx).Data(:,NcustSigs+(1:NumPhasor)) = SigMat;
for PhasorIndex = 1:NumPhasor
    if ErrFlagUser(PhasorIdx)
        PMUstruct(custPMUidx).Flag(:,NcustSigs+PhasorIndex,NFlags) = FlagMat(:,PhasorIndex);
    else
        PMUstruct(custPMUidx).Flag(:,NcustSigs+PhasorIndex,NFlags-1) = FlagMat(:,PhasorIndex);
    end
end
        
    
        
        
        
        
        
        
        
        
        