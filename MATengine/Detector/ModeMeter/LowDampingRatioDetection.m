function DampingRatio = LowDampingRatioDetection(Mode, DampRatioThreshold)
DampingRatio = -real(Mode)/abs(Mode);
if DampingRatio > DampRatioThreshold
    DampingRatio = NaN;
end