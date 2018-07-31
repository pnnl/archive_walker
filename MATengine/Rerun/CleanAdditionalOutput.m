function AdditionalOutputCondos = CleanAdditionalOutput(AdditionalOutput)

AdditionalOutputCondos = AdditionalOutput;

if isfield(AdditionalOutputCondos,'Ringdown')
    for DetIdx = 1:length(AdditionalOutputCondos)
        if ~isempty(AdditionalOutputCondos(DetIdx).Ringdown)
            FN = fieldnames(AdditionalOutputCondos(DetIdx).Ringdown);
            AdditionalOutputCondos(DetIdx).Ringdown = rmfield(AdditionalOutputCondos(DetIdx).Ringdown, setdiff(FN,{'FilterConditions', 'RMShist'}));
        end
    end
end

if isfield(AdditionalOutputCondos,'Periodogram')
    AdditionalOutputCondos = rmfield(AdditionalOutputCondos,'Periodogram');
end

if isfield(AdditionalOutputCondos,'SpectralCoherence')
    AdditionalOutputCondos = rmfield(AdditionalOutputCondos,'SpectralCoherence');
end

if isfield(AdditionalOutputCondos,'OutOfRangeGeneral')
    for DetIdx = 1:length(AdditionalOutputCondos)
        if ~isempty(AdditionalOutputCondos(DetIdx).OutOfRangeGeneral)
            FN = fieldnames(AdditionalOutputCondos(DetIdx).OutOfRangeGeneral);
            AdditionalOutputCondos(DetIdx).OutOfRangeGeneral = rmfield(AdditionalOutputCondos(DetIdx).OutOfRangeGeneral, setdiff(FN,{'AverageFilterConditions', 'FilterConditions', 'ExtremeLocs', 'ExtremeVals'}));
        end
    end
end

if isfield(AdditionalOutputCondos,'WindRamp')
    for DetIdx = 1:length(AdditionalOutputCondos)
        if ~isempty(AdditionalOutputCondos(DetIdx).WindRamp)
            FN = fieldnames(AdditionalOutputCondos(DetIdx).WindRamp);
            AdditionalOutputCondos(DetIdx).WindRamp = rmfield(AdditionalOutputCondos(DetIdx).WindRamp, setdiff(FN,{'FinalConditions', 'extremaType', 'extremaLoc', 'extrema', 'Hd', 'gd', 'ex', 'ProcessedSamples'}));
        end
    end
end

if isfield(AdditionalOutputCondos,'Thevenin')
    for DetIdx = 1:length(AdditionalOutputCondos)
        if ~isempty(AdditionalOutputCondos(DetIdx).Thevenin)
            FN = fieldnames(AdditionalOutputCondos(DetIdx).Thevenin);
            AdditionalOutputCondos(DetIdx).Thevenin = rmfield(AdditionalOutputCondos(DetIdx).Thevenin, setdiff(FN,{'PastVals'}));
        end
    end
end