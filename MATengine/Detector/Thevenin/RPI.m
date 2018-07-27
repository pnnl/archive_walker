function [E_thev, Z_thev] = RPI(Data,fs)
%% Data

Vm = Data(:,1);
Va = Data(:,2);
P = Data(:,3);
Q = Data(:,4);
f = Data(:,5);

VV=(1/sqrt(3))*Vm.*exp(j*(pi/180)*(Va-30));
% Because current measurements are presumed per phase, we
% convert voltage from line-to-line, to line-to-neutral (divide by sqrt(3))
% Not strictly necessary, but helps with clarity.
SS=(1/3)*(P+j*Q);
% Likewise, convert total power, to per phse power.
II=1000*conj(SS./VV);
% And finally, because power in in MW/MVar (10^6), while voltagle
% is in kV (10^3), we need a factor of 1000 to get the correct
% per phase current in amps.

Vbase = 500/sqrt(3);    % kV L-N
Sbase = 100/3;          % single phase

Zbase = Vbase^2/Sbase;
Ibase = (Sbase*1000)/Vbase;

Vm =    abs(VV)/Vbase;
Va =    angle(VV);
P =     real(SS)/Sbase;
Q =     imag(SS)/Sbase;
II =    -II/Ibase;

VV = Vm.*exp(j*Va);

% Just to keep all measurements at approximately the same magnitude for PCA
% calculation.
% f = f/60;


%% Minimum variance of Ethev

% I (Jim) am changing the range of the reactance to prevent it from
% becoming too large
% x_range = [1e-2:1e-3:1.0];  % range of Thevenin reactance
x_range = [1e-3:1e-3:0.02];  % range of Thevenin reactance
% x_range = 1e-2;

% x_range = [1e-2:1e-3:1];
% x_range = [1e-2:1e-3:0.1];

S = zeros(size(x_range));
for k = 1:length(x_range)
    x = x_range(k);
    E = VV + j*x*II;
    Em = abs(E);
    
    % In their papers, they use what they refer to as RMS error, but it's
    % really a sample standard deviation with N-1 replaced by N. So, I'm
    % just using the sample standard deviation.
    S(k) = std(Em);
end

[~,MinIdx] = min(S);
if MinIdx == 1
    X_opt = x_range(1);
elseif MinIdx == length(S)
    X_opt = x_range(end);
else
    % I'm not certain this is quite what they meant by "3-point quadratic
    % fit algorithm" in the paper, but it should be similar.
    %
    % Fit a quadratic to the minimum point and its two adjacent points
    p = polyfit(x_range(MinIdx-1:MinIdx+1),S(MinIdx-1:MinIdx+1),2);
    % Find where the derivative equals zero:
    % 0 = d/dx(aX^2 + bX + c) = 2aX + b -> X = -b/(2*a)
    X_opt = -p(2)/(2*p(1));
end

Z_thev = 1i*X_opt;
E_thev = abs(VV + Z_thev*II);


%% Returned values
E_thev = mean(E_thev);  % static Thevenin voltage

% Convert from pu to engineering units
E_thev = E_thev*Vbase;
Z_thev = Z_thev*Zbase;

% Repeat into vectors the same size as Vm
Z_thev = ones(size(Vm))*Z_thev;
E_thev = ones(size(Vm))*E_thev;