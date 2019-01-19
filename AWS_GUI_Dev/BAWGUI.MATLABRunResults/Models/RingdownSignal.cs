using BAWGUI.Core;
using JSISCSVWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class RingdownSignal : ISignal
    {
        public string Label;
        public List<System.DateTime> TimeStamps;
        public List<double> TimeStampNumber;
        public List<double> MATLABTimeStampNumber;
        public List<double> TestStatistic { get; set; }
        public List<double> Threshold { get; set; }

        public string SignalName { get; set; }
        public string PMUname { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public int SamplingRate { get; set; }
        public List<double> TimeStampInSeconds { get; set; }
        public List<double> Data { get; set; }

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
