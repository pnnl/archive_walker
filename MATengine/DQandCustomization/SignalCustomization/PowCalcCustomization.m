% function PMUstruct = PowCalcCustomization(PMUstruct,custPMUidx,Parameters)
% This function creates a customized signal using voltage and current
% phasor signals that gives power signal.
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
        % PMUstruct(i).Data: Matrix containing measurements by i^th PMU
                                %size: Number of data points by number of channels                              
        % PMUstruct(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged by different filter operation
                                %size: number of data points by number of channels by number of flag bits
        % PMUstruct.PMU_Name: a cell array containing strings specifying
        % name of PMUs
                                % size: Number of PMUs by 1
    % Parameters: structure containing user provided information to
    % create customized signal(s)
        % Parameters.powType: a string specifying type of power (complex,
        % apparent, real or reactive)
        % Parameters.Power: struct array containing information on signals
        % required to calculate power signals
        % (Size: 1 by number of power signals to be calculated)
                %   Parameters.Power{i}.Vphasor: a struct array containing
                %   information on voltage phasor signal to calculate i^th
                %   power signal
                        % Parameters.Power{i}.Vphasor.PMU: a string specifying
                        % name of the PMU containing voltage phasor signal
                        % Parameters.Power{i}.Vphasor.Channel: a string specifying
                        % the channel of PMU that represents voltage phasor signal
                %   Parameters.Power{i}.Iphasor: a struct array containing
                %   information on current phasor signal to calculate i^th
                %   power signal
                        % Parameters.Power{i}.Iphasor.PMU: a string specifying
                        % name of the PMU containing current phasor signal
                        % Parameters.Power{i}.Iphasor.Channel: a string specifying
                        % the channel of PMU that represents voltage phasor signal
                %   Parameters.Power{i}.Vmag: a struct array containing
                %   information on voltage magnitude signal to  calculate
                %   i^th power signal
                        % Parameters.Power{i}.Vmag.PMU: a string specifying
                        % name of the PMU containing voltage magnitude signal
                        % Parameters.Power{i}.Vmag.Channel: a string specifying
                        % the channel of PMU that represents voltage magnitude signal
                %   Parameters.Power{i}.Vmang: a struct array containing
                %   information on voltage angle signal to calculate
                %   i^th power signal
                        % Parameters.Power{i}.Vang.PMU: a string specifying
                        % name of the PMU consisting of voltage angle signal
                        % Parameters.Power{i}.Vang.Channel: a string specifying
                        % the ters.Power{i}.Imag: a struct array containing
                %   information on the current magnitude signal to
                %   calculate i^th power signal
                        % Parameters.Power{i}.Imag.PMU: a string specifying
                        % name of the PMU containing current magnitude signal
                        % Parameters.Power{i}.Imag.Channel: a string specifying
                        % the channel of PMU that represents current phasor signal                  
                %   Parameters.Power{i}.ang: a struct array containing
                %   information on the current angle signal to calculate i^th power signal
                        % Parameters.Power{i}.Iang.PMU: a string specifying
                        % name of the PMU containing current angle signal
                        % Parameters.Power{i}.Iang.Channel: a string specifying
                        % the channel of PMU that represents current angle signal       
                %   Parameters.Power{i}.CustName: a string specifying name for the i^th customized power signal               
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
function PMUstruct = PowCalcCustomization(PMUstruct,Num_Flags,Parameters,FlagBitCust)

NumPow = length(Parameters.power);
if NumPow == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.power = {Parameters.power};
end

for PowIdx = 1:NumPow  
    switch length(fieldnames(Parameters.power{PowIdx}))
        case 3
            % Should be a voltage phasor - current phasor pair
            [PMUstruct,ErrFlag] = PowCalcPhasor(PMUstruct,Parameters,PowIdx,Num_Flags,FlagBitCust);
        case 5
            % Should be voltage mag/ang - current mag/ang pair
            [PMUstruct,ErrFlag] = PowCalcMagAng(PMUstruct,Parameters,PowIdx,Num_Flags,FlagBitCust);
        otherwise
            ErrFlag = 1;
    end
    
    if ErrFlag
        % There was an error trying to calculate the power
        % Add a NaN signal to the custom PMU
        
        AvailablePMU = {PMUstruct.PMU_Name};
        custPMUidx = find(strcmp(Parameters.CustPMUname, AvailablePMU));
        if isempty(custPMUidx)
            % The custom PMU has not been created
            PMUstruct = InitCustomPMU(PMUstruct,Parameters.CustPMUname,PMUstruct(1).Signal_Time,Num_Flags);
            custPMUidx = length(PMUstruct);
        end
        
        SignalName = Parameters.power{PowIdx}.CustName;
        CheckSignalNameError(SignalName, PMUstruct(custPMUidx).Signal_Name);
        
        % Size of the current Data matrix for the custom PMU - N samples by NumSig signals
        NumSig = size(PMUstruct(custPMUidx).Data,2);

        % Fill in the custom PMU
        PMUstruct(custPMUidx).Signal_Name{NumSig+1} = SignalName;
        PMUstruct(custPMUidx).Signal_Unit{NumSig+1} = 'O';
        PMUstruct(custPMUidx).Signal_Type{NumSig+1} = 'OTHER';
        PMUstruct(custPMUidx).Data(:,NumSig+1) = NaN;
        % FlagBitCust(2) indicates error in user input
        PMUstruct(custPMUidx).Flag(:,NumSig+1,FlagBitCust(2)) = true;
    end
end