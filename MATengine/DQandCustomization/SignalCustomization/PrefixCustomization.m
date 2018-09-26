% function PMUstruct = PrefixCustomization(PMUstruct,custPMUidx,Parameters)
% This function changes metric prefix of the unit of given signal(s).
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
        % Parameters.ToConvert: a struct array containing information on 
        % signals whose unit's metric prefix is to be converted . (size: 1 by
        % number of signals to be converted)
                    % Parameters.ToConvert{i}.PMU: a string specifying
                    % name of the PMU consisting of i^th signal to be
                    % converted
                    % Parameters.ToConvert{i}.Channel: a string specifying
                    % the channel of PMU that represents i^th signal to be converted   
                    % Parameters.ToConvert{i}.CustName: a string specifying
                    % name for the i^th customized signal
                    % Parameters.ToConvert{i}.NewUnit: a string specifying
                    % new unit for the i^th customized signal
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
%Changed the flag matrix from a 2 dimensional double matrix to a 3
%dimensional logical matrix (3rd dimension represents flag bit)
%Modified on July 13, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Includes FlagBitCust variable
function PMUstruct = PrefixCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

NumToConvert = length(Parameters.ToConvert);
if NumToConvert == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.ToConvert = {Parameters.ToConvert};
end

for ToConvertIdx = 1:NumToConvert
    if isfield(Parameters.ToConvert{ToConvertIdx},'CustName')
        % A custom signal name was provided, so instead of replacing the input
        % signal add a new signal to the custom signal PMU
        
        AvailablePMU = {PMUstruct.PMU_Name};

        % Get the index of the PMU containing the signal to be customize
        PMUidx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.PMU,AvailablePMU));

        custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
        if isempty(custPMUidx)
            % Initialize the custom PMU sub-structure and add it to the PMU structure
            % using some of the fields from an existing PMU sub-structure.
            RefIdx = [PMUidx 1];  % The 1 is included to prevent errors when PMUidx is empty
            PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(RefIdx(1)).Signal_Time,Num_Flags);
            custPMUidx = length(PMUstruct);
        end

        % Make sure the custom signal name isn't already taken
        CheckSignalNameError(Parameters.ToConvert{ToConvertIdx}.CustName, PMUstruct(custPMUidx).Signal_Name);

        % Size of the current Data matrix for the custom PMU - N samples by NcustSigs signals
        NcustSigs = size(PMUstruct(custPMUidx).Data,2);
        
        % Set defaults in case of a problem with the specifications
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NaN;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = true;
        PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = 'OTHER';
        PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = 'O';
        PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = Parameters.ToConvert{ToConvertIdx}.CustName;
        
        % If the specified PMU is not in PMUstruct, issue warning
        if isempty(PMUidx)
            warning(['PMU ' Parameters.ToConvert{ToConvertIdx}.PMU ' could not be found. Values were set to NaN and Flags set.']);
            continue
        end
        
        SigIdx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
        % If the specified signal is not in PMUstruct, issue warning and do nothing
        if isempty(SigIdx)
            warning(['Signal ' Parameters.ToConvert{ToConvertIdx}.Channel ' could not be found. Values were set to NaN and Flags set.']);
            continue
        end
        % This coding approach is bulky, but it will allow for expansion to
        % more than two unit options for each type.
        NewUnit = Parameters.ToConvert{ToConvertIdx}.NewUnit;
        ErrFlagIdentical = 0;
        ErrFlagIncompatible = 0;
        switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
            case 'V'
                % From V to NewUnit
                if strcmp(NewUnit,'kV')
                    CustSig = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
                elseif strcmp(NewUnit,'V')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'kV'
                % From kV to NewUnit
                if strcmp(NewUnit,'V')
                    CustSig = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
                elseif strcmp(NewUnit,'kV')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'A'
                % From A to NewUnit
                if strcmp(NewUnit,'kA')
                    CustSig = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
                elseif strcmp(NewUnit,'A')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'kA'
                % From kA to NewUnit
                if strcmp(NewUnit,'A')
                    CustSig = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
                elseif strcmp(NewUnit,'kA')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'mHz/sec'
                % From Hz/sec to NewUnit
                if strcmp(NewUnit,'Hz/sec')
                    CustSig = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
                elseif strcmp(NewUnit,'mHz/sec')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'Hz/sec'
                % From mHz/sec to NewUnit
                if strcmp(NewUnit,'mHz/sec')
                    CustSig = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
                elseif strcmp(NewUnit,'Hz/sec')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            otherwise
                warning([PMUstruct(custPMUidx).Signal_Unit{SigIdx} ' is an unacceptable input unit. Values were set to NaN and Flags set.']);
                continue
        end
        
        if ErrFlagIdentical
            warning(['Old and new unit for ' Parameters.ToConvert{ToConvertIdx}.Channel ' are identical. Values were set to NaN and Flags set.']);
            continue
        end
        
        if ErrFlagIncompatible
            warning(['Old and new unit for ' Parameters.ToConvert{ToConvertIdx}.Channel ' are incompatible. Values were set to NaN and Flags set.']);
            continue
        end

        % Check to make sure signal length is compatible
        if length(CustSig) ~= size(PMUstruct(custPMUidx).Data,1)
            warning('Length of signal does not match the custom PMU. Values were set to NaN and Flags set.');
            continue
        end
        
        % Assign the new unit
        
        % All checks passed, set type and unit for input signal or create a new
        % custom signal with the type and unit
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = CustSig;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,:) = PMUstruct(PMUidx).Flag(:,SigIdx,:);
        FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3)>0;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(1)) = FlagVec;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = false;
        PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = PMUstruct(PMUidx).Signal_Type{SigIdx};
        PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = Parameters.ToConvert{ToConvertIdx}.NewUnit;
        PMUstruct(custPMUidx).Signal_Name{NcustSigs+1} = Parameters.ToConvert{ToConvertIdx}.CustName;
    else
        AvailablePMU = {PMUstruct.PMU_Name};
        
        PMUidx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.PMU,AvailablePMU));
        % If the specified PMU is not in PMUstruct, issue warning and do nothing
        if isempty(PMUidx)
            warning(['PMU ' Parameters.ToConvert{ToConvertIdx}.PMU ' could not be found. No changes made.']);
            continue
        end
        
        SigIdx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
        % If the specified signal is not in PMUstruct, issue warning and do nothing
        if isempty(SigIdx)
            warning(['Signal ' Parameters.ToConvert{ToConvertIdx}.Channel ' could not be found. No changes made.']);
            continue
        end
        % This coding approach is bulky, but it will allow for expansion to
        % more than two unit options for each type.
        NewUnit = Parameters.ToConvert{ToConvertIdx}.NewUnit;
        ErrFlagIdentical = 0;
        ErrFlagIncompatible = 0;
        switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
            case 'V'
                % From V to NewUnit
                if strcmp(NewUnit,'kV')
                    PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
                elseif strcmp(NewUnit,'V')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'kV'
                % From kV to NewUnit
                if strcmp(NewUnit,'V')
                    PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
                elseif strcmp(NewUnit,'kV')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'A'
                % From A to NewUnit
                if strcmp(NewUnit,'kA')
                    PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
                elseif strcmp(NewUnit,'A')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'kA'
                % From kA to NewUnit
                if strcmp(NewUnit,'A')
                    PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
                elseif strcmp(NewUnit,'kA')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'mHz/sec'
                % From Hz/sec to NewUnit
                if strcmp(NewUnit,'Hz/sec')
                    PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)/1000;
                elseif strcmp(NewUnit,'mHz/sec')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            case 'Hz/sec'
                % From mHz/sec to NewUnit
                if strcmp(NewUnit,'mHz/sec')
                    PMUstruct(PMUidx).Data(:,SigIdx) = PMUstruct(PMUidx).Data(:,SigIdx)*1000;
                elseif strcmp(NewUnit,'Hz/sec')
                    ErrFlagIdentical = 1;
                else
                    ErrFlagIncompatible = 1;
                end
            otherwise
                warning([PMUstruct(PMUidx).Signal_Unit{SigIdx} ' is an unacceptable input unit, no change made.']);
                continue
        end
        
        if ErrFlagIdentical
            warning(['Old and new unit for ' Parameters.ToConvert{ToConvertIdx}.Channel ' are identical. No changes made.']);
            continue
        end
        
        if ErrFlagIncompatible
            warning(['Old and new unit for ' Parameters.ToConvert{ToConvertIdx}.Channel ' are incompatible. No changes made.']);   
            continue
        end
        % A custom signal name was not provided, so replace the input signal's unit
        PMUstruct(PMUidx).Signal_Unit{SigIdx} = Parameters.ToConvert{ToConvertIdx}.NewUnit;
    end
end