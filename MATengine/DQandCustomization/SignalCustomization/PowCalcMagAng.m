% function [Signal,Flag,SignalType,SignalUnit] = PowCalcMagAng(VmagStruct,VangStruct,ImagStruct,IangStruct,PMUstruct,custPMUidx,PowType)
% This function returns power signal calculated using given voltage magnitude, voltage angle, current magnitude and current angle signals.
% 
% Inputs:
	% VmagStruct: a struct array consisting of information on the voltage phasor signal 
        % VphasorStruct.PMU: a string specifying the name of PMU consisting of voltage magnitude signal
        % VphasorStruct.Channel: a string specifying the channel of PMU that represents voltage magnitude signal
    % VangStruct: a struct array consisting of information on the voltage phasor signal 
        % VphasorStruct.PMU: a string specifying the name of PMU consisting of voltage angle signal
        % VphasorStruct.Channel: a string specifying the channel of PMU that represents voltage angle signal
    % ImagStruct: a struct array consisting of information on the current phasor signal 
        % IphasorStruct: a string specifying the name of PMU consisting of current phasor signal
        % IphasorStruct.Channel: a string specifying the channel of PMU that represents current magnitude signal 
    % IangStruct: a struct array consisting of information on the current phasor signal 
        % IphasorStruct: a string specifying the name of PMU consisting of current phasor signal
        % IphasorStruct.Channel: a string specifying the channel of PMU that represents current angle signal 
    % PMUstruct: structure in the common format for all PMUs (size: 1 by Number
	% of PMUs)
        % PMUstruct(i).Signal_Type: a cell array containing strings
        % specifying signal(s) type in the i^th PMU
                                    %size: 1 by Number of data channel in the i^th PMU
        % PMUstruct(i).Signal_Name: a cell array containing strings
        % specifying name of signal(s) in the i^th PMU
                                    %size: 1 by Number of data channel in the i^th PMU
        % PMUstruct(i).Signal_Unit: a cell array containing strings
        % specifying unit of signal(s) in the PMU
                                    %size: 1 by Number of data channel in the i^th PMU
        % PMUstruct(i).Data: Matrix consisting of measurements by i^th PMU
                                %size: Number of data points by number of channels                              
        % PMUstruct(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged by different filter operation
                                %size: number of data points by number of channels by number of flag bits
        % PMUstruct.PMU_Name: a cell array containing strings specifying
        % name of PMUs
                                % size: Number of PMUs by 1
    % custPMUidx: numerical identifier for PMU that would store customized signal 
    % PowType: a string specifying type of power (complex, apparent, real or reactive)  
% 
% Outputs:
    % Signal: Calculated power signal measurements
    % Flag: Matrix specifying flagged data in calculated power signal
    % SignalType: a string specifying type of calculated power signal 
    % SignalUnit: a string specifying unit of calculated power signal
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix.

function [PMUstruct,ErrFlag] = PowCalcMagAng(PMUstruct,Parameters,PowIdx,Num_Flags,FlagBitCust)

% Flag is disabled if the function makes it all the way to the end and the
% new signal is added to the custom PMU.
ErrFlag = 1;

% Get the structures for each
if isfield(Parameters.power{PowIdx},'Vmag')
    VmagStruct = Parameters.power{PowIdx}.Vmag;
else
    warning('Voltage magnitude could not be found, returning NaN and setting Flags.');
    return
end
%
if isfield(Parameters.power{PowIdx},'Vang')
    VangStruct = Parameters.power{PowIdx}.Vang;
else
    warning('Voltage angle could not be found, returning NaN and setting Flags.');
    return
end
%
if isfield(Parameters.power{PowIdx},'Imag')
    ImagStruct = Parameters.power{PowIdx}.Imag;
else
    warning('Current magnitude could not be found, returning NaN and setting Flags.');
    return
end
%
if isfield(Parameters.power{PowIdx},'Iang')
    IangStruct = Parameters.power{PowIdx}.Iang;
else
    warning('Current angle could not be found, returning NaN and setting Flags.');
    return
end

AvailablePMU = {PMUstruct.PMU_Name};

Struct = {VmagStruct, VangStruct, ImagStruct, IangStruct};

SigType = cell(1,4);
SigUnit = cell(1,4);
Sigs = cell(1,4);
Flags = cell(1,4);
for StructIdx = 1:4
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
end

% Make sure signal lengths agree
if sum([length(Sigs{1}) length(Sigs{2}) length(Sigs{3})] == length(Sigs{4})) < 3
    warning('Signal lengths differ, returning NaN and setting Flags');
    return
end

% Check signal types and units
if ~strcmp(SigType{1},'VMP')
    % Unacceptable type
    warning(['Signal type ' SigType{1} ' should be VMP, returning NaN and setting Flags']);
    return
elseif ~strcmp(SigType{2},'VAP')
    % Unacceptable type
    warning(['Signal type ' SigType{2} ' should be VAP, returning NaN and setting Flags']);
    return
elseif ~strcmp(SigType{3},'IMP')
    % Unacceptable type
    warning(['Signal type ' SigType{3} ' should be IMP, returning NaN and setting Flags']);
    return
elseif ~strcmp(SigType{4},'IAP')
    % Unacceptable type
    warning(['Signal type ' SigType{4} ' should be IAP, returning NaN and setting Flags']);
    return
elseif ~sum(strcmp(SigUnit{1},{'V', 'kV'}))
    % Unacceptable unit
    warning(['Signal unit ' SigUnit{1} ' should be V or kV, returning NaN and setting Flags']);
    return
elseif ~sum(strcmp(SigUnit{2},{'DEG', 'RAD'}))
    % Unacceptable unit
    warning(['Signal unit ' SigUnit{2} ' should be DEG or RAD, returning NaN and setting Flags']);
    return
elseif ~sum(strcmp(SigUnit{3},{'A', 'kA'}))
    % Unacceptable unit
    warning(['Signal unit ' SigUnit{3} ' should be A or kA, returning NaN and setting Flags']);
    return
elseif ~sum(strcmp(SigUnit{4},{'DEG', 'RAD'}))
    % Unacceptable unit
    warning(['Signal unit ' SigUnit{4} ' should be DEG or RAD, returning NaN and setting Flags']);
    return
end

% Get a Flag vector. If any signal had a flag, it is flagged in the custom
% signal
FlagVec = (Flags{1}~=0) | (Flags{2}~=0) | (Flags{3}~=0) | (Flags{4}~=0);
% FlagVec(FlagVec==1) = FlagBit;


% Convert angles to radians if necessary
for DegIdx = find(strcmp(SigUnit,'DEG'));
    Sigs{DegIdx} = Sigs{DegIdx}*pi/180;
end

% Convert V to kV if necessary
if strcmp(SigUnit{1},'V')
    % Convert
    Sigs{1} = Sigs{1}/1000;
end

% Convert A to kA if necessary
if strcmp(SigUnit{3},'A')
    % Convert
    Sigs{3} = Sigs{3}/1000;
end

% Make phasors
Vphasor = Sigs{1} .* exp(1i*Sigs{2});
Iphasor = Sigs{3} .* exp(1i*Sigs{4});

% Calculate complex power - other power types are calculated from it
ComplexPow = 3*Vphasor.*conj(Iphasor);

switch Parameters.PowType
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
        warning(['Power type ' Parameters.PowType ' is unrecognized, returning NaN and setting Flags']);
        return
end

custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
if isempty(custPMUidx)
    % Initialize the custom PMU sub-structure and add it to the PMU structure
    % using some of the fields from an existing PMU sub-structure.
    PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(PMUidx).Signal_Time,Num_Flags);
    custPMUidx = length(PMUstruct);
end

% Make sure the custom PMU is the appropriate size
if length(Signal) ~= size(PMUstruct(custPMUidx).Data,1)
    % Dimensions do not agree
    warning('Dimensions of custom signal do not match custom PMU');
    return
end

SignalName = Parameters.power{PowIdx}.CustName;
CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
NumSig = size(PMUstruct(custPMUidx).Data,2);

% Fill in the custom PMU
PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnit;
PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
PMUstruct(custPMUidx).Data(:,NumSig+1) = Signal;
PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(1)) = Flag;

% Function completed successfully, set error flag low
ErrFlag = 0;