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
            FODetectorParameters = new PeriodogramDetectorParametersViewModel(_model.FODetectorParameters);
            DeleteAMethod = new RelayCommand(_deleteAMethod);
            IsFODetecotrParametersVisible = _checkFOParameterVisibility();
            AddAMethod = new RelayCommand(_addAMethod);
        }

        private void OnModeMethodChanged(object sender, ModeMethodViewModel e)
        {
            IsFODetecotrParametersVisible = _checkFOParameterVisibility();
        }

        private bool _checkFOParameterVisibility()
        {
            foreach (var method in Methods)
            {
                if (method.Name == ModeMethods.LSARMAS)
                {
                    return true;
                }
                if (method.Name == ModeMethods.YWARMAS)
                {
                    return true;
                }
            }
            return false;
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
        public PeriodogramDetectorParametersViewModel FODetectorParameters { get; set; }
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
            IsFODetecotrParametersVisible = _checkFOParameterVisibility();
        }
        private bool _isFODetecotrParametersVisible = false;
        public bool IsFODetecotrParametersVisible
        {
            get { return _isFODetecotrParametersVisible; }
            set
            {
                _isFODetecotrParametersVisible = value;
                OnPropertyChanged();
            }
        }
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