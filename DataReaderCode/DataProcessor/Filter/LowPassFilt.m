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
    % rational filter
        % Parameters.PassRipple: Passband ripple in dB
        % Parameters.StopRipple: Stopband ripple in dB     
        % Parameters.PassCutoff: Passband-edge frequency in Hz
        % Parameters.StopCutoff: Stopband-edge frequency in Hz
        % Parameters.ZeroPhase: if TRUE, the output of filter has zero
        % phase delay   
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMU = LowPassFilt(PMU,SigsToFilt,Parameters)

PassRipple  = str2num(Parameters.PassRipple);
StopRipple  = str2num(Parameters.StopRipple);
PassCutoff  = str2num(Parameters.PassCutoff);
StopCutoff  = str2num(Parameters.StopCutoff);
SetZeroPhase  = Parameters.ZeroPhase;
CutoffFreq = [PassCutoff StopCutoff];

t = PMU.Signal_Time.Time_String;
t1 = t{1};
Ind1 = findstr(t1, '.');
T1 = str2num(t1(Ind1:end));
t11 = t{11};
Ind11 = findstr(t11, '.');
T11 = str2num(t11(Ind1:end));
fs = round(10/(T11 - T1));

a=[1 0];
dev = [(10^(PassRipple/20)-1)/(10^(PassRipple/20)+1)  10^(-StopRipple/20)];
[nLP,fo,ao,w] = firpmord(CutoffFreq,a,dev,fs);
b = firpm(nLP,fo,ao,w);

% If specific signals were not listed, apply to all signals except 
% digitals, scalars, and rocof
if isempty(SigsToFilt)
    SigIdx = find(~strcmp(PMU.Signal_Type, 'D'));
    SigsToFilt = PMU.Signal_Name(SigIdx);
end

for SigIdx = 1:length(SigsToFilt)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToFilt{SigIdx}));
    
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToFilt{SigIdx} ' could not be found.']);
        continue
    end
    
    if strcmp(SetZeroPhase,'TRUE')
        PMU.Data(:,ThisSig) = filfilt(b,a,PMU.Data(:,ThisSig));
    else
        PMU.Data(:,ThisSig) = filter(b,a,PMU.Data(:,ThisSig));
    end  

end
