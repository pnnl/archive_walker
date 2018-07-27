% function config = readConfigFrame(configFrame)
% This function reads C37.118 Configuration Frame and creates PDC object
% 
% Inputs:
	% configFrame: ConfigFrame is double array, each element=1 byte
% 
% Outputs:
    % config: PDC object 
%    
%Created by 

function config = readConfigFrame(configFrame)

config=struct;
sync=double(typecast(uint8([configFrame(2,1) configFrame(1,1)]),'uint16'));
framesize=double(typecast(uint8([configFrame(4,1) configFrame(3,1)]),'uint16'));
config.idcode=double(typecast(uint8([configFrame(6,1) configFrame(5,1)]),'uint16'));
config.SOC=double(typecast(uint8([configFrame(10,1) configFrame(9,1) configFrame(8,1) configFrame(7,1)]),'uint32'));
config.fracSec=double(typecast(uint8([configFrame(14,1) configFrame(13,1) configFrame(12,1) configFrame(11,1)]),'uint32'));
config.timeBase=double(typecast(uint8([configFrame(18,1) configFrame(17,1) configFrame(16,1) 0]),'uint32'));
config.numPMUs=double(typecast(uint8([configFrame(20,1) configFrame(19,1)]),'uint16'));


config.pmuNames=cell(config.numPMUs,1);
config.pmuIDs=zeros(config.numPMUs,1);
count=21;
dataFrameCount=15;

for ind=1:config.numPMUs
    config.pmu{ind}.name=strcat(configFrame(count:count+15,1)');
    config.pmuNames{ind,1}=config.pmu{ind}.name;
    config.pmu{ind}.idcode=double(typecast(uint8([configFrame(count+17,1) configFrame(count+16,1)]),'uint16'));
    config.pmuIDs(ind,1)=config.pmu{ind}.idcode;
    config.pmu{ind}.fmt=double(typecast(int8([configFrame(count+19,1) configFrame(count+18,1)]),'uint16'));
    
    fmtStr=dec2bin(config.pmu{ind}.fmt,16);
    config.pmu{ind}.frqFmt=str2double(fmtStr(13));
    config.pmu{ind}.anFmt=str2double(fmtStr(14));
    config.pmu{ind}.phFmt=str2double(fmtStr(15));
    config.pmu{ind}.phCoord=str2double(fmtStr(16));
    
    config.pmu{ind}.numPhsrs=double(typecast(int8([configFrame(count+21,1) configFrame(count+20,1)]),'uint16'));
    config.pmu{ind}.numAnlgs=double(typecast(int8([configFrame(count+23,1) configFrame(count+22,1)]),'uint16'));
    config.pmu{ind}.numDigs=double(typecast(int8([configFrame(count+25,1) configFrame(count+24,1)]),'uint16'));
    
    count=count+26;
    
    config.pmu{ind}.pmuOffset=dataFrameCount;
    dataFrameCount=dataFrameCount+2;
    
    for ind2=1:config.pmu{ind}.numPhsrs
        config.pmu{ind}.phsr{ind2}.name=strcat(configFrame(count:count+15,1)');
        config.pmu{ind}.phsr{ind2}.sigOffset=dataFrameCount;
        count=count+16;
        if config.pmu{ind}.phFmt==1
            dataFrameCount=dataFrameCount+8;
        else
            dataFrameCount=dataFrameCount+4;
        end
    end
    
    config.pmu{ind}.frqOffset=dataFrameCount;
    if config.pmu{ind}.frqFmt==1
        dataFrameCount=dataFrameCount+8;
    else
        dataFrameCount=dataFrameCount+4;
    end
    
    
    for ind2=1:config.pmu{ind}.numAnlgs
        config.pmu{ind}.anlg{ind2}.name=strcat(configFrame(count:count+15,1)');
        config.pmu{ind}.anlg{ind2}.sigOffset=dataFrameCount;
        count=count+16;
        if config.pmu{ind}.anFmt==1
            dataFrameCount=dataFrameCount+4;
        else
            dataFrameCount=dataFrameCount+2;
        end
    end
    
    for ind2=1:config.pmu{ind}.numDigs
        for ind3=1:16
            config.pmu{ind}.dig{ind2}.name{ind3,1}=strcat(configFrame(count:count+15,1)');
            count=count+16;
        end
        config.pmu{ind}.dig{ind2}.sigOffset=dataFrameCount;
        dataFrameCount=dataFrameCount+2;
    end
    
    for ind2=1:config.pmu{ind}.numPhsrs
        config.pmu{ind}.phsr{ind2}.phUnit=typecast(uint8([configFrame(count+3,1)...
            configFrame(count+2,1) configFrame(count+1,1) configFrame(count,1)]),'uint32');
        count=count+4;
        phUnitStr=dec2bin(config.pmu{ind}.phsr{ind2}.phUnit,32);
        config.pmu{ind}.phsr{ind2}.sigTphype=str2double(phUnitStr(8));
        config.pmu{ind}.phsr{ind2}.scale=bin2dec(phUnitStr(9:32));
        config.pmu{ind}.phsr{ind2}.scaleStr=num2str(config.pmu{ind}.phsr{ind2}.scale*10^-5);
    end
    
    for ind2=1:config.pmu{ind}.numAnlgs
        config.pmu{ind}.anlg{ind2}.anUnit=typecast(uint8([configFrame(count+3,1)...
            configFrame(count+2,1) configFrame(count+1,1) configFrame(count,1)]),'uint32');
        count=count+4;
        anUnitStr=dec2bin(config.pmu{ind}.anlg{ind2}.anUnit,32);
        config.pmu{ind}.anlg{ind2}.sigType=str2double(anUnitStr(7:8));
        config.pmu{ind}.anlg{ind2}.scale=str2double(anUnitStr(9:32));
        config.pmu{ind}.anlg{ind2}.scaleStr=num2str(bin2dec(anUnitStr(9:32)));
    end
    
    for ind2=1:config.pmu{ind}.numDigs
        config.pmu{ind}.dig{ind2}.digUnit=typecast(uint8([configFrame(count+3,1)...
            configFrame(count+2,1) configFrame(count+1,1) configFrame(count,1)]),'uint32');
        count=count+4;
        config.pmu{ind}.dig{ind2}.digUnitStr=dec2bin(config.pmu{ind}.dig{ind2}.digUnit,32);
    end
    
    nomFrqBit=typecast(int8([configFrame(count+1,1) configFrame(count,1)]),'uint16');
    if nomFrqBit==0
        config.pmu{ind}.nomFrq=60;
    else
        config.pmu{ind}.nomFrq=50;
    end
    
    config.pmu{ind}.cfgCount=typecast(int8([configFrame(count+3,1) configFrame(count+2,1)]),'uint16');
    count=count+4;
end
config.dataRate=double(typecast(int8([configFrame(count+1,1) configFrame(count,1)]),'int16'));
if config.dataRate<0
    config.dataRate=1/(abs(config.dataRate));
end
checkSum=typecast(int8([configFrame(count+3,1) configFrame(count+2,1)]),'uint16');
config.dataFrameSize=dataFrameCount+1;

% 
% pdc=struct;
% 
% %id=fopen(configFile,'r','b');
% 
% sync=fread(fid,1,'uint16');
% framesize=fread(fid,1,'uint16');
% pdc.idcode=fread(fid,1,'uint16');
% pdc.SOC=fread(fid,1,'uint32');
% pdc.fracSec=fread(fid,1,'uint32');
% pdc.timeBase=fread(fid,1,'uint32');
% pdc.numPMUs=fread(fid,1,'uint16');
% 
% 
% for ind=1:pdc.numPMUs
%     pdc.pmu{ind}.name=strcat(fread(fid,16,'char')');
%     pdc.pmu{ind}.idcode=fread(fid,1,'uint16');
%     pdc.pmu{ind}.fmt=fread(fid,1,'uint16');
%     
%     fmtStr=dec2bin(pdc.pmu{ind}.fmt,16);
%     pdc.pmu{ind}.frqFmt=str2double(fmtStr(13));
%     pdc.pmu{ind}.anFmt=str2double(fmtStr(14));
%     pdc.pmu{ind}.phFmt=str2double(fmtStr(15));
%     pdc.pmu{ind}.phCoord=str2double(fmtStr(16));
%     
%     pdc.pmu{ind}.numPhsrs=fread(fid,1,'uint16');
%     pdc.pmu{ind}.numAnlgs=fread(fid,1,'uint16');
%     pdc.pmu{ind}.numDigs=fread(fid,1,'uint16');
%     
%     for ind2=1:pdc.pmu{ind}.numPhsrs
%         pdc.pmu{ind}.phsr{ind2}.name=strcat(fread(fid,16,'char')');
%     end
%     
%     for ind2=1:pdc.pmu{ind}.numAnlgs
%         pdc.pmu{ind}.anlg{ind2}.name=strcat(fread(fid,16,'char')');
%     end
%     
%     for ind2=1:pdc.pmu{ind}.numDigs
%         for ind3=1:16
%             pdc.pmu{ind}.dig{ind2}.name{ind3,1}=strcat(fread(fid,16,'char')');
%         end
%     end
%     
%     for ind2=1:pdc.pmu{ind}.numPhsrs
%         pdc.pmu{ind}.phsr{ind2}.phUnit=fread(fid,1,'uint32');
%         phUnitStr=dec2bin(pdc.pmu{ind}.phsr{ind2}.phUnit,32);
%         pdc.pmu{ind}.phsr{ind2}.sigType=str2double(phUnitStr(8));
%         pdc.pmu{ind}.phsr{ind2}.scale=bin2dec(phUnitStr(9:32));
%         pdc.pmu{ind}.phsr{ind2}.scaleStr=num2str(pdc.pmu{ind}.phsr{ind2}.scale*10^-5);
%     end
%     
%     for ind2=1:pdc.pmu{ind}.numAnlgs
%         pdc.pmu{ind}.anlg{ind2}.anUnit=fread(fid,1,'uint32');
%         anUnitStr=dec2bin(pdc.pmu{ind}.anlg{ind2}.anUnit,32);
%         pdc.pmu{ind}.anlg{ind2}.sigType=str2double(anUnitStr(7:8));
%         pdc.pmu{ind}.anlg{ind2}.scale=str2double(anUnitStr(9:32));
%         pdc.pmu{ind}.anlg{ind2}.scaleStr=num2str(bin2dec(anUnitStr(9:32)));
%     end
%     
%     for ind2=1:pdc.pmu{ind}.numDigs
%         pdc.pmu{ind}.dig{ind2}.digUnit=fread(fid,1,'uint32');
%         pdc.pmu{ind}.dig{ind2}.digUnitStr=dec2bin(pdc.pmu{ind}.dig{ind2}.digUnit,32);
%     end
%     
%     nomFrqBit=fread(fid,1,'uint16');
%     if nomFrqBit==0
%         pdc.pmu{ind}.nomFrq=60;
%     else
%         pdc.pmu{ind}.nomFrq=50;
%     end
%     
%     pdc.pmu{ind}.cfgCount=fread(fid,1,'uint16');
% end
% 
% pdc.dataRate=fread(fid,1,'uint16');
% checkSum=fread(fid,1,'uint16');

%fclose(fid);
