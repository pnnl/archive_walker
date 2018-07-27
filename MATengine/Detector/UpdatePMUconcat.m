function PMUconcat = UpdatePMUconcat(PMUconcat,ResultUpdateInterval,PMUconcatLength,NumSlide)

% Remove older data from PMUconcat that is no longer needed
for PMUidx = 1:length(PMUconcat)
    % Current length of data in PMUconcat
    N = size(PMUconcat(PMUidx).Data,1);
    SampleUpdateInterval = floor(N*ResultUpdateInterval/PMUconcatLength);

    % Number of samples that need to be kept
    NumToKeep = N-NumSlide*SampleUpdateInterval;
    % Indices of samples that need to be kept (the newest
    % NumToKeep samples)
    idx = N-NumToKeep+1:N;

    % Remove all but the most recent NumToKeep samples from
    % PMUconcat
    PMUconcat(PMUidx).Stat = PMUconcat(PMUidx).Stat(idx);
    PMUconcat(PMUidx).Data = PMUconcat(PMUidx).Data(idx,:);
    PMUconcat(PMUidx).Flag = PMUconcat(PMUidx).Flag(idx,:,:);
    PMUconcat(PMUidx).Signal_Time.Time_String = PMUconcat(PMUidx).Signal_Time.Time_String(idx);
    PMUconcat(PMUidx).Signal_Time.Signal_datenum = PMUconcat(PMUidx).Signal_Time.Signal_datenum(idx);
end