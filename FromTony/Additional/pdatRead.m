%**************************************************************************
%
% pdatRead.m - read .pdat file, send config and data into matlab workspace
%
% config - configuration information, as reported by C37.118 config frame
% data - PMU data, as reported by C37.118 data frames
%
%
%**************************************************************************

function [config, data]=pdatRead(pdatFile)

% Read file in both signed and unsigned int format
m = memmapfile(pdatFile,'format','int8');
intData=m.data;

m = memmapfile(pdatFile,'format','uint8');
uintData=m.data;

% Read top of file flags
fileVersion=uintData(1,1);
c37Version=uintData(2,1);
%dataPadded=uintData(3,1);
flags=uintData(4,1);
flagBin=dec2bin(flags,8);

dataPadded = str2double(flagBin(1,5));
incIniFile=str2double(flagBin(1,6));
incCfgFile=str2double(flagBin(1,7));
incHdrFile=str2double(flagBin(1,8));

% Determine offset to data
offsetToData=hex2dec([dec2hex(uintData(5,1)) dec2hex(uintData(6,1)) dec2hex(uintData(7,1)) dec2hex(uintData(8,1))]);
idx=9;

% Read header information, if included
hdrPtrLength=hex2dec([dec2hex(uintData(idx,1)) dec2hex(uintData(idx+1,1)) dec2hex(uintData(idx+2,1)) dec2hex(uintData(idx+3,1))]);
hdrPtr=uintData(idx+4:idx+4+hdrPtrLength-1,1);
idx=idx+4+hdrPtrLength;

hdrDataLength=hex2dec([dec2hex(uintData(idx,1)) dec2hex(uintData(idx+1,1)) dec2hex(uintData(idx+2,1)) dec2hex(uintData(idx+3,1))]);
hdrFileData=uintData(idx+4:idx+4+hdrDataLength-1,1);
idx=idx+4+hdrDataLength;

if incHdrFile==1
    hdrData=hdrFileData;
elseif hdrPtrLength>0 && exist(hdrPtr,'file')==2
    hdrData=[];
else
    hdrData=[];
end

% Read config information, if included
cfgPtrLength=hex2dec([dec2hex(uintData(idx,1)) dec2hex(uintData(idx+1,1)) dec2hex(uintData(idx+2,1)) dec2hex(uintData(idx+3,1))]);
cfgPtr=uintData(idx+4:idx+4+cfgPtrLength-1,1);
idx=idx+4+cfgPtrLength;

cfgDataLength=hex2dec([dec2hex(uintData(idx,1)) dec2hex(uintData(idx+1,1)) dec2hex(uintData(idx+2,1)) dec2hex(uintData(idx+3,1))]);
cfgFileData=uintData(idx+4:idx+4+cfgDataLength-1,1);
idx=idx+4+cfgDataLength;

if incCfgFile==1
    config=readConfigFrame(cfgFileData);
elseif cfgPtrLength>0 && exist(cfgPtr,'file')==2
    %pdc=readConfigFile(cfgPtr);
else
    config=[];
end

% Read ini information, if included
iniPtrLength=hex2dec([dec2hex(uintData(idx,1)) dec2hex(uintData(idx+1,1)) dec2hex(uintData(idx+2,1)) dec2hex(uintData(idx+3,1))]);
iniPtr=uintData(idx+4:idx+4+iniPtrLength-1,1);
idx=idx+4+iniPtrLength;

iniDataLength=hex2dec([dec2hex(uintData(idx,1)) dec2hex(uintData(idx+1,1)) dec2hex(uintData(idx+2,1)) dec2hex(uintData(idx+3,1))]);
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
config.fileVersion=fileVersion;
config.c37Version=c37Version;

% Create data struct, get sync, framesize, idcode, SOC, timeQual, fracSec
data=struct;
ind=1;
while idx<size(uintData,1)
    data.sync(ind,1)=double(typecast([intData(idx+1,1) intData(idx,1)],'uint16'));
    data.frameSize(ind,1)=double(typecast([intData(idx+3,1) intData(idx+2,1)],'uint16'));
    idCode=double(typecast([intData(idx+5,1) intData(idx+4,1)],'uint16'));
    data.SOC(ind,1)=double(typecast(int8([intData(idx+9,1) intData(idx+8,1) intData(idx+7,1) intData(idx+6,1)]),'uint32'));
    data.timeQual(ind,1)=double(typecast(int8(intData(idx+10,1)),'uint8'));
    data.fracSec(ind,1)=double(typecast(int8([intData(idx+13,1) intData(idx+12,1) intData(idx+11,1) 0]),'uint32'));
    idx=idx+config.dataFrameSize;
    ind=ind+1;
end

% Create time array
data.timeArr=sortSOC(data.SOC,data.fracSec,config.timeBase,config.dataRate);
data.timeArr((data.timeArr(:,1)==4294967295),1)=nan;
data.timeArr((data.timeArr(:,2)==4294967295),2)=nan;
rawArr = reshape(intData(offsetToData+1:end),config.dataFrameSize,[])';

% Read data for each PMU
for ind=1:config.numPMUs
    pmuOffset=config.pmu{ind}.pmuOffset;
    tempData=rawArr(:,pmuOffset:pmuOffset+1);
    tempData=reshape(tempData',1,[]);
    data.pmu{ind}.stat=swapbytes(typecast(tempData,'uint16'))';
    
    % Read phasors based on format
    for ind2=1:config.pmu{ind}.numPhsrs
        phsrOffset=config.pmu{ind}.phsr{ind2}.sigOffset;
        if config.pmu{ind}.phFmt==1 && config.pmu{ind}.phCoord==1
            tempData=rawArr(:,phsrOffset:phsrOffset+3);
            tempData=reshape(tempData',1,[]);
            data.pmu{ind}.phsr{ind2}.mag=double(swapbytes(typecast(tempData,'single'))');
            tempData=rawArr(:,phsrOffset+4:phsrOffset+7);
            tempData=reshape(tempData',1,[]);
            data.pmu{ind}.phsr{ind2}.ang=double(swapbytes(typecast(tempData,'single'))')*180/pi;
        elseif config.pmu{ind}.phFmt==1 && config.pmu{ind}.phCoord==0
            tempData=rawArr(:,phsrOffset:phsrOffset+3);
            tempData=reshape(tempData',1,[]);
            phsrReal=double(swapbytes(typecast(tempData,'single'))');
            tempData=rawArr(:,phsrOffset+4:phsrOffset+7);
            tempData=reshape(tempData',1,[]);
            phsrImag=double(swapbytes(typecast(tempData,'single'))');
            [data.pmu{ind}.phsr{ind2}.ang, data.pmu{ind}.phsr{ind2}.mag]=cart2pol(phsrReal,phsrImag);
            data.pmu{ind}.phsr{ind2}.ang=data.pmu{ind}.phsr{ind2}.ang*180/pi;
        elseif config.pmu{ind}.phFmt==0 && config.pmu{ind}.phCoord==1
            tempData=rawArr(:,phsrOffset:phsrOffset+1);
            tempData=reshape(tempData',1,[]);
            data.pmu{ind}.phsr{ind2}.mag=double(swapbytes(typecast(tempData,'uint16'))')*config.pmu{ind}.phsr{ind2}.scale;
            tempData=rawArr(:,phsrOffset+2:phsrOffset+3);
            tempData=reshape(tempData',1,[]);
            data.pmu{ind}.phsr{ind2}.ang=double(swapbytes(typecast(tempData,'int16'))');
            data.pmu{ind}.phsr{ind2}.ang=data.pmu{ind}.phsr{ind2}.ang*(180/pi)*.0001;
        else
            tempData=rawArr(:,phsrOffset:phsrOffset+1);
            tempData=reshape(tempData',1,[]);
            phsrReal=double(swapbytes(typecast(tempData,'int16'))');
            tempData=rawArr(:,phsrOffset+2:phsrOffset+3);
            tempData=reshape(tempData',1,[]);
            phsrImag=double(swapbytes(typecast(tempData,'int16'))');
            [data.pmu{ind}.phsr{ind2}.ang, data.pmu{ind}.phsr{ind2}.mag]=cart2pol(phsrReal,phsrImag);
            data.pmu{ind}.phsr{ind2}.mag=data.pmu{ind}.phsr{ind2}.mag*config.pmu{ind}.phsr{ind2}.scale;
            data.pmu{ind}.phsr{ind2}.ang=data.pmu{ind}.phsr{ind2}.ang*180/pi;
        end
    end
    
    % Read analogs based on format
    for ind2=1:config.pmu{ind}.numAnlgs
        anlgOffset=config.pmu{ind}.anlg{ind2}.sigOffset;
        if config.pmu{ind}.anFmt==1
            tempData=rawArr(:,anlgOffset:anlgOffset+3);
            tempData=reshape(tempData',1,[]);
            data.pmu{ind}.anlg{ind2}.val=double(swapbytes(typecast(tempData,'single'))');
        else
            tempData=rawArr(:,anlgOffset:anlgOffset+1);
            tempData=reshape(tempData',1,[]);
            data.pmu{ind}.anlg{ind2}.val=double(swapbytes(typecast(tempData,'int16'))');
            data.pmu{ind}.anlg{ind2}.val=data.pmu{ind}.anlg{ind2}.val*config.pmu{ind}.anlg{ind2}.scale;
        end
    end
    
    % Read digitals
    for ind2=1:config.pmu{ind}.numDigs
        digOffset=config.pmu{ind}.dig{ind2}.sigOffset;
        tempData=rawArr(:,digOffset:digOffset+1);
        tempData=reshape(tempData',1,[]);
        data.pmu{ind}.dig{ind2}.val=double(swapbytes(typecast(tempData,'uint16'))');
    end
    
    % Read frequency and rocof, based on format
    frqOffset=config.pmu{ind}.frqOffset;
    if config.pmu{ind}.frqFmt==1       
        tempData=rawArr(:,frqOffset:frqOffset+3);
        tempData=reshape(tempData',1,[]);
        data.pmu{ind}.frq=double(swapbytes(typecast(tempData,'single'))');
        tempData=rawArr(:,frqOffset+4:frqOffset+7);
        tempData=reshape(tempData',1,[]);
        data.pmu{ind}.rocof=double(swapbytes(typecast(tempData,'single'))');
        chkOffset=frqOffset+8;
    else
        tempData=rawArr(:,frqOffset:frqOffset+1);
        tempData=reshape(tempData',1,[]);
        tempFrq=double(swapbytes(typecast(tempData,'int16'))');
        data.pmu{ind}.frq=tempFrq*.001+config.pmu{ind}.nomFrq;
        tempData=rawArr(:,frqOffset+1:frqOffset+2);
        tempData=reshape(tempData',1,[]);
        data.pmu{ind}.rocof=double(swapbytes(typecast(tempData,'int16'))')*.01;
        chkOffset=frqOffset+4;
    end
end
