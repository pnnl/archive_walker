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
    ProcessInterpolate = {ProcessInterpolate};
end

for InterpIdx = 1:NumInterp 
    
    Parameters = ProcessInterpolate{InterpIdx}.Parameters;
    if isfield(ProcessInterpolate{InterpIdx},'PMU')
        % Get list of PMUs 
        NumPMU = length(ProcessInterpolate{InterpIdx}.PMU);
        if NumPMU == 1
            % By default, the contents would not be in a cell array because
            % length is one. This makes it so the same indexing can be used
            % in the following for loop.
            ProcessInterpolate{InterpIdx}.PMU = {ProcessInterpolate{InterpIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            PMUstructIdx(PMUidx) = find(strcmp(ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));
            if isfield(ProcessInterpolate{InterpIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1
                    ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel = {ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel};
                end

                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = ProcessInterpolate{InterpIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name;
                end
            end
        end
    else
        NumPMU = length(PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end
    
    for PMUidx = 1:NumPMU
        PMU(PMUstructIdx(PMUidx))= interpo(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters,FlagBitInterpo);
    end
end
