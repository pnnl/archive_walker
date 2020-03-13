function [bBP,bLP] = DesignRMSfilters(Band,fs)

% Design filters for RMS calculation
switch Band
    case 'Band 1'
        % BP filter 
        rp = 0.5;           % Passband ripple
        rs = 40;          % Stopband ripple
        f = [0.0001 0.01 0.15 0.16];    % Cutoff frequencies
        a = [0 1 0];        % Desired amplitudes
        dev = [10^(-rs/20) (10^(rp/20)-1)/(10^(rp/20)+1)  10^(-rs/20)]; 
        ftype = 'hilbert';

        % LP filter for RMS calculation - actually a Kaiser window
        SLL = 44;
        MLWf = 0.02;
    case 'Band 2'
        % BP filter 
        rp = 1;           % Passband ripple
        rs = 50;          % Stopband ripple
        f = [0.01 0.15 1 1.2];    % Cutoff frequencies
        a = [0 1 0];        % Desired amplitudes
        dev = [10^(-rs/20) (10^(rp/20)-1)/(10^(rp/20)+1)  10^(-rs/20)]; 
        ftype = 'hilbert';

        % LP filter for RMS calculation - actually a Kaiser window
        SLL = 42;
        MLWf = 0.2;
    case 'Band 3'
        % BP filter 
        rp = 1;           % Passband ripple
        rs = 60;          % Stopband ripple
        f = [0.7 1 5 5.3];    % Cutoff frequencies
        a = [0 1 0];        % Desired amplitudes
        dev = [10^(-rs/20) (10^(rp/20)-1)/(10^(rp/20)+1)  10^(-rs/20)]; 
        ftype = 'hilbert';

        % LP filter for RMS calculation - actually a Kaiser window
        SLL = 40;
        MLWf = 0.7;
    case 'Band 4'
        % BP filter (actually high-pass)
        rp = 3;           % Passband ripple
        rs = 55;          % Stopband ripple
        f = [4 5];    % Cutoff frequencies
        a = [0 1];        % Desired amplitudes
        dev = [10^(-rs/20) (10^(rp/20)-1)/(10^(rp/20)+1)]; 
        ftype = [];

        % LP filter for RMS calculation - actually a Kaiser window
        SLL = 50;
        MLWf = 2.5;
    otherwise
        error('The desired band is not specified properly.');
end

if max(f) > fs/2
    error('Sampling rate of input signal is incompatible with filter design for RMS-energy calculation.');
end

% Design band-pass filter
[n,fo,ao,w] = firpmord(f,a,dev,fs);
bBP = firpm(n,fo,ao,w,ftype);

% Design low-pass filter
MLW = MLWf*4*pi/fs;
n = round(24*pi*(SLL + 12)/155/MLW + 1);
if SLL < 13.26
    a = 0;
elseif SLL < 60
    a = 0.76609*(SLL - 13.26)^0.4 + 0.09834*(SLL - 13.26);
else
    a = 0.12438*(SLL + 6.3);
end
bLP = kaiser(n,a);

bLP = bLP'/sum(bLP);   % Scale to make it a true average