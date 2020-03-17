% This function pulls the parameters, required for implementing mode meter algorithms, out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified.

function ExtractedParameters = ExtractModeMeterParams(Parameters,fs,fsFOdet,fsEVENTdet)

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
            
            MethodName = cell(1,NumMethods);
            AlgorithmSpecificParameters = cell(1,NumMethods);
            for MethodIdx = 1:NumMethods
                MethodName{MethodIdx} = ExtrctParamXML{MethodIdx}.Name;
                switch ExtrctParamXML{MethodIdx}.Name
                    case 'LS_ARMA'
                        FunctionName = 'LS_ARMApS';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit);
                    case 'YW_ARMA'
                        FunctionName = 'YW_ARMApS';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'L',L,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit);
                    case 'YW_ARMApS'
                        FunctionName = 'YW_ARMApS';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        LFO = str2double(ExtrctParamXML{MethodIdx}.LFO);
                        FOdetectorParaFlag = 1;
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'L',L,'LFO',LFO,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit);
                    case 'LS_ARMApS'
                        FunctionName = 'LS_ARMApS';
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        FOdetectorParaFlag = 1;
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit);
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
                FOdetector = ExtractFOdetectionParamsPer(TempXML.FOdetectorParam,fsFOdet{ModeIdx});
                
                FOdetector.PMU = TempXML.FOdetectorParam.PMU;
                
                if isfield(TempXML,'FOtimeLocParam')
                    % Retrieve time localization parameters
                    TimeLocParams = ExtractFOtimeLocParameters(TempXML.FOtimeLocParam,fsFOdet{ModeIdx});
                else
                    % Pass an empty structure to return parameters with
                    % time localization disabled.
                    TimeLocParams = ExtractFOtimeLocParameters(struct(),fsFOdet{ModeIdx});
                end
            else
                % FO detector is not desired. Setting the configuration
                % variable to empty ensures that the FO detector is
                % disabled in ModeMeterDetector
                FOdetector = [];
                % No need for time localization. Pass an empty structure to
                % return parameters with time localization disabled.
                TimeLocParams = ExtractFOtimeLocParameters(struct(),fsFOdet{ModeIdx});
            end
            
            % Event detection parameters
            if isfield(TempXML,'EventDetectorParam')
                EventParamExtrXML = TempXML.EventDetectorParam;
                
                % Length for RMS calculation
                if isfield(EventParamExtrXML,'RMSlength')
                    % Use specified length
                    RMSlength = str2double(EventParamExtrXML.RMSlength)*fsEVENTdet{ModeIdx};

                    if isnan(RMSlength)
                        % str2double sets the value to NaN when it can't make it a number
                        warning('RMSlength is not a number. Default of 8 will be used.');
                        RMSlength = 8*fsEVENTdet{ModeIdx};
                    end
                else
                    % Use default length
                    RMSlength = 8*fsEVENTdet{ModeIdx};
                end

                % Forgetting factor for threshold
                if isfield(EventParamExtrXML,'RMSmedianFilterTime')
                    % Use specified filter order, converted to samples from time
                    RMSmedianFilterOrder = round(str2double(EventParamExtrXML.RMSmedianFilterTime)*fsEVENTdet{ModeIdx});

                    if isnan(RMSmedianFilterOrder)
                        % str2double sets the value to NaN when it can't make it a number
                        warning('RMSmedianFilterTime is not a number. Default of 120 seconds will be used.');
                        RMSmedianFilterOrder = 120*fsEVENTdet{ModeIdx};
                    end

                    if (RMSmedianFilterOrder<=0)
                        warning('RMSmedianFilterTime must be positive. Default of 120 seconds will be used.');
                        RMSmedianFilterOrder = 120*fsEVENTdet{ModeIdx};
                    end
                else
                    % Use default filter time
                    RMSmedianFilterOrder = 120*fsEVENTdet{ModeIdx};
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
                        warning('RingThresholdScale is not a number. Default of 5 will be used.');
                        RingThresholdScale = 5;
                    end

                    if RingThresholdScale < 0
                        warning('RingThresholdScale cannot be negative. Default of 5 will be used.');
                        RingThresholdScale = 5;
                    end
                else
                    % Use default scaling term
                    RingThresholdScale = 5;
                end
                
                % Minimum analysis window length
                if isfield(EventParamExtrXML,'MinAnalysisLength')
                    % Use specified value
                    N2 = str2double(EventParamExtrXML.MinAnalysisLength)*fsEVENTdet{ModeIdx};

                    if isnan(N2)
                        % str2double sets the value to NaN when it can't make it a number
                        error('MinAnalysisLength is not a number.');
                    end

                    if N2 < 0
                        error('MinAnalysisLength cannot be negative.');
                    end
                else
                    % Use default value (no adjustment to shorter window)
                    N2 = AnalysisLength;
                end
                
                % Forgetting factor 1
                if isfield(EventParamExtrXML,'ForgetFactor1')
                    % Use specified value
                    if strcmp(EventParamExtrXML.ForgetFactor1, 'TRUE')
                        % Calculate the forgetting factor based on the
                        % window length
                        lam1 = (AnalysisLength-1)/AnalysisLength;
                    else
                        % No forgetting
                        lam1 = 1;
                    end
                else
                    % Use default value (no forgetting factor)
                    lam1 = 1;
                end
                
                % Forgetting factor 2
                if isfield(EventParamExtrXML,'ForgetFactor2')
                    % Use specified value
                    if strcmp(EventParamExtrXML.ForgetFactor2, 'TRUE')
                        % Calculate the forgetting factor based on the
                        % window length
                        lam2 = (N2-1)/N2;
                    elseif strcmp(EventParamExtrXML.ForgetFactor2, 'MATCH')
                        % Match the forgetting factor for window 1
                        lam2 = lam1;
                    else
                        % No forgetting 
                        lam2 = 1;
                    end
                else
                    % Use default value (no forgetting factor)
                    lam2 = 1;
                end
                
                % After an event occurs, adjust the window preceeding the
                % event
                if isfield(EventParamExtrXML,'PostEventWinAdj')
                    % Use specified value (DIMINISH, SHORTEN, or FALSE)
                    PostEventWinAdj = EventParamExtrXML.PostEventWinAdj;
                else
                    % Use default value (do ot adjust window)
                    PostEventWinAdj = 'FALSE';
                end
                
                % RMS-energy signals used to detect disturbances
                if isfield(EventParamExtrXML,'PMU')
                    % Use specified signals
                    EventDetPMU = EventParamExtrXML.PMU;
                else
                    % Throw error
                    error('RMS-energy signals must be specified for the mode meter event detector.');
                end

                EventDetector = struct('RingThresholdScale',RingThresholdScale,...
                    'RMSlength',RMSlength,'RMSmedianFilterOrder',RMSmedianFilterOrder,...
                    'N2',N2,'lam1',lam1,'lam2',lam2,'PostEventWinAdj',PostEventWinAdj,...
                    'PMU',EventDetPMU);
            else
                EventDetector = struct([]);
            end
        else
            % Proper informatio was not passed. Return empty parameter
            % structures. This might cause errors later, so issue a warning.
            warning('The configuration file is not properly formatted for a mode meter.');
            FOdetector = [];
            MethodName = [];
            AlgorithmSpecificParameters = [];
            TimeLocParams = ExtractFOtimeLocParameters(struct(),fsFOdet{ModeIdx});
        end
        ExtractedParameters{ModeIdx} = struct('ModeName',ModeName,...
            'AnalysisLength',AnalysisLength,'RetConTrackingStatus',RetConTrackingStatus,'MaxRetConLength',MaxRetConLength,...
            'DesiredModes',DesiredModes,'ResultPathFinal',ResultPathFinal,'FOdetectorPara',FOdetector,'EventDetectorPara',EventDetector,...
            'MethodName',[],'AlgorithmSpecificParameters',[],'TimeLocParams',TimeLocParams);
        % Due to the way Matlab initializes structures, these fields needs to
        % be added separately because they are comprised of cell arrays. This
        % keeps ExtractedParameters{ModeIdx} from becoming a struct array
        ExtractedParameters{ModeIdx}.MethodName = MethodName;
        ExtractedParameters{ModeIdx}.AlgorithmSpecificParameters = AlgorithmSpecificParameters;
    end
end
