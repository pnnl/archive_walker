% PMUstruct is the structure in the common format for a single PMU
%
% SigsToFilt is cell array. Each cell is a string specifying a signal to be
% filtered.

% function PMUstruct = VoltPhasorFilt(PMUstruct,SigsToFilt,VoltMin,VoltMax,SetToNaN,FlagVal)
function PMUstruct = VoltPhasorFilt(PMUstruct,SigsToFilt,Parameters)

VoltMin = str2num(Parameters.VoltMin);
VoltMax = str2num(Parameters.VoltMax);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagVal = str2num(Parameters.FlagVal);

% If specific signals were not listed, apply to all voltages
if isempty(SigsToFilt)
    VmagIdx = strcmp(cellfun(@GetFirstTwo,PMUstruct.Signal_Type,'UniformOutput',false),'VM');
    SigsToFilt = PMUstruct.Signal_Name(VmagIdx);
end

for SigIdx = 1:length(SigsToFilt)
    ThisSig = find(strcmp(PMUstruct.Signal_Name,SigsToFilt{SigIdx}));
    
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToFilt{SigIdx} ' could not be found.']);
        continue
    end
    
    ThisSigAng = find(strcmp(PMUstruct.Signal_Name,strrep(SigsToFilt{SigIdx},'.MAG','.ANG')));
    
    % Make sure signal is a voltage magnitude. If not, throw an error.
    if strcmp(PMUstruct.Signal_Type{ThisSig}(1:2), 'VM')
        % Get nominal value
        if isfield(Parameters,'NomVoltage')
            % The nominal voltage was specified in the XML
            NomVoltage = Parameters.NomVoltage;
        else
%             warning('Nominal voltage not provided, assuming WISP naming to identify.');
            NomVoltage = strsplit(PMUstruct.Signal_Name{ThisSig},'.');
            NomVoltage = str2num(NomVoltage{2}(2:4));
            % Nominal value is in kV, adjust if signal is in V
            if strcmp(PMUstruct.Signal_Unit{ThisSig}, 'V')
                % Convert to V from kV
                NomVoltage = NomVoltage*1000;
            end
        end
        
        OutIdx = find((PMUstruct.Data(:,ThisSig) > VoltMax*NomVoltage) | (PMUstruct.Data(:,ThisSig) < VoltMin*NomVoltage));
        PMUstruct.Flag(OutIdx,ThisSig) = FlagVal;
        if SetToNaN
            PMUstruct.Data(OutIdx,ThisSig) = NaN;
        end
        
        % Flag corresponding angles
        if ~isempty(ThisSigAng)
            PMUstruct.Flag(OutIdx,ThisSigAng) = FlagVal;
            if SetToNaN
                PMUstruct.Data(OutIdx,ThisSigAng) = NaN;
            end
        end
    else
        error('Only voltage magnitudes can be filtered');
    end
end
end

function out = GetFirstTwo(In)
out = In(1:min([length(In),2]));
end