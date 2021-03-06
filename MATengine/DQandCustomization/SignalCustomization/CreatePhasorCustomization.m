% function PMUstruct = CreatePhasorCustomization(PMUstruct,custPMUidx,Parameters)
% This function creates voltage, and/or current phasor signal(s) by using the given
% magnitude and phase signals
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
        % Parameters.phasor: a struct array containing information on signals
        % required to calculate phasor signals (Size: 1 by number of phasor signals to be calculated)
                %   Parameters.phasor{i}.mag: a struct array containing
                %   information on voltage phasor signal to calculate i^th
                %   phasor signal
                    % Parameters.phasor{i}.mag.PMU: a string specifying
                    % name of the PMU containing magnitude signal
                    % Parameters.phasor{i}.mag.Channel: a string specifying
                    % the channel of PMU that represents magnitude signal
                %   Parameters.phasor{i}.ang: a struct array containing
                %   information on the phasor angle signal to calculate
                %   i^th phasor signal
                    % Parameters.phasor{i}.ang.PMU: a string specifying
                    % name of the PMU consisting of angle signal
                    % Parameters.phasor{i}.ang.Channel: a string specifying
                    % the channel of PMU that represents angle signal               
                %   Parameters.phasor{i}.CustName: a string specifying name for the i^th customized signal               
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
%Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix.
%Modified on July 13, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Includes FlagBitCust variable
function PMUstruct = CreatePhasorCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

AvailablePMU = {PMUstruct.PMU_Name};

NumPhasor = length(Parameters.phasor);
if NumPhasor == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.phasor = {Parameters.phasor};
end

SigMat = [];
SignalType = cell(1,NumPhasor);
SignalUnit = cell(1,NumPhasor);
SignalName = cell(1,NumPhasor);
ErrFlagUser = zeros(NumPhasor,1);
PMUidxAll = [];
for PhasorIdx = 1:NumPhasor
    PMUidxMag = find(strcmp(Parameters.phasor{PhasorIdx}.mag.PMU,AvailablePMU));
    PMUidxAll = [PMUidxMag PMUidxAll];
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidxMag)
        warning(['PMU ' Parameters.phasor{PhasorIdx}.mag.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    PMUidxAng = find(strcmp(Parameters.phasor{PhasorIdx}.ang.PMU,AvailablePMU));
    PMUidxAll = [PMUidxAng PMUidxAll];
    % If the specified PMU is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(PMUidxAng)
        warning(['PMU ' Parameters.phasor{PhasorIdx}.ang.PMU ' could not be found, returning NaN and setting Flags.']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    SigIdxMag = find(strcmp(Parameters.phasor{PhasorIdx}.mag.Channel,PMUstruct(PMUidxMag).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdxMag)
        warning(['Signal ' Parameters.phasor{PhasorIdx}.mag.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    SigIdxAng = find(strcmp(Parameters.phasor{PhasorIdx}.ang.Channel,PMUstruct(PMUidxAng).Signal_Name));
    % If the specified signal is not in PMUstruct, skip the rest of the for 
    % loop so that Data remains NaNs and Flags remain set.
    if isempty(SigIdxAng)
        warning(['Signal ' Parameters.phasor{PhasorIdx}.ang.Channel ' could not be found, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    if isempty(SigMat)
        N = length(PMUstruct(PMUidxMag).Data(:,SigIdxMag));
        SigMat = NaN(N,NumPhasor);
        FlagMat = true(N,NumPhasor);
    else
        % Make sure dimensions match the data matrix. If not, skip the rest of the for 
        % loop so that Data remains NaNs and Flags remain set.
        if ~size(PMUstruct(PMUidxMag).Data(:,SigIdxMag),1) == N
            warning([Parameters.phasor{PhasorIdx}.mag.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
            ErrFlagUser(PhasorIdx) = 1;
            continue
        end

        % Make sure dimensions match the data matrix. If not, skip the rest of the for 
        % loop so that Data remains NaNs and Flags remain set.
        if ~size(PMUstruct(PMUidxAng).Data(:,SigIdxAng),1) == N
            warning([Parameters.phasor{PhasorIdx}.ang.Channel ' has a different length than the custom PMU Data field, returning NaN and setting Flags']);
            ErrFlagUser(PhasorIdx) = 1;
            continue
        end
    end
    
    % Make sure types make sense
    ErrFlag = 0;
    switch PMUstruct(PMUidxMag).Signal_Type{SigIdxMag}
        case 'VMP'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAP')
                SignalType{PhasorIdx} = 'VPP';
            else
                ErrFlag = 1;
            end
        case 'VMA'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAA')
                SignalType{PhasorIdx} = 'VPA';
            else
                ErrFlag = 1;
            end
        case 'VMB'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAB')
                SignalType{PhasorIdx} = 'VPB';
            else
                ErrFlag = 1;
            end
        case 'VMC'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'VAC')
                SignalType{PhasorIdx} = 'VPC';
            else
                ErrFlag = 1;
            end
        case 'IMP'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAP')
                SignalType{PhasorIdx} = 'IPP';
            else
                ErrFlag = 1;
            end
        case 'IMA'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAA')
                SignalType{PhasorIdx} = 'IPA';
            else
                ErrFlag = 1;
            end
        case 'IMB'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAB')
                SignalType{PhasorIdx} = 'IPB';
            else
                ErrFlag = 1;
            end
        case 'IMC'
            if strcmp(PMUstruct(PMUidxAng).Signal_Type{SigIdxAng},'IAC')
                SignalType{PhasorIdx} = 'IPC';
            else
                ErrFlag = 1;
            end
        otherwise
            % Magnitude type is not acceptable, skip the rest of the for
            % loop so that Data remains NaNs and Flags remain set.
            warning([Parameters.phasor{PhasorIdx}.mag.Channel ' is not a voltage or current magnitude, returning NaN and setting Flags']);
            ErrFlagUser(PhasorIdx) = 1;
            continue
    end
    if ErrFlag
        % Angle type is not acceptable, skip the rest of the for
        % loop so that Data remains NaNs and Flags remain set.
        warning([Parameters.phasor{PhasorIdx}.ang.Channel ' is not a voltage or current angle or does not match magnitude, returning NaN and setting Flags']);
        ErrFlagUser(PhasorIdx) = 1;
        continue
    end
    
    AngleUnits = PMUstruct(PMUidxAng).Signal_Unit{SigIdxAng};
    switch AngleUnits
        case 'DEG'
            % Convert to radians
            SigAngle = (PMUstruct(PMUidxAng).Data(:,SigIdxAng))*pi/180;
            FlagVec = sum(PMUstruct(PMUidxAng).Flag(:,SigIdxAng,:),3);
        case 'RAD'
            % Angle is already in radians, as desired
            SigAngle = PMUstruct(PMUidxAng).Data(:,SigIdxAng);
            FlagVec = sum(PMUstruct(PMUidxAng).Flag(:,SigIdxAng,:),3);
        otherwise
            % Units are not correct, skip the rest of the for loop so that
            % the Data remains NaNs and Flags remain set.
            warning([Parameters.phasor{PhasorIdx}.ang.Channel ' does not have units of RAD or DEG, returning NaN and setting Flags.']);
            ErrFlagUser(PhasorIdx) = 1;
            continue
    end
    
    SigMat(:,PhasorIdx) = PMUstruct(PMUidxMag).Data(:,SigIdxMag) .* exp(1i*SigAngle);
    
    FlagMat(:,PhasorIdx) = (FlagVec>0) | (sum(PMUstruct(PMUidxMag).Flag(:,SigIdxMag,:),3) > 0);   % Set flags 

    % Store SignalUnit
    SignalUnit{PhasorIdx} = PMUstruct(PMUidxMag).Signal_Unit{SigIdxMag};
    % Store SignalName
    SignalName{PhasorIdx} = Parameters.phasor{PhasorIdx}.CustName;
end

custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
if isempty(custPMUidx)
    % Initialize the custom PMU sub-structure and add it to the PMU structure
    % using some of the fields from an existing PMU sub-structure.
    RefIdx = [PMUidxAll 1];  % The 1 is included to prevent errors when PMUidx is empty
    PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(RefIdx(1)).Signal_Time,Num_Flags);
    custPMUidx = length(PMUstruct);
end

for PhasorIdx = 1:NumPhasor
    CheckSignalNameError(Parameters.phasor{PhasorIdx}.CustName, PMUstruct(custPMUidx).Signal_Name);
end

% Size of the current Data matrix for the custom PMU - N samples by NumSig
% signals
NcustSigs = size(PMUstruct(custPMUidx).Data,2);

% Make sure custom PMU is the right size (note that it may have existed
% before this function was called)
if size(PMUstruct(custPMUidx).Data,1) ~= size(SigMat,1)
    % Custom PMU is the wrong size
    warning('Custom signal does not match custom PMU size, returning NaN and setting Flags');
    ErrFlagUser(:) = 1;
    SigMat = NaN;
    FlagMat = true(size(PMUstruct(custPMUidx).Flag,1),NumPhasor);
end

% Store results in custom PMU
PMUstruct(custPMUidx).Signal_Name(NcustSigs+(1:NumPhasor)) = SignalName;
PMUstruct(custPMUidx).Signal_Type(NcustSigs+(1:NumPhasor)) = SignalType;
PMUstruct(custPMUidx).Signal_Unit(NcustSigs+(1:NumPhasor)) = SignalUnit;
PMUstruct(custPMUidx).Data(:,NcustSigs+(1:NumPhasor)) = SigMat;
for PhasorIndex = 1:NumPhasor
    if ErrFlagUser(PhasorIdx)
        PMUstruct(custPMUidx).Flag(:,NcustSigs+PhasorIndex,FlagBitCust(2)) = FlagMat(:,PhasorIndex);
    else
        PMUstruct(custPMUidx).Flag(:,NcustSigs+PhasorIndex,FlagBitCust(1)) = FlagMat(:,PhasorIndex);
    end
end