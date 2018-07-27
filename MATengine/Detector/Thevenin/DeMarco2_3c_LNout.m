function [E, Z] = DeMarco2_3c_LNout(Data,fs)

Vm = Data(:,1);
Va = Data(:,2);
P = Data(:,3);
Q = Data(:,4);
f = Data(:,5);

VV_complex=(1/sqrt(3))*Vm.*exp(j*(pi/180)*(Va-30));
% Because current measurements are presumed per phase, we
% convert voltage from line-to-line, to line-to-neutral (divide by sqrt(3))
% Not strictly necessary, but helps with clarity.
SS_complex=-(1/3)*(P+j*Q);
% Likewise, convert total power, to per phse power.
II_complex=1000*conj(SS_complex./VV_complex);
% And finally, because power in in MW/MVar (10^6), while voltagle
% is in kV (10^3), we need a factor of 1000 to get the correct
% per phase current in amps.
data_length=length(VV_complex);
%
% Compute X Thevenin from first seconds of "flat" behavior,
% number of seconds used for estimation of X Theveni set by
% window_lenth_seconds
%
window_lenth_seconds=2;
w_length=window_lenth_seconds*fs;
end_step=min((data_length-w_length-1),3*w_length);
X_Thevenin=zeros(data_length,1);
E_Thevenin=zeros(data_length,1)+j*zeros(data_length,1);
%
for kk=1:end_step
    first=kk;
    last=kk+w_length-1;
    MM=[imag(II_complex(first:last)) ones(w_length,1) zeros(w_length,1);-real(II_complex(first:last)) zeros(w_length,1) ones(w_length,1)];
    bb=[real(VV_complex(first:last));imag(VV_complex(first:last))];
    
    % Changing the following line because I (Jim) don't understand why the
    % matrix and vector are being added.
%     xx=inv(MM'*MM+diag([10^6 0 0]))*(MM'*bb+(10^6)*[.1;0;0]);
    if ~isnan(sum(sum(MM)) + sum(bb))
        xx=MM\bb;
    else
        xx = NaN(size(MM,2),1);
    end
    
    X_Thevenin(kk+w_length)=xx(1);
    E_Thevenin(kk+w_length)=xx(2)+j*xx(3);
end
%
start_avg=end_step-29;
X_fixed=(1/30)*sum(X_Thevenin(start_avg:end_step));
%

X_Thevenin=zeros(data_length,1);
E_Thevenin=zeros(data_length,1)+j*zeros(data_length,1);
%
window_lenth_seconds=0.1;
w_length=window_lenth_seconds*fs;
end_step=data_length-w_length-1;
%
%
for kk=1:end_step
    first=kk;
    last=kk+w_length-1;
    MM=[ones(w_length,1) zeros(w_length,1);zeros(w_length,1) ones(w_length,1)];
    bb=[real(VV_complex(first:last))-X_fixed*imag(II_complex(first:last)) ;imag(VV_complex(first:last))+X_fixed*real(II_complex(first:last)) ];
    xx=MM\bb;
    X_Thevenin(kk+w_length)=X_fixed;
    E_Thevenin(kk+w_length)=xx(1)+j*xx(2);
end



t_start=w_length+1;
t_end=end_step+w_length;
%
% t_start:t_end represents the set of indices over which
% we have identified E_Thevenin.
%
% So over this interval, we can identify the sample-by-sample
% instantaneous error between the current flow predicted by
% the Thevenin equivalent, versus the actual current flow.
% II_Thev_predicted=zeros(size(II_complex));
% II_Thev_predicted(t_start:t_end)= ...
%     (VV_complex(t_start:t_end)-E_Thevenin(t_start:t_end))./ ...
%     (j*X_Thevenin(t_start:t_end));

% Code below creates a dynamic impedance that can be thought
% of as a "correction" impedance.  This correction impedance,
% in circuit terms, sits in parallel to the simple fixed Thevenin
% impedance calculated in the steps above.  It is computed by performing
% estimation based on the error between the actual current, and the
% currecnt that is computed to flow in the fixed Thevenin impedance
% above.  Note that this error current is exactly what must flow through
% the Dynamic Thevenin impedance that is in parallel with the fixed
% Thevein impedance.  Note also that while the fixed Thevenin impedance is
% restricted to be purely imaginary (i.e., typically a reactance, X),
% the dynamic Thevenin impedance is allowed to have both real and
% imaginary part (i.e., both resistive and reactive part)
%
% For the steps below, the discrete time transfer function coefficients
% are computed by a least squares fit to the first 2 seconds (120 samples)
% of data.  The accuracy of the correction is evaluated on arbitrary two
% second interval later in the overall study interval; refer to this
% as the "evaluation interval."  Accuracy is tested by taking the
% error current calculated for the evaluaton interval, multiplying
% that by the  dynamic Thevenin impedance: this yields the predicted
% voltage drop across the Dynamic Thevenin Impedance.
%
% For the reference directions as defined, subtracting this quantity
% from the measured bus voltage would ideally re-created the Thevenin
% Equivalent voltage.  The difference between the ppreviously identified
% Thevenin voltage, and this re-created Thevenin voltage provides
% an evaluation of the quality of the correction provided by the
% added, parallel Dynamic Thevenin impedance.


order_of_dynamic_Thevenin_impedance=3;

nhist=order_of_dynamic_Thevenin_impedance+1;
%
% Here we'll train the Thevenin based on data appearing time points 100:200
% t_train_start=t_start;
% t_train_end=t_train_start+40;
% VV_train=VV_complex(t_train_start:t_train_end);
% 

% II_train=II_complex(t_train_start:t_train_end)-II_Thev_predicted(t_train_start:t_train_end);                                        

%
%
%  Code section below calculates Thevenin equivalent with dynamic Z_thevenin
%  and fixed zero E_thevenin, from a training case.
%  The coefficients of the discrete time filter are captured
%  in the elements of the vector xx.


% The type of "hist" matrix constructed below has rows
% in which the first entry is the value of the measurement
% at the time index equal to the (row index-1); the second entry
% is the value of the measurement at the time index equal to 
% the (row index ? 2); the third entry is the value of the
% measurement at the time index equal to the (row index - 3), etc.
% 
% Build a history matrix for current
%
% II_hist_train=II_train(nhist:length(VV_train));
% for kk=1:(nhist-1)
% II_hist_train=[II_hist_train ...
%                II_train((nhist-kk):(length(VV_train)-kk))];
% end
% 
% Build a history matrix for voltage
%
% VV_hist_train=VV_train((nhist-1):(length(VV_train)-1));
% for kk=2:(nhist-1)
% VV_hist_train=[VV_hist_train ...
%                VV_train((nhist-kk):(length(VV_train)-kk))];
% end


% first=nhist;
% last=length(VV_train);
% Vr=real(VV_hist_train); % history matrix real part
% Vi=imag(VV_hist_train); % history matrix imaginary part
% V_present_value_r=real(VV_train(first:last)); 
% V_present_value_i=imag(VV_train(first:last));
% Ir=real(II_hist_train);
% Ii=imag(II_hist_train);



% Mr=[Ir -Ii -Vr ];
% Mi=[Ii Ir -Vi ];



% MM=[Mr;Mi];
% bbr=V_present_value_r;
% bbi=V_present_value_i;
% xx=MM\[bbr;bbi]; % Here use MATLAB's built-in pseudo-inverse
                % to obtain the least-squares solution


t_eval_start=t_start;
t_eval_end=t_end;

VV_eval=VV_complex(t_eval_start:t_eval_end);
% 

% II_eval=II_complex(t_eval_start:t_eval_end)-II_Thev_predicted(t_eval_start:t_eval_end);                                        

   
   
 
% II_hist_eval=II_eval(nhist:length(VV_eval));
% for kk=1:(nhist-1)
% II_hist_eval=[II_hist_eval ...
%                II_eval((nhist-kk):(length(VV_eval)-kk))];
% end
% 
% VV_hist_eval=VV_eval((nhist-1):(length(VV_eval)-1));
% for kk=2:(nhist-1)
% VV_hist_eval=[VV_hist_eval ...
%                VV_eval((nhist-kk):(length(VV_eval)-kk))];
% end

% Vr=real(VV_hist_eval);
% Vi=imag(VV_hist_eval);
% Ir=real(II_hist_eval);
% Ii=imag(II_hist_eval);

% Mr=[Ir -Ii -Vr ];
% Mi=[Ii Ir -Vi ];

% Vest_eval_from_train=(Mr*xx+j*Mi*xx) ;

II_complex2 = II_complex(t_eval_start:t_eval_end);
II_complex2 = II_complex2(nhist:length(VV_eval));

VV_complex2 = VV_complex(t_eval_start:t_eval_end);
VV_complex2 = VV_complex2(nhist:length(VV_eval));

E_Thevenin2 = E_Thevenin(t_eval_start:t_eval_end);
E_Thevenin2 = E_Thevenin2(nhist:length(VV_eval));

FilledIdx = 1:length(II_complex);
FilledIdx = FilledIdx(t_eval_start:t_eval_end);
FilledIdx = FilledIdx(nhist:length(VV_eval));

% % Calculate the thevenin impedance in ohms
% idx = find(abs(E_Thevenin2)>0);
% % Z = (E_Thevenin2(idx) - Vest_eval_from_train(idx))./II_complex2(idx)*1000;
% Z = (E_Thevenin2(idx) - VV_complex2(idx))./II_complex2(idx)*1000;
% 
% E = E_Thevenin2(idx);

% Calculate the thevenin impedance in ohms
Z = (E_Thevenin2 - VV_complex2)./II_complex2*1000;
E = E_Thevenin2;

% Z and E are shorter than the input signals. Hold the first and last
% values to extend to the proper length. 
% For example, [3 8 2 6] becomes [3 ... 3 8 2 6 ... 6]
Z = [Z(1)*ones(FilledIdx(1)-1,1); Z; Z(end)*ones(length(II_complex)-FilledIdx(end),1)];
E = [E(1)*ones(FilledIdx(1)-1,1); E; E(end)*ones(length(II_complex)-FilledIdx(end),1)];