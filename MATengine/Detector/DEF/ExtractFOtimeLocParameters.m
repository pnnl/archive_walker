% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractFOtimeLocParameters(Parameters,fs)

if isfield(Parameters,'PerformTimeLoc')
    if ~strcmp(Parameters.PerformTimeLoc,'TRUE')
        ExtractedParameters = struct('PerformTimeLoc',false);
        return
    end
else
    ExtractedParameters = struct('PerformTimeLoc',false);
    return
end

% Minimum analysis window length
if isfield(Parameters,'LocMinLength')
    LocMinLength = str2double(Parameters.LocMinLength)*fs;
    
    if isnan(LocMinLength)
        % str2double sets the value to NaN when it can't make it a number
        warning('LocMinLength is not a number. Default of 60 seconds will be used.');
        LocMinLength = 60*fs;
    end
else
    % Use default value
    LocMinLength = 60*fs;
end

% Resolution for the window length (how big the difference is between each
% window that is checked)
if isfield(Parameters,'LocLengthStep')
    LocLengthStep = str2double(Parameters.LocLengthStep)*fs;
    
    if isnan(LocLengthStep)
        % str2double sets the value to NaN when it can't make it a number
        warning('LocLengthStep is not a number. Default of 60 seconds will be used.');
        LocLengthStep = 60*fs;
    end
else
    % Use default value
    LocLengthStep = 60*fs;
end

% Resolution for the location (how much to slide the window for each check)
if isfield(Parameters,'LocRes')
    LocRes = str2double(Parameters.LocRes)*fs;
    
    if isnan(LocRes)
        % str2double sets the value to NaN when it can't make it a number
        warning('LocRes is not a number. Default of 60 seconds will be used.');
        LocRes = 60*fs;
    end
else
    % Use default value
    LocRes = 60*fs;
end

ExtractedParameters = struct('PerformTimeLoc',true,...
    'LocMinLength',LocMinLength,'LocLengthStep',LocLengthStep,...
    'LocRes',LocRes);