function [config, data]=pdatRead(pdatFile)

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
    config=readConfigFrame(cfgFileData);
elseif cfgPtrLength>0 && exist(cfgPtr,'file')==2
    %pdc=readConfigFile(cfgPtr);
else
    config=[];
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
data.timeArr=sortSOC(data.SOC,data.fracSec,config.timeBase,config.dataRate);
data.timeArr((data.timeArr(:,1)==4294967295),1)=nan;
data.timeArr((data.timeArr(:,2)==4294967295),2)=nan;
if mod(length(intData(offsetToData+1:end)),config.dataFrameSize)~=0
    config=[];
    data=[];
else
    rawArr = reshape(intData(offsetToData+1:end),config.dataFrameSize,[])';
    for ind=1:config.numPMUs
        pmuOffset=config.pmu{ind}.pmuOffset;
        tempData=rawArr(:,pmuOffset:pmuOffset+1);
        tempData=reshape(tempData',1,[]);
        data.pmu{ind}.stat=swapbytes(typecast(tempData,'uint16'))';
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
        for ind2=1:config.pmu{ind}.numDigs
            digOffset=config.pmu{ind}.dig{ind2}.sigOffset;
            tempData=rawArr(:,digOffset:digOffset+1);
            tempData=reshape(tempData',1,[]);
            data.pmu{ind}.dig{ind2}.val=double(swapbytes(typecast(tempData,'uint16'))');
        end
        
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
end