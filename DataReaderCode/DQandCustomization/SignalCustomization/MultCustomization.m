% function PMUstruct = MultCustomization(PMUstruct,custPMUidx,Parameters)
% This function creates a customized signal by multiplying given signal(s)
% 
% Inputs:
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
    % Parameters: structure containing user provided information to
    % create a customized signal
        % Parameters.SignalName: a string specifying name for the
        % customized signal
        % Parameters.factor: a struct array consisting of information on 
        % signals required to create customized signal. (size: 1 by
        % number of signals to be multiplied)
                    % Parameters.factor{i}.PMU: a string specifying
                    % the name of PMU consisting of i^th signal to be multiplied
                    % Parameters.factor{i}.Channel: a string specifying
                    % the channel of PMU that represents i^th signal to be multiplied      
    % custPMUidx: numerical identifier for PMU that would store customized signal
% 
% Outputs:
    % PMUstruct
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Changed the flag matrix from a 2 dimensional double matrix to a 3
%dimensional logical matrix (3rd dimension represents flag bit)

function PMUstruct = MultCustomization(PMUstruct,custPMUidx,Parameters)

SignalName = Parameters.SignalName;
CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
[N,NumSig,NFlags] = size(PMUstruct(custPMUidx).Flag);
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
        FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
        SignalUnit = {PMUstruct(PMUidx).Signal_Unit{SigIdx}};
        SignalType = {PMUstruct(PMUidx).Signal_Type{SigIdx}};
    else
        % Track signal units and types
        SignalUnit{length(SignalUnit)+1} = PMUstruct(PMUidx).Signal_Unit{SigIdx};
        SignalType{length(SignalType)+1} = PMUstruct(PMUidx).Signal_Type{SigIdx};
        
        % Multiply the signal in
        CustSig = CustSig .* PMUstruct(PMUidx).Data(:,SigIdx);
        
        % Check for flags
        FlagVec = FlagVec | (sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0);
    end
end

PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
if ErrFlag
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,NFlags) = true; %flagged fpr error in user input
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
    
    PMUstruct(custPMUidx).Flag(:,NumSig+1,NFlags-1)= FlagVec; %flagged for error with input signal 
end