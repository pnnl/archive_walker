function ExtractedParameters = ExtractAlarmingParamsSC(Parameters)

% An alarm will be set for any detection with a coherence greater than CoherenceAlarm
if isfield(Parameters,'CoherenceAlarm')
    % Use specified limit
    CoherenceAlarm = str2num(Parameters.CoherenceAlarm);
else
    % Use default (disable)
    CoherenceAlarm = 1;
end

% Alarms will not be set for any detection with a coherence less
% than CoherenceMin, no matter how long they persist.
if isfield(Parameters,'CoherenceMin')
    % Use specified value
    CoherenceMin = str2num(Parameters.CoherenceMin);
else
    % Use default (disable)
    CoherenceMin = 0;
end

% Detected forced oscillations must persist for TimeMin seconds before an 
% alarm will be set (unless the coherence is greater than CoherenceAlarm)
if isfield(Parameters,'TimeMin')
    % Use specified value
    TimeMin = str2num(Parameters.TimeMin);
else
    % Use default (disable)
    TimeMin = 0;
end

% Specifies the coherence component of an ordered pair that is located on 
% the alarming curve. The ordered pair specifies the shape of the curve.
if isfield(Parameters,'CoherenceCorner')
    % Use specified value
    CoherenceCorner = str2num(Parameters.CoherenceCorner);
else
    % Use default (fairly arbitrary)
    CoherenceCorner = max([CoherenceMin*1.1 0.25]);
end

% Specifies the time component of an ordered pair that is located on the 
% alarming curve. The ordered pair specifies the shape of the curve.
if isfield(Parameters,'TimeCorner')
    % Use specified value
    TimeCorner = str2num(Parameters.TimeCorner);
else
    % Use default (fairly arbitrary)
    TimeCorner = max([TimeMin*1.1 60]);
end

ExtractedParameters = struct('CoherenceAlarm',CoherenceAlarm,...
    'CoherenceMin',CoherenceMin,'TimeMin',TimeMin,...
    'CoherenceCorner',CoherenceCorner,'TimeCorner',TimeCorner);