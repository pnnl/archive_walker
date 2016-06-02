%% get the pdat file folder for a given time number
%


function [currFolder,currFile] = getPdatFileFolder(dataDir,FileMnemonic, T)

yearStr = datestr(T,'yyyy');
monthDayStr = datestr(T,'yymmdd');

currFolder = [dataDir,'\',yearStr,'\',monthDayStr,'\'];

dayStr = datestr(T,'yyyymmdd');
timeStr = datestr(T,'HHMMSS');
currFile = [currFolder,FileMnemonic,'_',dayStr,'_',timeStr,'.pdat'];



    





