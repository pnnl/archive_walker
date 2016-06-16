%% convert JSIS-CSV format files to a common matlab structure
%

function PMU = JSIS_CSV_2_Mat(inFile)
%% read in headers
t1 = now;
fid = fopen(inFile);
signalNameStr = fgetl(fid);
signalTypeStr = fgetl(fid);
signalUnitStr = fgetl(fid);
signalDespStr = fgetl(fid);


%% read in data
data = readtable(inFile,'HeaderLines',3);

%% identify the format of time and get time information
T1 = data{1,1};
if(isnumeric(T1))
    timeFormat = 0;
else
    timeFormat = 1;
end


if(timeFormat == 1)
    % time are in strings
    timeStr = data{:,1};
    timeNum = datenum(timeStr, 'mm/dd/yy HH:MM:SS.FFF'); % need to make sure this format
elseif(timeFormat == 0)
    % time are in seconds
    try
        T0 = getPdatFileTime(inFile);   % inFile is already in PDAT format
        T = data{:,1};  % T is in seconds
        T = T/24/3600;
        timeNum = T+T0;
        timeStr = datestr(timeNum, 'mm/dd/yy HH:MM:SS.FFF');
    catch
        disp('Incorrect JSIS_CSV file name format');              
    end        
end

%% get signal Name, signal type, signal unit, and data
k = strfind(signalNameStr,',');
n = length(k);
signalNames = cell(1,n);
for i = 1:n-1
    signalNames{i} = signalNameStr(k(i)+1:k(i+1)-1);
end
signalNames{n} = signalNameStr(k(end)+1:end);

k = strfind(signalTypeStr,',');
n = length(k);
signalTypes = cell(1,n);
for i = 1:n-1
    signalTypes{i} = signalTypeStr(k(i)+1:k(i+1)-1);
end
signalTypes{n} = signalTypeStr(k(end)+1:end);
% match signal types to types in comman data structure
[signalTypes, typeFlag] = matchSignalTypes(signalTypes);

k = strfind(signalUnitStr,',');
n = length(k);
signalUnits = cell(1,n);
for i = 1:n-1
    signalUnits{i} = signalUnitStr(k(i)+1:k(i+1)-1);
end
signalUnits{n} = signalUnitStr(k(end)+1:end);
% set units for IFD, EFD, and SPD to O
k = find(typeFlag == 0);
if(~isempty(k))
    for i = 1:length(k)
        signalUnits{k(i)} = 'O';
    end
end

try
    signalData = data{:,2:end};
catch
    % some values are not in numbers, need to convert them into numbers
    data = data(:,2:end);
    [m,n] = size(data);
    signalData = zeros(m,n);
    for i = 1:n
        if(isnumeric(data{1,i}))
            signalData(:,i) = data{:,i};
        else
            % has some elements that are not 0
            currCol = data{:,i};
            currColNum = zeros(length(currCol),1);
            for j = 1:length(currCol)
                currData = currCol{j};
                currColNum(j) = str2double(currData);                
            end

    % tried to use cellfun, doesn't really work
%             currColNum = cellfun(@str2double,currCol,'UniformOutput',0);
%             idx = find(cellfun(@isnan,currColNum));
%             for j = 1:length(idx)
%                 currIdx = idx(j);
%                 currColNum{currIdx} = 'NaN';
%             end
            signalData(:,i) = currColNum;
            
        end
    end
end


%% for now, we will put all information into a common data structure for a JSIS-csv file
PMU.File_Name = inFile;   % file name
%    fprintf(flog,'%s\n',pdatFile);

% used file name as the PMU name
k = strfind(inFile,'\');
if(isempty(k))
    pmuName = inFile(1:end-4);
else
    idx = k(end);
    pmuName = inFile(idx+1:end-4);
end
currPMUName = pmuName;
PMU.PMU_Name = currPMUName;   % PMU name
PMU.Time_Zone = '-08:00';         % time zone; for now this is just the PST time

% signal time
PMU.Signal_Time.Time_String = cellstr(timeStr);
PMU.Signal_Time.Signal_datenum = timeNum;

% variable names
PMU.Signal_Name = signalNames;

% Signal_Type and Signal_Unit
PMU.Signal_Type = signalTypes;
PMU.Signal_Unit = signalUnits;

% PMU data
PMU.data = signalData;

% flag
[m,n] = size(signalData);
Flag = zeros(m,n);
PMU.Flag = Flag;

t2 = now;
dt = t2-t1;
% 

% outFile = [inFile(1:end-4),'.mat'];
% save(outFile,'PMU');

end


%% function to match JSIS-CSV names to common data structure names
function [signalTypes,flag] = matchSignalTypes(inTypes)

flag = zeros(1,length(inTypes));
% VPM to VMP
k = find(strcmp(inTypes,'VPM'));
if(~isempty(k))
    for i = 1:length(k)
        inTypes{k(i)} = 'VMP';
        flag(k(i)) = 1;
    end    
end

% VPA to VAP
k = find(strcmp(inTypes,'VPA'));
if(~isempty(k))
    for i = 1:length(k)
        inTypes{k(i)} = 'VAP';
        flag(k(i)) = 1;
    end
end

% IPM to IMP
k = find(strcmp(inTypes,'IPM'));
if(~isempty(k))
    for i = 1:length(k)
        inTypes{k(i)} = 'IMP';
        flag(k(i)) = 1;
    end
end

% IPA to IAP
k = find(strcmp(inTypes,'IPA'));
if(~isempty(k))
    for i = 1:length(k)
        inTypes{k(i)} = 'IAP';
        flag(k(i)) = 1;
    end
end

% F, no change
k = find(strcmp(inTypes,'F'));
if(~isempty(k))
    for i = 1:length(k)
        % no need to change the signal type
        flag(k(i)) = 1;
    end
end

% P, no change
k = find(strcmp(inTypes,'P'));
if(~isempty(k))
    for i = 1:length(k)
        % no need to change the signal type
        flag(k(i)) = 1;
    end
end

% Q, no change
k = find(strcmp(inTypes,'Q'));
if(~isempty(k))
    for i = 1:length(k)
        % no need to change the signal type
        flag(k(i)) = 1;
    end
end

% set IFD, EFD, and SPD to OTHER
k = find(flag == 0);
if(~isempty(k))
    for i = 1:length(k)
        inTypes{k(i)} = 'OTHER';
    end
end

signalTypes = inTypes;

end













