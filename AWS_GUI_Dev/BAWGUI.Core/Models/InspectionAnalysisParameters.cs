using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core.Models
{
    public class InspectionAnalysisParameters
    {
        public InspectionAnalysisParameters()
        {
            _analysisLength = 60;
            _windowType = DetectorWindowType.hann;
            _windowLength = 12;
            _windowOverlap = 6;
            ZeroPadding = -1;
            Fs = -1;
            LogScale = false;
            FreqMin = -1;
            FreqMax = -1;
        }
        private int _analysisLength;
        public int AnalysisLength
        {
            get
            {
                return _analysisLength;
            }
            set
            {
                _analysisLength = value;
                WindowLength = (int)Math.Floor(value / (double)5);

            }
        }
        private int _windowLength;
        public int WindowLength
        {
            get
            {
                return _windowLength;
            }
            set
            {
                _windowLength = value;
                WindowOverlap = (int)Math.Floor(value / (double)2);
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
        private int _windowOverlap;
        public int WindowOverlap
        {
            get
            {
                return _windowOverlap;
            }
            set
            {
                _windowOverlap = value;
            }
        }
        public int ZeroPadding { get; set; }
        public int Fs { get; set; }
        public bool LogScale { get; set; }
        public int FreqMin { get; set; }
        public int FreqMax { get; set; }
    }
}
