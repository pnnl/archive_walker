% function PMUstruct = DPWrap(PMUstruct,ProcessWrap)
% This function performs angle wraping operation on angle measurements specified
% in XML file
%
% Inputs:
	% PMUstruct: a struct array containing PMU structures     
        % PMUstruct.PMU.PMU_Name: a string specifying name of PMUs
        % PMUstruct.PMU(i).Data: Matrix containing data measured by the i^th PMU
        % PMUstruct.PMU(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged (size: number of data points by number of channels by number of flag bits) 
    % ProcessWrap: struct array containing information on angle wraping operation
    % (size: 1 by number of filter operation)
        % ProcessFilter{i}.PMU: struct array containing information on PMUs
        % of dimension 1 by number of PMUs
            %ProcessWrap{i}.PMU{j}.Name: a string specifying
            % name of j^th PMU
            % ProcessWrap{i}.PMU{j}.Channels: a struct array
            % containing information on data channels in j^th PMU for
            % carrying out angle unwraping operation
                % ProcessWrap{i}.PMU{j}.Channels.Channel{k}.Name: a
                % string specifying name of k^th data channel in j^th
                % PMU for i^th angle wraping operation
%
% Outputs:
    % PMUstruct
%    
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMUstruct = DPWrap(PMUstruct,ProcessWrap)
NumWraps = length(ProcessWrap);

if NumWraps == 0
    NumPMU = length(PMUstruct.PMU);
    PMUchans = struct('ChansToFilt',cell(NumPMU,1));
    PMUstructIdx = 1:NumPMU;
    
    for PMUidx = 1:NumPMU
        PMUstruct.PMU(PMUstructIdx(PMUidx))= WrapAngle(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt);
    end
    
end

if NumWraps == 1
    % By default, the contents of StageStruct.Filter
    % would not be in a cell array because length is one. This 
    % makes it so the same indexing can be used in the following for loop.
    ProcessWrap = {ProcessWrap};
end

for WrapIdx = 1:NumWraps 

    if isfield(ProcessWrap{WrapIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessWrap{WrapIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of StageStruct.Filter.PMU
            % would not be in a cell array because length is one. This 
            % makes it so the same indexing can be used in the following for loop.
            ProcessWrap{WrapIdx}.PMU = {ProcessWrap{WrapIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            PMUstructIdx(PMUidx) = find(strcmp(ProcessWrap{WrapIdx}.PMU{PMUidx}.Name, {PMUstruct.PMU.PMU_Name}));

            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered (appropriate channels are selected within the 
            % individual filter code)
            if isfield(ProcessWrap{WrapIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessWrap{WrapIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1
                    % By default, the contents of StageStruct.Filter.PMU.Channel
                    % would not be in a cell array because length is one. This 
                    % makes it so the same indexing can be used in the following for loop.
                    ProcessWrap{WrapIdx}.PMU{PMUidx}.Channel = {ProcessWrap{WrapIdx}.PMU{PMUidx}.Channel};
                end

                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = ProcessWrap{WrapIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name;
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
        PMUstruct.PMU(PMUstructIdx(PMUidx))= WrapAngle(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt);
    end
    
end
