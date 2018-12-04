% function PMU = HighPassFilt(PMU,SigsToFilt,Parameters)
% This function filters the given signal(s) with a highpass filter
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
    % highpass filter
        % Parameters.Order: Order of filter   
        % Parameters.Cutoff: Cutoff frequency in Hz
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function [PMU, FinalCondos] = FrequencyDerivationFilt(PMU,SigsToFilt,InitialCondos)

%calculates signal's sampling rate
fs = round(1/mean((diff(PMU.Signal_Time.Signal_datenum)*24*60*60)));

b = [1 -1];
a = 1;

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
    
    
    if sum(strcmp(PMU.Signal_Type{ThisSig},{'VAP','VAA','VAB','VAC'})) > 0
        % Signal is a voltage angle signal - get units
        switch PMU.Signal_Unit{ThisSig}
            case 'DEG'
                scale = fs/360;
                if max(abs(diff(PMU.Data(:,ThisSig)))) > 90
                    warning('Angles should be unwrapped before being passed into the frequency derivation filter.');
                end
            case 'RAD'
                scale = fs/(2*pi);
                if max(abs(diff(PMU.Data(:,ThisSig)))) > pi
                    warning('Angles should be unwrapped before being passed into the frequency derivation filter.');
                end
            otherwise
                warning('Units for input signal not DEG or RAD, output signal will not be properly scaled.');
                scale = 1;
        end
    else
        % Warn user that input signal is not a voltage angle
        scale = 1;
        warning('Frequency derivation is intended for application to voltage angle signals. Output signal will not be properly scaled.');
    end
    
    % apply filter operation here
    %
    % Find all groups of non-NaN values
    nonNaNloc = ~isnan(PMU.Data(:,ThisSig));
    Starts = find(diff([0; nonNaNloc]) == 1);
    Ends = find(diff(nonNaNloc) == -1);
    if length(Starts) > length(Ends)
        Ends = [Ends; length(PMU.Data(:,ThisSig))];
    end

    % For each group of clean data
    for CleanGroupIdx = 1:length(Starts)
        % Only use the initial conditions if the name of the channel is
        % correct
        if ~strcmp(SigsToFilt{SigIdx}, InitialCondos{SigIdx}.Name)
            InitialCondos{SigIdx}.delays = [];
        end
        
        % Filter this group of non-NaN values
        [PMU.Data(Starts(CleanGroupIdx):Ends(CleanGroupIdx),ThisSig), FinalCondos{SigIdx}.delays] = filter(scale*b,a,PMU.Data(Starts(CleanGroupIdx):Ends(CleanGroupIdx),ThisSig), InitialCondos{SigIdx}.delays);
        
        % Replace the first sample with NaN if:
        %   No initial conditions are available
        %       OR
        %   This isn't the first clean group (it follows NaN values)
        %       OR
        %   This is the first clean group but it doesn't start at the
        %   first sample (it follows NaN values)
        if isempty(InitialCondos{SigIdx}.delays) || (CleanGroupIdx > 1) || ((CleanGroupIdx == 1) && (Starts(1)~=1))
            PMU.Data(Starts(CleanGroupIdx),ThisSig) = NaN;
        end
    end
    
    % Set signal type and unit to reflect the derivation of a frequency
    % deviation signal
    PMU.Signal_Type{ThisSig} = 'F';
    PMU.Signal_Unit{ThisSig} = 'Hz';

    % Set FinalCondos to [] so that the filter resets when the next
    % file is processed if:
    %   There were no groups of non-NaN values (Ends==[])
    %       OR
    %   The last group of non-NaN values does not coincide with the 
    %   last sample of this signal, indicating that there are NaNs at 
    %   the end of the signal.
    if isempty(Ends)
        FinalCondos{SigIdx}.delays = [];
    else
        if Ends(end) ~= length(PMU.Data(:,ThisSig))
            FinalCondos{SigIdx}.delays = [];
        end
    end

    FinalCondos{SigIdx}.Name = SigsToFilt{SigIdx};
end