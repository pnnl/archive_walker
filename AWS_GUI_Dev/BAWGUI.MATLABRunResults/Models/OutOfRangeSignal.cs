using System.Collections.Generic;
using System.Linq;
using BAWGUI.Core.Models;
using JSISCSVWriter;

namespace BAWGUI.MATLABRunResults.Models
{
    public class OutOfRangeSignal : Signal
    {
        public OutOfRangeSignal()
        {
            IsByDuration = true;
            IsByROC = true;
        }
        public string Label;
        public bool IsByDuration { get; set; }
        public bool IsByROC { get; set; }

        public List<System.DateTime> TimeStamps;
        public List<double> TimeStampNumber;

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