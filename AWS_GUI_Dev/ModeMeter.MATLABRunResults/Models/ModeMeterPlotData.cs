using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeMeter.MATLABRunResults.Models
{
    public class ModeMeterPlotData
    {
        public ModeMeterPlotData()
        {
            TimeStamps = new List<DateTime>();
            TimeStampNumber = new List<double>();
            Signals = new List<ModeMeterResultSignal>();
            YLabel = "";
            Title = "";
        }
        public List<System.DateTime> TimeStamps { get; set; }
        public List<double> TimeStampNumber { get; set; }
        public string YLabel { get; set; }
        public string Title { get; set; }
        public List<ModeMeterResultSignal> Signals { get; set; }
    }
}
