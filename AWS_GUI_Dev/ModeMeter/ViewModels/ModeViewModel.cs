using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.ReadConfigXml;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using ModeMeter.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ModeMeter.ViewModels
{
    public class ModeViewModel : ViewModelBase
    {
        private Mode _model;
        public Mode Model
        {
            get { return _model; }
        }
        public ModeViewModel(Mode mode, SignalManager _signalMgr)
        {
            this._model = mode;
            //_showEventDetectionParameters = false;
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
            _methods = new ObservableCollection<ModeMethodViewModel>();
            foreach (var alg in _model.AlgNames)
            {
                var newMethodVM = new ModeMethodViewModel(alg);
                newMethodVM.MethodChanged += OnModeMethodChanged;
                _methods.Add(newMethodVM);
                //switch (alg.Name)
                //{
                //    case "YW-ARMA":
                //        Methods.Add(new YWARMAViewModel((YWARMA)alg));
                //        break;
                //    case "LS-ARMA":
                //        Methods.Add(new LSARMAViewModel((LSARMA)alg));
                //        break;
                //    case "YW-ARMA+S":
                //        Methods.Add(new YWARMASViewModel((YWARMAS)alg));
                //        break;
                //    case "LS-ARMA+S":
                //        Methods.Add(new LSARMASViewModel((LSARMAS)alg));
                //        break;
                //    default:
                //        break;
                //}
            }
            FODetectorParameters = new FOdetectorParametersViewModel(_model.FODetectorParas, _signalMgr);
            DeleteAMethod = new RelayCommand(_deleteAMethod);
            //IsFODetecotrParametersVisible = _checkFOParameterVisibility();
            AddAMethod = new RelayCommand(_addAMethod);
            EventDetectionParameters = new EventDetectionParametersViewModel(_model.EventDetectionPara, _signalMgr);
        }

        private void OnModeMethodChanged(object sender, ModeMethodViewModel e)
        {
            //IsFODetecotrParametersVisible = _checkFOParameterVisibility();
        }

        //private bool _checkFOParameterVisibility()
        //{
        //    foreach (var method in Methods)
        //    {
        //        if (method.Name == ModeMethods.LSARMAS)
        //        {
        //            return true;
        //        }
        //        if (method.Name == ModeMethods.YWARMAS)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

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
                FODetectorParameters.FODetectorParams.WindowLength = (int)Math.Floor(value / 3d);
                OnPropertyChanged();
            }
        }
        //public string DampRatioThreshold
        //{
        //    get { return _model.DampRatioThreshold; }
        //    set
        //    {
        //        _model.DampRatioThreshold = value;
        //        OnPropertyChanged();
        //    }
        //}
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
        //private bool _showEventDetectionParameters;
        public bool ShowRMSEnergyTransientParameters
        {
            get { return _model.ShowRMSEnergyTransientParameters; }
            set
            {
                _model.ShowRMSEnergyTransientParameters = value;
                OnPropertyChanged();
            }
        }
        public bool ShowFOParameters 
        {
            get { return _model.ShowFOParameters; }
            set
            {
                _model.ShowFOParameters = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ModeMethodViewModel> _methods = new ObservableCollection<ModeMethodViewModel>();
        public ObservableCollection<ModeMethodViewModel> Methods
        {
            get { return _methods; }
            set
            {
                _methods = value;
                OnPropertyChanged();
            }
        }
        public FOdetectorParametersViewModel FODetectorParameters { get; set; }
        public EventDetectionParametersViewModel EventDetectionParameters { get; set; }
        public ICommand DeleteAMethod { get; set; }
        private void _deleteAMethod(object obj)
        {
            var toBeDeleted = obj as ModeMethodViewModel;
            var oldMethods = new ObservableCollection<ModeMethodViewModel>(Methods);
            foreach (var method in oldMethods)
            {
                if (method == toBeDeleted)
                {
                    oldMethods.Remove(toBeDeleted);
                    break;
                }
            }
            foreach (var mthd in _model.AlgNames)
            {
                if (mthd == toBeDeleted.Model)
                {
                    _model.AlgNames.Remove(mthd);
                    break;
                }
            }
            Methods = oldMethods;
            //IsFODetecotrParametersVisible = _checkFOParameterVisibility();
        }
        //private bool _isFODetecotrParametersVisible = false;
        //public bool IsFODetecotrParametersVisible
        //{
        //    get { return _isFODetecotrParametersVisible; }
        //    set
        //    {
        //        _isFODetecotrParametersVisible = value;
        //        OnPropertyChanged();
        //    }
        //}
        public ICommand AddAMethod { get; set; }
        private void _addAMethod(object obj)
        {
            var newMethod = new ModeMethod();
            _model.AlgNames.Add(newMethod);
            var newMethodVM = new ModeMethodViewModel(newMethod);
            newMethodVM.MethodChanged += OnModeMethodChanged;
            _methods.Add(newMethodVM);
            //IsFODetecotrParametersVisible = _checkFOParameterVisibility();
        }
    }
    public class ModeMethodViewModel : ViewModelBase
    {
        private ModeMethod _model;
        public ModeMethod Model
        {
            get { return _model; }
        }
        //public ModeMethodBase Model { get { return _model; } }
        public ModeMethodViewModel() { }
        public ModeMethodViewModel(ModeMethod alg)
        {
            this._model = alg;
        }
        public ModeMethods Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnModeMethodChanged(this);
                OnPropertyChanged();
            }
        }

        public event EventHandler<ModeMethodViewModel> MethodChanged;
        protected virtual void OnModeMethodChanged(ModeMethodViewModel e)
        {
            MethodChanged?.Invoke(this, e);
        }

        public string ARModelOrder
        {
            get { return _model.ARModelOrder; }
            set
            {
                _model.ARModelOrder = value;
                OnPropertyChanged();
            }
        }
        public string MAModelOrder
        {
            get { return _model.MAModelOrder; }
            set
            {
                _model.MAModelOrder = value;
                OnPropertyChanged();
            }
        }
        public string ExaggeratedARModelOrder
        {
            get { return _model.ExaggeratedARModelOrder; }
            set
            {
                _model.ExaggeratedARModelOrder = value;
                OnPropertyChanged();
            }
        }
        public string NumberOfEquations
        {
            get { return _model.NumberOfEquations; }
            set
            {
                _model.NumberOfEquations = value;
                OnPropertyChanged();
            }
        }
        public string NumberOfEquationsWithFOpresent
        {
            get { return _model.NumberOfEquationsWithFOpresent; }
            set
            {
                _model.NumberOfEquationsWithFOpresent = value;
                OnPropertyChanged();
            }
        }
        public string NaNomitLimit
        {
            get { return _model.NaNomitLimit; }
            set
            {
                _model.NaNomitLimit = value;
                OnPropertyChanged();
            }
        }
        public string MaximumIterations
        {
            get { return _model.MaximumIterations; }
            set
            {
                _model.MaximumIterations = value;
                OnPropertyChanged();
            }
        }
        public string SVThreshold
        {
            get { return _model.SVThreshold; }
            set
            {
                _model.SVThreshold = value;
                OnPropertyChanged();
            }
        }
    }
    //public class LSARMASViewModel : ModeMethodViewModel
    //{
    //    private LSARMAS _model;
    //    public LSARMASViewModel(LSARMAS alg) : base(alg)
    //    {
    //        _model = alg;
    //    }
    //    public string ExaggeratedARModelOrder
    //    {
    //        get { return _model.ExaggeratedARModelOrder; }
    //        set
    //        {
    //            _model.ExaggeratedARModelOrder = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}
    //public class YWARMASViewModel : ModeMethodViewModel
    //{
    //    private YWARMAS _model;
    //    public YWARMASViewModel(YWARMAS alg) : base(alg)
    //    {
    //        _model = alg;
    //    }
    //    public string NumberOfEquations
    //    {
    //        get { return _model.NumberOfEquations; }
    //        set
    //        {
    //            _model.NumberOfEquations = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    public string NumberOfEquationsWithFOpresent
    //    {
    //        get { return _model.NumberOfEquationsWithFOpresent; }
    //        set
    //        {
    //            _model.NumberOfEquationsWithFOpresent = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}
    //public class LSARMAViewModel : ModeMethodViewModel
    //{
    //    private LSARMA _model;
    //    public LSARMAViewModel(LSARMA alg) : base(alg)
    //    {
    //        _model = alg;
    //    }
    //    public string ExaggeratedARModelOrder
    //    {
    //        get { return _model.ExaggeratedARModelOrder; }
    //        set
    //        {
    //            _model.ExaggeratedARModelOrder = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}
    //public class YWARMAViewModel : ModeMethodViewModel
    //{
    //    private YWARMA _model;
    //    public YWARMAViewModel(YWARMA alg) : base(alg)
    //    {
    //        _model = alg;
    //    }
    //    public string NumberOfEquations
    //    {
    //        get { return _model.NumberOfEquations; }
    //        set
    //        {
    //            _model.NumberOfEquations = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}

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
    public class FOdetectorParametersViewModel : ViewModelBase
    {
        private FOdetectorParameters _model;
        public FOdetectorParametersViewModel()
        {
            _model = new FOdetectorParameters();
            FODetectorParams = new PeriodogramDetectorParametersViewModel(_model.FODetectorParams);
            FOtimeLocParams = new FOtimeLocParametersViewModel(_model.FOtimeLocParams);
            PMUs = new ObservableCollection<SignalSignatureViewModel>();
        }
        public FOdetectorParametersViewModel(FOdetectorParameters parameters, SignalManager _signalMgr)
        {
            _model = parameters;
            FODetectorParams = new PeriodogramDetectorParametersViewModel(parameters.FODetectorParams);
            FOtimeLocParams = new FOtimeLocParametersViewModel(parameters.FOtimeLocParams);
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
        }
        public PeriodogramDetectorParametersViewModel FODetectorParams { get; set; }
        public FOtimeLocParametersViewModel FOtimeLocParams { get; set; }
        public string MinTestStatWinLength 
        {
            get { return _model.MinTestStatWinLength; }
            set
            {
                _model.MinTestStatWinLength = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SignalSignatureViewModel> PMUs { get; set; }
    }

    public class FOtimeLocParametersViewModel : ViewModelBase
    {
        private FOtimeLocParameters _model;
        public FOtimeLocParametersViewModel(FOtimeLocParameters parameters)
        {
            _model = parameters;
        }
        public bool PerformTimeLoc
        {
            get { return _model.PerformTimeLoc; }
            set
            {
                _model.PerformTimeLoc = value;
                OnPropertyChanged();
            }
        }
        public string LocMinLength
        {
            get { return _model.LocMinLength; }
            set
            {
                _model.LocMinLength = value;
                OnPropertyChanged();
            }
        }
        public string LocLengthStep
        {
            get { return _model.LocLengthStep; }
            set
            {
                _model.LocLengthStep = value;
                OnPropertyChanged();
            }
        }
        public string LocRes
        {
            get { return _model.LocRes; }
            set
            {
                _model.LocRes = value;
                OnPropertyChanged();
            }
        }
    }

    public class PeriodogramDetectorParametersViewModel : ViewModelBase
    {
        private PeriodogramDetectorModel _model;
        public PeriodogramDetectorParametersViewModel()
        {
            _model = new PeriodogramDetectorModel();
        }

        public PeriodogramDetectorParametersViewModel(PeriodogramDetectorModel fODetectorParameters)
        {
            _model = fODetectorParameters;
        }
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
        public bool CalcDEF
        {
            get
            {
                return _model.CalcDEF;
            }
            set
            {
                _model.CalcDEF = value;
                OnPropertyChanged();
            }
        }
    }
    public class EventDetectionParametersViewModel : ViewModelBase
    {
        private EventDetectionParameters _model;
        public EventDetectionParametersViewModel(EventDetectionParameters m, SignalManager _signalMgr)
        {
            _model = m;
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
        }
        //public string RMSlength
        //{
        //    get { return _model.RMSlength; }
        //    set
        //    {
        //        _model.RMSlength = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string RMSmedianFilterTime
        //{
        //    get { return _model.RMSmedianFilterTime; }
        //    set
        //    {
        //        _model.RMSmedianFilterTime = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string RingThresholdScale
        //{
        //    get { return _model.RingThresholdScale; }
        //    set
        //    {
        //        _model.RingThresholdScale = value;
        //        OnPropertyChanged();
        //    }
        //}
        public string MinAnalysisLength
        {
            get { return _model.MinAnalysisLength; }
            set
            {
                _model.MinAnalysisLength = value;
                OnPropertyChanged();
            }
        }
        public bool RingdownID
        {
            get { return _model.RingdownID; }
            set
            {
                _model.RingdownID = value;
                OnPropertyChanged();
            }
        }
        public string Threshold
        {
            get { return _model.Threshold; }
            set
            {
                _model.Threshold = value;
                OnPropertyChanged();
            }
        }
        public ForgetFactor1Type ForgetFactor1
        {
            get { return _model.ForgetFactor1; }
            set
            {
                _model.ForgetFactor1 = value;
                OnPropertyChanged();
            }
        }
        public ForgetFactor2Type ForgetFactor2
        {
            get { return _model.ForgetFactor2; }
            set
            {
                _model.ForgetFactor2 = value;
                OnPropertyChanged();
            }
        }
        public PostEventWinAdjType PostEventWinAdj
        {
            get { return _model.PostEventWinAdj; }
            set
            {
                _model.PostEventWinAdj = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SignalSignatureViewModel> PMUs { get; set; }
    }
}