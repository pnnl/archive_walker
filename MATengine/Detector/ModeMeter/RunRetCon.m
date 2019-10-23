function [ModeHistory,ModeDRHistory, ModeFreqHistory, DEFhistory, Modetrack,ModeRem]= RunRetCon(Mode, ModeHistory, ModeFreqHistory, ModeDRHistory, DEFhistory, Modetrack, MaxRetConLength, ResultUpdateInterval)
if length(Modetrack) > floor((MaxRetConLength)/ResultUpdateInterval)
    % Multiple poles have been viable candidates for too long. Run
    % retroactive continuity now, using the selected mode estimate
    % as the most recent.
    Modetrack{end} = Mode;
end
ModeRem = [];
% If the first level is NaN, which can only be true if Mtrack only
% has one level, reset Mtrack
if sum(isnan(Modetrack{1})) > 0
    Modetrack = {};
end

% If there is more than one level
if length(Modetrack) > 1
    % If there was one non-NaN candidate this implementation (the last
    % level)
    if (length(Modetrack{end}) == 1) && (sum(~isnan(Modetrack{end})) > 0)
        % If the previous implementation was either NaN or had
        % multiple candidates, run RetCon
        if (length(Modetrack{end-1}) > 1) || (sum(isnan(Modetrack{end-1})) > 0)
            % Perform retroactive continuity, replacing past mode
            % estimates with those that are smoother
            Mpath = GetRetConPath(Modetrack);
            Mpath = Mpath(:);
            if length(Mpath) <= size(ModeHistory,1)
                % The constructed path is completely contained within
                % the day, so replace the earlier estimates.
                ModeHistory(end-length(Mpath)+1:end) = Mpath;
                FreqMpath = abs(imag(Mpath))/2/pi;
                DRMpath =  -real(Mpath)./abs(Mpath);
                FreqMpath(isnan(DRMpath)) = NaN;    
                ChangeIdx = ModeFreqHistory(end-length(Mpath)+1:end) ~= FreqMpath;
                ModeFreqHistory(end-length(Mpath)+1:end) = FreqMpath;
                ModeDRHistory(end-length(Mpath)+1:end) = DRMpath;
                ModeRem = [];
                % Where the mode estimates changed, set the DEF to NaN
                % (The DEF is not calculated for all modes. A limitation if
                % retcon tracking is used)
                UpdateIdx = size(DEFhistory,1)-length(Mpath)+1:size(DEFhistory,1);
                DEFhistory(UpdateIdx(ChangeIdx),:) = NaN;
            else
                % The constructed path is longer than the matrix of
                % results because the estimates cross a day transition.
                ModeHistory = Mpath(end-size(ModeHistory,1)+1:end);
                Freq = abs(imag(ModeHistory))/2/pi;
                DR =  -real(ModeHistory)./abs(ModeHistory);
                ChangeIdx = ModeFreqHistory(end-size(ModeHistory,1)+1:end) ~= Freq;
                ModeFreqHistory(end-size(ModeHistory,1)+1:end) = Freq;
                ModeDRHistory(end-size(ModeHistory,1)+1:end) = DR;
                ModeRem = Mpath(1:end-size(ModeHistory,1));
                % Where the mode estimates changed, set the DEF to NaN
                % (The DEF is not calculated for all modes. A limitation if
                % retcon tracking is used)
                UpdateIdx = size(DEFhistory,1)-size(ModeHistory,1)+1:size(DEFhistory,1);
                DEFhistory(UpdateIdx(ChangeIdx),:) = NaN;
            end
        end
        % Reset the tracking cell, keeping the most recent
        % single candidate
        Modetrack = Modetrack(end);
    end
elseif length(Modetrack) == 1
    % This is the first level. If there are multiple entries,
    % replace them with the selected mode estimate. This should
    % only occur the very first time the mode meter is called.
    % Note that if Mode(ChanIdx)==NaN, this point of the code can't
    % be reached because of the check before this set of if loops.
    if length(Modetrack{1}) > 1
        Modetrack{1} = Mode;
    end
end

