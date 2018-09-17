function PMU = GetOutputSignalsRev(PMU,SignalSelection)

% If the PMU field was included in the configuration XML, then only signals
% from the specified PMUs will be retained. If the PMU field was omitted,
% retain all the PMU structures.
KeepIdxPMU = [];
if isfield(SignalSelection,'PmuName')
    % Only signals from the specified PMUs will be retained
    
    if length(SignalSelection.PmuName) == 1
        % By default, the contents of SignalSelection.PMU would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used regardless of the length.
        SignalSelection.PmuName = {SignalSelection.PmuName};
    end
    UniquePMUname = unique(SignalSelection.PmuName);
    for idxPMU = 1:length(UniquePMUname)
        % Index of the PMU of interest in the PMU structure
        ThisPMU = find(strcmp(UniquePMUname{idxPMU},{PMU.PMU_Name})==1);
        KeepIdxPMU = [KeepIdxPMU ThisPMU];
        if length(ThisPMU) == 1
            KeepIdxSignal = find(strcmp(SignalSelection.PmuName,UniquePMUname{idxPMU})==1);
            SelectedSignalName{idxPMU} = SignalSelection.ChannelName(KeepIdxSignal);
        elseif isempty(ThisPMU)
            warning([PMUname ' is not a valid PMU name, it will not be included.']);
        elseif length(ThisPMU) > 1
            error('Multiple PMUs in the file have the same name.');
        end
    end
end

% Keep only the desired PMUs
PMU = PMU(KeepIdxPMU);

% For each of the PMUs that are to be kept
for PMUidx = 1:length(PMU)
    % Lists of signal names and types in the PMU
    Signal_Name = PMU(PMUidx).Signal_Name;    
    % Initialize a vector to store indices for the signals to be kept
    KeepIdxSignal = [];
    
    if isfield(SignalSelection,'PmuName')
        if isfield(SignalSelection,'ChannelName')
            if length(SignalSelection.ChannelName) == 1
                % By default, the contents of SignalSelection.PMU{PMUidx}.Channels.Channel would not be
                % in a cell array because length is one. This makes it so the same
                % indexing can be used regardless of the length.
                SignalSelection.ChannelName = {SignalSelection.ChannelName};
            end
            
            % Get all the names of signals to be kept from the SignalSelection
            % structure
            SignalNamesToKeep = cellstr(SelectedSignalName{PMUidx});
            
            % For each signal that is to be kept based on its name
            for SigNameIdx = 1:length(SignalNamesToKeep)
                % Set KeepIdxSignal to 1 where the Signal_Name cell array matches
                % the current SignalNamesToKeep
                KeepIdxSignal = [KeepIdxSignal find(strcmp(Signal_Name,SignalNamesToKeep{SigNameIdx})==1)];
            end
        end
        
        % Keep only the signals that are of the correct type or have been
        % specified by name.       
        PMU(PMUidx).Signal_Name = PMU(PMUidx).Signal_Name(KeepIdxSignal);
        PMU(PMUidx).Signal_Type = PMU(PMUidx).Signal_Type(KeepIdxSignal);
        PMU(PMUidx).Signal_Unit = PMU(PMUidx).Signal_Unit(KeepIdxSignal);
        PMU(PMUidx).Data = PMU(PMUidx).Data(:,KeepIdxSignal);
        PMU(PMUidx).Flag = PMU(PMUidx).Flag(:,KeepIdxSignal,:);
    end
end