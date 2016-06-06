% PMUstruct is the structure in the common format for a single PMU
%
% SigsToFilt is cell array. Each cell is a string specifying a signal to be
% filtered.

% function PMUstruct = FreqFilt(PMUstruct,FreqMinChan,FreqMaxChan,FreqPctChan,FreqMinSamp,FreqMaxSamp,SetToNaN,FlagBitChan,FlagBitSamp)
function PMUstruct = FreqFilt(PMUstruct,Parameters)

FreqMinChan = str2num(Parameters.FreqMinChan);
FreqMaxChan = str2num(Parameters.FreqMaxChan);
FreqPctChan = str2num(Parameters.FreqPctChan);
FreqMinSamp = str2num(Parameters.FreqMinSamp);
FreqMaxSamp = str2num(Parameters.FreqMaxSamp);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBitChan = str2num(Parameters.FlagBitChan);
FlagBitSamp = str2num(Parameters.FlagBitSamp);

% Find all frequency signals. The filter is applied to each one.
SigIdx = find(strcmp(PMUstruct.Signal_Type, 'F'));

for ThisSig = SigIdx
    % Stage 1 - Overall DQ of channel
    OutIdx1 = (PMUstruct.Data(:,ThisSig) > FreqMaxChan) | (PMUstruct.Data(:,ThisSig) < FreqMinChan);
    if sum(OutIdx1)/length(OutIdx1)*100 > FreqPctChan
        % Too many points are ouside the tight bound - remove the whole
        % channel
        PMUstruct.Flag(:,ThisSig,FlagBitChan) = true;
%         if SetToNaN
%             PMUstruct.Data(:,ThisSig) = NaN;
%         end
    end
    
    % Stage 2 - individual measurements
    OutIdx2 = find((PMUstruct.Data(:,ThisSig) > FreqMaxSamp) | (PMUstruct.Data(:,ThisSig) < FreqMinSamp));
    PMUstruct.Flag(OutIdx2,ThisSig,FlagBitSamp) = true;
    if SetToNaN
        PMUstruct.Data(OutIdx2,ThisSig) = NaN;
    end
    if SetToNaN && sum(OutIdx1)/length(OutIdx1)*100 > FreqPctChan %changed so that when conditions for both the cases are met, both the flags are set to 1.
            PMUstruct.Data(:,ThisSig) = NaN;
    end
end