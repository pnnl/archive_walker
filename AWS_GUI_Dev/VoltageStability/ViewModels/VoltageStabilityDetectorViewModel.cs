using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoltageStability.Models;

namespace VoltageStability.ViewModels
{
    public class VoltageStabilityDetectorViewModel:DetectorBase
    {
        public VoltageStabilityDetectorViewModel(SignalManager signalMgr)
        {
            var newDetector = new VoltageStabilityDetector();
            newDetector.Sites = new ObservableCollection<Site>();
            var newSite = new Site();
            newSite.BranchesAndShunts = new List<object>();
            newSite.BranchesAndShunts.Add(new Branch());
            newSite.VoltageBuses = new List<VoltageBus>();
            newSite.VoltageBuses.Add(new VoltageBus());
            newDetector.Sites.Add(newSite);
            _model = newDetector;
            _signalMgr = signalMgr;
            _setUpVSViewModel();
        }
        public VoltageStabilityDetectorViewModel(VoltageStabilityDetector model, SignalManager signalMgr)
        {
            _model = model;
            _signalMgr = signalMgr;
            _setUpVSViewModel();
        }

        private void _setUpVSViewModel()
        {
            var newSites = new ObservableCollection<SiteViewModel>();
            int siteCount = 0;
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var sub in _model.Sites)
            {
                siteCount++;
                var aSite = new SiteViewModel(sub, _signalMgr);
                aSite.SiteNumber = siteCount;
                newSites.Add(aSite);
            }
            Sites = newSites;
            _updateInputChannel();

            AddSite = new RelayCommand(_addSite);
            AddVoltageBus = new RelayCommand(_addVoltageBus);
            AddBranch = new RelayCommand(_addBranch);
            AddShunt = new RelayCommand(_addShunt);
            SiteSelected = new RelayCommand(_siteSelected);
            SignalSelectedToChange = new RelayCommand(_signalSelectedToChange);
            DeleteASite = new RelayCommand(_deleteASite);
        }

        private void _updateInputChannel()
        {
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var aSite in Sites)
            {
                if (aSite.Frequency != null && !InputChannels.Contains(aSite.Frequency))
                {
                    InputChannels.Add(aSite.Frequency);
                }
                foreach (var item in aSite.VoltageBuses)
                {
                    if (item.Magnitude != null && !InputChannels.Contains(item.Magnitude))
                    {
                        InputChannels.Add(item.Magnitude);
                    }
                    if (item.Angle != null && !InputChannels.Contains(item.Angle))
                    {
                        InputChannels.Add(item.Angle);
                    }
                }
                foreach (var item in aSite.BranchesAndShunts)
                {
                    if (item is ShuntViewModel)
                    {
                        var shunt = item as ShuntViewModel;
                        if (shunt.ActivePower != null && !InputChannels.Contains(shunt.ActivePower))
                        {
                            InputChannels.Add(shunt.ActivePower);
                        }
                        if (shunt.ReactivePower != null && !InputChannels.Contains(shunt.ReactivePower))
                        {
                            InputChannels.Add(shunt.ReactivePower);
                        }
                        if (shunt.CurrentAngle != null && !InputChannels.Contains(shunt.CurrentAngle))
                        {
                            InputChannels.Add(shunt.CurrentAngle);
                        }
                        if (shunt.CurrentMagnitude != null && !InputChannels.Contains(shunt.CurrentMagnitude))
                        {
                            InputChannels.Add(shunt.CurrentMagnitude);
                        }
                    }
                    else
                    {
                        var branch = item as BranchViewModel;
                        if (branch.ActivePower != null && !InputChannels.Contains(branch.ActivePower))
                        {
                            InputChannels.Add(branch.ActivePower);
                        }
                        if (branch.ReactivePower != null && !InputChannels.Contains(branch.ReactivePower))
                        {
                            InputChannels.Add(branch.ReactivePower);
                        }
                        if (branch.CurrentAngle != null && !InputChannels.Contains(branch.CurrentAngle))
                        {
                            InputChannels.Add(branch.CurrentAngle);
                        }
                        if (branch.CurrentMagnitude != null && !InputChannels.Contains(branch.CurrentMagnitude))
                        {
                            InputChannels.Add(branch.CurrentMagnitude);
                        }
                    }
                }
            }
        }

        private SignalManager _signalMgr;
        private VoltageStabilityDetector _model;

        public override string Name
        {
            get { return "Voltage Stability Detector"; }
        }
        public bool IsDeMarcoMethod
        {
            get { return _model.IsDeMarcoMethod; }
            set
            {
                _model.IsDeMarcoMethod = value;
                OnPropertyChanged();
            }
        }
        public bool IsMitsubishiMethod
        {
            get { return _model.IsMitsubishiMethod; }
            set
            {
                _model.IsMitsubishiMethod = value;
                OnPropertyChanged();
            }
        }
        public bool IsQuantaMethod
        {
            get { return _model.IsQuantaMethod; }
            set
            {
                _model.IsQuantaMethod = value;
                OnPropertyChanged();
            }
        }
        public bool IsChowMethod
        {
            get { return _model.IsChowMethod; }
            set
            {
                _model.IsChowMethod = value;
                OnPropertyChanged();
            }
        }
        public bool IsTellegenMethod
        {
            get { return _model.IsTellegenMethod; }
            set
            {
                _model.IsTellegenMethod = value;
                OnPropertyChanged();
            }
        }
        public string DeMarcoAnalysisLength
        {
            get { return _model.DeMarcoAnalysisLength; }
            set
            {
                _model.DeMarcoAnalysisLength = value;
                OnPropertyChanged();
            }
        }
        public string MitsubishiAnalysisLength
        {
            get { return _model.MitsubishiAnalysisLength; }
            set
            {
                _model.MitsubishiAnalysisLength = value;
                OnPropertyChanged();
            }
        }
        public string RPIAnalysisLength
        {
            get { return _model.RPIAnalysisLength; }
            set
            {
                _model.RPIAnalysisLength = value;
                OnPropertyChanged();
            }
        }
        public string EventMergeWindow
        {
            get { return _model.EventMergeWindow; }
            set
            {
                _model.EventMergeWindow = value;
                OnPropertyChanged();
            }
        }
        public string DetectorGroupID { get { return _model.DetectorGroupID; } }
        public int MethodsCount { get { return _model.MethodsCount; } }
        public List<String> Methods { get { return _model.Methods; } }
        private ObservableCollection<SiteViewModel> _sites;
        public ObservableCollection<SiteViewModel> Sites
        {
            get { return _sites; }
            set
            {
                _sites = value;
                OnPropertyChanged();
            }
        }
        private SiteViewModel _selectedSite;
        public SiteViewModel SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                _selectedSite = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddSite { get; set; }
        private void _addSite(object obj)
        {
            var newSite = new SiteViewModel();
            newSite.SiteNumber = Sites.Count() + 1;
            newSite.BranchesAndShunts = new ObservableCollection<object>();
            newSite.BranchesAndShunts.Add(new BranchViewModel());
            newSite.VoltageBuses = new ObservableCollection<VoltageBusViewModel>();
            newSite.VoltageBuses.Add(new VoltageBusViewModel());
            _sites.Add(newSite);
        }
        public ICommand AddVoltageBus { get; set; }
        private void _addVoltageBus(object obj)
        {
            if (SelectedSite != null)
            {
                SelectedSite.VoltageBuses.Add(new VoltageBusViewModel());
            }
        }
        public ICommand AddBranch { get; set; }
        private void _addBranch(object obj)
        {
            if (SelectedSite != null)
            {
                SelectedSite.BranchesAndShunts.Add(new BranchViewModel());
            }
        }
        public ICommand AddShunt { get; set; }
        private void _addShunt(object obj)
        {
            if (SelectedSite != null)
            {
                SelectedSite.BranchesAndShunts.Add(new ShuntViewModel());
            }
        }
        public ICommand SiteSelected { get; set; }
        private void _siteSelected(object obj)
        {
            if (SelectedSite != null)
            {
                SelectedSite.IsSelected = false;
            }
            SelectedSite = (SiteViewModel)obj;
            SelectedSite.IsSelected = true;
        }
        public ICommand SignalSelectedToChange { get; set; }
        private void _signalSelectedToChange(object obj)
        {
            var parameters = (object[])obj;
            var parameter1 = parameters[0];
            var parameter2 = parameters[1];
            SignalSignatureViewModel keepSignal = null;
            switch (parameter1.GetType().Name)
            {
                case "SiteViewModel":
                    var parent = parameter1 as SiteViewModel;
                    switch (parameter2)
                    {
                        case "Frequency": 
                            keepSignal = parent.Frequency;
                            break;
                        default:
                            break;
                    }
                    break;
                case "VoltageBusViewModel":
                    var parent2 = parameter1 as VoltageBusViewModel;
                    switch (parameter2)
                    {
                        case "Magnitude":
                            keepSignal = parent2.Magnitude;
                            break;
                        case "Angle":
                            keepSignal = parent2.Angle;
                            break;
                        default:
                            break;
                    }
                    break;
                case "BranchViewModel":
                    var parent3 = parameter1 as BranchViewModel;
                    switch (parameter2)
                    {
                        case "ActivePower":
                            keepSignal = parent3.ActivePower;
                            break;
                        case "ReactivePower":
                            keepSignal = parent3.ReactivePower;
                            break;
                        case "CurrentMagnitude":
                            keepSignal = parent3.CurrentMagnitude;
                            break;
                        case "CurrentAngle":
                            keepSignal = parent3.CurrentAngle;
                            break;
                        default:
                            break;
                    }
                    break;
                case "ShuntViewModel":
                    var parent4 = parameter1 as ShuntViewModel;
                    switch (parameter2)
                    {
                        case "ActivePower":
                            keepSignal = parent4.ActivePower;
                            break;
                        case "ReactivePower":
                            keepSignal = parent4.ReactivePower;
                            break;
                        case "CurrentMagnitude":
                            keepSignal = parent4.CurrentMagnitude;
                            break;
                        case "CurrentAngle":
                            keepSignal = parent4.CurrentAngle;
                            break;
                        default:
                            break;
                    }
                    break;
                //case "VoltageBusViewModel":
                //    break;
                default:
                    break;
            }
                foreach (var sig in InputChannels)
                {
                    if (sig != keepSignal)
                    {
                        sig.IsChecked = false;
                    }
            }
            if (keepSignal != null)
            {
                keepSignal.IsChecked = true;
            }
            //_signalMgr.DetermineDataConfigPostProcessConfigAllParentNodeStatus();
            _signalMgr.DetermineAllParentNodeStatus();
            SelectedSignalToChange = obj;
            //SelectedSignalToChange = (SignalSignatureViewModel)obj[1];
        }
        public object SelectedSignalToChange { get; set; }
        public void ChangeASignal(SignalTypeHierachy obj)
        {
            if (obj.SignalList.Count() > 0)
            {
                throw new Exception("Please select only a single signal for voltage stability detector.");
            }
            var newSignal = obj.SignalSignature;
            var parameters = (object[])SelectedSignalToChange;
            var parameter1 = parameters[0];
            var parameter2 = parameters[1];
            switch (parameter1.GetType().Name)
            {
                case "SiteViewModel":
                    var parent = parameter1 as SiteViewModel;
                    switch (parameter2)
                    {
                        case "Frequency":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent.Frequency != null)
                                {
                                    parent.Frequency.IsChecked = false;
                                }
                                parent.Frequency = newSignal;
                            }
                            else
                            {
                                parent.Frequency = null;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "VoltageBusViewModel":
                    var parent2 = parameter1 as VoltageBusViewModel;
                    switch(parameter2)
                    {
                        case "Magnitude":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent2.Magnitude != null)
                                {
                                    parent2.Magnitude.IsChecked = false;
                                }
                                parent2.Magnitude = newSignal;
                            }
                            else
                            {
                                parent2.Magnitude = null;
                            }
                            break;
                        case "Angle":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent2.Angle != null)
                                {
                                    parent2.Angle.IsChecked = false;
                                }
                                parent2.Angle = newSignal;
                            }
                            else
                            {
                                parent2.Angle = null;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "BranchViewModel":
                    var parent3 = parameter1 as BranchViewModel;
                    switch (parameter2)
                    {
                        case "ActivePower":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent3.ActivePower != null)
                                {
                                    parent3.ActivePower.IsChecked = false;
                                }
                                parent3.ActivePower = newSignal;
                            }
                            else
                            {
                                parent3.ActivePower = null;
                            }
                            break;
                        case "ReactivePower":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent3.ReactivePower != null)
                                {
                                    parent3.ReactivePower.IsChecked = false;
                                }
                                parent3.ReactivePower = newSignal;
                            }
                            else
                            {
                                parent3.ReactivePower = null;
                            }
                            break;
                        case "CurrentMagnitude":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent3.CurrentMagnitude != null)
                                {
                                    parent3.CurrentMagnitude.IsChecked = false;
                                }
                                parent3.CurrentMagnitude = newSignal;
                            }
                            else
                            {
                                parent3.CurrentMagnitude = null;
                            }
                            break;
                        case "CurrentAngle":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent3.CurrentAngle != null)
                                {
                                    parent3.CurrentAngle.IsChecked = false;
                                }
                                parent3.CurrentAngle = newSignal;
                            }
                            else
                            {
                                parent3.CurrentAngle = null;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "ShuntViewModel":
                    var parent4 = parameter1 as ShuntViewModel;
                    switch (parameter2)
                    {
                        case "ActivePower":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent4.ActivePower != null)
                                {
                                    parent4.ActivePower.IsChecked = false;
                                }
                                parent4.ActivePower = newSignal;
                            }
                            else
                            {
                                parent4.ActivePower = null;
                            }
                            break;
                        case "ReactivePower":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent4.ReactivePower != null)
                                {
                                    parent4.ReactivePower.IsChecked = false;
                                }
                                parent4.ReactivePower = newSignal;
                            }
                            else
                            {
                                parent4.ReactivePower = null;
                            }
                            break;
                        case "CurrentMagnitude":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent4.CurrentMagnitude != null)
                                {
                                    parent4.CurrentMagnitude.IsChecked = false;
                                }
                                parent4.CurrentMagnitude = newSignal;
                            }
                            else
                            {
                                parent4.CurrentMagnitude = null;
                            }
                            break;
                        case "CurrentAngle":
                            if ((bool)newSignal.IsChecked)
                            {
                                if (parent4.CurrentAngle != null)
                                {
                                    parent4.CurrentAngle.IsChecked = false;
                                }
                                parent4.CurrentAngle = newSignal;
                            }
                            else
                            {
                                parent4.CurrentAngle = null;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                //case "VoltageBusViewModel":
                //    break;
                default:
                    break;
            }
            _updateInputChannel();
            _signalMgr.DetermineAllParentNodeStatus();
        }
        public ICommand DeleteASite { get; set; }
        private void _deleteASite(object obj)
        {
            var siteToBeDeleted = obj as SiteViewModel;
            _sites.Remove(siteToBeDeleted);
            foreach (var site in Sites)
            {
                if (site.SiteNumber > siteToBeDeleted.SiteNumber)
                {
                    site.SiteNumber = site.SiteNumber - 1;
                }
            }
        }

        public override bool CheckStepIsComplete()
        {
            return true;
        }
    }
}
