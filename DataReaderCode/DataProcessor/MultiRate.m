% function PMU_NewPMUidx = MultiRate(PMU,SigsToProc,p,q,PMU_NewPMUidx)
% This function carries out datarate change operation on a single PMU measurements 
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
    % p: factor by which data is upsampled
    % q: factor by which data is downsampled
    % PMU_New: Struct array for PMU containing data after rate change
%
% Outputs:
% PMU_New 
%
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMU_New = MultiRate(PMU,SigsToProc,p,q,PMU_New)
NSig = size(PMU_New.Data,2);
% If specific signals were not listed, apply to all signals except
% digitals
if isempty(SigsToProc)
    SigIdx = find(~strcmp(PMU.Signal_Type, 'D'));
    SigsToProc= PMU.Signal_Name(SigIdx);
end

for SigIdx = 1:length(SigsToProc)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToProc{SigIdx}));
    
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToProc{SigIdx} ' could not be found.']);
        continue
    end
    PMU_New.Signal_Name(NSig+SigIdx) = PMU.Signal_Name(ThisSig);
    PMU_New.Signal_Type(NSig+SigIdx) = PMU.Signal_Type(ThisSig);
    PMU_New.Signal_Unit(NSig+SigIdx) = PMU.Signal_Unit(ThisSig);       
    FlagMat = getFlagMat(PMU.Flag(:,ThisSig,:), p,q);
    PMU_New.Flag( 1:size(FlagMat,1), NSig+SigIdx,1:size(FlagMat,3))  = FlagMat;
    if ~isempty(p) &&  ~isempty(q)
        PMU_New.Data(:,NSig+SigIdx) = resample(PMU.Data(:,ThisSig),p,q);
    elseif isempty(p)
        PMU_New.Data(:,NSig+SigIdx) = downsample(PMU.Data(:,ThisSig),q);    
    elseif isempty(q)
        PMU_New.Data(:,NSig+SigIdx) = upsample(PMU.Data(:,ThisSig),p);
    end           
end
end


% function FlagMat = getFlagMat(FlagVec,p,q)
% This function flags appropriate data points after data rate change of a data channel
%
%Created on 06/28/2016 by Urmila Agrawal (urmila.agrawal@pnnl.gov)

function FlagMat = getFlagMat(FlagVec,p,q)
if isempty(p)
    p = 1;
end
if isempty(q)
    q = 1;
end
[N,~,FlagCol]= size(FlagVec);
FlagMat = false(p*N,1,FlagCol);
for colInd = 1:FlagCol
    FlagInd = find(FlagVec(:,1,colInd) > 0);
    NewFlagInd = (FlagInd-1)*p +1;
    FlagLowerBound =  NewFlagInd- (p-1);
    FlagLowerBound(FlagLowerBound<1) = 1;
    FlagUpperBound =  NewFlagInd + (p-1);
    FlagIdx = [];
    for groupIdx = 1:length(FlagLowerBound)
        FlagIdx = [FlagIdx FlagLowerBound(groupIdx):FlagUpperBound(groupIdx)];
    end
    FlagVecUpsampled = sort(unique(FlagIdx(:)),'ascend');
    FlagMat(FlagVecUpsampled,1,colInd) = true;
end
FlagMat = FlagMat(1:q:end,1,:);
end


