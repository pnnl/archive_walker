using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BAWGUI.CoordinateMapping.ViewModels
{
    public class SiteMappingViewModel : ViewModelBase
    {
        public SiteMappingViewModel()
        {
            DEFAreaSiteMappingVM = new DEFAreaSiteSetupViewModel();
            SignalCoordsMappingVM = new SignalCoordsMappingViewModel();
            _availableSites = new ObservableCollection<SiteCoordinatesViewModel>();
            TargetSelected = new RelayCommand(_targetSelected);
            _targetIsSignal = false;
        }
        public SiteMappingViewModel(ObservableCollection<SiteCoordinatesViewModel> siteCoords) : this()
        {
            AvailableSites = siteCoords;
        }
        private DEFAreaSiteSetupViewModel _defAreaSiteMappingVM;
        public DEFAreaSiteSetupViewModel DEFAreaSiteMappingVM 
        { get { return _defAreaSiteMappingVM; }
            set
            {
                _defAreaSiteMappingVM = value;
                OnPropertyChanged();
            }
        }
        private SignalCoordsMappingViewModel _signalCoordsMappingVM;
        public SignalCoordsMappingViewModel SignalCoordsMappingVM 
        { get { return _signalCoordsMappingVM; }
            set
            {
                _signalCoordsMappingVM = value;
                OnPropertyChanged();
            }
        }
        //private ObservableCollection<SiteCoordinatesViewModel> _availableSites;
        private ObservableCollection<SiteCoordinatesViewModel> _availableSites;
        public ObservableCollection<SiteCoordinatesViewModel> AvailableSites 
        {
            get { return _availableSites; }
            set
            {
                if (_availableSites != value)
                {
                    _availableSites = value;
                    OnPropertyChanged();
                    DEFAreaSiteMappingVM.AvailableSites = value;
                    SignalCoordsMappingVM.AvailableSites = value;
                }
            }
        }
        private bool _targetIsSignal;
        private ObservableCollection<SiteCoordinatesViewModel> siteCoords;
        private List<EnergyFlowAreaCoordsMappingModel> dEFAreaMappingConfig;
        private Dictionary<string, EnergyFlowAreaCoordsMappingViewModel>.ValueCollection values;
        private ObservableCollection<SiteCoordinatesViewModel> siteCoords1;
        private List<EnergyFlowAreaCoordsMappingModel> dEFAreaMappingConfig1;

        public bool TargetIsSignal 
        {
            get { return _targetIsSignal; }
            set
            {
                _targetIsSignal = value;
                OnPropertyChanged();
            }
        }
        public ICommand TargetSelected { get; set; }
        private void _targetSelected(object obj)
        {
            if (obj is DEFAreaSiteSetupViewModel)
            {
                TargetIsSignal = false;
            }
            else
            {
                TargetIsSignal = true;
            }
        }

    }
}
