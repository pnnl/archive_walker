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
% 
% Outputs:
    % custPMU
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov): 
%Changed the flag matrix from a 2 dimensional double matrix to a 3 
%dimensional logical matrix (3rd dimension represents flag bit)

function custPMU = ScalarRep(custPMU,Parameters)

% Number of signals and flags in the current custom PMU Data field
[~,NumSig,NFlags] = size(custPMU.Flag);

SignalName = Parameters.SignalName;

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
    custPMU.Data(:,NumSig+1) = scalar;
    custPMU.Flag(:,NumSig+1,:) = false;
    custPMU.Signal_Name{NumSig+1} = SignalName;
catch
    warning(['Scalar ' Parameters.scalar ' could not be converted to a scalar. Signal will be set to NaN and Flags set.']);
    custPMU.Data(:,NumSig+1) = NaN;
    custPMU.Flag(:,NumSig+1,NFlags) = true; %flagged for error in user input
    custPMU.Signal_Name{NumSig+1} = SignalName;
    custPMU.Signal_Type{NumSig+1} = 'SC';
    custPMU.Signal_Unit{NumSig+1} = 'SC';
end

% Assign SignalType and SignalUnit
custPMU.Signal_Type{NumSig+1} = SignalType;
custPMU.Signal_Unit{NumSig+1} = SignalUnit;