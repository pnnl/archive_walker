using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltageStability.MATLABRunResults.Models
{
    public class TheveninDetector
    {
        public List<TheveninSignal> TheveninSignals { get; set; }
        public string SiteName { get; set; } //PMU name, which is the <name> element under <sub> element in the xml.
    }
}
