% function PMUstruct = SubtractionCustomization(PMUstruct,custPMUidx,Parameters)
% This function creates a customized signal by subtracting two given signals.
% 
% Inputs:
	% PMUstruct: structure in the common format for all PMUs (size: 1 by Number
	% of PMUs)
        % PMUstruct(i).Signal_Type: a cell array containing strings
        % specifying signal(s) type in the i^th PMU
                                    %size: 1 by Number of data channel in the i^th PMU
        % PMUstruct(i).Signal_Name: a cell array consiting of strings
        % specifying name of signal(s) in the i^th PMU
                                    %size: 1 by Number of data channel in the i^th PMU
        % PMUstruct(i).Signal_Unit: a cell array containing strings
        % specifying unit of signal(s) in the PMU
                                    %size: 1 by Number of data channel in the i^th PMU
        % PMUstruct(i).Data: Matrix consisting of measurements by i^th PMU
                                %size: Number of data points by number of channels                              
        % PMUstruct(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged by different filter operation
                                %size: number of data points by number of channels by number of flag bits
        % PMUstruct.PMU_Name: a cell array containing strings specifying
        % name of PMUs
                                % size: Number of PMUs by 1
    % Parameters: structure containing user provided information to
    % create customized signal(s)
        % Parameters.SignalName: a string specifying name for the customized
        % signal
        % Parameters.minuend: a struct array containing information on
        % minuend signal
            % Parameters.minuend.PMU: a string specifying name of the PMU
            % containing signal representing minuend
            % Parameters.minuend.Channel: a string specifying name of the channel 
            % that represents minuend signal
        % Parameters.subtrahend: a struct array containing information on
        % subtrahend signal
            % Parameters.subtrahend.PMU:  a string specifying name of the PMU
            % containing signal representing subtrahend        
            % Parameters.subtrahend.Channel: a string specifying name of the channel 
            % that represents subtrahend signal        
    % custPMUidx: numerical identifier for PMU that would store customized signal
% 
% Outputs:
    % PMUstruct
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix.

function PMUstruct = SubtractionCustomization(PMUstruct,custPMUidx,Parameters)

SignalName = Parameters.SignalName;

% Size of the current Data matrix and number of flags for the custom PMU - N samples by NumSig signals by NumFlags Flags
[~,NumSig, NFlags] = size(PMUstruct(custPMUidx).Flag);

AvailablePMU = {PMUstruct.PMU_Name};

% Get Minuend (what you subtract from)
PMUidxMin = find(strcmp(Parameters.minuend.PMU,AvailablePMU));
if ~isempty(PMUidxMin)
    SigIdxMin = find(strcmp(Parameters.minuend.Channel,PMUstruct(PMUidxMin).Signal_Name));
end

% Get subtrahend (what you subtract)
PMUidxSub = find(strcmp(Parameters.subtrahend.PMU,AvailablePMU));
if ~isempty(PMUidxSub)
    SigIdxSub = find(strcmp(Parameters.subtrahend.Channel,PMUstruct(PMUidxSub).Signal_Name));
end

PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
if (~isempty(SigIdxMin)) && (~isempty(SigIdxSub))
    SignalUnitMin = PMUstruct(PMUidxMin).Signal_Unit{SigIdxMin};
    SignalTypeMin = PMUstruct(PMUidxMin).Signal_Type{SigIdxMin};
    SignalUnitSub = PMUstruct(PMUidxSub).Signal_Unit{SigIdxSub};
    SignalTypeSub = PMUstruct(PMUidxSub).Signal_Type{SigIdxSub};
    
    if strcmp(SignalTypeMin,SignalTypeSub)
        % Signal types agree, keep them
        SignalType = SignalTypeMin;
    else
        % Signal types disagree, set to OTHER
        SignalType = 'OTHER';
    end
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
    
    % Check if signal units agree
    if strcmp(SignalUnitMin,SignalUnitSub)
        % Units agree
        
        % Assign custom signal
        CustSig = PMUstruct(PMUidxMin).Data(:,SigIdxMin) - PMUstruct(PMUidxSub).Data(:,SigIdxSub);
        PMUstruct(custPMUidx).Data(:,NumSig+1) = CustSig;
        
        % Set flags
        FlagVec = sum(PMUstruct(PMUidxMin).Flag(:,SigIdxMin,:),3) > 0 | sum(PMUstruct(PMUidxSub).Flag(:,SigIdxSub,:),3) > 0; % if any of the data channel is flgged then the custom signal is flagged with flag for custom signal

        PMUstruct(custPMUidx).Flag(:,NumSig+1,NFlags-1) = FlagVec;
        
        % Set units
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnitMin;
    else
        % Units do not agree
        warning('Signal units did not agree. Values were set to NaN and Flags set.');
        PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NumSig+1,NFlags) = true;
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    end
else
    % Signals were not found
    warning('Signals were not found. Values were set to NaN and Flags set.');
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,NFlags) = true;
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
end