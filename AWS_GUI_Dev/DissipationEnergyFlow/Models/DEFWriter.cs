using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BAWGUI.Core;

namespace DissipationEnergyFlow.Models
{
    public class DEFWriter
    {
        private DissipationEnergyFlowDetectorModel _model;

        public DEFWriter(DissipationEnergyFlowDetectorModel detector)
        {
            _model = detector;
            _errorMessages = new List<string>();
        }

        public XElement WriteConfigToXMLFormat()
        {
            var ef = new XElement("EnergyFlow", string.Empty);
            var paths = new XElement("Paths");
            foreach (var pth in _model.Paths)
            {
                if (string.IsNullOrEmpty(pth.FromArea))
                {
                    _errorMessages.Add("A From Area for a path of a DEF detector is missing.");
                }
                var aPath = new XElement("Path", new XElement("From", pth.FromArea),
                                                 new XElement("To", pth.ToArea));
                if (!string.IsNullOrEmpty(pth.VoltageMag.SignalName) && !string.IsNullOrEmpty(pth.VoltageMag.PMUName))
                {
                    var vm = new XElement("VM", new XElement("PMU", new XElement("Name", pth.VoltageMag.PMUName),
                                                                    new XElement("Channel", new XElement("Name", pth.VoltageMag.SignalName))));
                    aPath.Add(vm);
                }
                if (!string.IsNullOrEmpty(pth.VoltageAng.SignalName) && !string.IsNullOrEmpty(pth.VoltageAng.PMUName))
                {
                    var va = new XElement("VA", new XElement("PMU", new XElement("Name", pth.VoltageAng.PMUName),
                                                                    new XElement("Channel", new XElement("Name", pth.VoltageAng.SignalName))));
                    aPath.Add(va);
                }
                if (!string.IsNullOrEmpty(pth.ActivePowerP.SignalName) && !string.IsNullOrEmpty(pth.ActivePowerP.PMUName))
                {
                    var p = new XElement("P", new XElement("PMU", new XElement("Name", pth.ActivePowerP.PMUName),
                                                                    new XElement("Channel", new XElement("Name", pth.ActivePowerP.SignalName))));
                    aPath.Add(p);
                }
                if (!string.IsNullOrEmpty(pth.ReactivePowerQ.SignalName) && !string.IsNullOrEmpty(pth.ReactivePowerQ.PMUName))
                {
                    var q = new XElement("Q", new XElement("PMU", new XElement("Name", pth.ReactivePowerQ.PMUName),
                                                                    new XElement("Channel", new XElement("Name", pth.ReactivePowerQ.SignalName))));
                    aPath.Add(q);
                }
                paths.Add(aPath);
            }
            ef.Add(paths);
            var paramenters = new XElement("Parameters", new XElement("PerformTimeLoc", _model.PerformTimeLoc.ToString().ToUpper()),
                                                         new XElement("LocMinLength", _model.LocMinLength),
                                                         new XElement("LocLengthStep", _model.LocLengthStep),
                                                         new XElement("LocRes", _model.LocRes));
            ef.Add(paramenters);
            return ef;
        }
        private List<string> _errorMessages;
        public List<string> GetErrorMessages()
        {
            return _errorMessages;
        }
    }
}
