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
                //vsElement.Add(new XElement("AnalysisLength"));
                foreach (var sub in detecotr.Sites)
                {
                    XElement subElement = _writeASubElement(sub);
                    vsElement.Add(subElement);
                }
                foreach (var mth in detecotr.Methods)
                {
                    var vsE = new XElement(vsElement);
                    vsE.Element("Method").Value = mth;
                    var methodElement = vsE.Element("Method");
                    switch (mth)
                    {
                        case "DeMarco":
                            methodElement.AddAfterSelf(new XElement("AnalysisLength", detecotr.DeMarcoAnalysisLength));
                            //vsE.Element("AnalysisLength").Value = detecotr.DeMarcoAnalysisLength;
                            break;
                        case "Mitsubishi":
                            methodElement.AddAfterSelf(new XElement("AnalysisLength", detecotr.MitsubishiAnalysisLength));
                            //vsE.Element("AnalysisLength").Value = detecotr.MitsubishiAnalysisLength;
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
            if (sub.Frequency != null)
            {
                newSub.Add(new XElement("Freq",
                    new XElement("PMU", sub.Frequency.PMUName),
                    new XElement("F", sub.Frequency.SignalName)));
            }
            foreach (var bus in sub.VoltageBuses)
            {
                XElement aBus = null;
                try
                {
                    aBus = _writeABusElement(bus);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error writing Voltage Bus element. Origianl error: " + ex.Message);
                }
                if (aBus !=  null)
                {
                    newSub.Add(aBus);
                }
            }
            foreach (var bs in sub.BranchesAndShunts)
            {
                XElement abs = null;
                try
                {
                    abs = _writeABranchOrShuntElement(bs);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error writing branch or shunt element! Original error: " + ex.Message);
                }
                if (abs != null)
                {
                    newSub.Add(abs);
                }
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
                if (shunt.CurrentMagnitude != null)
                {
                    newBS.Add(new XElement("PMU", shunt.CurrentMagnitude.PMUName));
                }
                if (shunt.ActivePower != null && !string.IsNullOrEmpty(shunt.ActivePower.SignalName))
                {
                    newBS.Add(new XElement("P", shunt.ActivePower.SignalName));
                }
                if (shunt.ReactivePower != null && !string.IsNullOrEmpty(shunt.ReactivePower.PMUName))
                {
                    newBS.Add(new XElement("Q", shunt.ReactivePower.SignalName));
                }
                if (shunt.CurrentMagnitude != null)
                {
                    newBS.Add(new XElement("Imag", shunt.CurrentMagnitude.SignalName));
                }
                else
                {
                    throw new Exception("Current Magnitude is required!");
                }
                if (shunt.CurrentAngle != null)
                {
                    newBS.Add(new XElement("Iang", shunt.CurrentAngle.SignalName));
                }
                else
                {
                    throw new Exception("Current Angle is required!");
                }
                return newBS;
            }
            else
            {
                newBS = new XElement("Branch");
                var branch = bs as BranchViewModel;
                newBS.Add(new XElement("PMU", branch.CurrentMagnitude.PMUName));
                if (branch.ActivePower != null && !string.IsNullOrEmpty(branch.ActivePower.PMUName))
                {
                    newBS.Add(new XElement("P", branch.ActivePower.SignalName));
                }
                if (branch.ReactivePower != null && !string.IsNullOrEmpty(branch.ReactivePower.SignalName))
                {
                    newBS.Add(new XElement("Q", branch.ReactivePower.SignalName));
                }
                if (branch.CurrentMagnitude != null)
                {
                    newBS.Add(new XElement("Imag", branch.CurrentMagnitude.SignalName));
                }
                else
                {
                    throw new Exception("Current Magnitude is required!");
                }
                if (branch.CurrentAngle != null)
                {
                    newBS.Add(new XElement("Iang", branch.CurrentAngle.SignalName));
                }
                else
                {
                    throw new Exception("Current Angle is required!");
                }
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
