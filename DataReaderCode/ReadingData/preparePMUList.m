%% 
% create a cell array of PMU structure that will be sued for concatenation
%
% Input:
%   PMUall: a cell array of PMUs that was created before
%   PMU: a PMU data structure that was just processed
%   oneMinuteEmptyPMU: an empty PMU one minute data structure
%   secondsNeeded: how many secondes PMUall should cover
%
% Output: 
%   PMUall:  a created cell array of PMUs that will be used for concatenation detecitons
%
% Created on 6/23/2016 by Tao Fu
% 

function outPMUall = preparePMUList(PMUall,PMU,oneMinuteEmptyPMU,secondsNeeded)

%% in case this is the first PMU
if(isempty(PMUall))
    PMUall{1} = PMU;
    outPMUall = PMUall;
    return;
end

%% return the input PMU if it already contains needed data time
T = PMU(1).Signal_Time.Signal_datenum;
dt = T(end)-T(1);
dt = dt*24*3600;
if(dt > secondsNeeded)
    PMUall{1} = PMU;
    outPMUall = PMUall;
    return;
end

%% return the input PMU if the needed time will not include the last processed PMU
lastPMU = PMUall{end};
prevEndT = lastPMU(1).Signal_Time.Signal_datenum(end); % ending time of last processed PMU
thisEndT = PMU(1).Signal_Time.Signal_datenum(end);     % starting time of this PMU
dt = thisEndT-prevEndT;
dt = dt*24*3600;
if(dt >= secondsNeeded)
    PMUall{1} = PMU;
    outPMUall = PMUall;
    return;
end

%% add the current PMU to the list, also need to take care of missing files
% add possible missing files
thisStartT = PMU(1).Signal_Time.Signal_datenum(1);     % starting time of this PMU

dt = thisStartT-prevEndT;
dt = dt*24*3600;
if(dt > 1) 
    % has more than 1s time gap
    missingT = (prevEndT*24*3600:1/60:thisStartT*24*3600);    
    missingT = missingT(2:end-1)/24/3600;
    missingT = missingT';
    missingT_str = datestr(missingT,'yyyy-mm-dd HH:MM:SS.FFF');
    missingT_str = cellstr(missingT_str);
    while(length(missingT) > 3600)
        addPMU = oneMinuteEmptyPMU;
        T = missingT(1:3600);
        T_str = missingT_str(1:3600);
        for i = 1:length(addPMU)
            addPMU(i).Signal_Time.Time_String = T_str;
            addPMU(i).Signal_Time.Signal_datenum = T;
        end
        PMUall = [PMUall,addPMU];
        if(length(missingT) > 3600)
            missingT = missingT(3600+1:end);
            missingT_str = missingT_str(3600+1:end);                       
        elseif(length(missingT) == 3600)
            missingT = [];
            missingT_str = {};
        end
    end
    
    if(length(missingT) > 0)
        % add a PMU that is less than one minute
        n = length(missingT);
        addPMU = oneMinuteEmptyPMU;
        for i = 1:length(addPMU)
            addPMU(i).Stat = addPMU(i).Stat(1:n);
            addPMU(i).Data = addPMU(i).Data(1:n,:);
            addPMU(i).Flag = addPMU(i).Flag(1:n,:,:);
            addPMU(i).Signal_Time.Signal_datenum = missingT;
            addPMU(i).Signal_Time.Time_String = missingT_str;
        end
        PMUall = [PMUall,addPMU];
    end
end



% add current PMU to the list
PMUall = [PMUall, PMU];

% 

%% selected PMUs from the list that covers needed seconds
n = length(PMUall);
idx = n;
flag = zeros(n,1);
enoughData = 0;
while(idx > 0 && ~enoughData)
    currPMU = PMUall{idx};
    flag(idx) = 1; % the current PMU will be output anyway
    
    if(idx == n)
        startT = currPMU(1).Signal_Time.Signal_datenum(1);
        endT = currPMU(1).Signal_Time.Signal_datenum(end); % endT will stay the same
    else
        startT = currPMU(1).Signal_Time.Signal_datenum(1);
    end
    
    dt = endT - startT;
    dt = dt*24*3600; % day to seconds
    
    if(dt >= secondsNeeded)
        enoughData = 1;
    end
    idx = idx-1; % move 1 PMU back
end

k = find(flag == 1);
outPMUall = PMUall(k);



