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
%
% Outputs:
    % PMU
%     
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMUstruct = interpo(PMUstruct,Interpolate)
Interpolate_type = Interpolate.Type;
Interpolate_limit = Interpolate.Limit;
FlagInterp = Interpolate.FlagInterp;
FlaBit_Iterp = 2;
NumPMU = length(PMUstruct.PMU);

for PMUidx = 1:NumPMU
    [N,NumChan] = size(PMUstruct.PMU(PMUidx).Data);    
    %     NaNData = find(isnan(PMU(PMUidx).Data));
    for ChanIdx = 1:NumChan
        if strcmp(FlagInterp, 'TRUE')
            InterDataIdx = find(sum(PMUstruct.PMU(PMUidx).Flag(:,ChanIdx,:),3) > 0 | isnan(PMUstruct.PMU(PMUidx).Data(:,ChanIdx)));
        else 
            InterDataIdx = find(isnan(PMUstruct.PMU(PMUidx).Data(:,ChanIdx)));
        end
%         NaNdata = find(isnan(PMU(PMUidx).Data(:,ChanIdx)));
%         if isempty(NaNdata)
%             NaNdata = false(N,1);
%         end
%         InterDataIdx = find((NaNdata+FlaggedData)>0);
        GoodDataIdx = setdiff(1:N,InterDataIdx);
        %to check the limit
        % Find the difference between each sequential measurement
        delta = diff(InterDataIdx);
        % Find where the difference between measurements was zero, indicating
        % stale data
        delta0 = find(delta==1);
        if ~isempty(delta0)
            % Find the differences between the (almost) indices of stale data. This
            % helps identify separate groups of stale data.
            delta2 = diff(delta0);
            % Indices of the indices of the start of stale data
            StartIdx = ([1; find(delta2>1)+1]);
            % Indices of the indices of the end of stale data (almost)
            EndIdx = ([find(delta2>1); length(delta0)]);
            % Duration of the stale data
            Dur = EndIdx - StartIdx + 1 + 1;
            LimitIdx = [];
            for BadGroupIdx = 1:length(Dur)
                if Dur(BadGroupIdx) > Interpolate_limit
                    % Indices of continous missing data that exceeds limit
                    IndExceed = delta0(StartIdx(BadGroupIdx)):delta0(EndIdx(BadGroupIdx))+1;
                    LimitIdx = [LimitIdx IndExceed];      
                end
            end
            InterDataIdx = setdiff(InterDataIdx, InterDataIdx(LimitIdx));
        end
        % Flag missing data that has been interpolated
        PMU(PMUidx).Flag(InterDataIdx,ChanIdx,FlaBit_Iterp) = true;
        
        if strcmp(Interpolate_type,'Linear') && ~isempty(InterDataIdx)
            PMUstruct.PMU(PMUidx).Data(InterDataIdx,ChanIdx) = interp1(GoodDataIdx, PMU(PMUidx).Data(GoodDataIdx,ChanIdx), InterDataIdx, 'linear');
            %carry out linear interpolation
        elseif strcmp(Interpolate_type,'Constant') && ~isempty(InterDataIdx)
            %carry out constant interpolation
            PMUstruct.PMU(PMUidx).Data(InterDataIdx,ChanIdx) = interp1(GoodDataIdx, PMU(PMUidx).Data(GoodDataIdx,ChanIdx), InterDataIdx,  'nearest');
        end
    end
end
end


