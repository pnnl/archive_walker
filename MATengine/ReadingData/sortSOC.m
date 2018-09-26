% function timeArr=sortSOC(SOC,fracSec,timeBase,dataRate)
% This function reads date and time information and returns time
% array sorted in ascending order
% 
% Inputs:
	% SOC: second of century taken as reference point for calculating
	% numerical value representing timstamp of PMU data
    % fracSec: an integer number representing fraction of a second
    % timeBase: fracSec divided by this number gives the true fraction of a
    % second
    % dataRate: Data reporting time
%     
% Outputs:
    % timeArr: Array dimension number of data points by 3, each column
    % representing SOC, fracSec, and indices of sorted data
%    
%Created by 

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
