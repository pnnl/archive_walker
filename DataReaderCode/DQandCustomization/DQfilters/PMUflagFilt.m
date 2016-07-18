% function [PMUstruct,setNaNMatrix] = PMUflagFilt(PMUstruct,Parameters,setNaNMatrix)
% This function flags each data frame for which PMU status indicate data quality issue, and sets to NaN
% depending on user provided parameters.
% 
% Inputs:
    % PMUstruct: structure in the common format for a single PMU
            % PMUstruct.Stat: a vector containing status of the PMU (size:
            % number of data points by 1)
            % PMUstruct.Flag: 3-dimensional matrix indicating PMU
            % measurement flagged by different filter operation (size: number 
            % of data points by number of data channels by number of flag bits)    
    % Parameters: a struct array containing user provided information to
    % check data quality
            % Parameters.SetToNaN: If TRUE, flagged data are set to NaN
            % Parameters.FlagBit: Flag bit for this filter operation
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

function [PMUstruct,setNaNMatrix] = PMUflagFilt(PMUstruct,Parameters,setNaNMatrix)
setNaNmatrixIni = zeros(size(setNaNMatrix));
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);

idx = find(PMUstruct.Stat >= 4096);
PMUstruct.Flag(idx,:,FlagBit) = true;
if SetToNaN
    setNaNmatrixIni(idx,:) = 1;
end
%setNaNmatrixIni has element '1' for the current PMU which
%is to be set to NaN for the current filter operation
%setNaNMatrix has non-zero positive elements for the current PMU which
%is to be set to NaN after all filter operation that has been carried out
setNaNMatrix = setNaNMatrix + setNaNmatrixIni;