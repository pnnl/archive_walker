%   function Frequency_est = DetectFO_SC(TestStatistic, Threshold, FrequencyTolerance, FreqInterest)
%   This function calculates and refines frequency estimates of forced
%   oscillations
%   Inputs:
%           TestStatistic: Test Statistic calculated using signal to be
%           analyzed
%           Threshold: Threshold for detecting Forced Oscillations
%           FrequencyTolerance = Tolerance used to refine the frequency estimate
%           FreqInterest: frequencies to be considered
%   Outputs:
%           Frequency_est: Estimates of the frequencies of the sinusoids
%           consisting of forced oscillationss
%
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 07/21/2016

function [Frequency_est,Freq_FO_Refined] = DetectFO_SC(TestStatistic, Threshold, FrequencyTolerance, FreqInterest)

%finds indices of frequency for which TestStatistic exceeds Threshold
Freq_FOind = find(TestStatistic > Threshold);
%Gives frequency for which TestStatistic exceeds Threshold
Freq_FO = FreqInterest(Freq_FOind);

% if TestStatistic does not exceed Threshold for any frequency, then
% rerurns NaN value as estimates
if isempty(Freq_FO)
    Frequency_est = NaN;
    Freq_FO_Refined = [];
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
end
end