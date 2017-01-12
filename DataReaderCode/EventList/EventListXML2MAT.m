function EventListMAT = EventListXML2MAT(EventListXML)

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
        EventListMAT.ForcedOscillation(idx).Amplitude = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).SNR = cell(1,NumOccurrences);
        EventListMAT.ForcedOscillation(idx).Coherence = cell(1,NumOccurrences);
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
            EventListMAT.ForcedOscillation(idx).Amplitude{idx2} = zeros(1,NumChannel);
            EventListMAT.ForcedOscillation(idx).SNR{idx2} = zeros(1,NumChannel);
            EventListMAT.ForcedOscillation(idx).Coherence{idx2} = zeros(1,NumChannel);
            for idx3 = 1:NumChannel
                EventListMAT.ForcedOscillation(idx).Channel{idx2}{idx3} = EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.Name;
                EventListMAT.ForcedOscillation(idx).Amplitude{idx2}(idx3) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.Amplitude);
                EventListMAT.ForcedOscillation(idx).SNR{idx2}(idx3) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.SNR);
                EventListMAT.ForcedOscillation(idx).Coherence{idx2}(idx3) = str2double(EventListXML.ForcedOscillation{idx}.Occurrence{idx2}.Channel{idx3}.Coherence);
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

% Translate the out-of-range frequency portion
if isfield(EventListXML,'OutOfRangeFrequency')
    EventListMAT.OutOfRangeFrequency = [];
    
    if length(EventListXML.OutOfRangeFrequency) == 1
        EventListXML.OutOfRangeFrequency = {EventListXML.OutOfRangeFrequency};
    end
    
    for idx = 1:length(EventListXML.OutOfRangeFrequency)
        EventListMAT.OutOfRangeFrequency(idx).ID = EventListXML.OutOfRangeFrequency{idx}.ID;
        EventListMAT.OutOfRangeFrequency(idx).Start = datenum(EventListXML.OutOfRangeFrequency{idx}.Start);
        EventListMAT.OutOfRangeFrequency(idx).End = datenum(EventListXML.OutOfRangeFrequency{idx}.End);
        
        NumChannel = length(EventListXML.OutOfRangeFrequency{idx}.Channel);
        if NumChannel == 1
            EventListXML.OutOfRangeFrequency{idx}.Channel = {EventListXML.OutOfRangeFrequency{idx}.Channel};
        end
        
        EventListMAT.OutOfRangeFrequency(idx).Channel = cell(1,NumChannel);
        EventListMAT.OutOfRangeFrequency(idx).PMU = cell(1,NumChannel);
        for idx2 = 1:NumChannel
            EventListMAT.OutOfRangeFrequency(idx).Channel{idx2} = EventListXML.OutOfRangeFrequency{idx}.Channel{idx2}.Name;
            EventListMAT.OutOfRangeFrequency(idx).PMU{idx2} = EventListXML.OutOfRangeFrequency{idx}.Channel{idx2}.PMU;
        end
    end
end