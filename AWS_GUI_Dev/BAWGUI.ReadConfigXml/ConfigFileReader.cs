using BAWGUI.CoordinateMapping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class ConfigFileReader
    {
        public ConfigFileReader(string configFile)
        {
            ConfigFilename = configFile;
            _configData = XDocument.Load(configFile);
            _dataConfigure = new DataConfigModel(_configData.Element("Config").Element("DataConfig"));
            _processConfigure = new ProcessConfigModel(_configData.Element("Config").Element("ProcessConfig"));
            _postProcessConfig = new PostProcessConfigModel(_configData.Element("Config").Element("PostProcessCustomizationConfig"));
            _detectorConfigure = new DetectorConfigModel(_configData.Element("Config").Element("DetectorConfig"));
            SignalSiteMappingConfig = new SignalMappingPlotConfigReader(_configData.Element("Config").Element("SignalMappingPlotConfig")).GetSignalCoordsMappingModel();
            DEFAreaMappingConfig = new DEFAreaMappingconfigReader(_configData.Element("Config").Element("DEFAreaMappingConfig")).GetDEFAreaMappoingModel();
            //_readConfigFile(_configData);
        }
        private XDocument _configData { get; set; }
        public string ConfigFilename { get; set; }
        private DataConfigModel _dataConfigure;
        public DataConfigModel DataConfigure
        {
            get
            {
                return _dataConfigure;
            }
            set
            {
                _dataConfigure = value;
            }
        }
        private ProcessConfigModel _processConfigure;
        public ProcessConfigModel ProcessConfigure
        {
            get
            {
                return _processConfigure;
            }
            set
            {
                _processConfigure = value;
            }
        }
        private PostProcessConfigModel _postProcessConfig;
        public PostProcessConfigModel PostProcessConfigure
        {
            get
            {
                return _postProcessConfig;
            }
            set
            {
                _postProcessConfig = value;
            }
        }
        private DetectorConfigModel _detectorConfigure;
        public DetectorConfigModel DetectorConfigure
        {
            get { return _detectorConfigure; }
            set { _detectorConfigure = value; }
        }
        private List<SignalCoordsMappingModel> _signalSiteMappingConfig;
        public List<SignalCoordsMappingModel> SignalSiteMappingConfig 
        { 
            get { return _signalSiteMappingConfig; }
            set { _signalSiteMappingConfig = value; }
        }
        private List<EnergyFlowAreaCoordsMappingModel> _defAreaMappingConfig;
        public List<EnergyFlowAreaCoordsMappingModel> DEFAreaMappingConfig 
        {
            get { return _defAreaMappingConfig; }
            set { _defAreaMappingConfig = value; }
        }
    }
}
