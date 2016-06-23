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
        % Parameters.ZeroPhase: if TRUE, the output of filter has zero
        % phase delay        
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)


function PMU = RationalFilt(PMU,SigsToFilt,Parameters)

b_char = strsplit(Parameters.Numerator,',');
for nb = 1:length(b_char)
    b(nb) = str2num(b_char{nb});
end

a_char = strsplit(Parameters.Denominator,',');
for na = 1:length(a_char)
    a(na) = str2num(a_char{na});
end

SetZeroPhase  = Parameters.ZeroPhase;


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
        PMU.Data(:,ThisSig) = filtfilt(b,a,PMU.Data(:,ThisSig));
    else
        PMU.Data(:,ThisSig) = filter(b,a,PMU.Data(:,ThisSig));
    end  
    

end
