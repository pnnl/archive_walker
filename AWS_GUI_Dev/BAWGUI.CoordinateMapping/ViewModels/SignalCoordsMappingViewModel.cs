﻿using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Core.Utilities;
using BAWGUI.SignalManagement.ViewModels;
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
    public class SignalCoordsMappingViewModel : ViewModelBase
    {
        //private SignalManager _signalMgr;
        public SignalManager SignalMgr { get; set; }
        private List<SignalCoordsMappingModel> _models;

        public SignalCoordsMappingViewModel()
        {
            _models = new List<SignalCoordsMappingModel>();
            SetCurrentSelectedSignal = new RelayCommand(_setCurrentSelectedSignal);
            SiteSelected = new RelayCommand(_siteSelected);
            //SelectedTextboxIndex = -1;
            ModifySiteSelection = new RelayCommand(_modifySiteSelection);
            //DeleteASite = new RelayCommand(_deleteASite);
        }
        public SignalCoordsMappingViewModel(ObservableCollection<SiteCoordinatesViewModel> siteCoords, SignalManager signalMgr) : this()
        {
            this.AvailableSites = siteCoords;
            SignalMgr = signalMgr;
        }
        //renamed sites
        //sites not found
        public SignalCoordsMappingViewModel(ObservableCollection<SiteCoordinatesViewModel> siteCoords, SignalManager signalMgr, List<SignalCoordsMappingModel> models) : this(siteCoords, signalMgr)
        {
            _models = models;
            //SetupSignalMapping();
        }

        public void SetupSignalMapping()
        {
            //sites not found
            var sitesNotFound = new List<string>();
            var sitesRenamed = new Dictionary<string, string>();
            foreach (var md in _models)
            {
                var sgl = SignalMgr.SearchForSignalInTaggedSignals(md.PMUName, md.SignalName);
                if (SignalMgr.UniqueMappingSignals.Contains(sgl))
                {
                    sgl.MapPlotType = md.Type;
                    sgl.Model.Locations.Clear();
                    foreach (var site in md.Locations)
                    {
                        //here, need to check if the site is dummy
                        if (!string.IsNullOrEmpty(site.Name))
                        {
                            var foundSite = _findSite(site.Latitude, site.Longitude);
                            if (foundSite != null)
                            {
                                sgl.Locations.Add(foundSite.Model);
                                if (foundSite.SiteName != site.Name && !sitesRenamed.ContainsKey(site.Name))
                                {
                                    sitesRenamed[site.Name] = foundSite.SiteName;
                                }
                            }
                            else
                            {
                                sgl.Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                                sitesNotFound.Add(site.Name);
                            }
                        }
                        else
                        {
                            sgl.Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                        }
                    }
                    sgl.SelectedLocation = sgl.Locations.FirstOrDefault();
                    sgl.SelectedLocationIndex = 0;
                    sgl.LocationSelectionChanged += Sgl_LocationSelectionChanged;
                }
            }
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

        private void Sgl_LocationSelectionChanged(object sender, EventArgs e)
        {
            CurrentSelectedSignal = sender as SignalSignatureViewModel;
            if (CurrentSelectedSignal != null && CurrentSelectedSignal.SelectedLocation != null)
            {
                foreach (var item in AvailableSites)
                {
                    if (CurrentSelectedSignal.SelectedLocation == item.Model)
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
        public ObservableCollection<SiteCoordinatesViewModel> AvailableSites { get; set; }
        private SignalSignatureViewModel _currentSelectedSignal;
        public SignalSignatureViewModel CurrentSelectedSignal
        {
            get { return _currentSelectedSignal; }
            set
            {
                _currentSelectedSignal = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// This command is invoked when a signal is clicked in the signal site mapping setting's tab
        /// to handle the currently selected signal and display the sites that this signal associates by checking them
        /// </summary>
        public ICommand SetCurrentSelectedSignal { get; set; }
        private void _setCurrentSelectedSignal(object obj)
        {
            //SelectedTextboxIndex = -1;
            //var values = (object[])obj;
            //CurrentSelectedSignal = (SignalSignatureViewModel)values[0];
            CurrentSelectedSignal = (SignalSignatureViewModel)obj;
            foreach (var item in AvailableSites)
            {
                if (CurrentSelectedSignal.Locations.Contains(item.Model))
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
            //if (SelectedTextboxIndex >= CurrentSelectedSignal.Locations.Count())
            //{
            //    SelectedTextboxIndex = -1;
            //}
        }
        public ICommand SiteSelected { get; set; }
        private void _siteSelected(object obj)
        {
            var site = obj as SiteCoordinatesViewModel;
            SiteCoordinatesModel CurrentCheckedItem = null;
            if (CurrentSelectedSignal != null)
            {
                if (CurrentSelectedSignal.MapPlotType == SignalMapPlotType.Dot)
                {
                    if (CurrentSelectedSignal.Locations.Count != 0)
                    {
                        CurrentCheckedItem = CurrentSelectedSignal.Locations[0];
                        CurrentSelectedSignal.Locations.Clear();
                    }
                    if (site.IsSelected)
                    {
                        CurrentSelectedSignal.Locations.Add(site.Model);
                    }
                }
                else //if(CurrentSelectedSignal.MapPlotType == SignalMapPlotType.Line)
                {
                    if (site.IsSelected)
                    {
                        if (CurrentSelectedSignal.SelectedLocation != CoreUtilities.DummySiteCoordinatesModel)
                        {
                            CurrentCheckedItem = CurrentSelectedSignal.SelectedLocation;
                        }
                        CurrentSelectedSignal.Locations[CurrentSelectedSignal.SelectedLocationIndex] = site.Model;
                        CurrentSelectedSignal.SelectedLocation = site.Model;
                        if (CurrentSelectedSignal.Locations.Contains(CurrentCheckedItem))
                        {
                            CurrentCheckedItem = null;
                        }
                    }else
                    {
                        if (CurrentSelectedSignal.SelectedLocation == site.Model)
                        {
                            CurrentCheckedItem = CurrentSelectedSignal.SelectedLocation;
                            CurrentSelectedSignal.Locations[CurrentSelectedSignal.SelectedLocationIndex] = CoreUtilities.DummySiteCoordinatesModel;
                            CurrentSelectedSignal.SelectedLocation = CoreUtilities.DummySiteCoordinatesModel;
                        }
                        if (CurrentSelectedSignal.Locations.Contains(CurrentCheckedItem))
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
                        break;
                    }
                }
            }
        }
        public ICommand ModifySiteSelection { get; set; }
        private void _modifySiteSelection(object obj)
        {
            var values = (object[])obj;
            CurrentSelectedSignal = (SignalSignatureViewModel)values[0];
            CurrentSelectedSignal.SelectedLocation = (SiteCoordinatesModel)values[1];
            CurrentSelectedSignal.SelectedLocationIndex = (int)values[2];
            //_setCurrentSelectedSignal(values[0]);
            //SelectedTextboxIndex = CurrentSelectedSignal.Locations.IndexOf((SiteCoordinatesModel)values[0]);
            foreach (var item in AvailableSites)
            {
                if (CurrentSelectedSignal.SelectedLocation == item.Model)
                //if (CurrentSelectedSignal.Locations[SelectedTextboxIndex] == item.Model)
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
