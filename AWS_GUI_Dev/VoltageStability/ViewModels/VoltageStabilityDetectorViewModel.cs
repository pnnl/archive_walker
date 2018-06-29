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
        public VoltageStabilityDetectorViewModel()
        {
            _model = new VoltageStabilityDetector();
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            ThisStepInputsAsSignalHerachyByType = new SignalTypeHierachy(new SignalSignatureViewModel());
            AddSite = new RelayCommand(_addSite);
            AddVoltageBus = new RelayCommand(_addVoltageBus);
            AddBranch = new RelayCommand(_addBranch);
            AddShunt = new RelayCommand(_addShunt);
            SiteSelected = new RelayCommand(_siteSelected);
            SignalSelectedToChange = new RelayCommand(_signalSelectedToChange);
        }
        public VoltageStabilityDetectorViewModel(VoltageStabilityDetector model, SignalManager signalMgr) : this()
        {
            _model = model;
            var newSites = new ObservableCollection<SiteViewModel>();
            int siteCount = 0;
            foreach (var sub in _model.Sites)
            {
                siteCount++;
                var aSite = new SiteViewModel(sub, signalMgr);
                aSite.SiteNumber = siteCount;
                newSites.Add(aSite);
            }
            Sites = newSites;
        }

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
        public string EventMergeWindow
        {
            get { return _model.EventMergeWindow; }
            set
            {
                _model.EventMergeWindow = value;
                OnPropertyChanged();
            }
        }
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
            SelectedSite = (SiteViewModel)obj;
        }
        public ICommand SignalSelectedToChange { get; set; }
        private void _signalSelectedToChange(object obj)
        {
            //var parameters = (object[])obj;
            //var parameter1 = parameters[0];
            //var parameter2 = parameters[1];
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
                            parent.Frequency = newSignal;
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
                            parent2.Magnitude = newSignal;
                            break;
                        case "Angle":
                            parent2.Angle = newSignal;
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
                            parent3.ActivePower = newSignal;
                            break;
                        case "ReactivePower":
                            parent3.ReactivePower = newSignal;
                            break;
                        case "CurrentMagnitude":
                            parent3.CurrentMagnitude = newSignal;
                            break;
                        case "CurrentAngle":
                            parent3.CurrentAngle = newSignal;
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
                            parent4.ActivePower = newSignal;
                            break;
                        case "ReactivePower":
                            parent4.ReactivePower = newSignal;
                            break;
                        case "CurrentMagnitude":
                            parent4.CurrentMagnitude = newSignal;
                            break;
                        case "CurrentAngle":
                            parent4.CurrentAngle = newSignal;
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

        }
    }
}
