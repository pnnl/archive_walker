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