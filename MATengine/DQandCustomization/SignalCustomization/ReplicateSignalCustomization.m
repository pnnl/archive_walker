% function PMUstruct = ReplicateSignalCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)
% This function replicates signals from an existing PMU to a separate
% custom PMU.
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
    % Num_Flags: number of flags to include when creating a custom PMU
    % Parameters: structure containing user provided information to
    % create customized signal(s)      
        % Parameters.ToReplicate: a struct array containing information on 
        % signals to be replicated. (size: 1 by number of signals to be replicated)
                    % Parameters.ToReplicate{i}.PMU: a string specifying
                    % name of the PMU containing i^th signal to be
                    % replicated
                    % Parameters.ToReplicate{i}.Channel: a string specifying
                    % the channel of PMU that represents i^th signal to be replicated
    % FlagBitCust: Flag bits reserved for flagging new customized signal
        % FlagBitCust(1): Indicates error associated with user specified
        % parameters for creating a customized signal
        % FlagBitCust(2): Indicates data points in customized signal that
        % used flagged input data points 
% 
% Outputs:
    % PMUstruct: 
%     
%Created by: Jim Follum (james.follum@pnnl.gov) on September 26, 2018
function PMUstruct = ReplicateSignalCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

if isfield(Parameters,'ToReplicate')
    NumToReplicate = length(Parameters.ToReplicate);
    if NumToReplicate == 1
        % By default, the contents of Parameters.term would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        Parameters.ToReplicate = {Parameters.ToReplicate};
    end
else
    % No signals were listed to replicate
    warning('The signal replication customization was called, but no signals were listed for replication.');
    return
end

if ~isfield(Parameters,'CustPMUname')
    error('A custom PMU name for the signal replication customization was not included in the configuration file.');
end


for ToReplicateIdx = 1:NumToReplicate
    AvailablePMU = {PMUstruct.PMU_Name};
    
    if ~isfield(Parameters.ToReplicate{ToReplicateIdx},'PMU')
        warning('Source PMU not listed for signal replication customization.');
        continue
    elseif ~isfield(Parameters.ToReplicate{ToReplicateIdx},'Channel')
        warning('Source channel not listed for signal replication customization.');
        continue
    end

    % Get the index of the PMU containing the signal to be customized
    PMUidx = find(strcmp(Parameters.ToReplicate{ToReplicateIdx}.PMU,AvailablePMU));
    
    % If the specified PMU is not in PMUstruct, issue error
    if isempty(PMUidx)
        error(['Signal replication failed because PMU ' Parameters.ToReplicate{ToReplicateIdx}.PMU ' could not be found.']);
    end
    
    SigIdx = find(strcmp(Parameters.ToReplicate{ToReplicateIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, issue error
    if isempty(SigIdx)
        error(['Signal replication failed because signal ' Parameters.ToReplicate{ToReplicateIdx}.Channel ' could not be found.']);
    end

    custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
    if isempty(custPMUidx)
        % Initialize the custom PMU sub-structure and add it to the PMU structure
        % using some of the fields from an existing PMU sub-structure.
        PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(PMUidx).Signal_Time,Num_Flags);
        custPMUidx = length(PMUstruct);
    end

    % Make sure the custom signal name isn't already taken
    CheckSignalNameError(Parameters.ToReplicate{ToReplicateIdx}.Channel, PMUstruct(custPMUidx).Signal_Name);

    % Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
    [N,NcustSigs] = size(PMUstruct(custPMUidx).Data);
    
    % Check to make sure signal length is compatible
    if length(PMUstruct(PMUidx).Data(:,SigIdx)) ~= N
        error('Signal replication failed because the length of the signal does not match the custom PMU.');
    end

    % Set defaults in case of a problem with the specifications
    PMUstruct(custPMUidx).Data(:,NcustSigs+1) = PMUstruct(PMUidx).Data(:,SigIdx);
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,:) = PMUstruct(PMUidx).Flag(:,SigIdx,:);
    FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(1)) = FlagVec; %flags is set for customized signal obtained from input signal with flagged data
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = false;
    PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = PMUstruct(PMUidx).Signal_Type{SigIdx};
    PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = PMUstruct(PMUidx).Signal_Unit{SigIdx};
    PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = PMUstruct(PMUidx).Signal_Name{SigIdx};
end