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
            Sites.Add(new Site());
            _vsDetectorCounter++;
            DetectorGroupID = _vsDetectorCounter.ToString();
        }

        private static int _vsDetectorCounter = 0;
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
                    _isDeMarcoMethod = true;
                    break;
                case "Mitsubishi":
                    _isMitsubishiMethod = true;
                    break;
                case "Quanta":
                    _isQuantaMethod = true;
                    break;
                case "RPI":
                    _isChowMethod = true;
                    break;
                case "Tellegen":
                    _isTellegenMethod = true;
                    break;
                default:
                    throw new Exception("Unknown Voltage Stability method found!");
            }
        }
        private bool _isDeMarcoMethod;
        public bool IsDeMarcoMethod
        {
            get { return _isDeMarcoMethod; }
            set
            {
                _isDeMarcoMethod = value;
                if (value)
                {
                    Methods.Add("DeMarco");
                }
                else
                {
                    Methods.Remove("DeMarco");
                }
            }
        }
        private bool _isMitsubishiMethod;
        public bool IsMitsubishiMethod
        {
            get { return _isMitsubishiMethod; }
            set
            {
                _isMitsubishiMethod = value;
                if (value)
                {
                    Methods.Add("Mitsubishi");
                }
                else
                {
                    Methods.Remove("Mitsubishi");
                }
            }
        }
        private bool _isQuantaMethod;
        public bool IsQuantaMethod
        {
            get { return _isQuantaMethod; }
            set
            {
                _isQuantaMethod = value;
                if (value)
                {
                    Methods.Add("Quanta");
                }
                else
                {
                    Methods.Remove("Quanta");
                }
            }
        }
        private bool _isChowMethod;
        public bool IsChowMethod
        {
            get { return _isChowMethod; }
            set
            {
                _isChowMethod = value;
                if (value)
                {
                    Methods.Add("RPI");
                }
                else
                {
                    Methods.Remove("RPI");
                }
            }
        }
        private bool _isTellegenMethod;
        public bool IsTellegenMethod
        {
            get { return _isTellegenMethod; }
            set
            {
                _isTellegenMethod = value;
                if (value)
                {
                    Methods.Add("Tellegen");
                }
                else
                {
                    Methods.Remove("Tellegen");
                }
            }
        }
        public string DeMarcoAnalysisLength { get; set; }
        public string MitsubishiAnalysisLength { get; set; }
        public string RPIAnalysisLength { get; set; }
        public string EventMergeWindow { get; set; }
        public ObservableCollection<Site> Sites { get; set; }

        public void AddSite()
        {

        }

        internal void AddVoltageBus()
        {
           
        }

        internal void AddBranch()
        {
           
        }

        internal void AddShunt()
        {
            
        }
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
