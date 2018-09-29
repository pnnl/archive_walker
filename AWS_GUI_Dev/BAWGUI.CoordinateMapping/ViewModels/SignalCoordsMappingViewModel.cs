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

        public SignalCoordsMappingViewModel(ObservableCollection<SiteCoordinatesViewModel> siteCoords, SignalManager signalMgr)
        {
            this.AvailableSites = siteCoords;
            SignalMgr = signalMgr;
            SetCurrentSelectedSignal = new RelayCommand(_setCurrentSelectedSignal);
            SiteSelected = new RelayCommand(_siteSelected);
            AddFirstSite = new RelayCommand(_add1stSite);
            AddSecondSite = new RelayCommand(_add2ndSite);
            SelectedTextboxIndex = -1;
            ModifySiteSelection = new RelayCommand(_modifySiteSelection);
        }
        public ObservableCollection<SiteCoordinatesViewModel> AvailableSites { get; set; }

        public SignalSignatureViewModel CurrentSelectedSignal{ get; set; }
        /// <summary>
        /// This command is invoked when a signal is clicked in the signal site mapping setting's tab
        /// to handle the currently selected signal and display the sites that this signal associates by checking them
        /// </summary>
        public ICommand SetCurrentSelectedSignal { get; set; }
        private void _setCurrentSelectedSignal(object obj)
        {
            SelectedTextboxIndex = -1;
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
            //SiteCoordinatesModel checkedItem = null;
            //if ((string)values[1] == "From")
            //{
            //    IsFromSelected = true;
            //    if (CurrentSelectedSignal.From != null)
            //    {
            //        checkedItem = CurrentSelectedSignal.From;
            //    }
            //}
            //else
            //{
            //    IsFromSelected = false;
            //    if (CurrentSelectedSignal.To != null)
            //    {
            //        checkedItem = CurrentSelectedSignal.To;
            //    }
            //}
            //foreach (var item in AvailableSites)
            //{
            //    if (checkedItem != null && item.Model == checkedItem)
            //    {
            //        item.IsSelected = true;
            //    }
            //    else
            //    {
            //        item.IsSelected = false;
            //    }
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
                else if (CurrentSelectedSignal.MapPlotType == SignalMapPlotType.Line)
                {
                    if (site.IsSelected)
                    {
                        if (SelectedTextboxIndex == -1)
                        {
                            SelectedTextboxIndex = CurrentSelectedSignal.Locations.IndexOf(CoreUtilities.DummySiteCoordinatesModel);
                        }
                        if (SelectedTextboxIndex == -1)
                        {
                            CurrentSelectedSignal.Locations.Add(site.Model);
                        }
                        else
                        {
                            if (CurrentSelectedSignal.Locations[SelectedTextboxIndex] != CoreUtilities.DummySiteCoordinatesModel)
                            {
                                CurrentCheckedItem = CurrentSelectedSignal.Locations[SelectedTextboxIndex];
                            }
                            CurrentSelectedSignal.Locations[SelectedTextboxIndex] = site.Model;
                        }
                        if (CurrentSelectedSignal.Locations.Contains(CurrentCheckedItem))
                        {
                            CurrentCheckedItem = null;
                        }
                    }else
                    {
                        if (SelectedTextboxIndex == -1 || CurrentSelectedSignal.Locations[SelectedTextboxIndex] != site.Model)
                        {
                            SelectedTextboxIndex = CurrentSelectedSignal.Locations.IndexOf(site.Model);
                        }
                        if (SelectedTextboxIndex != -1)
                        {
                            CurrentCheckedItem = CurrentSelectedSignal.Locations[SelectedTextboxIndex];
                            CurrentSelectedSignal.Locations[SelectedTextboxIndex] = CoreUtilities.DummySiteCoordinatesModel;
                        }
                        if (CurrentSelectedSignal.Locations.Contains(CurrentCheckedItem))
                        {
                            CurrentCheckedItem = null;
                        }
                    }
                    //if (CurrentSelectedSignal.Locations.Count == 0)
                    //{
                    //    //if (SelectedTextboxIndex == 0)
                    //    //{
                    //    //    CurrentSelectedSignal.Locations.Add(site.Model);
                    //    //}
                    //    if (SelectedTextboxIndex == 1)
                    //    {
                    //        CurrentSelectedSignal.Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                    //        CurrentSelectedSignal.Locations.Add(site.Model);
                    //    }
                    //    else
                    //    {
                    //        CurrentSelectedSignal.Locations.Add(site.Model);
                    //    }
                    //}
                    //else if (CurrentSelectedSignal.Locations.Count == 1 && SelectedTextboxIndex == 1)
                    //{
                    //    CurrentSelectedSignal.Locations.Add(site.Model);
                    //}
                    //else
                    //{

                    //}
                }
                else
                {

                }
            }
            else
            {
                site.IsSelected = false;
                MessageBox.Show("Please select a signal first.", "Warning", MessageBoxButtons.OK);
            }
            //SiteCoordinatesModel CurrentCheckedItem = null;
            //if (IsFromSelected)
            //{
            //    CurrentCheckedItem = CurrentSelectedSignal.From;
            //    if (site.IsSelected)
            //    {
            //        CurrentSelectedSignal.From = site.Model;
            //    }
            //    else
            //    {
            //        CurrentSelectedSignal.From = CoreUtilities.DummySiteCoordinatesModel;
            //    }
            //}
            //else
            //{
            //    CurrentCheckedItem = CurrentSelectedSignal.To;
            //    if (site.IsSelected)
            //    {
            //        CurrentSelectedSignal.To = site.Model;
            //    }
            //    else
            //    {
            //        CurrentSelectedSignal.To = CoreUtilities.DummySiteCoordinatesModel;
            //    }
            //}
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
        //public bool IsFromSelected { get; set; }
        public int SelectedTextboxIndex { get; set; }
        public ICommand AddFirstSite { get; set; }
        private void _add1stSite(object obj)
        {
            _setCurrentSelectedSignal(obj);
            SelectedTextboxIndex = 0;
        }
        public ICommand AddSecondSite { get; set; }
        private void _add2ndSite(object obj)
        {
            _setCurrentSelectedSignal(obj);
            SelectedTextboxIndex = 1;
        }
        public ICommand ModifySiteSelection { get; set; }
        private void _modifySiteSelection(object obj)
        {
            var values = (object[])obj;
            CurrentSelectedSignal = (SignalSignatureViewModel)values[0];
            _setCurrentSelectedSignal(values[0]);
            //SelectedTextboxIndex = CurrentSelectedSignal.Locations.IndexOf((SiteCoordinatesModel)values[0]);
            SelectedTextboxIndex = (int)values[1];
        }
        public ICommand DeleteASite { get; set; }
        public ICommand AddASite { get; set; }
    }
}
