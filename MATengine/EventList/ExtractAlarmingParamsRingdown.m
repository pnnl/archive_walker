% function ExtractedParameters = ExtractAlarmingParamsRingdown(Parameters)
%
% This function reads and sets defaults for parameters for
% setting alarms based on ringdown detection results.
%
% Inputs:
%   Parameters = structure array containing the parameters related to
%                setting alarms based on ringdown detection results. This
%                is a portion of the structure array containing all the
%                parameters from the detector XML.
%
% Outputs:
%   ExtractedParameters = structure array containing the extracted
%                parameters. Its fields are:
%                   MaxDuration = the maximum duration for a detected
%                                 ringdown. Detected events longer than
%                                 this will be removed from the event list.
%
% Created by Jim Follum (james.follum@pnnl.gov) on December 8, 2016.

function ExtractedParameters = ExtractAlarmingParamsRingdown(Parameters)

% Maximum duration
if isfield(Parameters,'MaxDuration')
    % Use specified maximum duration
    MaxDuration = str2double(Parameters.MaxDuration);
    
    if isnan(MaxDuration)
        % str2double sets the value to NaN when it can't make it a number
        warning('MaxDuration is not a number. No maximum will be set.');
        MaxDuration = Inf;
    end
else
    % Use default (disable the limit)
    MaxDuration = Inf;
end

ExtractedParameters = struct('MaxDuration',MaxDuration);