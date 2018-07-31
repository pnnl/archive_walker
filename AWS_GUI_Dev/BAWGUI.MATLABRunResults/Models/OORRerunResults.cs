using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Utilities;
using MathWorks.MATLAB.NET.Arrays;

namespace BAWGUI.MATLABRunResults.Models
{
    public class OORRerunResults
    {
        public OORRerunResults(MWStructArray rslts)
        {
            _results = rslts;
            _oorDetectorList = new List<OutOfRangeDetector>();
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
                    var tt = Utility.MatlabDateNumToDotNetDateTime(item);
                    timeStamps.Add(tt);
                    timeStampNumbers.Add(tt.ToOADate());
                }

                arr = (MWNumericArray)_results["Data", index];
                int[] dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Data of element {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var data = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                //size = dimEach[0];
                //Console.WriteLine("\tdata array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                //for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                //{
                //    Console.WriteLine("\nChannel " + (signalCount + 1).ToString() + ": first in Data: " + Data[signalCount * dimEach[0]].ToString() + ", last in Data: " + Data[(signalCount + 1) * dimEach[0] - 1].ToString());
                //}
                arr = (MWNumericArray)_results["Duration", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Duration of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var durationArr = ((double[])arr.ToVector(MWArrayComponent.Real));
                if (durationArr.Count() != 1)
                {
                    throw new Exception("Out-Of-Range detector returned more than 1 \"Duration\" parameters.");
                }
                var duration = durationArr.ToList();
                arr = (MWNumericArray)_results["DurationMaxMat", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("DurationMaxMat of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var durationMaxMat = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                arr = (MWNumericArray)_results["DurationMinMat", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("DurationMinMat of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var durationMinMat = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                arr = (MWNumericArray)_results["OutsideCount", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("OutsideCount of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var outsideCount = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["RateOfChange", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("RateOfChange of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var rocArr = ((double[])arr.ToVector(MWArrayComponent.Real));
                if (rocArr.Count() != 1)
                {
                    throw new Exception("Out-Of-Range detector returned more than 1 \"RateOfChange\" parameters.");
                }
                var rateOfChange = rocArr.ToList();
                arr = (MWNumericArray)_results["Rate", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Rate of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var rate = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                arr = (MWNumericArray)_results["RateOfChangeMaxMat", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("RateOfChangeMaxMat of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var rateOfChangeMaxMat = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                arr = (MWNumericArray)_results["RateOfChangeMinMat", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("RateOfChangeMinMat of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var rateOfChangeMinMat = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();



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
                List<string> dataType = new List<string>();
                foreach (char[,] item in ((MWCellArray)_results["DataType", index]).ToArray())
                {
                    string type = "";
                    foreach (var c in item)
                    {
                        type = type + c.ToString();
                    }
                    dataType.Add(type);
                }
                List<string> dataUnit = new List<string>();
                foreach (char[,] item in ((MWCellArray)_results["DataUnit", index]).ToArray())
                {
                    string unit = "";
                    foreach (var c in item)
                    {
                        unit = unit + c.ToString();
                    }
                    dataUnit.Add(unit);
                }
                var newDetector = new OutOfRangeDetector();
                newDetector.Label = index.ToString();
                for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                {
                    var newOORSignal = new OutOfRangeSignal();
                    newOORSignal.SignalName = dataChannel[signalCount];
                    newOORSignal.PMUname = dataPMU[signalCount];
                    newOORSignal.Type = dataType[signalCount];
                    newOORSignal.Unit = dataUnit[signalCount];
                    newOORSignal.TimeStamps = timeStamps;
                    newOORSignal.TimeStampNumber = timeStampNumbers;
                    newOORSignal.Data = data.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newOORSignal.DurationMaxMat = durationMaxMat.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newOORSignal.DurationMinMat = durationMinMat.GetRange(signalCount * dimEach[0], dimEach[0]);
                    if (outsideCount.All(x => x is Double.NaN))
                    {
                        newOORSignal.IsByDuration = false;
                    }
                    else
                    {
                        newOORSignal.OutsideCount = outsideCount.GetRange(signalCount * dimEach[0], dimEach[0]);
                    }
                    newOORSignal.RateOfChangeMaxMat = rateOfChangeMaxMat.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newOORSignal.RateOfChangeMinMat = rateOfChangeMinMat.GetRange(signalCount * dimEach[0], dimEach[0]);
                    if (rate.All(x => x is Double.NaN))
                    {
                        newOORSignal.IsByROC = false;
                    }
                    else
                    {
                        newOORSignal.Rate = rate.GetRange(signalCount * dimEach[0], dimEach[0]);
                    }
                    newOORSignal.Duration = duration.FirstOrDefault();
                    newOORSignal.RateOfChange = rateOfChange.FirstOrDefault();
                    newOORSignal.Label = index.ToString() + "s" + signalCount.ToString();
                    newDetector.OORSignals.Add(newOORSignal);
                }
                _oorDetectorList.Add(newDetector);
            }
        }

        public OORRerunResults()
        {
            _oorDetectorList = new List<OutOfRangeDetector>();
        }

        private MWStructArray _results;
        public MWStructArray Results
        {
            set
            {
                _results = value;
            }
        }
        private List<OutOfRangeDetector> _oorDetectorList;
        public List<OutOfRangeDetector> OORDetectorList
        {
            get { return _oorDetectorList; }
            set
            {
                _oorDetectorList = value;
            }
        }

    }
}
