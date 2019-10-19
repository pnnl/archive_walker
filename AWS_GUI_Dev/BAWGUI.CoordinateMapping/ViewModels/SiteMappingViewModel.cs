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
            AvailableSites = new ObservableCollection<SiteCoordinatesViewModel>();
            TargetSelected = new RelayCommand(_targetSelected);
        }
        public SiteMappingViewModel(ObservableCollection<SiteCoordinatesViewModel> siteCoords) : this()
        {
            AvailableSites = siteCoords;
        }

        public DEFAreaSiteSetupViewModel DEFAreaSiteMappingVM { get; set; }
        public SignalCoordsMappingViewModel SignalCoordsMappingVM { get; set; }
        //private ObservableCollection<SiteCoordinatesViewModel> _availableSites;
        public ObservableCollection<SiteCoordinatesViewModel> AvailableSites { get; set; }
        public bool TargetIsSignal { get; set; }
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
