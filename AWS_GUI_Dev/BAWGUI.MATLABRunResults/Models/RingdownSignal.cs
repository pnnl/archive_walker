using JSISCSVWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class RingdownSignal : Signal
    {
        public string Label;
        public List<System.DateTime> TimeStamps;
        public List<double> TimeStampNumber;
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
