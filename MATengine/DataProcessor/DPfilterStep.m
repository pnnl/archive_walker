% function PMUstruct = DPfilterStep(PMUstruct,ProcessFilter)
% This function carries out filter operation on PMU measurements specified
% in XML file
%
% Inputs:
	% PMUstruct: a struct array containing PMU structures     
        % PMUstruct.PMU.PMU_Name: a string specifying name of PMUs
        % PMUstruct.PMU(i).Data: Matrix containing data measured by the i^th PMU
        % PMUstruct.PMU(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged (size: number of data points by number of channels by number of flag bits)  
    %ProcessFilter: struct array containing information on different
    % filter operation (size: 1 by number of filter
    % operation)
        % ProcessFilter{i}.Type: a string specifying type of i^th filter operation
        % ProcessFilter.Parameters: a struct array containing
        % user-specified parameters for i^th filter operation
        % ProcessFilter{i}.PMU: struct array containing
        % information on PMUs of dimension 1 by number of PMUs
            %ProcessFilter{i}.PMU{j}.Name: a string specifying
            % name of j^th PMU whose data is to be filtered
            % ProcessFilter{i}.PMU{j}.Channel: a struct array
            % containing name of data channels in j^th PMU whose data is to be filtered
            % ProcessFilter{i}.PMU{j}.Channel{k}.Name: a
            % string specifying name of k^th data channel in j^th
            % PMU for i^th filter operation
%
% Outputs:
    % PMUstruct
%    
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)
    
function [PMU, FinalCondos] = DPfilterStep(PMU,ProcessFilter, InitialCondos)
NumFilts = length(ProcessFilter);

if NumFilts == 1
    % By default, the contents of ProcessFilter would not be in a cell array
    % because length is one. This makes it so the same indexing can be used
    % in the following for loop.
    ProcessFilter = {ProcessFilter};
end

FinalCondos = cell(1,NumFilts);
if isempty(InitialCondos)
    InitialCondos = cell(1,NumFilts);
end
for FiltIdx = 1:NumFilts 
    % Parameters for the filter - the structure contents are
    % specific to the filter
    Parameters = ProcessFilter{FiltIdx}.Parameters;
    if isfield(ProcessFilter{FiltIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessFilter{FiltIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of ProcessFilter{FiltIdx}.PMU
            % would not be in a cell array because length is one. This
            % makes it so the same indexing can be used in the following for loop.
            ProcessFilter{FiltIdx}.PMU = {ProcessFilter{FiltIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            TempIdx = find(strcmp(ProcessFilter{FiltIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));
            if ~isempty(TempIdx)
                PMUstructIdx(PMUidx) = TempIdx;
            else
                warning(['PMU ' ProcessFilter{FiltIdx}.PMU{PMUidx}.Name ' was not found.']);
            end

            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered (appropriate channels are selected within the 
            % individual filter code)
            if isfield(ProcessFilter{FiltIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1     
                    % By default, the contents of ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel 
                    % would not be in a cell array because length is one. This 
                    % makes it so the same indexing can be used in the following for loop.
                    ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel = {ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel};
                end

                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name;
                end
            end
        end
    else
        % Apply this filter to all signals in all PMUs
        NumPMU = length(PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end
    
    FinalCondos{FiltIdx} = cell(1,NumPMU);
    if isempty(InitialCondos{FiltIdx})
        InitialCondos{FiltIdx} = cell(1,NumPMU);
    end

    % Find the appropriate filter and apply it to each of the
    % specified PMUs and channels
    switch ProcessFilter{FiltIdx}.Type
        case 'HighPass'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if PMUstructIdx(PMUidx) == 0
                    continue;
                end
               [PMU(PMUstructIdx(PMUidx)), FinalCondos{FiltIdx}{PMUidx}] = HighPassFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters, InitialCondos{FiltIdx}{PMUidx});
            end
        case 'LowPass'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if PMUstructIdx(PMUidx) == 0
                    continue;
                end
                [PMU(PMUstructIdx(PMUidx)), FinalCondos{FiltIdx}{PMUidx}] = LowPassFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters, InitialCondos{FiltIdx}{PMUidx});
            end
        case 'Rational'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if PMUstructIdx(PMUidx) == 0
                    continue;
                end
                [PMU(PMUstructIdx(PMUidx)), FinalCondos{FiltIdx}{PMUidx}] = RationalFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters, InitialCondos{FiltIdx}{PMUidx});
            end
        case 'Median'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if PMUstructIdx(PMUidx) == 0
                    continue;
                end
                PMU(PMUstructIdx(PMUidx))= MedianFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
    end    
end
