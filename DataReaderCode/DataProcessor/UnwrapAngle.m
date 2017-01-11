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

function [PMU, FinalAngles] = UnwrapAngle(PMU,SigsToProc, PastAngles, MaxNaN)

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
    
    % If the duration of any group of NaN's is too long, set all
    % measurements afterward to NaN because they are unreliable.
    NaNloc = isnan(PMU.Data(:,ThisSig));
    Starts = find(diff([0; NaNloc]) == 1);
    Ends = find(diff(NaNloc) == -1);
    if length(Starts) > length(Ends)
        Ends = [Ends; length(PMU.Data(:,ThisSig))];
    end
    KillAfter = min(Ends(Ends - Starts + 1 > MaxNaN));
    % If KillAfter is empty, the following line won't do anything
    PMU.Data(KillAfter:end,ThisSig) = NaN;
    
    B4 = PMU.Data(end,ThisSig);
    
    %Unwraps angle measurements
    if strcmp(PMU.Signal_Unit(ThisSig),'DEG') 
        PMU.Data(:,ThisSig) = unwrap(PMU.Data(:,ThisSig)*pi/180)*180/pi;
    elseif strcmp(PMU.Signal_Unit(ThisSig),'RAD') 
        PMU.Data(:,ThisSig) = unwrap(PMU.Data(:,ThisSig));
    else 
        warning(['Signal ' SigsToProc{SigIdx} ' is not an angle signal.']);
    end
    
    
    if ((~isempty(PastAngles{SigIdx}.angle)) && (strcmp(SigsToProc{SigIdx}, PastAngles{SigIdx}.SignalName)))
        PMU.Data(:,ThisSig) = PMU.Data(:,ThisSig) + PastAngles{SigIdx}.angle;
    end
    
    FinalAngles{SigIdx}.angle = PMU.Data(end,ThisSig) - B4;
    if isnan(FinalAngles{SigIdx}.angle)
        FinalAngles{SigIdx}.angle = [];
    end
    FinalAngles{SigIdx}.SignalName = SigsToProc{SigIdx};
end
