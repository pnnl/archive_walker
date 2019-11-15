using System.Collections.Generic;
using System.Linq;
using BAWGUI.Core;
using BAWGUI.Core.Models;
using JSISCSVWriter;

namespace BAWGUI.MATLABRunResults.Models
{
    public class OutOfRangeSignal : ISignal
    {
        public OutOfRangeSignal()
        {
            IsByDuration = true;
            IsByROC = true;
        }
        public string Label;
        public bool IsByDuration { get; set; }
        public bool IsByROC { get; set; }

        public string SignalName { get; set; }
        public string PMUname { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public int SamplingRate { get; set; }
        public List<double> TimeStampInSeconds { get; set; } // as .net number of seconds
        public List<double> Data { get; set; }

        public List<System.DateTime> TimeStamps; // as datetime
        public List<double> TimeStampNumber { get; set; } //as .net number of days, microsoft 0 day which is midnight, 31 December 1899
        public List<double> MATLABTimeStampNumber; // as matlab number of days, matlab 0 day which is January 0, 0000

        public List<double> DurationMaxMat { get; set; }
        public List<double> DurationMinMat { get; set; }
        public List<double> OutsideCount { get; set; }
        public List<double> RateOfChangeMaxMat { get; set; }
        public List<double> RateOfChangeMinMat { get; set; }
        public List<double> Rate { get; set; }
        public double Duration { get; set; }
        public double RateOfChange { get; set; }

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