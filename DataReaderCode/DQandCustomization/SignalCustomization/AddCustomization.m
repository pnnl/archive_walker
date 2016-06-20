% function PMUstruct = AddCustomization(PMUstruct,custPMUidx,Parameters)
% This function creates a customized signal by adding given signal(s).
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
        % Parameters.term: a struct array containing information on 
        % signals required to calculate customized signals. (size: 1 by
        % number of signals to be added)
                    % Parameters.term{i}.PMU: a string specifying
                    % the name of PMU consisting of i^th signal to be added
                    % Parameters.term{i}.Channel: a string specifying
                    % the channel of PMU that represents i^th signal to be added      
    % custPMUidx: numerical identifier for PMU that would store customized signal
% 
% Outputs:
    % PMUstruct: 
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Changed the flag matrix from a 2 dimensional double matrix to a 3
%dimensional logical matrix (3rd dimension represents flag bit)
function PMUstruct = AddCustomization(PMUstruct,custPMUidx,Parameters)
SignalName = Parameters.SignalName;
CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);
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

