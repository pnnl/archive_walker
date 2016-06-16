% function [PMUstruct,setNaNMatrix] = VoltPhasorFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)
% This function flags any data point of voltage magnitude signal that
% falls outside a user selected range, and sets flagged data to NaN
% depending on user provided parameters.
% 
% Inputs:
	% PMUstruct: structure in the common format for a single PMU
        % PMUstruct.Signal_Type: a cell array of strings specifying
        % signal(s) type in the PMU (size:1 by number of data channel)
        % PMUstruct.Signal_Name: a cell array of strings specifying name of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMUstruct.Signal_Unit: a cell array of strings specifying unit of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMUstruct.Data: Matrix containing PMU measurements (size:
        % number of data points by number of channels in the PMU)
        % PMUstruct.Flag: 3-dimensional matrix indicating PMU
        % measurements flagged by different filter operation (size: number 
        % of data points by number of channels by number of flag bits)                     
    % SigsToFilt: a cell array of strings specifying name of signals to be
    % filtered
    % Parameters: a struct array containing user provided information to
    % check data quality
        % Parameters.VoltMin: Minimum voltage magnitude for which voltage
        % data is not flagged
        % Parameters.VoltMax: Maximum voltage magnitude for which voltage
        % data is not flagged
        % Parameters.SetToNaN: If TRUE, flagged data are set to NaN
        % Parameters.FlagBit: Flag bit for this filter operation
        % Parameters.NomVoltage: Nominal voltage of the voltage signal
    % setNaNMatrix: Matrix of size: number of data points by number of
    % channels in a PMU. '0' indicates data is not to be set to NaN after
    % filter operation, any other value indicates data should be set to NaN
% 
% Outputs:
    % PMUstruct
    % setNaNMatrix
%     
%Created by: Jim Follum(james.follum@pnnl.gov)
%Modified on June 7, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
    %1. Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix
    %2. data are set to NaN after carrying out all filter operation instead of setting data to NaN after each filter operation

function [PMUstruct,setNaNMatrix] = VoltPhasorFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)

VoltMin = str2num(Parameters.VoltMin);
VoltMax = str2num(Parameters.VoltMax);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);
setNaNmatrixIni = zeros(size(setNaNMatrix));
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
            NomVoltage = Parameters.NomVoltage/sqrt(3);
        else
%             warning('Nominal voltage not provided, assuming WISP naming to identify.');
            NomVoltage = strsplit(PMUstruct.Signal_Name{ThisSig},'.');
            NomVoltage = str2num(NomVoltage{2}(2:4))/sqrt(3);
            % Nominal value is in kV, adjust if signal is in V
            if strcmp(PMUstruct.Signal_Unit{ThisSig}, 'V')
                % Convert to V from kV
                NomVoltage = NomVoltage*1000; %changes voltage from line-to-line to per phase
            end
        end
        
        OutIdx = find((PMUstruct.Data(:,ThisSig) > VoltMax*NomVoltage) | (PMUstruct.Data(:,ThisSig) < VoltMin*NomVoltage));
        PMUstruct.Flag(OutIdx,ThisSig,FlagBit) = true;
        if SetToNaN
            setNaNmatrixIni(OutIdx,ThisSig) = 1;
        end
        
        % Flag corresponding angles
        if ~isempty(ThisSigAng)
            PMUstruct.Flag(OutIdx,ThisSigAng,FlagBit) = true;
            if SetToNaN
                setNaNmatrixIni(OutIdx,ThisSigAng) = 1;
            end
        end
    else
        error('Only voltage magnitudes can be filtered');
    end
end
setNaNMatrix = setNaNMatrix + setNaNmatrixIni;
end

function out = GetFirstTwo(In)
out = In(1:min([length(In),2]));
end