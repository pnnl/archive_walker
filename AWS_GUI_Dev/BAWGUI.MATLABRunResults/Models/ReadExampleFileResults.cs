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
        }

        public ReadExampleFileResults(MWStructArray rslt)
        {
            this._results = rslt;
        }
        public int SamplingRate { get; set; }
        public List<PMUSignals> PMUSignalsList { get; set; }

        public void GetSignals(MWStructArray rslt)
        {
            this._results = rslt;
            //var a = MathWorks.MATLAB.NET.createArray();
            int numberOfelements = 0;
            numberOfelements = _results.NumberOfElements;
            var newPMUSignalList = new List<PMUSignals>();
            for (int index = 1; index <= numberOfelements; index++)
            {
                if (index == 1)
                {
                    var fs = (MWNumericArray)_results["fs", index];
                    SamplingRate = (int)((double[])(fs.ToVector(MWArrayComponent.Real)))[0];
                }
                var newPMU = new PMUSignals();
                newPMU.PMUname = new string(Utility.GetRow(((MWCellArray)_results["PMU_Name", index]).ToArray().Cast<char[,]>().FirstOrDefault(), 0).ToArray());
                //foreach (char[,] item in pmuname)
                //{
                //    string row = new string(Utility.GetRow(item, 0).ToArray());
                //    newPMU.PMUname = row;
                //}
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
    }
}
