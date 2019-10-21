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
        }

        public XElement WriteConfigToXMLFormat()
        {
            var ef = new XElement("EnergyFlow", string.Empty);
            var paths = new XElement("Paths");
            foreach (var pth in _model.Paths)
            {
                var aPath = new XElement("Path", new XElement("From", pth.FromArea),
                                                 new XElement("To", pth.ToArea));
                var vm = new XElement("VM", new XElement("PMU", new XElement("Name", pth.VoltageMag.PMUName),
                                                                new XElement("Channel", new XElement("Name", pth.VoltageMag.SignalName))));
                aPath.Add(vm);
                var va = new XElement("VA", new XElement("PMU", new XElement("Name", pth.VoltageAng.PMUName),
                                                                new XElement("Channel", new XElement("Name", pth.VoltageAng.SignalName))));
                aPath.Add(va);
                var p = new XElement("P", new XElement("PMU", new XElement("Name", pth.ActivePowerP.PMUName),
                                                                new XElement("Channel", new XElement("Name", pth.ActivePowerP.SignalName))));
                aPath.Add(p);
                var q = new XElement("Q", new XElement("PMU", new XElement("Name", pth.ReactivePowerQ.PMUName),
                                                                new XElement("Channel", new XElement("Name", pth.ReactivePowerQ.SignalName))));
                aPath.Add(q);
                paths.Add(aPath);
            }
            ef.Add(paths);
            var paramenters = new XElement("Parameters", new XElement("LocMinLength", _model.LocMinLength),
                                                         new XElement("LocLengthStep", _model.LocLengthStep),
                                                         new XElement("LocRes", _model.LocRes));
            ef.Add(paramenters);
            return ef;
        }
    }
}
