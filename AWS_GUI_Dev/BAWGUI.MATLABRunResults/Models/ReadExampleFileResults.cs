using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Utilities;
using MathWorks.MATLAB.NET.Arrays;

namespace BAWGUI.MATLABRunResults.Models
{
    public class ReadExampleFileResults
    {
        private MWStructArray _results;

        public ReadExampleFileResults()
        {
            PMUSignalsList = new List<PMUSignals>();
            TimeStampNumber = new List<double>();
        }

        public ReadExampleFileResults(MWStructArray rslt) : this()
        {
            this._results = rslt;
        }
        //public int SamplingRate { get; set; }
        public List<PMUSignals> PMUSignalsList { get; set; }
        public List<double> TimeStampNumber { get; set; }
        public void GetSignals(MWStructArray rslt)
        {
            this._results = rslt;
            //var a = MathWorks.MATLAB.NET.createArray();
            int numberOfelements = 0;
            numberOfelements = _results.NumberOfElements;
            var newPMUSignalList = new List<PMUSignals>();
            for (int index = 1; index <= numberOfelements; index++)
            {
                //if (index == 1)
                //{
                    //var fs = (MWNumericArray)_results["fs", index];
                    //SamplingRate = (int)((double[])(fs.ToVector(MWArrayComponent.Real)))[0];
                //}
                var newPMU = new PMUSignals();
                newPMU.PMUname = new string(Utility.GetRow(((MWCellArray)_results["PMU_Name", index]).ToArray().Cast<char[,]>().FirstOrDefault(), 0).ToArray());
                var fs = (MWNumericArray)_results["fs", index];
                newPMU.SamplingRate = fs.ToScalarInteger();
                var arr = (MWCellArray)_results["Signal_Name", index];
                foreach (char[,] item in arr.ToArray())
                {
                    string row = new string(Utility.GetRow(item, 0).ToArray());
                    newPMU.SignalNames.Add(row);
                }
                arr = (MWCellArray)_results["Signal_Type", index];
                foreach (char[,] item in arr.ToArray())
                {
                    string row = new string(Utility.GetRow(item, 0).ToArray());
                    newPMU.SignalTypes.Add(row);
                }
                arr = (MWCellArray)_results["Signal_Unit", index];
                foreach (char[,] item in arr.ToArray())
                {
                    string row = new string(Utility.GetRow(item, 0).ToArray());
                    newPMU.SignalUnits.Add(row);
                }
                newPMUSignalList.Add(newPMU);
            }
            PMUSignalsList = newPMUSignalList;
        }
        public void GetSignalsWithData(MWStructArray rslt)
        {
            this._results = rslt;
            //var a = MathWorks.MATLAB.NET.createArray();
            int numberOfelements = 0;
            numberOfelements = _results.NumberOfElements;
            var newPMUSignalList = new List<PMUSignals>();
            for (int index = 1; index <= numberOfelements; index++)
            {
                var newPMU = new PMUSignals();
                newPMU.PMUname = new string(Utility.GetRow(((MWCellArray)_results["PMU_Name", index]).ToArray().Cast<char[,]>().FirstOrDefault(), 0).ToArray());
                var fs = (MWNumericArray)_results["fs", index];
                newPMU.SamplingRate = (int)((double[])(fs.ToVector(MWArrayComponent.Real)))[0];
                var arr = (MWCellArray)_results["Signal_Name", index];
                foreach (char[,] item in arr.ToArray())
                {
                    string row = new string(Utility.GetRow(item, 0).ToArray());
                    newPMU.SignalNames.Add(row);
                }
                arr = (MWCellArray)_results["Signal_Type", index];
                foreach (char[,] item in arr.ToArray())
                {
                    string row = new string(Utility.GetRow(item, 0).ToArray());
                    newPMU.SignalTypes.Add(row);
                }
                arr = (MWCellArray)_results["Signal_Unit", index];
                foreach (char[,] item in arr.ToArray())
                {
                    string row = new string(Utility.GetRow(item, 0).ToArray());
                    newPMU.SignalUnits.Add(row);
                }
                if (index == 1)
                {
                    TimeStampNumber.Clear();
                    MWNumericArray narr = (MWNumericArray)_results["Signal_Time", index];
                    double[] t = (double[])(narr.ToVector(MWArrayComponent.Real));

                    //var timeStamps = new List<System.DateTime>();
                    //var timeStampNumbers = new List<double>();
                    //var timeStampNumbersInSeconds = new List<double>();

                    foreach (var item in t)
                    {
                        //var tt = Utility.MatlabDateNumToDotNetDateTime(item);
                        //timeStamps.Add(tt);
                        TimeStampNumber.Add(item - 693960.0); // convert from matlab 0 day which is January 0, 0000 to microsoft 0 day which is midnight, 31 December 1899, by substracting the number of days in between them: 365 * 1900 - 1900 / 4 - 19 + 4, leap years are every 4 years, but not every 100 years and again, every 400 years.
                        //timeStampNumbers.Add(tt.ToOADate());
                        //timeStampNumbersInSeconds.Add(Utility.MatlabDateNumToDotNetSeconds(item));
                    }
                }
                var dataarr = (MWNumericArray)_results["Data", index];
                int[] dimEach = dataarr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Data of element {0} matrix dimension out of range in retrieving data.", index));
                }
                newPMU.Data = ((double[])dataarr.ToVector(MWArrayComponent.Real)).ToList();
                newPMU.SignalLength = dimEach[0];
                newPMU.SignalCount = dimEach[1];
                //var larr = (MWLogicalArray)_results["Flag", index];
                //dimEach = larr.Dimensions;
                //if (dimEach.Length != 3)
                //{
                //    throw new Exception(String.Format("logic matrix of element {0} matrix dimension out of range in retrieving data.", index));
                //}
                //newPMU.Flag = larr.ToVector().ToList();

                //var allSignal_Time = _results.GetField("Signal_Time");
                //var alldata = _results.GetField("Data");
                //var allflag = _results.GetField("Flag");
                //var allStat = _results.GetField("Stat");
                //var Stat1 = _results.GetField("Stat", index);
                //var stat = (MWLogicalArray)_results["Stat", index];
                //var statusFlag = stat.ToArray();

                newPMUSignalList.Add(newPMU);
            }
            PMUSignalsList = newPMUSignalList;
        }
    }
}
