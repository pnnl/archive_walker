using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class RingdownSignal
    {
        public string Label;
        public string SignalName;
        public string PMUname;
        public string Type;
        public string Unit;
        public List<System.DateTime> TimeStamps;
        public List<double> TimeStampNumber;
        public List<double> Data { get; set; }
        public List<double> TestStatistic { get; set; }
        public List<double> Threshold { get; set; }
        public double GetMaxOfMaximum()
        {
            return Data.Max();
        }
        public double GetMinOfMinimum()
        {
            return Data.Min();
        }
    }
}
