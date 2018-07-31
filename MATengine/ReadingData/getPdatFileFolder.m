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

function [currFolder,currFile] = getPdatFileFolder(dataDir,FileMnemonic, T)

yearStr = datestr(T,'yyyy');
monthDayStr = datestr(T,'yymmdd');

currFolder = [dataDir,'\',yearStr,'\',monthDayStr,'\'];

dayStr = datestr(T,'yyyymmdd');
timeStr = datestr(T,'HHMMSS');
currFile = [currFolder,FileMnemonic,'_',dayStr,'_',timeStr,'.pdat'];



    





