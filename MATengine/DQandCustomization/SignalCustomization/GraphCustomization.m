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

function PMUstruct = GraphCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

AvailablePMU = {PMUstruct.PMU_Name};

NumTerms = length(Parameters.term);
if NumTerms == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.term = {Parameters.term};
end

% Collect each input signal
InputData = [];
ErrFlag = 0; % Error flag
AngleInputs = true;
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
    
    switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
        case 'DEG'
            ThisSig = PMUstruct(PMUidx).Data(:,SigIdx)*pi/180;
            ThisFlag = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
        case 'RAD'
            ThisSig = PMUstruct(PMUidx).Data(:,SigIdx);
            ThisFlag = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
        otherwise
            AngleInputs = false;
            ThisSig = PMUstruct(PMUidx).Data(:,SigIdx);
            ThisFlag = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    end
    
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
    CustSig = NaN(size(InputData,1),1);
else
    % This unwrapping operation keeps the voltage angles from different PMUs at
    % a particular time close together (along dimension 2)
    if AngleInputs
        InputData = unwrap(InputData,[],2);
    end
    
    % Calculate the eigenvalue based on the form of the A matrix
    CustSig = zeros(size(InputData,1),1);
    for k = 1:NumTerms
        for r = k:NumTerms
            if k == r
                CustSig = CustSig - (NumTerms-1)*InputData(:,k).^2;
            else
                CustSig = CustSig + 2*InputData(:,k).*InputData(:,r);
            end
        end
    end
    CustSig = imag(sqrt(CustSig));
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
if length(CustSig) ~= size(PMUstruct(custPMUidx).Data,1)
    % Dimensions do not agree
    warning('Dimensions of custom signal do not match custom PMU, returning NaN and setting flags.');
    ErrFlag = 1;
end

SignalName = Parameters.SignalName;
CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);
% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
NumSig = size(PMUstruct(custPMUidx).Data,2);


PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
if ErrFlag
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;    
    PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true; %flagged for error in user input
else
    % Add the custom signal and set associated flags
    PMUstruct(custPMUidx).Data(:,NumSig+1) = CustSig;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(1)) = FlagVec;
end
PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';