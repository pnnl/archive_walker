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

function [PMU, FinalCondos] = LowPassFilt(PMU,SigsToFilt,Parameters, InitialCondos)

%User-specified parameters 
PassRipple  = str2num(Parameters.PassRipple);
StopRipple  = str2num(Parameters.StopRipple);
PassCutoff  = str2num(Parameters.PassCutoff);
StopCutoff  = str2num(Parameters.StopCutoff);
CutoffFreq = [PassCutoff StopCutoff]; % in Hz

%calculates signal's sampling rate
fs = round(1/mean((diff(PMU.Signal_Time.Signal_datenum)*24*60*60)));

if StopCutoff>fs
    error('Cut-off frequencies exceed folding frequency.');
end

%gives numerator and denominator of filter coefficients corresponding to
%the given user specified parameters
a=[1 0];
dev = [(10^(PassRipple/20)-1)/(10^(PassRipple/20)+1)  10^(-StopRipple/20)];
[nLP,fo,ao,w] = firpmord(CutoffFreq,a,dev,fs);
b = firpm(nLP,fo,ao,w);

% If specific signals were not listed, apply to all signals except 
% digitals
if isempty(SigsToFilt)
    SigIdx = find(~strcmp(PMU.Signal_Type, 'D'));
    SigsToFilt = PMU.Signal_Name(SigIdx);
end
% freqz(b,1,1024,fs)

FinalCondos = cell(1,length(SigsToFilt));
if isempty(InitialCondos)
    InitialCondos = cell(1,length(SigsToFilt));
    for SigIdx = 1:length(SigsToFilt)
        InitialCondos{SigIdx} = struct('Name',[],'delays',[]);
    end
end
for SigIdx = 1:length(SigsToFilt)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToFilt{SigIdx}));
    
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToFilt{SigIdx} ' could not be found.']);
        continue
    end
    
    % apply filter operation here
    %
    % Only use the initial conditions if the name of the channel is
    % correct
    if ~strcmp(SigsToFilt{SigIdx}, InitialCondos{SigIdx}.Name)
        InitialCondos{SigIdx}.delays = [];
    end
    FinalCondos{SigIdx}.Name = SigsToFilt{SigIdx};

    % If no initial conditions are available, get some by filtering data with a 
    % constant value equal to the first sample of Data.
    if isempty(InitialCondos{SigIdx}.delays)
        [~, InitialCondos{SigIdx}.delays] = filter(b,1,PMU.Data(1,ThisSig)*ones(ceil(max(grpdelay(b,1))),1));
    end

    [PMU.Data(:,ThisSig), FinalCondos{SigIdx}.delays] = filter(b,1,PMU.Data(:,ThisSig), InitialCondos{SigIdx}.delays);
end