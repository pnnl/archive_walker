using BAWGUI.Core;
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
        }

        public SignalMappingPlotConfigWriter(ObservableCollection<SignalSignatureViewModel> uniqueMappingSignals)
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
                var sgnl = new XElement("Signal", new XElement("PMU", signal.PMUName),
                                                    new XElement("SignalName", signal.SignalName),
                                                    new XElement("Type", signal.MapPlotType.ToString()));
                var sites = new XElement("Sites");
                foreach (var lctn in signal.Locations)
                {
                    if (string.IsNullOrEmpty(lctn.Name) || string.IsNullOrEmpty(lctn.Latitude) || string.IsNullOrEmpty(lctn.Longitude))
                    {
                        break;
                    }
                    else
                    {
                        var site = new XElement("Site", new XElement("Name", lctn.Name),
                                                        new XElement("Latitude", lctn.Latitude),
                                                        new XElement("Longitude", lctn.Longitude));
                        sites.Add(site);
                    }
                }
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
    }
}
