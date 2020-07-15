using BAWGUI.CoordinateMapping.ViewModels;
using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using DissipationEnergyFlow.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DissipationEnergyFlow.ViewModels
{
    public class DEFDetectorViewModel : DetectorBase
    {
        private SignalManager _signalMgr;
        private DissipationEnergyFlowDetectorModel _model;
        public DissipationEnergyFlowDetectorModel Model { get { return _model; } }
        public DEFDetectorViewModel()
        {
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            ThisStepInputsAsSignalHerachyByType = new SignalTypeHierachy();
            IsExpanded = false;
            _model = new DissipationEnergyFlowDetectorModel();
            AddAPath = new RelayCommand(_addAPath);
            DeleteAPath = new RelayCommand(_deleteAPath);
            Paths = new ObservableCollection<EnergyFlowPathViewModel>();
            Areas = new ObservableCollection<EnergyFlowAreaCoordsMappingViewModel>();
            UniqueAreas = new List<string>();
            SignalSelectedToChange = new RelayCommand(_setSelectedSignalToChange);
        }
        public DEFDetectorViewModel(SignalManager signalMgr) : this()
        {
            _signalMgr = signalMgr;
        }
        public DEFDetectorViewModel(DissipationEnergyFlowDetectorModel detector, SignalManager signalMgr) : this(signalMgr)
        {
            _model = detector;
            foreach (var path in _model.Paths)
            {
                var aPath = new EnergyFlowPathViewModel(path);
                aPath.VoltageMag = _signalMgr.SearchForSignalInTaggedSignals(path.VoltageMag.PMUName, path.VoltageMag.SignalName);
                if (aPath.VoltageMag != null && !InputChannels.Contains(aPath.VoltageMag))
                {
                    InputChannels.Add(aPath.VoltageMag);
                }
                aPath.VoltageAng = _signalMgr.SearchForSignalInTaggedSignals(path.VoltageAng.PMUName, path.VoltageAng.SignalName);
                if (aPath.VoltageAng != null && !InputChannels.Contains(aPath.VoltageAng))
                {
                    InputChannels.Add(aPath.VoltageAng);
                }
                aPath.ActivePowerP = _signalMgr.SearchForSignalInTaggedSignals(path.ActivePowerP.PMUName, path.ActivePowerP.SignalName);
                if (aPath.ActivePowerP != null && !InputChannels.Contains(aPath.ActivePowerP))
                {
                    InputChannels.Add(aPath.ActivePowerP);
                }
                aPath.ReactivePowerQ = _signalMgr.SearchForSignalInTaggedSignals(path.ReactivePowerQ.PMUName, path.ReactivePowerQ.SignalName);
                if (aPath.ReactivePowerQ != null && !InputChannels.Contains(aPath.ReactivePowerQ))
                {
                    InputChannels.Add(aPath.ReactivePowerQ);
                }
                //here need to check to see if the viewmodel of this DEF area exists in the DEF viewmodel's area dictionary or not
                //if exist, connect the DEF area VM to this DEF path
                //if not, create new vm for this DEF area and add this new vm to the dictionary.
                //if (!Areas.ContainsKey(path.FromArea.AreaName))
                //{
                //    aPath.FromArea = new EnergyFlowAreaCoordsMappingViewModel(path.FromArea);
                //    Areas[path.FromArea.AreaName] = aPath.FromArea;
                //}
                //else
                //{
                //    aPath.FromArea = Areas[path.FromArea.AreaName];
                //}
                //if (!Areas.ContainsKey(path.ToArea.AreaName))
                //{
                //    aPath.ToArea = new EnergyFlowAreaCoordsMappingViewModel(path.ToArea);
                //    Areas[path.ToArea.AreaName] = aPath.ToArea;
                //}
                //else
                //{
                //    aPath.ToArea = Areas[path.ToArea.AreaName];
                //}
                Paths.Add(aPath);
            }
            StepCounter = signalMgr.GroupedSignalByDetectorInput.Count + 1;
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " + (signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString() + " " + Name;
            try
            {
                ThisStepInputsAsSignalHerachyByType.SignalList = signalMgr.SortSignalByType(InputChannels);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sorting output signals by PMU in step: " + Name);
            }
            signalMgr.GroupedSignalByDetectorInput.Add(ThisStepInputsAsSignalHerachyByType);
        }
        public override string Name 
        {
            get { return _model.Name; }
        }
        public override bool CheckStepIsComplete()
        {
            return true;
        }
        public bool PerformTimeLoc 
        {
            get { return _model.PerformTimeLoc; }
            set {
                _model.PerformTimeLoc = value;
                OnPropertyChanged();
            } 
        }
        public int LocMinLength 
        {
            get { return _model.LocMinLength; }
            set
            {
                _model.LocMinLength = value;
                OnPropertyChanged();
            }
        }
        public int LocLengthStep
        {
            get { return _model.LocLengthStep; }
            set
            {
                _model.LocLengthStep = value;
                OnPropertyChanged();
            }
        }
        public int LocRes
        {
            get { return _model.LocRes; }
            set
            {
                _model.LocRes = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddAPath { get; set; }
        private void _addAPath(object obj)
        {
            var newPath = new EnergyFlowPathViewModel();
            _model.Paths.Add(newPath.Model);
            Paths.Add(newPath);
        }
        public ICommand DeleteAPath { get; set; }
        private void _deleteAPath(object obj)
        {
            var p = obj as EnergyFlowPathViewModel;
            _model.Paths.Remove(p.Model);
            Paths.Remove(p);
        }

        public ObservableCollection<EnergyFlowPathViewModel> Paths { get; set; }
        public ObservableCollection<EnergyFlowAreaCoordsMappingViewModel> Areas { get; set; }
        public List<string> UniqueAreas 
        {
            get { return _model.UniqueAreas; }
            set { _model.UniqueAreas = value; }
        }
        public EnergyFlowPathViewModel SelectedPathToChange { get; set; }
        public string SelectedSignalToChange { get; set; }
        public ICommand SignalSelectedToChange { get; set; }
        private void _setSelectedSignalToChange(object obj)
        {
            var parameters = (object[])obj;
            SelectedSignalToChange = parameters[0] as string;
            SelectedPathToChange = parameters[1] as EnergyFlowPathViewModel;
            foreach (var signal in InputChannels)
            {
                signal.IsChecked = false;
            }
            switch (SelectedSignalToChange)
            {
                case "VoltageMag":
                    if (SelectedPathToChange.VoltageMag != null)
                    {
                        SelectedPathToChange.VoltageMag.IsChecked = true;
                    }
                    break;
                case "VoltageAng":
                    if (SelectedPathToChange.VoltageAng != null)
                    {
                        SelectedPathToChange.VoltageAng.IsChecked = true;
                    }
                    break;
                case "ActivePowerP":
                    if (SelectedPathToChange.ActivePowerP != null)
                    {
                        SelectedPathToChange.ActivePowerP.IsChecked = true;
                    }
                    break;
                case "ReactivePowerQ":
                    if (SelectedPathToChange.ReactivePowerQ != null)
                    {
                        SelectedPathToChange.ReactivePowerQ.IsChecked = true;
                    }
                    break;
                default:
                    break;
            }
            _signalMgr.DetermineAllParentNodeStatus();
        }
        public void ChangeSignalSelection(SignalTypeHierachy obj)
        {
            if (obj.SignalList.Count > 0)
            {
                MessageBox.Show("Please select only one signal.");
            }
            else
            {
                bool checkStatus = (bool)obj.SignalSignature.IsChecked;
                if (checkStatus)
                {
                    switch (SelectedSignalToChange)
                    {
                        case "VoltageMag":
                            if (_checkTypeMatch("VoltageMag", obj.SignalSignature))
                            {
                                if (SelectedPathToChange.VoltageMag != null)
                                {
                                    SelectedPathToChange.VoltageMag.IsChecked = false;
                                }
                                SelectedPathToChange.VoltageMag = obj.SignalSignature;
                            }
                            else
                            {
                                MessageBox.Show("Please select voltage magnitude signal.");
                                obj.SignalSignature.IsChecked = false;
                            }
                            break;
                        case "VoltageAng":
                            if (_checkTypeMatch("VoltageAng", obj.SignalSignature))
                            {
                                if (SelectedPathToChange.VoltageAng != null)
                                {
                                    SelectedPathToChange.VoltageAng.IsChecked = false;
                                }
                                SelectedPathToChange.VoltageAng = obj.SignalSignature;
                            }
                            else
                            {
                                MessageBox.Show("Please select voltage angle signal.");
                                obj.SignalSignature.IsChecked = false;
                            }
                            break;
                        case "ActivePowerP":
                            if (_checkTypeMatch("ActivePowerP", obj.SignalSignature))
                            {
                                if (SelectedPathToChange.ActivePowerP != null)
                                {
                                    SelectedPathToChange.ActivePowerP.IsChecked = false;
                                }
                                SelectedPathToChange.ActivePowerP = obj.SignalSignature;
                            }
                            else
                            {
                                MessageBox.Show("Please select active power signal.");
                                obj.SignalSignature.IsChecked = false;
                            }
                            break;
                        case "ReactivePowerQ":
                            if (_checkTypeMatch("ReactivePowerQ", obj.SignalSignature))
                            {
                                if (SelectedPathToChange.ReactivePowerQ != null)
                                {
                                    SelectedPathToChange.ReactivePowerQ.IsChecked = false;
                                }
                                SelectedPathToChange.ReactivePowerQ = obj.SignalSignature;
                            }
                            else
                            {
                                MessageBox.Show("Please select reactive power signal.");
                                obj.SignalSignature.IsChecked = false;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (SelectedSignalToChange)
                    {
                        case "VoltageMag":
                            if (SelectedPathToChange.VoltageMag == obj.SignalSignature)
                            {
                                SelectedPathToChange.VoltageMag = null;
                            }
                            break;
                        case "VoltageAng":
                            if (SelectedPathToChange.VoltageAng == obj.SignalSignature)
                            {
                                SelectedPathToChange.VoltageAng = null;
                            }
                            break;
                        case "ActivePowerP":
                            if (SelectedPathToChange.ActivePowerP == obj.SignalSignature)
                            {
                                SelectedPathToChange.ActivePowerP = null;
                            }
                            break;
                        case "ReactivePowerQ":
                            if (SelectedPathToChange.ReactivePowerQ == obj.SignalSignature)
                            {
                                SelectedPathToChange.ReactivePowerQ = null;
                            }
                            break;
                        default:
                            break;
                    }
                }
                _updateInputChannel();
                _signalMgr.DetermineAllParentNodeStatus();
            }
        }

        private void _updateInputChannel()
        {
            InputChannels.Clear();
            foreach (var aPath in Paths)
            {
                if (aPath.VoltageMag != null && !InputChannels.Contains(aPath.VoltageMag))
                {
                    InputChannels.Add(aPath.VoltageMag);
                }
                if (aPath.VoltageAng != null && !InputChannels.Contains(aPath.VoltageAng))
                {
                    InputChannels.Add(aPath.VoltageAng);
                }
                if (aPath.ActivePowerP != null && !InputChannels.Contains(aPath.ActivePowerP))
                {
                    InputChannels.Add(aPath.ActivePowerP);
                }
                if (aPath.ReactivePowerQ != null && !InputChannels.Contains(aPath.ReactivePowerQ))
                {
                    InputChannels.Add(aPath.ReactivePowerQ);
                }
            }
        }

        private bool _checkTypeMatch(string v, SignalSignatureViewModel signal)
        {
            switch (v)
            {
                case "VoltageMag":
                    return signal.TypeAbbreviation == "VMP" || signal.TypeAbbreviation == "VMA" || signal.TypeAbbreviation == "VMB" || signal.TypeAbbreviation == "VMC";
                case "VoltageAng":
                    return signal.TypeAbbreviation == "VAP" || signal.TypeAbbreviation == "VAA" || signal.TypeAbbreviation == "VAB" || signal.TypeAbbreviation == "VAC";
                case "ActivePowerP":
                    return signal.TypeAbbreviation == "P";
                case "ReactivePowerQ":
                    return signal.TypeAbbreviation == "Q";
                default:
                    return false;
            }
        }
    }

    public class EnergyFlowPathViewModel : ViewModelBase
    {
        public EnergyFlowPathViewModel()
        {
            _model = new EnergyFlowPath();
            VoltageMag = new SignalSignatureViewModel();
            VoltageAng = new SignalSignatureViewModel();
            ActivePowerP = new SignalSignatureViewModel();
            ReactivePowerQ = new SignalSignatureViewModel();
            //FromArea = new EnergyFlowAreaCoordsMappingViewModel(_model.FromArea);
            //ToArea = new EnergyFlowAreaCoordsMappingViewModel(_model.ToArea);
        }
        public EnergyFlowPathViewModel(EnergyFlowPath path) : this()
        {
            _model = path;
        }
        private EnergyFlowPath _model;
        public EnergyFlowPath Model
        { 
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _voltageMag;
        public SignalSignatureViewModel VoltageMag 
        {
            get { return _voltageMag; }
            set
            {
                _voltageMag = value;
                if (value != null)
                {
                    _model.VoltageMag = value.Model;
                }
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _voltageAng;
        public SignalSignatureViewModel VoltageAng
        {
            get { return _voltageAng; }
            set
            {
                _voltageAng = value;
                if (value != null)
                {
                    _model.VoltageAng = value.Model;
                }
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _activePowerP;
        public SignalSignatureViewModel ActivePowerP
        {
            get { return _activePowerP; }
            set
            {
                _activePowerP = value;
                if (value != null)
                {
                    _model.ActivePowerP = value.Model;
                }
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _reactivePowerQ;
        public SignalSignatureViewModel ReactivePowerQ
        {
            get { return _reactivePowerQ; }
            set
            {
                _reactivePowerQ = value;
                if (value != null)
                {
                    _model.ReactivePowerQ = value.Model;
                }
                OnPropertyChanged();
            }
        }
        public string FromArea 
        {
            get { return _model.FromArea; }
            set
            {
                try
                {
                    _model.FromArea = value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                OnPropertyChanged();
            }
        }
        public string ToArea
        {
            get { return _model.ToArea; }
            set
            {
                try
                {
                    _model.ToArea = value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                OnPropertyChanged();
            }
        }
        //public EnergyFlowAreaCoordsMappingViewModel FromArea { get; set; }
        //public EnergyFlowAreaCoordsMappingViewModel ToArea { get; set; }
    }
}
