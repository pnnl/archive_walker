using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoltageStability.ViewModels;

namespace VoltageStability.Models
{
    public class VoltageStabilityDetectorGroupWriter
    {
        public VoltageStabilityDetectorGroupWriter()
        {

        }

        private string _configFilePath;

        public void WriteXmlCofigFile(string configFilePath, List<VoltageStabilityDetectorViewModel> detectorList)
        {
            _configFilePath = configFilePath;
            var configFile = XDocument.Load(configFilePath);
            var vsDetectors = configFile.Element("Config").Element("DetectorConfig").Element("Configuration").Element("Alarming");
            foreach (var detecotr in detectorList)
            {
                var vsElement = new XElement("Thevenin");
                vsElement.Add(new XElement("DetectorGroupID", detecotr.DetectorGroupID));
                vsElement.Add(new XElement("EventMergeWindow", detecotr.EventMergeWindow));
                vsElement.Add(new XElement("Method"));
                vsElement.Add(new XElement("AnalysisLength"));
                foreach (var sub in detecotr.Sites)
                {
                    XElement subElement = _writeASubElement(sub);
                    vsElement.Add(subElement);
                }
                foreach (var mth in detecotr.Methods)
                {
                    var vsE = new XElement(vsElement);
                    vsE.Element("Method").Value = mth;
                    switch (mth)
                    {
                        case "DeMarco":
                            vsE.Element("AnalysisLength").Value = detecotr.DeMarcoAnalysisLength;
                            break;
                        case "Mitsubishi":
                            vsE.Element("AnalysisLength").Value = detecotr.MitsubishiAnalysisLength;
                            break;
                        default:
                            break;
                    }
                    vsDetectors.AddBeforeSelf(vsE);
                }
            }
            configFile.Save(configFilePath);
        }

        private XElement _writeASubElement(SiteViewModel sub)
        {
            var newSub = new XElement("Sub");
            newSub.Add(new XElement("Name", sub.Name));
            newSub.Add(new XElement("LTIthresh", sub.StabilityThreshold));
            newSub.Add(new XElement("Freq", 
                new XElement("PMU", sub.Frequency.PMUName),
                new XElement("F", sub.Frequency.SignalName)));
            foreach (var bus in sub.VoltageBuses)
            {
                XElement aBus = _writeABusElement(bus);
                newSub.Add(aBus);
            }
            foreach (var bs in sub.BranchesAndShunts)
            {
                XElement abs = _writeABranchOrShuntElement(bs);
                newSub.Add(abs);
            }
            return newSub;
        }

        private XElement _writeABranchOrShuntElement(object bs)
        {
            XElement newBS = null;
            if (bs is ShuntViewModel)
            {
                newBS = new XElement("Shunt");
                var shunt = bs as ShuntViewModel;
                newBS.Add(
                new XElement("PMU", shunt.ActivePower.PMUName),
                new XElement("P", shunt.ActivePower.SignalName),
                new XElement("Q", shunt.ReactivePower.SignalName),
                new XElement("Imag", shunt.CurrentMagnitude.SignalName),
                new XElement("Iang", shunt.CurrentAngle.SignalName));
                return newBS;
            }
            else
            {
                newBS = new XElement("Branch");
                var branch = bs as BranchViewModel;
                newBS.Add(
                new XElement("PMU", branch.ActivePower.PMUName),
                new XElement("P", branch.ActivePower.SignalName),
                new XElement("Q", branch.ReactivePower.SignalName),
                new XElement("Imag", branch.CurrentMagnitude.SignalName),
                new XElement("Iang", branch.CurrentAngle.SignalName));
                return newBS;
            }
        }

        private XElement _writeABusElement(VoltageBusViewModel bus)
        {
            var newBus = new XElement("Vbus",
            new XElement("PMU", bus.Angle.PMUName),
            new XElement("MAG", bus.Magnitude.SignalName),
            new XElement("ANG", bus.Angle.SignalName));
            return newBus;
        }
    }
}
