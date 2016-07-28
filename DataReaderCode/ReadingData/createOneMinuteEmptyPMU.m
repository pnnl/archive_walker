%%
% create an one minute empty PMU structure array with all data set to NaN
%
% input:
%   refPMU: a reference PMU data strucutre array
%
% output:
%   outPMU: an empty one minute PMU structure array with all values set to NaN
%


%%
function PMU = createOneMinuteEmptyPMU(refPMU)

% update PMU data to NaN 
PMU = refPMU;
for i = 1:length(PMU)
    PMU(i).File_Name = '';   % file name
    PMU(i).PMU_Name = 'Empty Signals';   % PMU name
    PMU(i).Time_Zone = '-8:00';         % time zone; for now this is just the PST time
    %     m = length(PMU(i).Stat);  % # of frames
    %     n = length(PMU(i).Signal_Name);  % # of signals
    % gives number of data frames, signals and number of flags
    [m,n,NumFlags] = size(PMU(i).Flag);
    if(m == 3600)
        PMU(i).Stat = PMU(i).Stat*NaN;
        PMU(i).Data = PMU(i).Data*NaN;
        PMU(i).Flag(:,:,end) = true(m,n); %flag matrix
        
    else
        PMU(i).Stat = NaN(3600,1);
        PMU(i).Data = NaN(3600,n);
        PMU(i).Flag = false(3600,n,NumFlags);
        PMU(i).Flag(:,:,end) = true(3600,n); %flag matrix        
    end    
end