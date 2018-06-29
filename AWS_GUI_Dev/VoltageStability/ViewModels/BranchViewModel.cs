using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using VoltageStability.Models;

namespace VoltageStability.ViewModels
{
    public class BranchViewModel : ViewModelBase
    {
        private Branch _model;

        public BranchViewModel()
        {
            _model = new Branch();
            ActivePower = new SignalSignatureViewModel();
            ReactivePower = new SignalSignatureViewModel();
            CurrentMagnitude = new SignalSignatureViewModel();
            CurrentAngle = new SignalSignatureViewModel();
        }

        public BranchViewModel(Branch item, SignalManager signalMgr)
        {
            this._model = item;
            ActivePower = signalMgr.SearchForSignalInTaggedSignals(_model.ActivePower.PMU, _model.ActivePower.SignalName);
            ReactivePower = signalMgr.SearchForSignalInTaggedSignals(_model.ReactivePower.PMU, _model.ReactivePower.SignalName);
            CurrentMagnitude = signalMgr.SearchForSignalInTaggedSignals(_model.CurrentMagnitude.PMU, _model.CurrentMagnitude.SignalName);
            CurrentAngle = signalMgr.SearchForSignalInTaggedSignals(_model.CurrentAngle.PMU, _model.CurrentAngle.SignalName);
        }
        private SignalSignatureViewModel _activePower;
        public SignalSignatureViewModel ActivePower
        {
            get { return _activePower; }
            set
            {
                _activePower = value;
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _reactivePower;
        public SignalSignatureViewModel ReactivePower
        {
            get { return _reactivePower; }
            set
            {
                _reactivePower = value;
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _currentMagnitude;
        public SignalSignatureViewModel CurrentMagnitude
        {
            get { return _currentMagnitude; }
            set
            {
                _currentMagnitude = value;
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _currentAngle;
        public SignalSignatureViewModel CurrentAngle
        {
            get { return _currentAngle; }
            set
            {
                _currentAngle = value;
                OnPropertyChanged();
            }
        }
        public string Name { get { return _model.Name; } }
    }
    public class ShuntViewModel : BranchViewModel
    {
        private Shunt _model;
        public ShuntViewModel() : base()
        {
            _model = new Shunt();
        }
        public ShuntViewModel(Shunt item, SignalManager signalMgr)
        {
            this._model = item;
            ActivePower = signalMgr.SearchForSignalInTaggedSignals(_model.ActivePower.PMU, _model.ActivePower.SignalName);
            ReactivePower = signalMgr.SearchForSignalInTaggedSignals(_model.ReactivePower.PMU, _model.ReactivePower.SignalName);
            CurrentMagnitude = signalMgr.SearchForSignalInTaggedSignals(_model.CurrentMagnitude.PMU, _model.CurrentMagnitude.SignalName);
            CurrentAngle = signalMgr.SearchForSignalInTaggedSignals(_model.CurrentAngle.PMU, _model.CurrentAngle.SignalName);
        }
        public new string Name { get { return _model.Name; } }
    }
}