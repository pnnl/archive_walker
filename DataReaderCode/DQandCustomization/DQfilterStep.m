function PMU = DQfilterStep(PMU,StageStruct)

NumFilts = length(StageStruct.Filter);
if NumFilts == 1
    % By default, the con tents of StageStruct.Filter
    % would not be in a cell array because length is one. This 
    % makes it so the same indexing can be used in the following for loop.
    StageStruct.Filter = {StageStruct.Filter};
end
for FiltIdx = 1:NumFilts
    % Parameters for the filter - the structure contents are
    % specific to the filter
    Parameters = StageStruct.Filter{FiltIdx}.Parameters;

    if isfield(StageStruct.Filter{FiltIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(StageStruct.Filter{FiltIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of StageStruct.Filter.PMU
            % would not be in a cell array because length is one. This 
            % makes it so the same indexing can be used in the following for loop.
            StageStruct.Filter{FiltIdx}.PMU = {StageStruct.Filter{FiltIdx}.PMU};
        end

        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            PMUstructIdx(PMUidx) = find(strcmp(StageStruct.Filter{FiltIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));

            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered (appropriate channels are selected within the 
            % individual filter code)
            if isfield(StageStruct.Filter{FiltIdx}.PMU{PMUidx},'Channel')
                NumChan = length(StageStruct.Filter{FiltIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1
                    % By default, the contents of StageStruct.Filter.PMU.Channel
                    % would not be in a cell array because length is one. This 
                    % makes it so the same indexing can be used in the following for loop.
                    StageStruct.Filter{FiltIdx}.PMU{PMUidx}.Channel = {StageStruct.Filter{FiltIdx}.PMU{PMUidx}.Channel};
                end

% Should be able to remove this for loop, come back and change.
                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = StageStruct.Filter{FiltIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name;
                end
            end
        end
    else
        % Apply this filter to all signals in all PMUs
        NumPMU = length(PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end

    % Find the appropriate filter and apply it for each of the
    % specified PMUs
    switch StageStruct.Filter{FiltIdx}.Name
        case 'PMUflagFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = PMUflagFilt(PMU(PMUstructIdx(PMUidx)),Parameters);
            end
        case 'DropOutZeroFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = DropOutZeroFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'DropOutMissingFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = DropOutMissingFilt(PMU(PMUstructIdx(PMUidx)),Parameters);
            end
        case 'VoltPhasorFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = VoltPhasorFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'FreqFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = FreqFilt(PMU(PMUstructIdx(PMUidx)),Parameters);
            end
        case 'StaleFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = StaleFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'DataFrameFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = DataFrameFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'PMUchanFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = ChanFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'PMUallFilt'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx)) = EntirePMUfilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
    end    
end
PMU(PMUstructIdx(PMUidx)) = set2NaNfilt(PMU,StageStruct.Filter);