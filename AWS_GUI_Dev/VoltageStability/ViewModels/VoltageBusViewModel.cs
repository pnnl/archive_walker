using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using VoltageStability.Models;

namespace VoltageStability.ViewModels
{
    public class VoltageBusViewModel:ViewModelBase
    {
        private VoltageBus _model;

        public VoltageBusViewModel()
        {
            _model = new VoltageBus();
            Magnitude = new SignalSignatureViewModel();
            Angle = new SignalSignatureViewModel();
        }

        public VoltageBusViewModel(VoltageBus item, SignalManager signalMgr)
        {
            this._model = item;
            Magnitude = signalMgr.SearchForSignalInTaggedSignals(_model.Magnitude.PMU, _model.Magnitude.SignalName);
            Angle = signalMgr.SearchForSignalInTaggedSignals(_model.Angle.PMU, _model.Angle.SignalName);
        }
        private SignalSignatureViewModel _magnitude;
        public SignalSignatureViewModel Magnitude
        {
            get { return _magnitude; }
            set
            {
                _magnitude = value;
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _angle;
        public SignalSignatureViewModel Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                OnPropertyChanged();
            }
        }
    }
}