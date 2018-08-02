using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using ModeMeter.Models;
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
    public class ModeMethodViewModel
    {
        private ModeMethodBase _model;

        public ModeMethodViewModel(ModeMethodBase alg)
        {
            this._model = alg;
        }
    }
}