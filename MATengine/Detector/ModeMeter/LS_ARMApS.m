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

function [ModeEst, Mtrack, ExtraOutput] = LS_ARMApS(y,w,Parameters,DesiredModes,fs,Mtrack,FOfreq,TimeLoc,~,~)

%% Preliminaries
y = y(:); % Make sure y  is a column vector
na = Parameters.na;
nb = Parameters.nb;
n_alpha = Parameters.n_alpha;
NaNomitLimit = Parameters.NaNomitLimit;

% Under certain setups, FOfreq and TimeLoc can be non-empty even when the
% YW-ARMA algorithm was requested. In this case, set these inputs to [] so
% that FO robustness is not added.
if ~Parameters.FOrobust
    TimeLoc = [];
    FOfreq = [];
end

TimeLoc(isnan(FOfreq),:) = [];
FOfreq(isnan(FOfreq)) = [];
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
        ExtraOutput = struct();
        return
    end
end

%% Remove forced oscillations that are localized to a portion of the 
%  analysis window that is being removed

KillIdx = [];
for p = 1:P
    if sum(w(TimeLoc(p,1):TimeLoc(p,2))) == 0
        KillIdx = [KillIdx p];
    end
end
FOfreq(KillIdx) = [];
TimeLoc(KillIdx,:) = [];

P = length(FOfreq);

%% Remove leading and trailing values that are to be removed by windowing
KeepIdx = find(w,1):find(w,1,'last');
if length(KeepIdx) <= n_alpha + nb
    % Too much of the analysis window is to be removed, so return NaN for the
    % mode estimate
    ModeEst = NaN; 
    Mtrack{length(Mtrack)+1} = NaN;
    ExtraOutput = struct();
    return
else
    % Remove samples at beginning and end that are to be windowed out
    % anyway
    w = w(KeepIdx);
    y = y(KeepIdx);
    % Adjust the forced oscillation start and end times to account for the
    % samples that were removed from the beginning
    TimeLoc = TimeLoc - KeepIdx(1) + 1;
    % Make sure that the TimeLoc values aren't outside of the range between
    % 1 and the new length of the analysis window
    TimeLoc(TimeLoc < 1) = 1;
    TimeLoc(TimeLoc > length(y)) = length(y);
end

W = length(y);  % Estimation segment length

%% Stage 1
L = n_alpha;    % Term that specifies the number of equations to be used

ybar = y(L+1:W);    % Data vector (see eq. (2.49))

Y = -toeplitz(y(L:W-1),y(L:-1:1));   % Data matrix (see eq. (2.50))

% The weights at the input to this function correspond to y. If they were
% used in WLS directly, they would correspond only to ybar. As a result, a 
% bad value in y would be removed from ybar, but not from Y. This is okay
% in many uses of WLS because the values in ybar don't necessarily appear
% in Y, but for an ARMA model the same values appear in both places. 
% The weights calculated below are the minimum of all weights in the 
% equation (one row in the matrix equation). Thus, if an entry in y should
% be ignored, it's influence is removed from the calculation entirely.
wbar = min(toeplitz(w(L+1:W),w(L+1:-1:1)), [], 2);

% Form S matrix (see eq. (3.19))
% Note that k_i has been replaced with i. This will impact the phase
% estimates!
S = zeros(W-L,2*P);
if P > 0
    eps = TimeLoc(:,1);
    eta = TimeLoc(:,2);
end
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

Wp5 = diag(sqrt(wbar));

theta = pinv(Wp5*Z)*Wp5*ybar;   % Estimate of reduced parameter vector
a = [1; theta(1:L)];   % AR coefficients
b = 1;

e = [zeros(L,1); ybar-Z*theta];    % Estimates of process noise (see eq. (3.22))
                                   % Add zeros to make indexing easier
                                   % (does not impact sig2_LS below)


%% Stage 2
if nb > 0
    L = n_alpha + nb;  % Updated term that specifies the number of equations to be used
    
    ybar = y(L+1:W);    % Data vector (see eq. (2.49))

    Y = -toeplitz(y(L:W-1),y(L:-1:L-na+1));   % Data matrix (see eq. (2.62))
    
    E = toeplitz(e(L:W-1),e(L:-1:L-nb+1));   % Process noise matrix (see eq. (2.63))
    
    % Recalculation of the weights (see comment above first definition of
    % wbar for details)
    wbar = min(toeplitz(w(L+1:W),w(L+1:-1:1)), [], 2);
    
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
    
    Z = [Y E S];
    
    Wp5 = diag(sqrt(wbar));

    theta = pinv(Wp5*Z)*Wp5*ybar;   % Estimate of full parameter vector
    a = [1; theta(1:na)];   % AR coefficients
    b = [1; theta(na+1:na+nb)];
end

%% Select modes
[ModeEst, Mtrack] = SelectMode(a,fs,DesiredModes,Mtrack);

%% Store other values necessary outside of this function
ExtraOutput.a = a;
ExtraOutput.b = b;