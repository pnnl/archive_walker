using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.ReadConfigXml;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using ModeMeter.Models;
using System;
using System.Collections.ObjectModel;

namespace ModeMeter.ViewModels
{
    public class ModeViewModel : ViewModelBase
    {
        private Mode _model;
        public ModeViewModel(Mode mode, SignalManager _signalMgr)
        {
            this._model = mode;
            PMUs = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var signal in _model.PMUs)
            {
                if (!string.IsNullOrEmpty(signal.SignalName))
                {
                    var thisSignal = _signalMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName);
                    PMUs.Add(thisSignal);
                }
                else
                {
                    _signalMgr.FindSignalsOfAPMU(PMUs, signal.PMUName);
                }
            }
            DesiredModes = new DesiredModeAttributesViewModel(_model.DesiredModes);
            Methods = new ObservableCollection<ModeMethodViewModel>();
            foreach (var alg in _model.AlgNames)
            {
                Methods.Add(new ModeMethodViewModel(alg));
            }
            FODetectorParameters = new PeriodogramDetectorParametersViewModel(_model.FODetectorParameters);
        }
        public string ModeName
        {
            get { return _model.ModeName; }
            set
            {
                _model.ModeName = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SignalSignatureViewModel> PMUs { get; set; }
        public DesiredModeAttributesViewModel DesiredModes { get; set; }
        public int AnalysisLength
        {
            get { return _model.AnalysisLength; }
            set
            {
                _model.AnalysisLength = value;
                FODetectorParameters.WindowLength = (int)Math.Floor(value / 3d);
                OnPropertyChanged();
            }
        }
        public string DampRatioThreshold
        {
            get { return _model.DampRatioThreshold; }
            set
            {
                _model.DampRatioThreshold = value;
                OnPropertyChanged();
            }
        }
        public RetroactiveContinuityStatusType Status
        {
            get { return _model.RetConTracking.Status; }
            set
            {
                _model.RetConTracking.Status = value;
                OnPropertyChanged();
            }
        }
        public string MaxLength
        {
            get { return _model.RetConTracking.MaxLength; }
            set
            {
                _model.RetConTracking.MaxLength = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ModeMethodViewModel> Methods { get; set; }
        public PeriodogramDetectorParametersViewModel FODetectorParameters { get; set; }
    }

    public class DesiredModeAttributesViewModel : ViewModelBase
    {
        private DesiredModeAttributes _model;

        public DesiredModeAttributesViewModel(DesiredModeAttributes desiredModes)
        {
            this._model = desiredModes;
        }
        public string LowF
        {
            get { return _model.LowF; }
            set
            {
                _model.LowF = value;
                OnPropertyChanged();
            }
        }
        public string HighF
        {
            get { return _model.HighF; }
            set
            {
                _model.HighF = value;
                OnPropertyChanged();
            }
        }
        public string GuessF
        {
            get { return _model.GuessF; }
            set
            {
                _model.GuessF = value;
                OnPropertyChanged();
            }
        }
        public string DampMax
        {
            get { return _model.DampMax; }
            set
            {
                _model.DampMax = value;
                OnPropertyChanged();
            }
        }
    }
    public class ModeMethodViewModel : ViewModelBase
    {
        private ModeMethodBase _model;

        public ModeMethodViewModel(ModeMethodBase alg)
        {
            this._model = alg;
        }
    }
    public class PeriodogramDetectorParametersViewModel : ViewModelBase
    {
        private PeriodogramDetectorModel _model;

        public PeriodogramDetectorParametersViewModel(PeriodogramDetectorModel fODetectorParameters)
        {
            _model = fODetectorParameters;
        }
        private DetectorWindowType _windowType;
        public DetectorWindowType WindowType
        {
            get
            {
                return _model.WindowType;
            }
            set
            {
                _model.WindowType = value;
                OnPropertyChanged();
            }
        }
        private string _frequencyInterval;
        public string FrequencyInterval
        {
            get
            {
                return _model.FrequencyInterval;
            }
            set
            {
                _model.FrequencyInterval = value;
                OnPropertyChanged();
            }
        }
        private int _windowLength;
        public int WindowLength
        {
            get
            {
                return _model.WindowLength;
            }
            set
            {
                _model.WindowLength = value;
                WindowOverlap = (int)Math.Floor(value / 2d);
                OnPropertyChanged();
            }
        }
        private int _windowOverlap;
        public int WindowOverlap
        {
            get
            {
                return _model.WindowOverlap;
            }
            set
            {
                _model.WindowOverlap = value;
                OnPropertyChanged();
            }
        }
        private string _medianFilterFrequencyWidth;
        public string MedianFilterFrequencyWidth
        {
            get
            {
                return _model.MedianFilterFrequencyWidth;
            }
            set
            {
                _model.MedianFilterFrequencyWidth = value;
                OnPropertyChanged();
            }
        }
        private string _pfa;
        public string Pfa
        {
            get
            {
                return _model.Pfa;
            }
            set
            {
                _model.Pfa = value;
                OnPropertyChanged();
            }
        }
        private string _frequencyMin;
        public string FrequencyMin
        {
            get
            {
                return _model.FrequencyMin;
            }
            set
            {
                _model.FrequencyMin = value;
                OnPropertyChanged();
            }
        }
        private string _frequencyMax;
        public string FrequencyMax
        {
            get
            {
                return _model.FrequencyMax;
            }
            set
            {
                _model.FrequencyMax = value;
                OnPropertyChanged();
            }
        }
        private string _frequencyTolerance;
        public string FrequencyTolerance
        {
            get
            {
                return _model.FrequencyTolerance;
            }
            set
            {
                _model.FrequencyTolerance = value;
                OnPropertyChanged();
            }
        }
    }
}