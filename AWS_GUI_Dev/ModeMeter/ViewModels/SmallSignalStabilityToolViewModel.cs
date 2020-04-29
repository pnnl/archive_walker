using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public SmallSignalStabilityToolViewModel(SignalManager signalMgr) : this()
        {
            _model = new SmallSignalStabilityTool();
            // need to set up result path if not exists
            _signalMgr = signalMgr;
            _setupMMViewModel();
        }

        public SmallSignalStabilityToolViewModel(SmallSignalStabilityTool model, SignalManager signalMgr) : this()
        {
            this._model = model;
            _signalMgr = signalMgr;
            _setupMMViewModel();
        }

        public SmallSignalStabilityToolViewModel()
        {
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            ThisStepInputsAsSignalHerachyByType = new SignalTypeHierachy();
            IsExpanded = false;
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
            InputChannels = new ObservableCollection<SignalSignatureViewModel>(BaseliningSignals);
            BaseliningSignalBoxSelected = new RelayCommand(_baseliningSignalBoxSelected);
            ModePMUSignalBoxSelected = new RelayCommand(_modePMUSignalBoxSelected);
            StepCounter = _signalMgr.GroupedSignalByDetectorInput.Count + 1;
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " + (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString() + " " + Name;
            try
            {
                ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(InputChannels);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sorting output signals by PMU in step: " + Name);
            }
            _signalMgr.GroupedSignalByDetectorInput.Add(ThisStepInputsAsSignalHerachyByType);
            AModePMUSignalBoxSelected = new RelayCommand(_aModePMUSignalBoxSelected);
            SetCurrentMode = new RelayCommand(_setCurrentMode);
            CheckSignalSelectedEqual = new RelayCommand(_checkSignalSelectedEqual);
        }
        public override string Name
        {
            get { return _model.Name; }
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
        public bool CalcDEF
        {
            get { return _model.CalcDEF; }
            set
            {
                _model.CalcDEF = value;
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
            //InputChannels = new ObservableCollection<SignalSignatureViewModel>(BaseliningSignals);
            InputChannels = BaseliningSignals;
            foreach (var signal in BaseliningSignals)
            {
                signal.IsChecked = true;
            }
            _signalMgr.DetermineAllParentNodeStatus();
            _signalMgr.DetermineFileDirCheckableStatus();
            if (InputChannels.Count() > 0)
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, InputChannels.FirstOrDefault().SamplingRate);
            }
            else
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, -1);
            }
        }
        public ICommand ModePMUSignalBoxSelected { get; set; }
        private void _modePMUSignalBoxSelected(object obj)
        {
            foreach (var signal in InputChannels)
            {
                signal.IsChecked = false;
            }
            //var signals = obj as ObservableCollection<SignalSignatureViewModel>;
            //InputChannels = new ObservableCollection<SignalSignatureViewModel>(signals);
            InputChannels = obj as ObservableCollection<SignalSignatureViewModel>;
            foreach (var signal in InputChannels)
            {
                signal.IsChecked = true;
            }
            _signalMgr.DetermineAllParentNodeStatus();
            _signalMgr.DetermineFileDirCheckableStatus();
            if (InputChannels.Count() > 0)
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, InputChannels.FirstOrDefault().SamplingRate);
            }
            else
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, -1);
            }
        }
        public void ChangeSignalSelection(SignalTypeHierachy obj)
        {
            bool checkStatus = (bool)obj.SignalSignature.IsChecked;
            //_signalMgr.CheckAllChildren(obj, checkStatus);
            //if (checkStatus)
            //{
            //    _addInputSignal(obj);
            //}
            //else
            //{
            //    _deleteInputSignal(obj);
            //}
            if (InputChannels == BaseliningSignals)
            {
                _changeBaseLiningSignalSelection(obj, checkStatus);
            }
            else
            {
                _changeMMSignalSelection(obj, checkStatus);
            }
            _signalMgr.DetermineAllParentNodeStatus();
            _signalMgr.DetermineFileDirCheckableStatus();
            if (InputChannels.Count() > 0)
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, InputChannels.FirstOrDefault().SamplingRate);
            }
            else
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, -1);
            }
        }

        private void _changeMMSignalSelection(SignalTypeHierachy obj, bool checkStatus)
        {
            if (obj.SignalList.Count > 0)
            {
                foreach (var signal in obj.SignalList)
                {
                    _changeMMSignalSelection(signal, checkStatus);
                }
            }
            else
            {
                if (!checkStatus && InputChannels.Contains(obj.SignalSignature))
                {
                    if (InputChannels == CurrentMode.PMUs)
                    {
                        if (_currentSelectedPMUSignal == null)
                        {
                            _indexOfCurrentSelectedPMUSignal = InputChannels.IndexOf(obj.SignalSignature);
                        }
                        InputChannels.RemoveAt(_indexOfCurrentSelectedPMUSignal);
                        if (CurrentMode.ShowFOParameters)
                        {
                            CurrentMode.FODetectorParameters.PMUs.RemoveAt(_indexOfCurrentSelectedPMUSignal);
                        }
                        if (CurrentMode.ShowRMSEnergyTransientParameters)
                        {
                            CurrentMode.EventDetectionParameters.PMUs.RemoveAt(_indexOfCurrentSelectedPMUSignal);
                        }
                        _currentSelectedPMUSignal = null;
                        _indexOfCurrentSelectedPMUSignal = -1;
                    }
                    if (InputChannels == CurrentMode.FODetectorParameters.PMUs || InputChannels == CurrentMode.EventDetectionParameters.PMUs)
                    {
                        if (InputChannels.Count() == CurrentMode.PMUs.Count())
                        {
                            MessageBox.Show("Forced oscillation signal and/or RMS Energy transient detection signal should have the same number of signals as the mode meter signal selection.");
                            obj.SignalSignature.IsChecked = true;
                            return;
                        }
                        else { InputChannels.Remove(obj.SignalSignature); }
                    }
                }
                if (checkStatus)
                {
                    if (_currentSelectedPMUSignal != null)
                    {
                        //var i = InputChannels.IndexOf(_currentSelectedPMUSignal);
                        //InputChannels.Remove(_currentSelectedPMUSignal);
                        InputChannels.RemoveAt(_indexOfCurrentSelectedPMUSignal);
                        InputChannels.Insert(_indexOfCurrentSelectedPMUSignal, obj.SignalSignature);
                        _currentSelectedPMUSignal.IsChecked = false;
                        _currentSelectedPMUSignal = obj.SignalSignature;
                    }
                    else
                    {
                        if (InputChannels == CurrentMode.PMUs)
                        {
                            InputChannels.Add(obj.SignalSignature);
                            if (CurrentMode.ShowFOParameters)
                            {
                                CurrentMode.FODetectorParameters.PMUs.Add(obj.SignalSignature);
                            }
                            if (CurrentMode.ShowRMSEnergyTransientParameters)
                            {
                                CurrentMode.EventDetectionParameters.PMUs.Add(obj.SignalSignature);
                            }
                        }
                        if (InputChannels == CurrentMode.FODetectorParameters.PMUs || InputChannels == CurrentMode.EventDetectionParameters.PMUs)
                        {
                            if (InputChannels.Count() == CurrentMode.PMUs.Count())
                            {
                                MessageBox.Show("Forced oscillation signal and/or RMS Energy transient detection signal should have the same number of signals as the mode meter signal selection.");
                                obj.SignalSignature.IsChecked = false;
                                return;
                            }
                            else { InputChannels.Add(obj.SignalSignature); }
                        }
                    }                    
                }
                obj.SignalSignature.IsChecked = checkStatus;
            }
        }

        private void _changeBaseLiningSignalSelection(SignalTypeHierachy obj, bool checkStatus)
        {
            if (obj.SignalList.Count > 0)
            {
                foreach (var signal in obj.SignalList)
                {
                    _changeBaseLiningSignalSelection(signal, checkStatus);
                }
            }
            else
            {
                if (!checkStatus && InputChannels.Contains(obj.SignalSignature))
                {
                    InputChannels.Remove(obj.SignalSignature);
                }
                if (checkStatus && !InputChannels.Contains(obj.SignalSignature))
                {
                    InputChannels.Add(obj.SignalSignature);
                }
                obj.SignalSignature.IsChecked = checkStatus;
            }
        }

        private void _deleteInputSignal(SignalTypeHierachy obj)
        {
            if (obj.SignalList.Count > 0)
            {
                foreach (var signal in obj.SignalList)
                {
                    _deleteInputSignal(signal);
                }
            }
            else
            {
                if (InputChannels.Contains(obj.SignalSignature))
                {
                    InputChannels.Remove(obj.SignalSignature);
                    obj.SignalSignature.IsChecked = false;
                }
            }
        }

        private void _addInputSignal(SignalTypeHierachy obj)
        {
            if (obj.SignalList.Count > 0)
            {
                foreach (var signal in obj.SignalList)
                {
                    _addInputSignal(signal);
                }
            }
            else
            {
                if (!InputChannels.Contains(obj.SignalSignature))
                {
                    InputChannels.Add(obj.SignalSignature);
                    obj.SignalSignature.IsChecked = true;
                }
            }
        }

        public override bool CheckStepIsComplete()
        {
            return true;
        }
        private SignalSignatureViewModel _currentSelectedPMUSignal;
        private int _indexOfCurrentSelectedPMUSignal;
        public ICommand AModePMUSignalBoxSelected { get; set; }
        private void _aModePMUSignalBoxSelected(object obj)
        {
            _currentSelectedPMUSignal = null;
            _indexOfCurrentSelectedPMUSignal = -1;
            if (InputChannels != null)
            {
                foreach (var item in InputChannels)
                {
                    item.IsChecked = false;
                }
            }
            //if (obj is SignalSignatureViewModel)
            //{
            //    var sig = obj as SignalSignatureViewModel;
            //    sig.IsChecked = true;
            //    _currentSelectedPMUSignal = sig;
            //}
            if (obj is ObservableCollection<SignalSignatureViewModel>)
            {
                InputChannels = (ObservableCollection<SignalSignatureViewModel>)obj;
            }
            else
            {
                if (obj is object[])
                {
                    var pArray = obj as object[];
                    if (pArray.Length == 3)
                    {
                        var sig = pArray[0] as SignalSignatureViewModel;
                        sig.IsChecked = true;
                        _currentSelectedPMUSignal = sig;
                        _indexOfCurrentSelectedPMUSignal = (int)pArray[1];
                        InputChannels = pArray[2] as ObservableCollection<SignalSignatureViewModel>;
                    }
                }
            }
            _signalMgr.DetermineAllParentNodeStatus();
            _signalMgr.DetermineFileDirCheckableStatus();
            if (InputChannels.Count() > 0)
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, InputChannels.FirstOrDefault().SamplingRate);
            }
            else
            {
                _signalMgr.DetermineSamplingRateCheckableStatus(this, 4, -1);
            }
        }
        public ModeViewModel CurrentMode { get; set; }
        public ICommand SetCurrentMode { get; set; }
        private void _setCurrentMode(object obj)
        {
            CurrentMode = obj as ModeViewModel;
        }
        public ICommand CheckSignalSelectedEqual { get; set; }
        private void _checkSignalSelectedEqual(object obj)
        {
            bool good = true;
            if (CurrentMode.ShowFOParameters && CurrentMode.PMUs.Count() != CurrentMode.FODetectorParameters.PMUs.Count())
            {
                good = false;
            }
            if (CurrentMode.ShowRMSEnergyTransientParameters && CurrentMode.PMUs.Count() != CurrentMode.EventDetectionParameters.PMUs.Count())
            {
                good = false;
            }
            if (!good)
            {
                MessageBox.Show("Forced oscillation signal and/or RMS Energy transient detection signal should have the same number of signals as the mode meter signal selection.");
            }
        }
    }
}
