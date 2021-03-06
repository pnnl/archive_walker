% function PMUstruct = AngleUnitCustomization(PMUstruct,custPMUidx,Parameters)
% This function either creates customized signal(s) by changing the unit of the angle of given signal(s) or just changes the unit of the angle of given signal(s).
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
        % signals whose unit is to be converted. (size: 1 by
        % number of signals to be converted)
                    % Parameters.ToConvert{i}.PMU: a string specifying
                    % name of the PMU consisting of i^th signal to be converted
                    % Parameters.ToConvert{i}.Channel: a string specifying
                    % the channel of PMU that represents i^th signal to be converted   
                    % Parameters.ToConvert{i}.CustName: a string specifying
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
function PMUstruct = AngleUnitCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

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
        %flags is set for customized signal for error in user input
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

        switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
            case 'DEG'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*pi/180;
                NewUnit = 'RAD';
            case 'RAD'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*180/pi;
                NewUnit = 'DEG';
            otherwise
                warning([PMUstruct(PMUidx).Signal_Unit{SigIdx} ' is not an appropriate unit. Values were set to NaN and Flags set.']);
                continue
        end

        % Check to make sure signal length is compatible
        if length(NewAng) ~= size(PMUstruct(custPMUidx).Data,1)
            warning('Length of signal does not match the custom PMU. Values were set to NaN and Flags set.');
            continue
        end
        
        PMUstruct(custPMUidx).Data(:,NcustSigs+1) = NewAng;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,:) = PMUstruct(PMUidx).Flag(:,SigIdx,:);
        FlagVec = sum(PMUstruct(PMUidx).Flag(:,SigIdx,:),3) > 0;
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(1)) = FlagVec; %flags is set for customized signal obtained from input signal with flagged data
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,FlagBitCust(2)) = false;
        PMUstruct(custPMUidx).Signal_Type{NcustSigs+1} = PMUstruct(PMUidx).Signal_Type{SigIdx};
        PMUstruct(custPMUidx).Signal_Unit{NcustSigs+1} = NewUnit;
    else
        AvailablePMU = {PMUstruct.PMU_Name};
        
        PMUidx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.PMU,AvailablePMU));
         if isempty(PMUidx)
            warning(['PMU ' Parameters.ToConvert{ToConvertIdx}.PMU ' could not be found. No changes made']);
            continue
        end
        
        SigIdx = find(strcmp(Parameters.ToConvert{ToConvertIdx}.Channel,PMUstruct(PMUidx).Signal_Name));
        % If the specified signal is not in PMUstruct, issue warning and do nothing
        if isempty(SigIdx)
            warning(['Signal ' Parameters.ToConvert{ToConvertIdx}.Channel ' could not be found. No changes made']);
            continue
        end
        switch PMUstruct(PMUidx).Signal_Unit{SigIdx}
            case 'DEG'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*pi/180;
                NewUnit = 'RAD';
            case 'RAD'
                NewAng = PMUstruct(PMUidx).Data(:,SigIdx)*180/pi;
                NewUnit = 'DEG';
            otherwise
                warning([PMUstruct(PMUidx).Signal_Unit{SigIdx} ' is not an appropriate unit. No changes made']);
            continue
        end
        % A custom signal name was not provided, so replace the input signal and its unit
        PMUstruct(PMUidx).Data(:,SigIdx) = NewAng;
        PMUstruct(PMUidx).Signal_Unit{SigIdx} = NewUnit;
    end
end