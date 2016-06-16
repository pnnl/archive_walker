% Handles SignReversal, AbsVal, RealComponent, ImagComponent, and ComplexConj

function PMUstruct = PowCalcCustomization(PMUstruct,custPMUidx,Parameters)

PowType = Parameters.PowType;

AvailablePMU = {PMUstruct.PMU_Name};

NumPow = length(Parameters.power);
if NumPow == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.power = {Parameters.power};
end

for PowIdx = 1:NumPow
    ErrFlag = 0; % Error flag
    for junkIdx = 1:1  % Allows break from nested switch
    switch length(fieldnames(Parameters.power{PowIdx}))
        case 3
            % Should be a voltage phasor - current phasor pair
            % Get the structures for each

            if isfield(Parameters.power{PowIdx},'Vphasor')
                VphasorStruct = Parameters.power{PowIdx}.Vphasor;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Iphasor')
                IphasorStruct = Parameters.power{PowIdx}.Iphasor;
            else
                ErrFlag = 1;
                break
            end
            
            [Signal,Flag,SignalType,SignalUnit] = PowCalcPhasor(VphasorStruct,IphasorStruct,PMUstruct,custPMUidx,PowType);
        case 5
            % Should be voltage mag/ang - current mag/ang pair
            % Get the structures for each

            if isfield(Parameters.power{PowIdx},'Vmag')
                VmagStruct = Parameters.power{PowIdx}.Vmag;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Vang')
                VangStruct = Parameters.power{PowIdx}.Vang;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Imag')
                ImagStruct = Parameters.power{PowIdx}.Imag;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Iang')
                IangStruct = Parameters.power{PowIdx}.Iang;
            else
                ErrFlag = 1;
                break
            end
            
            [Signal,Flag,SignalType,SignalUnit] = PowCalcMagAng(VmagStruct,VangStruct,ImagStruct,IangStruct,PMUstruct,custPMUidx,PowType);
        otherwise
            ErrFlag = 1;
    end
    end
    
    % Size of the current Data matrix and number of flags for the custom PMU - N samples by NcustSigs signals by NFLags flags
    [N,NcustSigs,NFlags] = size(PMUstruct(custPMUidx).Flag);
    
    % Check error flag
    if ErrFlag
        warning('Voltages and currents were not specified properly, returning NaN and setting Flags.');
        SignalType = 'OTHER';
        SignalUnit = 'O';
        Signal = NaN*ones(N,1);
        Flag = true(N,1);
    end
    
    
    % Store results in custom PMU
    PMUstruct(custPMUidx).Data(:,NcustSigs+1) = Signal;
    if strcmp(SignalType, 'OTHER')
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = Flag; %indicating error in user input
    else
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags-1) = Flag; %indicating error in input signal
    end
    PMUstruct(custPMUidx).Signal_Name(NcustSigs+1) = {Parameters.power{PowIdx}.CustName};
    PMUstruct(custPMUidx).Signal_Type(NcustSigs+1) = {SignalType};
    PMUstruct(custPMUidx).Signal_Unit(NcustSigs+1) = {SignalUnit};
end