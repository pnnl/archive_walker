function pdatFileList=getPDATfiles(folderList)

pdatFileList={};

for ind=1:size(folderList,1)
    folderName=folderList{ind,1};
    k = dir([folderName '\*.pdat']);
    tempFiles = {k.name}';
    tempFiles=fullfile(folderName,tempFiles);
    pdatFileList=[pdatFileList;tempFiles];
end