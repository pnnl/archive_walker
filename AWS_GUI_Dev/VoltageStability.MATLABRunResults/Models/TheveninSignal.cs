using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltageStability.MATLABRunResults.Models
{
    public class TheveninSignal
    {
        public int ColumnNumber { get; set; } //this is decided by the number of sites/subs, each site/sub is a column
        public List<System.DateTime> TimeStamps;
        public List<double> TimeStampNumber;
        public List<double> LTI { get; set; }
        public double LTIthresh { get; set; }
        public List<double> E { get; set; }
        public List<double> Z { get; set; }
        public List<double> VbusMAG { get; set; }
        public List<double> VbusANG { get; set; }
        public List<double> SourceP { get; set; }
        public List<double> SourceQ { get; set; }
        public List<double> Vhat { get; set; }
        public List<double> VhatImage { get; set; }
        public List<double> VhatReal { get; set; }

        public string PMUname;
        public string Method;
    }
}
