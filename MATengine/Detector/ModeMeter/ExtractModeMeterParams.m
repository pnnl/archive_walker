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
    for ModeIdx = 1:length(Parameters.Mode)
        ModeName = Parameters.Mode{ModeIdx}.Name;
        TempXML = Parameters.Mode{ModeIdx};
        % Number of samples to use in the analysis
        if isfield(TempXML,'AnalysisLength')
            % Use specified value
            AnalysisLength = str2double(TempXML.AnalysisLength)*fs{ModeIdx};
        else
            % Use default value of 20 minutes
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
                        FunctionName = 'LS_ARMA';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName);
                    case 'YW_ARMA'
                        FunctionName = 'YW_ARMA';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'L',L,'FunctionName',FunctionName);
                    case 'YW_ARMApS'
                        FunctionName = 'YW_ARMA';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        LFO = str2double(ExtrctParamXML{MethodIdx}.LFO);
                        FOdetectorParaFlag =1;
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'L',L,'LFO',LFO,'FunctionName',FunctionName);
                    case 'LS_ARMApS'
                        FunctionName = 'LS_ARMA';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        FOdetectorParaFlag =1;
                        AlgSpecificParameters{1}{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName);
                    otherwise
                        error([ExtrctParamXML{MethodIdx}.AlgName ' is not a valid mode-meter algorithm. Select LS-ARMA, YW-ARMA, LS-ARMA+S or YW-ARMA+S.']);
                end
            end
            
            if FOdetectorParaFlag ==1 && isfield(TempXML,'FOdetectorParam')
                % Use the same parameter extractor that the main
                % periodogram detector uses
                % Set te analysis length for the detector equal to the
                % length of the mode meter analysis window
                TempXML.FOdetectorParam.AnalysisLength = num2str(round(AnalysisLength/fs{ModeIdx}));
                FOdetector = ExtractFOdetectionParamsPer(TempXML.FOdetectorParam,fs{ModeIdx});
                
                if isfield(TempXML,'FOtimeLocParam')
                    % Retrieve time localization parameters
                    TimeLocParams = ExtractFOtimeLocParameters(TempXML.FOtimeLocParam,fs{ModeIdx});
                else
                    % Pass an empty structure to return parameters with
                    % time localization disabled.
                    TimeLocParams = ExtractFOtimeLocParameters(struct(),fs{ModeIdx});
                end
            else
                % FO detector is not desired. Setting the configuration
                % variable to empty ensures that the FO detector is
                % disabled in ModeMeterDetector
                FOdetector = [];
                % No need for time localization. Pass an empty structure to
                % return parameters with time localization disabled.
                TimeLocParams = ExtractFOtimeLocParameters(struct(),fs{ModeIdx});
            end
            
            % Event detection parameters
            if isfield(TempXML,'EventDetectorParam')
                EventParamExtrXML = TempXML.EventDetectorParam;
                
                % Length for RMS calculation
                if isfield(EventParamExtrXML,'RMSlength')
                    % Use specified length
                    RMSlength = str2double(EventParamExtrXML.RMSlength)*fs{ModeIdx};

                    if isnan(RMSlength)
                        % str2double sets the value to NaN when it can't make it a number
                        warning('RMSlength is not a number. Default of 15 will be used.');
                        RMSlength = 15*fs{ModeIdx};
                    end
                else
                    % Use default length
                    RMSlength = 15*fs{ModeIdx};
                end

                % Forgetting factor for threshold
                if isfield(EventParamExtrXML,'RMSmedianFilterTime')
                    % Use specified filter order, converted to samples from time
                    RMSmedianFilterOrder = round(str2double(EventParamExtrXML.RMSmedianFilterTime)*fs{ModeIdx});

                    if isnan(RMSmedianFilterOrder)
                        % str2double sets the value to NaN when it can't make it a number
                        warning('RMSmedianFilterTime is not a number. Default of 120 seconds will be used.');
                        RMSmedianFilterOrder = 120*fs{ModeIdx};
                    end

                    if (RMSmedianFilterOrder<=0)
                        warning('RMSmedianFilterTime must be positive. Default of 120 seconds will be used.');
                        RMSmedianFilterOrder = 120*fs{ModeIdx};
                    end
                else
                    % Use default filter time
                    RMSmedianFilterOrder = 120*fs{ModeIdx};
                end
                % Ensure that the median filter order is odd
                if mod(RMSmedianFilterOrder,2) == 0
                    RMSmedianFilterOrder = RMSmedianFilterOrder + 1;
                end

                % Scaling term for threshold
                if isfield(EventParamExtrXML,'RingThresholdScale')
                    % Use specified scaling term
                    RingThresholdScale = str2double(EventParamExtrXML.RingThresholdScale);

                    if isnan(RingThresholdScale)
                        % str2double sets the value to NaN when it can't make it a number
                        warning('RingThresholdScale is not a number. Default of 3 will be used.');
                        RingThresholdScale = 3;
                    end

                    if RingThresholdScale < 0
                        warning('RingThresholdScale cannot be negative. Default of 3 will be used.');
                        RingThresholdScale = 3;
                    end
                else
                    % Use default scaling term
                    RingThresholdScale = 3;
                end
                
                % Minimum analysis window length
                if isfield(EventParamExtrXML,'MinAnalysisLength')
                    % Use specified value
                    MinAnalysisLength = str2double(EventParamExtrXML.MinAnalysisLength)*fs{ModeIdx};

                    if isnan(RingThresholdScale)
                        % str2double sets the value to NaN when it can't make it a number
                        error('MinAnalysisLength is not a number.');
                    end

                    if RingThresholdScale < 0
                        error('MinAnalysisLength cannot be negative.');
                    end
                else
                    % Use default value (no adjustment to shorter window)
                    MinAnalysisLength = AnalysisLength;
                end


                EventDetector = struct('RingThresholdScale',RingThresholdScale,...
                    'RMSlength',RMSlength,'RMSmedianFilterOrder',RMSmedianFilterOrder,...
                    'MinAnalysisLength',MinAnalysisLength);
            else
                EventDetector = struct([]);
            end
        else
            % Proper informatio was not passed. Return empty parameter
            % structures. This might cause errors later, so issue a warning.
            warning('The configuration file is not properly formatted for a mode meter.');
            FOdetector = [];
            MethodName = [];
            AlgSpecificParameters = [];
            TimeLocParams = ExtractFOtimeLocParameters(struct(),fs);
        end
        ExtractedParameters{ModeIdx} = struct('ModeName',ModeName,'DampRatioThreshold',DampRatioThreshold,...
            'AnalysisLength',AnalysisLength,'RetConTrackingStatus',RetConTrackingStatus,'MaxRetConLength',MaxRetConLength,...
            'DesiredModes',DesiredModes,'ResultPathFinal',ResultPathFinal,'FOdetectorPara',FOdetector,'EventDetectorPara',EventDetector,...
            'MethodName',MethodName,'AlgorithmSpecificParameters',AlgSpecificParameters,'TimeLocParams',TimeLocParams);
    end
end
