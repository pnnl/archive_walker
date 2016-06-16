clear ;
t = 0:.01:25;
x =-30+ (0:.01:10)*100;
y = x(end) - (0.01:.01:25)*100;
z = [x(:); y(:)];
z(1:10) = 100;
z(100:150) = -50;
z(3400:end) = 1000;
% zz = zeros(size(z));
zmax = min(z);
zz = z- min(z);
zz = abs(z(2:end))-abs(z(1:end-1));
% DiffInd = find(diff(zz) ~= diff(z));
for DataInd = 2:length(z)
    zz(DataInd) = abs(z(DataInd-1)) + abs(z(DataInd) - z(DataInd-1));
end
Sig = diff(zz);
plot(zz); figure; plot(Sig);
OutIdx = [];
OutIdxDiff = find(abs(Sig)>45); % Find outlier idx
if ~isempty(OutIdxDiff)
    OutlierDiff = Sig(OutIdxDiff);
    OutlierDiffLen = length(OutlierDiff);
    IndCurr = OutIdxDiff(1);
    IndBefore = 1;
    while(IndCurr <= OutIdxDiff(end))
        if Sig(IndCurr) < 0
            OutIdx = [OutIdx; IndBefore:IndCurr];
            IndBefore = IndCurr;
            if IndCurr ~= OutIdxDiff(end)
                IndCurr = OutIdxDiff(find(OutlierDiff == IndCurr) + 1);
            else
                IndCurr = OutIdxDiff(end) + 1;
            end
        else
            NegIdx = find(OutlierDiff<0);
            NegIdx1 = min(find(OutIdxDiff(NegIdx) > IndCurr));
            if ~isempty(NegIdx1)
                OutIdx = [OutIdx; IndCurr:OutIdxDiff(NegIdx1)];
                IndBefore = OutIdxDiff(NegIdx1)+1;
                IndCurr = OutIdxDiff(find(OutlierDiff == NegIdx) + 1);
            else
                OutIdx = [OutIdx; IndCurr:length(Sig)];
                IndCurr = OutIdxDiff(end) + 1;
            end
        end
    end
end