% function PMUstruct = PMUflagFilt(PMUstruct,SetToNaN,FlagBit)
function PMUstruct = PMUflagFilt(PMUstruct,Parameters)

SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);

idx = find(PMUstruct.Stat >= 4096);
PMUstruct.Flag(idx,:,FlagBit) = true;
if SetToNaN
    PMUstruct.Data(idx,:) = NaN;
end