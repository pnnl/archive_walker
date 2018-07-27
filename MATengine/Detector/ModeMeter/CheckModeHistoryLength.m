function [AdditionalOutput, DetectionResults] = CheckModeHistoryLength(AdditionalOutput, DetectionResults, ResultPath)
DaysBack = 1;
MpathRemainderHold = AdditionalOutput.MpathRemainder;
ModeHistoryHold = AdditionalOutput.ModeHistory;
% The updated path crosses a day boundary. Load MMoutput from previous days, replace
% the mode estimates, and then save them again until MpathRemainder is empty.

while ~isempty(AdditionalOutput.MpathRemainder)
    if exist([ResultPath '\MMresults_' datestr(FocusFileTime-DaysBack,'yymmdd') '.mat'],'file') > 0
        % Load the previous days MMresults file, which contains MMoutput
        load([ResultPath '\MMresults_' datestr(FocusFileTime-DaysBack,'yymmdd') '.mat']);
        
        if length(MpathRemainderHold) <= size(AdditionalOutput.ModeHistory,1)
            % The constructed path is completely contained within
            % the day, so replace the earlier estimates.
            AdditionalOutput.ModeHistory(end-length(Mpath)+1:end) = MpathRemainderHold;
            MpathRemainderHold = [];
        else
            % The constructed path is longer than the matrix of
            % results because the estimates cross a day transition.
            ModeHistoryHold = MpathRemainderHold(end-size(AdditionalOutput.ModeHistory,1)+1:end);
            MpathRemainderHold = MpathRemainderHold(1:end-size(AdditionalOutput.ModeHistory,1));
        end
        
        % Save the updated MMoutput structure back to where it was
        save([ResultPath '\MMresults_' datestr(FocusFileTime-DaysBack,'yymmdd') '.mat'],'DetectionResults','AdditionalOutput');
        
        % Increment the number of days back
        % that the code needs to go to update
        % the mode estimates
        DaysBack = DaysBack + 1;
    else
        warning([ResultPath '\MMresults_' datestr(FocusFileTime-DaysBack,'yymmdd') '.mat does not exist, cannot perform retroactive continuity on earlier day mode estiamtes.'])
        AdditionalOutput.MpathRemainder = [];
    end
end
AdditionalOutput.MpathRemainder = MpathRemainderHold;
AdditionalOutput.ModeHistory = ModeHistoryHold;