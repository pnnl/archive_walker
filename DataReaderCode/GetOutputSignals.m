% function PMU = GetOutputSignals(PMU,inputXML)
% This function removes unwanted signals from the input PMU structure so
% that the output PMU structure only contains the desired signals.
% 
% Inputs:
	% PMU: structure in the common format for PMU data. See the
        % specifications for the data reader.
    % inputXML: structure containing information from the configuration or 
        % processing XML
%  
% Outputs:
    % PMU: structure in the common format for PMU data containing only the
        % desired PMUs and signals
%     
% Created by: Jim Follum(james.follum@pnnl.gov) on June 17, 2016

function PMU = GetOutputSignals(PMU,inputXML)

% Get the structure of interest
SignalSelection = inputXML.Configuration.SignalSelection;

% If the PMU field was included in the configuration XML, then only signals
% from the specified PMUs will be retained. If the PMU field was omitted,
% retain all the PMU structures.
if isfield(SignalSelection,'PMU')
    % Only signals from the specified PMUs will be retained
    
    if length(SignalSelection.PMU) == 1
        % By default, the contents of SignalSelection.PMU would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used regardless of the length.
        SignalSelection.PMU = {SignalSelection.PMU};
    end
    
    % Find the PMUs that are to be kept
    KeepIdxPMU = [];
    for idxPMU = 1:length(SignalSelection.PMU)
        % The name of a PMU listed in the configuration XML file
        PMUname = SignalSelection.PMU{idxPMU}.Name;
        % Index of the PMU of interest in the PMU structure
        ThisPMU = find(strcmp(PMUname,{PMU.PMU_Name}));
        if length(ThisPMU) == 1
            % Store the index of the PMU of interest
            KeepIdxPMU = [KeepIdxPMU ThisPMU];
        elseif length(ThisPMU) == 0
            warning([PMUname ' is not a valid PMU name, it will not be included.']);
        elseif length(ThisPMU) > 1
            error('Multiple PMUs in the file have the same name.');
        end
    end
else
    % Specific PMUs were not specified, so keep them all
    KeepIdxPMU = 1:length(PMU);
end
% Keep only the desired PMUs
PMU = PMU(KeepIdxPMU);


% For the remaining PMUs, keep the signals that are either
%   a) of the appropriate type specified in the XML
%   b) specified as signals to be kept for the specific PMU

% Check if the XML listed any signal types to be kept
if isfield(SignalSelection,'SignalType')
    % Signal types were specified. Store them
    
    if length(SignalSelection.SignalType) == 1
        % By default, the contents of SignalSelection.SignalType would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used regardless of the length.
        SignalSelection.SignalType = {SignalSelection.SignalType};
    end
    
    % Get all the signal types to be kept from the SignalSelection
    % structure
    TypeCell    = cell(1, length(SignalSelection.SignalType));
    TypeCell(:) = {'Type'};
    SignalTypesToKeep = cellfun(@getfield,SignalSelection.SignalType,TypeCell,'UniformOutput',false);
else
    % Signal types were not specified, so set the list of signal types to
    % keep to empty. Signal types will not be considered.
    SignalTypesToKeep = {};
end

% For each of the PMUs that are to be kept
for PMUidx = 1:length(PMU)
    % Lists of signal names and types in the PMU
    Signal_Name = PMU(PMUidx).Signal_Name;
    Signal_Type = PMU(PMUidx).Signal_Type;
    
    % Initialize a vector to store indices for the signals to be kept
    KeepIdxSignal = zeros(1,length(Signal_Name));
    
    % If the Channels field was specified in the configuration XML,
    % keep only the specified signals. If not, keep all the signals.
    if isfield(SignalSelection,'PMU')
        if isfield(SignalSelection.PMU{PMUidx},'Channels')
            if length(SignalSelection.PMU{PMUidx}.Channels.Channel) == 1
                % By default, the contents of SignalSelection.PMU{PMUidx}.Channels.Channel would not be
                % in a cell array because length is one. This makes it so the same
                % indexing can be used regardless of the length.
                SignalSelection.PMU{PMUidx}.Channels.Channel = {SignalSelection.PMU{PMUidx}.Channels.Channel};
            end

            % Get all the names of signals to be kept from the SignalSelection
            % structure
            NameCell    = cell(1, length(SignalSelection.PMU{PMUidx}.Channels.Channel));
            NameCell(:) = {'Name'};
            SignalNamesToKeep = cellfun(@getfield,SignalSelection.PMU{PMUidx}.Channels.Channel,NameCell,'UniformOutput',false);

            % For each signal that is to be kept based on its name
            for SigNameIdx = 1:length(SignalNamesToKeep)
                % Set KeepIdxSignal to 1 where the Signal_Name cell array matches
                % the current SignalNamesToKeep
                KeepIdxSignal = KeepIdxSignal | strcmp(Signal_Name,SignalNamesToKeep{SigNameIdx});
            end

            % For each signal type that is to be kept
            for SigTypeIdx = 1:length(SignalTypesToKeep)
                % Set KeepIdxSignal to 1 where the Signal_Type cell array matches
                % the current SignalTypesToKeep
                KeepIdxSignal = KeepIdxSignal | strcmp(Signal_Type,SignalTypesToKeep{SigTypeIdx});
            end
        else
            % Signals were not specified, so keep all channels of the specified
            % types
            if isempty(SignalTypesToKeep)
                % Types were not specified, so keep all channels
                KeepIdxSignal(:) = 1;
            else
                % For each signal type that is to be kept
                for SigTypeIdx = 1:length(SignalTypesToKeep)
                    % Set KeepIdxSignal to 1 where the Signal_Type cell array matches
                    % the current SignalTypesToKeep
                    KeepIdxSignal = KeepIdxSignal | strcmp(Signal_Type,SignalTypesToKeep{SigTypeIdx});
                end
            end
        end
    else
        % Signals were not specified, so keep all channels of the specified
        % types
        if isempty(SignalTypesToKeep)
            % Types were not specified, so keep all channels
            KeepIdxSignal(:) = 1;
        else
            % For each signal type that is to be kept
            for SigTypeIdx = 1:length(SignalTypesToKeep)
                % Set KeepIdxSignal to 1 where the Signal_Type cell array matches
                % the current SignalTypesToKeep
                KeepIdxSignal = KeepIdxSignal | strcmp(Signal_Type,SignalTypesToKeep{SigTypeIdx});
            end
        end
    end
    
    % Keep only the signals that are of the correct type or have been
    % specified by name.
    KeepIdxSignal = find(KeepIdxSignal);
    PMU(PMUidx).Signal_Name = PMU(PMUidx).Signal_Name(KeepIdxSignal);
    PMU(PMUidx).Signal_Type = PMU(PMUidx).Signal_Type(KeepIdxSignal);
    PMU(PMUidx).Signal_Unit = PMU(PMUidx).Signal_Unit(KeepIdxSignal);
    PMU(PMUidx).Data = PMU(PMUidx).Data(:,KeepIdxSignal);
    PMU(PMUidx).Flag = PMU(PMUidx).Flag(:,KeepIdxSignal,:);
end