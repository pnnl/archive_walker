%   function GMSC_est = calcGMSC(Data, ZeroPaddingLen, WindowOverlap, Window, OmegaB)
%   This function estimates generalized maagnitude squared coherence
%
%   Inputs:
%           Data: Data file of dimension Number of data points by number of
%           channel
%           ZeroPaddingLen: Number of DFT points
%           WindowOverlap: Number of overlapped sample
%           Window: a vector containing data point for windowing signal
%           OmegaB: Frequency bins of interest
%
%   Outputs:
%           GMSC_est: Estimated generalized maagnitude squared coherence
%
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/18/2016

function GMSC_est = calcGMSC(Data, ZeroPaddingLen, WindowOverlap, Window, OmegaB, MedFiltOrd)
%dimensions of given data
NSigs = size(Data,2); 
%calculates power spectrum of signal
PSD = pwelch(Data, Window, WindowOverlap, ZeroPaddingLen); 
%initializing matrix for magnitude squared coherence
MSCohere = zeros(NSigs,NSigs,size(PSD,1)); 
for Sig1Ind = 1:NSigs
    for Sig2Ind = Sig1Ind:NSigs
        %gives magnitude squared coherence between signal-1 and signal-2
        MSCohere(Sig1Ind,Sig2Ind,:) = cpsd(Data(:,Sig1Ind), Data(:,Sig2Ind), Window, WindowOverlap, ZeroPaddingLen)./sqrt(PSD(:,Sig2Ind).*PSD(:,Sig1Ind)); 
        % gives magnitude squared coherence between  signal-2 and signal-1 
        MSCohere(Sig2Ind,Sig1Ind,:) = conj(MSCohere(Sig1Ind,Sig2Ind,:));
    end
end
%initializing vector for GMSC estimate
GMSC_est = zeros(length(OmegaB),1);
for FreqInd = 1:length(OmegaB)
    %gives maximum eigen value of matrix containing magnitude squared 
    % coherence spectra of signals for each frequency bin
    EigMax = max(eig(MSCohere(:,:,OmegaB(FreqInd)))); 
    %gives estimate of GMSC for each frequency bin
    GMSC_est(FreqInd) = (1/(NSigs-1)*(EigMax-1))^2; 
end

if ~isempty(MedFiltOrd)
    GMSC_est = medfilt1(GMSC_est,MedFiltOrd,'truncate');
end
