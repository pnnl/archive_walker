% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
% Created by: Jim Follum (james.follum@pnnl.gov)
function ExtractedParameters = ExtractFOdetectionParamsSC(Parameters,fs)

% Mode of operation - 'SingleChannel' or 'MultiChannel'
if isfield(Parameters,'Mode')
    % Use specified mode
    Mode = Parameters.Mode;
else
    % Use default mode: single channel
    Mode = 'SingleChannel';
end

% Number of samples to use in the analysis
if isfield(Parameters,'AnalysisLength')
    % Use specified value
    AnalysisLength = str2double(Parameters.AnalysisLength)*fs;
else
    error('AnalysisLength must be specified for the spectral coherence based forced oscillation detector.');
end

% delay in samples used to calculate self-GMC
if isfield(Parameters,'Delay')
    % Use specified value
    Delay = ceil(str2double(Parameters.Delay))*fs;
else
    % Use default value (length of the input signals)
    Delay = floor(AnalysisLength/5);
end

% Number of delays in the self-GMSC
if isfield(Parameters,'NumberDelays')
    % Use specified value
    NumberDelays = ceil(str2double(Parameters.NumberDelays));
    if NumberDelays < 2
        NumberDelays = 2;
    end
else
    % Use default value (length of the input signals)
    NumberDelays = 2;
end

% Number of delays in the self-GMSC
if isfield(Parameters,'ThresholdScale')
    % Use specified value
    ThresholdScale = str2double(Parameters.ThresholdScale);
    if ThresholdScale  < 2
        ThresholdScale  = 2;
    end
else
    % Use default value 
    ThresholdScale = 3;
end

%Type of window used for the self-GMSC. Options are rectangular, bartlett,
%hann, hamming, and blackman. If omitted, default is hann
if isfield(Parameters,'WindowType')
    % Use specified window
    WindowType = Parameters.WindowType;
else
    % Use default window
    WindowType = 'hann';
end

% Zero padded length of the test statistic periodogram, Daniell-Welch
% periodogram, and GMSC. If omitted, no zero padding is implemented.
if isfield(Parameters,'FrequencyInterval')
    % Use specified zero padding
    FrequencyInterval = str2double(Parameters.FrequencyInterval);
    ZeroPadding = round(fs/FrequencyInterval);
else
    % Use default zero padding (none for the test statistic periodogram)
    ZeroPadding = AnalysisLength*fs;
end

% Length of the sections for the self-GMSC. If 
% omitted, default is 1/8 of K (AnalyisLength).
if isfield(Parameters,'WindowLength')
    % Use specified window length
    WindowLength = str2double(Parameters.WindowLength)*fs;
else
    % Use default window length
    WindowLength = floor(AnalysisLength/8);
end

% Amount of overlap between sections for the self-GMSC. If omitted, default
% is half of WindowLength.
if isfield(Parameters,'WindowOverlap')
    % Use specified window overlap
    WindowOverlap = str2double(Parameters.WindowOverlap)*fs;
else
    % Use default window overlap
    WindowOverlap = floor(WindowLength/2);
end

% Minimum frequency to be considered. If omitted, the default is zero, but 
% in many cases this will cause excessive false alarms.
if isfield(Parameters,'FrequencyMin')
    % Use specified minimum frequency
    FrequencyMin = str2num(Parameters.FrequencyMin);
else
    % Use default minimum frequency
    FrequencyMin = 0;
end

% Maximum frequency to be considered. If omitted, the default is the 
% Nyquist frequency
if isfield(Parameters,'FrequencyMax')
    % Use specified maximum frequency
    FrequencyMax = str2num(Parameters.FrequencyMax);
else
    % Use default maximum frequency
    FrequencyMax = fs/2;
end

% Tolerance used to refine the frequency estimate. If omitted, the default 
% is the greater of 1) the main lobe width of the test statistic 
% periodogram's window and 2) half of FrequencyMin.
if isfield(Parameters,'FrequencyTolerance')
    % Use specified frequency tolerance
    FrequencyTolerance = str2num(Parameters.FrequencyTolerance);
else
    % Use default frequency tolerance
    % First term in max() is the minimum frequency divided by 2. The second
    % term is the sampling rate times the main lobe width in radians per
    % sample. The result is the main lobe width in Hz. The full expression
    % is included in the comments, but a reduced expression is used to
    % avoid numerical errors introduced by multiplying and dividing by pi.
    switch WindowType
        case 'rectwin'
            FrequencyTolerance = max([FrequencyMin/2, (2*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 4*pi/WindowLength / (2*pi))]);
        case 'bartlett'
            FrequencyTolerance = max([FrequencyMin/2, (4*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 8*pi/WindowLength / (2*pi))]);
        case 'hann'
            FrequencyTolerance = max([FrequencyMin/2, (4*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 8*pi/WindowLength / (2*pi))]);
        case 'hamming'
            FrequencyTolerance = max([FrequencyMin/2, (4*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 8*pi/WindowLength / (2*pi))]);
        case 'blackman'
            FrequencyTolerance = max([FrequencyMin/2, (6*fs/WindowLength)]); % max([FrequencyMin/2, (fs * 12*pi/WindowLength / (2*pi))]);
    end
end



ExtractedParameters = struct('Mode',Mode,'AnalysisLength',AnalysisLength,...
    'Delay',Delay,'NumberDelays',NumberDelays,'ThresholdScale',ThresholdScale,...
    'WindowType',WindowType,'ZeroPadding',ZeroPadding,...
    'WindowLength',WindowLength,'WindowOverlap',WindowOverlap,...    
    'FrequencyMin',FrequencyMin,'FrequencyMax',FrequencyMax,...
    'FrequencyTolerance',FrequencyTolerance);

end