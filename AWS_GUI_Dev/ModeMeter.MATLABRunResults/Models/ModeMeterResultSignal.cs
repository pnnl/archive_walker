using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeMeter.MATLABRunResults.Models
{
    public class ModeMeterResultSignal
    {
        public ModeMeterResultSignal()
        {
            Data = new List<double>();
            SignalName = "";
        }
        public List<double> Data;
        //public List<System.DateTime> TimeStamps;
        //public List<double> TimeStampNumber;
        public string SignalName { get; set; }
        //public string YLabel { get; set; }
        //public string Title { get; set; }
    }
}
