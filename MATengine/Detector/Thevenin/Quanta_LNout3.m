function [E_thev, Z_thev, PastVals] = Quanta_LNout3(Data,fs,PastAdditionalOutput)

PastAvailable = false;
if isfield(PastAdditionalOutput,'PastVals')
    if ~isempty(PastAdditionalOutput.PastVals)
        % Past values are available
        PastAvailable = true;
        
        % For order, see PastVals at the bottom of this function
        V1past = PastAdditionalOutput.PastVals(1);
        I1past = PastAdditionalOutput.PastVals(2);
        P1past = PastAdditionalOutput.PastVals(3);
        Q1past = PastAdditionalOutput.PastVals(4);
        Zl_1past = PastAdditionalOutput.PastVals(5);
        Eth1_1past = PastAdditionalOutput.PastVals(6);
        Eth1past = PastAdditionalOutput.PastVals(7);
    end
end

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

V1 =    abs(VV)/Vbase;
theta1 = zeros(size(V1));
% theta1 = angle(VV);
P1 =     real(SS)/Sbase;
Q1 =     imag(SS)/Sbase;
I1 =    abs(-II/Ibase);

% theta1 = angle(VV);
% theta_cur1 = angle(II);

InitializedFlag = PastAvailable;

% Set defaults
Eth1_1 = NaN(1,length(V1));
angle_th1 = NaN(1,length(V1));
Z_th1 = NaN(1,length(V1));
angle_Z = NaN(1,length(V1));

% Loop over all PMU data
for k=1:length(V1)
    if k > 1
        if abs(V1(k))<=0.3
            V1(k)=V1(k-1);
        end
        if abs(I1(k))<=0.3
            I1(k)=I1(k-1);
        end
        if abs(P1(k))<=0.05
            P1(k)=P1(k-1);
        end
    elseif PastAvailable
        if abs(V1(k))<=0.3
            V1(k)=V1past;
        end
        if abs(I1(k))<=0.3
            I1(k)=I1past;
        end
        if abs(P1(k))<=0.05
            P1(k)=P1past;
        end
    end

    thetaS1(k)=atan2(Q1(k),P1(k));
    theta_cur1(k)=theta1(k)-thetaS1(k);

    % Equivalent "load" impedance
    Zl_1(k)=V1(k)/I1(k);

    % Initialization of Thevenin equivalent parameters
    if ~InitializedFlag
        if ~isnan(sum([V1(k) I1(k) P1(k) Q1(k) Zl_1(k)]))
            theta_aux1=atan2(Q1(k),P1(k));
            beta_aux1=atan2((Zl_1(k)*(I1(k))+V1(k)*sin(theta_aux1)),(V1(k)*cos(theta_aux1)));
            Eth_max1=V1(k)*(cos(theta1(k))+j*sin(theta1(k)))*cos(theta_aux1)/cos(beta_aux1);
            Eth1(k)=0.95*(Eth_max1+V1(k)*(cos(theta1(k))+j*sin(theta1(k))))/2;
            Eth1_1(k)=sqrt(real(Eth1(k))^2+imag(Eth1(k))^2);

            V1past = V1(k);
            I1past = I1(k);
            P1past = P1(k);
            Q1past = Q1(k);
            Zl_1past = Zl_1(k);
            Eth1_1past = Eth1_1(k);
            Eth1past = Eth1(k);
            
            InitializedFlag = true;
        end

        continue
    end

    % Parameters cumputation (Thevenin voltage)
%     if k>=100
%         k1=0.001;
%     else
%         k1=0.00001;
%     end
    if PastAvailable % Not equivalent, but should accomplish the same thing (Jim)
        k1=0.001;
    else
        k1=0.00001;
    end
    if abs(V1(k)/(I1(k))-V1past/(I1(k))) < 0.01*V1past/(I1(k))
        if (Zl_1(k)-Zl_1past)>0.00005
        zz2_1=((Eth1_1past-V1(k))/(I1(k)))-((Eth1_1past-V1past)/(I1past));
        if(zz2_1>0)
            Eth_aux2_11=k1*Eth1_1past;
            Eth1(k)=Eth1past*(1-Eth_aux2_11);
        else
            Eth_aux2_11=k1*Eth1_1past;
            Eth1(k)=Eth1past*(1+Eth_aux2_11);
        end
        else
            Eth1(k)=Eth1past;
        end       
        if (Zl_1(k)-Zl_1past)<-0.00005
        zz2_1=((Eth1_1past-V1(k))/(I1(k)))-((Eth1_1past-V1past)/(I1past));
        if(zz2_1<0)
            Eth_aux2_11=k1*Eth1_1past;
            Eth1(k)=Eth1past*(1-Eth_aux2_11);
        else
            Eth_aux2_11=k1*Eth1_1past;
            Eth1(k)=Eth1past*(1+Eth_aux2_11);
        end
        else
            Eth1(k)=Eth1past;
        end       
    else
        Eth_aux1_1=3*(V1(k)-V1past);
        Eth1(k)=Eth1past*(1-Eth_aux1_1);
    end

% Parameters cumputation (Thevenin impedance)

    Ethr1(k)=real(Eth1(k));
    Ethi1(k)=imag(Eth1(k));
    b11_12=V1(k)*cos(theta1(k))-Ethr1(k);
    b12_12=V1(k)*sin(theta1(k))-Ethi1(k);
    b1_12=(I1(k))*cos(theta_cur1(k));
    b2_12=(I1(k))*sin(theta_cur1(k));
    a1_12=[-b1_12 b2_12];
    a2_12=[-b2_12 -b1_12];
    b_1=[b11_12;b12_12];
    A_1=[a1_12;a2_12];
    if ~isnan(sum(sum(A_1)) + sum(b_1))
        res1=A_1\b_1;
    else
        res1 = NaN(size(A_1,2),1);
    end

    Z_th1(k)=sqrt(res1(2)^2+res1(1)^2);
    angle_Z(k) = atan2(res1(1),res1(2));
    Eth1_1(k)=sqrt(Ethr1(k)^2+Ethi1(k)^2);
    angle_th1(k)=atan2(Ethi1(k),Ethr1(k));


    % Only update if none are NaN
    if ~isnan(sum([V1(k) I1(k) P1(k) Q1(k) Zl_1(k) Eth1_1(k) Eth1(k)]))
        V1past = V1(k);
        I1past = I1(k);
        P1past = P1(k);
        Q1past = Q1(k);
        Zl_1past = Zl_1(k);
        Eth1_1past = Eth1_1(k);
        Eth1past = Eth1(k);
    end
end

% Combine magnitude and angle values
E_thev = Eth1_1.*exp(1i*angle_th1);
Z_thev = Z_th1.*exp(1i*angle_Z);

% Convert from pu to engineering units
E_thev = E_thev.'*Vbase;
Z_thev = Z_thev.'*Zbase;

if InitializedFlag
    PastVals = [V1past I1past P1past Q1past Zl_1past Eth1_1past Eth1past];
else
    PastVals = [];
end