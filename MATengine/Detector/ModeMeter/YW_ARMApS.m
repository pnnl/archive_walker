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
na = Parameters.na;
nb = Parameters.nb;
NaNomitLimit = Parameters.NaNomitLimit;

TimeLoc(isnan(FOfreq),:) = [];
FOfreq(isnan(FOfreq)) = [];

if isempty(FOfreq)
    L = Parameters.L;
else
    L = Parameters.LFO;
end
P = length(FOfreq);

%% Handle the case where y contains NaN

% If there are NaNs in y, handle them according to the user's input
nanLoc = isnan(y);
if sum(nanLoc) > 0
    if sum(nanLoc) < NaNomitLimit
        % There are few enough NaNs that they can be omitted
        % Using the window, remove any samples that are NaN
        w(nanLoc) = 0;
        % For the window to work, y(nanLoc)*0 must equal 0, but nan*0 =
        % nan. So, replace the NaNs in y with an arbitrary value.
        y(nanLoc) = 0;
    else
        % There are too many NaNs in the input signal - return NaN for the mode
        % estimate
        ModeEst = NaN; 
        Mtrack{length(Mtrack)+1} = NaN;
        return
    end
end

%% Calculate the duration of each FO, accounting for the impact of windowing

m = zeros(1,P);
for p = 1:P
    m(p) = sum(w(TimeLoc(p,1):TimeLoc(p,2)) > 0);
end

% Remove any FOs that are completely windowed out
KillIdx = m == 0;
FOfreq(KillIdx) = [];
m(KillIdx) = [];

P = length(FOfreq);

%% Remove leading and trailing values that are to be removed by windowing
KeepIdx = find(w,1):find(w,1,'last');
if length(KeepIdx) <= 2*nb+2*L
    % Too much of the analysis window is to be removed, so return NaN for the
    % mode estimate
    ModeEst = NaN; 
    Mtrack{length(Mtrack)+1} = NaN;
    return
else
    % Remove samples at beginning and end that are to be windowed out
    % anyway
    w = w(KeepIdx);
    y = y(KeepIdx);
end

%% Estimate autocorrelation of y
r = xcorrWin(y,w,nb+L);

IdxAdj = nb+L+1;    % Amount added to each index given in the dissertation to match Matlab

%% Implement first part of YW-ARMA+S2 to get AR and transformed FO parameters

rbar = r(nb+1+IdxAdj:nb+L+IdxAdj);  % "data" vector (see eq. (3.58))

R = toeplitz(r(nb+IdxAdj:nb+L-1+IdxAdj),r(nb+IdxAdj:-1:nb-na+1+IdxAdj));    % ACF matrix (see eq. (3.59))

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