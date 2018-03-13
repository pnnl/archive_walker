using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class RDRerunResults
    {
        public RDRerunResults(MWStructArray rslts)
        {
            _results = rslts;
            _ringdownDetectorList = new List<RingdownDetector>();
            //int detectorCount = 0;
            int numberOfelements = 0;
            numberOfelements = _results.NumberOfElements;
            for (int index = 1; index <= numberOfelements; index++)
            {
                //Console.WriteLine("\nelement: " + index.ToString());
                MWNumericArray arr = (MWNumericArray)_results["t", index];
                double[] t = (double[])(arr.ToVector(MWArrayComponent.Real));
                var timeStamps = new List<System.DateTime>();
                var timeStampNumbers = new List<double>();
                foreach (var item in t)
                {
                    var tt = Resources.Utility.Utility.MatlabDateNumToDotNetDateTime(item);
                    timeStamps.Add(tt);
                    timeStampNumbers.Add(tt.ToOADate());
                }
                
                arr = (MWNumericArray)_results["Data", index];
                int[] dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Data of element {0} matrix dimension out of range in ringdown rerun.", index));
                }
                var data = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                //size = dimEach[0];
                //Console.WriteLine("\tdata array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                //for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                //{
                //    Console.WriteLine("\nChannel " + (signalCount + 1).ToString() + ": first in Data: " + Data[signalCount * dimEach[0]].ToString() + ", last in Data: " + Data[(signalCount + 1) * dimEach[0] - 1].ToString());
                //}
                arr = (MWNumericArray)_results["RMS", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("RMS of elements {0} matrix dimension out of range in ringdown rerun.", index));
                }
                var rms = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                //size = dimEach[0];
                //Console.WriteLine("\tRMS array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                //for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                //{
                //    Console.WriteLine("\nChannel " + (signalCount + 1).ToString() + ": first in Data: " + RMS[signalCount * dimEach[0]].ToString() + ", last in Data: " + RMS[(signalCount + 1) * dimEach[0] - 1].ToString());
                //}
                arr = (MWNumericArray)_results["Threshold", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Threshold of elements {0} matrix dimension out of range in ringdown rerun.", index));
                }
                var threshold = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                //size = dimEach[0];
                //Console.WriteLine("\tThreshold array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                //for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                //{
                //    Console.WriteLine("\nChannel " + (signalCount + 1).ToString() + ": first in Data: " + Threshold[signalCount * dimEach[0]].ToString() + ", last in Data: " + Threshold[(signalCount + 1) * dimEach[0] - 1].ToString());
                //}
                List<string> dataPMU = new List<string>();
                foreach (char[,] item in ((MWCellArray)_results["DataPMU", index]).ToArray())
                {
                    string pmu = "";
                    foreach (var c in item)
                    {
                        pmu = pmu + c.ToString();
                    }
                    dataPMU.Add(pmu);
                }
                //Console.WriteLine("\tthere are " + DataPMU.Count + " PMUs");
                //Console.WriteLine("first PMU is: " + DataPMU.FirstOrDefault() + ", last PMU is: " + DataPMU.LastOrDefault());
                List<string> dataChannel = new List<string>();
                foreach (char[,] item in ((MWCellArray)_results["DataChannel", index]).ToArray())
                {
                    string channel = "";
                    foreach (var c in item)
                    {
                        channel = channel + c.ToString();
                    }
                    dataChannel.Add(channel);
                }
                //Console.WriteLine("\tthere are " + DataChannel.Count + " channels");
                //Console.WriteLine("first data channel is: " + DataChannel.FirstOrDefault() + ", last data channel is: " + DataChannel.LastOrDefault());
                var newDetector = new RingdownDetector();
                newDetector.Label = index.ToString();
                for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                {
                    var newRingdownSignal = new RingdownSignal();
                    newRingdownSignal.SignalName = dataChannel[signalCount];
                    newRingdownSignal.PMUname = dataPMU[signalCount];
                    //newRingdownSignal.Type = dataType[signalCount];
                    //newRingdownSignal.Unit = dataUnit[signalCount];
                    newRingdownSignal.TimeStamps = timeStamps;
                    newRingdownSignal.TimeStampNumber = timeStampNumbers;
                    newRingdownSignal.Threshold = threshold.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newRingdownSignal.Data = data.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newRingdownSignal.TestStatistic = rms.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newRingdownSignal.Label = index.ToString() + "s" + signalCount.ToString();
                    newDetector.RingdownSignals.Add(newRingdownSignal);
                }
                _ringdownDetectorList.Add(newDetector);
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
        private List<RingdownDetector> _ringdownDetectorList;
        public List<RingdownDetector> RingdownDetectorList 
        {
            get { return _ringdownDetectorList; }
            set
            {
                _ringdownDetectorList = value;
            }
        }
    }
}
