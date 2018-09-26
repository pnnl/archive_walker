% function PMUstruct = AngleCustomization(PMUstruct,custPMUidx,Parameters)
% This function creates customized signal(s) consisting of only phase angle of given signal(s).
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
        % Parameters.signal: a struct array containing information on 
        % signal(s) to be customized. (size: 1 by number of signals to be customized)
                    % Parameters.signal{i}.PMU: a string specifying
                    % the name of PMU consisting of i^th signal to be customized
                    % Parameters.signal{i}.Channel: a string specifying
                    % the channel of PMU that represents i^th signal to be customized
                    % Parameters.signal{i}.CustName: a string specifying
                    % name for the i^th customized signal              
    % custPMUidx: numerical identifier for PMU that would store customized signall
    % FlagBitCust: Flag bits reserved for flagging new customized signal
        % FlagBitCust(1): Indicates error associated with user specified
        % parameters for creating a customized signal
        % FlagBitCust(2): Indicates data points in customized signal that
        % used flagged input data points 
% 
% Outputs:
    % PMUstruct: 
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Changed the flag matrix from a 2 dimensional double matrix to a 3
%dimensional logical matrix (3rd dimension represents flag bit)
%Modified on July 13, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Includes FlagBitCust variable
function PMUstruct = AngleCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

AvailablePMU = {PMUstruct.PMU_Name};

NumSigs = length(Parameters.signal);
if NumSigs == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.signal = {Parameters.signal};
end
SigMat = [];
SignalType = cell(1,NumSigs);
SignalName = cell(1,NumSigs);
ErrFlag = zeros(NumSigs,1);
PMUidxAll = [];
for SigCount = 1:NumSigs
    PMUidx = find(strcmp(Parameters.signal{SigCount}.PMU,AvailablePMU));
    PMUidxAll = [PMUidxAll PMUidx];
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidx)
        warning(['PMU ' Parameters.signal{SigCount}.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlag(SigCount) = 1;
        continue
    end
    
    SigIdx = find(strcmp(Parameters.signal{SigCount}.Channel,PMUstruct(PMUidx).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdx)
        warning(['Signal ' Parameters.signal{SigCount}.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlag(SigCount) = 1;
        continue
    end
    
    
    if isempty(SigMat)
        N = length(PMUstruct(PMUidx).Data(:,SigIdx));
        SigMat = NaN(N,NumSigs);
        FlagMat = true(N,NumSigs);
    else
        % Make sure dimensions match the data matrix. If not, skip the rest of the for 
        % loop so that Data remains NaNs and Flags remain set.
        if ~size(PMUstruct(PMUidx).Data(:,SigIdx),1) == N
            warning([Parameters.signal{SigCount}.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
            ErrFlag(SigCount) = 1;
            continue
        end
    end
    
    % Apply the operation and assign data
    SigMat(:,SigCount) = atan2(imag(PMUstruct(PMUidx).Data(:,SigIdx)),real(PMUstruct(PMUidx).Data(:,SigIdx)));
    
    % Set flags
    FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
    FlagMat(:,SigCount) = FlagVec;
    
    % Store SignalType
    switch PMUstruct(PMUidx).Signal_Type{SigIdx}
        case 'VPP'
            SignalType{SigCount} = 'VAP';
        case 'VPA'
            SignalType{SigCount} = 'VAA';
        case 'VPB'
            SignalType{SigCount} = 'VAB';
        case 'VPC'
            SignalType{SigCount} = 'VAC';
        case 'IPP'
            SignalType{SigCount} = 'IAP';
        case 'IPA'
            SignalType{SigCount} = 'IAA';
        case 'IPB'
            SignalType{SigCount} = 'IAB';
        case 'IPC'
            SignalType{SigCount} = 'IAC';
        otherwise
            SignalType{SigCount} = 'OTHER';
    end
    % Store SignalName
    SignalName{SigCount} = Parameters.signal{SigCount}.CustName;
end

custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
if isempty(custPMUidx)
    % Initialize the custom PMU sub-structure and add it to the PMU structure
    % using some of the fields from an existing PMU sub-structure.
    RefIdx = [PMUidxAll 1];  % The 1 is included to prevent errors when PMUidx is empty
    PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(RefIdx(1)).Signal_Time,Num_Flags);
    custPMUidx = length(PMUstruct);
end

% Make sure custom PMU is the right size (note that it may have existed
% before the previous block of code)
if size(PMUstruct(custPMUidx).Data,1) ~= size(SigMat,1)
    % Custom PMU is the wrong size
    warning('Custom signal does not match custom PMU size, returning NaN and setting Flags');
    ErrFlag(:) = 1;
    SigMat = NaN;
    FlagMat = true(size(PMUstruct(custPMUidx).Flag,1),NumSigs);
end

for SigCount = 1:NumSigs
    CheckSignalNameError(Parameters.signal{SigCount}.CustName, PMUstruct(custPMUidx).Signal_Name);
end

% Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
NcustSigs = size(PMUstruct(custPMUidx).Data,2);

% Store results in custom PMU
PMUstruct(custPMUidx).Signal_Name(NcustSigs+(1:NumSigs)) = SignalName;
PMUstruct(custPMUidx).Signal_Type(NcustSigs+(1:NumSigs)) = SignalType;
SignalUnit = cell(NumSigs,1);
[SignalUnit{:}] = deal('RAD');
PMUstruct(custPMUidx).Signal_Unit(NcustSigs+(1:NumSigs)) = SignalUnit;
PMUstruct(custPMUidx).Data(:,NcustSigs+(1:NumSigs)) = SigMat;
for SigInd = 1:NumSigs
    if ErrFlag(SigInd)
        PMUstruct(custPMUidx).Flag(:,NcustSigs+SigInd,FlagBitCust(2)) = FlagMat(:,SigInd); %flagged for error in user input
    else
        PMUstruct(custPMUidx).Flag(:,NcustSigs+SigInd,FlagBitCust(1)) = FlagMat(:,SigInd);%flagged for flagged input signal
    end
end



