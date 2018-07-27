% function RerunOut = RerunThevenin(RerunStartTime,RerunEndTime,ConfigFile,ControlPath)
%
% This function reruns the Thevenin detector so that detailed
% results can be displayed in the GUI.
%
% Inputs:
% RerunStartTime = start time for the run in the format MM/DD/YYYY HH:MM:SS
% RerunEndTime = end time for the run in the format MM/DD/YYYY HH:MM:SS
% ConfigFile = path to the configuration file
% ControlPath = path to the folder with control files
%
% Example inputs:
% RerunStartTime = '08/21/2016 23:55:30';
% RerunEndTime = '08/22/2016 00:03:52';
% ConfigFile = 'C:\Users\foll154\Documents\BPAoscillationApp\CodeForProject2\DataReaderCode\ConfigXML\RerunTest.xml'

function RerunOut = RerunThevenin(RerunStartTime,RerunEndTime,ConfigFile,ControlPath,EventPath,InitializationPath,PredictionDelay)

RerunDetector = 'Thevenin';

% Rerun the general out-of-range detector for the specified time period
[~, AdditionalOutputRerun] = BAWS_main9(ControlPath,EventPath,InitializationPath,ConfigFile, RerunStartTime, RerunEndTime, RerunDetector);

% If the rerun did not return any results return an empty structure.
if isempty(AdditionalOutputRerun)
    % The rerun did not return any results
    init = cell(1,0);
    RerunOut = struct('t',init,'LTI',init,'LTIthresh',init,'E',init, 'Z',init,'VbusMAG',init,'VbusANG',init,'SourceP',init,'SourceQ',init,'Vhat',init,'DataPMU',init,'Method',init);
    
    return
end
%
% Initialize the output structure
% Find out the number of Thevenin detectors
Ndet = 0;
for DetIdx = 1:length(AdditionalOutputRerun{1})
    if isfield(AdditionalOutputRerun{1},RerunDetector)
        if ~isempty(AdditionalOutputRerun{1}(DetIdx).(RerunDetector))
            Ndet = Ndet + 1;
        else
            break;
        end
    else
        break;
    end
end
init = cell(1,Ndet);
RerunOut = struct('t',init,'LTI',init,'LTIthresh',init,'E',init, 'Z',init,'VbusMAG',init,'VbusANG',init,'SourceP',init,'SourceQ',init,'Vhat',init,'DataPMU',init,'Method',init);

RerunStartTimeDT = datetime(RerunStartTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
RerunEndTimeDT = datetime(RerunEndTime,'InputFormat','MM/dd/yyyy HH:mm:ss');
% For each general out-of-range detector
for DetIdx = 1:Ndet
    LTIthresh = [];
    fs = [];
    % For each entry in AdditionalOutputRerun, which correspond to 
    % files of PMU data, store the relevant values
    for FileIdx = 1:length(AdditionalOutputRerun)
        RerunOut(DetIdx).t = [RerunOut(DetIdx).t; datetime(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).TimeString,'InputFormat','yyyy-MM-dd HH:mm:ss.SSS')];
        
        E = [];
        Z = [];
        LTI = [];
        VbusMAG = [];
        VbusANG = [];
        SourceP = [];
        SourceQ = [];
        for SubIdx = 1:length(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector))      
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).E)
                E = [E AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).E];
            else
                E = [E NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
            
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).Z)
                Z = [Z AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).Z];
            else
                Z = [Z NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
            
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).LTI)
                LTI = [LTI AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).LTI];
            else
                LTI = [LTI NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
            
            if FileIdx == 1
                if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).LTIthresh)
                    LTIthresh = [LTIthresh AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).LTIthresh];
                else
                    LTIthresh = [LTIthresh NaN];
                end
                
                if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).fs)
                    fs = [fs AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).fs];
                else
                    fs = [fs NaN];
                end
            end
            
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).VbusMAG)
                VbusMAG = [VbusMAG AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).VbusMAG];
            else
                VbusMAG = [VbusMAG NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
            
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).VbusANG)
                VbusANG = [VbusANG AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).VbusANG];
            else
                VbusANG = [VbusANG NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
            
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).SourceP)
                SourceP = [SourceP AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).SourceP];
            else
                SourceP = [SourceP NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
            
            if ~isempty(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).SourceQ)
                SourceQ = [SourceQ AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(SubIdx).SourceQ];
            else
                SourceQ = [SourceQ NaN(size(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector)(1).Data,1),1)];
            end
        end
        RerunOut(DetIdx).E = [RerunOut(DetIdx).E; E];       
        RerunOut(DetIdx).Z = [RerunOut(DetIdx).Z; Z];
        RerunOut(DetIdx).LTI = [RerunOut(DetIdx).LTI; LTI];
        RerunOut(DetIdx).VbusMAG = [RerunOut(DetIdx).VbusMAG; VbusMAG];
        RerunOut(DetIdx).VbusANG = [RerunOut(DetIdx).VbusANG; VbusANG];
        RerunOut(DetIdx).SourceP = [RerunOut(DetIdx).SourceP; SourceP];
        RerunOut(DetIdx).SourceQ = [RerunOut(DetIdx).SourceQ; SourceQ];
    end
    
    RerunOut(DetIdx).LTIthresh = LTIthresh;

    % Limit times to those specified by the inputs
    KeepIdx = (RerunStartTimeDT <= RerunOut(DetIdx).t) & (RerunOut(DetIdx).t <= RerunEndTimeDT);
    RerunOut(DetIdx).t = datenum(RerunOut(DetIdx).t(KeepIdx));
    RerunOut(DetIdx).E = RerunOut(DetIdx).E(KeepIdx,:);
    RerunOut(DetIdx).Z = RerunOut(DetIdx).Z(KeepIdx,:);
    RerunOut(DetIdx).LTI = RerunOut(DetIdx).LTI(KeepIdx,:);
    RerunOut(DetIdx).VbusMAG = RerunOut(DetIdx).VbusMAG(KeepIdx,:);
    RerunOut(DetIdx).VbusANG = RerunOut(DetIdx).VbusANG(KeepIdx,:);
    RerunOut(DetIdx).SourceP = RerunOut(DetIdx).SourceP(KeepIdx,:);
    RerunOut(DetIdx).SourceQ = RerunOut(DetIdx).SourceQ(KeepIdx,:);
    
    % Run Power Flow 
    Vhat = [];
    if ~isempty(PredictionDelay)
        for SubIdx = 1:length(AdditionalOutputRerun{FileIdx}(DetIdx).(RerunDetector))
            PredictionDelaySamp = PredictionDelay*fs(SubIdx);
            
            Psch = (0+RerunOut(DetIdx).SourceP(PredictionDelaySamp+1:end,SubIdx));
            Qsch = (0+RerunOut(DetIdx).SourceQ(PredictionDelaySamp+1:end,SubIdx));
            eps = 10^-3;
            ItrMax = 100;
            
            PredIdx = 1:size(RerunOut(DetIdx).E,1)-PredictionDelaySamp;
            [VhatTemp,ItrMaxTemp] = GSPF_puLLout(abs(RerunOut(DetIdx).E(PredIdx,SubIdx)),Psch,Qsch,RerunOut(DetIdx).Z(PredIdx,SubIdx),RerunOut(DetIdx).E(PredIdx,SubIdx),eps,ItrMax);
            Vhat = [Vhat [NaN(PredictionDelaySamp,1); VhatTemp]];
        end
    end
    RerunOut(DetIdx).Vhat = Vhat;

    % Add parameters that are constant across time
    RerunOut(DetIdx).DataPMU = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataPMU;
    RerunOut(DetIdx).Method = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).Method;
%     RerunOut(DetIdx).DataChannel = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataChannel;
%     RerunOut(DetIdx).DataType = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataType;
%     RerunOut(DetIdx).DataUnit = AdditionalOutputRerun{1}(DetIdx).(RerunDetector)(1).DataUnit;
end