function [E_thev, Z_thev] = Mitsubishi3_LNout(Data,fs)
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

% errH = [];
% errH2 = [];
error = 1e10; % large value for initial error
X_opt = NaN;    % Default Value
E_opt_all = NaN(size(VV));   % Default Value
for k = 1:length(x_range)
    x = x_range(k);
    E = VV + j*x*II;
    Em = abs(E);

    new_error = norm(cov([Em,Vm,Va,P,Q,f]));
%     errH = [errH new_error];
%
%     PP = corrcoef([Em,Vm,Va,P,Q,f]);
%     errH2 = [errH2 sqrt(mean(PP(1,2:end).^2))];
%     new_error = sqrt(mean(PP(1,2:end).^2));
%     errH2 = [errH2 sum(abs(PP(1,2:end)))];
%     new_error = sum(abs(PP(1,2:end)));
%     errH2 = [errH2 max(abs(PP(1,2:end)))];
%     new_error = max(abs(PP(1,2:end)));
    if new_error < error
        error = new_error;
        X_opt = x;
        E_opt_all = Em;
    end

end

% figure, subplot(1,2,1); plot(errH); subplot(1,2,2); plot(errH2); drawnow();
% figure, hold on; 
% plot(x_range,(errH-min(errH))/range(errH),'b'); 
% plot(x_range,errH2,'r'); 
% plot(x_range(errH==min(errH))*ones(1,2),[0 1],'b');
% plot(x_range(errH2==min(errH2))*ones(1,2),[0 1],'r--');
% hold off; drawnow();

% X_opt = x_range(1);

Z_thev = 1i*X_opt;
E_thev = E_opt_all;


%% Returned values
E_thev = mean(E_thev);  % static Thevenin voltage

% Convert from pu to engineering units
E_thev = E_thev*Vbase;
Z_thev = Z_thev*Zbase;

% Repeat into vectors the same size as Vm
Z_thev = ones(size(Vm))*Z_thev;
E_thev = ones(size(Vm))*E_thev;