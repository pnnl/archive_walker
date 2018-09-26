% function PMU = DPinterpolation(PMU,ProcessInterpolate,FlagBitInterpo)
% This function carries out interpolation on PMU measurements specified
% in XML file
%
% Inputs:
	% PMUstruct: a struct array containing PMU structures     
        % PMUstruct.PMU.PMU_Name: a cell array of strings specifying name
        % of  PMUs
        % PMUstruct.PMU(i).Data: Matrix containing data measured by the i^th PMU
        % PMUstruct.PMU(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurements flagged (size: number of data points by number of channels by number of flag bits)  
    %ProcessInterpolate: struct array containing information on different
    % interpolation operation (size: 1 by number of interpoaltion
    % operation)
        % ProcessInterpolate{i}.Type: a string specifying type of i^th filter operation
        % ProcessInterpolate.Parameters: a struct array containing
        % user-specified parameters for i^th data interpolation operation
        % ProcessInterpolate{i}.PMU: struct array containing
        % information on PMUs of dimension 1 by number of PMUs
            %ProcessInterpolate{i}.PMU{j}.Name: a string specifying
            % name of j^th PMU whose data is to be interpolated
            % ProcessFilter{i}.PMU{j}.Channel: a struct array
            % containing name of data channels in j^th PMU whose data is to be interpolated
            % ProcessInterpolate{i}.PMU{j}.Channel{k}.Name: a string
            % specifying name of k^th data channel in j^th PMU for i^th
            % data interpolation operation.
	% FlagBitInterpo: Flag bit indicating interpolated data
%
% Outputs:
    % PMUstruct
%    
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)
    
function PMU = DPinterpolation(PMU,ProcessInterpolate,FlagBitInterpo)
NumInterp = length(ProcessInterpolate);

if NumInterp == 1
    % By default, the contents of ProcessInterpolate would not be in a cell array
    % because length is one. This makes it so the same indexing can be used
    % in the following for loop.
    ProcessInterpolate = {ProcessInterpolate};
end

for InterpIdx = 1:NumInterp 
    % Parameters for the interpolation - the structure contents are
    % specific to each interpolation operation
    Parameters = ProcessInterpolate{InterpIdx}.Parameters;
    if isfield(ProcessInterpolate{InterpIdx},'PMU')
        % Get list of PMUs 
        NumPMU = length(ProcessInterpolate{InterpIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of ProcessFilter{FiltIdx}.PMU
            % would not be in a cell array because length is one. This
            % makes it so the same indexing can be used in the following for loop.
            ProcessInterpolate{InterpIdx}.PMU = {ProcessInterpolate{InterpIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            TempIdx = find(strcmp(ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));
            if ~isempty(TempIdx)
                PMUstructIdx(PMUidx) = TempIdx;
            else
                warning(['PMU ' ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Name ' was not found.']);
            end
            
            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered (appropriate channels are selected within the 
            % individual filter code)
            if isfield(ProcessInterpolate{InterpIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1
                    % By default, the contents of ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel
                    % would not be in a cell array because length is one. This 
                    % makes it so the same indexing can be used in the following for loop.
                    ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel = {ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel};
                end

                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name;
                end
            end
        end
    else
        % Apply this operation to all signals in all PMUs
        NumPMU = length(PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end
    % Interpolate data in each of the specified PMUs and channels
    for PMUidx = 1:NumPMU
        % If the PMU doesn't exist, skip to next index
        if PMUstructIdx(PMUidx) == 0
            continue;
        end
        
        PMU(PMUstructIdx(PMUidx))= interpo(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,FlagBitInterpo);
    end
end
