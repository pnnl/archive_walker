% function [PMU, FinalCondos] = RMSenergyFilt(PMU,SigsToFilt,Parameters,InitialCondos)
% This function calculates the RMS-energy for the input signals.
% Filter designs are based on the frequency- and impulse-response plots in
% Donnelly's paper.
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
    % the filter
        % Parameters.Band: a string specifying which of the four bands to
        % calculate the RMS-energy for. Options are 'Band 1', 'Band 2',
        % 'Band 3', and 'Band 4'
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function [PMU, FinalCondos] = RMSenergyFilt(PMU,SigsToFilt,Parameters,InitialCondos)

%User-specified parameters 
Band  = Parameters.Band;

%calculates signal's sampling rate
fs = round(1/mean((diff(PMU.Signal_Time.Signal_datenum)*24*60*60)));


FinalCondos = cell(1,length(SigsToFilt));
if isempty(InitialCondos)
    % First time that this code has been called
    
    % Initialize the initial conditions
    InitialCondos = cell(1,length(SigsToFilt));
    for SigIdx = 1:length(SigsToFilt)
        InitialCondos{SigIdx} = struct('Name',[],'delaysBP',[],'delaysLP',[]);
    end
    
    % Design filters for RMS calculation
    % Filter coefficients for all signals are stored in InitialCondos{1}
    [InitialCondos{1}.bBP, InitialCondos{1}.bLP] = DesignRMSfilters(Band,fs);
end

% Retrieve coefficients for easy reference
bBP = InitialCondos{1}.bBP;
bLP = InitialCondos{1}.bLP;

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
        InitialCondos{SigIdx}.delaysBP = [];
        InitialCondos{SigIdx}.delaysLP = [];
    end
    FinalCondos{SigIdx}.Name = SigsToFilt{SigIdx};
    
    
    % Band-pass filter

    % If no initial conditions are available, get some by filtering data with a 
    % constant value equal to the first sample of Data.
    if isempty(InitialCondos{SigIdx}.delaysBP)
        [~, InitialCondos{SigIdx}.delaysBP] = filter(bBP,1,PMU.Data(1,ThisSig)*ones(ceil(max(grpdelay(bBP,1))),1));
    end

    [PMU.Data(:,ThisSig), FinalCondos{SigIdx}.delaysBP] = filter(bBP,1,PMU.Data(:,ThisSig), InitialCondos{SigIdx}.delaysBP);
    
    
    % Square
    PMU.Data(:,ThisSig) = PMU.Data(:,ThisSig).^2;
    
    
    % Low-pass (moving average) filter
    % (It's okay if no initial conditions are available for this filter)
    [PMU.Data(:,ThisSig), FinalCondos{SigIdx}.delaysLP] = filter(bLP,1,PMU.Data(:,ThisSig), InitialCondos{SigIdx}.delaysLP);
    
    
    % Square root
    PMU.Data(:,ThisSig) = sqrt(PMU.Data(:,ThisSig));
end

% Transfer filter designs to FinalCondos
FinalCondos{1}.bBP = InitialCondos{1}.bBP;
FinalCondos{1}.bLP = InitialCondos{1}.bLP;