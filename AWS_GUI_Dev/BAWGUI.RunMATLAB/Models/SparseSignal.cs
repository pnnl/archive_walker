using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class SparseSignal
    {
        public string Label;
        public string SignalName;
        public string PMUname;
        public string Type;
        public string Unit;
        public List<System.DateTime> TimeStamps;
        public List<double> TimeStampNumber;
        public List<double> Maximum;
        public List<double> Minimum;
        public double GetMaxOfMaximum()
        {
            return Maximum.Max();
        }
        public double GetMinOfMinimum()
        {
            return Minimum.Min();
        }
    }
}
