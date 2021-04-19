function [gam,dt,PMUlist,ChannelList,BandList] = SetRMSenergyThresh(ConfigFile,Pfa,TrimPct,StartTime,EndTime,WindowSizeMin,WindowOverlapMin)

%#function rectwin
%#function bartlett
%#function hann
%#function hamming
%#function blackman

ToRetrieve = LocateRMSsignals(ConfigFile);

PMUlist = {ToRetrieve.PMU};
ChannelList = {ToRetrieve.Channel};
BandList = {ToRetrieve.Band};

% Analysis length (seconds)
Nsec = WindowSizeMin*60;
% Number of seconds before rerunning analyses
Nrerun = WindowOverlapMin*60;

TS = datestr(StartTime, 'yyyy-mm-dd HH:MM:SS.FFF');

gam = cell(1,length(ToRetrieve));
for RMSsig = 1:length(ToRetrieve)
    SavePath = ToRetrieve(RMSsig).SavePath;
    Band = ToRetrieve(RMSsig).Band;
    Chan = ToRetrieve(RMSsig).Channel;
    
    switch Band
        case 'Band 1'
            fBand = [0 0.15];
        case 'Band 2'
            fBand = [0.15 1];
        case 'Band 3'
            fBand = [1 5];
        case 'Band 4'
            fBand = [5 30];
    end
    
    if ToRetrieve(RMSsig).SeparatePMUs
        ThisSubFolder = ToRetrieve(RMSsig).PMU;
        ThisMn = ToRetrieve(RMSsig).PMU;
    else
        ThisSubFolder = '';
        ThisMn = ToRetrieve(RMSsig).Mnemonic;
    end
    % Replace spaces with underscores to prevent errors
    ThisMn = strrep(ThisMn,' ','_');
    ThisSubFolder = strrep(ThisSubFolder,' ','_');
    
    TS = erase(TS,{'-',' ',':'});    % Puts the timestamp in the form yyyymmddHHMMSS.FFF
    
    FullSavePath = fullfile(SavePath,ThisSubFolder,TS(1:4),TS(3:8));
    
    FileList = dir(FullSavePath);
    FileList = {FileList.name};
    FileList = FileList(contains(FileList,ThisMn));
    
    % Read the first file to get the file length
    PMU = JSIS_CSV_2_Mat(fullfile(FullSavePath,FileList{1}),0);
    fs = round(1/mean((diff(PMU.Signal_Time.Signal_datenum)*24*60*60)));
    FileSizeSec = size(PMU.Data,1)/fs;
    
    % Number of files to retrieve before rerunning analyses
    StepFiles = Nrerun/FileSizeSec;
    
    ChanIdx = find(strcmp(PMU.Signal_Name, Chan));
    
    [~, bLP] = DesignRMSfilters(Band,fs);
    nLP = length(bLP);
    
    % Frequency vector for resampling spectrums to match RMS calculations
    fResamp = (0:nLP-1)/nLP*fs;
    fResamp = fResamp(near(fResamp,fBand(1)-1e-9):near(fResamp,fBand(2)+1e-9));
    fResamp(fResamp > fBand(2)) = [];
    Nlam = length(fResamp); % Number of parameters in the HE (number of spectrum bins)
    
    RMS = [];
    FileCounter = 0;
    gam{RMSsig} = [];
    dt = NaT(0);
    for t = StartTime:seconds(FileSizeSec):EndTime
        TS = datestr(t, 'yyyy-mm-dd HH:MM:SS.FFF');
        TS = erase(TS,{'-',' ',':'});    % Puts the timestamp in the form yyyymmddHHMMSS.FFF
    
        FullSavePath = fullfile(SavePath,ThisSubFolder,TS(1:4),TS(3:8),[ThisMn '_' TS(1:8) '_' TS(9:14) '.csv']);
        
        PMU = JSIS_CSV_2_Mat(FullSavePath,0);
        RMS = [RMS; PMU.Data(:,ChanIdx)];
        
        FileCounter = FileCounter + 1;
    
        if FileCounter < StepFiles
            continue
        elseif length(RMS) < Nsec*fs
            continue
        end
        
        
        FileCounter = 0;
        
        RMS = RMS(end-Nsec*fs+1:end);
        
        % Using the observed sum of squared values
        % Note that the 0.5 multiplier has to do with matching the test
        % statistic when calculated using a periodogram
        % Also note that SumSq = N*RMS^2
        Tprime = 0.5*nLP*RMS.^2;
        
        % Remove any NaN values
        Tprime(isnan(Tprime)) = [];
        
        % Trim most extreme values
        zn = length(Tprime);
        k = zn*(TrimPct/100)/2;
        Tprime = sort(Tprime);
        Tprime = Tprime(k+1:end-k);
        %
        uSS = mean(Tprime);
        s2SS = var(Tprime);
        
        % Choose param vectors for the HE based on the means and variances
        try
            lam = SelectAlternativeHEparams(uSS,s2SS,Nlam);
            gamPrime = invhypoexp(1-Pfa,lam);
            gam{RMSsig} = [gam{RMSsig}; sqrt(1/(nLP/2)*gamPrime)];
        catch
            gam{RMSsig} = [gam{RMSsig}; NaN];
        end
        
        dt = [dt; t];
    end
end