% function [PMUstruct,setNaNMatrix] = DataFrameFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)
% This function flags PMU data of an entire data frame if the number of flagged data points
% in that data frame exceeds a user specified threshold 
% 
% Inputs:
	% PMUstruct: structure in the common format for a single PMU
        % PMUstruct.Signal_Type: a cell array of strings specifying
        % signal(s) type in the PMU (size:1 by number of data channel)
        % PMUstruct.Signal_Name: a cell array of strings specifying name of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMUstruct.Flag: 3-dimensional matrix indicating PMU
        % measurements flagged by different filter operation (size: number 
        % of data points by number of channels by number of flag bits) 
    % Parameters: a struct array containing user provided information to
    % check data quality
        % Parameters.PercentBadThresh: Threshold for number of flagged data
        % points in a data frame, if exceeded then entire data frame is
        % flagged
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

function [PMUstruct,setNaNMatrix] = DataFrameFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)

PctThresh = str2num(Parameters.PercentBadThresh);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);
setNaNmatrixIni = zeros(size(setNaNMatrix));
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
FlagBin = sum(PMUstruct.Flag(:,SigIdx,:),3);
FlagBin(FlagBin > 0) = true;

% For each frame, the percent of channels that have been flagged
PctFlagged = sum(FlagBin,2)./NumSigs*100;

% Indices of data frames where the percentage of flagged signals is above
% the specified threshold. 
FiltIdx = PctFlagged >= PctThresh;

% Flag entire data frames corresponding to FiltIdx
PMUstruct.Flag(FiltIdx,SigIdx,FlagBit) = true;
% If desired, set entire data frames corresponding to FiltIdx to NaN
if SetToNaN
    setNaNmatrixIni(FiltIdx,SigIdx) = 1;
end
setNaNMatrix = setNaNMatrix + setNaNmatrixIni;