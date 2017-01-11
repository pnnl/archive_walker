idx = 1;

x = AdditionalOutput.Ringdown.DataRing{idx};
N = size(x,1);
StartLabel = datestr(DetectionResults.Ringdown.RingStart{idx},'HH:MM:SS.FFF');
EndLabel = datestr(DetectionResults.Ringdown.RingEnd{idx},'HH:MM:SS.FFF');

figure, plot(x)
set(gca,'XTick',[0 N]);
set(gca,'XTickLabel',{StartLabel, EndLabel});