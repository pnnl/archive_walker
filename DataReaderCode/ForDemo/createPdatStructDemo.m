%% test function to convert pdat reader output into a Matlab structure
%
% last update: 5/5/2016
%

% create PMU structure list
function [PMU, tPMU] =  createPdatStructDemo(pdatFile,DataXML,flog)

[config, data]=pdatRead3(pdatFile,DataXML);

nPMU = config.numPMUs;  %number of PMUs
pmuNames = config.pmuNames; % PMU names

% get time 
t = (data.SOC+data.fracSec/config.timeBase)/86400 + datenum(1970,1,1); 

N = length(pdatFile);
t = datenum(str2num(pdatFile(N-19:N-16)),str2num(pdatFile(N-15:N-14)),str2num(pdatFile(N-13:N-12)),...
    str2num(pdatFile(N-10:N-9)),str2num(pdatFile(N-8:N-7)),str2num(pdatFile(N-6:N-5)));
t = t + (0:1/60:(60-1/60))/3600/24;

tPMU = t;
t_str = datestr(t,'yyyy-mm-dd HH:MM:SS.FFF');
for i = 1:nPMU
   % for each PMU
   PMU(i).File_Name = pdatFile;   % file name
%    fprintf(flog,'%s\n',pdatFile);
   currPMUName = pmuNames{i};   
   PMU(i).PMU_Name = currPMUName;   % PMU name
   PMU(i).Time_Zone = '-08:00';         % time zone; for now this is just the PST time 
   
   %
   PMU(i).Signal_Time.Time_String = cellstr(t_str);  
   PMU(i).Signal_Time.Signal_datenum = t;
   
   % get variable names
   currPMUConfig = config.pmu{i};
   [signalNames,nSignals] = getVarNames(currPMUConfig); % nSignals: 1x3 vector, number of signals of phasor, analog,  and digit
   
   
   PMU(i).Signal_Name = signalNames;
   
   % Signal_Type and Signal_Unit are empty cells for now
   % we will update them after the signal selection
   PMU(i).Signal_Type = cell(1,length(signalNames));
   PMU(i).Signal_Unit = cell(1,length(signalNames));
      
   % get PMU data
   currPMUData = data.pmu{i};   

   % stat
   PMU(i).Stat = currPMUData.stat;
   
   % get phasor data
   phsrData = getPhsrData(currPMUData);
%    if(size(phsrData,2) ~= nSignals.phsr)
%        fprintf(flog,'difference between number of phsr and number of phsr data\n');
%    end
   
   % get analog data
   anlgData = getAnlgData(currPMUData);
%    if(size(anlgData,2) ~= nSignals.anlg)
%        fprintf(flog,'difference between number of analog and number of analog data\n');
%    end
   
   % get digit data
   digitData = getDigitData(currPMUData);
   if(size(digitData,2) ~= nSignals.dig)
%        fprintf(flog,'difference between number of digit and number of digit data\n');
       % this happens
       if(nSignals.dig > size(digitData,2))
           % less number of data than signal names
           tmpData = zeros(size(phsrData,1),nSignals.dig);
           tmpData(:,1:size(digitData,2)) = digitData;
           digitData = tmpData;
       end
   end
   
   % set digital signal type and unit
   idx = nSignals.phsr+nSignals.anlg+1;     % index for the digital signal column
   PMU(i).Signal_Type{idx} = 'D';
   PMU(i).Signal_Unit{idx} = 'D';
      
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
   
   % flag 
   [m,n] = size(dataVal);
   Flag = zeros(m,n);
   PMU(i).Flag = Flag;
   
end

if strcmp(pdatFile(end-3:end),'pdat')
    % PDAT file
    PMU = SetNameAndUnit_PDAT(PMU);
elseif strcmp(pdatFile(end-3:end),'.csv')
    % CSV file
%     PMU = SetNameAndUnit_CSV(PMU);
end


end

% get variable names for a PMU
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
    %for i = 1:length(dig)
    %    digNames{i} = dig{i}.name;
    %end
%     if(~isempty(dig))
%         digNames = dig{1}.name; % need to check this
%         for i = 1:length(digNames)
%             digNames{i} = [currPMUName,'.',digNames{i}];
%         end
%         digNames = digNames';  
% 
%     else
%         digNames = {};
    
%    end
    % used one name for digital signal
    digNames = {[currPMUName,'.dig']};
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
function phsrData = getPhsrData(currPMUData)
phsrData = [];
phsrDataList = currPMUData.phsr;
for i = 1:length(phsrDataList)
    phsrData = [phsrData,phsrDataList{i}.mag];
    phsrData = [phsrData,phsrDataList{i}.ang];
end

end

% get analog data
function anlgData = getAnlgData(currPMUData)
anlgData = [];
anlgDataList = currPMUData.anlg;
for i = 1:length(anlgDataList)
    anlgData = [anlgData,anlgDataList{i}.val];
end


end


% get digit data
function digitData = getDigitData(currPMUData)
digitData = [];
if(isfield(currPMUData,'dig'))
    digitDataList = currPMUData.dig;
    for i = 1:length(digitDataList)
        digitData = [digitData,digitDataList{i}.val];
    end
end


end



