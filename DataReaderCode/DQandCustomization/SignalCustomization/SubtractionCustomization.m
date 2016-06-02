function PMUstruct = SubtractionCustomization(PMUstruct,custPMUidx,Parameters)

SignalName = Parameters.SignalName;
FlagVal = str2num(Parameters.FlagVal);
if isempty(FlagVal)
    warning(['Flag ' Parameters.FlagVal ' could not be converted to a number. Flags will be set to NaN.']);
    FlagVal = NaN;
end

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
[N,NumSig] = size(PMUstruct(custPMUidx).Data);

AvailablePMU = {PMUstruct.PMU_Name};

% Get Minuend (what you subtract from)
PMUidxMin = find(strcmp(Parameters.minuend.PMU,AvailablePMU));
if ~isempty(PMUidxMin)
    SigIdxMin = find(strcmp(Parameters.minuend.Channel,PMUstruct(PMUidxMin).Signal_Name));
end

% Get subtrahend (what you subtract)
PMUidxSub = find(strcmp(Parameters.subtrahend.PMU,AvailablePMU));
if ~isempty(PMUidxSub)
    SigIdxSub = find(strcmp(Parameters.subtrahend.Channel,PMUstruct(PMUidxSub).Signal_Name));
end

PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
if (~isempty(SigIdxMin)) && (~isempty(SigIdxSub))
    SignalUnitMin = PMUstruct(PMUidxMin).Signal_Unit{SigIdxMin};
    SignalTypeMin = PMUstruct(PMUidxMin).Signal_Type{SigIdxMin};
    SignalUnitSub = PMUstruct(PMUidxSub).Signal_Unit{SigIdxSub};
    SignalTypeSub = PMUstruct(PMUidxSub).Signal_Type{SigIdxSub};
    
    if strcmp(SignalTypeMin,SignalTypeSub)
        % Signal types agree, keep them
        SignalType = SignalTypeMin;
    else
        % Signal types disagree, set to OTHER
        SignalType = 'OTHER';
    end
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
    
    % Check if signal units agree
    if strcmp(SignalUnitMin,SignalUnitSub)
        % Units agree
        
        % Assign custom signal
        CustSig = PMUstruct(PMUidxMin).Data(:,SigIdxMin) - PMUstruct(PMUidxSub).Data(:,SigIdxSub);
        PMUstruct(custPMUidx).Data(:,NumSig+1) = CustSig;
        
        % Set flags
        FlagVec = double((PMUstruct(PMUidxMin).Flag(:,SigIdxMin) > 0) | (PMUstruct(PMUidxSub).Flag(:,SigIdxSub) > 0));
        FlagVec(FlagVec==1) = FlagVal;
        PMUstruct(custPMUidx).Flag(:,NumSig+1) = FlagVec;
        
        % Set units
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnitMin;
    else
        % Units do not agree
        warning('Signal units did not agree. Values were set to NaN and Flags set.');
        PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NumSig+1) = FlagVal;
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    end
else
    % Signals were not found
    warning('Signals were not found. Values were set to NaN and Flags set.');
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
    PMUstruct(custPMUidx).Flag(:,NumSig+1) = FlagVal;
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
end