function [PMUall,PMURem,PMUsegmentAll,n] = RetrieveBlock(PMU,oneMinuteEmptyPMU,PMUall,PMURem,ResultUpdateInterval,SecondsToConcat)

% The update interval wasn't specified, so forced oscillation detection
% cannot be implemented
if isempty(ResultUpdateInterval)
    PMUall = {};
    PMURem = [];
    PMUsegmentAll = [];
    n = -1;
    return;
end
            
% **********************
% Collect PMU Structures according to specified seconds
% **********************
if isempty(PMURem)
    % This if statement corresponds to the first few seconds
    % (or minutes) until enough files are concatenated for
    % further processing
    PMUall = preparePMUList(PMUall,PMU,oneMinuteEmptyPMU);
else
    if isempty(PMURem.Data)
        % No data was left over from previous file
        PMUall = preparePMUList(PMUall,PMU,oneMinuteEmptyPMU);
    else
        % Data was left over from previous file
        
        % Once the processing starts, PMURem contains remaining data from previous processed data
        % file(data corresponds to time duration(SecondsToConcat-ResultUpdateInterval)
        PMU_rem{1} = PMURem;
        PMUall = preparePMUList(PMU_rem,PMU,oneMinuteEmptyPMU);
    end
end

% Concatenate all the PMU structures on the list into one PMU
% structure for analysis
[PMURem,FlagContinue] = ConcatenatePMU(PMUall,SecondsToConcat);
if FlagContinue ==1
    %Incase concatenated data file do not have enough data, then
    %it skips other processes and continues with next iteration
    PMUsegmentAll = [];
    n = -1;
    return;
end

% n is the number of times an analysis window of length SecondsToConcat
% can slide ResultUpdateInterval seconds within the
% concatenated data.
n = floor((size(PMURem(1).Data,1)-SecondsToConcat*60)/(ResultUpdateInterval*60));
% PMUsegmentAll is the collection of all data that will be
% analyzed as the analysis window slides as many times as it
% can
PMUsegmentAll = ExtractPMUsegment(PMURem,SecondsToConcat+n*ResultUpdateInterval,ResultUpdateInterval);
% PMURem is the data that will be concatenated with the next
% file loaded to continue processing. 
[~, PMURem] = ExtractPMUsegment(PMURem,SecondsToConcat,(n+1)*ResultUpdateInterval);  