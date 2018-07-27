function [exMean, eSep, pSep, Cat] = WindAppCatAndRank(EventXMLdir,Hist)

% Get list of all files in EventXMLdir
filelist = dir(EventXMLdir);
dirIdx = [filelist.isdir];
filelist = {filelist.name}.';
filelist = filelist(~dirIdx);

% Translate file names to times
fileTimes = GetTimesFromNames(filelist);
% Remove times and corresponding files that were returned NaN - primarily
% for the Current and Current_Bkup files.
filelist = filelist(~isnan(fileTimes));
fileTimes = fileTimes(~isnan(fileTimes));
% Sort in descending order to put newest times first
[~,SortIdx] = sort(fileTimes,'descend');
% Retain only the most recent Hist files
filelist = filelist(SortIdx(1:min([length(SortIdx) Hist])));

% Add the current event list back on, if it exists
if exist([EventXMLdir '\EventList_Current.XML'],'file')>0
    filelist = [filelist; {'EventList_Current.XML'}];
end


eFactor = [];
ex = [];
Pow = [];
for fileIdx = 1:length(filelist)
    EventXML = fun_xmlread_comments([EventXMLdir '\' filelist{fileIdx}]);
    EventXML = EventListXML2MAT(EventXML);
    
    if ~isfield(EventXML,'WindApp')
        continue
    end
    
    for eIdx = 1:length(EventXML.WindApp)
        eFactor = [eFactor EventXML.WindApp(eIdx).ExtremaFactor];
        ex = [ex  EventXML.WindApp(eIdx).Extrema];
        
        PowTemp = [];
        for PowIdx = 1:length(EventXML.WindApp(eIdx).WindPower)
            split = strsplit(EventXML.WindApp(eIdx).WindPower{PowIdx},',');
            PowTemp = [PowTemp abs(str2double(split{3}))];
        end
        Pow = [Pow max(PowTemp)];
    end
end

% Cluster the events based on their extreme values. Conceptually, this
% breaks the events into frequency, 230 kV, 500 kV
if length(ex) > 1
    eva = evalclusters(ex.','kmeans','Silhouette','KList',2:min([5 length(ex)]));
    if max(eva.CriterionValues) > 0.9
        K = eva.OptimalK;
        Kidx = eva.OptimalY;
    else
        K = 1;
        Kidx = ones(size(eva.OptimalY));
    end
else
    K = 1;
    Kidx = 1;
end


exMean = zeros(1,K);
eSep = zeros(1,K);
pSep = zeros(1,K);
Cat = cell(1,K);
% For each category (V230, V500, Freq)
for k = 1:K
    % Mean of the extreme value for this variable type
    exMean(k) = mean(ex(Kidx==k));
    
    if ((59<exMean(k)) && (exMean(k)<61))
        Cat{k} = 'Frequency';
    elseif ((210<exMean(k)) && (exMean(k)<280))
        Cat{k} = '230 kV';
    elseif ((475<exMean(k)) && (exMean(k)<575))
        Cat{k} = '500 kV';
    else
        Cat{k} = 'Other';
    end
    
    % Cluster this variable into two groups - high and low
    eFactorK = eFactor(Kidx==k).';
    if length(eFactorK) > 1
        eFactorKidx = kmeans(eFactorK,2);
        k1idx = eFactorKidx==1;
        k2idx = eFactorKidx==2;
        % Depending on which cluster ended up being the high one, find a point
        % between the clusters to separate them.
        if mean(eFactorK(k1idx)) < mean(eFactorK(k2idx))
            eSep(k) = mean([max(eFactorK(k1idx)) min(eFactorK(k2idx))]);
        else
            eSep(k) = mean([max(eFactorK(k2idx)) min(eFactorK(k1idx))]);
        end
    else
        eSep(k) = eFactorK/2;
    end
    
    % Select the separator for power based on 
    PowK = Pow(Kidx==k);
    % If large events pull the mean up, use the mean. If small events pull
    % the mean low, use the median. 
    pSep(k) = max([median(PowK) mean(PowK)]);
end


end



function T = GetTimesFromNames(FileNames)
    T = zeros(1,length(FileNames));
    for idx = 1:length(FileNames)
        FileParts = strsplit(FileNames{idx},{'_','.'});
        if isnan(str2double(FileParts{2}))
            T(idx) = NaN;
        else
            T(idx) = datenum(FileParts{2},'yymmdd');
        end
    end
end