function ConfigSignalSelection = GetPMU_SignalList(DetectorXML,DetectorTypes,WindAppXML)
ConfigSignalSelection.PmuName = [];
ConfigSignalSelection.ChannelName = [];
for DetectorType = DetectorTypes
    % Check if the current DetectorType was included in the detector's
    % configuration file. If so, extract PMu and signal names
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
            if strcmp(DetectorType{1},'ModeMeter')
                if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}, 'Mode')
                    if length(DetectorXML.(DetectorType{1}){DetectorIndex}.Mode)==1
                        DetectorXML.(DetectorType{1}){DetectorIndex}.Mode = {DetectorXML.(DetectorType{1}){DetectorIndex}.Mode};
                    end
                    for ModIdx = 1:length(DetectorXML.(DetectorType{1}){DetectorIndex}.Mode)
                        [PMUDet, DataChannelDet] = ExtractSignalInfo(DetectorXML.(DetectorType{1}){DetectorIndex}.Mode{ModIdx});
                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; PMUDet];
                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DataChannelDet];
                    end
                end
                if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}, 'BaseliningSignals')
                    [PMUDet, DataChannelDet] = ExtractSignalInfo(DetectorXML.(DetectorType{1}){DetectorIndex}.BaseliningSignals);
                    ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; PMUDet];
                    ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DataChannelDet];
                end
            elseif strcmp(DetectorType{1},'Thevenin')
                if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}, 'Sub')
                    if length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub)==1
                        DetectorXML.(DetectorType{1}){DetectorIndex}.Sub = {DetectorXML.(DetectorType{1}){DetectorIndex}.Sub};
                    end
                    for SubIdx = 1:length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub)
                        if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}, 'Freq')
                            ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Freq.PMU];
                            ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Freq.F];
                        end
                        
                        if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}, 'Vbus')
                            if length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus)==1
                                DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus = {DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus};
                            end
                            for VbusIdx = 1:length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus)
                                if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus{VbusIdx},'PMU')
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus{VbusIdx},'MAG')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus{VbusIdx}.PMU;];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus{VbusIdx}.MAG;];
                                    end
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus{VbusIdx},'ANG')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus{VbusIdx}.PMU];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Vbus{VbusIdx}.ANG];
                                    end
                                end
                            end
                        end
                        if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}, 'Branch')
                            if length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch)==1
                                DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch = {DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch};
                            end
                            for BranchIdx = 1:length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch)
                                if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx},'PMU')
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx},'P')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.PMU;];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.P;];
                                    end
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx},'Q')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.PMU];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.Q];
                                    end
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx},'Imag')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.PMU];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.Imag];
                                    end
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx},'Iang')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.PMU];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Branch{BranchIdx}.Iang];
                                    end
                                end
                            end
                        end
                        if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}, 'Shunt')
                            if length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt)==1
                                DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt = {DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt};
                            end
                            for ShuntIdx = 1:length(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt)
                                if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx},'PMU')
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx},'P')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.PMU;];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.P;];
                                    end
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx},'Q')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.PMU];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.Q];
                                    end
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx},'Imag')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.PMU];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.Imag];
                                    end
                                    if isfield(DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx},'Iang')
                                        ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.PMU];
                                        ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName;  DetectorXML.(DetectorType{1}){DetectorIndex}.Sub{SubIdx}.Shunt{ShuntIdx}.Iang];
                                    end
                                end
                            end
                        end
                    end
                end
            else
                [PMUDet, DataChannelDet] = ExtractSignalInfo(DetectorXML.(DetectorType{1}){DetectorIndex});
                ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; PMUDet];
                ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName; DataChannelDet];
            end
        end
    end
end
if ~isempty(WindAppXML)
    if isfield(WindAppXML,'PMU')
        for PMUidx = 1:length(WindAppXML.PMU)
            if isfield(WindAppXML.PMU{PMUidx},'PowerChannel')
                for PowerChannelIdx = 1:length(WindAppXML.PMU{PMUidx}.PowerChannel)
                    ConfigSignalSelection.PmuName = [ConfigSignalSelection.PmuName; WindAppXML.PMU{PMUidx}.Name];
                    ConfigSignalSelection.ChannelName = [ConfigSignalSelection.ChannelName;  WindAppXML.PMU{PMUidx}.PowerChannel{PowerChannelIdx}.Name];
                end
            end
        end
    end
end
[ConfigSignalSelection.ChannelName,uniqueMdx] = unique(ConfigSignalSelection.ChannelName);
ConfigSignalSelection.PmuName = ConfigSignalSelection.PmuName(uniqueMdx,1);

