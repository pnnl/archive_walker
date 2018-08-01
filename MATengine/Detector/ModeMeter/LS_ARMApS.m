% [theta,sig2_LS,yhat] = LS_ARMApS(y,fs,f,eps,eta,n_alpha,n_a,n_b)
%
% This function implements the LS-ARMA+S algorithm as described in Section
% 3.2. This is an adapted two-stage LS algorithm that incorporates the
% presence of a sinusoid in the signal y. The standard two-stage LS
% algorithm is implemented if f=[]. If n_a or n_b are omitted, only
% the first stage is implemented.
%
% INPUTS:
% y = signal to be analyzed
% fs = sampling rate of y
% f = vector of sinusoidal frequencies. Standard two-stage LS algorithm is
%     implemented if f = []
% eps = vector of starting samples for the sinusoids with frequencies in f.
%       Samples correspond to the indices of y. Must be the same dimension as f
% eta = vector of ending samples for the sinusoids with frequencies in f.
%       Samples correspond to the indices of y. Must be the same dimension as f
% n_alpha = exaggerated AR model order used in first stage
% n_a = AR model order. Omit to run only the first stage.
% n_b = MA model order. Omit to run only the first stage.
%
% OUTPUTS:
% theta = estimated parameter vector. If only stage 1 is implemented it
%         contains exaggerated AR coefficients and B and C terms
%         corresponding to the sinusoids (see eq (3.16)). Note that 
%         alpha_0=1 is not included in the vector. If both stages
%         are implemented it contains AR coefficients, MA coefficients, 
%         and B and C terms corresponding to the sinusoids (see eq (3.14)).
%         Note that a_0=1 and b_0=1 are not included in the vector.
% sig2_LS = estimated variance of the process noise. The estimate is made
%         in stage 1 and then updated in stage 2. 
% yhat = reconstructed version of input signal y based on identified model

function [ModeEst, Mtrack] = LS_ARMApS(y,Parameters,DesiredModes,fs,Mtrack,FOfreq)

%% Preliminaries
y = y(:); % Make sure y  is a column vector
na = Parameters{1}.na;
nb = Parameters{1}.nb;
n_alpha = Parameters{1}.n_alpha;
W = length(y);  % Estimation segment length
FOfreq(isnan(FOfreq)) = [];
P = length(FOfreq);
%% Stage 1
L = n_alpha;    % Term that specifies the number of equations to be used

ybar = y(L+1:W);    % Data vector (see eq. (2.49))

Y = -toeplitz(y(L:W-1),y(L:-1:1));   % Data matrix (see eq. (2.50))

% Form S matrix (see eq. (3.19))
% Note that k_i has been replaced with i. This will impact the phase
% estimates!
S = zeros(W-L,2*P);
for p = 1:P
    % The psi term is used to incorporate the starting and ending samples
    % of each sinusoid. Each psi is the corresponding column of Psi in
    % eq. (3.20)
    psi = zeros(1,W);
    psi(eps(p):eta(p)) = 1;
    psi = psi(L+1:W);
    S(:,2*p-1) = cos(2*pi*FOfreq(p)/fs*(L+1:W)).*psi;
    S(:,2*p) = -sin(2*pi*FOfreq(p)/fs*(L+1:W)).*psi;
end

Z = [Y S];

theta = pinv(Z)*ybar;   % Estimate of reduced parameter vector
a = [1; theta(1:L)];   % AR coefficients

e = [zeros(L,1); ybar-Z*theta];    % Estimates of process noise (see eq. (3.22))
                                   % Add zeros to make indexing easier
                                   % (does not impact sig2_LS below)


%% Stage 2
if nb > 0
    L = n_alpha + nb;  % Updated term that specifies the number of equations to be used
    
    ybar = y(L+1:W);    % Data vector (see eq. (2.49))

    Y = -toeplitz(y(L:W-1),y(L:-1:L-na+1));   % Data matrix (see eq. (2.62))
    
    E = toeplitz(e(L:W-1),e(L:-1:L-nb+1));   % Process noise matrix (see eq. (2.63))
    
    % Form S matrix (see eq. (3.19))
    % Note that k_i has been replaced with i. This will impact the phase
    % estimates!
    S = zeros(W-L,2*P);
    for p = 1:P
        S(:,2*p-1) = cos(2*pi*FOfreq(p)/fs*(L+1:W));
        S(:,2*p) = -sin(2*pi*FOfreq(p)/fs*(L+1:W));
    end
    
    Z = [Y E S];

    theta = pinv(Z)*ybar;   % Estimate of reduced parameter vector
    a = [1; theta(1:na)];   % AR coefficients
end

%% Select modes


% Find all poles (includes spurious roots)
zPoles = roots(a);  % Find z-domain poles (see eq. (2.8))
sPoles = log(zPoles)*fs;    % Transform to s-domain (see eq. (2.7))


sPolesDamp = -cos(angle(sPoles))*100;
sPolesFreq = imag(sPoles)/(2*pi);


% 0: Remove all modes outside specified frequency range.
sPolesTemp = sPoles;
sPolesFreqTemp = sPolesFreq;
sPolesDampTemp = sPolesDamp;
KillIdx = [find(sPolesFreqTemp < DesiredModes(1)); find(sPolesFreqTemp > DesiredModes(2))];
sPolesTemp(KillIdx) = [];
sPolesFreqTemp(KillIdx) = [];
sPolesDampTemp(KillIdx) = [];

if length(sPolesTemp) > 1
    % 1: Which is closest to past s-domain estimate (specified frequency if not
    % available)?
    if imag(DesiredModes(3)) ~= 0
        % Past s-domain estimate is available
        sPoleErr = abs(sPolesTemp - DesiredModes(3));
        SelectIdx = find(sPoleErr == min(sPoleErr));
    else
        % Last estimate not available, use specified frequency instead
        SelectIdx = near(sPolesFreqTemp,DesiredModes(3));
    end

    ModeEst = sPolesTemp(SelectIdx);
    Mtrack{length(Mtrack)+1} = sPolesTemp;
elseif isempty(sPolesTemp)
    % No possibilities for this mode were identified
    ModeEst = NaN; 
    Mtrack{length(Mtrack)+1} = NaN;
else
    % One possibility for this mode was identified
    ModeEst = sPolesTemp;    
    Mtrack{length(Mtrack)+1} = sPolesTemp;
end