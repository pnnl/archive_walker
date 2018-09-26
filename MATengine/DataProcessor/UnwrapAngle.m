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
    % SigsToProc: a cell array of strings specifying name of signals to be
    % processed
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function [PMU, FinalAngles] = UnwrapAngle(PMU,SigsToProc, PastAngles)

% If specific signals were not listed, apply to all angle signals 
if isempty(SigsToProc)
    SigIdx = find(strcmp(PMU.Signal_Unit, 'DEG') | strcmp(PMU.Signal_Unit, 'RAD'));
    SigsToProc = PMU.Signal_Name(SigIdx);
end

FinalAngles = cell(1,length(SigsToProc));
for SigIdx = 1:length(SigsToProc)
    FinalAngles{SigIdx} = struct('SignalName',[],'angle',[]);
end
if isempty(PastAngles)
    PastAngles = cell(1,length(SigsToProc));
    for SigIdx = 1:length(SigsToProc)
        PastAngles{SigIdx} = struct('SignalName',[],'angle',[]);
    end
end
for SigIdx = 1:length(SigsToProc)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToProc{SigIdx}));
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToProc{SigIdx} ' could not be found.']);
        continue
    end
    
    % The function unwrap assumes the angles are in radians. piScale is
    % used to transform angles into radians. If already in radians, the
    % scaling has no effect.
    if strcmp(PMU.Signal_Unit(ThisSig),'DEG') 
        piScale = 180;
    elseif strcmp(PMU.Signal_Unit(ThisSig),'RAD') 
        piScale = pi;
    else 
        error(['Signal ' SigsToProc{SigIdx} ' is not an angle signal.']);
    end
    
    % Unwrap the signal
    % If the final measurement from the last file is available, start the
    % unwrap with it to provide continuity.
    if ((~isempty(PastAngles{SigIdx}.angle)) && (strcmp(SigsToProc{SigIdx}, PastAngles{SigIdx}.SignalName)))
        Temp = unwrap([PastAngles{SigIdx}.angle; PMU.Data(:,ThisSig)]*pi/piScale)*piScale/pi;
        PMU.Data(:,ThisSig) = Temp(2:end);
    else
        PMU.Data(:,ThisSig) = unwrap(PMU.Data(:,ThisSig)*pi/piScale)*piScale/pi;
    end
    
    % Store the final angle of the unwrapped signal for use when unwrapping
    % the next file.
    MostRecentGoodIdx = find(~isnan(PMU.Data(:,ThisSig)),1,'last');
    % Store this signal's most recent non-NaN value in FinalAngles. If the
    % signal does not have a non-NaN value, then don't update it (pass the
    % PastAngles value to FinalAngles).
    if ~isempty(MostRecentGoodIdx)
        FinalAngles{SigIdx}.angle = PMU.Data(MostRecentGoodIdx,ThisSig);
    else
        FinalAngles{SigIdx}.angle = PastAngles{SigIdx}.angle;
    end
    FinalAngles{SigIdx}.SignalName = SigsToProc{SigIdx};
end
