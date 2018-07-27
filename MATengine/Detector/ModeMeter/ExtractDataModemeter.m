function [DataEx, DataPMUEx, DataChannelEx, DataTypeEx, DataUnitEx, t, fs, TimeString] = ExtractDataModemeter(PMUstruct,Parameters)

% Throughout, warnings are placed before identical errors so that the user
% sees them (this function is placed inside a try-catch block in every
% place it is used that provides a generic warning that the data couldn't
% be used with specific information about which detector.

if isfield(Parameters,'Mode')
    % PMUs are specified - add their names to the list
    NumMode = length(Parameters.Mode);
    if NumMode == 1
        % By default, the contents of Parameters.PMU would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        Parameters.Mode = {Parameters.Mode};
    end
    DataEx = cell(1,NumMode);
    DataPMUEx = cell(1,NumMode);
    DataChannelEx = cell(1,NumMode);
    DataTypeEx = cell(1,NumMode);
    DataUnitEx = cell(1,NumMode);
    t = cell(1,NumMode);
    fs = cell(1,NumMode);
    TimeString = cell(1,NumMode);
    for ModeIdx = 1:NumMode
        Data = [];
        DataPMU = cell(0);
        DataChannel = cell(0);
        DataType = cell(0);
        DataUnit = cell(0);
        if isfield(Parameters.Mode{ModeIdx},'PMU')
            % PMUs are specified - add their names to the list
            NumPMU = length(Parameters.Mode{ModeIdx}.PMU);
            if NumPMU == 1
                % By default, the contents of Parameters.PMU would not be
                % in a cell array because length is one. This makes it so the same
                % indexing can be used in the following for loop.
                Parameters.Mode{ModeIdx}.PMU = {Parameters.Mode{ModeIdx}.PMU};
            end
            
            PMUstructIdxList = zeros(1,NumPMU);
            for PMUparamIdx = 1:NumPMU
                % Find the index of PMUstruct for this PMU
                PMUstructIdx = find(strcmp(Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Name, {PMUstruct.PMU_Name}),1);
                if isempty(PMUstructIdx)
                    warning(['PMU ' Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Name ' does not exist.']);
                    error(['PMU ' Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Name ' does not exist.']);
                end
                PMUstructIdxList(PMUparamIdx) = PMUstructIdx;
                
                if isfield(Parameters.Mode{ModeIdx}.PMU{PMUparamIdx},'Channel')
                    % Channels are specified - use only specified channels
                    NumChannel = length(Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Channel);
                    if NumChannel == 1
                        % By default, the contents of Parameters.PMU{PMUidx}.Channel would not be
                        % in a cell array because length is one. This makes it so the same
                        % indexing can be used in the following for loop.
                        Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Channel = {Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Channel};
                    end
                    
                    for ChannelIdx = 1:NumChannel
                        % Find the index of PMUstruct(PMUstructIdx).Signal_Name for this Channel
                        ChannelStructIdx = find(strcmp(Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name, PMUstruct(PMUstructIdx).Signal_Name),1);
                        
                        if isempty(ChannelStructIdx)
                            warning(['Channel ' Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name ' does not exist.']);
                            error(['Channel ' Parameters.Mode{ModeIdx}.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name ' does not exist.']);
                        end
                        
                        try
                            Data = [Data PMUstruct(PMUstructIdx).Data(:,ChannelStructIdx)];
                        catch
                            if size(Data,1) ~= length(PMUstruct(PMUstructIdx).Data(:,ChannelStructIdx))
                                warning('Data used together in mode-meters must have the same length.')
                                error('Data used together in mode-meters must have the same length.')
                            else
                                warning('Failed to collect data for mode-meter algorithms.')
                                error('Failed to collect data for mode-meter algorithms.')
                            end
                        end
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
                    DataPMU = [DataPMU DataPMUtemp];
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
                try
                    Data = [Data PMUstruct(PMUparamIdx).Data];
                catch
                    if size(Data,1) ~= size(PMUstruct(PMUparamIdx).Data,1)
                        warning('Data used together in mode-meters must have the same length.')
                        error('Data used together in mode-meters must have the same length.')
                    else
                        warning('Failed to collect data for mode-meter algorithms.')
                        error('Failed to collect data for mode-meter algorithms.')
                    end
                end
                % Replicate the PMU name for each channel of data added
                DataPMUtemp = cell(1,length(PMUstruct(PMUparamIdx).Signal_Name));
                [DataPMUtemp{:}] = deal(PMUstruct(PMUparamIdx).PMU_Name);
                DataPMU = [DataPMU; DataPMUtemp];
                % Add the channel names to the cell array
                DataChannel = [DataChannel PMUstruct(PMUparamIdx).Signal_Name];
                DataType = [DataType PMUstruct(PMUparamIdx).Signal_Type];
                DataUnit = [DataUnit PMUstruct(PMUparamIdx).Signal_Unit];
            end
        end
        DataEx{ModeIdx} =  Data;
        DataPMUEx{ModeIdx} = DataPMU;
        DataChannelEx{ModeIdx} = DataChannel;
        DataTypeEx{ModeIdx} =DataType;
        DataUnitEx{ModeIdx} = DataUnit;
        
        % All included PMUs have to have the same signal lengths, so just use the
        % first one to get the time stamps as strings
        if ~isempty(PMUstructIdxList)
            TimeString{ModeIdx} = PMUstruct(PMUstructIdxList(1)).Signal_Time.Time_String;
        else
            TimeString{ModeIdx} = {};
        end
        
        % Get the sampling rate. Try for each PMU that was included until it is
        % found
        fs{ModeIdx} = [];
        t{ModeIdx} = [];
        for PMUstructIdx = PMUstructIdxList
            % Try to calculate the sampling rate. It may fail if something was
            % wrong with the input data
            try
                % Rounds to the nearest 0.1 Hz
                fs{ModeIdx} = round(1/mean((diff(PMUstruct(PMUstructIdx).Signal_Time.Signal_datenum)*24*60*60))*10)/10;
            end
            
            % If fs was created and is not NaN, stop trying to calculate it
            if ~isempty(fs{ModeIdx}) && ~isnan(fs{ModeIdx})
                % Get a time vector in seconds
                t{ModeIdx} = (0:size(Data,1)-1)/fs{ModeIdx};
                break;
            end
        end
    end
end

