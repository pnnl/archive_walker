% function PMU = WrapAngle(PMU,SigsToFilt)
% This function wraps the angle measurements in a signal.
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
    % SigsToProc: a cell array of strings specifying name of signals to be
    % processed
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMU = WrapAngle(PMU,SigsToProc)

% If specific signals were not listed, apply to all angle signals 
if isempty(SigsToProc)
    SigIdx = find(strcmp(PMU.Signal_Unit, 'DEG') | strcmp(PMU.Signal_Unit, 'RAD'));
    SigsToProc = PMU.Signal_Name(SigIdx);
end

for SigIdx = 1:length(SigsToProc)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToProc{SigIdx}));
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToProc{SigIdx} ' could not be found.']);
        continue
    end
    % Wraps angle measurements
    if strcmp(PMU.Signal_Unit(ThisSig),'DEG')
        PMU.Data(:,ThisSig) = wrapTo180(PMU.Data(:,ThisSig));
    elseif strcmp(PMU.Signal_Unit(ThisSig),'RAD')
        PMU.Data(:,ThisSig) = wrapToPi(PMU.Data(:,ThisSig));
    else  
        warning(['Signal ' SigsToProc{SigIdx} ' is not an angle signal.']);
        continue
    end   
    
end
