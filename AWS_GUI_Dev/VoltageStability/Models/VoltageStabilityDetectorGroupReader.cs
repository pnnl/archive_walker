using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VoltageStability.Models
{
    public class VoltageStabilityDetectorGroupReader
    {
        //private XElement _detector;

        public VoltageStabilityDetectorGroupReader(XElement dt)
        {
            //this._detector = detector;
            _detector.DetectorGroupID = dt.Elements().Where(e=>e.Name.LocalName == "DetectorGroupID").Single().Value;
            _detector.EventMergeWindow = dt.Elements().Where(e => e.Name.LocalName == "EventMergeWindow").Single().Value;
            var method = dt.Elements().Where(e => e.Name.LocalName == "Method").Single().Value;
            _detector.AddMethod(method);
            switch (method)
            {
                case "DeMarco":
                    _detector.DeMarcoAnalysisLength = dt.Elements().Where(e => e.Name.LocalName == "AnalysisLength").Single().Value;
                    break;
                case "Mitsubishi":
                    _detector.MitsubishiAnalysisLength = dt.Elements().Where(e => e.Name.LocalName == "AnalysisLength").Single().Value;
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
                site.StabilityThreshold= sub.Elements().Where(e => e.Name.LocalName == "LTIthresh").Single().Value;

            }
        }
        private VoltageStabilityDetector _detector = new VoltageStabilityDetector();
        public VoltageStabilityDetector GetDetector()
        {
            return _detector;
        }
    }
}
