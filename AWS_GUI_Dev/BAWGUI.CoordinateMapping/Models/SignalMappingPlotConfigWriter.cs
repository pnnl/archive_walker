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
            var config = new XElement("SignalMappingPlotConfig");
            foreach (var signal in _mappingSignalsToBeWritten)
            {
                var sgnl = new XElement("Signal", new XElement("PMU", signal.PMUName),
                                                    new XElement("SignalName", signal.SignalName),
                                                    new XElement("Type", signal.MapPlotType.ToString()));
                var sites = new XElement("Sites");
                foreach (var lctn in signal.Locations)
                {
                    var site = new XElement("Site", new XElement("Name", lctn.Name),
                                                    new XElement("Latitude", lctn.Latitude),
                                                    new XElement("Longitude", lctn.Longitude));
                    sites.Add(site);
                }
                sgnl.Add(sites);
                config.Add(sgnl);
            }
            return config;
        }
    }
}
