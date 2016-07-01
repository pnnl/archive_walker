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
    
function PMU = DPfilterStep(PMU,ProcessFilter)
NumFilts = length(ProcessFilter);

if NumFilts == 1
    ProcessFilter = {ProcessFilter};
end

for FiltIdx = 1:NumFilts 
    
    Parameters = ProcessFilter{FiltIdx}.Parameters;
    if isfield(ProcessFilter{FiltIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessFilter{FiltIdx}.PMU);
        if NumPMU == 1
            ProcessFilter{FiltIdx}.PMU = {ProcessFilter{FiltIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            PMUstructIdx(PMUidx) = find(strcmp(ProcessFilter{FiltIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));

            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered (appropriate channels are selected within the 
            % individual filter code)
            if isfield(ProcessFilter{FiltIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1                 
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

    % Find the appropriate filter and apply it for each of the
    % specified PMUs
    switch ProcessFilter{FiltIdx}.Type
        case 'HighPass'
            for PMUidx = 1:NumPMU
               PMU(PMUstructIdx(PMUidx))= HighPassFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'LowPass'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx))= LowPassFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'Rational'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx))= RationalFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'Median'
            for PMUidx = 1:NumPMU
                PMU(PMUstructIdx(PMUidx))= MedianFilt(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
    end    
end
