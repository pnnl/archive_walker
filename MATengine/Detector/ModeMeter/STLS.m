% [theta,sig2_LS,yhat] = STLS_ARMApS(y,fs,f,eps,eta,n_alpha,n_a,n_b)
%
% This function implements the STLS-ARMA+S algorithm. This is an adapted two-stage STLS algorithm that incorporates the
% presence of a sinusoid in the signal y. The standard two-stage STLS
% algorithm is implemented if f=[]. If n_a or n_b are omitted, only
% the first stage is implemented.
%
% INPUTS:
% y = signal to be analyzed
% fs = sampling rate of y
% f = vector of sinusoidal frequencies. Standard two-stage STLS algorithm is
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

function [ModeEst, Mtrack, ExtraOutput] = STLS(y,~,Parameters,DesiredModes,fs,Mtrack,FOfreq,TimeLoc)

%% Preliminaries
y = y(:); % Make sure y  is a column vector
na = Parameters.na;
nb = Parameters.nb;
n_alpha = Parameters.n_alpha;
thresh = Parameters.thresh; %threshold for the STLS (smallest singular value is compared to this value)
if isfield(Parameters,'NumIteration') == 0
    NumIteration = 1000;
else
    NumIteration = Parameters.NumIteration; %number of iterations for the STLS (in case the th was set incorrectly, this variable will make sure we get solution); if set to 0, function will be regular 2 stage LS
end

% Under certain setups, FOfreq and TimeLoc can be non-empty even when the
% STLS algorithm was requested. In this case, set these inputs to [] so
% that FO robustness is not added.
if ~Parameters.FOrobust
    TimeLoc = [];
    FOfreq = [];
end

TimeLoc(isnan(FOfreq),:) = [];
FOfreq(isnan(FOfreq)) = [];
P = length(FOfreq);

%% Handle the case where y contains NaN

% If there are NaNs in y the mode cannot be estimated
if sum(isnan(y)) > 0
    % There are NaNs in the input signal - return NaN for the mode
    % estimate
    ModeEst = NaN; 
    Mtrack{length(Mtrack)+1} = NaN;
    ExtraOutput = struct();
    return
end

%% Stage 1

W = length(y);  % Estimation segment length

L = n_alpha;    % Term that specifies the number of equations to be used

ybar = y(L+1:W);    % Data vector (see eq. (2.49))

Y = -toeplitz(y(L:W-1),y(L:-1:1));   % Data matrix (see eq. (2.50))

%ww = ones(W,1);

% The weights at the input to this function correspond to y. If they were
% used in WLS directly, they would correspond only to ybar. As a result, a 
% bad value in y would be removed from ybar, but not from Y. This is okay
% in many uses of WLS because the values in ybar don't necessarily appear
% in Y, but for an ARMA model the same values appear in both places. 
% The weights calculated below are the minimum of all weights in the 
% equation (one row in the matrix equation). Thus, if an entry in y should
% be ignored, it's influence is removed from the calculation entirely.
%wbar = min(toeplitz(ww(L+1:W),ww(L+1:-1:1)), [], 2);

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

Y1 = stls1(Y,ybar,thresh,NumIteration);
B = -Y1(:,1);
A = [Y1(:,2:L+1) S];
theta=A\B;  % Estimate of reduced parameter vector

a = [1; theta(1:L)];   % AR coefficients
b = 1;

e = [zeros(L,1); ybar-A*theta];    % Estimates of process noise (see eq. (3.22))
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
        % The psi term is used to incorporate the starting and ending samples
        % of each sinusoid. Each psi is the corresponding column of Psi in
        % eq. (3.20)
        psi = zeros(1,W);
        psi(eps(p):eta(p)) = 1;
        psi = psi(L+1:W);
        
        S(:,2*p-1) = cos(2*pi*FOfreq(p)/fs*(L+1:W)).*psi;
        S(:,2*p) = -sin(2*pi*FOfreq(p)/fs*(L+1:W)).*psi;
    end
    
    Y1 = stls1(Y,ybar,thresh,NumIteration);

    B = -Y1(:,1);
    A = [Y1(:,2:na+1) E S];
    theta=A\B;  % Estimate of full parameter vector
    a = [1; theta(1:na)];   % AR coefficients
    b = [1; theta(na+1:na+nb)];
end

%% Select modes
[ModeEst, Mtrack] = SelectMode(a,fs,DesiredModes,Mtrack);

%% Store other values necessary outside of this function
ExtraOutput.a = [];
ExtraOutput.b = [];

end

function [S] = stls1(a,b,eps,noi)

    [m, n]   = size(a);            % a is m by n
    S = [-b a]; 
    
    %initialization

    [U, Sig, V] = svd(S,0);

    %sv is the smalled singular value
    sv=Sig(n+1,n+1);
    l=0;
    while sv>eps && l<noi
        l=l+1;
        %Step 2.1 %reducing rank for 1
        Sig(n+1,n+1)=0;
        S_a=U*Sig*V';
        %Step 2.2 
        %calculate new S closest (in the by W specified L2 norm) matrix to Sa having the requested structur  
        C=zeros(1,m);
        R=zeros(1,n+1);
        v=diag(S_a);
        if sum(v)/size(v,1)>0
            C(1)=norm(v)/sqrt(size(v,1));
        else
            C(1)=-norm(v)/sqrt(size(v,1));
        end
        R(1)=C(1);
        for i=1:(m-1)
            %extract diagonal
            v=diag(S_a,-i);
            if sum(v)/size(v,1)>0
                C(i+1)=norm(v)/sqrt(size(v,1));
            else
                C(i+1)=-norm(v)/sqrt(size(v,1));
            end
        end
        for i=1:(n)
            %extract diagonal
            v=diag(S_a,i);
            if sum(v)/size(v,1)>0
                R(i+1)=norm(v)/sqrt(size(v,1));
            else
                R(i+1)=-norm(v)/sqrt(size(v,1));
            end      
        end

        S=toeplitz(C,R);
        [U, Sig, V] = svd(S,0);
        sv=Sig(n+1,n+1);
    end
end






