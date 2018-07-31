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
    % FlagBitCust: Flag bits reserved for flagging new customized signal
        % FlagBitCust(1): Indicates error associated with user specified
        % parameters for creating a customized signal
        % FlagBitCust(2): Indicates data points in customized signal that
        % used flagged input data points 
% 
% Outputs:
    % PMUstruct
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix.
%Modified on July 13, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Includes FlagBitCust variable

function PMUstruct = SubtractionCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

AvailablePMU = {PMUstruct.PMU_Name};

% Get Minuend (what you subtract from)
PMUidxMin = find(strcmp(Parameters.minuend.PMU,AvailablePMU));
if ~isempty(PMUidxMin)
    SigIdxMin = find(strcmp(Parameters.minuend.Channel,PMUstruct(PMUidxMin).Signal_Name));
else
    % PMU was not found, so the signal index cannot be found
    SigIdxMin = [];
end

% Get subtrahend (what you subtract)
PMUidxSub = find(strcmp(Parameters.subtrahend.PMU,AvailablePMU));
if ~isempty(PMUidxSub)
    SigIdxSub = find(strcmp(Parameters.subtrahend.Channel,PMUstruct(PMUidxSub).Signal_Name));
else
    % PMU was not found, so the signal index cannot be found
    SigIdxSub = [];
end

custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
if isempty(custPMUidx)
    % Initialize the custom PMU sub-structure and add it to the PMU structure
    % using some of the fields from an existing PMU sub-structure.
    RefIdx = [PMUidxMin PMUidxSub 1];  % The 1 is included to prevent errors when PMUidxMin and PMUidxSub are empty
    PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(RefIdx(1)).Signal_Time,Num_Flags);
    custPMUidx = length(PMUstruct);
end

SignalName = Parameters.SignalName;
CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);
% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
NumSig = size(PMUstruct(custPMUidx).Data,2);

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
    
    % Ensure that sizes are compatible
    N = size(PMUstruct(custPMUidx).Data,1);
    if (N==length(PMUstruct(PMUidxMin).Data(:,SigIdxMin))) && (N==length(PMUstruct(PMUidxSub).Data(:,SigIdxSub)))
        % Check if signal units agree
        if strcmp(SignalUnitMin,SignalUnitSub)
            % Units agree

            % Assign custom signal
            CustSig = PMUstruct(PMUidxMin).Data(:,SigIdxMin) - PMUstruct(PMUidxSub).Data(:,SigIdxSub);
            PMUstruct(custPMUidx).Data(:,NumSig+1) = CustSig;

            % Set flags
            FlagVec = sum(PMUstruct(PMUidxMin).Flag(:,SigIdxMin,:),3) > 0 | sum(PMUstruct(PMUidxSub).Flag(:,SigIdxSub,:),3) > 0; % if any of the data channel is flgged then the custom signal is flagged with flag for custom signal

            PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(1)) = FlagVec;

            % Set units
            PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnitMin;
        else
            % Units do not agree
            warning('Signal units did not agree. Values were set to NaN and Flags set.');
            PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
            PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true;
            PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
        end
    else
        % Sizes do not agree
        warning('Signal lengths did not agree. Values were set to NaN and Flags set.');
        PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true;
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    end
else
    % Signals were not found
    warning('Signals were not found. Values were set to NaN and Flags set.');
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true;
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
end