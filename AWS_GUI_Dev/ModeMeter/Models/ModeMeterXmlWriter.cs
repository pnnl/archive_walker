using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ModeMeter.ViewModels;

namespace ModeMeter.Models
{
    public class ModeMeterXmlWriter
    {
        public void WriteXmlCofigFile(string configFilePath, List<SmallSignalStabilityToolViewModel> modeMeterList)
        {
            _configFilePath = configFilePath;
            var configFile = XDocument.Load(configFilePath);
            var mmDetectors = configFile.Element("Config").Element("DetectorConfig").Element("Configuration").Element("Alarming");
            foreach (var detector in modeMeterList)
            {
                var mmElement = new XElement("ModeMeter");
                mmElement.Add(new XElement("ResultPath", configFilePath + "\\MM\\" + detector.ModeMeterName));
                var baseliningSignals = new XElement("BaseliningSignals");
                foreach (var signal in detector.BaseliningSignals)
                {
                    //baseliningSignals.Add(new XElement("PMU", detector.EventMergeWindow));
                }
                foreach (var mode in detector.Modes)
                {
                    var modeElement = new XElement("Mode");
                    mmElement.Add(modeElement);
                }
                mmDetectors.AddBeforeSelf(mmElement);
            }
            configFile.Save(configFilePath);
        }
        private string _configFilePath;
    }
}
