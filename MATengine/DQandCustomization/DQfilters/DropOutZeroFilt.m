% function [PMUstruct,setNaNMatrix] = DropOutZeroFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)
% This function flags the data point that equals zero.
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
        % Parameters.SetToNaN: If TRUE, flagged data are set to NaN
        % Parameters.FlagBit: Flag bit for this filter operation.
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

function [PMUstruct,setNaNMatrix] = DropOutZeroFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)

SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);

setNaNmatrixIni = zeros(size(setNaNMatrix));

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
        setNaNmatrixIni(ZeroIdx,ThisSig) = 1;
    end
end
%setNaNmatrixIni has element '1' for the current PMU which
%is to be set to NaN for the current filter operation
%setNaNMatrix has non-zero positive elements for the current PMU which
%is to be set to NaN after all filter operation that has been carried out
setNaNMatrix = setNaNMatrix + setNaNmatrixIni;