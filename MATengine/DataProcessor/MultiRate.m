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

function [PMU_New, FinalCondos] = MultiRate(PMU,SigsToProc,p,q,PMU_New, InitialCondos)

NSig = size(PMU_New.Data,2);
% If specific signals were not listed, apply to all signals except
% digitals
if isempty(SigsToProc)
    SigIdx = find(~strcmp(PMU.Signal_Type, 'D'));
    SigsToProc= PMU.Signal_Name(SigIdx);
end

FinalCondos = cell(1,length(SigsToProc));
if isempty(InitialCondos)
    InitialCondos = cell(1,length(SigsToProc));
    for SigIdx = 1:length(SigsToProc)
        InitialCondos{SigIdx} = struct('Name',[],'delays',[],'bLO',[]);
    end
end

% If delays have been set to NaN due to missing or corrupt data reset to
% empty
for SigIdx = 1:length(SigsToProc)
    if sum(isnan(InitialCondos{SigIdx}.delays))>0
        InitialCondos{SigIdx}.delays = [];
    end
end

if isempty(PMU_New.Data)
    pp = p;
    qq = q;
    if isempty(pp)
        pp = 1;
    end
    if isempty(qq)
        qq = 1;
    end
    PMU_New.Data = NaN(round(size(PMU.Data,1)*pp/qq),length(SigsToProc));
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
        % Only use the initial conditions if the name of the channel is
        % correct
        if ~strcmp(SigsToProc{SigIdx}, InitialCondos{SigIdx}.Name)
            InitialCondos{SigIdx}.delays = [];
        end
        FinalCondos{SigIdx}.Name = SigsToProc{SigIdx};
        
        % Upsample
        temp = upsample(PMU.Data(:,ThisSig),p);
        
        %Lowpass filter setup
        if isempty(InitialCondos{SigIdx}.bLO)
            rp=0.1;
            rs=50;
            v = max([p q]);
            f=[0.6/v 1/v];  % Scaled as if fs=2 (default for firpmord)
            a=[1 0];
            dev = [(10^(rp/20)-1)/(10^(rp/20)+1)  10^(-rs/20)];
            [nLP,fo,ao,w] = firpmord(f,a,dev);
            if nLP > 1000
                error('The required filter order for this sampling rate conversion is greater than 1000. Implement in stages instead.');
            end
            bLO = firpm(nLP,fo,ao,w);
            
            InitialCondos{SigIdx}.bLO = bLO;
        end
        FinalCondos{SigIdx}.bLO = InitialCondos{SigIdx}.bLO;
        
        % If initial conditions are not available, make them available by
        % filtering some constant data with value equal to the first sample
        % of data. This will allow filter transient to die out.
        if isempty(InitialCondos{SigIdx}.delays)
            % The constant signal is given a length equal to the order of
            % the FIR filter
            [~, InitialCondos{SigIdx}.delays] = filter(InitialCondos{SigIdx}.bLO,1,temp(1)*ones(1,length(InitialCondos{SigIdx}.bLO)));
        end
            
        %Lowpass filter
        [temp, FinalCondos{SigIdx}.delays] = filter(InitialCondos{SigIdx}.bLO,1,temp,InitialCondos{SigIdx}.delays);
        
        % Downsample
        if q > length(temp)
            error('The specified sampling rate conversion is not compatible this signal length.');
        end
        if mod(length(temp),q) ~= 0
            error('The specified sampling rate conversion is not compatible this signal length.');
        end
        PMU_New.Data(:,NSig+SigIdx) = downsample(temp,q);
        
        
    elseif isempty(p)
        if mod(length(PMU.Data(:,ThisSig)),q) ~= 0
            error('The specified sampling rate conversion is not compatible this signal length.');
        end
        PMU_New.Data(:,NSig+SigIdx) = downsample(PMU.Data(:,ThisSig),q);
        FinalCondos = [];
    elseif isempty(q)
        PMU_New.Data(:,NSig+SigIdx) = upsample(PMU.Data(:,ThisSig),p);
        FinalCondos = [];
    end           
end

PMU_New.Stat = zeros(size(PMU_New.Data,1),1);
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