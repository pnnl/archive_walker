function [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs] = ExtractData(PMUstruct,Parameters)

Data = [];
DataPMU = {};
DataChannel = {};
DataType = {};
DataUnit = {};
if isfield(Parameters,'PMU')
    % PMUs are specified - add their names to the list
    NumPMU = length(Parameters.PMU);
    if NumPMU == 1
        % By default, the contents of Parameters.PMU would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        Parameters.PMU = {Parameters.PMU};
    end
    
    PMUstructIdxList = zeros(1,NumPMU);
    for PMUparamIdx = 1:NumPMU
        % Find the index of PMUstruct for this PMU
        PMUstructIdx = find(strcmp(Parameters.PMU{PMUparamIdx}.Name, {PMUstruct.PMU_Name}),1);
        PMUstructIdxList(PMUparamIdx) = PMUstructIdx;
        
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
                % Find the index of PMUstruct(PMUstructIdx).Signal_Name for this Channel
                ChannelStructIdx = find(strcmp(Parameters.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name, PMUstruct(PMUstructIdx).Signal_Name),1);
                
                Data = [Data PMUstruct(PMUstructIdx).Data(:,ChannelStructIdx)];
                DataPMU = [DataPMU PMUstruct(PMUstructIdx).PMU_Name];
                DataChannel = [DataChannel PMUstruct(PMUstructIdx).Signal_Name{ChannelStructIdx}];
                DataType = [DataType PMUstruct(PMUstructIdx).Signal_Type{ChannelStructIdx}];
                DataUnit = [DataUnit PMUstruct(PMUstructIdx).Signal_Unit{ChannelStructIdx}];
            end
        else
            % Channels are not specified - use all of them in this PMU
            
            % Add the data to the matrix - each column is a channel of data
            Data = [Data PMUstruct(PMUstructIdx).Data];
            % Replicate the PMU name for each channel of data added
            DataPMUtemp = cell(1,length(PMUstruct(PMUstructIdx).Signal_Name));
            [DataPMUtemp{:}] = deal(PMUstruct(PMUstructIdx).PMU_Name);
            DataPMU = [DataPMU, DataPMUtemp];
            % Add the channel names to the cell array
            DataChannel = [DataChannel PMUstruct(PMUstructIdx).Signal_Name];
            DataType = [DataType PMUstruct(PMUstructIdx).Signal_Type];
            DataUnit = [DataUnit PMUstruct(PMUstructIdx).Signal_Unit];
        end
    end
else
    PMUstructIdxList = 1:length(PMUstruct);

    % PMUs are not specified - add all channels in all PMUs
    for PMUparamIdx = 1:length(PMUstruct)
        % Add the data to the matrix - each column is a channel of data
        Data = [Data PMUstruct(PMUparamIdx).Data];
        % Replicate the PMU name for each channel of data added
        DataPMUtemp = cell(1,length(PMUstruct(PMUparamIdx).Signal_Name));
        [DataPMUtemp{:}] = deal(PMUstruct(PMUparamIdx).PMU_Name);
        DataPMU = [DataPMU, DataPMUtemp];
        % Add the channel names to the cell array
        DataChannel = [DataChannel PMUstruct(PMUparamIdx).Signal_Name];
        DataType = [DataType PMUstruct(PMUparamIdx).Signal_Type];
        DataUnit = [DataUnit PMUstruct(PMUparamIdx).Signal_Unit];
    end
end

% Get the sampling rate. Try for each PMU that was included until it is
% found
fs = [];
t = [];
for PMUstructIdx = PMUstructIdxList
    % Try to calculate the sampling rate. It may fail if something was
    % wrong with the input data
    try
        fs = round(1/(diff(PMUstruct(PMUstructIdx).Signal_Time.Signal_datenum(1:2))*24*60*60));
    end

    % If fs was created and is not NaN, stop trying to calculate it
    if ~isempty(fs) && ~isnan(fs)
        % Get a time vector in seconds
        t = (0:size(Data,1)-1)/fs;
        break;
    end
end