%   function [Frequency_est, Amplitude_est] = DetectFO(TestStatistic, Threshold, SignalPSD, AmbientNoisePSD, FrequencyTolerance, Window, FreqInterest)
%   This function calculates frequency and amplitude estimates of forced
%   oscillations
%   Inputs:
%           TestStatistic: Test Statistic calculated using signal to be
%           analyzed
%           Threshold: Threshold for detecting Forced Oscillations
%           SignalPSD: Power spectrum estimate of the signals to be
%           analyzed
%           AmbientNoisePSD: Estimated power spectrum estimate of the
%           ambient noise using signals to be analyzed
%           FrequencyTolerance = Tolerance used to refine the frequency estimate
%           Window: a vector containing data point for windowing signal
%           FreqInterest: frequencies to be considered
%   Outputs:
%           Frequency_est: Estimates of the frequencies of the sinusoids
%           consisting of forced oscillations
%           Amplitude_est: Estimates of the amplitude of the sinusoids
%           consisting of forced oscillations
%
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/18/2016

function [Frequency_est, Amplitude_est] = DetectFO(TestStatistic, Threshold, SignalPSD, AmbientNoisePSD, FrequencyTolerance, Window, FreqInterest)

%finds indices of frequency for which TestStatistic exceeds Threshold
Freq_FOind = find(TestStatistic > Threshold);
%Gives frequency for which TestStatistic exceeds Threshold
Freq_FO = FreqInterest(Freq_FOind);

% if TestStatistic does not exceed Threshold for any frequency, then
% rerurns NaN value as estimates
if isempty(Freq_FO)
    Amplitude_est = NaN*ones(1,size(SignalPSD,2));
    Frequency_est = NaN;
    return;
else    
    % Breaks frequency bins with detections into groups separated by at least tol Hz
    Loc = [0, find(diff(Freq_FO) > FrequencyTolerance), length(Freq_FO)];

    Frequency_est = zeros(1,length(Loc)-1); % Refined frequency vector
    Freq_FO_Refined = zeros(1,length(Loc)-1); % indices of the refined frequencies

    for L = 1:(length(Loc)-1) % For each group
        Lidx = (Loc(L)+1):Loc(L+1);
        MaxIdx = find(TestStatistic(Freq_FOind(Lidx)) == max(TestStatistic(Freq_FOind(Lidx))));
        Frequency_est(L) = Freq_FO(Lidx(MaxIdx));
        Freq_FO_Refined(L) = Freq_FOind(Lidx(MaxIdx));  % Indices of f that correspond to refined frequency estimates
    end
    
    % gives length of window used to calculate periodogram of signal    
    N = length(Window);
    % gives power of window used to calculate periodogram of signal   
    U = 1/N*abs(sum(Window.^2));
    % gives coherent gain of window used to calculate periodogram of signal   
    CG = 1/N*abs(sum(Window));
    
    % gives an estimates of amplitude of FOs
    Amplitude_est = sqrt((SignalPSD(Freq_FO_Refined,:) - AmbientNoisePSD(Freq_FO_Refined,:))*4*U/N/CG^2); 
    Amplitude_est(imag(Amplitude_est) ~= 0) = NaN;
end
end