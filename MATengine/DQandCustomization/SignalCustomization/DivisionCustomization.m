% function PMUstruct = DivisionCustomization(PMUstruct,custPMUidx,Parameters)
% This function creates a customized signal by dividing given dividend
% signal with given divisor signal
% 
% Inputs:
	% PMUstruct: structure in the common format for all PMUs (size: 1 by Number
	% of PMUs)
        % PMUstruct(i).Signal_Type: a cell array containing strings
        % specifying signal(s) type in the i^th PMU
                                    %size: 1 by Number of data channel in the i^th PMU
        % PMUstruct(i).Signal_Name: a cell array containing strings
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
        % PMUstruct.PMU_Name: a struct array containing strings specifying
        % name of PMUs
                                % size: Number of PMUs by 1
        % Parameters: structure containing user provided information to
         % create customized signal(s)
        % Parameters.SignalName: a string specifying name for the customized
        % signal
        % Parameters.dividend: a struct array of strings containing information on
        % dividend signal
            % Parameters.dividend.PMU: a string specifying name of the PMU
            % containing signal representing dividend signal
            % Parameters.dividend.Channel: a string specifying name of the channel 
            % that represents dividend signal
        % Parameters.divisor:a cell array of strings containing information on
        % divisor signal
            % Parameters.divisor.PMU:  a string specifying name of the PMU
            % containing signal representing divisor signal       
            % Parameters.divisor.Channel: a string specifying name of the channel 
            % that represents divisor signal        
    % custPMUidx: numerical identifier for PMU that would store customized signall
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
function PMUstruct = DivisionCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

AvailablePMU = {PMUstruct.PMU_Name};

% Get dividend (what you divide by the divisor)
PMUidxDend = find(strcmp(Parameters.dividend.PMU,AvailablePMU));
if ~isempty(PMUidxDend)
    SigIdxDend = find(strcmp(Parameters.dividend.Channel,PMUstruct(PMUidxDend).Signal_Name));
else
    % PMU was not found, so the signal index cannot be found
    SigIdxDend = [];
end

% Get divisor (what you divide by)
PMUidxDisor = find(strcmp(Parameters.divisor.PMU,AvailablePMU));
if ~isempty(PMUidxDisor)
    SigIdxDisor = find(strcmp(Parameters.divisor.Channel,PMUstruct(PMUidxDisor).Signal_Name));
else
    % PMU was not found, so the signal index cannot be found
    SigIdxDisor = [];
end

custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
if isempty(custPMUidx)
    % Initialize the custom PMU sub-structure and add it to the PMU structure
    % using some of the fields from an existing PMU sub-structure.
    RefIdx = [PMUidxDend PMUidxDisor 1];  % The 1 is included to prevent errors when PMUidxDend and PMUidxDisor are empty
    PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(RefIdx(1)).Signal_Time,Num_Flags);
    custPMUidx = length(PMUstruct);
end

SignalName = Parameters.SignalName;
CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);
% Size of the current Data matrix for the custom PMU - N samples by NumSig
% signals by NFlags flags
NumSig = size(PMUstruct(custPMUidx).Data,2);

PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
if (~isempty(SigIdxDend)) && (~isempty(SigIdxDisor))
    SignalUnitDend = PMUstruct(PMUidxDend).Signal_Unit{SigIdxDend};
    SignalTypeDend = PMUstruct(PMUidxDend).Signal_Type{SigIdxDend};
    SignalUnitDisor = PMUstruct(PMUidxDisor).Signal_Unit{SigIdxDisor};
    SignalTypeDisor = PMUstruct(PMUidxDisor).Signal_Type{SigIdxDisor};
    
    if strcmp(SignalTypeDisor,'SC') && strcmp(SignalUnitDisor,'SC')
        % Dividing by a scalar results in the units and type of the
        % dividend
        SignalType = SignalTypeDend;
        SignalUnit = SignalUnitDend;
    elseif strcmp(SignalUnitDend,SignalUnitDisor)
        % If units are the same, the result is a scalar
        SignalType = 'SC';
        SignalUnit = 'SC';
    else
        % Signal types disagree, set to OTHER
        SignalType = 'OTHER';
        SignalUnit = 'O';
    end
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
    
    % Make sure units and type make sense together
    if CheckTypeAndUnits(SignalType,SignalUnit)
        % SignalUnit and SignalType are acceptable
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnit;
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
    else
        % Specified SignalUnit and SignalType were not acceptable, so set to OTHER
        warning('Disagreement between signal units and type, setting to other.');
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
    end
    
    % Check signal sizes
    N = size(PMUstruct(custPMUidx).Data,1);
    if (N==length(PMUstruct(PMUidxDend).Data(:,SigIdxDend))) && (N==length(PMUstruct(PMUidxDisor).Data(:,SigIdxDisor)))
        % Compatible
        PMUstruct(custPMUidx).Data(:,NumSig+1) = PMUstruct(PMUidxDend).Data(:,SigIdxDend) ./ PMUstruct(PMUidxDisor).Data(:,SigIdxDisor);
        FlagVec =sum(PMUstruct(PMUidxDend).Flag(:,SigIdxDend,:),3) > 0 | sum(PMUstruct(PMUidxDisor).Flag(:,SigIdxDisor,:),3) > 0;
        PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(1)) = FlagVec; %flagged for flags in input signal
    else
        % Signals do not have compatible sizes
        warning('Signals length disagreement. Values were set to NaN and Flags set.');
        PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true; %flagged for error in user input
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
    end
else
    % Signals were not found
    warning('Signals were not found. Values were set to NaN and Flags set.');
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true; %flagged for error in user input
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
end