using BAWGUI.Core;
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
    public class SiteViewModel:ViewModelBase
    {
        private Site _model;

        public SiteViewModel()
        {
            _model = new Site();
            Frequency = new SignalSignatureViewModel();
            VoltageBuses = new ObservableCollection<VoltageBusViewModel>();
            //VoltageBuses.Add(new VoltageBusViewModel());
            BranchesAndShunts = new ObservableCollection<object>();
            //BranchesAndShunts.Add(new BranchViewModel());
            DeleteAVoltageBus = new RelayCommand(_deleteAVoltageBus);
            DeleteABranchOrShunt = new RelayCommand(_deleteABranchOrShunt);
        }
        public SiteViewModel(Site sub, BAWGUI.SignalManagement.ViewModels.SignalManager signalMgr)
        {
            this._model = sub;
            Frequency = signalMgr.SearchForSignalInTaggedSignals(_model.Frequency.PMU, _model.Frequency.SignalName);
            var newVoltageBuses = new ObservableCollection<VoltageBusViewModel>();
            foreach (var item in _model.VoltageBuses)
            {
                newVoltageBuses.Add(new VoltageBusViewModel(item, signalMgr));
            }
            VoltageBuses = newVoltageBuses;
            var newBranchesAndShunts = new ObservableCollection<object>();
            foreach (var item in _model.BranchesAndShunts)
            {
                if (item is Shunt)
                {
                    newBranchesAndShunts.Add(new ShuntViewModel((Shunt)item, signalMgr));
                }
                else
                {
                    newBranchesAndShunts.Add(new BranchViewModel((Branch)item, signalMgr));
                }
            }
            BranchesAndShunts = newBranchesAndShunts;
            DeleteAVoltageBus = new RelayCommand(_deleteAVoltageBus);
            DeleteABranchOrShunt = new RelayCommand(_deleteABranchOrShunt);
        }

        public SiteViewModel(Site sub)
        {
            this._model = sub;
            Frequency = new SignalSignatureViewModel();
            VoltageBuses = new ObservableCollection<VoltageBusViewModel>();
            foreach (var bus in _model.VoltageBuses)
            {
                VoltageBuses.Add(new VoltageBusViewModel(bus));
            }
            BranchesAndShunts = new ObservableCollection<object>();
            foreach (var bs in _model.BranchesAndShunts)
            {
                BranchesAndShunts.Add(new BranchViewModel((Branch)bs));
            }
            DeleteAVoltageBus = new RelayCommand(_deleteAVoltageBus);
            DeleteABranchOrShunt = new RelayCommand(_deleteABranchOrShunt);
        }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }
        public string StabilityThreshold
        {
            get { return _model.StabilityThreshold; }
            set
            {
                _model.StabilityThreshold = value;
                OnPropertyChanged();
            }
        }
        private SignalSignatureViewModel _frequency;
        public SignalSignatureViewModel Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<VoltageBusViewModel> _voltageBuses;
        public ObservableCollection<VoltageBusViewModel> VoltageBuses
        {
            get { return _voltageBuses; }
            set
            {
                _voltageBuses = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<object> _branchesAndShunts;
        public ObservableCollection<object> BranchesAndShunts
        {
            get { return _branchesAndShunts; }
            set
            {
                _branchesAndShunts = value;
                OnPropertyChanged();
            }
        }
        private int _siteNumber;
        public int SiteNumber
        {
            get { return _siteNumber; }
            set
            {
                _siteNumber = value;
                OnPropertyChanged();
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        public ICommand DeleteAVoltageBus { get; set; }
        private void _deleteAVoltageBus(object obj)
        {
            var vbToBeDeleted = obj as VoltageBusViewModel;
            VoltageBuses.Remove(vbToBeDeleted);
        }
        public ICommand DeleteABranchOrShunt { get; set; }
        private void _deleteABranchOrShunt(object obj)
        {
            BranchesAndShunts.Remove(obj);
        }
    }
}
