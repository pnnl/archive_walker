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
        if isempty(ThisPMUidx)
            warning([Parameters.PMU{index}.Name ' is not a valid PMU name. Change of name, type, and unit impossible.']);
        else
            ThisSIGidx = find(strcmp(Parameters.PMU{index}.CurrentChannel, PMU(ThisPMUidx).Signal_Name));
            if isempty(ThisSIGidx)
                warning([Parameters.PMU{index}.CurrentChannel ' is not a valid channel name. Change of name, type, and unit impossible.']);
            else
                PMU(ThisPMUidx).Signal_Name{ThisSIGidx} = Parameters.PMU{index}.NewChannel;
                PMU(ThisPMUidx).Signal_Unit{ThisSIGidx} = Parameters.PMU{index}.NewUnit;
                PMU(ThisPMUidx).Signal_Type{ThisSIGidx} = Parameters.PMU{index}.NewType;
            end
        end
    end
elseif isfield(Parameters,'NewUnit')
    for PMUidx = 1:length(PMU)
        for ChanIdx = 1:length(PMU(PMUidx).Signal_Name)
            PMU(PMUidx).Signal_Unit{ChanIdx} = Parameters.NewUnit;
            PMU(PMUidx).Signal_Type{ChanIdx} = Parameters.NewType;
        end
    end
end