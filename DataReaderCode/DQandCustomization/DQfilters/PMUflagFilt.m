% function PMUstruct = PMUflagFilt(PMUstruct,SetToNaN,FlagVal)
function PMUstruct = PMUflagFilt(PMUstruct,Parameters)

SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagVal = str2num(Parameters.FlagVal);

idx = find(PMUstruct.Stat >= 4096);
PMUstruct.Flag(idx,:) = FlagVal;
if SetToNaN
    PMUstruct.Data(idx,:) = NaN;
end