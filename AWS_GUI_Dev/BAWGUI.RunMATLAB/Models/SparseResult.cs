using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class SparseResult
    {
        public string SignalName;
        public string PMUname;
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
