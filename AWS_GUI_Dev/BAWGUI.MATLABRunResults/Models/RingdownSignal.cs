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
        public List<System.DateTime> TimeStamps; // as .net number of seconds
        public List<double> TimeStampNumber { get; set; } //as .net number of days, microsoft 0 day which is midnight, 31 December 1899
        public List<double> MATLABTimeStampNumber; // as matlab number of days, matlab 0 day which is January 0, 0000
        public List<double> TestStatistic { get; set; }
        public List<double> Threshold { get; set; }

        public string SignalName { get; set; }
        public string PMUname { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public int SamplingRate { get; set; }
        public List<double> TimeStampInSeconds { get; set; }// as .net number of seconds
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
