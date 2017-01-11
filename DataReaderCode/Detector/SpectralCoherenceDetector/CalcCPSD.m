function CPSD= CalcCPSD(Sig1FFT, Sig2FFT)

P_xyM = Sig1FFT.*conj(Sig2FFT);
P_xy = mean(P_xyM,2);

P_xxM = Sig1FFT.*conj(Sig1FFT);
P_xx = mean(P_xxM,2);

P_yyM = Sig2FFT.*conj(Sig2FFT);
P_yy = mean(P_yyM,2);

CPSD = abs(P_xy).^2./P_xx ./P_yy;

