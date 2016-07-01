% function PMU = RationalFilt(PMU,SigsToFilt,Parameters)
% This function filters the given signal(s) with a median filter
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
    % median filter
        % Parameters.Order: Order of filter
        % Parameters.Endpoints: Gives information on handing
        % endpoints(either truncates or zeropads endpoints)        
        % Parameters.HandleNaN: Gives information on handing
        % nanpoints(either includes or omits NaN points))            
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMU = MedianFilt(PMU,SigsToFilt,Parameters)

FiltOrder  = str2num(Parameters.Order);
Endpoints  = Parameters.Endpoints;
HandleNaN  = Parameters.HandleNaN;

% If specific signals were not listed, apply to all signals except 
% digitals
if isempty(SigsToFilt)
    SigIdx = find(~strcmp(PMU.Signal_Type, 'D'));
    SigsToFilt = PMU.Signal_Name(SigIdx);
end

for SigIdx = 1:length(SigsToFilt)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToFilt{SigIdx}));
    if imag(PMU.Data(:,ThisSig))~=0
        warning(['Signal ' SigsToFilt{SigIdx} ' is complex valued. Median filter is not applied. Data not changed.']);
        continue
    end
    
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToFilt{SigIdx} ' could not be found.']);
        continue
    end
    % apply filter operation here
    PMU.Data(:,ThisSig) = medfilt1(PMU.Data(:,ThisSig),FiltOrder, HandleNaN, Endpoints);
end
