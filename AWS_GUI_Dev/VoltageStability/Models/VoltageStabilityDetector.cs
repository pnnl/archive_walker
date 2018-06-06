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
        public VoltageStabilityDetector()
        {
            Methods = new List<string>();
            Sites = new ObservableCollection<Site>();
        }

        public string DetectorGroupID { get; set; }
        public List<string> Methods { get; }
        public int MethodsCount
        {
            get { return Methods.Count; }
        }
        public void AddMethod(string mthd)
        {
            Methods.Add(mthd);
            switch (mthd)
            {
                case "DeMarco":
                    IsDeMarcoMethod = true;
                    break;
                case "Mitsubishi":
                    IsMitsubishiMethod = true;
                    break;
                case "Quanta":
                    IsQuantaMethod = true;
                    break;
                case "Chow":
                    IsChowMethod = true;
                    break;
                case "Tellegen":
                    IsTellegenMethod = true;
                    break;
                default:
                    throw new Exception("Unknown Voltage Stability method found!");
            }
        }
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
