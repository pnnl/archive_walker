% PMUstruct is the structure in the common format for a single PMU
%
% SigsToFilt is cell array. Each cell is a string specifying a signal to be
% filtered.

% function PMUstruct = DropOutZeroFilt(PMUstruct,SigsToFilt,SetToNaN,FlagBit)
function PMUstruct = DropOutZeroFilt(PMUstruct,SigsToFilt,Parameters)

SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);

% If specific signals were not listed, apply to all signals except 
% digitals, scalars, and rocof
if isempty(SigsToFilt)
    SigIdx = find(~strcmp(PMUstruct.Signal_Type, 'D') &...
        ~strcmp(PMUstruct.Signal_Type, 'SC') &...
        ~strcmp(PMUstruct.Signal_Type, 'RCF'));
    SigsToFilt = PMUstruct.Signal_Name(SigIdx);
end

for SigIdx = 1:length(SigsToFilt)
    ThisSig = find(strcmp(PMUstruct.Signal_Name,SigsToFilt{SigIdx}));
    
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToFilt{SigIdx} ' could not be found.']);
        continue
    end
    
    % Indices where the reported value was zero
    ZeroIdx = find(PMUstruct.Data(:,ThisSig) == 0);
    
    % Set flags for drop-outs
    PMUstruct.Flag(ZeroIdx,ThisSig,FlagBit) = true;
    % If desired, replace zeros in Data matrix with NaN
    if SetToNaN
        PMUstruct.Data(ZeroIdx,ThisSig) = NaN;
    end
end