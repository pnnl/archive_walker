% function [PMUstruct,setNaNMatrix] = OutlierFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)
% This function detects and flags outlier in a signal excluding angle
% measurements. Angle measurements of a signal are flagged corresponding to the
% magnitude measurements of the same signal.
% 
% Inputs:
	% 
	% PMUstruct: structure in the common format for a single PMU
        % PMUstruct.Signal_Type: a cell array of strings specifying
        % signal(s) type in the PMU (size:1 by number of data channel)
        % PMUstruct.Signal_Name: a cell array of strings specifying name of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMUstruct.Signal_Unit: a cell array of strings specifying unit of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMUstruct.Data: Matrix containing PMU measurements (size:
        % number of data by number of channels in the PMU)
        % PMUstruct.Flag: 3-dimensional matrix indicating PMU
        % measurement flagged by different filter operation (size: number 
        % of data points by number of channels by number of flag bits)                     
    % SigsToFilt: a cell array of strings specifying name of signals to be
    % filtered
    % Parameters: a struct array containing user provided information to
    % check data quality
        % Parameters.StdDevMult: This parameter times the standard
        % deviation of data determines the range for identifying outliers.
        % Parameters.SetToNaN: If TRUE, flagged data are set to NaN
        % Parameters.FlagBit: Flag bit for this filter operation
    % setNaNMatrix: Matrix of size: number of data points by number of
    % channels in a PMU. '0' indicates data is not to be set to NaN after
    % filter operation, any other value indicates data should be set to NaN.
%  
% Outputs:
    % PMUstruct
    % setNaNMatrix
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function [PMUstruct,setNaNMatrix] = OutlierFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)

StdDevMult = str2num(Parameters.StdDevMult);
% AngleDev = str2num(Parameters.AngDev);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);

% If specific signals were not listed, apply to all signals except 
% digitals, scalars, and rocof
if isempty(SigsToFilt)
    SigIdx = find(~strcmp(PMUstruct.Signal_Type, 'D'));
    SigsToFilt = PMUstruct.Signal_Name(SigIdx);
end

setNaNmatrixIni = zeros(size(setNaNMatrix));

for SigIdx = 1:length(SigsToFilt)
    
    ThisSig = find(strcmp(PMUstruct.Signal_Name,SigsToFilt{SigIdx}));
    
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToFilt{SigIdx} ' could not be found.']);
        continue
    end
    OutIdx = [];
    if ~strcmp(PMUstruct.Signal_Unit(ThisSig),'DEG') || strcmp(PMUstruct.Signal_Unit(ThisSig),'RAD')
        OutIdx = find(abs(PMUstruct.Data(:,ThisSig) - median(PMUstruct.Data(:,ThisSig))) > StdDevMult*std(PMUstruct.Data(:,ThisSig))); % Find outlier idx
        if strcmp(PMUstruct.Signal_Type{ThisSig}(1:end-1), 'VM') || strcmp(PMUstruct.Signal_Type{ThisSig}(1:end-1), 'IM')
            ThisSigAng = find(strcmp(PMUstruct.Signal_Name,strrep(SigsToFilt{SigIdx},'.MAG','.ANG')));
            if ~isempty(ThisSigAng)
                PMUstruct.Flag(OutIdx,ThisSigAng,FlagBit) = true;
                if SetToNaN
                    setNaNmatrixIni(OutIdx,ThisSigAng) = 1;
                end
            end
        end
                    
    end
    % Set flags for outliers
    PMUstruct.Flag(OutIdx,ThisSig,FlagBit) = true;
    % If desired, replace outliers in Data matrix with NaN
    if SetToNaN
        setNaNmatrixIni(OutIdx,ThisSig) = 1;
    end
end
%setNaNmatrixIni has element '1' for the current PMU which
%is to be set to NaN for the current filter operation
%setNaNMatrix has non-zero positive elements for the current PMU which
%is to be set to NaN after all filter operation that has been carried out
setNaNMatrix = setNaNMatrix + setNaNmatrixIni;