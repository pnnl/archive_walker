% function PMU = WrapAngle(PMU,SigsToFilt)
% This function unwraps the angle measurements in a signal.
% 
% Inputs:
	% PMU: structure in the common format for a single PMU
        % PMU.Signal_Type: a cell array of strings specifying
        % signal(s) type in the PMU (size:1 by number of data channel)
        % PMU.Signal_Name: a cell array of strings specifying name of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMU.Signal_Unit: a cell array of strings specifying unit of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMU.Data: Matrix containing PMU measurements (size:
        % number of data points by number of channels in the PMU)
        % PMU.Flag: 3-dimensional matrix indicating PMU
        % measurements flagged by different filter operation (size: number 
        % of data points by number of channels by number of flag bits)        
    % SigsToFilt: a cell array of strings specifying name of signals to be
    % filtered
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

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
