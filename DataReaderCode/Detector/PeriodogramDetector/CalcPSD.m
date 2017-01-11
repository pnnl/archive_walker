%   function [PSD,f] = CalcPSD(Data, ZeroPaddingLen, WindowOverlap, Window, MedFiltOrd,fs)
%   This function calculates Welch's periodogram and or modified
%   Daniell-Welch's periodogram depepnding on the parameter MedFiltOrd
%
%   Inputs:
%           Data: Data file of dimension Number of data points by number of
%           channel
%           ZeroPaddingLen: Number of DFT points
%           WindowOverlap: Number of overlapped sample
%           Window: a vector containing data point for windowing signal
%           MedFiltOrd: Order of median filter. If empty, median filter is
%           not applied to the PSD
%           fs: Sampling frequency
%
%   Outputs:
%           PSD: Estimated power spectrum
%           f: frequency of estimated power spectrum
%
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/18/2016

function [PSD,f] = CalcPSD(Data, ZeroPaddingLen, WindowOverlap, Window, MedFiltOrd,fs)

[N,NSigs] = size(Data); %dimensions of given data
L = length(Window); %Length of window
M = 1 + floor((N-L)/(L-WindowOverlap)); % Number of averages
U = 1/L*sum(Window.^2); % window power
win = repmat(Window,1,NSigs); %window vector
%initlizing matrix for power spectrum density of all signals
PSD = zeros(ZeroPaddingLen,NSigs); 
for m = 1:M    
    %gives m^th segment of data for calculating m^th PSD
    DataWinInd = ((m-1)*(L-WindowOverlap)+1):(m*(L-WindowOverlap)+WindowOverlap); 
    %gives windowed signal for calculating m^th PSD
    signal_w = Data(DataWinInd,:).*win;
    if isempty(MedFiltOrd)
        % if Median filter order is empty, PSD is obtained without any
        % median filtering operation
        PSD = PSD + 1/(M*L*U)*abs(fft(signal_w,ZeroPaddingLen)).^2;
    else
        %applied median filter to the estimated PSD.
        Q = sum(((MedFiltOrd+1)/2:MedFiltOrd).^-1);
        PSD = PSD + 1/(M*L*U)*medfilt1(abs(fft(signal_w,ZeroPaddingLen)).^2,MedFiltOrd)/Q;
    end
end
%gives frequency vector for whch PSD is estimated
f = fs*(0: ZeroPaddingLen-1)/ ZeroPaddingLen;