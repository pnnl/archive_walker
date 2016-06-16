% function fileList = getArchivedFiles(FileDirectory, DateTimeStart, DateTimeEnd)
% This function gets Pdat files that need to be processed in the archive mode
%
% Inputs:
	% FileDirectory: Filepath to the PMU data file 
    % DateTimeStart: a string specifying start date and time of PMU data to
    % be analyzed
    % DateTimeEnd:  a string specifying end date and time of PMU data to be
    % analyzed
%
% Outputs:
    % fileList: Name of the files consisting of data to be analyzed
%   
%Created by 

function fileList = getArchivedFiles(FileDirectory, DateTimeStart, DateTimeEnd)
 
% get the day of the start time
startDay = floor(datenum(DateTimeStart)); 
endDay = floor(datenum(DateTimeEnd));
startHMS = str2num(datestr(DateTimeStart,'HHMMSS'));
endHMS = str2num(datestr(DateTimeEnd,'HHMMSS'));

fileList = {};

if(startDay == endDay)
    currDay = startDay;
    yearStr = datestr(currDay,'yyyy');
    monthDayStr = datestr(currDay,'yymmdd');
    currFolder = [FileDirectory,'\',yearStr,'\',monthDayStr];
    
    if(exist(currFolder,'dir'))
        allNames = dir([currFolder,'\*.pdat']);
        if(~isempty(allNames))
            allNames = {allNames.name};
            for i = 1:length(allNames)
                currFile = allNames{i};
                currFile = [currFolder,'\',currFile];
                currT = str2num(currFile(end-10:end-5));
                if(currT >= startHMS && currT <= endHMS)
                    fileList = [fileList,{currFile}];
                    %n = length(fileList);
                    %fileList{n+1} = currFile;
                end
                
            end
        end
    end
else
    % start day and end day are two different days
    for currDay = startDay:endDay
        yearStr = datestr(currDay,'yyyy');
        monthDayStr = datestr(currDay,'yymmdd');
        currFolder = [FileDirectory,'\',yearStr,'\',monthDayStr];
        
        if(exist(currFolder,'dir'))
            allNames = dir([currFolder,'\*.pdat']);
            if(~isempty(allNames))
                allNames = {allNames.name};
                if(currDay > startDay)
                    if(currDay ~= endDay)
                        % output all files for now
                        % may need come back to check file times
                        for i = 1:length(allNames)
                            allNames{i} = [currFolder,'\',allNames{i}];
                        end
                        fileList = [fileList,allNames];
                    else
                        % the last day
                        for i = 1:length(allNames)
                            currFile = allNames{i};
                            currFile = [currFolder,'\',currFile];
                            currT = str2num(currFile(end-10:end-5));
                            if(currT <= endHMS)
                                fileList = [fileList,{currFile}];
                            end                            
                        end                        
                    end
                else
                    % currDay is the start day
                    for i = 1:length(allNames)
                        currFile = allNames{i};
                        currFile = [currFolder,'\',currFile];
                        currT = str2num(currFile(end-10:end-5));
                        if(currT >= startHMS)
                            fileList = [fileList,{currFile}];
                        end
                        
                    end
                end
            else
                for i = 1:length(allNames)
                    currFile = allNames{i};
                    currFile = [currFolder,'\',currFile];
                    currT = str2num(currFile(end-10:end-5));
                    if(currT >= startHMS)
                        fileList = [fileList,{currFile}];
                    end                    
                end
            end            
        end
    end           
end
end


