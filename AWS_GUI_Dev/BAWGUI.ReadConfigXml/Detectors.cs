using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.ReadConfigXml
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using BAWGUI.Core;
    using BAWGUI.Core.Models;
    using Microsoft.VisualBasic;

    public class PeriodogramDetectorModel
    {
        public PeriodogramDetectorModel()
        {
            _pfa = "0.01";
            _analysisLength = 600;
            _windowType = DetectorWindowType.hann;
            _windowLength = 200;
            _windowOverlap = 100;
            _pfa = "0.001";
            _frequencyMin = "0.1";
            _frequencyMax = "15";
            _frequencyTolerance = "0.05";
        }

        private XElement _item;
        public PeriodogramDetectorModel(XElement item)
        {
            this._item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(_item);
            var par = _item.Element("Mode");
            if (par!=null)
            {
                Mode = (DetectorModeType)Enum.Parse(typeof(DetectorModeType), par.Value);
            }
            par = _item.Element("AnalysisLength");
            if (par != null)
            {
                try
                {
                    AnalysisLength = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("WindowType");
            if (par != null)
            {
                WindowType = (DetectorWindowType)Enum.Parse(typeof(DetectorWindowType), par.Value);
            }
            par = _item.Element("FrequencyInterval");
            if (par != null)
            {
                FrequencyInterval = par.Value;
            }
            par = _item.Element("WindowLength");
            if (par != null)
            {
                try
                {
                    WindowLength = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("WindowOverlap");
            if (par != null)
            {
                try
                {
                    WindowOverlap = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("MedianFilterFrequencyWidth");
            if (par != null)
            {
                MedianFilterFrequencyWidth = par.Value;
            }
            par = _item.Element("Pfa");
            if (par != null)
            {
                Pfa = par.Value;
            }
            par = _item.Element("FrequencyMin");
            if (par != null)
            {
                FrequencyMin = par.Value;
            }
            par = _item.Element("FrequencyMax");
            if (par != null)
            {
                FrequencyMax = par.Value;
            }
            par = _item.Element("FrequencyTolerance");
            if (par != null)
            {
                FrequencyTolerance = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Periodogram Forced Oscillation Detector";
            }
        }
        private DetectorModeType _mode;
        public DetectorModeType Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;                
            }
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
                WindowLength = (int)Math.Floor(value / (double)3);                
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
        private string _frequencyInterval;
        public string FrequencyInterval
        {
            get
            {
                return _frequencyInterval;
            }
            set
            {
                _frequencyInterval = value;                
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
        private string _medianFilterFrequencyWidth;
        public string MedianFilterFrequencyWidth
        {
            get
            {
                return _medianFilterFrequencyWidth;
            }
            set
            {
                _medianFilterFrequencyWidth = value;                
            }
        }
        private string _pfa;
        public string Pfa
        {
            get
            {
                return _pfa;
            }
            set
            {
                _pfa = value;                
            }
        }
        private string _frequencyMin;
        public string FrequencyMin
        {
            get
            {
                return _frequencyMin;
            }
            set
            {
                _frequencyMin = value;                
            }
        }
        private string _frequencyMax;
        public string FrequencyMax
        {
            get
            {
                return _frequencyMax;
            }
            set
            {
                _frequencyMax = value;                
            }
        }
        private string _frequencyTolerance;
        public string FrequencyTolerance
        {
            get
            {
                return _frequencyTolerance;
            }
            set
            {
                _frequencyTolerance = value;                
            }
        }
        public List<SignalSignatures> PMUElementList { get; set; }
    }

    public class SpectralCoherenceDetectorModel
    {
        public SpectralCoherenceDetectorModel()
        {
            _analysisLength = 60;
            _delay = 10;
            _numberDelays = 2;
            _thresholdScale = 3;
            _windowType = DetectorWindowType.hann;
            _windowLength = 12;
            _windowOverlap = 6;
            _frequencyMin = "0.1";
            _frequencyMax = "15";
            _frequencyTolerance = "0.05";
        }

        private XElement _item;
        public SpectralCoherenceDetectorModel(XElement item)
        {
            this._item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(_item);
            var par = _item.Element("Mode");
            if (par != null)
            {
                Mode = (DetectorModeType)Enum.Parse(typeof(DetectorModeType), par.Value);
            }
            par = _item.Element("AnalysisLength");
            if (par != null)
            {
                try
                {
                    AnalysisLength = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("Delay");
            if (par != null)
            {
                try
                {
                    Delay = Double.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Double expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("NumberDelays");
            if (par != null)
            {
                try
                {
                    NumberDelays = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("ThresholdScale");
            if (par != null)
            {
                try
                {
                    ThresholdScale = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("WindowType");
            if (par != null)
            {
                WindowType = (DetectorWindowType)Enum.Parse(typeof(DetectorWindowType), par.Value);
            }
            par = _item.Element("FrequencyInterval");
            if (par != null)
            {
                FrequencyInterval = par.Value;
            }
            par = _item.Element("WindowLength");
            if (par != null)
            {
                try
                {
                    WindowLength = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("WindowOverlap");
            if (par != null)
            {
                try
                {
                    WindowOverlap = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = _item.Element("FrequencyMin");
            if (par != null)
            {
                FrequencyMin = par.Value;
            }
            par = _item.Element("FrequencyMax");
            if (par != null)
            {
                FrequencyMax = par.Value;
            }
            par = _item.Element("FrequencyTolerance");
            if (par != null)
            {
                FrequencyTolerance = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Spectral Coherence Forced Oscillation Detector";
            }
        }
        private DetectorModeType _mode;
        public DetectorModeType Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                
            }
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
                // Delay = _analysisLength / 10
                WindowLength = (int)Math.Floor(value / (double)5);
                
            }
        }
        private double _delay;
        public double Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
                
            }
        }
        private int _numberDelays;
        public int NumberDelays
        {
            get
            {
                return _numberDelays;
            }
            set
            {
                _numberDelays = value;
                if (_numberDelays < 2)
                {
                    _numberDelays = 2;
                    throw new Exception("Error! Number of delays in Spectral Coherence detector must be an integer greater than or equal to 2.");
                }
                
            }
        }
        private int _thresholdScale;
        public int ThresholdScale
        {
            get
            {
                return _thresholdScale;
            }
            set
            {
                _thresholdScale = value;
                if (_thresholdScale <= 1)
                {
                    _thresholdScale = 3;
                    throw new Exception("Error! ThresholdScale in Spectral Coherence detector must be greater than1.");
                }
                
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
        private string _frequencyInterval;
        public string FrequencyInterval
        {
            get
            {
                return _frequencyInterval;
            }
            set
            {
                _frequencyInterval = value;                
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
        private string _frequencyMin;
        public string FrequencyMin
        {
            get
            {
                return _frequencyMin;
            }
            set
            {
                _frequencyMin = value;                
            }
        }
        private string _frequencyMax;
        public string FrequencyMax
        {
            get
            {
                return _frequencyMax;
            }
            set
            {
                _frequencyMax = value;                
            }
        }
        private string _frequencyTolerance;
        public string FrequencyTolerance
        {
            get
            {
                return _frequencyTolerance;
            }
            set
            {
                _frequencyTolerance = value;                
            }
        }
        public List<SignalSignatures> PMUElementList { get; set; }
    }

    public class RingdownDetectorModel
    {
        public RingdownDetectorModel()
        {
            RMSlength = "15";
            RMSmedianFilterTime = "120";
            RingThresholdScale = "3";
        }

        private XElement _item;
        public RingdownDetectorModel(XElement item)
        {
            this._item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(_item);
            var par = _item.Element("RMSmedianFilterTime");
            if (par != null)
            {
                RMSmedianFilterTime = par.Value;
            }
            par = _item.Element("RMSlength");
            if (par != null)
            {
                RMSlength = par.Value;
            }
            par = _item.Element("RingThresholdScale");
            if (par != null)
            {
                RingThresholdScale = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Ringdown Detector";
            }
        }
        private string _rmsLength;
        public string RMSlength
        {
            get
            {
                return _rmsLength;
            }
            set
            {
                _rmsLength = value;                
            }
        }
        private string _rmsmedianFilterTime;
        public string RMSmedianFilterTime
        {
            get
            {
                return _rmsmedianFilterTime;
            }
            set
            {
                _rmsmedianFilterTime = value;                
            }
        }
        private string _ringThresholdScale;
        public string RingThresholdScale
        {
            get
            {
                return _ringThresholdScale;
            }
            set
            {
                _ringThresholdScale = value;                
            }
        }
        private string _maxDuration;

        public string MaxDuration
        {
            get
            {
                return _maxDuration;
            }
            set
            {
                _maxDuration = value;                
            }
        }
        public List<SignalSignatures> PMUElementList { get; set; }
    }

    public class OutOfRangeGeneralDetectorModel
    {
        public OutOfRangeGeneralDetectorModel()
        {
        }
        public string Name
        {
            get
            {
                return "Out-Of-Range Detector";
            }
        }
        private double _max;
        public double Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;                
            }
        }
        private double _min;
        public double Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;                
            }
        }
        private string _duration;
        public string Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;                
            }
        }
        private string _analysisWindow;
        public string AnalysisWindow
        {
            get
            {
                return _analysisWindow;
            }
            set
            {
                _analysisWindow = value;                
            }
        }
    }

    public class OutOfRangeFrequencyDetectorModel
    {
        public OutOfRangeFrequencyDetectorModel()
        {
        }

        private XElement _item;
        public OutOfRangeFrequencyDetectorModel(XElement item)
        {
            this._item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(_item);
            var par = _item.Element("AverageWindow");
            if (par != null)
            {
                AverageWindow = par.Value;
                Type = OutOfRangeFrequencyDetectorType.AvergeWindow;
            }
            par = _item.Element("Nominal");
            if (par != null)
            {
                Nominal = par.Value;
                Type = OutOfRangeFrequencyDetectorType.Nominal;
            }
            par = _item.Element("DurationMax");
            if (par != null)
            {
                DurationMax = par.Value;
            }
            par = _item.Element("DurationMin");
            if (par != null)
            {
                DurationMin = par.Value;
            }
            par = _item.Element("Duration");
            if (par != null)
            {
                Duration = par.Value;
            }
            par = _item.Element("AnalysisWindow");
            if (par != null)
            {
                AnalysisWindow = par.Value;
            }
            par = _item.Element("RateOfChangeMax");
            if (par != null)
            {
                RateOfChangeMax = par.Value;
            }
            par = _item.Element("RateOfChangeMin");
            if (par != null)
            {
                RateOfChangeMin = par.Value;
            }
            par = _item.Element("RateOfChange");
            if (par != null)
            {
                RateOfChange = par.Value;
            }
            par = _item.Element("EventMergeWindow");
            if (par != null)
            {
                EventMergeWindow = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Out-Of-Range Detector";
            }
        }
        private OutOfRangeFrequencyDetectorType _type;
        public OutOfRangeFrequencyDetectorType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;                
            }
        }
        private string _averageWindow;
        public string AverageWindow
        {
            get
            {
                return _averageWindow;
            }
            set
            {
                _averageWindow = value;                
            }
        }
        private string _nominal;
        public string Nominal
        {
            get
            {
                return _nominal;
            }
            set
            {
                _nominal = value;                
            }
        }
        /// <summary>
        ///         ''' This value holds either the nominal or averageWindow value depends on the selected baseline creation type
        ///         ''' </summary>
        private string _durationMax;
        public string DurationMax
        {
            get
            {
                return _durationMax;
            }
            set
            {
                _durationMax = value;                
            }
        }
        private string _durationMin;
        public string DurationMin
        {
            get
            {
                return _durationMin;
            }
            set
            {
                _durationMin = value;                
            }
        }
        private string _duration;
        public string Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;                
            }
        }
        private string _analysisWindow;
        public string AnalysisWindow
        {
            get
            {
                return _analysisWindow;
            }
            set
            {
                _analysisWindow = value;                
            }
        }
        private string _rateOfChangeMax;
        public string RateOfChangeMax
        {
            get
            {
                return _rateOfChangeMax;
            }
            set
            {
                _rateOfChangeMax = value;                
            }
        }
        private string _rateOfChangeMin;
        public string RateOfChangeMin
        {
            get
            {
                return _rateOfChangeMin;
            }
            set
            {
                _rateOfChangeMin = value;                
            }
        }
        private string _rateOfChange;
        public string RateOfChange
        {
            get
            {
                return _rateOfChange;
            }
            set
            {
                _rateOfChange = value;                
            }
        }
        private string _eventMergeWindow;

        public string EventMergeWindow
        {
            get
            {
                return _eventMergeWindow;
            }
            set
            {
                _eventMergeWindow = value;                
            }
        }
        public List<SignalSignatures> PMUElementList { get; set; }
    }

    public class WindRampDetectorModel
    {
        public WindRampDetectorModel()
        {
            _isLongTrend = true;
            _apass = "1";
            _astop = "60";
            _fpass = "0.00005";
            _fstop = "0.0002";
            _valMin = "400";
            _valMax = "1000";
            _timeMin = "14400";
            _timeMax = "45000";
        }

        private XElement _item;
        public WindRampDetectorModel(XElement item)
        {
            this._item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(_item);
            var par = _item.Element("Fpass");
            if (par != null)
            {
                Fpass = par.Value;
                if (Fpass == "0.00005")
                {
                    IsLongTrend = true;
                }
                else
                {
                    IsLongTrend = false;
                }
            }
            par = _item.Element("Fstop");
            if (par != null)
            {
                Fstop = par.Value;
            }
            par = _item.Element("Apass");
            if (par != null)
            {
                Apass = par.Value;
            }
            par = _item.Element("Astop");
            if (par != null)
            {
                Astop = par.Value;
            }
            par = _item.Element("ValMin");
            if (par != null)
            {
                ValMin = par.Value;
            }
            par = _item.Element("TimeMin");
            if (par != null)
            {
                TimeMin = par.Value;
            }
            par = _item.Element("ValMax");
            if (par != null)
            {
                ValMax = par.Value;
            }
            par = _item.Element("TimeMax");
            if (par != null)
            {
                TimeMax = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Wind Ramp Detector";
            }
        }
        /// <summary>
        ///         ''' if false, is short trend
        ///         ''' </summary>
        private bool _isLongTrend;
        public bool IsLongTrend
        {
            get
            {
                return _isLongTrend;
            }
            set
            {
                _isLongTrend = value;
                if (value)
                {
                    _fpass = "0.00005";
                    _fstop = "0.0002";
                    ValMin = "400";
                    ValMax = "1000";
                    TimeMin = "14400";
                    TimeMax = "45000";
                }
                else
                {
                    _fpass = "0.03";
                    _fstop = "0.05";
                    ValMin = "50";
                    ValMax = "300";
                    TimeMin = "30";
                    TimeMax = "300";
                }                
            }
        }
        private string _fpass;
        public string Fpass
        {
            get
            {
                return _fpass;
            }
            set
            {
                _fpass = value;                
            }
        }
        private string _fstop;
        public string Fstop
        {
            get
            {
                return _fstop;
            }
            set
            {
                _fstop = value;                
            }
        }
        private string _apass;
        public string Apass
        {
            get
            {
                return _apass;
            }
            set
            {
                _apass = value;                
            }
        }
        private string _astop;
        public string Astop
        {
            get
            {
                return _astop;
            }
            set
            {
                _astop = value;                
            }
        }
        private string _valMin;
        public string ValMin
        {
            get
            {
                return _valMin;
            }
            set
            {
                _valMin = value;                
            }
        }
        private string _valMax;
        public string ValMax
        {
            get
            {
                return _valMax;
            }
            set
            {
                _valMax = value;                
            }
        }
        private string _timeMin;
        public string TimeMin
        {
            get
            {
                return _timeMin;
            }
            set
            {
                _timeMin = value;                
            }
        }
        private string _timeMax;
        public string TimeMax
        {
            get
            {
                return _timeMax;
            }
            set
            {
                _timeMax = value;                
            }
        }
        public List<SignalSignatures> PMUElementList { get; set; }
    }

    //public enum DetectorModeType
    //{
    //    [System.ComponentModel.Description("Single Channel")]
    //    SingleChannel,
    //    [System.ComponentModel.Description("Multichannel")]
    //    MultiChannel
    //}

    //public enum DetectorWindowType
    //{
    //    [System.ComponentModel.Description("Hann")]
    //    hann,
    //    [System.ComponentModel.Description("Rectangular")]
    //    rectwin,
    //    [System.ComponentModel.Description("Bartlett")]
    //    bartlett,
    //    [System.ComponentModel.Description("Hamming")]
    //    hamming,
    //    [System.ComponentModel.Description("Blackman")]
    //    blackman
    //}

    //public enum OutOfRangeFrequencyDetectorType
    //{
    //    [System.ComponentModel.Description("Nominal Value")]
    //    Nominal,
    //    [System.ComponentModel.Description("History for Baseline (seconds)")]
    //    AvergeWindow
    //}

    public class AlarmingSpectralCoherenceModel
    {
        public AlarmingSpectralCoherenceModel()
        {
            
        }

        private XElement _alarm;
        public AlarmingSpectralCoherenceModel(XElement alarm)
        {
            this._alarm = alarm;
            var par = _alarm.Element("CoherenceAlarm");
            if (par != null)
            {
                CoherenceAlarm = par.Value;
            }
            par = _alarm.Element("CoherenceMin");
            if (par != null)
            {
                CoherenceMin = par.Value;
            }
            par = _alarm.Element("TimeMin");
            if (par != null)
            {
                TimeMin = par.Value;
            }
            par = _alarm.Element("CoherenceCorner");
            if (par != null)
            {
                CoherenceCorner = par.Value;
            }
            par = _alarm.Element("TimeCorner");
            if (par != null)
            {
                TimeCorner = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Spectral Coherence Forced Oscillation Detector";
            }
        }
        private string _coherenceAlarm;
        public string CoherenceAlarm
        {
            get
            {
                return _coherenceAlarm;
            }
            set
            {
                _coherenceAlarm = value;                
            }
        }
        private string _coherenceMin;
        public string CoherenceMin
        {
            get
            {
                return _coherenceMin;
            }
            set
            {
                _coherenceMin = value;                
            }
        }
        private string _timeMin;
        public string TimeMin
        {
            get
            {
                return _timeMin;
            }
            set
            {
                _timeMin = value;                
            }
        }
        private string _coherenceCorner;
        public string CoherenceCorner
        {
            get
            {
                return _coherenceCorner;
            }
            set
            {
                _coherenceCorner = value;                
            }
        }
        private string _timeCorner;
        public string TimeCorner
        {
            get
            {
                return _timeCorner;
            }
            set
            {
                _timeCorner = value;                
            }
        }
    }

    public class AlarmingPeriodogramModel
    {
        public AlarmingPeriodogramModel()
        {
        }

        private XElement _alarm;
        public AlarmingPeriodogramModel(XElement alarm)
        {
            this._alarm = alarm;
            var par = _alarm.Element("SNRalarm");
            if (par != null)
            {
                SNRalarm = par.Value;
            }
            par = _alarm.Element("SNRmin");
            if (par != null)
            {
                SNRmin = par.Value;
            }
            par = _alarm.Element("TimeMin");
            if (par != null)
            {
                TimeMin = par.Value;
            }
            par = _alarm.Element("SNRcorner");
            if (par != null)
            {
                SNRcorner = par.Value;
            }
            par = _alarm.Element("TimeCorner");
            if (par != null)
            {
                TimeCorner = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Periodogram Forced Oscillation Detector";
            }
        }
        private string _snrAlarm;
        public string SNRalarm
        {
            get
            {
                return _snrAlarm;
            }
            set
            {
                _snrAlarm = value;                
            }
        }
        private string _snrMin;
        public string SNRmin
        {
            get
            {
                return _snrMin;
            }
            set
            {
                _snrMin = value;                
            }
        }
        private string _timeMin;
        public string TimeMin
        {
            get
            {
                return _timeMin;
            }
            set
            {
                _timeMin = value;                
            }
        }
        private string _snrCorner;
        public string SNRcorner
        {
            get
            {
                return _snrCorner;
            }
            set
            {
                _snrCorner = value;                
            }
        }
        private string _timeCorner;
        public string TimeCorner
        {
            get
            {
                return _timeCorner;
            }
            set
            {
                _timeCorner = value;                
            }
        }
    }

    public class AlarmingRingdownModel
    {
        public AlarmingRingdownModel()
        {
        }

        private XElement _alarm;
        public AlarmingRingdownModel(XElement alarm)
        {
            this._alarm = alarm;
            var par = _alarm.Element("MaxDuration");
            if (par != null)
            {
                MaxDuration = par.Value;
            }
        }

        public string Name
        {
            get
            {
                return "Ringdown Detector";
            }
        }
        private string _maxDuration;
        public string MaxDuration
        {
            get
            {
                return _maxDuration;
            }
            set
            {
                _maxDuration = value;                
            }
        }
    }

}
