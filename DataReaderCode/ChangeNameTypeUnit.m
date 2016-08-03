function PMU = ChangeNameTypeUnit(PMU,Parameters)

if length(Parameters) == 1
    % By default, the contents of Parameters would not be in a cell array
    % because length is one. This makes it so the same indexing can be used
    % in the following for loop.
    Parameters = {Parameters};
end

for index = 1:length(Parameters)
    ThisPMUidx = find(strcmp(Parameters{index}.Name, {PMU.PMU_Name}));
    ThisSIGidx = find(strcmp(Parameters{index}.CurrentChannel, PMU(ThisPMUidx).Signal_Name));
    
    PMU(ThisPMUidx).Signal_Name{ThisSIGidx} = Parameters{index}.NewChannel;
    PMU(ThisPMUidx).Signal_Unit{ThisSIGidx} = Parameters{index}.NewUnit;
    PMU(ThisPMUidx).Signal_Type{ThisSIGidx} = Parameters{index}.NewType;
end