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
    % carries out frequency estimates refining if at least one frequency exists for which TestStatistic exceeds
    % Threshold
    count = 1;
    initialInd = 1; 
    while(1)
        % calculates difference of frequencies with the initialInd of
        % frequency vector
        Freq_FODiff = Freq_FO(initialInd:end) - Freq_FO(initialInd);
        %finds indices of Freq_FO which is to be refined
        Ind = find(Freq_FODiff < FrequencyTolerance);
        %finds the index of Freq_FO falling in one group for which
        %TestStatistic has maximum value
        IndInterest = initialInd + Ind-1;
        Freq_FO_Refined(count) = IndInterest(find(TestStatistic(Freq_FOind(IndInterest)) == max(TestStatistic(Freq_FOind(IndInterest)))));
        % If last index of ind matches with the length of Freq_FODiff, it
        % means all frequency estimates are refined
        if Ind(end)  == length(Freq_FODiff)
            break
        else
            % if more frequencies are left to be refined, then changes
            % initialInd value to the first index of remaining group of frequencies to be
            % refined
            initialInd = initialInd + Ind(end);
        end
        %gives count of frequency estimates which are refined
        count = count+1;
    end
    
    % gives length of window used to calculate periodogram of signal    
    N = length(Window);
    % gives power of window used to calculate periodogram of signal   
    U = 1/N*abs(sum(Window.^2));
    % gives coherent gain of window used to calculate periodogram of signal   
    CG = 1/N*abs(sum(Window));
    
    Freq_IndRefined = Freq_FOind(Freq_FO_Refined);
    % gives an estimates of amplitude of FOs
    Amplitude_est = sqrt((SignalPSD(Freq_IndRefined,:) - AmbientNoisePSD(Freq_IndRefined,:))*4*U/N/CG^2);    
    
    %Finds the indices for which signal spectrum is smaller than noise
    %spectrum and assigns NaN value to the corresponding amplitude estimate
    SignalPSDSmallInd = find(SignalPSD(Freq_IndRefined,:) < AmbientNoisePSD(Freq_IndRefined,:));    
    Amplitude_est(SignalPSDSmallInd) = NaN;  
    
    %Gives estimates of refined frequencies of FO
    Frequency_est = FreqInterest(Freq_IndRefined);
end
end