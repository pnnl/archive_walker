%% get the file time in Matlab datenum for an input pdat file
%
%

function timeNum = getPdatFileTime(inFile)
k1 = strfind(inFile,'_');
k2 = strfind(inFile,'.');

dayStr = inFile(k1(end-1)+1:k1(end)-1);
timeStr = inFile(k1(end)+1:k2-1);


year = str2num(dayStr(1:4));
month = str2num(dayStr(5:6));
day = str2num(dayStr(7:8));

hour = str2num(timeStr(1:2));
minute = str2num(timeStr(3:4));
second = str2num(timeStr(5:6));


timeNum = datenum([year month day hour minute second]);



