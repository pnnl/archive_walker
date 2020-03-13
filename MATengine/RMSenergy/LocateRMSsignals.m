function ToRetrieve = LocateRMSsignals(ConfigFile)

ConfigAll = fun_xmlread_comments(ConfigFile);
% DataXML = ConfigAll.Config.DataConfig.Configuration;
ProcessXML = ConfigAll.Config.ProcessConfig.Configuration;
% PostProcessCustXML = ConfigAll.Config.PostProcessCustomizationConfig.Configuration;
DetectorXML = ConfigAll.Config.DetectorConfig.Configuration;
% WindAppXML = ConfigAll.Config.WindAppConfig.Configuration;

%% Use ProcessXML to get the list of RMS-energy signals

% Processing is done in stages. Each stage is composed of a filtering step
% and a multirate step.
if isfield(ProcessXML.Processing,'Stages')
    NumProcessingStages = length(ProcessXML.Processing.Stages);
    if NumProcessingStages == 1
        % By default, the contents of ProcessXML.Processing.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        ProcessXML.Processing.Stages = {ProcessXML.Processing.Stages};
    end
else
    NumProcessingStages = 0;
end

FilterParams = struct('CustPMU',{},'CustSignals',{},'Band',{});
for StageIdx = 1:NumProcessingStages
    if isfield(ProcessXML.Processing.Stages{StageIdx},'Filter')
        ProcessFilter = ProcessXML.Processing.Stages{StageIdx}.Filter;
        
        NumFilts = length(ProcessFilter);

        if NumFilts == 1
            % By default, the contents of ProcessFilter would not be in a cell array
            % because length is one. This makes it so the same indexing can be used
            % in the following for loop.
            ProcessFilter = {ProcessFilter};
        end
        
        for FiltIdx = 1:NumFilts
            % Skip any filter that is not an RMS-energy filter
            if ~strcmp(ProcessFilter{FiltIdx}.Type,'RMSenergyFilt')
                continue;
            end
            
            % Get list of PMUs to apply filter to
            NumPMU = length(ProcessFilter{FiltIdx}.PMU);
            if NumPMU == 1
                % By default, the contents of ProcessFilter{FiltIdx}.PMU
                % would not be in a cell array because length is one. This
                % makes it so the same indexing can be used in the following for loop.
                ProcessFilter{FiltIdx}.PMU = {ProcessFilter{FiltIdx}.PMU};
            end

            % Collect all the signals that are to be passed to this filter
            CustName = {};
            for PMUidx = 1:NumPMU
                NumChan = length(ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1     
                    % By default, the contents of ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel 
                    % would not be in a cell array because length is one. This 
                    % makes it so the same indexing can be used in the following for loop.
                    ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel = {ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel};
                end

                for ChanIdx = 1:NumChan
                    if isfield(ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel{ChanIdx},'CustName')
                        CustName = [CustName ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel{ChanIdx}.CustName];
                    else
                        CustName = [CustName ProcessFilter{FiltIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name];
                    end
                end
            end
            
            FilterParams(end+1).CustPMU = ProcessFilter{FiltIdx}.CustPMU;
            FilterParams(end).CustSignals = CustName;
            FilterParams(end).Band = ProcessFilter{FiltIdx}.Parameters.Band;
        end
    end
end

%% Use DetectorXML to find where the RMS-energy signals were written to

DWparams = struct('SavePath',{},'SeparatePMUs',{},'Mnemonic',{},'PMU',{},'Channel',{});
if isfield(DetectorXML,'DataWriter')
    % Find the number of separate instances of the data writer
    NumDetectors = length(DetectorXML.DataWriter);
    if NumDetectors == 1
        % By default, the contents of DetectorXML.DataWriter would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        DetectorXML.DataWriter = {DetectorXML.DataWriter};
    end

    % Iterate through each instance of this detector type
    for DetectorIndex = 1:NumDetectors
        Parameters = DetectorXML.DataWriter{DetectorIndex};
        
        DataPMU = {};
        DataChannel = {};
        % PMUs are specified - add their names to the list
        NumPMU = length(Parameters.PMU);
        if NumPMU == 1
            % By default, the contents of Parameters.PMU would not be
            % in a cell array because length is one. This makes it so the same
            % indexing can be used in the following for loop.
            Parameters.PMU = {Parameters.PMU};
        end
        
        for PMUparamIdx = 1:NumPMU
            % Channels are specified - use only specified channels
            NumChannel = length(Parameters.PMU{PMUparamIdx}.Channel);
            if NumChannel == 1
                % By default, the contents of Parameters.PMU{PMUidx}.Channel would not be
                % in a cell array because length is one. This makes it so the same
                % indexing can be used in the following for loop.
                Parameters.PMU{PMUparamIdx}.Channel = {Parameters.PMU{PMUparamIdx}.Channel};
            end

            for ChannelIdx = 1:NumChannel
                DataPMU = [DataPMU Parameters.PMU{PMUparamIdx}.Name];
                DataChannel = [DataChannel Parameters.PMU{PMUparamIdx}.Channel{ChannelIdx}.Name];
            end
        end
        
        temp = ExtractDWparameters(Parameters);
        DWparams(end+1).SavePath = temp.SavePath;
        DWparams(end).SeparatePMUs = temp.SeparatePMUs;
        DWparams(end).Mnemonic = temp.Mnemonic;
        DWparams(end).PMU = DataPMU;
        DWparams(end).Channel = DataChannel;
    end
end


%% Link RMS-energy signals to the data writer detectors so that you know where to look for them

ToRetrieve = struct('PMU',{},'Channel',{},'Band',{},'SavePath',{},'SeparatePMUs',{},'Mnemonic',{});
for FiltIdx = 1:length(FilterParams)
    % Check if the custom PMU for this RMS-energy filter appears in any of
    % the data writers
    RMSpmu = FilterParams(FiltIdx).CustPMU;
    
    for DWidx = 1:length(DWparams)
        PMUmatch = strcmp(RMSpmu,DWparams(DWidx).PMU);
        
        if sum(PMUmatch) > 0
            for RMSsig = FilterParams(FiltIdx).CustSignals
                SigMatch = strcmp(RMSsig,DWparams(DWidx).Channel);
                
                if sum(SigMatch) > 0
                    temp = struct('PMU',RMSpmu,'Channel',RMSsig,...
                        'Band',FilterParams(FiltIdx).Band,...
                        'SavePath',DWparams(DWidx).SavePath,...
                        'SeparatePMUs',DWparams(DWidx).SeparatePMUs,...
                        'Mnemonic',DWparams(DWidx).Mnemonic);
                    ToRetrieve(end+1) = temp;
                end
            end
        end
    end
end


%%
end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractDWparameters(Parameters)

% Path to save output files
if isfield(Parameters,'SavePath')
    SavePath = Parameters.SavePath;
    if exist(SavePath,'dir') == 0
        error(['Save path ' SavePath ' does not exist.']);
    end
else
    error(['Save path ' SavePath ' does not exist.']);
end

% Flag indicating whether each PMU should be stored in its own folder
if isfield(Parameters,'SeparatePMUs')
    if strcmp('TRUE',Parameters.SeparatePMUs)
        SeparatePMUs = true;
    else
        SeparatePMUs = false;
    end
else
    SeparatePMUs = false;
end

if SeparatePMUs
    % PMUs are to be placed in separate folders. The PMU name is used as
    % the mnemonic for file naming
    Mnemonic = '';
else
    % PMUs are all going in one folder. If the user specified a mnemonic,
    % use it. Otherwise, use the generic 'ExportedData'
    if isfield(Parameters,'Mnemonic')
        Mnemonic = Parameters.Mnemonic;
    else
        Mnemonic = 'ExportedData';
    end
end

ExtractedParameters = struct('SavePath',SavePath,...
    'SeparatePMUs',SeparatePMUs,'Mnemonic',Mnemonic);
end