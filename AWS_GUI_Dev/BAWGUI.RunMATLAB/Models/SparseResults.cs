using System;
using System.Collections.Generic;
using System.Linq;
using BAWGUI.Utilities;
using MathWorks.MATLAB.NET.Arrays;

namespace BAWGUI.RunMATLAB.Models
{
    public class SparseResults
    {
        public SparseResults(MWStructArray rslts)
        {
            _results = rslts;
            _sparseDetectorList = new List<SparseDetector>();
            //_dataPMU = new List<string>();
            //_dataChannel = new List<string>();
            for (int index = 1; index <= rslts.NumberOfElements; index++)
            {
                //Console.WriteLine("element: " + index.ToString());
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
                var timeStampNumbers = new List<double>();
                foreach (var item in t)
                {
                    var tt =Utility.MatlabDateNumToDotNetDateTime(item);
                    timeStamps.Add(tt);
                    timeStampNumbers.Add(tt.ToOADate());
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
                var dataType = new List<string>();
                foreach (char[,] item in ((MWCellArray)rslts["DataType", index]).ToArray())
                {
                    string type = "";
                    foreach (var c in item)
                    {
                        type = type + c.ToString();
                    }
                    dataType.Add(type);
                }
                var dataUnit = new List<string>();
                foreach (char[,] item in ((MWCellArray)rslts["DataUnit", index]).ToArray())
                {
                    string unit = "";
                    foreach (var c in item)
                    {
                        unit = unit + c.ToString();
                    }
                    dataUnit.Add(unit);
                }
                var detector = new SparseDetector();
                detector.Label = index.ToString();
                for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                {
                    var newSparseSignal = new SparseSignal();
                    newSparseSignal.SignalName = dataChannel[signalCount];
                    newSparseSignal.PMUname = dataPMU[signalCount];
                    newSparseSignal.Label = newSparseSignal.PMUname + newSparseSignal.SignalName;
                    newSparseSignal.Type = dataType[signalCount];
                    newSparseSignal.Unit = dataUnit[signalCount];
                    newSparseSignal.TimeStamps = timeStamps;
                    newSparseSignal.TimeStampNumber = timeStampNumbers;
                    newSparseSignal.Minimum = dataMin.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newSparseSignal.Maximum = dataMax.GetRange(signalCount * dimEach[0], dimEach[0]);
                    detector.SparseSignals.Add(newSparseSignal);
                }
                _sparseDetectorList.Add(detector);
            }
        }
        public int NumberOfDetectors
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


        //private List<string> _dataPMU;
        //public List<string> DataPMU
        //{
        //    get
        //    {
        //        return _dataPMU;
        //    }
        //}
        //private List<string> _dataChannel;
        //public List<string> DataChannel
        //{
        //    get
        //    {
        //        return _dataChannel;
        //    }
        //}
        private List<SparseDetector> _sparseDetectorList;
        public List<SparseDetector> SparseDetectorList
        {
            get { return _sparseDetectorList; }
            set
            {
                _sparseDetectorList = value;
            }
        }

    }
}
