using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltageStability.Models
{
    public class VoltageStabilityDetector
    {
        public string DetectorGroupID { get; set; }
        public bool IsDeMarcoMethod { get; set; }
        public bool IsMitsubishiMethod { get; set; }
        public bool IsQuantaMethod { get; set; }
        public bool IsChowMethod { get; set; }
        public bool IsTellegenMethod { get; set; }
        public string DeMarcoAnalysisLength { get; set; }
        public string MitsubishiAnalysisLength { get; set; }
        public string EventMergeWindow { get; set; }
        public ObservableCollection<Site> Sites { get; set; }
    }
    public enum DetectorGroupMethods
    {
        DeMarco,
        Mitsubishi,
        Quanta,
        Chow,
        Tellegen
    }
}
