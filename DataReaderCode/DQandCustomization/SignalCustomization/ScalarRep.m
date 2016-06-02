% function custPMU = ScalarRep(custPMU,scalar,SignalName,SignalType,SignalUnit)
function custPMU = ScalarRep(custPMU,Parameters)

% Number of signals in the current custom PMU Data field
[~,NumSig] = size(custPMU.Data);

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
    custPMU.Flag(:,NumSig+1) = 0;
    custPMU.Signal_Name{NumSig+1} = SignalName;
catch
    warning(['Scalar ' Parameters.scalar ' could not be converted to a scalar. Signal will be set to NaN and Flags set.']);
    FlagVal = str2num(Parameters.FlagVal);
    if isempty(FlagVal)
        warning(['Flag ' Parameters.FlagVal ' could not be converted to a number. Flags will be set to NaN.']);
        FlagVal = NaN;
    end
    custPMU.Data(:,NumSig+1) = NaN;
    custPMU.Flag(:,NumSig+1) = FlagVal;
    custPMU.Signal_Name{NumSig+1} = SignalName;
    custPMU.Signal_Type{NumSig+1} = 'SC';
    custPMU.Signal_Unit{NumSig+1} = 'SC';
end

% Assign SignalType and SignalUnit
custPMU.Signal_Type{NumSig+1} = SignalType;
custPMU.Signal_Unit{NumSig+1} = SignalUnit;