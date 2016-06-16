%prepare workspace
% close all;
clear all;
% clc;

%XML file
XMLFile='ConfigXML.xml';

% Parse XML file to MATLAB structure
DataXML = fun_xmlread_comments(XMLFile);

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
% PMU = createPdatStruct(FileName);
% Only PMUs listed in DataXML are stored. 
[PMU, ~, Num_Flags] = createPdatStruct(FileName,DataXML);

%Changing data value for testing the data quality check filterPMU(1).Data(1:1800,1:2:15) =  20000*ones(1800,8);
PMU(1).Data(1:60,38) =  60*ones(60,1);

PMU = DQandCustomization(PMU,DataXML,NumStages,Num_Flags);

%Flag matrices for different PMUs and filter
A1 = PMU(1).Flag(:,:,1); %For PMU-1
A2 = PMU(1).Flag(:,:,2);
A3 = PMU(1).Flag(:,:,3);
A4 = PMU(1).Flag(:,:,4);
A5 = PMU(1).Flag(:,:,5);
A6 = PMU(1).Flag(:,:,6);
A7 = PMU(1).Flag(:,:,7);
A8 = PMU(1).Flag(:,:,8);
A9 = PMU(1).Flag(:,:,9);
A10 = PMU(1).Flag(:,:,10);
A11 = PMU(1).Flag(:,:,11);
A12 = PMU(1).Flag(:,:,12);
A13 = PMU(1).Flag(:,:,13);
A14 = PMU(1).Flag(:,:,14);
B1 = PMU(2).Flag(:,:,1); %For PMU-2
B2 = PMU(2).Flag(:,:,2);
B3 = PMU(2).Flag(:,:,3);
B4 = PMU(2).Flag(:,:,4);
B5 = PMU(2).Flag(:,:,5);
B6 = PMU(2).Flag(:,:,6);
B7 = PMU(2).Flag(:,:,7);
B8 = PMU(2).Flag(:,:,8);
B9 = PMU(2).Flag(:,:,9);
B10 = PMU(2).Flag(:,:,10);
B11 = PMU(2).Flag(:,:,11);
B12 = PMU(2).Flag(:,:,12);
B13 = PMU(2).Flag(:,:,13);
B14 = PMU(2).Flag(:,:,14);
C1 = PMU(3).Flag(:,:,1); %For PMU with customized signal
C2 = PMU(3).Flag(:,:,2);
C3 = PMU(3).Flag(:,:,3);
C4 = PMU(3).Flag(:,:,4);
C5 = PMU(3).Flag(:,:,5);
C6 = PMU(3).Flag(:,:,6);
C7 = PMU(3).Flag(:,:,7);
C8 = PMU(3).Flag(:,:,8);
C9 = PMU(3).Flag(:,:,9);
C10 = PMU(3).Flag(:,:,10);
C11 = PMU(3).Flag(:,:,11);
C12= PMU(3).Flag(:,:,12);
C13 = PMU(3).Flag(:,:,13);
C14 = PMU(3).Flag(:,:,14);




