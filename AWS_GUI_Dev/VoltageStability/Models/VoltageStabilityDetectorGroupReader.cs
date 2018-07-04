using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VoltageStability.Models
{
    public class VoltageStabilityDetectorGroupReader
    {
        //private XElement _detector;

        //public VoltageStabilityDetectorGroupReader(XElement dt)
        //{
        //    ReaderVoltageStabilityDetector(dt);
        //}

        //public VoltageStabilityDetectorGroupReader()
        //{
        //}
        private string _configFilePath;
        public VoltageStabilityDetectorGroupReader(string configFilePath)
        {
            _configFilePath = configFilePath;
            var configFile = XDocument.Load(configFilePath);
            var vsDetectors = configFile.Element("Config").Element("DetectorConfig").Element("Configuration").Elements("Thevenin");
            var detectors = new List<VoltageStabilityDetector>();
            VoltageStabilityDetector previousVSdetector = null;
            foreach(var vs in vsDetectors)
            {
                VoltageStabilityDetector newVSdetector = null;
                var newID = vs.Element("DetectorGroupID").Value;
                if (previousVSdetector != null && previousVSdetector.DetectorGroupID == newID)
                {
                    var newMethod = vs.Element("Method").Value;
                    previousVSdetector.AddMethod(newMethod);
                    switch (newMethod)
                    {
                        case "DeMarco":
                            previousVSdetector.DeMarcoAnalysisLength = vs.Elements().Where(e => e.Name.LocalName == "AnalysisLength").Single().Value;
                            break;
                        case "Mitsubishi":
                            previousVSdetector.MitsubishiAnalysisLength = vs.Elements().Where(e => e.Name.LocalName == "AnalysisLength").Single().Value;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    newVSdetector = ReaderVoltageStabilityDetector(vs);
                    newVSdetector.DetectorGroupID = newID;
                    detectors.Add(newVSdetector);
                    previousVSdetector = newVSdetector;
                }
            }
            _detectors = detectors;
        }
        public VoltageStabilityDetector ReaderVoltageStabilityDetector(XElement dt)
        {
            var detector = new VoltageStabilityDetector();
            detector.EventMergeWindow = dt.Elements().Where(e => e.Name.LocalName == "EventMergeWindow").Single().Value;
            var method = dt.Elements().Where(e => e.Name.LocalName == "Method").Single().Value;
            detector.AddMethod(method);
            switch (method)
            {
                case "DeMarco":
                    detector.DeMarcoAnalysisLength = dt.Elements().Where(e => e.Name.LocalName == "AnalysisLength").Single().Value;
                    break;
                case "Mitsubishi":
                    detector.MitsubishiAnalysisLength = dt.Elements().Where(e => e.Name.LocalName == "AnalysisLength").Single().Value;
                    break;
                default:
                    break;
            }
            var subs = dt.Elements().Where(e => e.Name.LocalName == "Sub");
            var sites = new ObservableCollection<Site>();
            foreach (var sub in subs)
            {
                var site = new Site();
                site.Name = sub.Elements().Where(e => e.Name.LocalName == "Name").Single().Value;
                site.StabilityThreshold = sub.Elements().Where(e => e.Name.LocalName == "LTIthresh").Single().Value;
                site.Frequency = new Signal(sub.Element("Freq").Element("PMU").Value, sub.Element("Freq").Element("F").Value);
                var buses = sub.Elements("Vbus");
                foreach (var bus in buses)
                {
                    var mag = new Signal(bus.Element("PMU").Value, bus.Element("MAG").Value);
                    var ang = new Signal(bus.Element("PMU").Value, bus.Element("ANG").Value);
                    site.VoltageBuses.Add(new VoltageBus(mag, ang));
                }
                var branchesAndShunts = sub.Elements().Where((e => e.Name.LocalName == "Branch" || e.Name.LocalName == "Shunt"));
                foreach (var item in branchesAndShunts)
                {
                    if (item.Name == "Branch")
                    {
                        var bas = new Branch();
                        var p = item.Element("P");
                        if (p!=null)
                        {
                            bas.ActivePower = new Signal(item.Element("PMU").Value, item.Element("P").Value);
                        }
                        var q = item.Element("Q");
                        if (q!=null)
                        {
                            bas.ReactivePower = new Signal(item.Element("PMU").Value, item.Element("Q").Value);
                        }
                        bas.CurrentMagnitude = new Signal(item.Element("PMU").Value, item.Element("Imag").Value);
                        bas.CurrentAngle = new Signal(item.Element("PMU").Value, item.Element("Iang").Value);
                        site.BranchesAndShunts.Add(bas);
                    }
                    else
                    {
                        var bas = new Shunt();
                        var p = item.Element("P");
                        if (p != null)
                        {
                            bas.ActivePower = new Signal(item.Element("PMU").Value, item.Element("P").Value);
                        }
                        var q = item.Element("Q");
                        if (q != null)
                        {

                            bas.ReactivePower = new Signal(item.Element("PMU").Value, item.Element("Q").Value);
                        }
                        bas.CurrentMagnitude = new Signal(item.Element("PMU").Value, item.Element("Imag").Value);
                        bas.CurrentAngle = new Signal(item.Element("PMU").Value, item.Element("Iang").Value);
                        site.BranchesAndShunts.Add(bas);
                    }
                }
                sites.Add(site);
            }
            detector.Sites = sites;
            return detector;
        }
        private List<VoltageStabilityDetector> _detectors = new List<VoltageStabilityDetector>();
        public List<VoltageStabilityDetector> GetDetector()
        {
            return _detectors;
        }
    }

    public class Signal
    {
        public Signal()
        {
        }

        public Signal(string pmu, string name)
        {
            PMU = pmu;
            SignalName = name;
        }
        public string PMU { get; set; }
        public string SignalName { get; set; }
    }

}
