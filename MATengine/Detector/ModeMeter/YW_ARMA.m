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

function [ModeEst, Mtrack] = YW_ARMA(y,w,Parameters,DesiredModes,fs,Mtrack,~)

%% Preliminaries
y = y(:); % Make sure y  is a column vector
na = Parameters{1}.na;
nb = Parameters{1}.nb;
L = Parameters{1}.L;

%% Estimate autocorrelation of y
% r = xcorr(y,y,nb+L,'biased');

% Handles zeros in y for robustness
N = length(y);
r = zeros(nb+L+1,1);
for lag = 0:nb+L
    % New version handles windows with values other than 0 and 1
    temp = w(lag+1:N).*y(lag+1:N).*w(1:N-lag).*y(1:N-lag);
    r(lag+1) = sum(temp)/(sum(w(lag+1:N).*w(1:N-lag))); % unbiased estimate
    
    % Older version that only handled windows with values of 0 or 1
%     temp = y(l+1:N).*y(1:N-l);
%     r(l+1) = sum(temp)/(N-sum(temp==0));
end
r = [r(end:-1:2); r];

IdxAdj = nb+L+1;    % Amount added to each index given in the dissertation to match Matlab

%% Implement first part of YW-ARMA+S2 to get AR and transformed FO parameters

rbar = r(nb+1+IdxAdj:nb+L+IdxAdj);  % "data" vector (see eq. (3.58))

R = toeplitz(r(nb+IdxAdj:nb+L-1+IdxAdj),r(nb+IdxAdj:-1:nb-na+1+IdxAdj));    % ACF matrix (see eq. (3.59))


theta = -pinv(R)*rbar;
a = [1; theta(1:na)];

[ModeEst, Mtrack] = SelectMode(a,fs,DesiredModes,Mtrack);