% function PMU = DPMultiRate(PMU,ProcessMultirate)
% This function carries out data-rate change operation on PMU measurements specified
% in XML file. 
%
% Inputs:
    % PMU: Struct array in common format for PMUs
        % PMU.PMU_Name: a string specifying name of i^th PMU
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
    %ProcessMultirate: struct array containing information on different
    % upsampling and downsampling operation (size: 1 by number of datarate
    % change operation)
        % ProcessMultirate.Parameters: a struct array containing user-specified
        % parameters for i^th datarate change operation.
            % ProcessMultirate.Parameters.Newrate: New sampling frequency
            % of signal
            % ProcessMultirate.Parameters.p: factor by which data is
            % upsampled
            % ProcessMultirate.Parameters.q: factor by which data is
            % downsampled
        % ProcessMultirate.PMU: struct array containing information on PMUs of
        % dimension 1 by number of PMUs.
        % ProcessMultirate{i}.PMU{j}.Name: a string specifying name of j^th PMU
        % ProcessMultirate{i}.PMU{j}.Channels: a struct array containing
        % information on data channels in j^th PMU.
        % ProcessMultirate{i}.PMU{j}.Channels.Channel{k}.Name: a string specifying
        % name of k^th data channel in j^th PMU for i^th datarate change operation
%
% Outputs:
% PMU
%
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov)

function PMU = DPMultiRate(PMU,ProcessMultiRate)

NumMultiRate = length(ProcessMultiRate);

%calculating frequency of signal
t = PMU(1).Signal_Time.Time_String;
Ndata = length(t);
t1 = t{1};
Ind1 = findstr(t1, '.');
T1 = str2num(t1(Ind1:end));
t5 = t{5};
Ind5 = findstr(t5, '.');
T5 = str2num(t5(Ind5:end));
FsOld = round(4/(T5 - T1)); %calculating frequency using time index of 1st and 5th sample to increase accuracy

if NumMultiRate == 1
    % By default, the contents of ProcessMultiRate
    % would not be in a cell array because length is one. This
    % makes it so the same indexing can be used in the following for loop.
    ProcessMultiRate = {ProcessMultiRate};
end

for MultiRateIdx = 1:NumMultiRate
    
    Parameters = ProcessMultiRate{MultiRateIdx}.Parameters;
    NewPMUidx = length(PMU) + 1;
    
    if isfield(Parameters,'MultiRatePMU')
        PMUName  = Parameters.MultiRatePMU;
    else
        error('PMU name must be assigned by user when changing data rate.');
    end
    
    
    PMU(NewPMUidx).PMU_Name = PMUName;   % PMU name
    PMU(NewPMUidx).Time_Zone = PMU(1).Time_Zone;         % time zone; for now this is just the PST time
    PMU(NewPMUidx).Signal_Name = cell(1,0);
    PMU(NewPMUidx).Signal_Type = cell(1,0);
    PMU(NewPMUidx).Signal_Unit = cell(1,0);
    PMU(NewPMUidx).Stat = [];
    PMU(NewPMUidx).Data = [];  
    PMU(NewPMUidx).Flag = false();
    
    if isfield(Parameters,'NewRate')
        FsNew = str2num(Parameters.NewRate);
        [p,q] = rat(FsNew / FsOld);        
    elseif isfield(Parameters,'p') && isfield(Parameters,'q')
        p = str2num(Parameters.p);
        q = str2num(Parameters.q);      
    elseif isfield(Parameters,'p') && ~isfield(Parameters,'q')
        p = str2num(Parameters.p);
        q = [];
    elseif ~isfield(Parameters,'p') && isfield(Parameters,'q')
        q = str2num(Parameters.q);
        p = [];        
    elseif ~isfield(Parameters,'p') && ~isfield(Parameters,'q')
        warning('Parameters for changing data rate not provided');
        continue;
    end
    [TimeString,DateNumArray] = GetNewTime(PMU(1).Signal_Time.Signal_datenum,p,q);
    PMU(NewPMUidx).Signal_Time.Time_String = TimeString;
    PMU(NewPMUidx).Signal_Time.Signal_datenum = DateNumArray;
    %PMU(NewPMUidx).Flag = false(floor(Ndata*p/q),0,2);
    
    
    if isfield(ProcessMultiRate{MultiRateIdx},'PMU')
        % Get list of PMUs to apply filter to
        NumPMU = length(ProcessMultiRate{MultiRateIdx}.PMU);
        if NumPMU == 1
            % By default, the contents of ProcessMultiRate{MultiRateIdx}.PMU
            % would not be in a cell array because length is one. This
            % makes it so the same indexing can be used in the following for loop.
            ProcessMultiRate{MultiRateIdx}.PMU = {ProcessMultiRate{MultiRateIdx}.PMU};
        end
        
        PMUstructIdx = zeros(1,NumPMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        for PMUidx = 1:NumPMU
            % Add the PMU to the list of PMUs to be passed to the
            % filter.
            PMUstructIdx(PMUidx) = find(strcmp(ProcessMultiRate{MultiRateIdx}.PMU{PMUidx}.Name, {PMU.PMU_Name}));
            
            % Get ChansToFilt for this PMU
            % If channels aren't specified, PMUchans(PMUidx).ChansToFilt
            % is left empty to indicate that all channels should be
            % included
            if isfield(ProcessMultiRate{MultiRateIdx}.PMU{PMUidx},'Channel')
                NumChan = length(ProcessMultiRate{MultiRateIdx}.PMU{PMUidx}.Channel);
                if NumChan == 1                 
                    ProcessMultiRate{MultiRateIdx}.PMU{PMUidx}.Channel = {ProcessMultiRate{MultiRateIdx}.PMU{PMUidx}.Channel};
                end
                
                PMUchans(PMUidx).ChansToFilt = cell(NumChan,1);
                for ChanIdx = 1:NumChan
                    PMUchans(PMUidx).ChansToFilt{ChanIdx} = ProcessMultiRate{MultiRateIdx}.PMU{PMUidx}.Channel{ChanIdx}.Name;
                end
            end
        end
    else
        % Apply this filter to all signals in all PMUs
        NumPMU = length(PMU);
        PMUchans = struct('ChansToFilt',cell(NumPMU,1));
        PMUstructIdx = 1:NumPMU;
    end
    
    for PMUidx = 1:NumPMU
        if PMUstructIdx(PMUidx) == NewPMUidx
            continue
        elseif size(PMU(PMUstructIdx(PMUidx)).Data,1) ~= size(PMU(PMUstructIdx(1)).Data,1)
            warning('At least one PMU size does not match')
            continue
        else
            PMU(NewPMUidx)= MultiRate(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,p,q,PMU(NewPMUidx));
        end
    end
    
end
end

% function [TimeString,DateNumArray] = GetNewTime(Signal_datenum,p,q)
% This function creates time string cell array to accomodate change in data rate
%
%Created on 06/28/2016 by Urmila Agrawal (urmila.agrawal@pnnl.gov)

function [TimeString,DateNumArray] = GetNewTime(Signal_datenum,p,q)

SigDate = [Signal_datenum(1) ;Signal_datenum(end)];

N_old = length(Signal_datenum);
if ~isempty(p)
    N_new = N_old*p-(p-1);
    NSamp = N_new - 1;
    SampleDiff = diff(SigDate)'/(NSamp);
    DateNumArray =  SigDate(1) + (0:NSamp)*SampleDiff;
    for AddTim= 1:p-1
        DateNumArray = [DateNumArray DateNumArray(end) + SampleDiff;];
    end
    if ~isempty(q)
        DateNumArray = downsample(DateNumArray,q);
    end
end
if ~isempty(q) && isempty(p)
    DateNumArray = downsample(Signal_datenum,q);
end

TimeString = cellstr(datestr(DateNumArray, 'yyyy-mm-dd HH:MM:SS.FFF'));

end


