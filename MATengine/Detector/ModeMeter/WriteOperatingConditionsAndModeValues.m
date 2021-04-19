% Created by Urmila Agrawal(urmila.agrawal@pnnl.gov) on 07/09/2018

function WriteOperatingConditionsAndModeValues(AdditionalOutput,ResultPath,DEFparams)
TimeString = datestr(AdditionalOutput(1).TimeString{end},'yymmdd');
if exist(ResultPath,'dir') == 0
    mkdir(ResultPath);
end
FileName = [ResultPath '\' TimeString '.csv'];
tt = [];
for tIdx = 1:length(AdditionalOutput(1).t)
    t = (datestr(AdditionalOutput(1).t(tIdx),'HH:MM:SS.FFF'));
    if ~isempty(t)
        tt  = [tt;(str2double(t(1:2))*60*60+ str2double(t(4:5))*60 + ceil(str2double(t(7:end))))/3600];
    else
        tt = 24;
    end
end
tt = round(tt,3);
if exist(FileName,'file') > 0
    Habc = readtable(FileName,'Format','auto');
    H1 = Habc.Properties.VariableNames;
    H2 = Habc{1,:};
    H3 = Habc{2,:};
    H4 = Habc{3,:};
else
    H1 = {'Time'};
    H2 = {''};
    H3 = {''};
    H4 = {'Hrs'};
    for SysCondIdx = 1:length(AdditionalOutput(1).OperatingNames)
        H1 = [H1, 'OperatingValue'];
        H2= [H2, AdditionalOutput(1).OperatingNames{SysCondIdx}];
        H3 = [H3, AdditionalOutput(1).OperatingType{SysCondIdx}];
        H4 = [H4, AdditionalOutput(1).OperatingUnits{SysCondIdx}];
    end
    for ModeIdx = 1:length(AdditionalOutput)
        for ModeEstIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
            H1 = [H1, 'DampingRatio'];
            H2 = [H2, AdditionalOutput(ModeIdx).ModeOfInterest{ModeEstIdx}];
            H3 = [H3, AdditionalOutput(ModeIdx).ChannelsName{ModeEstIdx}];
            H4 = [H4, AdditionalOutput(ModeIdx).MethodName{ModeEstIdx}];
        end
    end
    for ModeIdx = 1:length(AdditionalOutput)
        for ModeEstIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
            H1 = [H1, 'Frequency'];
            H2 = [H2, AdditionalOutput(ModeIdx).ModeOfInterest{ModeEstIdx}];
            H3 = [H3, AdditionalOutput(ModeIdx).ChannelsName{ModeEstIdx}];
            H4 = [H4, AdditionalOutput(ModeIdx).MethodName{ModeEstIdx}];
        end
    end
    if ~isempty(DEFparams)
        for ModeIdx = 1:length(AdditionalOutput)
            for ModeEstIdx = 1:length(AdditionalOutput(ModeIdx).ModeOfInterest)
                for PathIdx = 1:size(DEFparams.PathDescription,2)
                    H1 = [H1, ['DEF_' DEFparams.PathDescription{1,PathIdx} '_to_' DEFparams.PathDescription{2,PathIdx}]];
                    H2 = [H2, AdditionalOutput(ModeIdx).ModeOfInterest{ModeEstIdx}];
                    H3 = [H3, DEFparams.PathDescription{1,PathIdx}];
                    if isempty(DEFparams.PathDescription{2,PathIdx})
                        % To area was not specified
                        H4 = [H4, ' '];
                    else
                        % To area was specified
                        H4 = [H4, DEFparams.PathDescription{2,PathIdx}];
                    end
                end
            end
        end
    end
end
% end
if ~isempty(AdditionalOutput(1).OperatingNames)
    for SysCondIdx = 1:length(AdditionalOutput(1).OperatingNames)
        T1 = cell2mat({AdditionalOutput(1).OperatingValues});
    end
else
    T1 = [];
end
T2 = [];
for ModeIdx = 1:length(AdditionalOutput)
    T2 = [T2 cell2mat(AdditionalOutput(ModeIdx).ModeDRHistory)];
end
for ModeIdx = 1:length(AdditionalOutput)
    T2 = [T2 cell2mat(AdditionalOutput(ModeIdx).ModeFreqHistory)];
end
T3 = [];
if ~isempty(DEFparams)
    for ModeIdx = 1:length(AdditionalOutput)
        T3 = [T3 cell2mat(AdditionalOutput(ModeIdx).DEFhistory)];
    end
end
T = [tt T1 T2 T3];
H = {H1,H2,H3,H4};
fid = fopen(FileName,'w');
for idx = 1:4
    commaHeader = [H{idx};repmat({','},1,numel(H{idx}))]; %insert commaas
    commaHeader = commaHeader(:)';
    commaHeader = commaHeader(1:end-1);
    textHeader = cell2mat(commaHeader); %cHeader in text with commas
    fprintf(fid,'%s\n',textHeader);
end
fclose(fid);
dlmwrite(FileName,T,'-append');
