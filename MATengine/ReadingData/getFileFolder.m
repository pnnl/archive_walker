% function [currFolder,currFile] = getPdatFileFolder(dataDir,FileMnemonic, T)
% This function gets the pdat file folder for a given time number
%
% Inputs:
	% dataDir: Filepath to the directory consisting of PMU data files
    % FileMnemonic: a string specifying file mnemonic
    % T: Numerical array representing timestamp of PMU data
% 
% Outputs:
    % currFile: Name of file containing data for a given time number
    % currFolder: Folder containing file given by currFile 
%    
%Created by 

function [currFolder,currFile] = getFileFolder(dataDir,FileMnemonic, T, fileType)

yearStr = datestr(T,'yyyy');
monthDayStr = datestr(T,'yymmdd');

currFolder = [dataDir,'\',yearStr,'\',monthDayStr,'\'];

dayStr = datestr(T,'yyyymmdd');
timeStr = datestr(T,'HHMMSS');
currFile = [currFolder,FileMnemonic,'_',dayStr,'_',timeStr];


if(fileType == 1)
    currFile = [currFile '.pdat'];
elseif(fileType == 2)
    currFile = [currFile '.csv'];
elseif(fileType == 3)
    currFile = [currFile '.mat'];
elseif(fileType == 4)
    currFile = [currFile '.dat'];
end



    





