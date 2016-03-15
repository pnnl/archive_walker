function timeArr=sortSOC(SOC,fracSec,timeBase,dataRate)

timeArr=[SOC fracSec];

a=1:2:dataRate*2-2;
lowLim=[0; a']/2;
highLim=[a'; dataRate*2]/2;

x=fracSec/timeBase*dataRate;

SOCvals=unique(SOC);

for ind=1:size(SOCvals,1)
    SOCloc=find(SOC==SOCvals(ind,1));
    tempFracSec=fracSec(SOCloc,1);
    for ind2=1:length(tempFracSec)
        for ind3=1:dataRate
            if x(SOCloc(ind2),1)>=lowLim(ind3)&& x(SOCloc(ind2),1)<highLim(ind3)
                timeArr(SOCloc(ind2),3)=ind3;
            end
        end
    end
end