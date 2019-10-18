% [theta,sig2_YW,rhat] = YW_ARMApS(y,fs,L,na,nb,n_gamma,f,eps,eta)
%
% This function implements the YW-ARMA+S algorithm as described in Section
% 3.2. This is an adapted OMYW algorithm that incorporates the
% presence of a sinusoid in the signal y. The standard OMYW
% algorithm is implemented if f=[].
%
% INPUTS:
% y = signal to be analyzed
% fs = sampling rate of y
% L = number of normal equations to use
% na = AR model order
% nb = MA model order
% n_gamma = exaggerated AR model order for MA(+S) process in second part of
%           algorithm
% f = vector of frequencies of sinusoids in FO. If empty, standard OMYW is
%     implemented
% eps = vector of starting samples for the sinusoids with frequencies in f.
%       Samples correspond to the indices of y. Must be the same dimension as f
% eta = vector of ending samples for the sinusoids with frequencies in f.
%       Samples correspond to the indices of y. Must be the same dimension as f
%
% OUTPUTS:
% theta = parameter vector containing AR and MA coefficients and the
%         transformed FO parameters U and V (if f is not empty). Note that
%         a_0=1 and b_0=1 are not included.
% sig2_YW = estimate of the process noise variance
% rhat = column one is estimated autocorrelation of y. Column two is 
%        reconstructed autocorrelation of y based on identified model

function [ModeEst, Mtrack] = YW_ARMApS(y,w,Parameters,DesiredModes,fs,Mtrack,FOfreq,TimeLoc)

%% Preliminaries
y = y(:); % Make sure y  is a column vector
na = Parameters{1}.na;
nb = Parameters{1}.nb;

TimeLoc(isnan(FOfreq),:) = [];
FOfreq(isnan(FOfreq)) = [];

if isempty(FOfreq)
    L = Parameters{1}.L;
else
    L = Parameters{1}.LFO;
end
P = length(FOfreq);
%% Estimate autocorrelation of y
r = xcorrWin(y,w,nb+L);
r = r';

IdxAdj = nb+L+1;    % Amount added to each index given in the dissertation to match Matlab

%% Implement first part of YW-ARMA+S2 to get AR and transformed FO parameters

rbar = r(nb+1+IdxAdj:nb+L+IdxAdj);  % "data" vector (see eq. (3.58))

R = toeplitz(r(nb+IdxAdj:nb+L-1+IdxAdj),r(nb+IdxAdj:-1:nb-na+1+IdxAdj));    % ACF matrix (see eq. (3.59))

m = diff(TimeLoc,1,2) + 1;  % Durations of each sinusoid in FO
SM = zeros(L,2*P);  % S matrix (see eq. (3.60)) .* M matrix (see eq. (3.61))
for p = 1:P
    Mcol = (m(p)-nb-1:-1:m(p)-nb-L)'; % Corresponding column of M matrix
    Mcol(Mcol < 0) = 0;         % If lag value large enough, sinusoidal portion dies out (see eq. (3.43))
    
    SM(:,2*p-1) = cos(2*pi*FOfreq(p)/fs*(nb+1:nb+L)').*Mcol;
    SM(:,2*p) = -sin(2*pi*FOfreq(p)/fs*(nb+1:nb+L)').*Mcol;
end

theta = pinv([-R SM])*rbar;
a = [1; theta(1:na)];

[ModeEst, Mtrack] = SelectMode(a,fs,DesiredModes,Mtrack);