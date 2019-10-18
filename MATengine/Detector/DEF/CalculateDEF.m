%   function DEF = PerformDEF(PMU,params,Freq)
%
%   This function calculates the Dissipation Energy Flow (DEF) for the 
%   Periodogram, Spectral Coherence, and Mode Meter detectors.
%
%   Inputs:
%           PMU: Structure containing PMU data
%           params: Parameters for calculating the DEF, along with the
%                   signals used in the calculation
%           Freq: A vector specifying the frequencies the DEF should be
%           calculated at.
%   Outputs:
%           DEF: DEF values

function DEF = CalculateDEF(PMU,Parameters,Freq)

NumF = length(Freq);


% Extract the data used in calculating the DEF
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString, TimeDT] = ExtractData(PMU,Parameters);
catch
    warning('Input data for the DEF calculator could not be used.');
    DEF = NaN(length(Parameters.PathIdx),NumF);
    return
end
N = size(Data,1);

% In InitializeBAWS, the signals listed in params are in the following
% order for each path: VM, VA, P, Q
DataVM = Data(:,1:4:end);
DataVA = Data(:,2:4:end);
DataP = Data(:,3:4:end);
DataQ = Data(:,4:4:end);

VAunits = DataUnit(2:4:end);

ExtractedParams = ExtractParameters(Parameters.params,fs);

% Unwrap voltage angles and convert to radians (conversion to radians is
% for unwrap, but there's not reason to convert back after)
for k = 1:size(DataVA,2)
    if strcmp(VAunits{k},'DEG')
        % Data is in degrees
        DataVA(:,k) = unwrap(DataVA(:,k)*pi/180);
    else
        % Data is already in radians
        DataVA(:,k) = unwrap(DataVA(:,k));
    end
end

% Perform time localization
if ExtractedParams.PerformTimeLoc
    TimeLoc = RunTimeLocalization([DataP DataQ],Freq,ExtractedParams,fs);
    
    % If input signals had too many NaNs, time localization will return a
    % matrix full of NaNs (only need to check one entry)
    if isnan(TimeLoc(1,1))
        % Use entire signal
        TimeLoc = [ones(NumF,1) N*ones(NumF,1)];
    end
else
    % Do not perform time localization - use entire signal
    TimeLoc = [ones(NumF,1) N*ones(NumF,1)];
end

% For each of the FO frequencies of interest
DEFline = NaN(size(DataVM,2),NumF);
for Fidx = 1:NumF
    % Index corresponding to the time localization results
    tidx = TimeLoc(Fidx,1):TimeLoc(Fidx,2);
    Nt = length(tidx);

    % VM - convert to L-L kV (scaling shouldn't actually matter), take log, and remove mean
    VM = log(DataVM(tidx,:));
    VM = VM - (ones(Nt,1)*mean(VM));

    % VA - convert to angular frequency and remove mean
    if tidx(1) > 1
        % If possible, add a sample at the beginning to account for the
        % sample lost due to diff()
        tidx2 = [tidx(1)-1 tidx];
        VA = diff(DataVA(tidx2,:))*fs;
    elseif tidx(end) < N
        % If possible, add a sample at the end to account for the
        % sample lost due to diff()
        tidx2 = [tidx tidx(end)+1];
        VA = diff(DataVA(tidx2,:))*fs;
    else
        % Repeat the first value to account for the sample lost due to
        % diff()
        VA = diff(DataVA(tidx,:))*fs;
        VA = [VA(1,:); VA];
    end
    VA = VA - (ones(Nt,1)*mean(VA));

    % PP - subtract mean
    PP = DataP(tidx,:) - (ones(Nt,1)*mean(DataP(tidx,:)));
    % PQ - subtract mean
    PQ = DataQ(tidx,:) - (ones(Nt,1)*mean(DataQ(tidx,:)));
    
    % Calculate DEF
    Spw = cpsd(PP,VA,boxcar(Nt),0,[Freq(Fidx) Freq(Fidx)+0.1],fs);
    Sqv = cpsd(PQ,VM,boxcar(Nt),0,[Freq(Fidx) Freq(Fidx)+0.1],fs);
    if size(PP,2) == 1
        DEFline(:,Fidx) = 2*(real(Spw(1)) + 2*pi*Freq(Fidx)*imag(Sqv(1)));
    else
        DEFline(:,Fidx) = 2*(real(Spw(1,:)) + 2*pi*Freq(Fidx)*imag(Sqv(1,:)));
    end
end

% Iterate through each of the paths. For each path, sum up the DEF for the
% lines making up that path. The sign of the DEF may be switched to make
% sure the DEF is flowing in the same direction for all lines. This was
% determined in InitializeBAWS.
DEF = zeros(length(Parameters.PathIdx),NumF);
for p = 1:length(Parameters.PathIdx)
    for pp = Parameters.PathIdx{p}
        DEF(p,:) = DEF(p,:) + DEFline(pp,:)*Parameters.PathSign(pp);
    end
end

end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters,fs)

if isfield(Parameters,'PerformTimeLoc')
    if strcmp(Parameters.PerformTimeLoc,'TRUE')
        PerformTimeLoc = true;
    else
        PerformTimeLoc = false;
        ExtractedParameters = struct('PerformTimeLoc',PerformTimeLoc);
        return
    end
else
    PerformTimeLoc = false;
    ExtractedParameters = struct('PerformTimeLoc',PerformTimeLoc);
    return
end

% Minimum analysis window length
if isfield(Parameters,'LocMinLength')
    LocMinLength = str2double(Parameters.LocMinLength);
    
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
    LocLengthStep = str2double(Parameters.LocLengthStep);
    
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
    LocRes = str2double(Parameters.LocRes);
    
    if isnan(LocRes)
        % str2double sets the value to NaN when it can't make it a number
        warning('LocRes is not a number. Default of 60 seconds will be used.');
        LocRes = 60*fs;
    end
else
    % Use default value
    LocRes = 60*fs;
end

ExtractedParameters = struct('PerformTimeLoc',PerformTimeLoc,...
    'LocMinLength',LocMinLength,'LocLengthStep',LocLengthStep,...
    'LocRes',LocRes);
end