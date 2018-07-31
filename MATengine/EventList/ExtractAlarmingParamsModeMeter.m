% function ExtractedParameters = ExtractAlarmingParamsPer(Parameters)
%
% This function reads and sets defaults for parameters for
% setting alarms for modemeters.
%
% Inputs:
%   Parameters = structure array containing the parameters related to
%                setting alarms for modemeter. This is a portion of
%                the structure array containing all the parameters from the
%                detector XML.
%
% Outputs:
%   ExtractedParameters = structure array containing the extracted
%                parameters. Its fields are:
%                   TimeMin 

function ExtractedParameters = ExtractAlarmingParamsModeMeter(Parameters)

% An alarm will be set for any detection with an SNR greater than SNRalarm
if isfield(Parameters,'DampRatioThreshold')
    % Use specified limit
    ExtractedParameters.DampRatioThreshold = str2num(Parameters.DampRatioThreshold);
else
    % Use default (disable)
    ExtractedParameters.DampRatioThreshold = .05;
end

