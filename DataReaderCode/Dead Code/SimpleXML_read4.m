%prepare workspace
% close all;
clear all;
% clc;

%XML file
XMLFile='ConfigXML.xml';
% Parse XML file to MATLAB structure
DataXML = fun_xmlread_comments(XMLFile);

%XML file
XMLFile='ProcessXML.xml';
% Parse XML file to MATLAB structure
ProcessXML = fun_xmlread_comments(XMLFile);

% DQ and customization are done in stages. Each stage is composed of a DQ
% step and a customization step.
if isfield(DataXML.Configuration,'Stages')
    NumStages = length(DataXML.Configuration.Stages);
    if NumStages == 1
        % By default, the contents of DataXML.Configuration.Stages would not be
        % in a cell array because length is one. This makes it so the same
        % indexing can be used in the following for loop.
        DataXML.Configuration.Stages = {DataXML.Configuration.Stages};
    end
else
    NumStages = 0;
end

DateTimeStart = DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
FileDirectory = DataXML.Configuration.ReaderProperties.FileDirectory;
FileMnemonic = DataXML.Configuration.ReaderProperties.Mnemonic;
FilePath = [FileDirectory '\' datestr(DateTimeStart(1:19),'yyyy') '\' datestr(DateTimeStart(1:19),'yymmdd') '\' FileMnemonic];
FileDate = datestr(DateTimeStart(1:19),'_yyyymmdd_HHMMSS');
FileName = [FilePath FileDate '.pdat'];

% This takes in the full path to the file and the XML structure and
% produces a Matlab structure in the common format. 
% All PMUs retained
[PMU, ~, Num_Flags] = createPdatStruct(FileName,DataXML);

PMU = DQandCustomization(PMU,DataXML,NumStages,Num_Flags);

PMU = GetOutputSignals(PMU,DataXML);

%% Detection

% Processing (need to code)

% Limit PMUs and channels to those desired
PMU = GetOutputSignals(PMU,ProcessXML);