function EventListMAT = EventListXML2MAT(EventListXML)

% Default if EventListXML has no entries
EventListMAT = [];

EventListXML = EventListXML.Events;

% Translate the wind ramp portion
if isfield(EventListXML,'WindRamp')
    EventListMAT.WindRamp = [];
    
    if length(EventListXML.WindRamp) == 1
        EventListXML.WindRamp = {EventListXML.WindRamp};
    end
    
    for idx = 1:length(EventListXML.WindRamp)
        EventListMAT.WindRamp(idx).ID = EventListXML.WindRamp{idx}.ID;
        EventListMAT.WindRamp(idx).PMU = EventListXML.WindRamp{idx}.PMU;
        EventListMAT.WindRamp(idx).Channel = EventListXML.WindRamp{idx}.Channel;
        EventListMAT.WindRamp(idx).TrendStart = EventListXML.WindRamp{idx}.TrendStart;
        EventListMAT.WindRamp(idx).TrendEnd = EventListXML.WindRamp{idx}.TrendEnd;
        EventListMAT.WindRamp(idx).TrendValue = str2double(EventListXML.WindRamp{idx}.TrendValue);
        EventListMAT.WindRamp(idx).ValueStart = str2double(EventListXML.WindRamp{idx}.ValueStart);
        EventListMAT.WindRamp(idx).ValueEnd = str2double(EventListXML.WindRamp{idx}.ValueEnd);
    end
end


% Translate the forced oscillation portion
if isfield(EventListXML,'ForcedOscillation')
    EventListMAT.ForcedOscillation = [];
    
    if length(EventListXML.ForcedOscillation) == 1
        EventListXML.ForcedOscillation = {EventListXML.ForcedOscillation};
    end
    
    for idx = 1:length(EventListXML.ForcedOscillation)
        NumOccurrences = length(EventListXML.ForcedOscillation{idx}.Occurrence);
        if NumOccurrences == 1
            EventListXML.ForcedOscillation{idx}.Occurrence = {EventListXML.ForcedOscillation{idx}.Occurrence};
        end
        
        EventListMAT.ForcedOscillation(idx).ID = EventListXML.ForcedOscillation{idx}.ID;
        EventListMAT.ForcedOscillation(idx).OverallStart = datenum(EventListXML.ForcedOscillation{idx}.OverallStart);
        EventListMAT.ForcedOscillation(idx).OverallEnd = datenum(EventListXML.ForcedOscillation{idx}.OverallEnd);
        
        EventListMAT.ForcedOscillation(idx).OccurrenceID = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Frequency = zeros(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Start = zeros(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).End = zeros(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Persistence = zeros(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).AlarmFlag = zeros(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Channel = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).PMU = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Unit = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Amplitude = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).SNR = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Coherence = cell(1,NumOccurrences);
        if isfield(EventListXML.ForcedOscillation{idx}.Occurrence{1},'Path')
            NumPath = length(EventListXML.ForcedOscillation{idx}.Occurrence{1}.Path);
            EventListMAT.ForcedOscillation(idx).DEF = zeros(NumPath,NumOccurrences);
            EventListMAT.ForcedOscillation(idx).PathDescription = cell(2,NumPath);
        else
            NumPath = 0;
        end
        for idx2 = 1:NumOccurrences
            EventListMAT.ForcedOscillation(idx).OccurrenceID{idx2} = EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.OccurrenceID;
            EventListMAT.ForcedOscillation(idx).Frequency(idx2) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Frequency);
            EventListMAT.ForcedOscillation(idx).Start(idx2) = datenum(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Start);
            EventListMAT.ForcedOscillation(idx).End(idx2) = datenum(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.End);
            EventListMAT.ForcedOscillation(idx).Persistence(idx2) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Persistence);
            EventListMAT.ForcedOscillation(idx).AlarmFlag(idx2) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.AlarmFlag);
            
            NumChannel = length(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel);
            if NumChannel == 1
                EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel = {EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel};
            end
            
            EventListMAT.ForcedOscillation(idx).Channel{idx2} = cell(1,NumChannel);
            EventListMAT.ForcedOscillation(idx).PMU{idx2} = cell(1,NumChannel);
            EventListMAT.ForcedOscillation(idx).Unit{idx2} = cell(1,NumChannel);
            EventListMAT.ForcedOscillation(idx).Amplitude{idx2} = zeros(1,NumChannel);
            EventListMAT.ForcedOscillation(idx).SNR{idx2} = zeros(1,NumChannel);
            EventListMAT.ForcedOscillation(idx).Coherence{idx2} = zeros(1,NumChannel);
            for idx3 = 1:NumChannel
                EventListMAT.ForcedOscillation(idx).Channel{idx2}{idx3} = EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.Name;
                EventListMAT.ForcedOscillation(idx).PMU{idx2}{idx3} = EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.PMU;
                EventListMAT.ForcedOscillation(idx).Unit{idx2}{idx3} = EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.Unit;
                EventListMAT.ForcedOscillation(idx).Amplitude{idx2}(idx3) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.Amplitude);
                EventListMAT.ForcedOscillation(idx).SNR{idx2}(idx3) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.SNR);
                EventListMAT.ForcedOscillation(idx).Coherence{idx2}(idx3) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.Coherence);
            end
            
            for pIdx = 1:NumPath
                EventListMAT.ForcedOscillation(idx).DEF(pIdx,idx2) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Path{pIdx}.DEF);
                EventListMAT.ForcedOscillation(idx).PathDescription{1,pIdx} = EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Path{pIdx}.From;
                EventListMAT.ForcedOscillation(idx).PathDescription{2,pIdx} = EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Path{pIdx}.To;
            end
        end
    end
end


% Translate the ringdown portion
if isfield(EventListXML,'Ringdown')
    EventListMAT.Ringdown = [];
    
    if length(EventListXML.Ringdown) == 1
        EventListXML.Ringdown = {EventListXML.Ringdown};
    end
    
    for idx = 1:length(EventListXML.Ringdown)
        EventListMAT.Ringdown(idx).ID = EventListXML.Ringdown{idx}.ID;
        EventListMAT.Ringdown(idx).Start = datenum(EventListXML.Ringdown{idx}.Start);
        EventListMAT.Ringdown(idx).End = datenum(EventListXML.Ringdown{idx}.End);
        
        NumChannel = length(EventListXML.Ringdown{idx}.Channel);
        if NumChannel == 1
            EventListXML.Ringdown{idx}.Channel = {EventListXML.Ringdown{idx}.Channel};
        end
        
        EventListMAT.Ringdown(idx).Channel = cell(1,NumChannel);
        EventListMAT.Ringdown(idx).PMU = cell(1,NumChannel);
        for idx2 = 1:NumChannel
            EventListMAT.Ringdown(idx).Channel{idx2} = EventListXML.Ringdown{idx}.Channel{idx2}.Name;
            EventListMAT.Ringdown(idx).PMU{idx2} = EventListXML.Ringdown{idx}.Channel{idx2}.PMU;
        end
    end
end

% Translate the general out-of-range portion
if isfield(EventListXML,'OutOfRangeGeneral')
    EventListMAT.OutOfRangeGeneral = [];
    
    if length(EventListXML.OutOfRangeGeneral) == 1
        EventListXML.OutOfRangeGeneral = {EventListXML.OutOfRangeGeneral};
    end
    
    for idx = 1:length(EventListXML.OutOfRangeGeneral)
        EventListMAT.OutOfRangeGeneral(idx).ID = EventListXML.OutOfRangeGeneral{idx}.ID;
        EventListMAT.OutOfRangeGeneral(idx).Start = datenum(EventListXML.OutOfRangeGeneral{idx}.Start);
        EventListMAT.OutOfRangeGeneral(idx).End = datenum(EventListXML.OutOfRangeGeneral{idx}.End);
        EventListMAT.OutOfRangeGeneral(idx).Extrema = str2double(EventListXML.OutOfRangeGeneral{idx}.Extrema);
        EventListMAT.OutOfRangeGeneral(idx).ExtremaFactor = str2double(EventListXML.OutOfRangeGeneral{idx}.ExtremaFactor);
        
        NumChannel = length(EventListXML.OutOfRangeGeneral{idx}.Channel);
        if NumChannel == 1
            EventListXML.OutOfRangeGeneral{idx}.Channel = {EventListXML.OutOfRangeGeneral{idx}.Channel};
        end
        
        EventListMAT.OutOfRangeGeneral(idx).Channel = cell(1,NumChannel);
        EventListMAT.OutOfRangeGeneral(idx).PMU = cell(1,NumChannel);
        for idx2 = 1:NumChannel
            EventListMAT.OutOfRangeGeneral(idx).Channel{idx2} = EventListXML.OutOfRangeGeneral{idx}.Channel{idx2}.Name;
            EventListMAT.OutOfRangeGeneral(idx).PMU{idx2} = EventListXML.OutOfRangeGeneral{idx}.Channel{idx2}.PMU;
        end
    end
end

% Translate the wind application portion
if isfield(EventListXML,'WindApp')
    EventListMAT.WindApp = [];
    
    if length(EventListXML.WindApp) == 1
        EventListXML.WindApp = {EventListXML.WindApp};
    end
    
    for idx = 1:length(EventListXML.WindApp)
        EventListMAT.WindApp(idx).ID = EventListXML.WindApp{idx}.ID;
        EventListMAT.WindApp(idx).Start = datenum(EventListXML.WindApp{idx}.Start);
        EventListMAT.WindApp(idx).End = datenum(EventListXML.WindApp{idx}.End);
        EventListMAT.WindApp(idx).Extrema = str2double(EventListXML.WindApp{idx}.Extrema);
        EventListMAT.WindApp(idx).ExtremaFactor = str2double(EventListXML.WindApp{idx}.ExtremaFactor);
        
        NumChannel = length(EventListXML.WindApp{idx}.Channel);
        if NumChannel == 1
            EventListXML.WindApp{idx}.Channel = {EventListXML.WindApp{idx}.Channel};
        end
        
        EventListMAT.WindApp(idx).Channel = cell(1,NumChannel);
        EventListMAT.WindApp(idx).PMU = cell(1,NumChannel);
        for idx2 = 1:NumChannel
            EventListMAT.WindApp(idx).Channel{idx2} = EventListXML.WindApp{idx}.Channel{idx2}.Name;
            EventListMAT.WindApp(idx).PMU{idx2} = EventListXML.WindApp{idx}.Channel{idx2}.PMU;
        end
        
        NumWindPow = length(EventListXML.WindApp{idx}.WindPower);
        if NumWindPow == 1
            EventListXML.WindApp{idx}.WindPower = {EventListXML.WindApp{idx}.WindPower};
        end
        
        EventListMAT.WindApp(idx).WindPower = cell(1,NumWindPow);
        for idx2 = 1:NumWindPow
            EventListMAT.WindApp(idx).WindPower{idx2} = EventListXML.WindApp{idx}.WindPower{idx2}.Info;
        end
    end
end