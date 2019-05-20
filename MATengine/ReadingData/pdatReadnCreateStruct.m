function [PMU, tPMU, fs] = pdatReadnCreateStruct(pdatFile,Num_Flags,FileLength)
m = memmapfile(pdatFile,'format','int8');
intData=m.data;

m = memmapfile(pdatFile,'format','uint8');
uintData=m.data;

fileVersion=uintData(1,1);
c37Version=uintData(2,1);
%dataPadded=uintData(3,1);
flags=uintData(4,1);
flagBin=dec2bin(flags,8);

dataPadded = str2double(flagBin(1,5));
incIniFile=str2double(flagBin(1,6));
incCfgFile=str2double(flagBin(1,7));
incHdrFile=str2double(flagBin(1,8));

offsetToData=hex2dec([dec2hex(uintData(5,1),2) dec2hex(uintData(6,1),2) dec2hex(uintData(7,1),2) dec2hex(uintData(8,1),2)]);
idx=9;

hdrPtrLength=hex2dec([dec2hex(uintData(idx,1),2) dec2hex(uintData(idx+1,1),2) dec2hex(uintData(idx+2,1),2) dec2hex(uintData(idx+3,1),2)]);
hdrPtr=uintData(idx+4:idx+4+hdrPtrLength-1,1);
idx=idx+4+hdrPtrLength;

hdrDataLength=hex2dec([dec2hex(uintData(idx,1),2) dec2hex(uintData(idx+1,1),2) dec2hex(uintData(idx+2,1),2) dec2hex(uintData(idx+3,1),2)]);
hdrFileData=uintData(idx+4:idx+4+hdrDataLength-1,1);
idx=idx+4+hdrDataLength;

if incHdrFile==1
    hdrData=hdrFileData;
elseif hdrPtrLength>0 && exist(hdrPtr,'file')==2
    % read header file
    hdrData=[];
else
    hdrData=[];
end

cfgPtrLength=hex2dec([dec2hex(uintData(idx,1),2) dec2hex(uintData(idx+1,1),2) dec2hex(uintData(idx+2,1),2) dec2hex(uintData(idx+3,1),2)]);
cfgPtr=uintData(idx+4:idx+4+cfgPtrLength-1,1);
idx=idx+4+cfgPtrLength;

cfgDataLength=hex2dec([dec2hex(uintData(idx,1),2) dec2hex(uintData(idx+1,1),2) dec2hex(uintData(idx+2,1),2) dec2hex(uintData(idx+3,1),2)]);
cfgFileData=uintData(idx+4:idx+4+cfgDataLength-1,1);
idx=idx+4+cfgDataLength;

if incCfgFile==1
    [PMU, config] =readConfigFrameRev(cfgFileData);
elseif cfgPtrLength>0 && exist(cfgPtr,'file')==2
else
    config=[];
    PMU = [];
end

iniPtrLength=hex2dec([dec2hex(uintData(idx,1),2) dec2hex(uintData(idx+1,1),2) dec2hex(uintData(idx+2,1),2) dec2hex(uintData(idx+3,1),2)]);
iniPtr=uintData(idx+4:idx+4+iniPtrLength-1,1);
idx=idx+4+iniPtrLength;

iniDataLength=hex2dec([dec2hex(uintData(idx,1),2) dec2hex(uintData(idx+1,1),2) dec2hex(uintData(idx+2,1),2) dec2hex(uintData(idx+3,1),2)]);
iniFileData=uintData(idx+4:idx+4+iniDataLength-1,1);
idx=idx+4+iniDataLength;

if incIniFile==1
    iniData=iniFileData;
elseif iniPtrLength>0 && exist(iniPtr,'file')==2
    % read ini file
    iniData=[];
else
    iniData=[];
end

config.dataPadded=dataPadded;
config.incIniFile=incIniFile;
config.incCfgFile=incCfgFile;
config.incHdrFile=incHdrFile;

data=struct;
ind=1;
while idx<size(uintData,1)
    data.sync(ind,1)=double(typecast([intData(idx+1,1) intData(idx,1)],'uint16'));
    data.frameSize(ind,1)=double(typecast([intData(idx+3,1) intData(idx+2,1)],'uint16'));
    idCode=double(typecast([intData(idx+5,1) intData(idx+4,1)],'uint16'));
    data.SOC(ind,1)=double(typecast(int8([intData(idx+9,1) intData(idx+8,1) intData(idx+7,1) intData(idx+6,1)]),'uint32'));
    data.fracSec(ind,1)=double(typecast(int8([intData(idx+13,1) intData(idx+12,1) intData(idx+11,1) intData(idx+10,1)]),'uint32'));
    idx=idx+config.dataFrameSize;
    ind=ind+1;
end
fs = config.dataRate;
% get time of
t = (data.SOC+data.fracSec/config.timeBase)/86400 + datenum(1970,1,1);

% Perform checks on the timestamps.
dt = diff(t);
if (max(dt) > min(dt)*1.5) || (sum(isnan(dt)) > 0)
    % There are either missing samples or the timestamps are bad.
    
    % If FileLength hasn't been calculated yet (this is the first file)
    % then there's nothing that can be done so check first.
    if ~isempty(FileLength)
        if length(t)/config.dataRate == FileLength
            % All the samples are present, so the timestamps must be
            % corrupt.
            % This assumes that config.dataRate is correct, but if it's not
            % there's no way to fix the timestamps anyway.
            warning(['Timestamps in ' pdatFile ' appear corrupt. Attempting to correct.']);
            T0 = getPdatFileTime(pdatFile);
            t = T0 + (0:1/config.dataRate:FileLength)/(60*60*24);
            t = t(1:end-1)';
        else
            % Assuming config.dataRate is correct, some samples must be
            % missing. This should be handled by the data quality filter
            % for missing samples, rather than here (assume that the
            % timestamps are accurate for the samples that are present).
            warning([pdatFile ' appears to have missing samples. Problems will ensue if the data quality filter for missing data is not implemented.']);
        end
    else
        warning(['The file ' pdatFile ' has missing samples or corrupt timestamps. Continued problems are likely because a file has not yet been successfully loaded.']);
    end
end

tPMU = t;
t_str = cellstr(datestr(t,'yyyy-mm-dd HH:MM:SS.FFF'));
nData = length(tPMU); % number of data samples in each channel
if mod(length(intData(offsetToData+1:end)),config.dataFrameSize)~=0
    config=[];
    data=[];
else
    rawArr = reshape(intData(offsetToData+1:end),config.dataFrameSize,[])';
    for ind=1:config.numPMUs
        dataVal = zeros(nData,config.pmu{ind}.numPhsrs*2 + config.pmu{ind}.numAnlgs + config.pmu{ind}.numDigs +2);
        PMU(ind).Signal_Time.Time_String = t_str;
        PMU(ind).Signal_Time.Signal_datenum = t;
        PMU(ind).Signal_Time.datetime = datetime(t,'ConvertFrom','datenum','Format','MM/dd/yy HH:mm:ss.SSSSSS');
        
        PMU(ind).File_Name = pdatFile;   % file name
        
        pmuOffset=config.pmu{ind}.pmuOffset;
        tempData=rawArr(:,pmuOffset:pmuOffset+1);
        tempData=reshape(tempData',1,[]);
        PMU(ind).Stat=swapbytes(typecast(tempData,'uint16'))';
        TotalNumPhasorSignal = config.pmu{ind}.numPhsrs;
        try
            for ind2=1:TotalNumPhasorSignal
                phsrOffset=config.pmu{ind}.phsr{ind2}.sigOffset;
                if config.pmu{ind}.phFmt==1 && config.pmu{ind}.phCoord==1
                    tempData=rawArr(:,phsrOffset:phsrOffset+3);
                    tempData=reshape(tempData',1,[]);
                    dataVal(:,2*ind2-1) =double(swapbytes(typecast(tempData,'single'))'); % gives magnitude of phasor signal
                    tempData=rawArr(:,phsrOffset+4:phsrOffset+7);
                    tempData=reshape(tempData',1,[]);
                    dataVal(:,2*ind2) =double(swapbytes(typecast(tempData,'single'))')*180/pi; % gives the phase of the phasor signal
                elseif config.pmu{ind}.phFmt==1 && config.pmu{ind}.phCoord==0
                    tempData=rawArr(:,phsrOffset:phsrOffset+3);
                    tempData=reshape(tempData',1,[]);
                    phsrReal=double(swapbytes(typecast(tempData,'single'))');
                    tempData=rawArr(:,phsrOffset+4:phsrOffset+7);
                    tempData=reshape(tempData',1,[]);
                    phsrImag=double(swapbytes(typecast(tempData,'single'))');
                    [dataTempAng, dataVal(:,2*ind2-1)]=cart2pol(phsrReal,phsrImag);  % gives magnitude of phasor signal
                    dataVal(:,2*ind2) =dataTempAng*180/pi;  % gives phase of phasor signal
                elseif config.pmu{ind}.phFmt==0 && config.pmu{ind}.phCoord==1
                    tempData=rawArr(:,phsrOffset:phsrOffset+1);
                    tempData=reshape(tempData',1,[]);
                    dataVal(:,2*ind2-1)=double(swapbytes(typecast(tempData,'uint16'))')*config.pmu{ind}.phsr{ind2}.scale;  % gives magnitude of phasor signal
                    tempData=rawArr(:,phsrOffset+2:phsrOffset+3);
                    tempData=reshape(tempData',1,[]);
                    dataTempAng=double(swapbytes(typecast(tempData,'int16'))');
                    dataVal(:,2*ind2)=dataTempAng*(180/pi)*.0001;  % gives phase of phasor signal
                else
                    tempData=rawArr(:,phsrOffset:phsrOffset+1);
                    tempData=reshape(tempData',1,[]);
                    phsrReal=double(swapbytes(typecast(tempData,'int16'))');
                    tempData=rawArr(:,phsrOffset+2:phsrOffset+3);
                    tempData=reshape(tempData',1,[]);
                    phsrImag=double(swapbytes(typecast(tempData,'int16'))');
                    [dataTempang, dataTempmag]=cart2pol(phsrReal,phsrImag);
                    dataVal(:,2*ind2-1)=dataTempmag*config.pmu{ind}.phsr{ind2}.scale;  % gives magnitude of phasor signal
                    dataVal(:,2*ind2)=dataTempang*180/pi;  % gives phase of phasor signal
                    %dataVal(:,[2*ind2-1, 2*ind2]) = [data.pmu{ind}.phsr{ind2}.mag data.pmu{ind}.phsr{ind2}.ang];
                end
            end
            TotalNumAnalogSignal = config.pmu{ind}.numAnlgs;
            for ind2=1:TotalNumAnalogSignal
                anlgOffset=config.pmu{ind}.anlg{ind2}.sigOffset;
                if config.pmu{ind}.anFmt==1
                    tempData=rawArr(:,anlgOffset:anlgOffset+3);
                    tempData=reshape(tempData',1,[]);
                    dataVal(:,TotalNumPhasorSignal*2 + ind2)=double(swapbytes(typecast(tempData,'single'))');
                else
                    tempData=rawArr(:,anlgOffset:anlgOffset+1);
                    tempData=reshape(tempData',1,[]);
                    dataVal(:,TotalNumPhasorSignal*2 + ind2)=double(swapbytes(typecast(tempData,'int16'))')*config.pmu{ind}.anlg{ind2}.scale;
                end
            end
            for ind2=1:config.pmu{ind}.numDigs
                digOffset=config.pmu{ind}.dig{ind2}.sigOffset;
                tempData=rawArr(:,digOffset:digOffset+1);
                tempData=reshape(tempData',1,[]);
                dataVal(:,TotalNumPhasorSignal*2 + TotalNumAnalogSignal + ind2)=double(swapbytes(typecast(tempData,'uint16'))');     
                PMU(ind).Signal_Type{TotalNumPhasorSignal*2 + TotalNumAnalogSignal + ind2} = 'D';
                PMU(ind).Signal_Unit{TotalNumPhasorSignal*2 + TotalNumAnalogSignal + ind2} = 'D';
            end
            
            frqOffset=config.pmu{ind}.frqOffset;
            if config.pmu{ind}.frqFmt==1
                tempData=rawArr(:,frqOffset:frqOffset+3);
                tempData=reshape(tempData',1,[]);
                dataVal(:,end-1)=double(swapbytes(typecast(tempData,'single'))'); % Gives frequency signal
                tempData=rawArr(:,frqOffset+4:frqOffset+7);
                tempData=reshape(tempData',1,[]);
                dataVal(:,end)=double(swapbytes(typecast(tempData,'single'))'); % Gives rocof signal
                chkOffset=frqOffset+8;
            else
                tempData=rawArr(:,frqOffset:frqOffset+1);
                tempData=reshape(tempData',1,[]);
                tempFrq=double(swapbytes(typecast(tempData,'int16'))');
                dataVal(:,end-1)=tempFrq*.001+config.pmu{ind}.nomFrq; % Gives frequency signal
                tempData=rawArr(:,frqOffset+1:frqOffset+2);
                tempData=reshape(tempData',1,[]);
                dataVal(:,end)=double(swapbytes(typecast(tempData,'int16'))')*.01; % Gives rocof signal
                chkOffset=frqOffset+4;
            end            
        catch
            disp([pdatFile,':data reading error in PMU:',num2str(ind)]);
            disp('Possible different number of data frames');
            
        end
        [m,n] = size(dataVal);
        Flag = false(m,n,Num_Flags);
        PMU(ind).Flag = Flag;
        PMU(ind).Data = dataVal;
    end
end
end
