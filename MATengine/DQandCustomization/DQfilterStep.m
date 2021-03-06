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
    % FileType: Data file type (csv or PDAT)
%
% Outputs:
    % PMU
%    
%Created by: Jim Follum(james.follum@pnnl.gov)
%Modified on June 7, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
    %1. Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix
    %2. data are set to NaN after carrying out all filter operation instead of setting data to NaN after each filter operation
% Modified July 28, 2016 by Urmila Agrawal:
% Added file type input parameter to be used by voltage quality check
% filter as voltage quantity given in pdat file is per phase and that in
% csv file is line-to-line
%
% Modified December 1, 2016 by Jim Follum:
% Commented out the check for NumFilts == 1 because it is already performed
% in the main script.
%
% Modified March 31, 2017 by Jim Follum
    % The FileType is no longer needed by VoltPhasorFilt() (see that
    % function for details) so it was removed as an input.
    
function PMU = DQfilterStep(PMU,StageStruct)

NumFilts = length(StageStruct.Filter);
%defining a matrix whose values are set to 1 if the the content is to be
%set to NaN after filtering operation

% This check is now performed in the main script, so I'm commenting it out
% on 12-1-2016 - Jim.
% if NumFilts == 1
%     % By default, the contents of StageStruct.Filter
%     % would not be in a cell array because length is one. This 
%     % makes it so the same indexing can be used in the following for loop.
%     StageStruct.Filter = {StageStruct.Filter};
% end
for PMUidx = 1:length(PMU)
    [n,nm] = size(PMU(PMUidx).Data);
    setNaNMatrix{PMUidx} = zeros(n,nm);
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
            TempIdx = find(strcmp(StageStruct.Filter{FiltIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));
            if ~isempty(TempIdx)
                PMUstructIdx(PMUidx) = TempIdx;
            else
                warning(['PMU ' StageStruct.Filter{FiltIdx}.PMU{PMUidx}.Name ' was not found.']);
            end

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
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = PMUflagFilt(PMU(PMUstructIdx(PMUidx)),Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'DropOutZeroFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = DropOutZeroFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'DropOutMissingFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                PMU(PMUstructIdx(PMUidx)) = DropOutMissingFilt(PMU(PMUstructIdx(PMUidx)),Parameters);
            end
        case 'VoltPhasorFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = VoltPhasorFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'FreqFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = FreqFilt(PMU(PMUstructIdx(PMUidx)),Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'OutlierFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = OutlierFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'StaleFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = StaleFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'DataFrameFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = DataFrameFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'PMUchanFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = ChanFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'PMUallFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = EntirePMUfilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
        case 'WrappingFailureFilt'
            for PMUidx = 1:NumPMU
                % If the PMU doesn't exist, skip to next index
                if (PMUstructIdx(PMUidx) == 0); continue; end
                [PMU(PMUstructIdx(PMUidx)),setNaNMatrix{PMUstructIdx(PMUidx)}] = WrappingFailureFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,setNaNMatrix{PMUstructIdx(PMUidx)});
            end
    end
end
%setNaNMatrix{i} has non-zero positive elements for the elements in i^th PMU that is to be
%set NaN
for PMUidx = 1:length(PMU)
    NaNM = setNaNMatrix{PMUidx};
    NaNid = find(NaNM >0);
    PMU(PMUidx).Data(NaNid) = NaN;
end
