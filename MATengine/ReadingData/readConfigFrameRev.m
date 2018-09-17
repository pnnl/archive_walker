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

function [PMU, config] = readConfigFrameRev(configFrame)

config=struct;
sync=double(typecast(uint8([configFrame(2,1) configFrame(1,1)]),'uint16'));
framesize=double(typecast(uint8([configFrame(4,1) configFrame(3,1)]),'uint16'));
config.idcode=double(typecast(uint8([configFrame(6,1) configFrame(5,1)]),'uint16'));
config.SOC=double(typecast(uint8([configFrame(10,1) configFrame(9,1) configFrame(8,1) configFrame(7,1)]),'uint32'));
config.fracSec=double(typecast(uint8([configFrame(14,1) configFrame(13,1) configFrame(12,1) configFrame(11,1)]),'uint32'));
config.timeBase=double(typecast(uint8([configFrame(18,1) configFrame(17,1) configFrame(16,1) 0]),'uint32'));
config.numPMUs=double(typecast(uint8([configFrame(20,1) configFrame(19,1)]),'uint16'));

%%
% initalize an empty PMU structure
nPMU = config.numPMUs;
singlePMU = struct('File_Name',[],'PMU_Name',[], 'Time_Zone',[], 'Signal_Name',[], 'Signal_Type',[], 'Signal_Unit',[], 'Signal_Time', [], 'Stat', [], 'Data', [], 'Flag', []);
PMU = repmat(singlePMU, 1,nPMU);


config.pmuNames=cell(config.numPMUs,1);
% config.pmuIDs=zeros(config.numPMUs,1);
count=21;
dataFrameCount=15;

for ind=1:nPMU
    % for each PMU
    PMU(ind).Signal_Name = [];
    % read in all fixed fields for the 1st time
    PMU(ind).PMU_Name = strcat(configFrame(count:count+15,1)');
    PMU(ind).Time_Zone = '-08:00';         % time zone; for now this is just the PST time
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
        PMU(ind).Signal_Name{2*ind2-1} = [PMU(ind).PMU_Name,'.',config.pmu{ind}.phsr{ind2}.name,'.MAG'];
        PMU(ind).Signal_Name{2*ind2} = [PMU(ind).PMU_Name,'.',config.pmu{ind}.phsr{ind2}.name,'.ANG'];
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
        PMU(ind).Signal_Name{2*config.pmu{ind}.numPhsrs + ind2} = [PMU(ind).PMU_Name,'.',config.pmu{ind}.anlg{ind2}.name];
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
        PMU(ind).Signal_Name{2*config.pmu{ind}.numPhsrs + config.pmu{ind}.numAnlgs + ind2} = [PMU(ind).PMU_Name,'.dig',num2str(ind2)];
        config.pmu{ind}.dig{ind2}.sigOffset=dataFrameCount;
        dataFrameCount=dataFrameCount+2;
    end
    
    % add names for freq and rocof
    frqName = [PMU(ind).PMU_Name,'.','frq'];
    rocofName = [PMU(ind).PMU_Name,'.','rocof'];
    
    PMU(ind).Signal_Name = [PMU(ind).Signal_Name frqName rocofName];
    
    % Signal_Type and Signal_Unit are empty cells for now
    % we will update them after the signal selection
    PMU(ind).Signal_Type = cell(1,length(PMU(ind).Signal_Name));
    PMU(ind).Signal_Unit = cell(1,length(PMU(ind).Signal_Name));
    
    for ind2=1:config.pmu{ind}.numPhsrs
        config.pmu{ind}.phsr{ind2}.phUnit=typecast(uint8([configFrame(count+3,1)...
            configFrame(count+2,1) configFrame(count+1,1) configFrame(count,1)]),'uint32');
        count=count+4;
        phUnitStr=dec2bin(config.pmu{ind}.phsr{ind2}.phUnit,32);
        config.pmu{ind}.phsr{ind2}.sigType=str2double(phUnitStr(8));
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
% set PMU Structure
PMU = SetNameAndUnit_PDAT(PMU);
config.dataRate=double(typecast(int8([configFrame(count+1,1) configFrame(count,1)]),'int16'));
if config.dataRate<0
    config.dataRate=1/(abs(config.dataRate));
end
checkSum=typecast(int8([configFrame(count+3,1) configFrame(count+2,1)]),'uint16');
config.dataFrameSize=dataFrameCount+1;

end



