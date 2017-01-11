PMUnum = 17;
CHANnum = 1;

fs = 60;
N = 600*fs;
f = fs*(0:N-1)/N;

x = PMU_ProcessorOutput(PMUnum).Data(:,CHANnum);
x = x(end-N+1:end);

Px = 1/N*abs(fft(x)).^2;
% figure, plot(f,Px);

% Pw = pwelch(x,60*fs,30*fs,f,fs);
% Pw = Pw*fs;
% % plot(f,Pw);

ZeroPadding = N;
WindowOverlap = 6000;
GMSCandPSDwindow = hamming(12000);
MedianFilterOrder = [];
Pw = CalcPSD(x, ZeroPadding, WindowOverlap, GMSCandPSDwindow,MedianFilterOrder,fs);

MedianFilterOrder = 37;
PmDW = CalcPSD(x, ZeroPadding, WindowOverlap, GMSCandPSDwindow,MedianFilterOrder,fs);

N1 = 60*fs;
f1 = fs*(0:N1-1)/N1;
x1 = x(end-N1+1:end);
Px1 = 1/N1*abs(fft(x1)).^2;

n_alpha = 26;
na = 16;
nb = 10;
fsNew = 5;
%
Px1 = Px1(f1 < fsNew/2);
f1 = f1(f1 < fsNew/2);
%
tx = (0:N-1)*1/fs;
xPP = PreProcessDiss(x,tx,fs,fsNew);
% fLS = fsNew*(0:length(xPP)-1)/length(xPP);
fLS = f1;
[theta,sig2] = LS_ARMApS(xPP,fsNew,0.4983,1,length(xPP),n_alpha,na,nb);
PLS = LS_ARMApS_Analyze(theta,na,nb,sig2,2*pi*fLS/fsNew,[],fsNew,0.4983);

figure, plot(f,Px,'b',f,Pw,'r',f,PmDW,'g',fLS,PLS*fs/fsNew,'c');
xlim([0.15 1]);
propedit;

figure, plot(f1,Px1,'b',fLS,PLS*fs/fsNew,'r');
xlim([0.15 1]);
propedit;

T1 = 2*Px1./(PLS*fs/fsNew);
figure, plot(f1,T1);
xlim([0.15 1]);
propedit;