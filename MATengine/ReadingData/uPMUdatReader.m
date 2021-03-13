% Function to read the DAT files from a
% PowerSide microPMU (formerly PSL)

function [PMU, tPMU, fs] = uPMUdatReader(datFile,Num_Flags,FileLength)

% Create result storage. Empty to start, but if FileLength is available
% from loading a previous file then the correct length will be allocated
% once the frame rate is determined.
tPMU=zeros(0,1);
BasePhasorData=zeros(0,12);
ExtendedPhasorData=zeros(0,5);
StatusData=zeros(0,1);

%Open the file
fHandle=fopen(datFile,'rb');

%Make sure it isn't an empty file
eofStatus=feof(fHandle);

%Reset tracking variable
SecondsRead=0;

%Loop until the end of file
%Note that each variable is hard-coded to 120
%Not sure what happens when it is a different rate - need example to revisit
while (eofStatus~=1)
    %Read the sample interval
    SampleIntMS=fread(fHandle,1,'float');
    
    %Make sure we didn't reach EOF
    if (feof(fHandle)==1)
        break;
    end
    
    % Calculate frame rate
    fs = round(1000/SampleIntMS);
    
    % Preallocate if possible (FileLength must be known from reading in the
    % first file). Otherwise, create empty result storage.
    if ~isempty(FileLength) && isempty(tPMU)
        PreAllocSize = FileLength*fs;
        
        tPMU=zeros(PreAllocSize,1);
        BasePhasorData=zeros(PreAllocSize,12);
        ExtendedPhasorData=zeros(PreAllocSize,5);
        StatusData=zeros(PreAllocSize,1);
    end
    

    %Counter - keep track of how many seconds we've read
    SecondsRead=SecondsRead+1;
    
    %Read in the timestamp information - initial timestamp
    Timestamp=fread(fHandle,6,'int32');

    %Read the status variables
    StatusInfoPre=fread(fHandle,120,'int32');
    
    %Convert it to a character array for "ease of read"
    StatusInfo=fliplr(dec2bin(StatusInfoPre,11));
    
    %% Paired phasor data
    l1data=fread(fHandle,240,'float');  %l1 phase and magnitude
    l2data=fread(fHandle,240,'float');  %l2 phase and magnitude
    l3data=fread(fHandle,240,'float');  %l3 phase and magnitude
    c1data=fread(fHandle,240,'float');  %c1 phase and magnitude
    c2data=fread(fHandle,240,'float');  %c2 phase and magnitude
    c3data=fread(fHandle,240,'float');  %c3 phase and magnitude

    %Put them into a "more common" form
    l1data=reshape(l1data,2,[]).';
    l2data=reshape(l2data,2,[]).';
    l3data=reshape(l3data,2,[]).';
    c1data=reshape(c1data,2,[]).';
    c2data=reshape(c2data,2,[]).';
    c3data=reshape(c3data,2,[]).';
    
    %% PLL debug info
    fread(fHandle,4,'uint32');

    %% GPS information
    fread(fHandle,7,'float');

    %% Check if expanded
    if (strcmp(StatusInfo(1,6),'1') == 1)   %Assumes it is consistent through a packet
        DoExpanded=true;
    else
        DoExpanded=false;
    end

    %See if expanded
    if (DoExpanded)
        ExtendedPMUData=1;  %Flag for later
        watts_total=fread(fHandle,120,'float');
        var_total=fread(fHandle,120,'float');
        va_total=fread(fHandle,120,'float');
        dpf_total=fread(fHandle,120,'float');
        freq_l1_e_one_sec=fread(fHandle,120,'float');
        freq_l1_e_c37=fread(fHandle,120,'float');
    else
        ExtendedPMUData=0;  %Flag for later
    end
   
    %% Update status
    eofStatus=feof(fHandle);
    
    %% Populate the data arrays
    % Compute indices
    StartStoreIdx=(SecondsRead-1)*fs+1;
    EndStoreIdx=SecondsRead*fs;
    
    %Convert the base time into MATLAB time
    BaseTime=datenum(Timestamp.');
    OffsetVals=(0:(fs-1)).'*SampleIntMS/1000/86400;  %Value in MS, plus convert to "MATLAB-day"
    
    %Store the time
    tPMU(StartStoreIdx:EndStoreIdx,1)=BaseTime+OffsetVals;
    
    %Store the base phasor data
    BasePhasorData(StartStoreIdx:EndStoreIdx,1:12)=[l1data l2data l3data c1data c2data c3data];
    
    %See if the extended is available
    if (DoExpanded)
        ExtendedPhasorData(StartStoreIdx:EndStoreIdx,1:5)=[watts_total var_total va_total dpf_total freq_l1_e_c37];
    else
        ExtendedPhasorData(StartStoreIdx:EndStoreIdx,1:5)=NaN;
    end
    
    %Store status, if wanted
    StatusData(StartStoreIdx:EndStoreIdx,1)=StatusInfoPre;
end

%Close the file
fclose(fHandle);

% Perform checks on the timestamps.
dt = diff(tPMU);
if (max(dt) > min(dt)*1.5) || (sum(isnan(dt)) > 0)
    % There are either missing samples or the timestamps are bad.
    
    % If FileLength hasn't been calculated yet (this is the first file)
    % then there's nothing that can be done so check first.
    if ~isempty(FileLength)
        if length(tPMU)/fs == FileLength
            % All the samples are present, so the timestamps must be
            % corrupt.
            % This assumes that fs is correct, but if it's not
            % there's no way to fix the timestamps anyway.
            warning(['Timestamps in ' datFile ' appear corrupt. Attempting to correct.']);
            T0 = getPdatFileTime(datFile);
            tPMU = T0 + (0:1/fs:FileLength)/(60*60*24);
            tPMU = tPMU(1:end-1)';
        else
            % Assuming fs is correct, some samples must be
            % missing. This should be handled by the data quality filter
            % for missing samples, rather than here (assume that the
            % timestamps are accurate for the samples that are present).
            warning([datFile ' appears to have missing samples. Problems will ensue if the data quality filter for missing data is not implemented.']);
        end
    else
        warning(['The file ' datFile ' has missing samples or corrupt timestamps. Continued problems are likely because a file has not yet been successfully loaded.']);
    end
end

[~,PMUname] = fileparts(datFile);
PMUname = strsplit(PMUname,'_');
PMUname = PMUname{end-2};

PMU.PMU_Name = PMUname;

if (ExtendedPMUData==0)
    PMU.Data = BasePhasorData;
    PMU.Signal_Type = {'VAA','VAM','VBA','VBM','VCA','VCM','IAA','IAM','IBA','IBM','ICA','ICM'};
    PMU.Signal_Unit = {'DEG','V','DEG','V','DEG','V','DEG','A','DEG','A','DEG','A'};
    PMU.Signal_Name = strcat([PMUname '_'],PMU.Signal_Type);
else
    PMU.Data = [BasePhasorData ExtendedPhasorData];
    PMU.Signal_Type = {'VAA','VAM','VBA','VBM','VCA','VCM','IAA','IAM','IBA','IBM','ICA','ICM','P','Q','S','RCF','F'};
    PMU.Signal_Unit = {'DEG','V','DEG','V','DEG','V','DEG','A','DEG','A','DEG','A','W','VAR','VA','Hz/sec','Hz'};
    PMU.Signal_Name = strcat([PMUname '_'],PMU.Signal_Type);
end

PMU.Signal_Time.Time_String = cellstr(datestr(tPMU,'yyyy-mm-dd HH:MM:SS.FFF'));
PMU.Signal_Time.Signal_datenum = tPMU;
PMU.Signal_Time.datetime = datetime(tPMU,'ConvertFrom','datenum','Format','MM/dd/yy HH:mm:ss.SSSSSS');

PMU.File_Name = datFile;
PMU.Stat = StatusData;
PMU.Flag = false(size(PMU.Data,1),size(PMU.Data,2),Num_Flags);
PMU.Time_Zone = '00:00'; % Assumption