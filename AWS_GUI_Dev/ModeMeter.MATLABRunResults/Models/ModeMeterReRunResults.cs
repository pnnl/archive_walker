using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Utilities;
using MathWorks.MATLAB.NET.Arrays;

namespace ModeMeter.MATLABRunResults.Models
{
    public class ModeMeterReRunResults
    {
        private MWStructArray _results;
        public List<ModeMeterPlotData> PlotData { get; set; }
        public ModeMeterReRunResults()
        {
            PlotData = new List<ModeMeterPlotData>();
        }
        public ModeMeterReRunResults(MWStructArray rslts) : this()
        {
            this._results = rslts;
            int numberOfelements = 0;
            numberOfelements = _results.NumberOfElements;
            for (int index = 1; index <= numberOfelements; index++)
            {
                var newPlot = new ModeMeterPlotData();
                MWNumericArray arr = (MWNumericArray)_results["Time", index];
                double[] t = (double[])(arr.ToVector(MWArrayComponent.Real));
                var timeStampNumbersInSeconds = new List<double>();
                foreach (var item in t)
                {
                    var tt = Utility.MatlabDateNumToDotNetDateTime(item);
                    newPlot.TimeStamps.Add(tt);
                    newPlot.TimeStampNumber.Add(item - 693960.0); // convert from matlab 0 day which is January 0, 0000 to microsoft 0 day which is midnight, 31 December 1899, by substracting the number of days in between them: 365 * 1900 - 1900 / 4 - 19 + 4, leap years are every 4 years, but not every 100 years and again, every 400 years.
                    timeStampNumbersInSeconds.Add(Utility.MatlabDateNumToDotNetSeconds(item));
                }
                arr = (MWNumericArray)_results["Data", index];
                int[] dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("Data of element {0} matrix dimension out of range in ringdown rerun.", index));
                }
                var data = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
                newPlot.Title = ((MWCharArray)_results["Title", index]).ToString();
                List<string> signalNames = new List<string>();
                foreach (char[,] item in ((MWCellArray)_results["SignalNames", index]).ToArray())
                {
                    string channel = "";
                    foreach (var c in item)
                    {
                        channel = channel + c.ToString();
                    }
                    signalNames.Add(channel);
                }
                newPlot.YLabel = ((MWCharArray)_results["Ylabel", index]).ToString();
                for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                {
                    var newSignal = new ModeMeterResultSignal();
                    newSignal.SignalName = signalNames[signalCount];
                    newSignal.Data = data.GetRange(signalCount * dimEach[0], dimEach[0]);
                    newPlot.Signals.Add(newSignal);
                }
                PlotData.Add(newPlot);
            }
        }
    }
}
