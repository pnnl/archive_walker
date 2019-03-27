using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;

namespace BAWGUI.MATLABRunResults.Models
{
    public class InspectionAnalysisResults
    {
        private MWStructArray _results;
        public string Xlabel { get; set; }
        public string Ylabel { get; set; }
        public List<string> Signalnames { get; set; }
        public List<double> X { get; set; }
        public List<List<double>> Y { get; set; }
        public InspectionAnalysisResults(MWStructArray rslts)
        {
            this._results = rslts;
            if (rslts.NumberOfFields != 0)
            {
                Signalnames = new List<string>();
                X = new List<double>();
                Y = new List<List<double>>();
                int numberOfelements = 0;
                numberOfelements = _results.NumberOfElements;
                for (int index = 1; index <= numberOfelements; index++)
                {
                    MWNumericArray arr = (MWNumericArray)_results["x", index];
                    X = ((double[])(arr.ToVector(MWArrayComponent.Real))).ToList();
                    arr = (MWNumericArray)_results["y", index];
                    int[] dimEach = arr.Dimensions;
                    if (dimEach.Length != 2)
                    {
                        throw new Exception(String.Format("Data of element {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                    }
                    var data = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                    for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                    {
                        var aSignal = new List<double>();
                        aSignal = data.GetRange(signalCount * dimEach[0], dimEach[0]);
                        Y.Add(aSignal);
                    }
                    var s = _results["SigNames", index].ToArray();
                    Signalnames = ((MWCellArray)_results["SigNames", index]).ToString().Split('\n').Select(x => x.Trim(new char[] { '\'', ' ' })).ToList();
                    Xlabel = _results["xlab", index].ToString();
                    Ylabel = _results["ylab", index].ToString();
                }
            }
            else
            {
                throw new Exception("Empty structure returned from MATLAB");
            }
        }
    }
}
