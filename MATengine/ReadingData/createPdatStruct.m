% function [PMU, tPMU, Num_Flags] =  createPdatStruct(pdatFile,DataXML)
% This function creates PMU structure list using pdat reader output
%
% Inputs:
	% pdatFile: Filepath to the PMU data file including file name
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
        % DataXML.Flag_Bit: Consists of flag bits used by different data
        % quality check filters
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
%   
%Created by 
%Modified on May 5, 2016
%Modified on June 6, 2016 by Urmila Agrawal (urmila.agrawal@pnnl.gov):
%Function now calculates and returns total number of flag bits needed for different data quality  check filter operation
%
% updated on June 24, 2016 by Tao Fu
%   add one flag for missing file in Num_Flags
%
% updted on 7/5, 2016 by Tao Fu
%   added one flag for non-value data points in the input file, which was
%   only seen in JSIS-CSV input files
%
% Updated on July 11, 2016 by Tao Fu
%   modified to handle the case that there are more than 1 digital signal in a PMM
%   digital signals are names as dig1, dig2, ...
%
% updated July 12, 2016 by Tao Fu
%   deleted counting the maximum number of flags that will be needed
%   (Flag_Bit), which is implemented in BAWS_main() now.
%


function [PMU, tPMU, fs] =  createPdatStruct(pdatFile,Num_Flags,FileLength)

[config, data]=pdatRead(pdatFile);
fs = config.dataRate;

%%
nPMU = config.numPMUs;  %number of PMUs
pmuNames = config.pmuNames; % PMU names

% get time of 
t = (data.SOC+data.fracSec/config.timeBase)/86400 + datenum(1970,1,1); 

% N = length(pdatFile);
% t = datenum(str2num(pdatFile(N-19:N-16)),str2num(pdatFile(N-15:N-14)),str2num(pdatFile(N-13:N-12)),...
%     str2num(pdatFile(N-10:N-9)),str2num(pdatFile(N-8:N-7)),str2num(pdatFile(N-6:N-5)));
% t = t + (0:1/60:(60-1/60))/3600/24;

% Perform checks on the timestamps.
dt = diff(t);
if (max(dt) > min(dt)*1.5) || (sum(isnan(dt)) > 0)
    % There are either missing samples or the timestamps are bad.
    
    % If FileLength hasn't been calculated yet (this is the first file) 
    % then there's nothing that can be done so check first.
    if ~isempty(FileLength)
        if length(t)/config.dataRate == FileLength
            % All the samples are present, so the timestamps must be
            % corrupt. 
            % This assumes that config.dataRate is correct, but if it's not
            % there's no way to fix the timestamps anyway.
            warning(['Timestamps in ' pdatFile ' appear corrupt. Attempting to correct.']);
            T0 = getPdatFileTime(pdatFile);
            t = T0 + (0:1/config.dataRate:FileLength)/(60*60*24);
            t = t(1:end-1)';
        else
            % Assuming config.dataRate is correct, some samples must be
            % missing. This should be handled by the data quality filter
            % for missing samples, rather than here (assume that the
            % timestamps are accurate for the samples that are present).
            warning([pdatFile ' appears to have missing samples. Problems will ensue if the data quality filter for missing data is not implemented.']);
        end
    else
        warning(['The file ' pdatFile ' has missing samples or corrupt timestamps. Continued problems are likely because a file has not yet been successfully loaded.']);
    end
end

tPMU = t;
t_str = cellstr(datestr(t,'yyyy-mm-dd HH:MM:SS.FFF'));
nData = length(tPMU); % number of data in each channel


%%
% initalize an empty PMU structure
singlePMU = struct('File_Name',[],'PMU_Name',[], 'Time_Zone',[], 'Signal_Name',[], 'Signal_Type',[], 'Signal_Unit',[], 'Signal_Time', [], 'Stat', [], 'Data', [], 'Flag', []);
PMU = repmat(singlePMU, 1,nPMU);

for i = 1:nPMU
   % for each PMU
   
   % read in all fixed fields for the 1st time
   currPMUName = pmuNames{i};
   PMU(i).PMU_Name = currPMUName;   % PMU name
   PMU(i).Time_Zone = '-08:00';         % time zone; for now this is just the PST time

   % get variable names
   currPMUConfig = config.pmu{i};
   [signalNames,nSignals] = getVarNames(currPMUConfig); % nSignals: 1x3 vector, number of signals of phasor, analog,  and digit

   PMU(i).Signal_Name = signalNames;

   % Signal_Type and Signal_Unit are empty cells for now
   % we will update them after the signal selection
   PMU(i).Signal_Type = cell(1,length(signalNames));
   PMU(i).Signal_Unit = cell(1,length(signalNames));          
   
   PMU(i).Signal_Time.Time_String = t_str;
   PMU(i).Signal_Time.Signal_datenum = t;
   
   PMU(i).File_Name = pdatFile;   % file name

   % get PMU data
   currPMUData = data.pmu{i};   

   % stat
   PMU(i).Stat = currPMUData.stat;
   
   % get phasor data
   phsrData = getPhsrData(currPMUData,nData);
   if(size(phsrData,2) ~= nSignals.phsr)
       error('Mismatch between number of phasor channels and channel names.');
   end
   
   % get analog data
   anlgData = getAnlgData(currPMUData,nData);
   if(size(anlgData,2) ~= nSignals.anlg)
       error('Mismatch between number of analog channels and channel names.');
   end
   
   % get digit data
   digitData = getDigitData(currPMUData,nData);
   if(size(digitData,2) ~= nSignals.dig)
       error('Mismatch between number of digital channels and channel names.');
   end
   
   % set digital signal type and unit
   for m = 1:nSignals.dig
       idx = nSignals.phsr+nSignals.anlg+m;     % index for the digital signal column
       PMU(i).Signal_Type{idx} = 'D';
       PMU(i).Signal_Unit{idx} = 'D';
   end
   
   % get frequency and rocof
   freqData = currPMUData.frq;
   rocofData = currPMUData.rocof;
   
   try
       dataVal = [phsrData,anlgData,digitData,freqData,rocofData];
       PMU(i).Data = dataVal;
   catch
       disp([pdatFile,':data reading error in PMU:',num2str(i)]);
       disp('Possible different number of data frames');
       
   end
   [m,n] = size(dataVal);   
   Flag = false(m,n,Num_Flags);
   PMU(i).Flag = Flag;
   
end

% set PMU Structure
PMU = SetNameAndUnit_PDAT(PMU);


end

%% get variable names for a PMU
function [signalNames,nSignals] = getVarNames(currPMUConfig)

% PMU name
currPMUName = currPMUConfig.name;
% read phasor names
if(isfield(currPMUConfig, 'phsr'))
    phsr = currPMUConfig.phsr;
    for i = 1:length(phsr)
        currPhsrName = phsr{i}.name;
        %phsrNames{i} = currPhsrName;
        phsrNames{2*i-1} = [currPMUName,'.',currPhsrName,'.MAG'];
        phsrNames{2*i} = [currPMUName,'.',currPhsrName,'.ANG'];
    end
else
    phsrNames = {};
end

% read analog variable names
if(isfield(currPMUConfig,'anlg'))
    anlg = currPMUConfig.anlg;
    for i = 1:length(anlg)
        anlgNames{i} = [currPMUName,'.',anlg{i}.name];
    end
else
    anlgNames = {};
end

% read digit variable names
if(isfield(currPMUConfig,'dig'))
    dig = currPMUConfig.dig;
    
    % give name to each digital signal
    for i = 1:length(dig)
       digNames{i} = [currPMUName,'.dig',num2str(i)]; 
    end
    
else
    digNames = {};
end

% add names for freq and rocof
frqName = [currPMUName,'.','frq'];
rocofName = [currPMUName,'.','rocof'];

signalNames = [phsrNames, anlgNames, digNames, frqName, rocofName];
nSignals.phsr = length(phsrNames);
nSignals.anlg = length(anlgNames);
nSignals.dig = length(digNames);

end

%% funcitons to get data
% get phasor data
function phsrData = getPhsrData(currPMUData,nData)
if isfield(currPMUData,'phsr')
    phsrDataList = currPMUData.phsr;
    phsrData = zeros(nData,2*length(phsrDataList));
    for i = 1:length(phsrDataList)
        phsrData(:,2*i-1) = phsrDataList{i}.mag;
        phsrData(:,2*i) = phsrDataList{i}.ang;
    end
else
    phsrData = [];
end

end

% get analog data
function anlgData = getAnlgData(currPMUData,nData)
if isfield(currPMUData,'anlg')
   anlgDataList = currPMUData.anlg;
    anlgData = zeros(nData,length(anlgDataList));
    for i = 1:length(anlgDataList)
        anlgData(:,i) = anlgDataList{i}.val;
    end
else
   anlgData = [];
end

end


% get digit data
function digitData = getDigitData(currPMUData,nData)
if(isfield(currPMUData,'dig'))
    digitDataList = currPMUData.dig;
    digitData = zeros(nData,length(digitDataList));
    for i = 1:length(digitDataList)
        digitData(:,i) = digitDataList{i}.val;
    end
else
    digitData = [];
end


end