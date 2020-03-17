% ADD documentation!!!

function PMUcut = ReduceSignals(PMU,DetectorXML,BlockDetectors)

PMUlist = [];
ChanList = {};
for DetectorType = BlockDetectors
    
    % Note that the {1} following each DetectorType is necessary.
    % DetectorType on its own is a cell. Adding the {1} accesses the string
    % within the cell.
    
    % Check if the current DetectorType was included in the detector's
    % configuration file. If so, run the detector.
    if isfield(DetectorXML,DetectorType{1})
        % Find the number of separate instances of this detector type.
        NumDetectors = length(DetectorXML.(DetectorType{1}));
        if NumDetectors == 1
            % By default, the contents of DetectorXML.(DetectorType{1}) would not be
            % in a cell array because length is one. This makes it so the same
            % indexing can be used in the following for loop.
            DetectorXML.(DetectorType{1}) = {DetectorXML.(DetectorType{1})};
        end
        
        % Implement each instance of this detector type
        for DetectorIndex = 1:NumDetectors
            Parameters = DetectorXML.(DetectorType{1}){DetectorIndex};
            
            % Special case for mode meters
            if strcmp(DetectorType{1},'ModeMeter')
                NumMode = length(Parameters.Mode); % gives number of modes of interest
                if NumMode==1
                    Parameters.Mode = {Parameters.Mode};
                end
                for ModeIdx = 1:NumMode
                    [PMUlistTemp, ChanListTemp] = ListSignals(PMU,Parameters.Mode{ModeIdx});
                    PMUlist = [PMUlist PMUlistTemp];
                    ChanList = [ChanList ChanListTemp];
                    
                    if isfield(Parameters.Mode{ModeIdx},'FOdetectorParam')
                        [PMUlistTemp, ChanListTemp] = ListSignals(PMU,Parameters.Mode{ModeIdx}.FOdetectorParam);
                        PMUlist = [PMUlist PMUlistTemp];
                        ChanList = [ChanList ChanListTemp];
                    end
                    if isfield(Parameters.Mode{ModeIdx},'EventDetectorParam')
                        [PMUlistTemp, ChanListTemp] = ListSignals(PMU,Parameters.Mode{ModeIdx}.EventDetectorParam);
                        PMUlist = [PMUlist PMUlistTemp];
                        ChanList = [ChanList ChanListTemp];
                    end
                end
                if isfield(Parameters,'BaseliningSignals')
                    [PMUlistTemp, ChanListTemp] = ListSignals(PMU,Parameters.BaseliningSignals);
                    PMUlist = [PMUlist PMUlistTemp];
                    ChanList = [ChanList ChanListTemp];
                end
            elseif strcmp(DetectorType{1},'Thevenin')
                % Special case for Thevenin - just include all signals
                PMUlist = [PMUlist 1:length(PMU)];
                for PMUidx = 1:length(PMU)
                    ChanList = [ChanList {1:length(PMU(PMUidx).Signal_Name)}];
                end
            else
                [PMUlistTemp, ChanListTemp] = ListSignals(PMU,Parameters);
                PMUlist = [PMUlist PMUlistTemp];
                ChanList = [ChanList ChanListTemp];
            end
        end
    end
end

PMUkeep = unique(PMUlist);
ChanKeep = cell(1,length(PMUkeep));
for k = 1:length(PMUkeep)
    MatchIdx = find(PMUlist == PMUkeep(k));
    for m = MatchIdx
        ChanKeep{k} = [ChanKeep{k} ChanList{m}];
    end
    ChanKeep{k} = unique(ChanKeep{k});
end

PMUcut = PMU(PMUkeep);
for PMUidx = 1:length(PMUcut)
    PMUcut(PMUidx).Data = PMUcut(PMUidx).Data(:,ChanKeep{PMUidx});
    PMUcut(PMUidx).Flag = PMUcut(PMUidx).Flag(:,ChanKeep{PMUidx},:);
    PMUcut(PMUidx).Signal_Name = PMUcut(PMUidx).Signal_Name(ChanKeep{PMUidx});
    PMUcut(PMUidx).Signal_Type = PMUcut(PMUidx).Signal_Type(ChanKeep{PMUidx});
    PMUcut(PMUidx).Signal_Unit = PMUcut(PMUidx).Signal_Unit(ChanKeep{PMUidx});
end

end


function [PMUlist, ChanList] = ListSignals(PMUstruct,Parameters)

% Throughout, warnings are placed before identical errors so that the user
% sees them (this function is placed inside a try-catch block in every
% place it is used that provides a generic warning that the data couldn't
% be used with specific information about which detector.

if isfield(Parameters,'PMU')
    % PMUs are specified - add their names to the list
    NumPMU = length(Parameters.PMU);
    if NumPMU == 1
        % By default, the contents of Parameters.PMU would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        Parameters.PMU = {Parameters.PMU};
    end
    
    PMUlist = zeros(1,NumPMU);
    ChanList = cell(1,NumPMU);
    for PMUparamIdx = 1:NumPMU
        % Find the index of PMUstruct for this PMU
        PMUstructIdx = find(strcmp(Parameters.PMU{PMUparamIdx}.Name, {PMUstruct.PMU_Name}),1);
        if isempty(PMUstructIdx)
            warning(['PMU ' Parameters.PMU{PMUparamIdx}.Name ' does not exist.']);
            error(['PMU ' Parameters.PMU{PMUparamIdx}.Name ' does not exist.']);
        end
        PMUlist(PMUparamIdx) = PMUstructIdx;
        
        if isfield(Parameters.PMU{PMUparamIdx},'Channel')
            % Channels are specified - use only specified channels
            NumChannel = length(Parameters.PMU{PMUparamIdx}.Channel);
            if NumChannel == 1
                % By default, the contents of Parameters.PMU{PMUidx}.Channel would not be
                % in a cell array because length is one. This makes it so the same
                % indexing can be used in the following for loop.
                Parameters.PMU{PMUparamIdx}.Channel = {Parameters.PMU{PMUparamIdx}.Channel};
            end
            
            ChanList{PMUparamIdx} = zeros(1,NumChannel);
            for ChannelIdx = 1:NumChannel
                % Find the index of PMUstruct(PMUstructIdx).Signal_Name for this Channel
                ChannelStructIdx = find(strcmp(Parameters.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name, PMUstruct(PMUstructIdx).Signal_Name),1);
                
                if isempty(ChannelStructIdx)
                    warning(['Channel ' Parameters.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name ' does not exist.']);
                    error(['Channel ' Parameters.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name ' does not exist.']);
                end
                
                ChanList{PMUparamIdx}(ChannelIdx) = ChannelStructIdx;
            end
        else
            % Channels are not specified - use all of them in this PMU
            ChanList{PMUparamIdx} = 1:length(PMUstruct(PMUstructIdx).Signal_Name);
        end
    end
else
    PMUlist = 1:length(PMUstruct);

    % PMUs are not specified - add all channels in all PMUs
    ChanList = cell(1,length(PMUstruct));
    for PMUparamIdx = 1:length(PMUstruct)
        ChanList{PMUparamIdx} = 1:length(PMUstruct(PMUparamIdx).Signal_Name);
    end
end

end