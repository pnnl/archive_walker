% function ExtractedParameters = ExtractAlarmingParamsPer(Parameters)
%
% This function reads and sets defaults for parameters for
% setting alarms based on forced oscillation detection results from the
% periodogram method.
%
% Inputs:
%   Parameters = structure array containing the parameters related to
%                setting alarms based on forced oscillation detection
%                results from the periodogram method. This is a portion of
%                the structure array containing all the parameters from the
%                detector XML.
%
% Outputs:
%   ExtractedParameters = structure array containing the extracted
%                parameters. Its fields are:
%                   TimeMin = minimum time in seconds that an oscillation
%                             must persist for an alarm to be set, unless
%                             it exceeds SNRalarm (CoherenceAlarm).
%                   TimeCorner = Along with SNRcorner (CoherenceCorner),
%                                specifies a point on the alarm curve to
%                                adjust the shape of the curve.
%                   SNRalarm = SNR at which an alarm will be set,
%                              regardless of duration 
%                   SNRmin = minimum SNR required for an alarm to be
%                            set, regardless of duration
%                   SNRcorner = Along with TimeCorner, specifies a point on
%                               the alarm curve to adjust the shape of the
%                               curve
%
% Created by Jim Follum (james.follum@pnnl.gov) in November, 2016.

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