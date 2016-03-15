function pdatList=pdatFileTimeScan(startTime,endTime,archiveFolder)

% Initialize pdatList
pdatList={};

% Get subfolder of start and end times
startDayFolder=datestr(startTime,'yymmdd');
startYearFolder=datestr(startTime,'yyyy');
endDayFolder=datestr(endTime,'yymmdd');
endYearFolder=datestr(endTime,'yyyy');

startFolder=fullfile(archiveFolder,startYearFolder,startDayFolder);
endFolder=fullfile(archiveFolder,endYearFolder,endDayFolder);

% If start date = end date, get files from same folder
if startTime(1:3)==endTime(1:3)
    tempPDATlist=getPDATfiles({startFolder});
    pdatTimes=zeros(size(tempPDATlist,1),6);
    for ind=1:size(tempPDATlist,1)
        [~,fileName,~] = fileparts(tempPDATlist{ind,1});
        nameInd=strfind(fileName,'_');
        pdatTimes(ind,1)=str2double(fileName(nameInd(1)+1:nameInd(1)+4));
        pdatTimes(ind,2)=str2double(fileName(nameInd(1)+5:nameInd(1)+6));
        pdatTimes(ind,3)=str2double(fileName(nameInd(1)+7:nameInd(1)+8));
        pdatTimes(ind,4)=str2double(fileName(nameInd(2)+1:nameInd(2)+2));
        pdatTimes(ind,5)=str2double(fileName(nameInd(2)+3:nameInd(2)+4));
        pdatTimes(ind,6)=str2double(fileName(nameInd(2)+5:nameInd(2)+6));
        
    end
    pdatTimes=datetime(pdatTimes);
    fileLoc=datetime(startTime)<=pdatTimes & datetime(endTime)>=pdatTimes;
    pdatList=tempPDATlist(fileLoc);
else
    % Search for folders between (not including) start and end times
    yearArr=startTime(1):1:endTime(1);
    folderList={};
    folderListDir={};
    for ind=1:size(yearArr,2)
        yearFolder=fullfile(archiveFolder,num2str(yearArr(ind)));
        d=dir(yearFolder);
        isub = [d(:).isdir];
        tempFolds ={d(isub).name}';
        tempFolds(ismember(tempFolds,{'.','..'})) = [];
        folderList=[folderList; tempFolds];
        folderListDir=[folderListDir; fullfile(yearFolder,tempFolds)];
    end
    folderList=str2double(folderList);
    x=str2double(startDayFolder);
    y=str2double(endDayFolder);
    folderLoc=find(x<folderList & y>folderList);
    pdatFolderList=folderListDir(folderLoc);
    pdatMiddleList=getPDATfiles(pdatFolderList);
    
    % Search for files on start date
    tempPDATlist=getPDATfiles({startFolder});
    pdatTimes=zeros(size(tempPDATlist,1),6);
    for ind=1:size(tempPDATlist,1)
        [~,fileName,~] = fileparts(tempPDATlist{ind,1});
        nameInd=strfind(fileName,'_');
        pdatTimes(ind,1)=str2double(fileName(nameInd(1)+1:nameInd(1)+4));
        pdatTimes(ind,2)=str2double(fileName(nameInd(1)+5:nameInd(1)+6));
        pdatTimes(ind,3)=str2double(fileName(nameInd(1)+7:nameInd(1)+8));
        pdatTimes(ind,4)=str2double(fileName(nameInd(2)+1:nameInd(2)+2));
        pdatTimes(ind,5)=str2double(fileName(nameInd(2)+3:nameInd(2)+4));
        pdatTimes(ind,6)=str2double(fileName(nameInd(2)+5:nameInd(2)+6));
        
    end
    pdatTimes=datetime(pdatTimes);
    fileLoc=datetime(startTime)<=pdatTimes;
    pdatStartList=tempPDATlist(fileLoc);
    
    % Search for files on end date
    tempPDATlist=getPDATfiles({endFolder});
    pdatTimes=zeros(size(tempPDATlist,1),6);
    for ind=1:size(tempPDATlist,1)
        [~,fileName,~] = fileparts(tempPDATlist{ind,1});
        nameInd=strfind(fileName,'_');
        pdatTimes(ind,1)=str2double(fileName(nameInd(1)+1:nameInd(1)+4));
        pdatTimes(ind,2)=str2double(fileName(nameInd(1)+5:nameInd(1)+6));
        pdatTimes(ind,3)=str2double(fileName(nameInd(1)+7:nameInd(1)+8));
        pdatTimes(ind,4)=str2double(fileName(nameInd(2)+1:nameInd(2)+2));
        pdatTimes(ind,5)=str2double(fileName(nameInd(2)+3:nameInd(2)+4));
        pdatTimes(ind,6)=str2double(fileName(nameInd(2)+5:nameInd(2)+6));
        
    end
    pdatTimes=datetime(pdatTimes);
    fileLoc=datetime(endTime)>=pdatTimes;
    pdatEndList=tempPDATlist(fileLoc);
    
    pdatList=[pdatStartList; pdatMiddleList; pdatEndList];

end