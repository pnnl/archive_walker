

function [DummyDetectionResults, DummyAdditionalOutput] = DataWriterDetector(PMUstruct,Parameters,~)

% Not actually needed, but must be returned to match the output of other
% detectors.
DummyDetectionResults = struct([]);
DummyAdditionalOutput = struct([]);
    
%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString, TimeDT] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the data writer could not be used.');
    return
end

%% Get detector parameters

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. 
% Additional inputs, such as the length of the input data or the sampling 
% rate, can be added as necessary. 
ExtractedParameters = ExtractParameters(Parameters);

% Store the parameters in variables for easier access
SavePath = ExtractedParameters.SavePath;
SeparatePMUs = ExtractedParameters.SeparatePMUs;
Mnemonic = ExtractedParameters.Mnemonic;

% Get the indices for channels of data to be included in each folder. Each
% folder corresponds to a PMU if SeparatePMUs==1. If SeparatePMUs==0, then
% everything just goes in one folder.
if SeparatePMUs
    % PMUs are to be separated into different folders
    uPMU = unique(DataPMU);
    FolderIdx = cell(1,length(uPMU));
    
    for uIdx = 1:length(uPMU)
        FolderIdx{uIdx} = find(strcmp(uPMU{uIdx}, DataPMU));
    end
    
    % PMU names are used as the mnemonics for file naming and for the
    % subfolder names
    Mnemonic = uPMU;
    SubFolder = uPMU;
else
    % All PMUs going into one folder
    FolderIdx = {1:length(DataChannel)};
    Mnemonic = {Mnemonic};
    SubFolder = {''};
end



TS = erase(TimeString{1},{'-',' ',':'});    % Puts the timestamp in the form yyyymmddHHMMSS.FFF
for idx = 1:length(FolderIdx)
    ThisIdx = FolderIdx{idx};
    ThisMn = Mnemonic{idx};
    ThisSubFolder = SubFolder{idx};
    
    H1 = ['Time' DataChannel(ThisIdx)];
    H2 = ['Type' DataType(ThisIdx)];
    H3 = ['second' DataUnit(ThisIdx)];
    H4 = ['Time' strcat(DataPMU(ThisIdx), '_', DataChannel(ThisIdx))];
    H = {H1,H2,H3,H4};
    
    FullSavePath = fullfile(SavePath,ThisSubFolder,TS(1:4),TS(3:8));
    SaveFile = fullfile(FullSavePath,[ThisMn '_' TS(1:8) '_' TS(9:14) '.csv']);
    
    if exist(FullSavePath,'dir') == 0
        mkdir(FullSavePath);
    end
    
    fid = fopen(SaveFile,'w'); 
    for hidx = 1:4
        commaHeader = [H{hidx};repmat({','},1,numel(H{hidx}))]; %insert commaas
        commaHeader = commaHeader(:)';
        commaHeader = commaHeader(1:end-1);
        textHeader = cell2mat(commaHeader); %cHeader in text with commas
        fprintf(fid,'%s\n',textHeader);
    end
    fclose(fid);

    dlmwrite(SaveFile,[t' Data(:,ThisIdx)],'-append','precision',6);
end

end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters)

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