%% convert JSIS-CSV format files to a common matlab structure
% Inputs:
	% inFile: input CSV file name
    % DataXML: structure containing configuration from the input XML file
        % DataXML.Configuration.Stages: struct array containing
        % information on filter and customization operation in each stage
        % (dimension is 1 by number of stages)
            % DataXML.Configuration.Stages{i}.Filter: array of struct consisting of
            % information on filter and customization operation in each stage
            % (dimension is 1 by number of stages)
                % DataXML.Configuration.Stages{i}.Filter{j}.Parameters.FlagBit:
                % a string specifying bit to be flagged for j^th filter
                % operation in i^th stage
%
% Outputs:
    % PMU: struct array of dimension 1 by Number of PMUs
        % PMU(i).File_Name: a string specifying file name containing i^th PMU data        
        % PMU(i).PMU_Name: a string specifying name of i^th PMU
        % PMU(i).Time_zone:  a string specifying time-zone from where data are recorded
        % PMU(i).Signal_Time: a string specifying time stamp of i^th PMU data
        % PMU(i).Signal_Name: a cell array of strings specifying name of Signals in i^th PMU
        % PMU(i).Signal_Type: a cell array of strings specifying type of Signals in i^th PMU
        % PMU(i).Signal_Unit: a cell array of strings specifying unit of Signals in i^th PMU
        % PMU(i).Stat: Numerical array specifying status of i^th PMU
        % PMU(i).Data: Numerical matrix containing data measured by the i^th PMU
        % PMU(i).Flag: 3-dimensional matrix providing information on the
        % i^th PMU data that are flagged by different filters
    % tPMU: numerical array representing time stamp of PMU measurements
    % Num_Flags: Number of flag bits
    

% created on June 30, 2016 by Tao Fu
% 
% Need to do: need a SetNameAndUnit() function for JSIS_CSV input files 
 
% updated on July 5, 2016 by Tao Fu
%  1. added one flag bit for non-value data points
%  2. modified the code to speed up data reading
%  3. fixed PMU name
%
% updated July 12, 2016 by Tao Fu
%   deleted counting the maximum number of flags that will be needed
%   (Flag_Bit), which is implemented in BAWS_main() now.
%
% modified on 27th July,2016 by Urmila Agrawal
%   Changed PMU.Signal_Time.Signal_datenum from column to row vector
%   changed PMU.Flag matrix from double to logical matrix


function [PMU,tPMU,Num_Flags] = JSIS_CSV_2_Mat(inFile,DataXML)

% add 4 extra flags
% the first additional bit is flagged when the customized signal uses flagged input signal
% the second additional input is if the customized signal was not created becasue of some error in user input.
% the third additional flag is used when data points are not values
% the forth additional flag is used when the file is missing
Num_Flags = max(DataXML.Flag_Bit)+4; 


%% read in headers
%t1 = now;
fid = fopen(inFile);
signalNameStr = fgetl(fid);
signalTypeStr = fgetl(fid);
signalUnitStr = fgetl(fid);
signalDespStr = fgetl(fid);


%% read in data
data = readtable(inFile,'HeaderLines',3,'TreatAsEmpty',{'.','NA','N/A','#VALUE!'} );

% The CSVs we have repeat a value from one file to the next. If the number 
% of samples is odd, assume this is the case and drop the first sample.
if mod(size(data,1),2) == 1
    data = data(2:end,:);
end

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

tPMU = timeNum;

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

k = strfind(pmuName,'_');
if(~isempty(k))
    currPMUName = pmuName(1:k(1)-1);
end
PMU.PMU_Name = currPMUName;   % PMU name
PMU.Time_Zone = '-08:00';         % time zone; for now this is just the PST time

% signal time
PMU.Signal_Time.Time_String = cellstr(timeStr);
PMU.Signal_Time.Signal_datenum = timeNum(:)';

% variable names
PMU.Signal_Name = signalNames;

% Signal_Type and Signal_Unit
PMU.Signal_Type = signalTypes;
PMU.Signal_Unit = signalUnits;



% flag
[m,n] = size(signalData);
Flag = false(m,n,Num_Flags);
PMU.Flag = Flag;

% PMU data and Stat
PMU.Data = signalData;
PMU.Stat = zeros(m,1);

% update flag if there are data points that were set to strings or empty
[row, col] = find(isnan(signalData));
if(~isempty(row))
    % has data points that were set to strings or empty
    flag = false(m,n);
    idx = sub2ind(size(flag), row, col);  % convert row and col to matlab linear indices
    flag(idx) = true;
    PMU.Flag(:,:,end-1) = flag;      
end



% Unnecessary here, signal types and units are already set. Left over from
% original PDAT code. (Jim Follum 7/1/16)
% PMU = SetNameAndUnit_PDAT(PMU); 

%t2 = now;
%dt = t2-t1;
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













