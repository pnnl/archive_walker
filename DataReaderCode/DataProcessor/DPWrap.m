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
    % (size: 1 by number of angle wraping operation)
        % ProcessFilter{i}.PMU: struct array containing information on PMUs
        % of dimension 1 by number of PMUs
            %ProcessWrap{i}.PMU{j}.Name: a string specifying
            % name of j^th PMU
            % ProcessWrap{i}.PMU{j}.Channels: a struct array
            % containing information on data channels in j^th PMU for
            % carrying out angle wraping operation
                % ProcessWrap{i}.PMU{j}.Channels.Channel{k}.Name: a
                % string specifying name of k^th data channel in j^th
                % PMU for i^th angle wraping operation
%
% Outputs:
    % PMUstruct
%    
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMU = DPWrap(PMU,ProcessWrap)
NumWraps = length(ProcessWrap);

if NumWraps == 0 % Perform angle wraping operation on all signals in all PMUs
    NumPMU = length(PMU);
    PMUchans = struct('ChansToFilt',cell(NumPMU,1));
    PMUstructIdx = 1:NumPMU;
    
    for PMUidx = 1:NumPMU
        PMU(PMUstructIdx(PMUidx))= WrapAngle(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt);
    end
    
end

if NumWraps == 1
    ProcessWrap = {ProcessWrap};
end

for WrapIdx = 1:NumWraps 

    if isfield(ProcessWrap{WrapIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessWrap{WrapIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of ProcessWrap{WrapIdx}.PMU would
            % not be in a cell array because length is one. This makes it
            % so the same indexing can be used in the following for loop.
            ProcessWrap{WrapIdx}.PMU = {ProcessWrap{WrapIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed
            PMUstructIdx(PMUidx) = find(strcmp(ProcessWrap{WrapIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));

            if isfield(ProcessWrap{WrapIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessWrap{WrapIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1
                    % By default, the contents of ProcessWrap{WrapIdx}.PMU{PMUidx}.Channel
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
       PMU(PMUstructIdx(PMUidx))= WrapAngle(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt);
    end
    
end
