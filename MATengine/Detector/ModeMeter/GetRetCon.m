if length(MMoutput.Mtrack{ChanIdx}{end}) > floor(str2double(Config.MaxRetConLength)/ResultUpdateInterval)
    % Multiple poles have been viable candidates for too long. Run
    % retroactive continuity now, using the selected mode estimate
    % as the most recent.
    MMoutput.Mtrack{ChanIdx}{end} = Mode(ChanIdx);
end

% If the first level is NaN, which can only be true if Mtrack only
% has one level, reset Mtrack
if sum(isnan(MMoutput.Mtrack{ChanIdx}{1})) > 0
    MMoutput.Mtrack{ChanIdx} = {};
end

% If there is more than one level
if length(MMoutput.Mtrack{ChanIdx}) > 1
    % If there was one non-NaN candidate this implementation (the last
    % level)
    if (length(MMoutput.Mtrack{ChanIdx}{end}) == 1) && (sum(~isnan(MMoutput.Mtrack{ChanIdx}{end})) > 0)
        % If the previous implementation was either NaN or had
        % multiple candidates, run RetCon
        if (length(MMoutput.Mtrack{ChanIdx}{end-1}) > 1) || (sum(isnan(MMoutput.Mtrack{ChanIdx}{end-1})) > 0)
            % Perform retroactive continuity, replacing past mode
            % estimates with those that are smoother
            Mpath = GetRetConPath(MMoutput.Mtrack{ChanIdx});
            
            if length(Mpath) <= size(MMoutput.Mode,1)
                % The constructed path is completely contained within
                % the day, so replace the earlier estimates.
                MMoutput.Mode(end-length(Mpath)+1:end,ChanIdx) = Mpath;
                MMoutput.MpathRemainder{ChanIdx} = [];
            else
                % The constructed path is longer than the matrix of
                % results because the estimates cross a day transition.
                MMoutput.Mode(:,ChanIdx) = Mpath(end-size(MMoutput.Mode,1)+1:end);
                MMoutput.MpathRemainder{ChanIdx} = Mpath(1:end-size(MMoutput.Mode,1));
            end
        end
        % Reset the tracking cell, keeping the most recent
        % single candidate
        MMoutput.Mtrack{ChanIdx} = MMoutput.Mtrack{ChanIdx}(end);
    end
elseif length(MMoutput.Mtrack{ChanIdx}) == 1
    % This is the first level. If there are multiple entries,
    % replace them with the selected mode estimate. This should
    % only occur the very first time the mode meter is called.
    % Note that if Mode(ChanIdx)==NaN, this point of the code can't
    % be reached because of the check before this set of if loops.
    if length(MMoutput.Mtrack{ChanIdx}{1}) > 1
        MMoutput.Mtrack{ChanIdx}{1} = Mode(ChanIdx);
    end
end
