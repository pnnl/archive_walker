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

        public DEFAreaSiteSetupViewModel DEFAreaSiteMappingVM { get; set; }
        public SignalCoordsMappingViewModel SignalCoordsMappingVM { get; set; }
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
