using BAWGUI.Utilities;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltageStability.MATLABRunResults.Models
{
    public class TheveninReRunResults
    {

        private MWStructArray _results;
        public MWStructArray Results
        {
            set
            {
                _results = value;
            }
        }

        public TheveninReRunResults()
        {
            _vsDetectorList = new List<TheveninDetector>();
        }

        public TheveninReRunResults(MWStructArray rslts)
        {
            _results = rslts;
            _vsDetectorList = new List<TheveninDetector>();
            int numberOfElements = 0;
            numberOfElements = _results.NumberOfElements;
            var allTheveninSignals = new List<TheveninSignal>();
            for (int index = 1; index <= numberOfElements; index++)
            {
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

                arr = (MWNumericArray)_results["LTI", index];
                int[] dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("LTI of element {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var lti = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                var signalCount = dimEach[1];
                var signalLength = dimEach[0];

                arr = (MWNumericArray)_results["LTIthresh", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("LTIthresh of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var LTIthresh = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["E", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("E of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var E = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["Z", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Z of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var Z = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["VbusMAG", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("VbusMAG of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var VbusMAG = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["VbusANG", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("VbusANG of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var VbusANG = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["SourceP", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("SourceP of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var SourceP = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["SourceQ", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("SourceQ of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var SourceQ = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["Vhat", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Vhat of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var VhatReal = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                var VhatImage = ((double[])arr.ToVector(MWArrayComponent.Imaginary)).ToList();

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

                var mthd = (MWCharArray)_results["Method", index];
                dimEach = mthd.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Method of elements {0} matrix dimension out of range in Voltage Stability rerun.", index));
                }
                var Method = mthd.ToString();
                //var signalCount = dataPMU.Count();
                //var signalLength = lti.Count() / signalCount;
                for (int signalNumber = 0; signalNumber < signalCount; signalNumber++)
                {
                    var newSignal = new TheveninSignal();
                    newSignal.ColumnNumber = signalNumber;
                    newSignal.TimeStampNumber = timeStampNumbers;
                    newSignal.TimeStamps = timeStamps;
                    newSignal.LTI = lti.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.LTIthresh = LTIthresh[signalNumber];
                    newSignal.E = E.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.Z = Z.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.VbusMAG = VbusMAG.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.VbusANG = VbusANG.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.SourceP = SourceP.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.SourceQ = SourceQ.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.VhatReal = VhatReal.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.VhatImage = VhatImage.Skip(signalNumber * signalLength).Take(signalLength).ToList();
                    newSignal.PMUname = dataPMU[signalNumber];
                    newSignal.Method = Method;
                    allTheveninSignals.Add(newSignal);
                }
            }
            if (allTheveninSignals.Count > 0)
            {
                var theveninSignalPMUPair = allTheveninSignals.GroupBy(x => x.PMUname);
                foreach (var pair in theveninSignalPMUPair)
                {
                    var newDetector = new TheveninDetector();
                    newDetector.SiteName = pair.Key;
                    newDetector.TheveninSignals = pair.ToList();
                    _vsDetectorList.Add(newDetector);
                }
            }
        }
        private List<TheveninDetector> _vsDetectorList;
        public List<TheveninDetector> VSDetectorList
        {
            get { return _vsDetectorList; }
            set { _vsDetectorList = value; }
        }
    }
}
