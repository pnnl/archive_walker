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
%Created by: Tao Fu(tao.fu@pnnl.gov) from DropOutZeroFilt.m

function [PMUstruct,setNaNMatrix] = WrappingFailureFilt(PMUstruct,SigsToFilt,Parameters,setNaNMatrix)

if(isfield(Parameters,'SetToNaN'))
    SetToNaN = strcmp(Parameters.SetToNaN,'TRUE');
else
    SetToNaN = 1;
end

if(isfield(Parameters,'FlagBit')) % value for the flag bit 
    FlagBit = str2num(Parameters.FlagBit);
else
    % no flag if it is not set in XML
    FlagBit = []; 
end

if(isfield(Parameters,'AngleThresh'))  % angle threshold
    AngleThreshold = str2double(Parameters.AngleThresh);
    if isnan(AngleThreshold)
        warning('The angle threshold for the angle wrapping data quality filter could not be converted to a number, setting to default of 90 degrees.');
        AngleThreshold = 90;
    end
else
    % set default angle threshold  to 90
    AngleThreshold = 90;
end

setNaNmatrixIni = zeros(size(setNaNMatrix));

% If specific signals were not listed, only apply to VAP signal
if isempty(SigsToFilt)
    SigsToFilt = PMUstruct.Signal_Name(strcmp(PMUstruct.Signal_Unit, 'DEG') | strcmp(PMUstruct.Signal_Unit, 'RAD'));
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
    currData = PMUstruct.Data(:,ThisSig);
    
    % If angles are in radians, convert to degrees before passing to
    % checkFailureAngle(). Note that this does not change the units of the
    % actual signal.
    if strcmp(PMUstruct.Signal_Unit{ThisSig},'RAD')
        currData = currData*pi/180;
    elseif ~strcmp(PMUstruct.Signal_Unit{ThisSig},'DEG')
        warning([SigsToFilt{SigIdx} ' is not an angle signal, the data quality filter for angle wrapping cannot be applied.']);
        continue;
    end
    
    % get data indices that angle paused during angle wrapping
    pauseIdx = checkFailureAngle(currData,AngleThreshold);
    
    if(~isempty(pauseIdx))
        if(~isempty(FlagBit))
            % Set flags for angle pause
            PMUstruct.Flag(pauseIdx,ThisSig,FlagBit) = true;
        end
        % If desired, replace zeros in Data matrix with NaN
        if SetToNaN
            setNaNmatrixIni(pauseIdx,ThisSig) = 1;
        end
    end
end
%setNaNmatrixIni has element '1' for the current PMU which
%is to be set to NaN for the current filter operation
%setNaNMatrix has non-zero positive elements for the current PMU which
%is to be set to NaN after all filter operation that has been carried out
setNaNMatrix = setNaNMatrix + setNaNmatrixIni;

end

function outIdx = checkFailureAngle(checkData,thresholdDeg)
% get data points that angles are close to 0 during angle wrapping

% figure(1);
% plot(checkData,'*-');
% get the differernce between every two points
diffData = abs(diff(checkData));

% set threshold degree
% Do not check final point to avoid an error in diffData(idx+1) below
k = find(diffData(1:end-1) > thresholdDeg);

% find data points that are discontinued with neighbor data points
check = zeros(length(k),2);
for i = 1:length(k)
    idx = k(i);
    if(diffData(idx) > thresholdDeg && diffData(idx+1) > thresholdDeg)
        check(i,1) = idx;
        check(i,2) = 1;
    else
        check(i,1) = idx;
    end
end
    
% check if identified data points are in the middle of wrapping (from 180 to -180 or from -180 to 180)
k = find(check(:,2) == 1);
if(~isempty(k))
    idx = check(k,1);
    idx = idx+1;
    for i = 1:length(idx)
        currIdx = idx(i);
        if(currIdx > 1 || currIdx < length(checkData)) % we are not do anything to the first and the last data point
            diffDeg = abs(checkData(currIdx-1)-checkData(currIdx+1)); 
            if(diffDeg > 180) 
                % set data to NaN if angle difference is larger than 180 
                checkData(currIdx) = NaN;                
            else
                % reset the flag to 0, no need to set the data to NaN    
                check(:,2) = 0;
            end
        end
    end    
end

k = find(check(:,2) == 1);
if(~isempty(k))
    outIdx = check(k,1);
    outIdx = outIdx+1;
else
    outIdx = [];
end

% figure(2);
% plot(checkData,'*-');


end

