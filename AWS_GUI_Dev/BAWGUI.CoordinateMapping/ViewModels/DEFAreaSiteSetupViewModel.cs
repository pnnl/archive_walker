using BAWGUI.CoordinateMapping.Models;
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
            //SelectedTextboxIndex = -1;
            //ModifySiteSelection = new RelayCommand(_modifySiteSelection);
            _defAreaMappingConfig = new List<EnergyFlowAreaCoordsMappingModel>();
            AvailableSites = new ObservableCollection<SiteCoordinatesViewModel>();
        }
        public DEFAreaSiteSetupViewModel(ObservableCollection<SiteCoordinatesViewModel> siteCoords, List<EnergyFlowAreaCoordsMappingModel> dEFAreaMappingConfig) : this()
        {
            AvailableSites = siteCoords;
            _defAreaMappingConfig = dEFAreaMappingConfig;
        }
        //public DEFAreaSiteSetupViewModel(ObservableCollection<SiteCoordinatesViewModel> siteCoords, List<EnergyFlowAreaCoordsMappingModel> dEFAreaMappingConfig, List<string> areas) : this(siteCoords, dEFAreaMappingConfig)
        //{
        //    SetupAreaMapping(dEFAreaMappingConfig, areas);
        //}

        public void SetupAreaMapping(List<string> areas)
        {
            var newAreas = new ObservableCollection<EnergyFlowAreaCoordsMappingViewModel>();
            var areaConfigDict = _defAreaMappingConfig.ToDictionary(x => x.AreaName);
            var sitesRenamed = new Dictionary<string, string>();
            var sitesNotFound = new List<string>();
            foreach (var area in areas)
            {
                var newArea = new EnergyFlowAreaCoordsMappingViewModel(area);
                if (areaConfigDict.ContainsKey(area))
                {
                    var thisAreaConfig = areaConfigDict[area];
                    newArea.Type = thisAreaConfig.Type;
                    newArea.Locations.Clear();
                    foreach (var lc in thisAreaConfig.Locations)
                    {
                        if (!string.IsNullOrEmpty(lc.Name))
                        {
                            var foundSite = _findSite(lc.Latitude, lc.Longitude);
                            if (foundSite != null)
                            {
                                newArea.Locations.Add(foundSite.Model);
                                if (foundSite.SiteName != lc.Name && !sitesRenamed.ContainsKey(lc.Name))
                                {
                                    sitesRenamed[lc.Name] = foundSite.SiteName;
                                }
                            }
                            else
                            {
                                newArea.Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                                sitesNotFound.Add(lc.Name);
                            }
                        }
                        else
                        {
                            newArea.Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                        }
                    }
                }
                newAreas.Add(newArea);
            }
            Areas = newAreas;
            var siteNotFoundMessage = "";
            var siteRenamedMessage = "";
            if (sitesNotFound.Count != 0)
            {
                siteNotFoundMessage = string.Format("The following site(s) is(are) not found in the coordinates table:\n {0}", string.Join(",", sitesNotFound));
            }
            if (sitesRenamed.Count != 0)
            {
                var siteRenamedMessages = new List<string>();
                foreach (var item in sitesRenamed)
                {
                    siteRenamedMessages.Add(string.Format("{0} to {1}", item.Key, item.Value));
                }
                siteRenamedMessage = string.Format("\nThe following site(s) is(are) re-named to match the name in the coordinates table:\n{0}", string.Join("\n", siteRenamedMessages));
            }
            if (!string.IsNullOrEmpty(siteNotFoundMessage) || !string.IsNullOrEmpty(siteRenamedMessage))
            {
                MessageBox.Show(siteNotFoundMessage + "\n" + siteRenamedMessage, "Warnings", MessageBoxButtons.OK);
            }
        }

        private SiteCoordinatesViewModel _findSite(string Lat, string Lng)
        {
            foreach (var site in AvailableSites)
            {
                if (site.Latitude == Lat && site.Longitude == Lng) //instead of equal, we could give a certain percentage to decide if they mean the same location even if the number are not exact the same.
                {
                    return site;
                }
            }
            return null;
        }
        private ObservableCollection<SiteCoordinatesViewModel> _siteCoords;
        private List<EnergyFlowAreaCoordsMappingModel> _defAreaMappingConfig;

        private Dictionary<string, EnergyFlowAreaCoordsMappingViewModel> areas;

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
                }
                OnPropertyChanged();
            }
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
        public ICommand SiteSelected { get; set; }
        private void _siteSelected(object obj)
        {
            var site = obj as SiteCoordinatesViewModel;
            SiteCoordinatesModel CurrentCheckedItem = null;
            if (CurrentSelectedArea != null && CurrentSelectedArea.SelectedLocation != null)
            {
                if (site.IsSelected) //when a site is checked
                {
                    if (CurrentSelectedArea.SelectedLocation != CoreUtilities.DummySiteCoordinatesModel) //if current textbox has a site in it
                    {
                        CurrentCheckedItem = CurrentSelectedArea.SelectedLocation; // need to remember this old site so we can uncheck it later
                    }
                    CurrentSelectedArea.Locations[CurrentSelectedArea.SelectedLocationIndex] = site.Model; // assign new site value to this textbos
                    CurrentSelectedArea.SelectedLocation = site.Model; // make selected site of this area point to this newly checked site
                    if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem)) // if in this area there are duplicate site, i.e. two textboxes have the same site, we don't need to uncheck it
                    {
                        CurrentCheckedItem = null;
                    }
                }
                else //when a site is unchecked, means user wants to make this textbox empty
                {
                    if (CurrentSelectedArea.SelectedLocation == site.Model) // in this situation, the unchecked site is usually the same as the selected location/textbox
                    {
                        CurrentCheckedItem = CurrentSelectedArea.SelectedLocation;  // need to remember this old site so we can uncheck it later
                        CurrentSelectedArea.Locations[CurrentSelectedArea.SelectedLocationIndex] = CoreUtilities.DummySiteCoordinatesModel; //set this textbox empty
                        CurrentSelectedArea.SelectedLocation = CoreUtilities.DummySiteCoordinatesModel; // also make the selected location point to the empty site
                    }
                    if (CurrentSelectedArea.Locations.Contains(CurrentCheckedItem))  // if in this area there are duplicate site, i.e. two textboxes have the same site, we don't need to uncheck it
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
            if (CurrentCheckedItem != null) // go through all the available sites and find the old site that need to be unchecked
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
        //public ICommand ModifySiteSelection { get; set; }
        //private void _modifySiteSelection(object obj)
        //{
        //    var values = (object[])obj;
        //    CurrentSelectedArea = (EnergyFlowAreaCoordsMappingViewModel)values[0];
        //    var currentLocation = (SiteCoordinatesModel)values[1];
        //    CurrentSelectedArea.SelectedLocationIndex = (int)values[2];
        //    foreach (var item in AvailableSites)
        //    {
        //        if (currentLocation == item.Model)
        //        {
        //            item.IsSelected = true;
        //        }
        //        else
        //        {
        //            item.IsSelected = false;
        //        }
        //    }
        //}

    }
}
