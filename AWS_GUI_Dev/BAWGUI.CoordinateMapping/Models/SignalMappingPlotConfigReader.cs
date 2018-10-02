using BAWGUI.Core.Models;
using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BAWGUI.CoordinateMapping.Models
{
    public class SignalMappingPlotConfigReader
    {
        private string configFilePath;
        public List<SignalCoordsMappingModel> SignalMappingList { get; set; }
        public SignalMappingPlotConfigReader()
        {
            SignalMappingList = new List<SignalCoordsMappingModel>();
        }
        public SignalMappingPlotConfigReader(string configFilePath) : this()
        {
            this.configFilePath = configFilePath;
            _readSignalMappingConfig();
        }
        public void ReadSignalMappingConfig(string configFilePath)
        {
            this.configFilePath = configFilePath;
            _readSignalMappingConfig();
        }
        private void _readSignalMappingConfig()
        {
            var configData = XDocument.Load(configFilePath);
            var smpc = configData.Element("Config").Element("SignalMappingPlotConfig");
            if (smpc != null)
            {
                var signals = smpc.Elements("Signal");
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
                    var sites = new List<ConfigSite>();
                    XmlSerializer serializer = null;
                    try
                    {
                        serializer = new XmlSerializer(typeof(ConfigSite), new XmlRootAttribute("Site"));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    if (serializer != null)
                    {
                        foreach (var item in signal.Element("Sites").Elements("Site"))
                        {
                            var b = item.CreateReader();
                            ConfigSite a = null;
                            try
                            {
                                a = (ConfigSite)serializer.Deserialize(b);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            if (a != null)
                            {
                                sites.Add(a);
                            }
                        }
                    }
                    newSignalMapping.Locations = sites;
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
