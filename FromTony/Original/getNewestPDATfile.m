function pdatFile=getNewestPDATfile(archiveFolder)

pdatFile=[];

d=dir(archiveFolder);
isub=[d(:).isdir];
tempFolds ={d(isub).name}';
tempFolds(ismember(tempFolds,{'.','..'})) = [];
yearFolders=fullfile(archiveFolder,tempFolds);

for ind=size(yearFolders,1):-1:1
    d=dir(yearFolders{ind,1});
    isub=[d(:).isdir];
    tempFolds ={d(isub).name}';
    tempFolds(ismember(tempFolds,{'.','..'})) = [];
    dayFolders=fullfile(yearFolders{ind,1},tempFolds);
    if ~isempty(dayFolders)
        for ind2=size(dayFolders,1):-1:1
            pdatList=getPDATfiles({dayFolders{ind2,1}});
            if ~isempty(pdatList)
                pdatFile=pdatList{end,1};
                return
            end
        end
    end
end
