function DetectorXML = DisableDetectors(DetectorXML,Detector)

fields = fieldnames(DetectorXML);
switch Detector
    case 'ForcedOscillation'
        DetectorXML = rmfield(DetectorXML, setdiff(fields,{'Periodogram', 'SpectralCoherence', 'EventPath', 'ResultUpdateInterval', 'Alarming'}));
    case 'OutOfRangeGeneral'
        DetectorXML = rmfield(DetectorXML, setdiff(fields,{'OutOfRangeGeneral', 'EventPath', 'ResultUpdateInterval', 'Alarming'}));
    case 'Ringdown'
        DetectorXML = rmfield(DetectorXML, setdiff(fields,{'Ringdown', 'EventPath', 'ResultUpdateInterval', 'Alarming'}));
    case 'WindRamp'
        DetectorXML = rmfield(DetectorXML, setdiff(fields,{'WindRamp', 'EventPath', 'ResultUpdateInterval', 'Alarming'}));
    case 'Thevenin'
        DetectorXML = rmfield(DetectorXML, setdiff(fields,{'Thevenin', 'EventPath', 'ResultUpdateInterval'}));
    case 'ModeMeter'
        DetectorXML = rmfield(DetectorXML, setdiff(fields,{'ModeMeter', 'EventPath', 'ResultUpdateInterval'}));
end