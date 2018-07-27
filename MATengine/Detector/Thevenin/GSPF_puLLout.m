%   ___    E      _____           V
%  /   \   |      |   |      S    |
% |  G  |--|------| Z |------>----|
%  \___/   |      |___|           |--|
%                                    |
%                                   \ /
%                                    V
% 
% E = voltage at slack bus
% V = voltage to be calculated
% Z = impedance between slack and load buses
% S = Psch + jQsch = scheduled power flowing into load bus
%
% Vhat = initial estimate for V
% eps = threshold for terminating iterations
% ItrMax = maximum number of allowed iterations


function [Vhat,ItrMaxFlag] = GSPF_puLLout(Vhat,Psch,Qsch,Z,E,eps,ItrMax)

% Convert to pu
Vbase = 500/sqrt(3);    % kV L-N
Sbase = 100/3;          % single phase
Zbase = Vbase^2/Sbase;

Vhat = Vhat/Vbase;
Psch = Psch/3/Sbase;
Qsch = Qsch/3/Sbase;
Z = Z/Zbase;
E = E/Vbase;

ItrMaxFlag = false(size(Vhat));

for idx = 1:length(Vhat)
    Y22 = 1/Z(idx);
    Y21 = -1/Z(idx);
    delta = inf;
    ItrCount = 0;
    while delta > eps
        ItrCount = ItrCount + 1;
        if ItrCount > ItrMax
            ItrMaxFlag(idx) = true;
            break
        end

        VhatNew = 1/Y22*((Psch(idx)-1i*Qsch(idx))/Vhat(idx)' - Y21*E(idx));
        delta = abs(VhatNew-Vhat(idx));

        Vhat(idx) = VhatNew;
    end
end

Vhat = Vhat*Vbase;
Vhat = Vhat*sqrt(3)*exp(1i*pi/6);