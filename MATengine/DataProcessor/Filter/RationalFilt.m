% function PMU = RationalFilt(PMU,SigsToFilt,Parameters)
% This function filters the given signal(s) with a user-specified rational filter
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
    % SigsToFilt: a cell array of strings specifying name of signals to be
    % filtered
    % Parameters: a struct array containing user defined paramters for
    % rational filter
        % Parameters.Numerator: numerator coefficient of filter
        % Parameters.Denominator: denominator coefficient of filter
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)


function [PMU, FinalCondos] = RationalFilt(PMU,SigsToFilt,Parameters, InitialCondos)

%Numerator and denominator of filter coefficients corresponding to
% the given user specified parameters
b_char = strsplit(Parameters.Numerator,',');
for nb = 1:length(b_char)
    b(nb) = str2num(b_char{nb});
end

a_char = strsplit(Parameters.Denominator,',');
for na = 1:length(a_char)
    a(na) = str2num(a_char{na});
end

% If specific signals were not listed, apply to all signals except 
% digitals
if isempty(SigsToFilt)
    SigIdx = find(~strcmp(PMU.Signal_Type, 'D'));
    SigsToFilt = PMU.Signal_Name(SigIdx);
end

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
    if length(a) > 1
        % This is an IIR filter, so the filter is reset after every set
        % of NaN values

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

            % Get initial conditions for the filter by filtering data with
            % a constant value equal to the first sample of Data if:
            %   No initial conditions are available
            %       OR
            %   This isn't the first clean group (it follows NaN values)
            %       OR
            %   This is the first clean group but it doesn't start at the
            %   first sample (it follows NaN values)
            if isempty(InitialCondos{SigIdx}.delays) || (CleanGroupIdx > 1) || ((CleanGroupIdx == 1) && (Starts(1)~=1))
                [~, InitialCondos{SigIdx}.delays] = filter(b,a,PMU.Data(Starts(CleanGroupIdx),ThisSig)*ones(ceil(max(grpdelay(b,a))),1));
            end

            % Filter this group of non-NaN values
            [PMU.Data(Starts(CleanGroupIdx):Ends(CleanGroupIdx),ThisSig), FinalCondos{SigIdx}.delays] = filter(b,a,PMU.Data(Starts(CleanGroupIdx):Ends(CleanGroupIdx),ThisSig), InitialCondos{SigIdx}.delays);
        end

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
    else
        % This is an FIR filter, so no special handling of NaNs is
        % necessary

        % Only use the initial conditions if the name of the channel is
        % correct
        if ~strcmp(SigsToFilt{SigIdx}, InitialCondos{SigIdx}.Name)
            InitialCondos{SigIdx}.delays = [];
        end
        FinalCondos{SigIdx}.Name = SigsToFilt{SigIdx};

        % If no initial conditions are available, get some by filtering data with a 
        % constant value equal to the first sample of Data.
        if isempty(InitialCondos{SigIdx}.delays)
            [~, InitialCondos{SigIdx}.delays] = filter(b,a,PMU.Data(1,ThisSig)*ones(ceil(max(grpdelay(b,a))),1));
        end

        [PMU.Data(:,ThisSig), FinalCondos{SigIdx}.delays] = filter(b,a,PMU.Data(:,ThisSig), InitialCondos{SigIdx}.delays);
    end
end