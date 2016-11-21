function ExtractedParameters = ExtractAlarmingParamsPer(Parameters)

% An alarm will be set for any detection with an SNR greater than SNRalarm
if isfield(Parameters,'SNRalarm')
    % Use specified limit
    SNRalarm = str2num(Parameters.SNRalarm);
else
    % Use default (disable)
    SNRalarm = inf;
end

% Alarms will not be set for any detection with an SNR less
% than SNRmin, no matter how long they persist.
if isfield(Parameters,'SNRmin')
    % Use specified value
    SNRmin = str2num(Parameters.SNRmin);
else
    % Use default (disable)
    SNRmin = -inf;
end

% Detected forced oscillations must persist for TimeMin seconds before an 
% alarm will be set (unless the SNR is greater than SNRalarm)
if isfield(Parameters,'TimeMin')
    % Use specified value
    TimeMin = str2num(Parameters.TimeMin);
else
    % Use default (disable)
    TimeMin = 0;
end

% Specifies the SNR component of an ordered pair that is located on 
% the alarming curve. The ordered pair specifies the shape of the curve.
if isfield(Parameters,'SNRcorner')
    % Use specified value
    SNRcorner = str2num(Parameters.SNRcorner);
else
    % Use default (fairly arbitrary)
    SNRcorner = max([SNRmin*1.1 0]);
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

ExtractedParameters = struct('SNRalarm',SNRalarm,...
    'SNRmin',SNRmin,'TimeMin',TimeMin,...
    'SNRcorner',SNRcorner,'TimeCorner',TimeCorner);