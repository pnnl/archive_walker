using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Core.Utilities;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace BAWGUI.CoordinateMapping.ViewModels
{
    public class DEFAreaSiteSetupViewModel : ViewModelBase
    {
        public DEFAreaSiteSetupViewModel()
        {
            _areas = new ObservableCollection<EnergyFlowAreaCoordsMappingViewModel>();
            SetCurrentSelectedArea = new RelayCommand(_setCurrentSelectedArea);
            SiteSelected = new RelayCommand(_siteSelected);
            SelectedTextboxIndex = -1;
        }
        private ObservableCollection<EnergyFlowAreaCoordsMappingViewModel> _areas;
        public ObservableCollection<EnergyFlowAreaCoordsMappingViewModel> Areas 
        {
            get { return _areas; }
            set
            {
                _areas = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SiteCoordinatesViewModel> AvailableSites { get; set; }
        public EnergyFlowAreaCoordsMappingViewModel CurrentSelectedArea { get; set; }
        public ICommand SetCurrentSelectedArea { get; set; }
        private void _setCurrentSelectedArea(object obj)
        {
            CurrentSelectedArea = (EnergyFlowAreaCoordsMappingViewModel)obj;
            foreach (var item in AvailableSites)
            {
                if (CurrentSelectedArea.Locations.Contains(item.Model))
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
        }
        public int SelectedTextboxIndex { get; set; }
        public ICommand SiteSelected { get; set; }
        private void _siteSelected(object obj)
        {
            var site = obj as SiteCoordinatesViewModel;
            SiteCoordinatesModel CurrentCheckedItem = null;
            if (CurrentSelectedArea != null)
            {
                if (CurrentSelectedArea.Type == SignalMapPlotType.Dot)
                {
                    if (CurrentSelectedArea.Locations.Count != 0)
                    {
                        CurrentCheckedItem = CurrentSelectedArea.Locations[0];
                        CurrentSelectedArea.Locations.Clear();
                    }
                    if (site.IsSelected)
                    {
                        CurrentSelectedArea.Locations.Add(site.Model);
                    }
                }
                else //if(CurrentSelectedSignal.MapPlotType == SignalMapPlotType.Line)
                {
                    if (site.IsSelected)
                    {
                        if (SelectedTextboxIndex == -1)
                        {
                            SelectedTextboxIndex = CurrentSelectedArea.Locations.IndexOf(CoreUtilities.DummySiteCoordinatesModel);
                        }
                        if (SelectedTextboxIndex == -1)
                        {
                            CurrentSelectedArea.Locations.Add(site.Model);
                        }
                        else
                        {
                            if (CurrentSelectedArea.Locations[SelectedTextboxIndex] != CoreUtilities.DummySiteCoordinatesModel)
                            {
                                CurrentCheckedItem = CurrentSelectedArea.Locations[SelectedTextboxIndex];
                            }
                            CurrentSelectedArea.Locations[SelectedTextboxIndex] = site.Model;
                        }
                        if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem))
                        {
                            CurrentCheckedItem = null;
                        }
                    }
                    else
                    {
                        if (SelectedTextboxIndex == -1 || CurrentSelectedArea.Locations[SelectedTextboxIndex] != site.Model)
                        {
                            SelectedTextboxIndex = CurrentSelectedArea.Locations.IndexOf(site.Model);
                        }
                        if (SelectedTextboxIndex != -1)
                        {
                            CurrentCheckedItem = CurrentSelectedArea.Locations[SelectedTextboxIndex];
                            CurrentSelectedArea.Locations[SelectedTextboxIndex] = CoreUtilities.DummySiteCoordinatesModel;
                        }
                        if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem))
                        {
                            CurrentCheckedItem = null;
                        }
                    }
                }
            }
            else
            {
                site.IsSelected = false;
                MessageBox.Show("Please select a signal first.", "Warning", MessageBoxButtons.OK);
            }
            if (CurrentCheckedItem != null)
            {
                foreach (var item in AvailableSites)
                {
                    if (item.Model == CurrentCheckedItem)
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }

    }
}
