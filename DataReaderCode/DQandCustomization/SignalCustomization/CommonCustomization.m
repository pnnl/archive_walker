% function PMUstruct = CommonCustomization(PMUstruct,custPMUidx,Parameters,Operation)
% This function creates customized signal(s) by performing operations like
% SignReversal, AbsVal, RealComponent, ImagComponent, and/or ComplexConj on
% given signal(s)
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
    % Parameters: structure containing user provided information to
    % create customized signal(s)
        % Parameters.signal: a struct array containing information on signals
        % required to calculate customized signal(s)(Size: 1 by number of signals to be customized)
                % Parameters.signal{i}.PMU: a string specifying name of the PMU
                % containing i^th signal to be customized
                % Parameters.signal{i}.Channel: a string specifying the channel of PMU that represents i^th signal to be customized            
                % Parameters.signal{i}.CustName: a string specifying name for the i^th customized signal 
    % custPMUidx: numerical identifier for PMU that would store customized signal
    % Operation: a string specifying which operation is to be carried out
    % on given signal(s)l
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
function PMUstruct = CommonCustomization(PMUstruct,custPMUidx,Parameters,Operation,FlagBitCust)

% Size of the current Data matrix for the custom PMU - N samples by NumSig signals
[N,NcustSigs] = size(PMUstruct(custPMUidx).Data);

AvailablePMU = {PMUstruct.PMU_Name};

NumSigs = length(Parameters.signal);
if NumSigs == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.signal = {Parameters.signal};
end

SigMat = NaN*ones(N,NumSigs);
FlagMat = true(N,NumSigs);
SignalType = cell(1,NumSigs);
SignalUnit = cell(1,NumSigs);
SignalName = cell(1,NumSigs);
ErrFlagUser = zeros(NumSigs,1);
for SigCount = 1:NumSigs
    CheckSignalNameError(Parameters.signal{SigCount}.CustName, PMUstruct(custPMUidx).Signal_Name);
    PMUidx = find(strcmp(Parameters.signal{SigCount}.PMU,AvailablePMU));
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidx)
        warning(['PMU ' Parameters.signal{SigCount}.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlagUser(SigCount) = 1;
        continue
    end
    
    SigIdx = find(strcmp(Parameters.signal{SigCount}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdx)
        warning(['Signal ' Parameters.signal{SigCount}.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlagUser(SigCount) = 1;
        continue
    end
    
    % Make sure dimensions match the data matrix. If not, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if ~size(PMUstruct(PMUidx).Data(:,SigIdx),1) == N
        warning([Parameters.signal{SigCount}.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
        ErrFlagUser(SigCount) = 1;
        continue
    end
    
    % Apply the operation and assign data
    switch Operation
        case 'SignReversal'
            SigMat(:,SigCount) = -PMUstruct(PMUidx).Data(:,SigIdx);
        case 'AbsVal' 
            SigMat(:,SigCount) = abs(PMUstruct(PMUidx).Data(:,SigIdx));
        case 'RealComponent'
            SigMat(:,SigCount) = real(PMUstruct(PMUidx).Data(:,SigIdx));
        case 'ImagComponent'
            SigMat(:,SigCount) = imag(PMUstruct(PMUidx).Data(:,SigIdx));
        case 'ComplexConj'
            SigMat(:,SigCount) = conj(PMUstruct(PMUidx).Data(:,SigIdx));
    end
    
    FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    
    % Set flags
    FlagMat(:,SigCount) = FlagVec;
    % Store SignalType
    SignalType{SigCount} = PMUstruct(PMUidx).Signal_Type{SigIdx};
    % Store SignalUnit
    SignalUnit{SigCount} = PMUstruct(PMUidx).Signal_Unit{SigIdx};
    % Store SignalName
    SignalName{SigCount} = Parameters.signal{SigCount}.CustName;
end

% Store results in custom PMU
PMUstruct(custPMUidx).Signal_Name(NcustSigs+(1:NumSigs)) = SignalName;
PMUstruct(custPMUidx).Signal_Type(NcustSigs+(1:NumSigs)) = SignalType;
PMUstruct(custPMUidx).Signal_Unit(NcustSigs+(1:NumSigs)) = SignalUnit;
PMUstruct(custPMUidx).Data(:,NcustSigs+(1:NumSigs)) = SigMat;
for SigInd = 1:NumSigs
    if ErrFlagUser(SigInd)
        PMUstruct(custPMUidx).Flag(:,NcustSigs+SigInd,FlagBitCust(2)) = FlagMat(:,SigInd); %flagged for error in user input
    else
        PMUstruct(custPMUidx).Flag(:,NcustSigs+SigInd,FlagBitCust(1)) = FlagMat(:,SigInd);%flagged for flagged input signal
    end
end


