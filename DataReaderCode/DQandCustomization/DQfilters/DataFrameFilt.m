% function PMUstruct = DataFrameFilt(PMUstruct,SigsToFilt,PctThresh,SetToNaN,FlagBit)
function PMUstruct = DataFrameFilt(PMUstruct,SigsToFilt,Parameters)

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

% Number of signals to be filtered
NumSigs = length(SigIdx);

% Create a matrix that indicates frames and signals that have been flagged
% with a 1 and is zero elsewhere.
FlagBin = PMUstruct.Flag(:,SigIdx);
FlagBin(FlagBin ~= 0) = 1;

% For each frame, the percent of channels that have been flagged
PctFlagged = sum(FlagBin,2)./NumSigs*100;

% Indices of data frames where the percentage of flagged signals is above
% the specified threshold. 
FiltIdx = PctFlagged >= PctThresh;

% Flag entire data frames corresponding to FiltIdx
PMUstruct.Flag(FiltIdx,SigIdx,FlagBit) = true;
% If desired, set entire data frames corresponding to FiltIdx to NaN
if SetToNaN
    PMUstruct.Data(FiltIdx,SigIdx) = NaN;
end