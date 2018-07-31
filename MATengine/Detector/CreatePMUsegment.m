function PMUsegment = CreatePMUsegment(PMUconcat,SecondsToConcat,PMUconcatLength,ResultUpdateInterval,SlideIdx)

PMUsegment = PMUconcat;
for PMUidx = 1:length(PMUsegment)
    N = size(PMUsegment(PMUidx).Data,1);
    SamplesToConcat = floor(N*SecondsToConcat/PMUconcatLength);
    SampleUpdateInterval = floor(N*ResultUpdateInterval/PMUconcatLength);

    % Indices of PMUsegment, which was made equal to
    % PMUconcat at the beginning of this loop, that
    % correspond to the current analysis window. Only
    % these indices are retained in PMUsegment.
    idx = (SlideIdx-1)*SampleUpdateInterval+(1:SamplesToConcat);

    % Retain only the current analysis window
    PMUsegment(PMUidx).Stat = PMUsegment(PMUidx).Stat(idx);
    PMUsegment(PMUidx).Data = PMUsegment(PMUidx).Data(idx,:);
    PMUsegment(PMUidx).Flag = PMUsegment(PMUidx).Flag(idx,:,:);
    PMUsegment(PMUidx).Signal_Time.Time_String = PMUsegment(PMUidx).Signal_Time.Time_String(idx);
    PMUsegment(PMUidx).Signal_Time.Signal_datenum = PMUsegment(PMUidx).Signal_Time.Signal_datenum(idx);
end