% Ulast = voltage at last time Thevenin was calculated
% Ilast = current at last time Thevenin was calculated
% Zk = load impedance values since Thevenin was last calculated. Averaged
%      to get the value used in the stability index calculation

function [E_thev, Z_thev, PastVals, Zload] = Tellegen_LNout2(Data,fs,Zload,PastAdditionalOutput)
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

%%

VVpast = VV(1);
IIpast = II(1);
ZloadPast = NaN;
ZloadEnd = Zload(end);
if isfield(PastAdditionalOutput,'PastVals')
    if ~isempty(PastAdditionalOutput.PastVals)
        % For order, see PastVals at the bottom of this function
        VVpast = PastAdditionalOutput.PastVals(1);
        IIpast = PastAdditionalOutput.PastVals(2);
        ZloadPast = PastAdditionalOutput.PastVals(3);
    end
end
Zload = [ZloadPast; Zload(1:end-1)];


%%

% Find the Thevenin equivalent for all samples, then discard for any that
% do not meet the change requirement for II

% Minimum change in abs(II) required to calculate the Thevenin equivalent
deltaImin = 0.015;

% Sample-to-Sample changes in voltage and current
deltaVV = diff([VVpast; VV]);
deltaII = diff([IIpast; II]);

% Remove all changes in current less than the specified minimum. This will
% result in Z_thev = E_thev = NaN
deltaII(abs(deltaII) < deltaImin) = NaN;

% Equation (22)
Z_thev = conj(deltaVV)./deltaII;

% Equation (11) rearranged 
E_thev = II.*Z_thev + VV;


%% Returned values

% Convert from pu to engineering units
E_thev = E_thev*Vbase;
Z_thev = Z_thev*Zbase;

PastVals = [VV(end) II(end) ZloadEnd];