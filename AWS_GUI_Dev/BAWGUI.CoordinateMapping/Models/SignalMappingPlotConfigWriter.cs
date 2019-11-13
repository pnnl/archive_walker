using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BAWGUI.CoordinateMapping.Models
{
    public class SignalMappingPlotConfigWriter
    {
        private ObservableCollection<SignalSignatureViewModel> _mappingSignalsToBeWritten;

        public SignalMappingPlotConfigWriter()
        {
            _errorMessages = new List<string>();
        }

        public SignalMappingPlotConfigWriter(ObservableCollection<SignalSignatureViewModel> uniqueMappingSignals) : this()
        {
            this._mappingSignalsToBeWritten = uniqueMappingSignals;
        }

        public XElement WriteConfigToXMLFormat(ObservableCollection<SignalSignatureViewModel> uniqueMappingSignals)
        {
            this._mappingSignalsToBeWritten = uniqueMappingSignals;
            return _writeMappingSignalConfigToXMLFormat();
        }

        public XElement WriteConfigToXMLFormat()
        {
            return _writeMappingSignalConfigToXMLFormat();
        }

        private XElement _writeMappingSignalConfigToXMLFormat()
        {
            var config = new XElement("SignalMappingPlotConfig", string.Empty);
            foreach (var signal in _mappingSignalsToBeWritten)
            {
                var type = signal.MapPlotType;
                var numberOfSites = signal.Locations.Count();
                bool dummySiteFound = false;
                var sites = new XElement("Sites");
                foreach (var lctn in signal.Locations)
                {
                    if (lctn == CoreUtilities.DummySiteCoordinatesModel)
                    {
                        dummySiteFound = true;
                        _errorMessages.Add(string.Format("Invalid site is setup with signal {1} of PMU {0} and the type is supposed to be {2}. This signal might be drawn incorrectly on the map", signal.PMUName, signal.SignalName, signal.MapPlotType.ToString()));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(lctn.Name) || string.IsNullOrEmpty(lctn.Latitude) || string.IsNullOrEmpty(lctn.Longitude))
                        {
                            _errorMessages.Add(string.Format("Site {0} with latitude {1} and longitude {2} is set up with signal {3} of PMU {4}, which might be drawn incorrectly on the map", lctn.Name, lctn.Latitude, lctn.Longitude, signal.PMUName, signal.SignalName));
                            continue;
                        }
                        else
                        {
                            var site = new XElement("Site", new XElement("Name", lctn.Name),
                                                            new XElement("Latitude", lctn.Latitude),
                                                            new XElement("Longitude", lctn.Longitude));
                            sites.Add(site);
                        }
                    }
                }
                var siteCount = sites.Elements("Site").Count();
                if (dummySiteFound && siteCount == 1)
                {
                    _errorMessages.Add(string.Format("The mapping type of signal {0} of PMU {1} has been changed from {2} to Dot", signal.PMUName, signal.SignalName, type.ToString()));
                    type = SignalMapPlotType.Dot;
                    dummySiteFound = false;
                }
                if (dummySiteFound && siteCount == 2 && type != SignalMapPlotType.Line)
                {
                    _errorMessages.Add(string.Format("The mapping type of signal {0} of PMU {1} has been changed from {2} to Line", signal.PMUName, signal.SignalName, type.ToString()));
                    type = SignalMapPlotType.Line;
                    dummySiteFound = false;
                }
                var sgnl = new XElement("Signal", new XElement("PMU", signal.PMUName),
                                                    new XElement("SignalName", signal.SignalName),
                                                    new XElement("Type", type));
                if (sites.HasElements)
                {
                    sgnl.Add(sites);
                }
                else
                {
                    continue;
                }
                if (sgnl.HasElements)
                {
                    config.Add(sgnl);
                }
            }
            return config;
        }
        private List<string> _errorMessages;
        public List<string> GetErrorMessages()
        { 
            return _errorMessages;
        }
    }
}
