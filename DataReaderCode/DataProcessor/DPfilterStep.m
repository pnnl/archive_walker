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
    
function PMUstruct = DPfilterStep(PMUstruct,ProcessFilter)
NumFilts = length(ProcessFilter);
%defining a matrix whose values are set to 1 if the the content is to be
%set to NaN after filtering operation

if NumFilts == 1
    % By default, the contents of StageStruct.Filter
    % would not be in a cell array because length is one. This 
    % makes it so the same indexing can be used in the following for loop.
    ProcessFilter = {ProcessFilter};
end

for FiltIdx = 1:NumFilts 
    
    Parameters = ProcessFilter{FiltIdx}.Parameters;
    if isfield(ProcessFilter{FiltIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessFilter{FiltIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of StageStruct.Filter.PMU
            % would not be in a cell array because length is one. This 
            % makes it so the same indexing can be used in the following for loop.
            ProcessFilter{FiltIdx}.PMU = {ProcessFilter.Filter{FiltIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            PMUstructIdx(PMUidx) = find(strcmp(ProcessFilter{FiltIdx}.PMU{PMUidx}.Name, {PMUstruct.PMU.PMU_Name}));

            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % filtered (appropriate channels are selected within the 
            % individual filter code)
            if isfield(ProcessFilter{FiltIdx}.PMU{PMUidx},'Channels')
                NumChan = length(ProcessFilter{FiltIdx}.PMU{PMUidx}.Channels.Channel);
                if NumChan == 1
                    % By default, the contents of StageStruct.Filter.PMU.Channel
                    % would not be in a cell array because length is one. This 
                    % makes it so the same indexing can be used in the following for loop.
                    ProcessFilter{FiltIdx}.PMU{PMUidx}.Channels.Channel = {ProcessFilter{FiltIdx}.PMU{PMUidx}.Channels.Channel};
                end

                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = ProcessFilter{FiltIdx}.PMU{PMUidx}.Channels.Channel{ChanIdx}.Name;
                end
            end
        end
    else
        % Apply this filter to all signals in all PMUs
        NumPMU = length(PMUstruct.PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end

    % Find the appropriate filter and apply it for each of the
    % specified PMUs
    switch ProcessFilter{FiltIdx}.Type
        case 'HighPass'
            for PMUidx = 1:NumPMU
                PMUstruct.PMU(PMUstructIdx(PMUidx))= HighPassFilt(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'LowPass'
            for PMUidx = 1:NumPMU
                PMUstruct.PMU(PMUstructIdx(PMUidx))= LowPassFilt(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'Rational'
            for PMUidx = 1:NumPMU
                PMUstruct.PMU(PMUstructIdx(PMUidx))= RationalFilt(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
        case 'Median'
            for PMUidx = 1:NumPMU
                PMUstruct.PMU(PMUstructIdx(PMUidx))= MedianFilt(PMUstruct.PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,Parameters);
            end
    end    
end
