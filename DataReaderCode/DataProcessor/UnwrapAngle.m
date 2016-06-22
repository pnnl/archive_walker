% function [PMUstruct,setNaNMatrix] = ChanFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)
% This function flags PMU data of an entire data channel if the number of flagged data points
% in that channel exceeds a user specified threshold 
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
        % points in a data channel, if exceeded then entire data channel is
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

function PMU = UnwrapAngle(PMU,SigsToFilt)

% If specific signals were not listed, apply to all signals 
if isempty(SigsToFilt)
    SigIdx = find(strcmp(PMU.Signal_Unit, 'DEG') | strcmp(PMU.Signal_Unit, 'RAD'));
    SigsToFilt = PMU.Signal_Name(SigIdx);
end

for SigIdx = 1:length(SigsToFilt)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToFilt{SigIdx}));
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToFilt{SigIdx} ' could not be found.']);
        continue
    end
    
    if strcmp(PMU.Signal_Unit(ThisSig),'DEG')        
        PMU.Data(:,ThisSig) = unwrap(PMU.Data(:,ThisSig)*pi/180)*180/pi;
    elseif strcmp(PMU.Signal_Unit(ThisSig),'RAD') 
        PMU.Data(:,ThisSig) = unwrap(PMU.Data(:,ThisSig));
    else 
        warning(['Signal ' SigsToFilt{SigIdx} ' is not an angle signal.']);
        continue
    end       
end
