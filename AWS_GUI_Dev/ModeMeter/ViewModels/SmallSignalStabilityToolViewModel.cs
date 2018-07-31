using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using ModeMeter.Models;

namespace ModeMeter.ViewModels
{
    public class SmallSignalStabilityToolViewModel:DetectorBase
    {
        private SmallSignalStabilityTool _model;
        private SignalManager _signalMgr;

        public SmallSignalStabilityToolViewModel(SignalManager signalMgr)
        {
            _model = new SmallSignalStabilityTool();
            // need to set up result path if not exists
            _signalMgr = signalMgr;
            _setupMMViewModel();
        }

        public SmallSignalStabilityToolViewModel(SmallSignalStabilityTool model, SignalManager signalMgr)
        {
            this._model = model;
            _signalMgr = signalMgr;
            _setupMMViewModel();
        }
        private void _setupMMViewModel()
        {
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            AddAMode = new RelayCommand(_addAMode);
            Modes = new ObservableCollection<ModeViewModel>();
            foreach (var mode in _model.Modes)
            {
                Modes.Add(new ModeViewModel(mode));
            }
            BaseliningSignals = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var signal in _model.BaseliningSignals)
            {
                if (!string.IsNullOrEmpty(signal.SignalName))
                {
                    var thisSignal = _signalMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName);
                    BaseliningSignals.Add(thisSignal);
                }
                else
                {
                    _signalMgr.FindSignalsOfAPMU(BaseliningSignals, signal.PMUName);
                }
            }
        }
        public override string Name
        {
            get { return "Small Signal Stability Tool"; }
        }
        public string ModeMeterName
        {
            get { return _model.ModeMeterName; }
            set
            {
                _model.ModeMeterName = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddAMode { get; set; }
        private void _addAMode(object obj)
        {
            _model.Modes.Add(new Mode());
        }
        public ObservableCollection<ModeViewModel> Modes
        {
            get; set;
        }
        public ObservableCollection<SignalSignatureViewModel> BaseliningSignals { get; set; }
    }
}
