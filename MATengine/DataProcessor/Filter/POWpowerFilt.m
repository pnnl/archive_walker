% function PMU = LowPassFilt(PMU,SigsToFilt,Parameters)
% This function filters the given signal(s) with a low pass filter
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
    % SigsToFilt: a cell array of strings specifying name of signals to be
    % filtered
    % Parameters: a struct array containing user defined paramters for
    % lowpass filter
        % Parameters.PassRipple: Passband ripple in dB
        % Parameters.StopRipple: Stopband ripple in dB     
        % Parameters.PassCutoff: Passband-edge frequency in Hz
        % Parameters.StopCutoff: Stopband-edge frequency in Hz
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function [PMU, FinalCondos] = POWpowerFilt(PMU,~,Parameters, InitialCondos)

SigIdx = zeros(1,6);
[va,SigIdx(1)] = RetrieveData(PMU,Parameters.VA);
[vb,SigIdx(2)] = RetrieveData(PMU,Parameters.VB);
[vc,SigIdx(3)] = RetrieveData(PMU,Parameters.VC);
[ia,SigIdx(4)] = RetrieveData(PMU,Parameters.IA);
[ib,SigIdx(5)] = RetrieveData(PMU,Parameters.IB);
[ic,SigIdx(6)] = RetrieveData(PMU,Parameters.IC);

thetaV = str2num(Parameters.PhaseShiftV)*pi/180;
thetaI = str2num(Parameters.PhaseShiftI)*pi/180;

%calculates signal's sampling rate
fs = round(1/mean((diff(PMU.Signal_Time.Signal_datenum)*24*60*60)));
% Time vector for DQ tranformation
time = (0:length(va)-1)'/fs;

% DQ transformation for voltage
vd = (sqrt(2)/ sqrt(3) * ( va.*cos(120*pi*time - thetaV) + vb.*cos(120*pi*time - 2*pi/3 - thetaV) + vc.*cos(120*pi*time + 2*pi/3 - thetaV)));  
vq = (sqrt(2)/ sqrt(3) * ( va.*sin(120*pi*time - thetaV) + vb.*sin(120*pi*time - 2*pi/3 - thetaV) + vc.*sin(120*pi*time + 2*pi/3 - thetaV)));  
% DQ transformation for current
id = sqrt(2)/3 * ( ia.*cos(120*pi*time - thetaI) + ib.*cos(120*pi*time - 2*pi/3 - thetaI) + ic.*cos(120*pi*time + 2*pi/3 - thetaI));  
iq = sqrt(2)/3 * ( ia.*sin(120*pi*time - thetaI) + ib.*sin(120*pi*time - 2*pi/3 - thetaI) + ic.*sin(120*pi*time + 2*pi/3 - thetaI));  

% Number of samples for running average
N = str2num(Parameters.WindowLength)*fs;

b = 1/N*ones(N,1);

FinalCondos = cell(1,4);
if isempty(InitialCondos)
    InitialCondos = cell(1,4);
    for idx = 1:4
        [~,InitialCondos{idx}] = filter(b, 1, vd(1)*ones(N,1));
    end
end

[vd,FinalCondos{1}] = filter(b, 1, vd, InitialCondos{1});
[vq,FinalCondos{2}] = filter(b, 1, vq, InitialCondos{2});
[id,FinalCondos{3}] = filter(b, 1, id, InitialCondos{3});
[iq,FinalCondos{4}] = filter(b, 1, iq, InitialCondos{4});

S = (vd+1i*vq).*(id-1i*iq);

P = sqrt(3)*real(S);
Q = -sqrt(3)*imag(S);

% Add the P and Q signals to the PMU
PMU.Signal_Name = [PMU.Signal_Name Parameters.Pname Parameters.Qname];
PMU.Signal_Type = [PMU.Signal_Type 'OTHER' 'OTHER'];
PMU.Signal_Unit = [PMU.Signal_Unit 'O' 'O'];
PMU.Flag = [PMU.Flag logical(sum(PMU.Flag(:,SigIdx,:),2)) logical(sum(PMU.Flag(:,SigIdx,:),2))];
PMU.Data = [PMU.Data P Q];

% Remove the input signals from the PMU (it's a custom PMU that was created
% in DPfilterStep) 
PMU.Signal_Name(SigIdx) = [];
PMU.Signal_Type(SigIdx) = [];
PMU.Signal_Unit(SigIdx) = [];
PMU.Flag(:,SigIdx,:) = [];
PMU.Data(:,SigIdx) = [];

end

function [data,ThisSig] = RetrieveData(PMU,SigName)
    ThisSig = find(strcmp(PMU.Signal_Name,SigName));
    if isempty(ThisSig)
        error(['The signal ' Parameters.VA ' could not be found; point on wave power calculation cannot continue.']);
    elseif length(ThisSig) > 1
        error(['The signal ' Parameters.VA ' was found twice in the same PMU structure; point on wave power calculation cannot continue.']);
    else
        data = PMU.Data(:,ThisSig);
    end
end