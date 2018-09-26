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
        }
        public string PMUname { get; set; }
        public List<string> SignalNames { get; set; }
        public List<string> SignalTypes { get; set; }
        public List<string> SignalUnits { get; set; }
    }
}