using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Utilities;
using MathWorks.MATLAB.NET.Arrays;

namespace BAWGUI.MATLABRunResults.Models
{
    public class PDATExampleResults
    {
        private MWStructArray _results;

        public PDATExampleResults()
        {
        }

        public PDATExampleResults(MWStructArray rslt)
        {
            this._results = rslt;
            //var a = MathWorks.MATLAB.NET.createArray();
            int numberOfelements = 0;
            numberOfelements = _results.NumberOfElements;
            for (int index = 1; index <= numberOfelements; index++)
            {
                var newPMU = new PMUSignals();
                newPMU.PMUname = ((MWCharArray)_results["PMU_Name", index]).ToString();
                var arr = (MWCellArray)_results["Signal_Name", index];
                foreach (char[,] item in arr.ToArray())
                {
                    string row = new string(Utility.GetRow(item, 0).ToArray());
                    newPMU.SignalNames.Add(row);
                }
            }
        }
        public int SamplingRate { get; set; }
        public List<PMUSignals> SPMUignalsList { get; set; }
    }
}
