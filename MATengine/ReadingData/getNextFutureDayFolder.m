%%
% get the file folder that contains the nearest future file later than the focusFileTime
%
% Input:
%   focusFileFolder: file directory for the current focus file. created in function getPdatFileFoder()
%   fileType: file type in the folder, 1, pdat; 2, csv
%
% Output:
%   nextFileFolder: the file directory that contains the nearest future file later than the focusFileTime
%
% Created on June 28, 2016
%

%%
function nextFileFolder = getNextFutureDayFolder(focusFileFolder,fileType)

nextFileFolder = '';

k = strfind(focusFileFolder,'\');
generalFolder = focusFileFolder(1:k(end-2));
currYearFolder = focusFileFolder(1:k(end-1));
currYearStr = focusFileFolder(k(end-2)+1:k(end-1)-1); % string of current year
currYMDStr = focusFileFolder(k(end-1)+1:k(end)-1);  % string of current day

currYear = str2num(currYearStr);
currYMD = str2num(currYMDStr);

foundFolder = 0; % a flag to indicate if the next folder is found
%% check current year folder
if(exist(currYearFolder,'dir'))
    YMDfolders = dir(currYearFolder);
    YMDfolders = {YMDfolders.name};
    
    % compare currYMD with folder names in the current year folder
    if(length(YMDfolders) > 2) % there are always '.' and '..' subfolders
        YMDfolders = YMDfolders(3:end);
        YMDfolders_num = cellfun(@str2num,YMDfolders);
        k = find(YMDfolders_num > currYMD);
        if(~isempty(k))
            % has folders after the current day
            i = k(1);
            % look for the 1st non-empty folder
            while(~foundFolder && i <= length(YMDfolders))
                checkFolder = [currYearFolder,'\',YMDfolders{i},'\'];
                if(fileType == 1)
                    checkFiles = dir([checkFolder,'\*.pdat']);
                elseif(fileType == 2)
                    checkFiles = dir([checkFolder,'\*.csv']);
                end
                if(~isempty(checkFiles))
                    % found future files
                    foundFolder = 1;
                    nextFileFolder = checkFolder;
                end
                i = i+1;
            end            
        end
    else
        % this folder is empty
        % move to the next year
    end
end

if(~foundFolder)
    % current year folder doesn't exist or couldn't find the next future file in the current year folder
    % check folders in later years
    yearFolders = dir(generalFolder);
    yearFolders = {yearFolders.name};
    if(length(yearFolders) > 2) % there are always '.' and '..' subfolders
        yearFolders = yearFolders(3:end);
        %yearFolders_num = cellfun(@str2num,yearFolders); %sometimes, there are hidden files in the folder, which makes this command not working
        yearFolders_num = zeros(1,length(yearFolders));
        for m = 1:length(yearFolders)
            currFolder = yearFolders{m};
            strFolder = [generalFolder,currFolder];
            if(isdir(strFolder))
                currFolderNum = str2num(currFolder);
                if(~isempty(currFolderNum))
                    yearFolders_num(m) = currFolderNum;
                end
            end
        end
        k = find(yearFolders_num > currYear);
        if(~isempty(k))
            % has folders for later years
            i = k(1);   % starting from the 1st future year
            while(~foundFolder && i <= length(yearFolders))
                checkYearFolder = [generalFolder,'\',yearFolders{i}];
                YMDfolders =  dir(checkYearFolder);
                YMDfolders = {YMDfolders.name};                
                if(length(YMDfolders) > 2)
                    % has some YMD sub-folders
                    YMDfolders = YMDfolders(3:end);
                    j = 1;
                    % look for the 1st non-empty folder
                    while(~foundFolder && j <= length(YMDfolders))
                        checkFolder = [checkYearFolder,'\',YMDfolders{j},'\'];
                        if(fileType == 1)
                            checkFiles = dir([checkFolder,'\*.pdat']);
                        elseif(fileType == 2)
                            checkFiles = dir([checkFolder,'\*.csv']);
                        end
                        if(~isempty(checkFiles))
                            % found future files
                            foundFolder = 1;
                            nextFileFolder = checkFolder;
                        end
                        j = j+1;
                    end                    
                    i = i+1;
                end
            end
        else
            % no folder for later years 
            % do nothing
        end
    else
        % no year folders
        % do nothing
    end
end
    


end


