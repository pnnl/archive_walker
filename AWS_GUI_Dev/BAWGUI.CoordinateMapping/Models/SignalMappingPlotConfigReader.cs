using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BAWGUI.CoordinateMapping.Models
{
    public class SignalMappingPlotConfigReader
    {
        private string configFilePath;
        private XElement _config;

        public List<SignalCoordsMappingModel> SignalMappingList { get; set; }
        public SignalMappingPlotConfigReader()
        {
            SignalMappingList = new List<SignalCoordsMappingModel>();
        }
        public SignalMappingPlotConfigReader(string configFilePath) : this()
        {
            this.configFilePath = configFilePath;
            _readSignalMappingConfigFromFile();
        }

        public SignalMappingPlotConfigReader(XElement xElement) : this()
        {
            _config = xElement;
            _readSignalSiteMappingConfig();
        }

        public void ReadSignalMappingConfig(string configFilePath)
        {
            this.configFilePath = configFilePath;
            _readSignalMappingConfigFromFile();
        }
        private void _readSignalMappingConfigFromFile()
        {
            var configData = XDocument.Load(configFilePath);
            _config = configData.Element("Config").Element("SignalMappingPlotConfig");
            _readSignalSiteMappingConfig();
        }

        private void _readSignalSiteMappingConfig()
        {
            if (_config != null)
            {
                var signals = _config.Elements("Signal");
                foreach (var signal in signals)
                {
                    var newSignalMapping = new SignalCoordsMappingModel();
                    newSignalMapping.PMUName = signal.Element("PMU").Value;
                    newSignalMapping.SignalName = signal.Element("SignalName").Value;
                    var t = signal.Element("Type");
                    if (t != null)
                    {
                        newSignalMapping.Type = (SignalMapPlotType)Enum.Parse(typeof(SignalMapPlotType), t.Value);
                    }
                    newSignalMapping.Locations = SitesReader.ReadSites(signal);
                    SignalMappingList.Add(newSignalMapping);
                }
            }
        }

        public List<SignalCoordsMappingModel> GetSignalCoordsMappingModel()
        {
            return SignalMappingList;
        }
    }
}
