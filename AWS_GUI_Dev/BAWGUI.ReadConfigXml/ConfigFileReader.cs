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
            _dataConfigure = new DataConfig(_configData.Element("Config").Element("DataConfig"));
            _processConfigure = new ProcessConfig(_configData.Element("Config").Element("ProcessConfig"));
            _postProcessConfig = new PostProcessConfig(_configData.Element("Config").Element("PostProcessCustomizationConfig"));
            _detectorConfigure = new DetectorConfig(_configData.Element("Config").Element("DetectorConfig"));
            //_readConfigFile(_configData);
        }
        private XDocument _configData { get; set; }
        public string ConfigFilename { get; set; }
        private DataConfig _dataConfigure;
        public DataConfig DataConfigure
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
        private ProcessConfig _processConfigure;
        public ProcessConfig ProcessConfigure
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
        private PostProcessConfig _postProcessConfig;
        public PostProcessConfig PostProcessConfigure
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
        private DetectorConfig _detectorConfigure;
        public DetectorConfig DetectorConfigure
        {
            get { return _detectorConfigure; }
            set { _detectorConfigure = value; }
        }
    }
}
