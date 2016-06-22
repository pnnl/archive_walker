% function PMU = DQfilterStep(PMU,StageStruct)
% This function carries out filter operation on PMU measurements specified
% in XML file
%
% Inputs:
	% PMU: a struct array of dimension 1 by Number of PMUs      
        % PMU(i).PMU_Name: a string specifying name of i^th PMU
        % PMU(i).Data: Matrix containing data measured by the i^th PMU
        % PMU(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged by different filter operation (size: number
        % of data points by number of channels by number of flag bits)
    % StageStruct: struct array containing information on data quality filters and customization
    % operation to be carried out for a single stage
        % StageStruct.Filter: struct array containing information on different
        % filter operation (size: 1 by number of filter
        % operation)
            % StageStruct.Filter{i}.Name: a string specifying type of i^th filter operation
            % StageStruct.Filter{i}.Parameters: a struct array containing
            % user-specified parameters for i^th filter operation
            % StageStruct.Filter{i}.PMU: struct array containing
            % information on PMUs of dimension 1 by number of PMUs
                %StageStruct.Filter{i}.PMU{j}.Name: a string specifying
                % name of j^th PMU whose data is to be filtered
                % StageStruct.Filter{i}.PMU{j}.Channel: a struct array
                % containing information on data channels in j^th PMU whose data is to be filtered
                    % StageStruct.Filter{i}.PMU{j}.Channel{k}.Name: a
                    % string specifying name of k^th data channel in j^th
                    % PMU for i^th filter operation
%
% Outputs:
    % PMU
%    
%Created by: Jim Follum(james.follum@pnnl.gov)
%Modified on June 7, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
    %1. Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix
    %2. data are set to NaN after carrying out all filter operation instead of setting data to NaN after each filter operation
    
function PMUstruct = DPUnwrap(PMUstruct,ProcessUnwrap)
NumUnwraps = length(ProcessUnwrap);

if NumUnwraps == 0
    NumPMU = length(PMUstruct.PMU);
    PMUchans = struct('ChansToFilt',cell(NumPMU,1));
    PMUstructIdx = 1:NumPMU;
    
    for PMUidx = 1:NumPMU
        PMUstruct.PMU(PMUstructIdx(PMUidx))= UnwrapAngle(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt);
    end    
end
        
if NumUnwraps == 1
    % By default, the contents of StageStruct.Filter
    % would not be in a cell array because length is one. This 
    % makes it so the same indexing can be used in the following for loop.
    ProcessUnwrap = {ProcessUnwrap};
end

for UnwrapIdx = 1:NumUnwraps 

    if isfield(ProcessUnwrap{UnwrapIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessUnwrap{UnwrapIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of StageStruct.Filter.PMU
            % would not be in a cell array because length is one. This 
            % makes it so the same indexing can be used in the following for loop.
            ProcessUnwrap{UnwrapIdx}.PMU = {ProcessUnwrap{UnwrapIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            PMUstructIdx(PMUidx) = find(strcmp(ProcessUnwrap{UnwrapIdx}.PMU{PMUidx}.Name, {PMUstruct.PMU.PMU_Name}));

            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered (appropriate channels are selected within the 
            % individual filter code)
            if isfield(ProcessUnwrap{UnwrapIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessUnwrap{UnwrapIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1
                    % By default, the contents of StageStruct.Filter.PMU.Channel
                    % would not be in a cell array because length is one. This 
                    % makes it so the same indexing can be used in the following for loop.
                    ProcessUnwrap{UnwrapIdx}.PMU{PMUidx}.Channel = {ProcessUnwrap{UnwrapIdx}.PMU{PMUidx}.Channel};
                end

                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = ProcessUnwrap{UnwrapIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name;
                end
            end
        end
    else
        % Apply this filter to all signals in all PMUs
        NumPMU = length(PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end

    for PMUidx = 1:NumPMU
        PMUstruct.PMU(PMUstructIdx(PMUidx))= UnwrapAngle(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt);
    end
    
end
