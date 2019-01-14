function Res = InspectionSpectral(Data,Params)

% Pull out parameters
fs = Params.fs;
N = round(Params.AnalysisLength*fs);
Nw = round(Params.WindowLength*fs);
Nover = round(Params.WindowOverlap*fs);
Nzp = round(Params.ZeroPadding);
LS = Params.LogScale;

% Create window
switch Params.Window
    case 'Rectangular'
        win = rectwin(Nw);
    case 'Bartlett'
        win = bartlett(Nw);
    case 'Hann'
        win = hann(Nw);
    case 'Hamming'
        win = hamming(Nw);
    case 'Blackman'
        win = blackman(Nw);
    otherwise
        error([Params.Window ' is not an acceptable window type']);
end

% FreqMin and FreqMax are optional parameters. If they're not specified,
% return frequencies from zero to Nyquist.
if isfield(Params,'FreqMin')
    FreqMin = Params.FreqMin;
else
    FreqMin = 0;
end
%
if isfield(Params,'FreqMax')
    FreqMax = Params.FreqMax;
else
    FreqMax = fs/2;
end

% Truncate to the specified analysis length. This makes it easier for the
% user to select a window that will minimize leakage.
Data = Data(1:N,:);

% Calculate the Welch Periodogram, then trim to the specified frequency
% range.
[P,f] = pwelch(Data,win,Nover,Nzp,fs);
fidx = near(f,FreqMin):near(f,FreqMax);
P = P(fidx,:)/2*fs;
f = f(fidx);

% Convert to dB if specified
if strcmp(LS,'TRUE')
    P = 10*log10(P);
    ylab = 'PSD (dB)';
else
    ylab = 'PSD';
end

% Package output in the generic format
Res.y = P;
Res.x = f;
Res.SigNames = Params.SigNames;
Res.xlab = 'F (Hz)';
Res.ylab = ylab;