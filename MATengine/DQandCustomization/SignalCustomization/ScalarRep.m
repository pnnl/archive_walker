% function function custPMU = ScalarRep(custPMU,Parameters)
% This function creates a signal by replicating a given scalar number.
% 
% Inputs:
	% custPMU: structure in the common format for PMU storing the customized
	% signal
        % custPMU.Signal_Type:a cell array containing strings specifying signal(s) type
                                    %size: 1 by number of customized signals
        % custPMU.Signal_Name: a cell array containing strings specifying name of signal(s) 
                                    %size: 1 by number of customized signals
        % custPMU.Signal_Unit:a cell array containing strings specifying unit of signal(s) 
                                    %size: 1 by number of customized signalsl
        % custPMU.Data: Matrix containing customized signals
                                %size: number of data points by number of customized signals                              
        % custPMU.Flag: 3-dimensional matrix indicating PMU measurements
        % flagged by different filter operation.
                                %size: number of data points by number of customized signals  by number of flag bits
    % Parameters: structure containing user provided information to
    % create customized signal(s)
        % Parameters.SignalName: a string specifying name for the customized
        % signal     
        % Parameters.SigType: a string specifying the signal type for
        % customized signal
        % Parameters.SigUnit: a string specifying the signal unit for
        % customized signal
        % Parameters.Scalar: scalar value used to create a matrix by
        % replicating this value
    % FlagBitCust: Flag bits reserved for flagging new customized signal
        % FlagBitCust(1): Indicates error associated with user specified
        % parameters for creating a customized signal
        % FlagBitCust(2): Indicates data points in customized signal that
        % used flagged input data points 
% 
% Outputs:
    % custPMU
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov): 
%Changed the flag matrix from a 2 dimensional double matrix to a 3 
%dimensional logical matrix (3rd dimension represents flag bit)
%Modified on July 13, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Includes FlagBitCust variable

function PMUstruct = ScalarRep(PMUstruct,Num_Flags,Parameters,FlagBitCust)

AvailablePMU = {PMUstruct.PMU_Name};

custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
if isempty(custPMUidx)
    % Initialize the custom PMU sub-structure and add it to the PMU structure
    % using some of the fields from an existing PMU sub-structure.

    % The user must specify a reference PMU to provide time stamps
    PMUidx = find(strcmp(Parameters.TimeSourcePMU,AvailablePMU));

    if isempty(PMUidx)
        warning(['The time source PMU ' Parameters.TimeSourcePMU ' could not be found, using first PMU in structure.']);
        PMUidx = 1;
    end

    PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(PMUidx).Signal_Time,Num_Flags);
    custPMUidx = length(PMUstruct);
end

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
NumSig = size(PMUstruct(custPMUidx).Data,2);

SignalName = Parameters.SignalName;
CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);

% If the SignalType is not provided, set to scalar
if isfield(Parameters,'SignalType')
    SignalType = Parameters.SignalType;
else
    SignalType = 'SC';
end

% If the SignalUnit is not provided, set to scalar
if isfield(Parameters,'SignalUnit')
    SignalUnit = Parameters.SignalUnit;
else
    SignalUnit = 'SC';
end

% If SignalType and SignalUnit are not acceptable, set them to scalar
if ~CheckTypeAndUnits(SignalType,SignalUnit)
    warning('SignalType and/or SignalUnit were unacceptable and are being set to scalar');
    SignalType = 'SC';
    SignalUnit = 'SC';
end

try
    scalar = str2num(Parameters.scalar);
    PMUstruct(custPMUidx).Data(:,NumSig+1) = scalar;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,:) = false;
    PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
catch
    warning(['Scalar ' Parameters.scalar ' could not be converted to a scalar. Signal will be set to NaN and Flags set.']);
    PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
    PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true; %flagged for error in user input
    PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
    PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'SC';
    PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'SC';
end

% Assign SignalType and SignalUnit
PMUstruct(custPMUidx).Signal_Type{NumSig+1} = SignalType;
PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = SignalUnit;