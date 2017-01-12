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

function [PMU, FinalCondos] = DPMultiRate(PMU,ProcessMultiRate, InitialCondos)

NumMultiRate = length(ProcessMultiRate);

if NumMultiRate == 1
    % By default, the contents of ProcessMultiRate
    % would not be in a cell array because length is one. This
    % makes it so the same indexing can be used in the following for loop.
    ProcessMultiRate = {ProcessMultiRate};
end

FinalCondos = cell(1,NumMultiRate);
if isempty(InitialCondos)
    InitialCondos = cell(1,NumMultiRate);
end

for MultiRateIdx = 1:NumMultiRate
    % Parameters for the MultiRate - the structure contents are
    % specific to the eaach MultiRate operation
    Parameters = ProcessMultiRate{MultiRateIdx}.Parameters;
    NewPMUidx = length(PMU) + 1;
    
    if isfield(Parameters,'MultiRatePMU')
        PMUName  = Parameters.MultiRatePMU;
    else
        error('PMU name must be assigned by user when changing data rate.');
    end
    
    % Initializing new PMU for storing data after multi-rate operation
    PMU(NewPMUidx).PMU_Name = PMUName;   % PMU name
    PMU(NewPMUidx).Time_Zone = PMU(1).Time_Zone;         % time zone; for now this is just the PST time
    PMU(NewPMUidx).Signal_Name = cell(1,0);
    PMU(NewPMUidx).Signal_Type = cell(1,0);
    PMU(NewPMUidx).Signal_Unit = cell(1,0);
    PMU(NewPMUidx).Stat = [];
    PMU(NewPMUidx).Data = [];  
    PMU(NewPMUidx).Flag = false();  
    
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
                    % By default, the contents of ProcessMultiRate{MultiRateIdx}.PMU{PMUidx}.Channel
                    % would not be in a cell array because length is one. This
                    % makes it so the same indexing can be used in the following for loop.
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
    
    t = PMU(PMUstructIdx(1)).Signal_Time.Signal_datenum;
    FsOld = round(1/(mean(diff(t)*24*60*60)));
    
    % Checks given user-specified parameters and then calculates the
    % upsampling and downsampling rate
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
%     elseif ~isfield(Parameters,'p') && ~isfield(Parameters,'q')
%         warning('Parameters for changing data rate not provided');
%         continue;
    end
    
    %Gets timestring and dateNumArray for data after multirate
    %operation
    [TimeString,DateNumArray] = GetNewTime(PMU(PMUstructIdx(1)).Signal_Time.Signal_datenum,p,q);
    PMU(NewPMUidx).Signal_Time.Time_String = TimeString;
    PMU(NewPMUidx).Signal_Time.Signal_datenum = DateNumArray; 
    
    FinalCondos{MultiRateIdx} = cell(1,NumPMU);
    if isempty(InitialCondos{MultiRateIdx})
        InitialCondos{MultiRateIdx} = cell(1,NumPMU);
    end
    
    for PMUidx = 1:NumPMU
%         if PMUstructIdx(PMUidx) == NewPMUidx %this is to make sure that the new PMU is not included in the PMU list whose data is to be upsampled and downsampled
%             continue
%         elseif size(PMU(PMUstructIdx(PMUidx)).Data,1) ~= size(PMU(PMUstructIdx(1)).Data,1) %checks that the number of data points for all PMUs is same
%             warning('At least one PMU size does not match')
%             continue
%         else
%             [PMU(NewPMUidx), FinalCondos{MultiRateIdx}{PMUidx}] = MultiRate(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,p,q,PMU(NewPMUidx), InitialCondos{MultiRateIdx}{PMUidx}); %if all conditions are satisfied, then calls function to change data rate
%         end
        
        % Ignores the checks above. Note that the elseif statement is not a
        % good way to check for the condition anyway. On 12-13-2016 I
        % updated this code so that not all PMUs have to have the same
        % sampling rate, only all the PMUs referenced in a single MultiRate
        % element in the XML.
        [PMU(NewPMUidx), FinalCondos{MultiRateIdx}{PMUidx}] = MultiRate(PMU(PMUstructIdx(PMUidx)),PMUchans(PMUidx).ChansToFilt,p,q,PMU(NewPMUidx), InitialCondos{MultiRateIdx}{PMUidx}); %if all conditions are satisfied, then calls function to change data rate
    end
    
end
end

% function [TimeString,DateNumArray] = GetNewTime(Signal_datenum,p,q)
% This function creates time string cell array to accomodate change in data rate
%
%Created on 06/28/2016 by Urmila Agrawal (urmila.agrawal@pnnl.gov)

function [TimeString,DateNumArray] = GetNewTime(Signal_datenum,p,q)

%dateNUmArray of first and last sample to interpolate datenumArray for in
%between data points
SigDate = [Signal_datenum(1) ;Signal_datenum(end)];

N_old = length(Signal_datenum);
if ~isempty(p)
    
    %Gives number of data points lying between the SigDate time
    N_new = N_old*p-(p-1);
    NSamp = N_new - 1;
    
    %Gives difference of dateNumArray between consecutive data points
    SampleDiff = diff(SigDate)'/(NSamp);
    
    %calculated dateNumArray for all data points
    DateNumArray =  SigDate(1) + (0:NSamp)*SampleDiff;
    
    %adds dateNumArray for last (p-1) samples
    for AddTim= 1:p-1
        DateNumArray = [DateNumArray DateNumArray(end) + SampleDiff;];
    end
    %if 'q' is not empty, then downsamples dateNumArray
    if ~isempty(q)
        DateNumArray = downsample(DateNumArray,q);
    end
end
%if 'q' is not empty and 'p' is empty, then downsamples dateNumArray
if ~isempty(q) && isempty(p)
    DateNumArray = downsample(Signal_datenum,q);
end

%Changes dateNumArray to time string in standard format
TimeString = cellstr(datestr(DateNumArray, 'yyyy-mm-dd HH:MM:SS.FFF'));

end


