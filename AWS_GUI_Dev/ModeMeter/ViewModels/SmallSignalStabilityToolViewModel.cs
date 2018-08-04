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
            DeleteAMode = new RelayCommand(_deleteAMode);
            _modes = new ObservableCollection<ModeViewModel>();
            foreach (var mode in _model.Modes)
            {
                _modes.Add(new ModeViewModel(mode, _signalMgr));
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
            BaseliningSignalBoxSelected = new RelayCommand(_baseliningSignalBoxSelected);
            ModePMUSignalBoxSelected = new RelayCommand(_modePMUSignalBoxSelected);
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
            var newMode = new Mode();
            _model.Modes.Add(newMode);
            Modes.Add(new ModeViewModel(newMode, _signalMgr));
        }
        private ObservableCollection<ModeViewModel> _modes = new ObservableCollection<ModeViewModel>();
        public ObservableCollection<ModeViewModel> Modes
        {
            get { return _modes; }
            set
            {
                _modes = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SignalSignatureViewModel> BaseliningSignals { get; set; }
        public ICommand DeleteAMode { get; set; }
        private void _deleteAMode(object obj)
        {
            var oldModes = new ObservableCollection<ModeViewModel>(Modes);
            var toBeDeleted = obj as ModeViewModel;
            foreach (var mode in oldModes)
            {
                if (mode == toBeDeleted)
                {
                    oldModes.Remove(toBeDeleted);
                    break;
                }
            }
            foreach (var md in _model.Modes)
            {
                if (md == toBeDeleted.Model)
                {
                    _model.Modes.Remove(md);
                    break;
                }
            }
            Modes = oldModes;
        }
        public ICommand BaseliningSignalBoxSelected { get; set; }
        private void _baseliningSignalBoxSelected(object obj)
        {
            foreach (var signal in InputChannels)
            {
                signal.IsChecked = false;
            }
            InputChannels = new ObservableCollection<SignalSignatureViewModel>(BaseliningSignals);
            foreach (var signal in BaseliningSignals)
            {
                signal.IsChecked = true;
            }
        }
        public ICommand ModePMUSignalBoxSelected { get; set; }
        private void _modePMUSignalBoxSelected(object obj)
        {
            foreach (var signal in InputChannels)
            {
                signal.IsChecked = false;
            }
            var signals = obj as ObservableCollection<SignalSignatureViewModel>;
            InputChannels = new ObservableCollection<SignalSignatureViewModel>(signals);
            foreach (var signal in signals)
            {
                signal.IsChecked = true;
            }
        }


    }
}
