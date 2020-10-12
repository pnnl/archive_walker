% function PMUstruct = VAgraphCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)
% This function calculates a metric for system stress based on a graph
% where each edge is weighted by the difference between voltage angles at
% each vertex.
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
    % FlagBitCust: Flag bits reserved for flagging new customized signal
        % FlagBitCust(1): Indicates error associated with user specified
        % parameters for creating a customized signal
        % FlagBitCust(2): Indicates data points in customized signal that
        % used flagged input data points 
% 
% Outputs:
    % PMUstruct: 
%     
%Created by: Jim Follum (james.follum@pnnl.gov) October 8, 2020

function PMUstruct = PCAcustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

AvailablePMU = {PMUstruct.PMU_Name};

NumTerms = length(Parameters.term);
if NumTerms == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.term = {Parameters.term};
end

NumCustSigs = length(Parameters.CustomSignals);
if NumCustSigs == 1
    % By default, the contents of Parameters.NumCustSigs would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.CustomSignals = {Parameters.CustomSignals};
end

% Collect each input signal
InputData = [];
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
    ThisSig = PMUstruct(PMUidx).Data(:,SigIdx);
    ThisFlag = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    
    if isempty(InputData)
        % This is the first term.
        InputData = ThisSig;
        FlagVec = ThisFlag;
    else
        % Check dimensions
        if size(InputData,1) == length(ThisSig)
            % Dimensions are okay, so add the signal in
            InputData = [InputData ThisSig];
            % Track flags
            FlagVec = FlagVec | ThisFlag;
        else
            % Dimensions are not in agreement
            warning('Dimensions of terms in sum are not in agreement, returning NaN and setting Flags');
            ErrFlag = 1;
            break
        end
    end
end


if ErrFlag
    % Input signals caused an error
    CustSig = NaN(size(InputData,1),NumCustSigs);
else
    [~,CustSig] = pca(InputData);
    
    % CustSig returns as empty if one of the signals is all NaN. It returns
    % as NaN if all signals are NaN. Checking the size of the first
    % dimension (time) checks for both of these problems.
    if size(CustSig,1) ~= size(InputData,1)
        CustSig = NaN(size(InputData,1),NumCustSigs);
    else
        CustSig = CustSig(:,1:NumCustSigs);
    end
end


custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
if isempty(custPMUidx)
    % Initialize the custom PMU sub-structure and add it to the PMU structure
    % using some of the fields from an existing PMU sub-structure.
    RefIdx = [PMUidx 1];  % The 1 is included to prevent errors when PMUidx is empty
    PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(RefIdx(1)).Signal_Time,Num_Flags);
    custPMUidx = length(PMUstruct);
end

% Make sure the custom PMU is the appropriate size
if size(CustSig,1) ~= size(PMUstruct(custPMUidx).Data,1)
    % Dimensions do not agree
    warning('Dimensions of custom signal do not match custom PMU, returning NaN and setting flags.');
    ErrFlag = 1;
end

for OutIdx = 1:length(Parameters.CustomSignals)
    SignalName = Parameters.CustomSignals{OutIdx}.SignalName;
    CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);
    % Size of the current Data matrix for the custom PMU - N samples by NumSig signals
    NumSig = size(PMUstruct(custPMUidx).Data,2);


    PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
    if ErrFlag || (OutIdx > size(CustSig,2))
        PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;    
        PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true; %flagged for error in user input
    else
        % Add the custom signal and set associated flags
        PMUstruct(custPMUidx).Data(:,NumSig+1) = CustSig(:,OutIdx);
        PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(1)) = FlagVec;
    end
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
end