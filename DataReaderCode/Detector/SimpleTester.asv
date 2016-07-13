clear;

%% Options

% Make changes to DetectorConfig.XML to check your coding and different
% options.
DetectorXMLfile = 'C:\Users\foll154\Documents\BPA Oscillation App\CodeForProject2\DataReaderCode\ConfigXML\DetectorConfig.XML';

% Don't worry about what's inside this XML, just specify the filepath 
DataReadXMLfile = 'C:\Users\foll154\Documents\BPA Oscillation App\CodeForProject2\DataReaderCode\ConfigXML\ConfigXML.XML';
% Don't worry about looking at different minutes of data, just get the code
% to work
PMUfilePath = 'C:\Users\foll154\Documents\BPA Oscillation App\CodeForProject2\DataReaderCode\OriginalData\2015\151017\WISPDitt_20151017_123000.pdat';

% Choose the detector type you're working on:
DetectorType = 'OutOfRangeGeneral';
% DetectorType = 'Ringdown';
% DetectorType = 'OutOfRangeFrequency';
% DetectorType = 'OutOfRangeVoltage';

% You can add multiples of each detector to DetectorConfig.XML to try out
% different things. This is the index for the detector.
DetectorNumber = 1;

%% Run Detector

DataXML = fun_xmlread_comments(DataReadXMLfile);
% set the counter
count = 0;
% get the number of filtering stages
NumStages = length(DataXML.Configuration.Stages);
for StageId = 1:NumStages
    if isfield(DataXML.Configuration.Stages{StageId},'Filter')
        % number of filters used in this stage
        NumFilters = length(DataXML.Configuration.Stages{StageId}.Filter);
        if NumFilters ==1
            % By default, the contents of StageStruct.Customization
            % would not be in a cell array because length is one. This
            % makes it so the same indexing can be used in the following for loop.
            DataXML.Configuration.Stages{StageId}.Filter = {DataXML.Configuration.Stages{StageId}.Filter};
        end
        for FilterIdx = 1:NumFilters
            % count filters that used FlagBit as a parameter
            if isfield(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters,'FlagBit')
                Flag_Bit(count+1) = str2num(DataXML.Configuration.Stages{StageId}.Filter{FilterIdx}.Parameters.FlagBit);
                count = count + 1;
            end
        end
    end
end
% save Flag_Bit information into DataXML structure
DataXML.Flag_Bit = Flag_Bit;
PMU = createPdatStruct(PMUfilePath,DataXML);

DetectorXML = fun_xmlread_comments(DetectorXMLfile);
Parameters = DetectorXML.Configuration.(DetectorType);
if length(Parameters) == 1
    Parameters = {Parameters};
end

% [DetectionResults, AdditionalOutput] = PeriodogramDetector(PMU,Parameters);
[DetectionResults, AdditionalOutput] = eval([DetectorType 'Detector(PMU,Parameters{DetectorNumber})']);