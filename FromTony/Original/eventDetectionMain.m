%**************************************************************************
%
% eventDetectionMain.m - initial version of .pdat archive walker
%
%**************************************************************************

clear all; close all; clc;

% Select between historical, live data, or both
hist=0;
hist_and_live=1;
live=0;

% Enter start and end times, if needed
startTime=[2014 10 1 0 0 0];
endTime=[2014 10 1 0 3 0];

% .pdat archive folder and log file for events
archiveFolder='\\connie-1\projects\disat\PDAT_data';
logFile='C:\Users\foll154\Documents\BPA Oscillation App\eventLog.txt';

% For historical search, get all .pdat files in time range from archive
if hist==1
    pdatFiles=pdatFileTimeScan(startTime,endTime,archiveFolder);
    maxWaitTime=5;
elseif hist_and_live==1
    lastPDATfile=getNewestPDATfile(archiveFolder);
    [~,fileName,~] = fileparts(lastPDATfile);
    nameInd=strfind(fileName,'_');
    endTime(1,1)=str2double(fileName(nameInd(1)+1:nameInd(1)+4));
    endTime(1,2)=str2double(fileName(nameInd(1)+5:nameInd(1)+6));
    endTime(1,3)=str2double(fileName(nameInd(1)+7:nameInd(1)+8));
    endTime(1,4)=str2double(fileName(nameInd(2)+1:nameInd(2)+2));
    endTime(1,5)=str2double(fileName(nameInd(2)+3:nameInd(2)+4));
    endTime(1,6)=str2double(fileName(nameInd(2)+5:nameInd(2)+6));
    pdatFiles=pdatFileTimeScan(startTime,endTime,archiveFolder);
    maxWaitTime=4000;
else
    pdatFiles={};
    pdatFile=getNewestPDATfile(archiveFolder);
    maxWaitTime=4000;
end

%**************************************************************************
% Initialize event detection algorithms and parameters
init=1;
% trig=frqDetectAlg(init,0);
init=0;
eventFound=0;
refTime=[1970 1 1 0 0 0];
%**************************************************************************

waitCount=0;

% Continuous while loop getting any new .pdat files
while waitCount<maxWaitTime  
    
    if ~isempty(pdatFiles)
        pdatFile=pdatFiles{1,1};
        
        %**************************************************************************
        % Code for frequency detection algorithm
%         [pmuNames, pmuIDs, timeArr, frqArr, rocofArr]= pdatReadFrq(pdatFile);
        
%         for ind=1:size(frqArr(:,1),1)
%             frqRow=frqArr(ind,:);
%             frqRow(1,frqRow==0)=nan;
%             trig=frqDetectAlg(init,frqRow);
%             
%             
%             if trig==1 && eventFound==0
%                 startSOC=timeArr(ind,1);
%                 startTime=datevec(addtodate(datenum(refTime),startSOC,'second'));
%                 disp(['Event Found! Start Time: ' datestr(startTime)]);
%                 eventFound=1;
%             end
%             
%             if eventFound==1 && trig==0 && median(frqRow(~isnan(frqRow)))>=59.95
%                 endSOC=timeArr(ind,1);
%                 endTime=datevec(addtodate(datenum(refTime),endSOC,'second'));
%                 disp(['End Time: ' datestr(endTime)]);
%                 logEntry=[datestr(startTime,'mm/dd/yyyy HH:MM:SS') ',' datestr(endTime,'mm/dd/yyyy HH:MM:SS')];
%                 fid = fopen(logFile,'at');
%                 fprintf(fid,['\n' logEntry '\n'],'%s');
%                 fclose(fid);
%                 eventFound=0;
%             end
%         end
                
        %Read pdatFile
        %Pass each sample to algorithm here
        
        %**************************************************************************
        
        % Shift pdatFiles in buffer
        if size(pdatFiles,1)>1
            pdatFiles=pdatFiles(2:end,1);
        else
            pdatFiles={};
        end
    end
    
    
    % If pdat buffer is empty, scan archive for more files
    if isempty(pdatFiles)
        waitCount=waitCount+1;
        pause(1);       %Pause for one second
        if hist_and_live==1 || live==1
            lastPDATfile=getNewestPDATfile(archiveFolder);
            if strcmpi(pdatFile,lastPDATfile)
                %disp('same')
            else
                %disp(lastPDATfile)
                [~,fileName,~] = fileparts(pdatFile);
                nameInd=strfind(fileName,'_');
                startTime(1,1)=str2double(fileName(nameInd(1)+1:nameInd(1)+4));
                startTime(1,2)=str2double(fileName(nameInd(1)+5:nameInd(1)+6));
                startTime(1,3)=str2double(fileName(nameInd(1)+7:nameInd(1)+8));
                startTime(1,4)=str2double(fileName(nameInd(2)+1:nameInd(2)+2));
                startTime(1,5)=str2double(fileName(nameInd(2)+3:nameInd(2)+4));
                startTime(1,6)=str2double(fileName(nameInd(2)+5:nameInd(2)+6));
                
                [~,fileName,~] = fileparts(lastPDATfile);
                nameInd=strfind(fileName,'_');
                endTime(1,1)=str2double(fileName(nameInd(1)+1:nameInd(1)+4));
                endTime(1,2)=str2double(fileName(nameInd(1)+5:nameInd(1)+6));
                endTime(1,3)=str2double(fileName(nameInd(1)+7:nameInd(1)+8));
                endTime(1,4)=str2double(fileName(nameInd(2)+1:nameInd(2)+2));
                endTime(1,5)=str2double(fileName(nameInd(2)+3:nameInd(2)+4));
                endTime(1,6)=str2double(fileName(nameInd(2)+5:nameInd(2)+6));
                pdatFiles=[pdatFiles; pdatFileTimeScan(startTime,endTime,archiveFolder)];
                pdatFiles=setdiff(pdatFiles,pdatFile);
                waitCount=0;
            end
        end
    end
end