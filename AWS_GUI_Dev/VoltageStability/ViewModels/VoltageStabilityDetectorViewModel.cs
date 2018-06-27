using BAWGUI.Core;
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
        }

        public VoltageStabilityDetectorViewModel(VoltageStabilityDetector model)
        {
            _model = model;
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            ThisStepInputsAsSignalHerachyByType = new SignalTypeHierachy(new SignalSignatureViewModel());
            AddSite = new RelayCommand(_addSite);
            AddVoltageBus = new RelayCommand(_addVoltageBus);
            AddBranch = new RelayCommand(_addBranch);
            AddShunt = new RelayCommand(_addShunt);
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
        public ObservableCollection<Site> Sites
        {
            get { return _model.Sites; }
            set
            {
                _model.Sites = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddSite { get; set; }
        private void _addSite(object obj)
        {
            _model.AddSite();
        }
        public ICommand AddVoltageBus { get; set; }

        private void _addVoltageBus(object obj)
        {
            _model.AddVoltageBus();
        }
        public ICommand AddBranch { get; set; }

        private void _addBranch(object obj)
        {
            _model.AddBranch();
        }
        public ICommand AddShunt { get; set; }

        private void _addShunt(object obj)
        {
            _model.AddShunt();
        }
    }
}
