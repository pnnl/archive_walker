% function PMU = CustomizationStep(PMU,custPMUidx,StageStruct)
% This function creates customized signal using PMU measurements specified
% in XML files
%
% Inputs:
	% PMU: a struct array of dimension 1 by Number of PMUs      
        % PMU(i).PMU_Name: a string specifying name of i^th PMU
        % PMU(i).Data: Matrix containing data measured by the i^th PMU
        % PMU(i).Flag: 3-dimensional matrix indicating i^th PMU
        % measurement flagged by different filter operation (size: number
        % of data points by number of channels by number of flag bits)
    % StageStruct: struct array containing information on data quality filters and customization
    % operation to be carried out for a single stage
        % StageStruct.Customization: struct array containing information on different
        % customization operation (size: 1 by number of customization
        % operation)
            % StageStruct.Customization{i}.Name: a string specifying type of i^th customization operation
            % StageStruct.Customization{i}.Parameters: a struct array
            % containing user-specified parameters for i^th customization operation          
    % custPMUidx: numerical identifier for PMU that would store customized signal
    % Flag_Bit: Cotains information on flag bits used by different data quality check filters
%
% Outputs:
    % PMU
%    
%Created by: Jim Follum(james.follum@pnnl.gov)
%Modified on June 7, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
    %1. Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix

function PMU = CustomizationStep(PMU,Num_Flags,StageStruct)

% The number of flags includes two bits reserved for customizations. The
% first indicates that input signals were flagged and the second indicates
% that there was a problem with the user input. 
FlagBitCust = [Num_Flags-1; Num_Flags];

NumCusts = length(StageStruct.Customization);
if NumCusts == 1
    % By default, the contents of StageStruct.Customization
    % would not be in a cell array because length is one. This 
    % makes it so the same indexing can be used in the following for loop.
    StageStruct.Customization = {StageStruct.Customization};
end

for CustIdx = 1:NumCusts
    % Parameters for the customization - the structure contents are
    % specific to the customization
    Parameters = StageStruct.Customization{CustIdx}.Parameters;
    
    % Find the appropriate filter and apply it for each of the
    % specified PMUs
    switch StageStruct.Customization{CustIdx}.Name
        case 'ScalarRep'
            PMU = ScalarRep(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'Addition'
            PMU = AddCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'Subtraction'
            PMU = SubtractionCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'Multiplication'
            PMU = MultCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'Division'
            PMU = DivisionCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'Exponent'
            PMU = ExpCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case {'SignReversal' 'AbsVal' 'RealComponent' 'ImagComponent' 'ComplexConj'}
            PMU = CommonCustomization(PMU,Num_Flags,Parameters,StageStruct.Customization{CustIdx}.Name,FlagBitCust);
        case 'Angle'
            PMU = AngleCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'CreatePhasor'
            PMU = CreatePhasorCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'PowerCalc'
            PMU = PowCalcCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'SpecTypeUnit'
            PMU = SpecTypeUnitCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'MetricPrefix'
            PMU = PrefixCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'AngleConversion'
            PMU = AngleUnitCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'ReplicateSignal'
            PMU = ReplicateSignalCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'PointOnWavePower'
            PMU = PointOnWavePowerCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'VoltageAngleGraph'
            PMU = VAgraphCustomization(PMU,Num_Flags,Parameters,FlagBitCust);
        case 'PCA'
            PMU = PCAcustomization(PMU,Num_Flags,Parameters,FlagBitCust);
    end
end