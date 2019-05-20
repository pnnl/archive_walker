using BAWGUI.Core;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSISCSVWriter
{
    public class Signal : ISignal
    {
        public string SignalName { get; set; }
        public string PMUname { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public int SamplingRate { get; set; }
        //public List<double> TimeStampInSeconds { get; set; }
        public List<double> TimeStampNumber { get; set; }
        private List<double> _data;
        public List<double> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                DataVector = Vector<double>.Build.Dense(value.ToArray());
            }
        }
        public Vector<double> DataVector { get; set; }
    }
}
