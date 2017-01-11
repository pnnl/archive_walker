function PMU = ChangeNameTypeUnit(PMU,Parameters)

if isfield(Parameters,'PMU')
    if length(Parameters.PMU) == 1
        % By default, the contents of Parameters would not be in a cell array
        % because length is one. This makes it so the same indexing can be used
        % in the following for loop.
        Parameters.PMU = {Parameters.PMU};
    end

    for index = 1:length(Parameters.PMU)
        ThisPMUidx = find(strcmp(Parameters.PMU{index}.Name, {PMU.PMU_Name}));
        ThisSIGidx = find(strcmp(Parameters.PMU{index}.CurrentChannel, PMU(ThisPMUidx).Signal_Name));

        PMU(ThisPMUidx).Signal_Name{ThisSIGidx} = Parameters.PMU{index}.NewChannel;
        PMU(ThisPMUidx).Signal_Unit{ThisSIGidx} = Parameters.PMU{index}.NewUnit;
        PMU(ThisPMUidx).Signal_Type{ThisSIGidx} = Parameters.PMU{index}.NewType;
    end
elseif isfield(Parameters,'NewUnit')
    for PMUidx = 1:length(PMU)
        for ChanIdx = 1:length(PMU(PMUidx).Signal_Name)
            PMU(PMUidx).Signal_Unit{ChanIdx} = Parameters.NewUnit;
            PMU(PMUidx).Signal_Type{ChanIdx} = Parameters.NewType;
        end
    end
end