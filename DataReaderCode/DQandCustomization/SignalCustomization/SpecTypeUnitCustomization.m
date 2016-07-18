% function PMUstruct = SpecTypeUnitCustomization(PMUstruct,custPMUidx,Parameters)
% This function changes the type amd unit of a signal.
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
        % PMUstruct.PMU_Name: a cell array containing strings specifying
        % name of PMUs
                                % size: Number of PMUs by 1
    % Parameters: Structure containing user provided information to
    % create a customized signal
        % Parameters.CustName: a string specifying name for the customized
        % signal
        % Parameters.PMU: a string specifying name of the PMU
        % consisting of the signal to be customized
        % Parameters.Channel: a string specifying name of the channel 
        % that represents signal to be customized
        % Parameters.SigType: a string specifying the signal type for
        % customized signal
        % Parameters.SigUnit: a string specifying the signal unit for
        % customized signal
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
function PMUstruct = SpecTypeUnitCustomization(PMUstruct,custPMUidx,Parameters,FlagBitCust)

% Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals

NcustSigs = size(PMUstruct(custPMUidx).Data,2);

if isfield(Parameters,'CustName')
    CheckSignalNameError(Parameters.CustName, PMUstruct(custPMUidx).Signal_Name);
    % Make sure the specified signal type and unit are compatible
    if ~CheckTypeAndUnits(Parameters.SigType,Parameters.SigUnit)
        % Not compatible
        warning([Parameters.SigType ' and ' Parameters.SigUnit ' are incompatible. Values were set to NaN and Flags set.']);
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = true; %flags is set for customized signal for error in user input
        return
    end
    
    AvailablePMU = {PMUstruct.PMU_Name};
    
    PMUidx = find(strcmp(Parameters.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, issue warning and do nothing
    if isempty(PMUidx)
        warning(['PMU ' Parameters.PMU ' could not be found. Values were set to NaN and Flags set.']);
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = true; %flags is set for customized signal for error in user input
        return
    end
    
    SigIdx = find(strcmp(Parameters.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, issue warning and do nothing
    if isempty(SigIdx)
        warning(['Signal ' Parameters.Channel ' could not be found. Values were set to NaN and Flags set.']);
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = true; %flags is set for customized signal for error in user input
        return
    end
    
    % All checks passed, set type and unit for input signal or create a new
    % custom signal with the type and unit
    % A custom signal name was provided, so instead of replacing the input
    % signal add a new signal to the custom signal PMU
    PMUstruct(custPMUidx).Data(:,NcustSigs+1) = PMUstruct(PMUidx).Data(:,SigIdx);
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,:) = PMUstruct(PMUidx).Flag(:,SigIdx,:);
    FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(1)) = FlagVec; %flags is set for customized signal obtained from input signal with flagged data
    PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = false; %flags is set for customized signal obtained from input signal with flagged data
    PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = Parameters.SigType;
    PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = Parameters.SigUnit;
    PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = Parameters.CustName;
else
    % A custom signal name was not provided, so replace the input signal's type
    % and unit
    % Make sure the specified signal type and unit are compatible
    if ~CheckTypeAndUnits(Parameters.SigType,Parameters.SigUnit)
        % Not compatible
        warning([Parameters.SigType ' and ' Parameters.SigUnit ' are incompatible. No changes made.']);
        return
    end
    
    AvailablePMU = {PMUstruct.PMU_Name};
    
    PMUidx = find(strcmp(Parameters.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, issue warning and do nothing
    if isempty(PMUidx)
        warning(['PMU ' Parameters.PMU ' could not be found. No changes made.']);
        return
    end
    
    SigIdx = find(strcmp(Parameters.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, issue warning and do nothing
    if isempty(SigIdx)
        warning(['Signal ' Parameters.Channel ' could not be found. No changes made.']);
        return
    end
    PMUstruct(PMUidx).Signal_Type{SigIdx} = Parameters.SigType;
    PMUstruct(PMUidx).Signal_Unit{SigIdx} = Parameters.SigUnit;
end