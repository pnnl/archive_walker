%   function GMSC_est = calcSelfGMSC(Data, ZeroPaddingLen, WindowOverlap, Window,OmegaB,Delay, NumberDelays)
%   This function estimates generalized maagnitude squared coherence
%
%   Inputs:
%           Data: Data file of dimension Number of data points by number of
%           channel
%           ZeroPaddingLen: Number of DFT points
%           WindowOverlap: Number of overlapped sample
%           Window: a vector containing data point for windowing signal
%           OmegaB: Frequency bins of interest
%           Delay = delay in samples used to calculate the self-GMSC
%           NumberDelays =  Number of delays in the self-GMSC
%
%   Outputs:
%           GMSC_est: Estimated generalized maagnitude squared coherence
%
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/18/2016

function [GMSC_est, FlagIndicator]  = calcSelfGMSC(Data, ZeroPaddingLen, WindowOverlap, Window,OmegaB,Delay, NumberDelays,AnalysisLength)
%dimensions of given data
[N,NSigs] = size(Data);

FlagIndicator = 0;

%initializing vector for GMSC estimate
GMSC_est = zeros(length(OmegaB),NSigs);

% DataStartingIdx = (0:Delay:Delay*(NumberDelays-1))+1;
if (N-Delay*(NumberDelays-1) - AnalysisLength) < 0
     FlagIndicator = 1;
     return;
end
DataSegement = zeros(AnalysisLength,NSigs,NumberDelays);
DataEndingIndex = (N-Delay*(NumberDelays-1)):Delay:N;
DataStartingIdx = DataEndingIndex - AnalysisLength + 1;

for NumDelayIdx = 1:NumberDelays
    DataSegement(:,:,NumDelayIdx) = Data(DataStartingIdx(NumDelayIdx):DataEndingIndex(NumDelayIdx),:);
end

for SigIdx = 1:NSigs
    % matrix containing delayed signal from one channel
    DataMat = squeeze(DataSegement(:,SigIdx,:));
    %calculates power spectrum of signal
    PSD = pwelch(DataMat,  Window, WindowOverlap, ZeroPaddingLen);
    %initializing matrix for magnitude squared coherence
    MSCohere = zeros(NumberDelays,NumberDelays,size(PSD,1));
    for Sig1Ind = 1:NumberDelays
        for Sig2Ind = Sig1Ind:NumberDelays
            %gives magnitude squared coherence between signal-1 and signal-2
            MSCohere(Sig1Ind,Sig2Ind,:) = cpsd(DataMat(:,Sig1Ind), DataMat(:,Sig2Ind), Window, WindowOverlap, ZeroPaddingLen)./sqrt(PSD(:,Sig2Ind).*PSD(:,Sig1Ind));
            % gives magnitude squared coherence between  signal-2 and signal-1
            MSCohere(Sig2Ind,Sig1Ind,:) = conj(MSCohere(Sig1Ind,Sig2Ind,:));
        end
    end   
%     a = mscohere(DataMat(:,1), DataMat(:,2), Window, WindowOverlap, ZeroPaddingLen);
%     GMSC_est(:,SigIdx)  = a(OmegaB);
    for FreqInd = 1:length(OmegaB)
        %gives maximum eigen value of matrix containing magnitude squared 
        % coherence of signals for each frequency bin
        EigMax = max(eig(MSCohere(:,:,OmegaB(FreqInd))));
        %gives estimate of GMSC for each frequency bin
        GMSC_est(FreqInd,SigIdx) = (1/(NumberDelays-1)*(EigMax-1))^2;
    end
%      
end


