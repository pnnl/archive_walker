% function PMUstruct = DPUnwrap(PMUstruct,ProcessWrap)
% This function performs angle unwraping operation on angle measurements specified
% in XML file
%
% Inputs:
	% PMUstruct: a struct array containing PMU structures     
        % PMUstruct.PMU.PMU_Name: a cell array of string specifying name of PMUs
        % PMUstruct.PMU(i).Data: Matrix containing data measured by the i^th PMU
        % PMUstruct.PMU(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged(size: number of data points by number of
        % channels by number of flag bits)
    % ProcessUnwrap: struct array containing information on angle unwraping operation
    % (size: 1 by number of angle unwrap operation)
        % ProcessUnwrap{i}.PMU: struct array containing information on PMUs
        % of dimension 1 by number of PMUs
            %ProcessUnwrap{i}.PMU{j}.Name: a string specifying
            % name of j^th PMU 
            % ProcessUnwrap{i}.PMU{j}.Channels: a struct array
            % containing information on data channels in j^th PMU for
            % carrying out angle unwraping operation
                % ProcessUnwrap{i}.PMU{j}.Channels.Channel{k}.Name: a
                % string specifying name of k^th data channel in j^th
                % PMU for i^th angle unwraping operation
%
% Outputs:
    % PMUstruct
%    
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)
    
function [PMU, FinalAngles] = DPUnwrap(PMU,ProcessUnwrap,PastAngles)

%Gives number of angle unwraping operation specified in XML file
NumUnwraps = length(ProcessUnwrap);

if NumUnwraps == 1
    % By default, the contents of ProcessUnwrap would not be in a cell array
    % because length is one. This makes it so the same indexing can be used
    % in the following for loop.
    ProcessUnwrap = {ProcessUnwrap};
end

FinalAngles = cell(1,NumUnwraps);
if isempty(PastAngles)
    PastAngles = cell(1,NumUnwraps);
end
for UnwrapIdx = 1:NumUnwraps 
    if isfield(ProcessUnwrap{UnwrapIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessUnwrap{UnwrapIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of ProcessUnwrap{UnwrapIdx}.PMU would
            % not be in a cell array because length is one. This makes it
            % so the same indexing can be used in the following for loop.
            ProcessUnwrap{UnwrapIdx}.PMU = {ProcessUnwrap{UnwrapIdx}.PMU};
        end

        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed
            PMUstructIdx(PMUidx) = find(strcmp(ProcessUnwrap{UnwrapIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));

            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered
            if isfield(ProcessUnwrap{UnwrapIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessUnwrap{UnwrapIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1
                    % By default, the contents of ProcessUnwrap{WrapIdx}.PMU{PMUidx}.Channel
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
        % Perform angle wraping operation on all angle signals in all PMUs
        NumPMU = length(PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end

    MaxNaN = str2double(ProcessUnwrap{UnwrapIdx}.MaxNaN);

    % Apply angle wraping operation to each of the specified PMUs and channels
    FinalAngles{UnwrapIdx} = cell(1,NumPMU);
    for PMUidx = 1:NumPMU
        FinalAngles{UnwrapIdx}{PMUidx} = struct('PMUname',[],'Signals',[]);
    end
    if isempty(PastAngles{UnwrapIdx})
        PastAngles{UnwrapIdx} = cell(1,NumPMU);
        for PMUidx = 1:NumPMU
            PastAngles{UnwrapIdx}{PMUidx} = struct('PMUname',[],'Signals',[]);
        end
    end
    for PMUidx = 1:NumPMU
        if ~strcmp(PastAngles{UnwrapIdx}{PMUidx}.PMUname, PMU(PMUstructIdx(PMUidx)).PMU_Name)
            PastAngles{UnwrapIdx}{PMUidx}.Signals = [];
        end

        FinalAngles{UnwrapIdx}{PMUidx}.PMUname = PMU(PMUstructIdx(PMUidx)).PMU_Name;
        [PMU(PMUstructIdx(PMUidx)), FinalAngles{UnwrapIdx}{PMUidx}.Signals] = UnwrapAngle(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt, PastAngles{UnwrapIdx}{PMUidx}.Signals, MaxNaN);
    end    
end