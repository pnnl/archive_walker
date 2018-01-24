using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;

namespace BAWGUI.RunMATLAB.Models
{
    public class SparseResults
    {
        public SparseResults(MWStructArray rslts)
        {
            _results = rslts;
            _uniqueSparseResults = new List<SparseResult>();
            _dataPMU = new List<string>();
            _dataChannel = new List<string>();
            for (int index = 1; index <= rslts.NumberOfElements; index++)
            {
                Console.WriteLine("element: " + index.ToString());
                MWNumericArray arr = (MWNumericArray)rslts["DataMin", index];
                int[] dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("DataMin of element {0} matrix dimension out of range in sparse rerun.", index));
                }
                var dataMin = ((double[])(arr.ToVector(MWArrayComponent.Real))).ToList();
                arr = (MWNumericArray)rslts["DataMax", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("DataMax of element {0} matrix dimension out of range in sparse rerun.", index));
                }
                var dataMax = ((double[])(arr.ToVector(MWArrayComponent.Real))).ToList();
                arr = (MWNumericArray)rslts["t", index];
                var t =(double[])(arr.ToVector(MWArrayComponent.Real));
                var timeStamps = new List<System.DateTime>();
                foreach (var item in t)
                {
                    timeStamps.Add(_numbTimeConvert(item));
                }
                var dataPMU = new List<string>();
                foreach (char[,] item in ((MWCellArray)rslts["DataPMU", index]).ToArray())
                {
                    string pmu = "";
                    foreach (var c in item)
                    {
                        pmu = pmu + c.ToString();
                    }
                    dataPMU.Add(pmu);
                }
                var dataChannel = new List<string>();
                foreach (char[,] item in ((MWCellArray)rslts["DataChannel", index]).ToArray())
                {
                    string channel = "";
                    foreach (var c in item)
                    {
                        channel = channel + c.ToString();
                    }
                    dataChannel.Add(channel);
                }
                for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                {
                    if (!_dataChannel.Contains(dataChannel[signalCount])){
                        var newResult = new SparseResult();
                        newResult.SignalName = dataChannel[signalCount];
                        newResult.PMUname = dataPMU[signalCount];
                        newResult.TimeStamps = timeStamps;
                        newResult.TimeStampNumber = t.ToList();
                        newResult.Minimum = dataMin.GetRange(signalCount * dimEach[0], dimEach[0]);
                        newResult.Maximum = dataMax.GetRange(signalCount * dimEach[0], dimEach[0]);
                        _uniqueSparseResults.Add(newResult);
                        _dataChannel.Add(dataChannel[signalCount]);
                        _dataPMU.Add(dataPMU[signalCount]);
                    }
                }
            }
        }
        public int NumberOfElements
        {
            get
            {
                return _results.NumberOfElements;
            }
        }
        private MWStructArray _results;
        public MWStructArray Results
        {
            set
            {
                _results = value;
            }
        }
        //private double[] _dataMin;
        //public double[] DataMin { get { return _dataMin; } }
        //private double[] _dataMax;
        //public double[] DataMax { get { return _dataMax; } }
        //private double[] _t;
        //public override double[] t
        //{
        //    get
        //    {
        //        return _t;
        //    }
        //}


        private List<string> _dataPMU;
        public List<string> DataPMU
        {
            get
            {
                return _dataPMU;
            }
        }
        private List<string> _dataChannel;
        public List<string> DataChannel
        {
            get
            {
                return _dataChannel;
            }
        }
        private List<SparseResult> _uniqueSparseResults;
        public List<SparseResult> UniqueSparseResults
        {
            get { return _uniqueSparseResults; }
            set
            {
                _uniqueSparseResults = value;
            }
        }
        private System.DateTime _numbTimeConvert(double item)
        {
            System.DateTime dtDateTime = new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            System.DateTime bbb = dtDateTime.AddSeconds((item - 367) * 86400);
            return bbb;
        }
    }
}
