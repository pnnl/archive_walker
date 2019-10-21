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
            ModifySiteSelection = new RelayCommand(_modifySiteSelection);
        }
        private ObservableCollection<EnergyFlowAreaCoordsMappingViewModel> _areas;
        public ObservableCollection<EnergyFlowAreaCoordsMappingViewModel> Areas 
        {
            get { return _areas; }
            set
            {
                _areas = value;
                foreach (var item in _areas)
                {
                    item.LocationSelectionChanged += Item_LocationSelectionChanged;
                    item.DEFAreaSelected += Item_DEFAreaSelected;
                }
                OnPropertyChanged();
            }
        }

        private void Item_DEFAreaSelected(object sender, EventArgs e)
        {
            //var thisArea = sender as EnergyFlowAreaCoordsMappingViewModel;
            CurrentSelectedArea = sender as EnergyFlowAreaCoordsMappingViewModel;
        }

        private void Item_LocationSelectionChanged(object sender, EventArgs e)
        {
            CurrentSelectedArea = sender as EnergyFlowAreaCoordsMappingViewModel;
            if (CurrentSelectedArea != null && CurrentSelectedArea.SelectedLocation != null)
            {
                foreach (var item in AvailableSites)
                {
                    if (CurrentSelectedArea.SelectedLocation == item.Model)
                    {
                        item.IsSelected = true;
                    }
                    else
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }

        public ObservableCollection<SiteCoordinatesViewModel> AvailableSites { get; set; }
        public EnergyFlowAreaCoordsMappingViewModel CurrentSelectedArea { get; set; }
        public ICommand SetCurrentSelectedArea { get; set; }
        private void _setCurrentSelectedArea(object obj)
        {
            CurrentSelectedArea = (EnergyFlowAreaCoordsMappingViewModel)obj;
            if (CurrentSelectedArea.SelectedLocation == null)
            {
                CurrentSelectedArea.SelectedLocation = CurrentSelectedArea.Locations.FirstOrDefault();
            }
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
            if (SelectedTextboxIndex >= CurrentSelectedArea.Locations.Count())
            {
                SelectedTextboxIndex = -1;
            }
        }
        public int SelectedTextboxIndex { get; set; }
        public ICommand SiteSelected { get; set; }
        private void _siteSelected(object obj)
        {
            var site = obj as SiteCoordinatesViewModel;
            SiteCoordinatesModel CurrentCheckedItem = null;
            if (CurrentSelectedArea != null && CurrentSelectedArea.SelectedLocation != null)
            {
                if (site.IsSelected)
                {
                    if (CurrentSelectedArea.SelectedLocation != CoreUtilities.DummySiteCoordinatesModel)
                    {
                        CurrentCheckedItem = CurrentSelectedArea.SelectedLocation;
                    }
                    var idx = CurrentSelectedArea.Locations.IndexOf(CurrentSelectedArea.SelectedLocation);
                    CurrentSelectedArea.Locations[idx] = site.Model;
                    CurrentSelectedArea.SelectedLocation = site.Model;
                    if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem))
                    {
                        CurrentCheckedItem = null;
                    }
                }
                else
                {
                    if (CurrentSelectedArea.SelectedLocation != site.Model)
                    {
                        CurrentSelectedArea.SelectedLocation = site.Model;
                        var idx = CurrentSelectedArea.Locations.IndexOf(site.Model);
                        CurrentCheckedItem = CurrentSelectedArea.SelectedLocation;
                        CurrentSelectedArea.Locations[idx] = CoreUtilities.DummySiteCoordinatesModel;
                    }
                    if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem))
                    {
                        CurrentCheckedItem = null;
                    }
                }                
            }
            else
            {
                site.IsSelected = false;
                MessageBox.Show("Please select an area first.", "Warning", MessageBoxButtons.OK);
            }
            if (CurrentCheckedItem != null)
            {
                foreach (var item in AvailableSites)
                {
                    if (item.Model == CurrentCheckedItem)
                    {
                        item.IsSelected = false;
                        break;
                    }
                }
            }
        }

        //private void _siteSelected(object obj)
        //{
        //    var site = obj as SiteCoordinatesViewModel;
        //    SiteCoordinatesModel CurrentCheckedItem = null;
        //    if (CurrentSelectedArea != null)
        //    {
        //        if (CurrentSelectedArea.Type == SignalMapPlotType.Dot)
        //        {
        //            if (CurrentSelectedArea.Locations.Count != 0)
        //            {
        //                CurrentCheckedItem = CurrentSelectedArea.Locations[0];
        //                CurrentSelectedArea.Locations.Clear();
        //            }
        //            if (site.IsSelected)
        //            {
        //                CurrentSelectedArea.Locations.Add(site.Model);
        //            }
        //        }
        //        else //if(CurrentSelectedSignal.MapPlotType == SignalMapPlotType.Line)
        //        {
        //            if (site.IsSelected)
        //            {
        //                if (SelectedTextboxIndex == -1)
        //                {
        //                    SelectedTextboxIndex = CurrentSelectedArea.Locations.IndexOf(CoreUtilities.DummySiteCoordinatesModel);
        //                }
        //                if (SelectedTextboxIndex == -1)
        //                {
        //                    CurrentSelectedArea.Locations.Add(site.Model);
        //                }
        //                else
        //                {
        //                    if (CurrentSelectedArea.Locations[SelectedTextboxIndex] != CoreUtilities.DummySiteCoordinatesModel)
        //                    {
        //                        CurrentCheckedItem = CurrentSelectedArea.Locations[SelectedTextboxIndex];
        //                    }
        //                    CurrentSelectedArea.Locations[SelectedTextboxIndex] = site.Model;
        //                }
        //                if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem))
        //                {
        //                    CurrentCheckedItem = null;
        //                }
        //            }
        //            else
        //            {
        //                if (SelectedTextboxIndex == -1 || CurrentSelectedArea.Locations[SelectedTextboxIndex] != site.Model)
        //                {
        //                    SelectedTextboxIndex = CurrentSelectedArea.Locations.IndexOf(site.Model);
        //                }
        //                if (SelectedTextboxIndex != -1)
        //                {
        //                    CurrentCheckedItem = CurrentSelectedArea.Locations[SelectedTextboxIndex];
        //                    CurrentSelectedArea.Locations[SelectedTextboxIndex] = CoreUtilities.DummySiteCoordinatesModel;
        //                }
        //                if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem))
        //                {
        //                    CurrentCheckedItem = null;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        site.IsSelected = false;
        //        MessageBox.Show("Please select an area first.", "Warning", MessageBoxButtons.OK);
        //    }
        //    if (CurrentCheckedItem != null)
        //    {
        //        foreach (var item in AvailableSites)
        //        {
        //            if (item.Model == CurrentCheckedItem)
        //            {
        //                item.IsSelected = false;
        //            }
        //        }
        //    }
        //}
        public ICommand ModifySiteSelection { get; set; }
        private void _modifySiteSelection(object obj)
        {
            var values = (object[])obj;
            CurrentSelectedArea = (EnergyFlowAreaCoordsMappingViewModel)values[0];
            var currentLocation = (SiteCoordinatesModel)values[1];
            SelectedTextboxIndex = (int)values[2];
            foreach (var item in AvailableSites)
            {
                if (currentLocation == item.Model)
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
        }

    }
}
