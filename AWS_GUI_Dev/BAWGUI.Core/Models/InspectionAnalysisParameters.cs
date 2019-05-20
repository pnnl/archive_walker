using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core.Models
{
    public class InspectionAnalysisParameters
    {
        public InspectionAnalysisParameters()
        {
            _analysisLengthStr = "";
            _windowType = DetectorWindowType.hann;
            _windowLengthStr = "";
            _windowOverlapStr = "";
            //ZeroPadding = -1;
            Fs = -1;
            LogScale = false;
            //FreqMin = -1;
            //FreqMax = -1;
        }
        private string _analysisLengthStr;
        public string AnalysisLengthStr
        {
            get
            {
                return _analysisLengthStr;
            }
            set
            {
                _analysisLengthStr = value;
                //DataTable dt = new DataTable();
                //AnalysisLength = (double)dt.Compute(value, "");

            }
        }
        private string _windowLengthStr;
        public string WindowLengthStr
        {
            get
            {
                return _windowLengthStr;
            }
            set
            {
                _windowLengthStr = value;
                //DataTable dt = new DataTable();
                //WindowLength = (double)dt.Compute(value, "");
            }
        }
        private DetectorWindowType _windowType;
        public DetectorWindowType WindowType
        {
            get
            {
                return _windowType;
            }
            set
            {
                _windowType = value;
            }
        }
        private string _windowOverlapStr;
        public string WindowOverlapStr
        {
            get
            {
                return _windowOverlapStr;
            }
            set
            {
                _windowOverlapStr = value;
                //DataTable dt = new DataTable();
                //WindowOverlap = (double)dt.Compute(value, "");
            }
        }
        public int? ZeroPadding { get; set; }
        public int _fs;
        public int Fs
        {
            get { return _fs; }
            set
            {
                _fs = value;
                //NumberOfSamplesInAnalysisLength = _analysisLength * Fs;
                //NumberOfSamplesInWindowLength = _windowLength * Fs;
                //NumberOfSamplesInWindowOverlap = _windowOverlap * Fs;
            }
        }
        public bool LogScale { get; set; }
        public double? FreqMin { get; set; }
        public double? FreqMax { get; set; }
        private double _analysisLength;
        public double AnalysisLength
        {
            get { return _analysisLength; }
            set
            {
                _analysisLength = value;
                //NumberOfSamplesInAnalysisLength = value * Fs;
            }
        }
        private double _windowLength;
        public double WindowLength
        {
            get { return _windowLength; }
            set
            {
                _windowLength = value;
                //NumberOfSamplesInWindowLength = value * Fs;
            }
        }
        private double _windowOverlap;
        public double WindowOverlap
        {
            get { return _windowOverlap; }
            set
            {
                _windowOverlap = value;
                //NumberOfSamplesInWindowOverlap = value * Fs;
            }
        }



        private double _numberOfSamplesInAnalysisLength;
        public double NumberOfSamplesInAnalysisLength
        {
            get { return _numberOfSamplesInAnalysisLength; }
            set { _numberOfSamplesInAnalysisLength = value; }
        }
        public double NumberOfSamplesInWindowLength { get; set; }
        public double NumberOfSamplesInWindowOverlap { get; set; }
    }
}
