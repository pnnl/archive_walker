function [DataPMU, DataChannel] = ExtractSignalInfo(Parameters)
DataPMU = {};
DataChannel = {};
if isfield(Parameters,'PMU')
    % PMUs are specified - add their names to the list
    NumPMU = length(Parameters.PMU);
    if NumPMU == 1
        % By default, the contents of Parameters.PMU would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        Parameters.PMU = {Parameters.PMU};
    end
    
    for PMUparamIdx = 1:NumPMU
        % Find the index of PMUstruct for this PMU
        
        if isfield(Parameters.PMU{PMUparamIdx},'Channel')
            % Channels are specified - use only specified channels
            NumChannel = length(Parameters.PMU{PMUparamIdx}.Channel);
            if NumChannel == 1
                % By default, the contents of Parameters.PMU{PMUidx}.Channel would not be
                % in a cell array because length is one. This makes it so the same
                % indexing can be used in the following for loop.
                Parameters.PMU{PMUparamIdx}.Channel = {Parameters.PMU{PMUparamIdx}.Channel};
            end
            
            for ChannelIdx = 1:NumChannel
                DataPMU = [DataPMU; Parameters.PMU{PMUparamIdx}.Name];
                DataChannel = [DataChannel; Parameters.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name];
            end            
        else
            DataPMU = [];
            DataChannel = [];
        end
    end
end