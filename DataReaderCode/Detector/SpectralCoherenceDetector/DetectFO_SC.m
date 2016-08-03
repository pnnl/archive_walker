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
        Freq_FO_Refined(count) = Freq_FOind(IndInterest(find(TestStatistic(Freq_FOind(IndInterest)) == max(TestStatistic(Freq_FOind(IndInterest))))));
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
%     Freq_FO_Refined = Freq_FOind(Freq_FO_Refined);
    %Gives estimates of refined frequencies of FO
    Frequency_est = FreqInterest(Freq_FO_Refined);
    
end
end