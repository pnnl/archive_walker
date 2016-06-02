% PMUstruct is the structure in the common format for a single PMU
%
% SigsToFilt is cell array. Each cell is a string specifying a signal to be
% filtered.

% function PMUstruct = FreqFilt(PMUstruct,FreqMinChan,FreqMaxChan,FreqPctChan,FreqMinSamp,FreqMaxSamp,SetToNaN,FlagValChan,FlagValSamp)
function PMUstruct = FreqFilt(PMUstruct,Parameters)

FreqMinChan = str2num(Parameters.FreqMinChan);
FreqMaxChan = str2num(Parameters.FreqMaxChan);
FreqPctChan = str2num(Parameters.FreqPctChan);
FreqMinSamp = str2num(Parameters.FreqMinSamp);
FreqMaxSamp = str2num(Parameters.FreqMaxSamp);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagValChan = str2num(Parameters.FlagValChan);
FlagValSamp = str2num(Parameters.FlagValSamp);

% Find all frequency signals. The filter is applied to each one.
SigIdx = find(strcmp(PMUstruct.Signal_Type, 'F'));

for ThisSig = SigIdx
    % Stage 1 - Overall DQ of channel
    OutIdx = (PMUstruct.Data(:,ThisSig) > FreqMaxChan) | (PMUstruct.Data(:,ThisSig) < FreqMinChan);
    if sum(OutIdx)/length(OutIdx)*100 > FreqPctChan
        % Too many points are ouside the tight bound - remove the whole
        % channel
        PMUstruct.Flag(:,ThisSig) = FlagValChan;
        if SetToNaN
            PMUstruct.Data(:,ThisSig) = NaN;
        end
    end
    
    % Stage 2 - individual measurements
    OutIdx = find((PMUstruct.Data(:,ThisSig) > FreqMaxSamp) | (PMUstruct.Data(:,ThisSig) < FreqMinSamp));
    PMUstruct.Flag(OutIdx,ThisSig) = FlagValSamp;
    if SetToNaN
        PMUstruct.Data(OutIdx,ThisSig) = NaN;
    end
end