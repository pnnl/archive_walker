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
%
% Outputs:
    % PMU
%    
%Created by: Jim Follum(james.follum@pnnl.gov)
%Modified on June 7, 2016 by Urmila Agrawal(urmila.agrawal@pnnl.gov):
    %1. Changed the flag matrix from a 2 dimensional double matrix to a 3 dimensional logical matrix

function PMU = CustomizationStep(PMU,custPMUidx,StageStruct)

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
            PMU(custPMUidx) = ScalarRep(PMU(custPMUidx),Parameters);
        case 'Addition'
            PMU = AddCustomization(PMU,custPMUidx,Parameters);
        case 'Subtraction'
            PMU = SubtractionCustomization(PMU,custPMUidx,Parameters);
        case 'Multiplication'
            PMU = MultCustomization(PMU,custPMUidx,Parameters);
        case 'Division'
            PMU = DivisionCustomization(PMU,custPMUidx,Parameters);
        case 'Exponent'
            PMU = ExpCustomization(PMU,custPMUidx,Parameters);
        case {'SignReversal' 'AbsVal' 'RealComponent' 'ImagComponent' 'ComplexConj'}
            PMU = CommonCustomization(PMU,custPMUidx,Parameters,StageStruct.Customization{CustIdx}.Name);
        case 'Angle'
            PMU = AngleCustomization(PMU,custPMUidx,Parameters);
        case 'CreatePhasor'
            PMU = CreatePhasorCustomization(PMU,custPMUidx,Parameters);
        case 'PowerCalc'
            PMU = PowCalcCustomization(PMU,custPMUidx,Parameters);
        case 'SpecTypeUnit'
            PMU = SpecTypeUnitCustomization(PMU,custPMUidx,Parameters);
        case 'MetricPrefix'
            PMU = PrefixCustomization(PMU,custPMUidx,Parameters);
        case 'AngleConversion'
            PMU = AngleUnitCustomization(PMU,custPMUidx,Parameters);
    end
end