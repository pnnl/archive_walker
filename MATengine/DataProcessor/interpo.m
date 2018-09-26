% function PMUstruct = interpo(PMUstruct,Interpolate)
% This function interpolates missing data, and flagged data only if user
% specifies.
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
    % Interpolate: a cell array of user provided information
        % Interpolate.Type: Type of interpolation
        % Interpolate_limit: Limit on the maximmum numebr of consecutive
        % missing data to be interpolated
        % Interpolate.FlagInterp = if TRUE, flagged data are interpolated
        % as well.
    % SigsToProc: a cell array of strings specifying name of signals to be
    % finterpolated        
    % FlagBitInterpo: Flag bit indicating interpolated data
%
% Outputs:
    % PMU
%
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMU = interpo(PMU,SigsToInterpo,Parameters,FlagBitInterpo)

%User-specified parameters
Interpolate_type = Parameters.Type;
Interpolate_limit = str2num(Parameters.Limit);
FlagInterp = Parameters.FlagInterp;

%Gives number of data points in each PMU channel
N = size(PMU.Data,1);

% If specific signals were not listed, apply to all signals except Digital
% signals
if isempty(SigsToInterpo)
    SigIdx = find(~strcmp(PMU.Signal_Type, 'D'));
    SigsToInterpo = PMU.Signal_Name(SigIdx);
end

for SigIdx = 1:length(SigsToInterpo)
    ThisSig = find(strcmp(PMU.Signal_Name,SigsToInterpo{SigIdx}));
    % If the specified signal is not in PMUstruct, skip the rest of the
    % code and go to the next SigIdx.
    if isempty(ThisSig)
        warning(['Signal ' SigsToInterpo{SigIdx} ' could not be found.']);
        continue
    end
    
    PMUdata = PMU.Data(:,ThisSig);
    if strcmp(FlagInterp, 'TRUE')
        %includes flagged data indices to interpolate data
        InterDataIdx = find(sum(PMU.Flag(:,ThisSig,:),3) > 0 | isnan(PMU.Data(:,ThisSig)));
    else
        %includes only missing data indices
        InterDataIdx = find(isnan(PMU.Data(:,ThisSig)));
    end
    %Data indices that are used to interpolate missing and flagged data
    GoodDataIdx = setdiff(1:N,InterDataIdx);
    
    %Calculates difference between consecutive indices of data to be
    %interpolated
    delta = diff(InterDataIdx);
    % Finds where the difference between measurements is '1', indicating
    % consecutive data points that are to be interpolated
    delta0 = find(delta==1);
    if ~isempty(delta0)
        delta2 = diff(delta0);
        % Indices of the indeices of the start of data to be interpolated present in
        % continous timeframe
        StartIdx = ([1; find(delta2>1)+1]);
        % Indices of the indices of the end of data to be interpolated present in
        % continous timeframe
        EndIdx = ([find(delta2>1); length(delta0)]);
        % Duration of the data to be interpolated present in
        % continous timeframe
        Dur = EndIdx - StartIdx + 1 + 1;
        LimitIdx = [];
        for BadGroupIdx = 1:length(Dur)
            if Dur(BadGroupIdx) > Interpolate_limit
                % Indices of data to be interpolated present in continous timeframe that exceeds limit
                IndExceed = delta0(StartIdx(BadGroupIdx)):delta0(EndIdx(BadGroupIdx))+1;
                LimitIdx = [LimitIdx IndExceed];
            end
        end
        %Removes indices of continous data to be interpolated if it exceeds
        %the limit of maximum number of consecutive data points to be
        %interpolated
        InterDataIdx = setdiff(InterDataIdx, InterDataIdx(LimitIdx));
    end
       
    % Can only interpolate if some good data exists
    if ~isempty(GoodDataIdx)
        if strcmp(Interpolate_type,'Linear') && ~isempty(InterDataIdx)
            %carries out linear interpolation
            PMU.Data(InterDataIdx,ThisSig) = interp1(GoodDataIdx, PMU.Data(GoodDataIdx,ThisSig), InterDataIdx, 'linear','extrap');

        elseif strcmp(Interpolate_type,'Constant') && ~isempty(InterDataIdx)
            %carries out constant interpolation
            PMU.Data(InterDataIdx,ThisSig) = interp1(GoodDataIdx, PMU.Data(GoodDataIdx,ThisSig), InterDataIdx, 'nearest','extrap');
            
        elseif strcmp(Interpolate_type,'Cubic') && ~isempty(InterDataIdx)
            %carries out cubic interpolation
            PMU.Data(InterDataIdx,ThisSig) = interp1(GoodDataIdx, PMU.Data(GoodDataIdx,ThisSig), InterDataIdx, 'pchip','extrap');
            
        elseif ~isempty(InterDataIdx)
            warning([Interpolate_type ' is not a valid type of interpolation. Interpolation will not be carried out.']);
        end
    end
    %Gives the indices of data that are interpoalted
    InterDataInd = find(PMUdata ~= PMU.Data(:,ThisSig) & ~isnan(PMU.Data(:,ThisSig)));
    
    % Flag missing data that has been interpolated
    PMU.Flag(InterDataInd,ThisSig,FlagBitInterpo) = true;    
end
end


