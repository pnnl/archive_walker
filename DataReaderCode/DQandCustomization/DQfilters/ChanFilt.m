% function PMUstruct = ChanFilt(PMUstruct,SigsToFilt,PctThresh,SetToNaN,FlagBit)
function PMUstruct = ChanFilt(PMUstruct,SigsToFilt,Parameters)

PctThresh = str2num(Parameters.PercentBadThresh);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);

if isempty(SigsToFilt)
    % If specific signals were not listed, get indices of all signals 
    % except digitals and scalars
    SigIdx = find(~strcmp(PMUstruct.Signal_Type, 'D') & ~strcmp(PMUstruct.Signal_Type, 'SC'));
else
    % If specific signals were listed, get indices of the signals to be
    % filtered
    SigIdx = [];
    for ThisSig = 1:length(SigsToFilt)
        ThisSigIdx = find(strcmp(PMUstruct.Signal_Name,SigsToFilt{ThisSig}));
        SigIdx = [SigIdx; ThisSigIdx];
        
        if isempty(ThisSigIdx)
            warning(['Signal ' SigsToFilt{ThisSig} ' could not be found.']);
        end
    end
end

% Create a matrix that indicates frames and signals that have been flagged
% with a 1 and is zero elsewhere.
FlagBin = sum(PMUstruct.Flag(:,SigIdx,:),3);
FlagBin(FlagBin ~= 0) = true;

% Length of signals to be filtered
N = size(FlagBin,1);

% For each signal, the percent of frames that have been flagged
PctFlagged = sum(FlagBin,1)/N*100;

% Indices of signals where the percentage of flagged frames is above
% the specified threshold. 
FiltIdx = PctFlagged >= PctThresh;

% Flag entire signals corresponding to FiltIdx
PMUstruct.Flag(:,SigIdx(FiltIdx),FlagBit) = true;
% If desired, set entire signals corresponding to FiltIdx to NaN
if SetToNaN
    PMUstruct.Data(:,SigIdx(FiltIdx)) = NaN;
end