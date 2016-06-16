% function [PMUstruct,setNaNMatrix] = StaleFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)
% This function flags stale data if the data in a channel has repeated values for
% more than a user selected threshold, and sets to NaN depending on user provided parameters
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
        % number of data by number of channels in the PMU)
        % PMUstruct.Flag: 3-dimensional matrix indicating PMU
        % measurement flagged by different filter operation (size: number 
        % of data points by number of channels by number of flag bits)                     
    % SigsToFilt: a cell array of strings specifying name of signals to be filtered.
    % Parameters: a struct array containing user provided information to
    % check data quality
        % Parameters.StaleThresh: Threshold for number of stale data
        % points, if exceeded then data is flagged
        % Parameters.SetToNaN: If TRUE, flagged data are set to NaN
        % Parameters.FlagBit: Flag bit for this filter operation
        % Parameters.FlagBitFreq: Additional flag bit if the flagged channel
        % is of type frequency
        % Parameters.FlagAllByFreq: If TRUE, all data points in a data
        % frame corresponding to the flagged frequency data point is
        % flagged
    % setNaNMatrix: Matrix of size: number of data points by number of
    % channels in a PMU. '0' indicates data is not to be set to NaN after
    % filter operation, any other value indicates data should be set to NaN
% Outputs:
    % PMUstruct
    % setNaNMatrix
%     
%Created by: Jim Follum(james.follum@pnnl.gov)
%Modified on June 7, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
    %1. Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix
    %2. data are set to NaN after carrying out all filter operation instead of setting data to NaN after each filter operation

function [PMUstruct,setNaNMatrix] = StaleFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)

StaleThresh = str2num(Parameters.StaleThresh);
FlagAllByFreq = strcmp(Parameters.FlagAllByFreq,'TRUE');
SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
FlagBit = str2num(Parameters.FlagBit);
FlagBitFreq = str2num(Parameters.FlagBitFreq);
setNaNmatrixIni = zeros(size(setNaNMatrix));

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
    for BadGroupIdx = 1:length(find(Dur >= StaleThresh))
        % Indices of stale data
        StaleIdx = delta0(StartIdx(BadGroupIdx)):delta0(EndIdx(BadGroupIdx))+1;
        
        % Flag stale data in the signal
        PMUstruct.Flag(StaleIdx,ThisSig,FlagBit) = true;
        % If desired, set stale data to NaN
        if SetToNaN
            setNaNmatrixIni(StaleIdx,ThisSig) = 1;
        end
        
        % If this is a frequency signal and the function inputs indicate that all
        % signals should be flagged if the frequency is stale, then do so.
        if (strcmp(PMUstruct.Signal_Type{ThisSig},'F') && FlagAllByFreq)
            PMUstruct.Flag(StaleIdx,:,FlagBitFreq) = true;
            % If desired, also set the data to NaN
            if SetToNaN
                setNaNmatrixIni(StaleIdx,:) = 1;
            end
        end
    end
    setNaNMatrix = setNaNMatrix + setNaNmatrixIni;
end