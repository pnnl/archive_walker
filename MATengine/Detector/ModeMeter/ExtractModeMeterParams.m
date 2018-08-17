% This function pulls the parameters, required for implementing mode meter algorithms, out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified.

function ExtractedParameters = ExtractModeMeterParams(Parameters,fs)

% Number of samples to use in the analysis
if isfield(Parameters,'ResultPathFinal')
    % Use specified value
    ResultPathFinal =Parameters.ResultPathFinal;
end

if isfield(Parameters,'Mode')
    % Find the number of separate instances of this detector type.
    %     ModeName = cell(1,length(Parameters.Mode));
    %     AnalysisLength = ones(1,length(Parameters.Mode));
    %     DampRatioThreshold = .05*ones(1,length(Parameters.Mode)); %default value
    %     RetConTrackingStatus = cell(1,length(Parameters.Mode));
    %     MaxRetConLength = NaN(1,length(Parameters.Mode));
    %     DesiredModes = cell(1,length(Parameters.Mode));
    %AlgSpecificParameters = cell(1,length(Parameters.Mode));
    %     FOdetector = cell(1,length(Parameters.Mode));
    %     MethodName = cell(1,length(Parameters.Mode));
    ExtractedParameters = cell(1,length(Parameters.Mode));
    if length(Parameters.Mode)==1
        Parameters.Mode = {Parameters.Mode};
    end
    for ModeIdx = 1:length(Parameters.Mode)
        ModeName = Parameters.Mode{ModeIdx}.Name;
        TempXML = Parameters.Mode{ModeIdx};
        % Number of samples to use in the analysis
        if isfield(TempXML,'AnalysisLength')
            % Use specified value
            AnalysisLength = str2double(TempXML.AnalysisLength)*fs{ModeIdx};
        else
            % Use default value of 10 minutes
            AnalysisLength = 20*60*fs{ModeIdx};
        end
        if isfield(TempXML,'DampRatioThreshold')
            % Use specified limit
            DampRatioThreshold = str2num(TempXML.DampRatioThreshold);
            if DampRatioThreshold>100
                DampRatioThreshold = 100;
                warning('Damping ratio detection threshold input cannot exceed 100%, so setting the threshold to 100%.');
            end
        else
            % Use default (disable)
            DampRatioThreshold = .05;
        end
        %Parameter for implementing retroactice continuity
        if isfield(TempXML,'RetConTracking')
            % User specified value
            if isfield(TempXML.RetConTracking,'Status')
                RetConTrackingStatus = TempXML.RetConTracking.Status;
            else
                RetConTrackingStatus = 'OFF';
            end
            if isfield(TempXML.RetConTracking,'MaxLength')
                MaxRetConLength = str2double(TempXML.RetConTracking.MaxLength);
            else
                MaxRetConLength = NaN;
            end
        else
            % Use default value of 'OFF' setting
            RetConTrackingStatus = 'OFF';
            MaxRetConLength = NaN;
        end
        
        
        if isfield(TempXML,'DesiredModes')
            % Desired mode parameters: min freq, max freq, freq estimate, max damp
            DesiredModes = [str2double(TempXML.DesiredModes.LowF) str2double(TempXML.DesiredModes.HighF) str2double(TempXML.DesiredModes.GuessF) str2double(TempXML.DesiredModes.DampMax)];
            %         else
            %             warning('Please enter parameters required for selecting the mode of interest');
            %DesiredModes = [];
        end
        FOdetectorParaFlag  = 0;
        if isfield(TempXML,'AlgNames')
            %FInds the number of different algorithms implemented for a
            %given instance of selected modemeter
            NumMethods = length(TempXML.AlgNames);
            ExtrctParamXML = TempXML.AlgNames;
            if NumMethods == 1
                % By default, the contents of DetectorXML.(DetectorType{1}) would not be
                % in a cell array because length is one. This makes it so the same
                % indexing can be used in the following for loop.
                ExtrctParamXML = {ExtrctParamXML};
            end
            %             MethodName = cell(1,NumMethods);
            %             AlgSpecificParameters = cell(1,NumMethods);
            for MethodIdx = 1:NumMethods
                MethodName{1}{MethodIdx} = ExtrctParamXML{MethodIdx}.Name;
                switch ExtrctParamXML{MethodIdx}.Name
                    case 'LS_ARMA'
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha);
                    case 'YW_ARMA'
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'L',L);
                    case 'YW_ARMApS'
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        LFO = str2double(ExtrctParamXML{MethodIdx}.LFO);
                        FOdetectorParaFlag =1;
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'L',L,'LFO',LFO);
                    case 'LS_ARMApS'
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        FOdetectorParaFlag =1;
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha);
                    otherwise
                        error([ExtrctParamXML{MethodIdx}.AlgName ' is not a valid mode-meter algorithm. Select LS-ARMA, YW-ARMA, LS-ARMA+S or YW-ARMA+S.']);
                end
            end
            %FO detection parameters
            if FOdetectorParaFlag ==1 && isfield(TempXML,'FOdetectorParam')
                FOParamExtrXML = TempXML.FOdetectorParam;
                FOdetector = struct('WindowType',[], 'FrequencyInterval',[],'ZeroPadding',[],...
                    'WindowLength',[],'WindowOverlap',[],'MedianFilterFrequencyWidth',[],...
                    'FrequencyTolerance',[],'MedianFilterOrder',[],'Pfa',[],'FrequencyMin',[],'FrequencyMax',[]);
                FOdetector.FrequencyMin = 0;
                FOdetector.FrequencyMax = fs{ModeIdx}/2;
                if isfield(FOParamExtrXML,'WindowType')
                    % Use specified window
                    FOdetector.WindowType = FOParamExtrXML.WindowType;
                else
                    % Use default window
                    FOdetector.WindowType = 'hann';
                end
                
                % Zero padded length of the test statistic periodogram, Daniell-Welch
                % periodogram, and GMSC. If omitted, no zero padding is implemented.
                if isfield(FOParamExtrXML,'FrequencyInterval')
                    % Use specified zero padding
                    FOdetector.FrequencyInterval = str2double(FOParamExtrXML.FrequencyInterval);
                    FOdetector.ZeroPadding = round(fs{ModeIdx}/FOParamExtrXML.FrequencyInterval);
                else
                    % Use default zero padding (none for the test statistic periodogram)
                    FOdetector.ZeroPadding = AnalysisLength;
                    FOdetector.FrequencyInterval = fs{ModeIdx}/FOdetector.ZeroPadding;
                end
                
                % Length of the sections for the Daniell-Welch periodogram and GMSC. If
                % omitted, default is 1/3 of K (AnalyisLength).
                if isfield(FOParamExtrXML,'WindowLength')
                    % Use specified window length
                    FOdetector.WindowLength = str2double(FOParamExtrXML.WindowLength)*fs{ModeIdx};
                else
                    % Use default window length
                    FOdetector.WindowLength = floor(AnalysisLength/3);
                end
                
                % Amount of overlap between sections for the Daniell-Welch periodogram and
                % GMSC. If omitted, default is half of WindowLength
                if isfield(FOParamExtrXML,'WindowOverlap')
                    % Use specified window overlap
                    FOdetector.WindowOverlap = str2double(FOParamExtrXML.WindowOverlap)*fs{ModeIdx};
                else
                    % Use default window overlap
                    FOdetector.WindowOverlap = floor(FOdetector.WindowLength/2);
                end
                
                % Order for the median filter used in the Daniell-Welch periodogram. If
                % omitted, the default is the smallest odd integer greater than or equal to
                % three times the main lobe width of the applied window. If an even number,
                % a number less than 3, or a non-integer is specified, the smallest odd
                % integer greater than or equal to 3 and the specified value will be used.
                if isfield(FOParamExtrXML,'MedianFilterFrequencyWidth')
                    % Use specified median filter order
                    FOdetector.MedianFilterFrequencyWidth = str2double(FOParamExtrXML.MedianFilterFrequencyWidth);
                    
                    % Round to ensure that an integer is selected
                    FOdetector.MedianFilterOrder = round(FOParamExtrXML.MedianFilterFrequencyWidth/FOParamExtrXML.FrequencyInterval);
                    
                    % If the median filter is less than 3, set it to 3
                    if FOdetector.MedianFilterOrder < 3
                        FOdetector.MedianFilterOrder = 3;
                    end
                else
                    % Use default median filter order.
                    % The term inside ceil() is 3 times the main lobe width in radians per
                    % sample times the amount of zero padding divided by 2 times pi. The
                    % result is 3 times the main lobe width in bins. The full expressions
                    % are included in the comments, but the expressions have been reduced
                    % to avoid numerical errors when multiplying and dividing by pi.
                    switch FOdetector.WindowType
                        case 'rectwin'
                            FOdetector.MedianFilterOrder = ceil(6*FOdetector.ZeroPadding/FOdetector.WindowLength); % ceil(3 * 4*pi/WindowLength * K0/(2*pi));
                        case 'bartlett'
                            FOdetector.MedianFilterOrder = ceil(12*FOdetector.ZeroPadding/FOdetector.WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
                        case 'hann'
                            FOdetector.MedianFilterOrder = ceil(12*FOdetector.ZeroPadding/FOdetector.WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
                        case 'hamming'
                            FOdetector.MedianFilterOrder = ceil(12*FOdetector.ZeroPadding/FOdetector.WindowLength); % ceil(3 * 8*pi/WindowLength * K0/(2*pi));
                        case 'blackman'
                            FOdetector.MedianFilterOrder = ceil(18*FOdetector.ZeroPadding/FOdetector.WindowLength); % ceil(3 * 12*pi/WindowLength * K0/(2*pi));
                    end
                end
                % If the median filter order is even, add one to make it odd.
                if mod(FOdetector.MedianFilterOrder,2) == 0
                    FOdetector.MedianFilterOrder = FOdetector.MedianFilterOrder + 1;
                end
                
                
                % Probability of false alarm. With zero padding it becomes a maximum
                if isfield(FOParamExtrXML,'Pfa')
                    % Use specified probability of false alarm
                    FOdetector.Pfa = str2num(FOParamExtrXML.Pfa);
                else
                    % Use default probability of false alarm
                    FOdetector.Pfa = 0.01;
                end
                
                % Minimum frequency to be considered. If omitted, the default is zero, but
                % in many cases this will cause excessive false alarms.
                if isfield(FOParamExtrXML,'FrequencyMin')
                    % Use specified minimum frequency
                    FOdetector.FrequencyMin = str2num(FOParamExtrXML.FrequencyMin);
                else
                    % Use default minimum frequency
                    FOdetector.FrequencyMin = 0;
                end
                 if isfield(FOParamExtrXML,'FrequencyMax')
                    % Use specified minimum frequency
                    FOdetector.FrequencyMax = str2num(FOParamExtrXML.FrequencyMax);
                    if FOdetector.FrequencyMax>fs{ModeIdx}/2
                        FOdetector.FrequencyMax = fs{ModeIdx}/2;
                        warning('Maximum frequency for forced oscillation detection exceeds folding frequency of the signal, so changing maximum frequency to the folding frequency');
                    end
                else
                    % Use default minimum frequency
                    FOdetector.FrequencyMax = fs{ModeIdx}/2;
                end
                
                % Tolerance used to refine the frequency estimate. If omitted, the default
                % is the greater of 1) the main lobe width of the test statistic
                % periodogram's window and 2) half of FrequencyMin.
                if isfield(FOParamExtrXML,'FrequencyTolerance')
                    % Use specified frequency tolerance
                    FOdetector.FrequencyTolerance = str2num(FOParamExtrXML.FrequencyTolerance);
                else
                    % Use default frequency tolerance: the sampling rate (samples/second)
                    % times the main lobe width (radians/sample) times the cycle-radians
                    % ratio (cycles/(2pi radians)). The result is the main lobe width
                    % in Hz. The full expression is included in the comments, but a reduced
                    % expression is used to avoid numerical errors introduced by
                    % multiplying and dividing by pi.
                    switch FOdetector.WindowType
                        case 'rectwin'
                            FOdetector.FrequencyTolerance = 2*fs{ModeIdx}/AnalysisLength; % fs * 4*pi/AnalysisLength / (2*pi);
                        case 'bartlett'
                            FOdetector.FrequencyTolerance = 4*fs{ModeIdx}/AnalysisLength; % fs * 8*pi/AnalysisLength / (2*pi);
                        case 'hann'
                            FOdetector.FrequencyTolerance = 4*fs{ModeIdx}/AnalysisLength; % fs * 8*pi/AnalysisLength / (2*pi);
                        case 'hamming'
                            FOdetector.FrequencyTolerance = 4*fs{ModeIdx}/AnalysisLength; % fs * 8*pi/AnalysisLength / (2*pi);
                        case 'blackman'
                            FOdetector.FrequencyTolerance = 6*fs{ModeIdx}/AnalysisLength; % fs * 12*pi/AnalysisLength / (2*pi);
                    end
                    % Round to nearest 100th of a Hz
                    FOdetector.FrequencyTolerance = round(FOdetector.FrequencyTolerance,2);
                end
            else
                FOdetector = struct([]);
            end
        end
        ExtractedParameters{ModeIdx} = struct('ModeName',ModeName,'DampRatioThreshold',DampRatioThreshold,...
            'AnalysisLength',AnalysisLength,'RetConTrackingStatus',RetConTrackingStatus,'MaxRetConLength',MaxRetConLength,...
            'DesiredModes',DesiredModes,'ResultPathFinal',ResultPathFinal,'FOdetectorPara',FOdetector,'MethodName',MethodName,'AlgorithmSpecificParameters',AlgSpecificParameters);%
    end
end
