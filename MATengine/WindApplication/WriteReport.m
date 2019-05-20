clear;

ConfigFile = 'WindApp.XML';

StartTime = '2017-06-21 23:25:00';
EndTime = '2017-06-21 23:35:00';

% SortType = 'EventSize';
% OR
% SortType = 'NewestFirst';
% OR
SortType = 'NewestLast';

ReportPath = 'C:\Users\Tony\Documents\MATLAB\ArchiveWalker_WindApp\WindAppStorage\Reports\Report3.docx';

GenerateWindAppReport(ConfigFile,StartTime,EndTime,SortType,ReportPath);