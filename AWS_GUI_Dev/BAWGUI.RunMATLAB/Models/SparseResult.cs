using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class SparseResult
    {
        public string SignalName;
        public string PMUname;
        public List<double> TimeStamps;
        public List<double> Maximum;
        public List<double> Minimum;
    }
}
