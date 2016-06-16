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
    % custPMUidx: numerical identifier for PMU that would store customized signal
% 
% Outputs:
    % PMUstruct
%     
%Created by: Jim Follum (james.follum@pnnl.gov)
%Modified on June 3, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
%Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix.

function PMUstruct = PowCalcCustomization(PMUstruct,custPMUidx,Parameters)

PowType = Parameters.PowType;

AvailablePMU = {PMUstruct.PMU_Name};

NumPow = length(Parameters.power);
if NumPow == 1
    % By default, the contents of Parameters.term would not be
    % in a cell array because length is one. This makes it so the same
    % indexing can be used in the following for loop.
    Parameters.power = {Parameters.power};
end

for PowIdx = 1:NumPow
    ErrFlag = 0; % Error flag
    for junkIdx = 1:1  % Allows break from nested switch
    switch length(fieldnames(Parameters.power{PowIdx}))
        case 3
            % Should be a voltage phasor - current phasor pair
            % Get the structures for each

            if isfield(Parameters.power{PowIdx},'Vphasor')
                VphasorStruct = Parameters.power{PowIdx}.Vphasor;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Iphasor')
                IphasorStruct = Parameters.power{PowIdx}.Iphasor;
            else
                ErrFlag = 1;
                break
            end
            
            [Signal,Flag,SignalType,SignalUnit] = PowCalcPhasor(VphasorStruct,IphasorStruct,PMUstruct,custPMUidx,PowType);
        case 5
            % Should be voltage mag/ang - current mag/ang pair
            % Get the structures for each

            if isfield(Parameters.power{PowIdx},'Vmag')
                VmagStruct = Parameters.power{PowIdx}.Vmag;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Vang')
                VangStruct = Parameters.power{PowIdx}.Vang;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Imag')
                ImagStruct = Parameters.power{PowIdx}.Imag;
            else
                ErrFlag = 1;
                break
            end

            if isfield(Parameters.power{PowIdx},'Iang')
                IangStruct = Parameters.power{PowIdx}.Iang;
            else
                ErrFlag = 1;
                break
            end
            
            [Signal,Flag,SignalType,SignalUnit] = PowCalcMagAng(VmagStruct,VangStruct,ImagStruct,IangStruct,PMUstruct,custPMUidx,PowType);
        otherwise
            ErrFlag = 1;
    end
    end
    
    % Size of the current Data matrix and number of flags for the custom PMU - N samples by NcustSigs signals by NFLags flags
    [N,NcustSigs,NFlags] = size(PMUstruct(custPMUidx).Flag);
    
    % Check error flag
    if ErrFlag
        warning('Voltages and currents were not specified properly, returning NaN and setting Flags.');
        SignalType = 'OTHER';
        SignalUnit = 'O';
        Signal = NaN*ones(N,1);
        Flag = true(N,1);
    end
    
    
    % Store results in custom PMU
    PMUstruct(custPMUidx).Data(:,NcustSigs+1) = Signal;
    if strcmp(SignalType, 'OTHER')
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags) = Flag; %indicating error in user input
    else
        PMUstruct(custPMUidx).Flag(:,NcustSigs+1,NFlags-1) = Flag; %indicating error in input signal
    end
    PMUstruct(custPMUidx).Signal_Name(NcustSigs+1) = {Parameters.power{PowIdx}.CustName};
    PMUstruct(custPMUidx).Signal_Type(NcustSigs+1) = {SignalType};
    PMUstruct(custPMUidx).Signal_Unit(NcustSigs+1) = {SignalUnit};
end