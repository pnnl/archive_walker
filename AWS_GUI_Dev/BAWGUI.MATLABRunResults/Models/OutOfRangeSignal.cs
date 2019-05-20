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
        public List<double> TimeStampInSeconds { get; set; }
        public List<double> Data { get; set; }

        public List<System.DateTime> TimeStamps;
        public List<double> TimeStampNumber { get; set; }
        public List<double> MATLABTimeStampNumber;

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