function UpdatePreviousDayModeEst(ModeRem, ResultPath, TimeString ,ModeEstimateCalcIdx)
% TimeStr = datestr(TimeString-1,'yymmdd');
ModeRem = ModeRem(:);
DaysBack = 1;
FreqMpath = abs(imag(ModeRem))/2/pi;
DRMpath =  -real(ModeRem)./abs(ModeRem);
FreqMpath(isnan(DRMpath)) = NaN;
while ~isempty(FreqMpath)
    % The updated path crosses a day boundary.
    % Load MMoutput from previous days, replace
    % the mode estimates, and then save them
    % again until MpathRemainder is empty.
    
    FileName = [ResultPath '\' datestr(TimeString-DaysBack,'yymmdd') '.csv'];
    if exist(FileName,'file') > 0
        % Load the previous days value by accessing previous day file
        ModeEstTable = readtable(FileName);     
        ModeEstLength = size(ModeEstTable,1)-3;
        H1 = ModeEstTable.Properties.VariableNames;
        DRChanIdx = find(strcmp(H1,'DampingRatio'), 1 ) + ModeEstimateCalcIdx-1;
        FreqChanIdx = find(strcmp(H1,'Frequency'), 1 ) + ModeEstimateCalcIdx-1;
        T = table2array(ModeEstTable(4:end,:));
        if length(FreqMpath) <= ModeEstLength
            % The constructed path is completely contained within
            % the day, so replace the earlier estimates.
            T(end-length(DRMpath)+1:end,DRChanIdx) = cellstr(num2str(DRMpath));
            T(end-length(FreqMpath)+1:end,FreqChanIdx) = cellstr(num2str(FreqMpath));
            DRMpath = [];
            FreqMpath = [];
        else
            % The constructed path is longer than the matrix of
            % results because the estimates cross a day transition.
            T(:,DRChanIdx) = cellstr(num2str(DRMpath(end-ModeEstLength+1:end)));
            T(:,FreqChanIdx) = cellstr(num2str(FreqMpath(end-ModeEstLength+1:end)));
            DRMpath = DRMpath(1:end-ModeEstLength,1);
            FreqMpath= FreqMpath(1:end-ModeEstLength,1);
        end
        H = {H1, ModeEstTable{1,:},ModeEstTable{2,:},ModeEstTable{3,:}};
        fid = fopen(FileName,'w');
        for idx = 1:4
            commaHeader = [H{idx};repmat({','},1,numel(H{idx}))]; %insert commaas
            commaHeader = commaHeader(:)';
            commaHeader = commaHeader(1:end-1);
            textHeader = cell2mat(commaHeader); %cHeader in text with commas
            fprintf(fid,'%s\n',textHeader);
        end
        fclose(fid);
        dlmwrite(FileName,str2double(T),'-append');
        % Increment the number of days back that the code needs to go to update
        % the mode estimates
        DaysBack = DaysBack + 1;
    else
        warning([FileName ' does not exist, cannot perform retroactive continuity on earlier day mode estimates.'])
    end
end
