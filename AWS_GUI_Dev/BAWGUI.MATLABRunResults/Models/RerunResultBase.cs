using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;

namespace BAWGUI.MATLABRunResults.Models
{
    public abstract class RerunResultBase
    {
        public abstract MWStructArray Results { set; }
        public abstract int NumberOfElements { get; }
        public abstract double[] t { get; }
        public abstract List<string> DataPMU { get; }
        public abstract List<string> DataChannel { get; }
    }
}
