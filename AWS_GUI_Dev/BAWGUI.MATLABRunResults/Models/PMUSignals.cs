using System.Collections.Generic;

namespace BAWGUI.MATLABRunResults.Models
{
    public class PMUSignals
    {
        public PMUSignals()
        {
            SignalNames = new List<string>();
            SignalTypes = new List<string>();
            SignalUnits = new List<string>();
            Data = new List<double>();
            Stat = new List<int>();
            Flag = new List<bool>();
        }
        public string PMUname { get; set; }
        public List<string> SignalNames { get; set; }
        public List<string> SignalTypes { get; set; }
        public List<string> SignalUnits { get; set; }
        public int SamplingRate { get; set; }

        public List<double> Data { get; set; }
        public List<int> Stat { get; set; }
        public List<bool> Flag { get; set; }

        public int SignalLength { get; set; }
        public int SignalCount { get; set; }
        public List<double> TimeStampNumber { get; set; }
    }
}