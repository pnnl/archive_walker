%   function [DetectionResults, AdditionalOutput] = TheveninDetector(PMUstruct,Parameters,PastAdditionalOutput)
%   This function implemements the Thevenin application
%   Inputs:
%           PMUstruct: PMU structure in a common format for all PMUs
%           Parameters: 
%           PastAdditionalOutput:
%
%   Outputs:
%           DetectionResults: 
%           AdditionalOutput:
%
% Created by Jim Follum (james.follum@pnnl.gov) on 4/13/2018

function [DetectionResults, AdditionalOutput] = TheveninDetector(PMUstruct,Parameters,PastAdditionalOutput)

ResultUpdateInterval = Parameters.ResultUpdateInterval;

if isfield(Parameters,'AnalysisLength')
    % This detector operates on an analysis window. Detection is applied
    % based only on the most recent ResultUpdateInterval seconds of data so
    % that the same data isn't considered multiple times. 
    AnalysisLength = str2double(Parameters.AnalysisLength);
else
    % This detector operates sample by sample, rather than based on an
    % analysis window. Set the AnalysisLength parameter equal to 
    % ResultUpdateInterval so that only the new data is analyzed and used
    % for detection.
    AnalysisLength = ResultUpdateInterval;
end

if AnalysisLength < ResultUpdateInterval
    error('The analysis window cannot be shorter than the result update interval for Thevenin detectors.');
end

if isfield(Parameters,'EventMergeWindow')
    EventMergeWindowSec = str2num(Parameters.EventMergeWindow);
    if isnan(EventMergeWindowSec)
        error([Parameters.EventMergeWindow ' is not a valid entry for the Thevenin EventMergeWindow parameter.']);
    end
else
    EventMergeWindowSec = 0;
end

DetectionResults = struct('StartTime',cell(1,length(Parameters.Sub)),'EndTime',cell(1,length(Parameters.Sub)),'SubName',cell(1,length(Parameters.Sub)));
AdditionalOutput = struct('VbusMAG',cell(1,length(Parameters.Sub)),'VbusANG',cell(1,length(Parameters.Sub)),'E',cell(1,length(Parameters.Sub)),'Z',cell(1,length(Parameters.Sub)),'LTI',cell(1,length(Parameters.Sub)),'LTIthresh',cell(1,length(Parameters.Sub)),'fs',cell(1,length(Parameters.Sub)),'TimeString',cell(1,length(Parameters.Sub)));

if isempty(PastAdditionalOutput)
    % PastAdditionalOutput isn't available. Initialize it to the empty 
    % AdditionalOutput structure so that the code can run.
    PastAdditionalOutput = AdditionalOutput;
end

Data = [];
DataPMU = cell(1,length(Parameters.Sub));
DataChannel = cell(1,length(Parameters.Sub));
DataType = cell(1,length(Parameters.Sub));
DataUnit = cell(1,length(Parameters.Sub));

DataType(:) = {'LTI'};
DataUnit(:) = {Parameters.Method};

% If only one substation is specified, place configuration in cell to match
% formatting when more than one sub is specified.
if length(Parameters.Sub) == 1
    Parameters.Sub = {Parameters.Sub};
end

% For each substation to be considered
for SubIdx = 1:length(Parameters.Sub)
    % Retrieve the configuration information for this substation
    ThisSub = Parameters.Sub{SubIdx};
    
    fs = [];
    
    % Get frequency
    if isfield(ThisSub,'Freq')
        [f,~,~,fs] = RetrieveData(PMUstruct,ThisSub.Freq,fs);
        f = f.F;
    else
        f = NaN;
    end
    
    % Get a bus voltage
    if length(ThisSub.Vbus) == 1
        % One voltage was specified, use it as the bus voltage
        [Vbus, VbusUnits, TimeString, fs] = RetrieveData(PMUstruct,ThisSub.Vbus,fs);
        % If necessary, convert angle units to degrees
        if strcmp(VbusUnits.ANG,'RAD')
            Vbus.ANG = Vbus.ANG*180/pi;
            VbusUnits.ANG = 'DEG';
        end
    else
        % More than one voltage was specified. Average them to get the bus
        % voltage.
        Vphasor = 0;
        for Vidx = 1:length(ThisSub.Vbus)
            [VbusTemp,VbusUnits,TimeString, fs] = RetrieveData(PMUstruct,ThisSub.Vbus{Vidx}, fs);
            
            % Convert to polar for averaging
            if strcmp(VbusUnits.ANG,'RAD')
                % Angles are in radianse
                Vphasor = Vphasor + (VbusTemp.MAG .* exp(1i*VbusTemp.ANG))/length(ThisSub.Vbus);
            else
                % Angles are in degrees
                Vphasor = Vphasor + (VbusTemp.MAG .* exp(1i*VbusTemp.ANG*pi/180))/length(ThisSub.Vbus);
            end
        end
        % Convert to rectangular
        Vbus.MAG = abs(Vphasor);
        Vbus.ANG = angle(Vphasor)*180/pi;
    end
    
    
    % Get P, Q, Imag, and Iang for each branch
    if isfield(ThisSub,'Branch')
        if length(ThisSub.Branch) == 1
            % Place in cell for consistent indexing when length > 1
            ThisSub.Branch = {ThisSub.Branch};
        end
        
        Branch = cell(1,length(ThisSub.Branch));
        BranchUnits = cell(1,length(ThisSub.Branch));
        for BranchIdx = 1:length(ThisSub.Branch)
            [Branch{BranchIdx}, BranchUnits{BranchIdx}, ~, fs] = RetrieveData(PMUstruct,ThisSub.Branch{BranchIdx}, fs);
            
            % If current angles are in radians, convert to degrees
            if strcmp(BranchUnits{BranchIdx}.Iang,'RAD')
                Branch{BranchIdx}.Iang = Branch{BranchIdx}.Iang*180/pi;
                BranchUnits{BranchIdx}.Iang = 'DEG';
            end
            
            % If active and reactive power were not specified for the
            % branch, calculate them from the voltage and currents
            if ~isfield(Branch{BranchIdx},'P')
                Branch{BranchIdx}.P = Vbus.MAG .* Branch{BranchIdx}.Imag .* cosd(Vbus.ANG - Branch{BranchIdx}.Iang) * 3/1000000;
                Branch{BranchIdx}.Q = Vbus.MAG .* Branch{BranchIdx}.Imag .* sind(Vbus.ANG - Branch{BranchIdx}.Iang) * 3/1000000;
                
                BranchUnits{BranchIdx}.P = 'MW';
                BranchUnits{BranchIdx}.Q = 'MVAR';
            end
        end
    else
        % No branches are specified
        Branch = {};
        BranchUnits = {};
    end
    
    
    % Get P, Q, Imag, and Iang for each shunt
    if isfield(ThisSub,'Shunt')
        if length(ThisSub.Shunt) == 1
            % Place in cell for consistent indexing when length > 1
            ThisSub.Shunt = {ThisSub.Shunt};
        end
        
        Shunt = cell(1,length(ThisSub.Shunt));
        ShuntUnits = cell(1,length(ThisSub.Shunt));
        for ShuntIdx = 1:length(ThisSub.Shunt)
            [Shunt{ShuntIdx}, ShuntUnits{ShuntIdx}, ~, fs] = RetrieveData(PMUstruct,ThisSub.Shunt{ShuntIdx},fs);
            
            % If current angles are in radians, convert to degrees
            if strcmp(ShuntUnits{ShuntIdx}.Iang,'RAD')
                Shunt{ShuntIdx}.Iang = Shunt{ShuntIdx}.Iang*180/pi;
                ShuntUnits{ShuntIdx}.Iang = 'DEG';
            end
            
            % If active and reactive power were not specified for the
            % shunt, calculate them from the voltage and currents
            if ~isfield(Shunt{ShuntIdx},'P')
                Shunt{ShuntIdx}.P = Vbus.MAG .* Shunt{ShuntIdx}.Imag .* cosd(Vbus.ANG - Shunt{ShuntIdx}.Iang) * 3/1000000;
                Shunt{ShuntIdx}.Q = Vbus.MAG .* Shunt{ShuntIdx}.Imag .* sind(Vbus.ANG - Shunt{ShuntIdx}.Iang) * 3/1000000;
                
                ShuntUnits{ShuntIdx}.P = 'MW';
                ShuntUnits{ShuntIdx}.Q = 'MVAR';
            end
        end
    else
        % No shunts are specified
        Shunt = {};
        ShuntUnits = {};
    end
    
    
    % Sort branches and shunts into sinks and sources and get the total P
    % and Q for each
    SinkP = 0;
    SinkQ = 0;
    SourceP = 0;
    SourceQ = 0;
    SourceI = 0;
    %
    % Shunts are always considered sinks
    for ShuntIdx = 1:length(Shunt)
        SinkP = SinkP + Shunt{ShuntIdx}.P;
        SinkQ = SinkQ + Shunt{ShuntIdx}.Q;
    end
    ShuntQ = SinkQ;
    %
    % Iterate through each branch. If P is leaving the substation 
    % (positive), the branch is a sink. If P is entering the substation 
    % (negative), the branch is a source.
    for BranchIdx = 1:length(Branch)
        if median(Branch{BranchIdx}.P,'omitnan') > 0
            % P is positive -> Sink
            SinkP = SinkP + Branch{BranchIdx}.P;
            SinkQ = SinkQ + Branch{BranchIdx}.Q;
        else
            % P is negative -> Source
            SourceP = SourceP + Branch{BranchIdx}.P;
            SourceQ = SourceQ + Branch{BranchIdx}.Q;
            SourceI = SourceI + Branch{BranchIdx}.Imag.*exp(1i*Branch{BranchIdx}.Iang*pi/180);
        end
    end
    SourceImag = abs(SourceI);
    SourceIang = angle(SourceI)*180/pi;
    
    if isempty(fs) || isnan(fs)
        error('The sampling rate was not successfully calculated');
    end
    
    % Indices of samples used to estimate E and Z
    AnalysisIdx = 1:length(Vbus.MAG);
    AnalysisIdx = AnalysisIdx(end-AnalysisLength*fs+1:end);
    %
    %
    Vbus.MAG = Vbus.MAG(AnalysisIdx);
    %
    Vbus.ANG = Vbus.ANG(AnalysisIdx);
    %
    if length(f) == 1
        f = f*ones(length(AnalysisIdx),1);
    else
        f = f(AnalysisIdx);
    end
    %
    if length(SourceP) == 1
        SourceP = SourceP*ones(length(AnalysisIdx),1);
    else
        SourceP = SourceP(AnalysisIdx);
    end
    %
    if length(SourceQ) == 1
        SourceQ = SourceQ*ones(length(AnalysisIdx),1);
    else
        SourceQ = SourceQ(AnalysisIdx);
    end
    %
    if length(SinkP) == 1
        SinkP = SinkP*ones(length(AnalysisIdx),1);
    else
        SinkP = SinkP(AnalysisIdx);
    end
    %
    if length(SinkQ) == 1
        SinkQ = SinkQ*ones(length(AnalysisIdx),1);
    else
        SinkQ = SinkQ(AnalysisIdx);
    end
    %
    if length(ShuntQ) == 1
        ShuntQ = ShuntQ*ones(length(AnalysisIdx),1);
    else
        ShuntQ = ShuntQ(AnalysisIdx);
    end
    %
    TimeString = TimeString(AnalysisIdx);
    
    % Use the P and Q from the sink lines to calculate a load/sink
    % impedance (unit is ohms)
%     Zload = (Vbus.MAG.^2)./((SinkP - 1i*SinkQ)/3)*10^6;
    %
    % Switching to using the souce lines to match the Tellegen's paper.
    % This shouldn't make any difference because power into the substation
    % should match power coming out of the substation.
    Zload = (Vbus.MAG.^2)./((SourceP - 1i*SourceQ)/3)*10^6;
    
    % Convert voltage magnitude from L-N in V to L-L in kV
    Vbus.MAG = Vbus.MAG*sqrt(3)/1000;
    Vbus.ANG = Vbus.ANG+30;
    
    Meas = [Vbus.MAG, Vbus.ANG, SourceP, SourceQ, f];
    
    % Perform analysis using the analysis window
    switch Parameters.Method
        case 'DeMarco'
            [E, Z] = DeMarco2_3c_LNout(Meas, fs);
        case 'Mitsubishi'
            [E, Z] = Mitsubishi3_LNout(Meas, fs);
        case 'Quanta'
            [E, Z, AdditionalOutput(SubIdx).PastVals] = Quanta_LNout3(Meas, fs, PastAdditionalOutput(SubIdx));
        case 'RPI'
            [E, Z] = RPI(Meas, fs);
        case 'Tellegen'
            [E, Z, AdditionalOutput(SubIdx).PastVals, Zload] = Tellegen_LNout2(Meas, fs, Zload, PastAdditionalOutput(SubIdx));
        otherwise
            error([Parameters.Method ' is not a valid Thevenin estimation method. Select DeMarco, Mitsubishi, Quanta, RPI, or Tellegen.']);
    end
    % Perform detection based on the most recent ResultUpdateInterval
    % seconds of data.
    % Also trim everything that gets stored (Data, DataPMU, ...,
    % AdditionalOutput entries to ResultUpdateInterval seconds.
    DetectionWindowIdx = length(Vbus.MAG)-ResultUpdateInterval*fs+1:length(Vbus.MAG);
    
    Vbus.MAG = Vbus.MAG(DetectionWindowIdx);
    Vbus.ANG = Vbus.ANG(DetectionWindowIdx);
    SourceP = SourceP(DetectionWindowIdx);
    SourceQ = SourceQ(DetectionWindowIdx);
    f = f(DetectionWindowIdx);
    ShuntQ = ShuntQ(DetectionWindowIdx);
    SinkQ = SinkQ(DetectionWindowIdx);
    TimeString = TimeString(DetectionWindowIdx);
    E = E(DetectionWindowIdx);
    Z = Z(DetectionWindowIdx);
    Zload = Zload(DetectionWindowIdx);
    
    
    % Local Thevenin Index 
%     LTI = abs(Z./Zload);
    LTI = (abs(Zload) - abs(Z))./abs(Zload);
    
    % Perform detection
    EventMergeWindow = EventMergeWindowSec*fs;
    StartTime = {};
    EndTime = {};
    DetIdx = find(LTI < str2num(ThisSub.LTIthresh));
    if ~isempty(DetIdx)
        JumpIdx = find(diff(DetIdx)-1 > EventMergeWindow);
        st = [DetIdx(1); DetIdx(JumpIdx)];
        en = [DetIdx(JumpIdx+1); DetIdx(end)];
        
        for GroupIdx = 1:length(st)
            StartTime{length(StartTime)+1} = TimeString{st(GroupIdx)};
            EndTime{length(EndTime)+1} = TimeString{en(GroupIdx)};
        end
    end
    
    DetectionResults(SubIdx).StartTime = StartTime;
    DetectionResults(SubIdx).EndTime = EndTime;
    DetectionResults(SubIdx).SubName = ThisSub.Name;
    
    Data = [Data LTI];
    DataPMU{SubIdx} = ThisSub.Name;
    DataChannel{SubIdx} = ThisSub.Name;
    
    AdditionalOutput(SubIdx).fs = fs;
    AdditionalOutput(SubIdx).LTI = LTI;
    AdditionalOutput(SubIdx).LTIthresh = str2num(ThisSub.LTIthresh);
    AdditionalOutput(SubIdx).E = E;
    AdditionalOutput(SubIdx).Z = Z;
    AdditionalOutput(SubIdx).VbusMAG = Vbus.MAG;
    AdditionalOutput(SubIdx).VbusANG = Vbus.ANG;
    AdditionalOutput(SubIdx).SourceP = SourceP;
    AdditionalOutput(SubIdx).SourceQ = SourceQ;
%     AdditionalOutput(SubIdx).f = f;
%     AdditionalOutput(SubIdx).ShuntQ = ShuntQ;
%     AdditionalOutput(SubIdx).SinkQ = SinkQ;
    AdditionalOutput(SubIdx).TimeString = TimeString;
%     AdditionalOutput(SubIdx).fs = fs;
end

AdditionalOutput(1).Data = Data;
AdditionalOutput(1).DataPMU = DataPMU;
AdditionalOutput(1).DataChannel = DataChannel;
AdditionalOutput(1).DataType = DataType;
AdditionalOutput(1).DataUnit = DataUnit;
AdditionalOutput(1).Method = Parameters.Method;

end



function [Data,Units,TimeString,fs] = RetrieveData(PMU,DataConfig,fs)

names = setdiff(fieldnames(DataConfig),'PMU');
Data = struct();
Units = struct();

PMUidx = find(strcmp({PMU.PMU_Name},DataConfig.PMU));

if isempty(PMUidx)
    TimeString = {};
else
    TimeString = PMU(PMUidx).Signal_Time.Time_String;
    
    % If fs hasn't been obtained yet, attempt to calculate it. Otherwise,
    % just return the input value.
    if isempty(fs) || isnan(fs)
        try
            % Rounds to the nearest 0.1 Hz
            fs = round(1/mean((diff(PMU(PMUidx).Signal_Time.Signal_datenum)*24*60*60))*10)/10;
        catch
            fs = [];
        end
    end
end

for idx = 1:length(names)
    if isempty(PMUidx)
        % PMU could not be found, return empty
        ChanIdx = [];
    else
        ChanIdx = find(strcmp(PMU(PMUidx).Signal_Name,DataConfig.(names{idx})));
    end
    
    if ~isempty(ChanIdx)
        % PMU and Channel were found
        Data.(names{idx}) = PMU(PMUidx).Data(:,ChanIdx);
        Units.(names{idx}) = PMU(PMUidx).Signal_Unit{ChanIdx};
    else
        % PMU and channel were not found
        error(['Channel with PMU ' DataConfig.PMU ' and name ' DataConfig.(names{idx}) ' could not be found.']);
    end
end

end