% function [PMU, FinalCondos] = POWpmuFilt(PMU,SigsToFilt,Parameters, InitialCondos)
% This function emulates PMU functionality by fitting point-on-wave (POW)
% measurements to a sinusoid with odd harmonics and a DC offset.
% 
% Inputs:
	% PMU: structure in the common format for a single PMU
        % PMU.Signal_Type: a cell array of strings specifying
        % signal(s) type in the PMU (size:1 by number of data channel)
        % PMU.Signal_Name: a cell array of strings specifying name of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMU.Signal_Unit: a cell array of strings specifying unit of
        % signal(s) in the PMU (size:1 by number of data channel)
        % PMU.Data: Matrix containing PMU measurements (size:
        % number of data points by number of channels in the PMU)
        % PMU.Flag: 3-dimensional matrix indicating PMU
        % measurements flagged by different filter operation (size: number 
        % of data points by number of channels by number of flag bits)  
        % PMU.Signal_Time.Time_String: a cell array of strings containing
        % time-stamp of PMU data
    % SigsToFilt: Unused because this function is a special case of
    % functions implemented as a filter.
    % Parameters: a struct array containing user defined parameters
        % Parameters.PA(PB,PC): Signal names for a, b, and c POW measurements
        % Parameters.ReportRate: Number of phasor reports per second
        % Parameters.WinLength: Number of samples in the analysis window
        % Parameters.SynchFreq: Synchronous frequency (50 or 60 Hz)
        % Parameters.(OutputSignalName) where OutputSignalName fields are
        % the following: PmagName, PangName, AmagName, AangName, AfitName,
        % BmagName, BangName, BfitName CmagName CangName CfitName Fname ROCOFname
    % InitialCondos: contains initial conditions (filters, angles) used to
    % provide continuity between files.
%
% Outputs:
    % PMU: structure with input signals replaced by output signals
    % FinalCondos: filter conditions and angles that will be used to
    % provide continuity when the next file is read.
%     
%Created by: Jim Follum (james.follum@pnnl.gov)

function [PMU, FinalCondos] = POWpmuFilt(PMU,~,Parameters, InitialCondos)

% Read the input POW data and record the signal types and units
InputType = cell(1,3);
InputUnit = cell(1,3);
[pa,InputType{1},InputUnit{1}] = RetrieveData(PMU,Parameters.PA);
[pb,InputType{2},InputUnit{2}] = RetrieveData(PMU,Parameters.PB);
[pc,InputType{3},InputUnit{3}] = RetrieveData(PMU,Parameters.PC);

% Reports per second
ReportRate = str2double(Parameters.ReportRate);
% Samples in window
C = str2double(Parameters.WinLength);
% Synchronous frequency
SynchFreq = str2double(Parameters.SynchFreq);

%% Setup
% Calculate signal's sampling rate
fs = round(1/mean((diff(PMU.Signal_Time.Signal_datenum)*24*60*60)));
% Sample step for reporting rate 
RR = round(fs/ReportRate);
% Odd harmonics to fit
H = 1:2:fs/2/60;
%
% Form initial S matrix. The S matrix is used in the equation theta = pinv(S)*x
% to estimate the sinusoid parameters in theta. S contains cos and sin
% terms. Rather than creating the S matrix for every report, an integer 
% number of cycles is generated here. Then indices of the S matrix are
% retrieved for use in generating each report. A simpler implementation is
% probably possible because files will have an integer number of seconds
% and the reporting rate is an integer.
%
% Number of electrical cycles in the window, rounded up
Ncyc = ceil(C/fs*SynchFreq);
% Has to be an integer number of cycles
Scyc = max([setdiff(factor(SynchFreq),factor(fs)) Ncyc]);
St = (0:Scyc/60*fs-1)/fs;
% Extra column is for a DC offset. Doesn't impact phasor estimation, but
% has slight impact on the RMSE.
Sinit = ones(length(St),2*length(H)+1);
for h = 1:length(H)
    Sinit(:,2*h-1) = cos(2*pi*60*H(h)*St);
    Sinit(:,2*h) = -sin(2*pi*60*H(h)*St);
end

% Index of end of analysis window
c = RR:RR:length(pa);
% Time stamps (center of window)
Tstamp = PMU.Signal_Time.datetime(c) - ((C+1)/2-1)/(fs*60*60*24);

% Append past data onto the POW measurements to provide continuity between
% files.
if isempty(InitialCondos)
    pa = [NaN(C-RR,1); pa];
    pb = [NaN(C-RR,1); pb];
    pc = [NaN(C-RR,1); pc];
else
    pa = [InitialCondos.Hist(:,1); pa];
    pb = [InitialCondos.Hist(:,2); pb];
    pc = [InitialCondos.Hist(:,3); pc];
end
FinalCondos.Hist = [pa(end-(C-RR)+1:end) pb(end-(C-RR)+1:end) pc(end-(C-RR)+1:end)];

% Adjust indices for data that was just added
c = c + (C-RR);

% Starting index for Sinit matrix. The starting index moves forward along with
% the analysis window. The S matrix is formed as S = [Sinit(Sidx:end,:); Sinit(1:Sidx-1,:)]
SstIdx = 1;
% Starting time vector used in generating reconstruction of input POW
xt0 = (0:C-1)/fs;
% Matrices to store phasors and RMSE for each of the three phases
X = zeros(length(c),3);
RMSE = zeros(length(c),3);
for cidx = 1:length(c)
    % Indices for analysis window
    idx = c(cidx) - (C-1:-1:0);
    
    % Form S matrix (see comments when Sinit was formed above)
    Sidx = SstIdx + (0:C-1);
    Sidx(Sidx > length(St)) = [];
    Sidx = [Sidx 1:(C-length(Sidx))];
    S = Sinit(Sidx,:);
    % Move the starting index forward by the reporting rate
    SstIdxNew = SstIdx + RR;
    % If starting index is beyond the size of Sinit, subtract the length of
    % Sinit until it is an appropriate index again
    if SstIdxNew > length(St)
        SstIdxNew = SstIdxNew - length(St);
    end
    SstIdx = SstIdxNew;
    
    % Estimate the parameters (magnitude and phase) of each sinusoid
    thetaA = pinv(S)*pa(idx);
    thetaB = pinv(S)*pb(idx);
    thetaC = pinv(S)*pc(idx);  
    
    % Reconstruct input waveforms and calculate RMSE
    xt = St(Sidx(1)) + xt0;
    % Start with DC offset, then iterate through harmonics and add them
    paHat = thetaA(end);
    pbHat = thetaB(end);
    pcHat = thetaC(end);
    % Go in reverse order so that the final A and P correspond to the
    % fundamental, which is recorded as the phasor estimate
    for h = length(H):-1:1
        Aa = sqrt(thetaA(2*h-1)^2 + thetaA(2*h)^2) / sqrt(2);
        Pa = atan2(thetaA(2*h),thetaA(2*h-1));
        paHat = paHat + Aa*sqrt(2)*cos(2*pi*60*xt + Pa).';
        
        Ab = sqrt(thetaB(2*h-1)^2 + thetaB(2*h)^2) / sqrt(2);
        Pb = atan2(thetaB(2*h),thetaB(2*h-1));
        pbHat = pbHat + Ab*sqrt(2)*cos(2*pi*60*xt + Pb).';
        
        Ac = sqrt(thetaC(2*h-1)^2 + thetaC(2*h)^2) / sqrt(2);
        Pc = atan2(thetaC(2*h),thetaC(2*h-1));
        pcHat = pcHat + Ac*sqrt(2)*cos(2*pi*60*xt + Pc).';
    end
    % Record phasor estimate for each phase in complex form
    X(cidx,1) = Aa*exp(1i*Pa);
    X(cidx,2) = Ab*exp(1i*Pb);
    X(cidx,3) = Ac*exp(1i*Pc);
    % Record RMSE for LS fit
    RMSE(cidx,1) = sqrt(mean((pa(idx) - paHat).^2));
    RMSE(cidx,2) = sqrt(mean((pb(idx) - pbHat).^2));
    RMSE(cidx,3) = sqrt(mean((pc(idx) - pcHat).^2));
end

% Calculate positive sequence
a = exp(1i*120*pi/180);
Xpos = 1/3*(X(:,1) + a*X(:,2) + a^2*X(:,3));

% Initialize filters used to estimate frequency and ROCOF
if isempty(InitialCondos)
    XposAngle = unwrap(angle(Xpos));
    
    [FrTemp, InitialCondos.Fr] = filter([6 -3 -2 -1],20*pi/ReportRate,XposAngle(end:-1:1));
    [~, InitialCondos.ROCOF] = filter([1 -1],1/ReportRate,FrTemp);
else
    % Continuity in the angle unwrap is necessary because of the memory of
    % the filter used to estimate frequency.
    XposAngle = unwrap([InitialCondos.XposAngle; angle(Xpos)]);
    XposAngle = XposAngle(2:end);
end

% Estimate frequency. Filter is from C37.118 standard
FinalCondos.XposAngle = XposAngle(end);
[Fr, FinalCondos.Fr] = filter([6 -3 -2 -1],20*pi/ReportRate,XposAngle,InitialCondos.Fr);
Fr = Fr + 60;

% Estimate ROCOF. Filter is from C37.118 standard
[ROCOF, FinalCondos.ROCOF] = filter([1 -1],1/ReportRate,Fr,InitialCondos.ROCOF);

% Store all output data. Undesired signals are removed later
OutputData = [abs(Xpos), angle(Xpos)*180/pi,...
    abs(X(:,1)), angle(X(:,1))*180/pi, RMSE(:,1),...
    abs(X(:,2)), angle(X(:,2))*180/pi, RMSE(:,2),...
    abs(X(:,3)), angle(X(:,3))*180/pi, RMSE(:,3),...
    Fr, ROCOF];

% Field names in configuration file that correspond to the output data
FieldNames = {'PmagName', 'PangName',...
    'AmagName', 'AangName', 'AfitName',...
    'BmagName', 'BangName', 'BfitName',...
    'CmagName', 'CangName', 'CfitName',...
    'Fname', 'ROCOFname'};

% Based on configuration, determine which signals are to be returned
% (OutputIndicator) and record all output signal names (OutputNames).
OutputNames = cell(1,length(FieldNames));
OutputIndicator = false(1,length(FieldNames));
for k = 1:length(FieldNames)
    if ~isempty(Parameters.(FieldNames{k}))
        OutputIndicator(k) = true;
        OutputNames{k} = Parameters.(FieldNames{k});
    end
end

% Determine whether the input POW signals were voltages or currents and
% assign signal types and units accordingly.
if contains(InputType,{'VWA','VWB','VWC'})
    % Inputs are voltage waveforms
    OutputTypes = {'VMP','VAP','VMA','VAA','OTHER','VMB','VAB','OTHER','VMC','VAC','OTHER','F','RCF'};
    
    if strcmp(InputUnit,'V')
        % All units are volts
        OutputUnits = {'V','DEG','V','DEG','O','V','DEG','O','V','DEG','O','Hz','Hz/sec'};
    elseif strcmp(InputUnit,'kV')
        % All units are kV
        OutputUnits = {'kV','DEG','kV','DEG','O','kV','DEG','O','kV','DEG','O','Hz','Hz/sec'};
    else
        % Units are not exclusively V or kV
        OutputTypes{:} = 'OTHER';
        OutputUnits = cell(1,length(FieldNames));
        OutputUnits{:} = 'O';
    end
elseif contains(InputType,{'IWA','IWB','IWC'})
    % Inputs are current waveforms
    OutputTypes = {'IMP','IAP','IMA','IAA','OTHER','IMB','IAB','OTHER','IMC','IAC','OTHER','F','RCF'};
    
    if strcmp(InputUnit,'A')
        % All units are amps
        OutputUnits = {'A','DEG','A','DEG','O','A','DEG','O','A','DEG','O','Hz','Hz/sec'};
    elseif strcmp(InputUnit,'kA')
        % All units are kA
        OutputUnits = {'kA','DEG','kA','DEG','O','kA','DEG','O','kA','DEG','O','Hz','Hz/sec'};
    else
        % Units are not exclusively A or kA
        OutputTypes{:} = 'OTHER';
        OutputUnits = cell(1,length(FieldNames));
        OutputUnits{:} = 'O';
    end
else
    % Inputs are not exclusively voltage or current waveforms
    OutputTypes = cell(1,length(FieldNames));
    OutputTypes{:} = 'OTHER';
    OutputUnits = cell(1,length(FieldNames));
    OutputUnits{:} = 'O';
end

% Limit output signals to those requested by the user
OutputNames = OutputNames(OutputIndicator);
OutputTypes = OutputTypes(OutputIndicator);
OutputUnits = OutputUnits(OutputIndicator);
OutputData = OutputData(:,OutputIndicator);

% Reform the PMU structure to include only the output signals.
PMU.Signal_Name = OutputNames;
PMU.Signal_Type = OutputTypes;
PMU.Signal_Unit = OutputUnits;
PMU.Data = OutputData;
PMU.Flag = false(size(PMU.Data,1),size(PMU.Data,2),size(PMU.Flag,3));
PMU.Stat = zeros(size(PMU.Data,1),1);
PMU.Signal_Time.Time_String = cellstr(datestr(Tstamp, 'yyyy-mm-dd HH:MM:SS.FFF'));
PMU.Signal_Time.Signal_datenum = datenum(Tstamp);
PMU.Signal_Time.datetime = Tstamp;

end

% This function retrieves the data, types, and units for input signals.
function [data,SigType,SigUnit] = RetrieveData(PMU,SigName)
    ThisSig = find(strcmp(PMU.Signal_Name,SigName));
    if isempty(ThisSig)
        error(['The signal ' SigName ' could not be found; point on wave PMU calculation cannot continue.']);
    elseif length(ThisSig) > 1
        error(['The signal ' SigName ' was found twice in the same PMU structure; point on wave PMU calculation cannot continue.']);
    else
        data = PMU.Data(:,ThisSig);
        SigType = PMU.Signal_Type{ThisSig};
        SigUnit = PMU.Signal_Unit{ThisSig};
    end
end