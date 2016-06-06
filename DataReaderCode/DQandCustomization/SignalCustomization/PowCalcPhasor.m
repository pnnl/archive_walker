function [Signal,Flag,SignalType,SignalUnit] = PowCalcPhasor(VphasorStruct,IphasorStruct,PMUstruct,custPMUidx,PowType)

% Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
[N,~] = size(PMUstruct(custPMUidx).Data);

% If a problem is detected, the function returns these default values
Signal = NaN*ones(N,1);
Flag = true(N,1);
SignalType = 'OTHER';
SignalUnit = 'O';

AvailablePMU = {PMUstruct.PMU_Name};

Struct = {VphasorStruct, IphasorStruct};

SigType = cell(1,2);
SigUnit = cell(1,2);
Sigs = cell(1,2);
Flags = cell(1,2);
for StructIdx = 1:2
    ThisStruct = Struct{StructIdx};
    PMUidx = find(strcmp(ThisStruct.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, set the error flag and skip 
    % the rest of the for loop
    if isempty(PMUidx)
        warning(['PMU ' ThisStruct.PMU ' could not be found, returning NaN and setting Flags.']);
        return
    end
    
    SigIdx = find(strcmp(ThisStruct.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, set the error flag and skip 
    % the rest of the for loop
    if isempty(SigIdx)
        warning(['Signal ' ThisStruct.Channel ' could not be found, returning NaN and setting Flags']);
        return
    end
    
    SigType(StructIdx) = PMUstruct(PMUidx).Signal_Type(SigIdx);
    SigUnit(StructIdx) = PMUstruct(PMUidx).Signal_Unit(SigIdx);
    Sigs{StructIdx} = PMUstruct(PMUidx).Data(:,SigIdx);
    Flags{StructIdx} = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3);
    
    % Make sure signal length is appropriate
    if length(Sigs{StructIdx})~=N
        warning('Signal has a different length than the custom PMU Data field, returning NaN and setting Flags');
        return
    end
end

% Check signal types and units
if ~strcmp(SigType{1},'VPP')
    % Unacceptable type
    warning(['Signal type ' SigType{1} ' should be VPP, returning NaN and setting Flags']);
    return
elseif ~strcmp(SigType{2},'IPP')
    % Unacceptable type
    warning(['Signal type ' SigType{2} ' should be IPP, returning NaN and setting Flags']);
    return
elseif ~sum(strcmp(SigUnit{1},{'V', 'kV'}))
    % Unacceptable unit
    warning(['Signal unit ' SigUnit{1} ' should be V or kV, returning NaN and setting Flags']);
    return
elseif ~sum(strcmp(SigUnit{2},{'A', 'kA'}))
    % Unacceptable unit
    warning(['Signal unit ' SigUnit{2} ' should be A or kA, returning NaN and setting Flags']);
    return
end

% Get a Flag vector. If any signal had a flag, it is flagged in the custom
% signal
FlagVec = (Flags{1}~=0) | (Flags{2}~=0);

% Convert V to kV if necessary
if strcmp(SigUnit{1},'V')
    % Convert
    Sigs{1} = Sigs{1}/1000;
    SigUnit{1} = 'kV';
end

% Convert A to kA if necessary
if strcmp(SigUnit{2},'A')
    % Convert
    Sigs{2} = Sigs{2}/1000;
    SigUnit{2} = 'kA';
end

% Calculate complex power - other power types are calculated from it
ComplexPow = 3*Sigs{1}.*conj(Sigs{2});

switch PowType
    case 'Complex'
        Signal = ComplexPow;
        Flag = FlagVec;
        SignalType = 'CP';
        SignalUnit = 'MVA';
    case 'Apparent'
        Signal = abs(ComplexPow);
        Flag = FlagVec;
        SignalType = 'S';
        SignalUnit = 'MVA';
    case 'Active'
        Signal = real(ComplexPow);
        Flag = FlagVec;
        SignalType = 'P';
        SignalUnit = 'MW';
    case 'Reactive'
        Signal = imag(ComplexPow);
        Flag = FlagVec;
        SignalType = 'Q';
        SignalUnit = 'MVAR';
    otherwise
        warning(['Power type ' PowType ' is unrecognized, returning NaN and setting Flags']);
        return
end