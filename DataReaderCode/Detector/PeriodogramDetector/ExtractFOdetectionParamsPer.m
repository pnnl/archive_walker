% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified.
% Created by: Jim Follum (james.follum@pnnl.gov)
function ExtractedParameters = ExtractFOdetectionParamsPer(Parameters,fs)

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
    % Use default value of 10 minutes
    AnalysisLength = 10*60*fs;
end

% Type of window used for the test statistic periodogram, Daniell-Welch
% periodogram, and GMSC. Options are rectwin, bartlett, hann, hamming, and
% blackman. If omitted, default is hann.
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
    ZeroPadding = AnalysisLength;
    FrequencyInterval = fs/ZeroPadding;
end

% Length of the sections for the Daniell-Welch periodogram and GMSC. If
% omitted, default is 1/3 of K (AnalyisLength).
if isfield(Parameters,'WindowLength')
    % Use specified window length
    WindowLength = str2double(Parameters.WindowLength)*fs;
else
    % Use default window length
    WindowLength = floor(AnalysisLength/3);
end

% Amount of overlap between sections for the Daniell-Welch periodogram and
% GMSC. If omitted, default is half of WindowLength
if isfield(Parameters,'WindowOverlap')
    % Use specified window overlap
    WindowOverlap = str2double(Parameters.WindowOverlap)*fs;
else
    % Use default window overlap
    WindowOverlap = floor(WindowLength/2);
end

% Order for the median filter used in the Daniell-Welch periodogram. If
% omitted, the default is the smallest odd integer greater than or equal to
% three times the main lobe width of the applied window. If an even number,
% a number less than 3, or a non-integer is specified, the smallest odd
% integer greater than or equal to 3 and the specified value will be used.
if isfield(Parameters,'MedianFilterFrequencyWidth')
    % Use specified median filter order
    MedianFilterFrequencyWidth = str2double(Parameters.MedianFilterFrequencyWidth);
    
    % Round to ensure that an integer is selected
    MedianFilterOrder = round(MedianFilterFrequencyWidth/FrequencyInterval);
    
    % If the median filter is less than 3, set it to 3
    if MedianFilterOrder < 3
        MedianFilterOrder = 3;
    end
else
    % Use default median filter order.
    % The term inside ceil() is 3 times the main lobe width in radians per
    % sample times the amount of zero padding divided by 2 times pi. The
    % result is 3 times the main lobe width in bins. The full expressions
    % are included in the comments, but the expressions have been reduced
    % to avoid numerical errors when multiplying and dividing by pi.
    switch WindowType
        case 'rectwin'
            MedianFilterOrder = ceil(6*ZeroPadding/WindowLength); % ceil(3 * 4*pi/WindowLength * K0/(2*pi));
        case 'bartlett'
            MedianFilterOrder = ceil(12*ZeroPadding/WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
        case 'hann'
            MedianFilterOrder = ceil(12*ZeroPadding/WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
        case 'hamming'
            MedianFilterOrder = ceil(12*ZeroPadding/WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
        case 'blackman'
            MedianFilterOrder = ceil(18*ZeroPadding/WindowLength); % ceil(3 * 12*pi/WindowLength * K0/(2*pi));
    end
end
% If the median filter order is even, add one to make it odd.
if mod(MedianFilterOrder,2) == 0
    MedianFilterOrder = MedianFilterOrder + 1;
end


% Probability of false alarm. With zero padding it becomes a maximum
if isfield(Parameters,'Pfa')
    % Use specified probability of false alarm
    Pfa = str2num(Parameters.Pfa);
else
    % Use default probability of false alarm
    Pfa = 0.01;
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
    'WindowType',WindowType,'ZeroPadding',ZeroPadding,...
    'WindowLength',WindowLength,'WindowOverlap',WindowOverlap,...
    'MedianFilterOrder',MedianFilterOrder,'Pfa',Pfa,...
    'FrequencyMin',FrequencyMin,'FrequencyMax',FrequencyMax,...
    'FrequencyTolerance',FrequencyTolerance);

end