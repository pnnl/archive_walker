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
using System.Windows.Input;

namespace DissipationEnergyFlow.ViewModels
{
    public class DEFDetectorViewModel : DetectorBase
    {
        private SignalManager _signalMgr;
        private DissipationEnergyFlowDetectorModel _model;
        public DEFDetectorViewModel()
        {
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            ThisStepInputsAsSignalHerachyByType = new SignalTypeHierachy();
            IsExpanded = false;
            _model = new DissipationEnergyFlowDetectorModel();
            AddAPath = new RelayCommand(_addAPath);
            DeleteAPath = new RelayCommand(_deleteAPath);
            Paths = new ObservableCollection<EnergyFlowPathViewModel>();
            Areas = new Dictionary<string, EnergyFlowAreaCoordsMappingViewModel>();
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
                aPath.VoltageAng = _signalMgr.SearchForSignalInTaggedSignals(path.VoltageAng.PMUName, path.VoltageAng.SignalName);
                aPath.ActivePowerP = _signalMgr.SearchForSignalInTaggedSignals(path.ActivePowerP.PMUName, path.ActivePowerP.SignalName);
                aPath.ReactivePowerQ = _signalMgr.SearchForSignalInTaggedSignals(path.ReactivePowerQ.PMUName, path.ReactivePowerQ.SignalName);
                //here need to check to see if the viewmodel of this DEF area exists in the DEF viewmodel's area dictionary or not
                //if exist, connect the DEF area VM to this DEF path
                //if not, create new vm for this DEF area and add this new vm to the dictionary.
                if (!Areas.ContainsKey(path.FromArea.AreaName))
                {
                    aPath.FromArea = new EnergyFlowAreaCoordsMappingViewModel(path.FromArea);
                    Areas[path.FromArea.AreaName] = aPath.FromArea;
                }
                else
                {
                    aPath.FromArea = Areas[path.FromArea.AreaName];
                }
                if (!Areas.ContainsKey(path.ToArea.AreaName))
                {
                    aPath.ToArea = new EnergyFlowAreaCoordsMappingViewModel(path.ToArea);
                    Areas[path.ToArea.AreaName] = aPath.ToArea;
                }
                else
                {
                    aPath.ToArea = Areas[path.ToArea.AreaName];
                }
                Paths.Add(aPath);
            }
        }
        public override string Name 
        {
            get { return _model.Name; }
        }
        public override bool CheckStepIsComplete()
        {
            return true;
        }
        public int LocMinLength 
        {
            get { return _model.LocLengthStep; }
            set
            {
                _model.LocLengthStep = value;
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
        public Dictionary<string, EnergyFlowAreaCoordsMappingViewModel> Areas { get; set; }
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
            FromArea = new EnergyFlowAreaCoordsMappingViewModel(_model.FromArea);
            ToArea = new EnergyFlowAreaCoordsMappingViewModel(_model.ToArea);
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
        public SignalSignatureViewModel VoltageMag { get; set; }
        public SignalSignatureViewModel VoltageAng { get; set; }
        public SignalSignatureViewModel ActivePowerP { get; set; }
        public SignalSignatureViewModel ReactivePowerQ { get; set; }
        public EnergyFlowAreaCoordsMappingViewModel FromArea { get; set; }
        public EnergyFlowAreaCoordsMappingViewModel ToArea { get; set; }
    }
}
