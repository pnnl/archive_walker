using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSISCSVWriter
{
    public interface ISignal
    {
        string SignalName { get; set; }
        string PMUname { get; set; }
        string Type { get; set; }
        string Unit { get; set; }
        int SamplingRate { get; set; }
        List<double> Data { get; set; }
        List<double> TimeStampInSeconds { get; set; }
    }
}
