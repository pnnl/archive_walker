% function [PMUstruct,setNaNMatrix] = FreqFilt(PMUstruct,Parameters,setNaNMatrix)
% This function checks data points in frequency signal that falls outside a user selected
% range, and flags data and sets flagged data to NaN depending on user provided
% parameters
% 
% Inputs:
	% PMUstruct: structure in the common format for a single PMU
        % PMUstruct.Data: Matrix containing PMU measurements (size:
        % number of data points by number of channels in the PMU)
        % PMUstruct.Flag: 3-dimensional matrix indicating PMU
        % measurements flagged by different filter operation (size: number 
        % of data points by number of channels by number of flag bits)      
    % Parameters: a struct array containing user provided information to
    % check data quality
        % Parameters.FreqMinChan: Minimum value of frequency to be in
        % normal range for checking overall data quality of channel
        % Parameters.FreqMaxChan: Maximum value of frequency to be in
        % normal range for checking overall data quality of channel
        % Parameters.FreqPctChan: Threshold for percent of frequency
        % measurements outside normal range, if exceeded then the data
        % channel is flagged
        % Parameters.FreqMinSamp: Minimum value of frequency to be in
        % normal range for checking individual measurement sample
        % Parameters.FreqMaxSamp: Maximum value of frequency to be in
        % normal range for checking individual measurement sample
        % Parameters.SetToNaN: If TRUE, flagged data are set to NaN
        % Parameters.FlagBitChan: Flag bit for checking overall data
        % quality of the channel
        % Parameters.FlagBitSamp: Flag bit for checking data quality of
        % individual frequency sample
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

function [PMUstruct,setNaNMatrix] = FreqFilt(PMUstruct,Parameters,setNaNMatrix)

FreqMinChan = str2num(Parameters.FreqMinChan);
FreqMaxChan = str2num(Parameters.FreqMaxChan);
FreqPctChan = str2num(Parameters.FreqPctChan);
FreqMinSamp = str2num(Parameters.FreqMinSamp);
FreqMaxSamp = str2num(Parameters.FreqMaxSamp);
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBitChan = str2num(Parameters.FlagBitChan);
FlagBitSamp = str2num(Parameters.FlagBitSamp);

% Find all frequency signals. The filter is applied to each one.
SigIdx = find(strcmp(PMUstruct.Signal_Type, 'F'));

setNaNmatrixIni = zeros(size(setNaNMatrix));

for ThisSig = SigIdx
    % Stage 1 - Overall DQ of channel
    OutIdx = (PMUstruct.Data(:,ThisSig) > FreqMaxChan) | (PMUstruct.Data(:,ThisSig) < FreqMinChan);
    if sum(OutIdx)/length(OutIdx)*100 > FreqPctChan
        % Too many points are ouside the tight bound - remove the whole
        % channel
        PMUstruct.Flag(:,ThisSig,FlagBitChan) = true;
        if SetToNaN
            setNaNmatrixIni(:,ThisSig) = 1;
        end
    end
    
    % Stage 2 - individual measurements
    OutIdx = find((PMUstruct.Data(:,ThisSig) > FreqMaxSamp) | (PMUstruct.Data(:,ThisSig) < FreqMinSamp));
    PMUstruct.Flag(OutIdx,ThisSig,FlagBitSamp) = true;
    if SetToNaN
        setNaNmatrixIni(OutIdx,ThisSig) = 1;
    end
end
setNaNMatrix = setNaNMatrix + setNaNmatrixIni;