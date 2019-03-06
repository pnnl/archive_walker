function Data = ReadHistorian(Configuration)
%% GetMeasurementDetails from xml
ActiveMeasurements = GetMeasurementDetails(Configuration.instanceName,Configuration.SystemXML);
AMList             = cell2mat({ActiveMeasurements.PointID});
if isempty(Configuration.measurementIDs)
    IDList = AMList;
    nm = length(ActiveMeasurements);
else
    IDList = str2num(Configuration.measurementIDs);
    nm = length(IDList);
end

%% GetDataTable from OpenHistorian
if isempty(Configuration.datacsv)
    Configuration.datacsv = '.\datalist.csv';
    GetHistorian.GetHistorianMeasurement.Main_CSV(Configuration.datacsv,Configuration.historianServer,Configuration.instanceName,Configuration.startTime,Configuration.stopTime,Configuration.measurementIDs);
    DataTable = readtable(Configuration.datacsv,'Delimiter',',','Format','%d %s %f');
    delete datalist.csv
else
    GetHistorian.GetHistorianMeasurement.Main_CSV(Configuration.datacsv,Configuration.historianServer,Configuration.instanceName,Configuration.startTime,Configuration.stopTime,Configuration.measurementIDs);
    DataTable = readtable(Configuration.datacsv,'Delimiter',',','Format','%d %s %f');
end


%% Combine DataTable and Measurement Details
allocCell = cell(nm,1);
Data  = struct('PointID',allocCell,'PointTag',allocCell,'Device',allocCell, ...
    'FramesPerSecond',allocCell,'SignalType',allocCell,'EngineeringUnits',allocCell, ...
    'TimeSeries',allocCell);

for i = 1: nm
    ID = IDList(i);
    IndexM  = AMList == ID;
    IndexD  = DataTable.PointID == ID;
    Data(i).PointID          = ActiveMeasurements(IndexM).PointID;
    Data(i).PointTag         = ActiveMeasurements(IndexM).MeasurementDetails.PointTag;
    Data(i).Device           = ActiveMeasurements(IndexM).MeasurementDetails.Device;
    Data(i).FramesPerSecond  = str2num(ActiveMeasurements(IndexM).MeasurementDetails.FramesPerSecond);
    Data(i).SignalType       = ActiveMeasurements(IndexM).MeasurementDetails.SignalType;
    try
        Data(i).EngineeringUnits = ActiveMeasurements(IndexM).MeasurementDetails.EngineeringUnits;
    catch
        Data(i).EngineeringUnits = '';
    end
    Data(i).TimeSeries       = DataTable(IndexD,2:3);
end

end


function ActiveMeasurements = GetMeasurementDetails(instanceName,SystemConfiguration)
% Read Active Measurement Details From: C:\Program Files\openHistorian\ConfigurationCache\SystemConfiguration.xml
% A structure array is returned containing the details for each measurement

if nargin > 1
    xmlpath = SystemConfiguration;
else
    xmlpath = 'C:\Program Files\openHistorian\ConfigurationCache\SystemConfiguration.xml';
end

%% Processing SystemConfiguration.xml
xdoc           = xmlread(xmlpath);
xroot          = xdoc.item(0).getChildNodes;
numChildNodes  = xroot.getLength; % Children:Iaon
MeasurementIDs = [];

for count = 1:numChildNodes
    x1 = xroot.item(count-1);
    if strcmpi(x1.getNodeName, 'ActiveMeasurements')
        x2   = x1.getChildNodes; % Children: ActiveMeasurements
        numChildNodes1 = x2.getLength;
        for count1 = 1:numChildNodes1
            x3 = x2.item(count1-1);
            if strcmpi(x3.getNodeName, 'ID')
                if contains(char(x3.item(0).getData), instanceName)
                    MeasurementIDs     = [MeasurementIDs;count-1];
                end
            end
        end
    end
end

allocCell = cell(length(MeasurementIDs),1);
ActiveMeasurements = struct('MeasurementDetails', allocCell);

for count = 1:length(MeasurementIDs)
    theChild = xroot.item(MeasurementIDs(count)).getChildNodes;
    s = struct;
    for count1 = 1:theChild.getLength
        if theChild.item(count1-1).getChildNodes.getLength
            s.(char(theChild.item(count1-1).getNodeName)) ...
                = char(theChild.item(count1-1).item(0).getData);
        end
    end
    ActiveMeasurements(count).MeasurementDetails = s;
    PointID = strsplit(s.ID,':');
    ActiveMeasurements(count).PointID = str2num(PointID{2});
end
end