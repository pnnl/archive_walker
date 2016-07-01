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
NumStages = length(DataXML.Configuration.Stages);
if NumStages == 1
    % By default, the contents of DataXML.Configuration.Stages would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    DataXML.Configuration.Stages = {DataXML.Configuration.Stages};
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

% PMU(1).Data(21:30,1) =  200000*ones(10,1) + 100*rand(10,1);

PMU = DQandCustomization(PMU,DataXML,NumStages,Num_Flags);

% PMU(1).Data(2:199,1) = NaN;
% PMU(25).Data(2:50,1) = NaN;
% PMU(1).Data(202:250,1) = NaN;

% PMU = GetOutputSignals(PMU,DataXML);

% %% Detection
% % 
% DateTimeStart = DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
% FileDirectory = DataXML.Configuration.ReaderProperties.FileDirectory;
% FileMnemonic = DataXML.Configuration.ReaderProperties.Mnemonic;
% FilePath = [FileDirectory '\' datestr(DateTimeStart(1:19),'yyyy') '\' datestr(DateTimeStart(1:19),'yymmdd') '\' FileMnemonic];
% FileDate = datestr(DateTimeStart(1:19),'_yyyymmdd_HHMMSS');
% FileName = [FilePath FileDate '.pdat'];
DataProcessorStruct{1} = PMU;%createPdatStruct(FileName,DataXML);
% 
DateTimeStart = '2015-10-17 12:31:00 GMT';%DataXML.Configuration.ReaderProperties.Mode.Params.DateTimeStart;
FileDirectory = DataXML.Configuration.ReaderProperties.FileDirectory;
FileMnemonic = DataXML.Configuration.ReaderProperties.Mnemonic;
FilePath = [FileDirectory '\' datestr(DateTimeStart(1:19),'yyyy') '\' datestr(DateTimeStart(1:19),'yymmdd') '\' FileMnemonic];
FileDate = datestr(DateTimeStart(1:19),'_yyyymmdd_HHMMSS');
FileName = [FilePath FileDate '.pdat'];

[PMU, ~, Num_Flags] = createPdatStruct(FileName,DataXML);

% PMU(25).Data(1:30,1) =  200000*ones(30,1) + 100*rand(30,1);

PMU = DQandCustomization(PMU,DataXML,NumStages,Num_Flags);

DataProcessorStruct{2} = PMU;%createPdatStruct(FileName,DataXML);

NumStages = length(ProcessXML.Configuration.Processing.Stages);
if NumStages == 1
    % By default, the contents of DataXML.Configuration.Stages would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    ProcessXML.Configuration.Processing.Stages = {ProcessXML.Configuration.Processing.Stages};
end

FlagBitInput = 1; %Flag bit for indicating flagged input data points
FlagBitInterpo = 2; %Flag bit for indicating interpolated data
NumFlags = 2; %Total number of flag bits to indicate flagged processed data
PMUStruct = ConcatenatePMU(DataProcessorStruct,FlagBitInput,NumFlags);

ProcessorOutputStruct = DataProcessor(PMUStruct, ProcessXML,NumStages,FlagBitInterpo);
% PMU = GetOutputSignals(ProcessorOutputStruct.PMU,DataXML);
% % % % PMU = GetOutputSignals(PMU,ProcessXML);