% This function pulls the parameters, required for implementing mode meter algorithms, out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified.

function ExtractedParameters = ExtractModeMeterParams(Parameters,fs,fsFOdet,fsEVENTdet)

% Folder where results are to be stored. This parameter is created in
% InitializeBAWS from the ResultPath parameter that is in the config file.
if isfield(Parameters,'ResultPathFinal')
    % Use specified value
    ResultPathFinal = Parameters.ResultPathFinal;
else
    error('The path for results is not specified for the Mode Meter. This should be done automatically by the GUI.');
end

if isfield(Parameters,'Mode')
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
        else
            error('The mode parameters must be specified.');
        end
        if isfield(TempXML,'AlgNames')
            %Finds the number of different algorithms implemented for a
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
                        FOrobust = false;   % This flag is needed because the same FunctionName is used for LS-ARMA and LS-ARMA+S
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit,'FOrobust',FOrobust);
                    case 'YW_ARMA'
                        FunctionName = 'YW_ARMApS';
                        FOrobust = false;   % This flag is needed because the same FunctionName is used for YW-ARMA and YW-ARMA+S
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        if isfield(ExtrctParamXML{MethodIdx},'ng')
                            ng = str2double(ExtrctParamXML{MethodIdx}.ng);
                        else
                            ng = 0;
                        end
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'ng',ng,'L',L,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit,'FOrobust',FOrobust);
                    case 'YW_ARMApS'
                        FunctionName = 'YW_ARMApS';
                        FOrobust = true;   % This flag is needed because the same FunctionName is used for YW-ARMA and YW-ARMA+S
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        if isfield(ExtrctParamXML{MethodIdx},'ng')
                            ng = str2double(ExtrctParamXML{MethodIdx}.ng);
                        else
                            ng = 0;
                        end
                        L = str2double(ExtrctParamXML{MethodIdx}.L);
                        LFO = str2double(ExtrctParamXML{MethodIdx}.LFO);
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'ng',ng,'L',L,'LFO',LFO,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit,'FOrobust',FOrobust);
                    case 'LS_ARMApS'
                        FunctionName = 'LS_ARMApS';
                        FOrobust = true;   % This flag is needed because the same FunctionName is used for LS-ARMA and LS-ARMA+S
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        NaNomitLimit = str2double(ExtrctParamXML{MethodIdx}.NaNomitLimit)*fs{ModeIdx};
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName,'NaNomitLimit',NaNomitLimit,'FOrobust',FOrobust);
                    case 'STLS'
                        FunctionName = 'STLS';
                        FOrobust = false;   % This flag is needed because the same FunctionName is used for STLS and STLS+S
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        NumIteration = str2double(ExtrctParamXML{MethodIdx}.NumIteration);
                        thresh = str2double(ExtrctParamXML{MethodIdx}.thresh);
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName,'NumIteration',NumIteration,'thresh',thresh,'FOrobust',FOrobust);
                    case 'STLSpS'
                        FunctionName = 'STLS';
                        FOrobust = true;   % This flag is needed because the same FunctionName is used for STLS and STLS+S
                        na = str2double(ExtrctParamXML{MethodIdx}.na);
                        nb = str2double(ExtrctParamXML{MethodIdx}.nb);
                        n_alpha = str2double(ExtrctParamXML{MethodIdx}.n_alpha);
                        NumIteration = str2double(ExtrctParamXML{MethodIdx}.NumIteration);
                        thresh = str2double(ExtrctParamXML{MethodIdx}.thresh);
                        AlgorithmSpecificParameters{MethodIdx} = struct('na',na,'nb',nb,'n_alpha',n_alpha,'FunctionName',FunctionName,'NumIteration',NumIteration,'thresh',thresh,'FOrobust',FOrobust);
                    otherwise
                        error([ExtrctParamXML{MethodIdx}.AlgName ' is not a valid mode-meter algorithm. Select LS-ARMA, LS-ARMA+S, YW-ARMA, YW-ARMA+S, STLS, or STLS+S.']);
                end
            end
            
            if isfield(TempXML,'FOdetectorParam')
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
                
                % Threshold for RMS-energy detector
                if isfield(EventParamExtrXML,'Threshold')
                    % Use specified value
                    Threshold = str2double(EventParamExtrXML.Threshold);
                else
                    % Throw error
                    error('RMS-energy threshold must be specified for the mode meter event detector.');
                end
                
                % Flag determining whether ringdowns are identified or if
                % all high-energy events are removed
                if isfield(EventParamExtrXML,'RingdownID')
                    if strcmpi(EventParamExtrXML.RingdownID,'TRUE')
                        % User desires ringdown identification
                        RingdownID = true;
                    else
                        % User does not desire ringdown identification
                        RingdownID = false;
                    end
                else
                    % Flag was not specified - disable ringdown
                    % identification
                    RingdownID = false;
                end
                
                % RMS-energy signals used to detect disturbances
                if isfield(EventParamExtrXML,'PMU')
                    % Use specified signals
                    EventDetPMU = EventParamExtrXML.PMU;
                else
                    % Throw error
                    error('RMS-energy signals must be specified for the mode meter event detector.');
                end

                EventDetector = struct('N2',N2,'lam1',lam1,'lam2',lam2,'PostEventWinAdj',PostEventWinAdj,...
                    'PMU',EventDetPMU,'Threshold',Threshold,'RingdownID',RingdownID);
            else
                EventDetector = struct([]);
            end
        else
            % Proper information was not passed. Return empty parameter
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
