% PMUstruct is the structure in the common format for a single PMU
%
% SigsToFilt is cell array. Each cell is a string specifying a signal to be
% filtered.

% function PMUstruct = StaleFilt(PMUstruct,SigsToFilt,StaleThresh,FlagAllByFreq,SetToNaN,FlagBit,FlagBitFreq)
function PMUstruct = StaleFilt(PMUstruct,SigsToFilt,Parameters)

StaleThresh = str2num(Parameters.StaleThresh);
FlagAllByFreq = strcmp(Parameters.FlagAllByFreq,'TRUE');
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);
FlagBitFreq = str2num(Parameters.FlagBitFreq);


% If specific signals were not listed, apply to all signals except digitals
% and scalars
if isempty(SigsToFilt)
    SigIdx = find(~strcmp(PMUstruct.Signal_Type, 'D') & ~strcmp(PMUstruct.Signal_Type, 'SC'));
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
    
    % Find the difference between each sequential measurement
    delta = diff(PMUstruct.Data(:,ThisSig));
    % Find where the difference between measurements was zero, indicating
    % stale data
    delta0 = find(delta==0);
    % Find the differences between the (almost) indices of stale data. This
    % helps identify separate groups of stale data.
    delta2 = diff(delta0);
    % Indices of the indices of the start of stale data
    StartIdx = ([1; find(delta2>1)+1]);
    % Indices of the indices of the end of stale data (almost)
    EndIdx = ([find(delta2>1); length(delta0)]);
    % Duration of the stale data
    Dur = EndIdx - StartIdx + 1 + 1;
    
    % If no measurement differences of zero were found, there's no stale
    % data.
    if isempty(delta0)
        Dur = [];
    end
    
    for BadGroupIdx = find(Dur >= StaleThresh)
        % Indices of stale data
        StaleIdx = delta0(StartIdx(BadGroupIdx)):delta0(EndIdx(BadGroupIdx))+1;
        
        % Flag stale data in the signal
        PMUstruct.Flag(StaleIdx,ThisSig,FlagBit) = true;
        % If desired, set stale data to NaN
        if SetToNaN
            PMUstruct.Data(StaleIdx,ThisSig) = NaN;
        end
        
        % If this is a frequency signal and the function inputs indicate that all
        % signals should be flagged if the frequency is stale, then do so.
        if (strcmp(PMUstruct.Signal_Type{ThisSig},'F') && FlagAllByFreq)
            PMUstruct.Flag(StaleIdx,:,FlagBitFreq) = true;
            % If desired, also set the data to NaN
            if SetToNaN
                PMUstruct.Data(StaleIdx,:) = NaN;
            end
        end
    end
end