% function ExtractedParameters = ExtractAlarmingParamsSC(Parameters)
%
% This function reads and sets defaults for parameters for
% setting alarms based on forced oscillation detection results from the
% spectral coherence method.
%
% Inputs:
%   Parameters = structure array containing the parameters related to
%                setting alarms based on forced oscillation detection
%                results from the spectral coherence method. This is a portion of
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
%                   CoherenceAlarm = Coherence at which an alarm will 
%                                        be set, regardless of duration
%                   CoherenceMin = minimum Coherence required for an
%                                      alarm to be set, regardless of
%                                      duration 
%                   CoherenceCorner = Along with TimeCorner, specifies
%                                   a point on the alarm curve to adjust
%                                   the shape of the curve
%
% Created by Jim Follum (james.follum@pnnl.gov) in November, 2016.

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