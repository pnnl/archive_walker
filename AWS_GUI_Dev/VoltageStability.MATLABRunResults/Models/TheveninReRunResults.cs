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
                    throw new Exception(String.Format("Data of element {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var lti = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["LTIthresh", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("DurationMaxMat of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var LTIthresh = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["E", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("DurationMaxMat of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var E = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();

                arr = (MWNumericArray)_results["E", index];
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    throw new Exception(String.Format("DurationMaxMat of elements {0} matrix dimension out of range in Out-Of-Range rerun.", index));
                }
                var E = ((double[])arr.ToVector(MWArrayComponent.Real)).ToList();
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
