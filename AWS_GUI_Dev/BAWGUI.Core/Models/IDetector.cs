using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public interface IDetector
    {
        string Type { get; }
        string Unit { get; }
        string Label { get; set; }
        int SamplingRate { get; set; }
    }
}
