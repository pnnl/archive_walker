using BAWGUI.MATLABRunResults.Models;
using JSISCSVWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.Models
{
    public class RerunResultDetectGeneral : IDetector
    {
        public string Type { get; set; }

        public string Unit { get; set; }

        public string Label { get; set; }
        public int SamplingRate { get ; set; }
        public bool IsChecked { get; set; }
        public List<Signal> SignalsList { get; set; } = new List<Signal>();
    }
}
