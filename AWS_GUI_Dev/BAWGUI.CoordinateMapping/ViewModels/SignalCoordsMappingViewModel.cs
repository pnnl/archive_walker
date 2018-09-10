using BAWGUI.Core;
using BAWGUI.Core.Utilities;
using BAWGUI.SignalManagement.ViewModels;
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
        }
        public ObservableCollection<SiteCoordinatesViewModel> AvailableSites { get; set; }

        public SignalSignatureViewModel CurrentSelectedSignal{ get; set; }

        public ICommand SetCurrentSelectedSignal { get; set; }
        private void _setCurrentSelectedSignal(object obj)
        {
            var values = (object[])obj;
            CurrentSelectedSignal = (SignalSignatureViewModel)values[0];
            SiteCoordinatesModel checkedItem = null;
            if ((string)values[1] == "From")
            {
                IsFromSelected = true;
                if (CurrentSelectedSignal.From != null)
                {
                    checkedItem = CurrentSelectedSignal.From;
                }
            }
            else
            {
                IsFromSelected = false;
                if (CurrentSelectedSignal.To != null)
                {
                    checkedItem = CurrentSelectedSignal.To;
                }
            }
            foreach (var item in AvailableSites)
            {
                if (checkedItem != null && item.Model == checkedItem)
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
            //if (site.IsSelected)
            //{
            if (IsFromSelected)
            {
                CurrentCheckedItem = CurrentSelectedSignal.From;
                if (site.IsSelected)
                {
                    CurrentSelectedSignal.From = site.Model;
                }
                else
                {
                    CurrentSelectedSignal.From = CoreUtilities.DummySiteCoordinatesModel;
                }
            }
            else
            {
                CurrentCheckedItem = CurrentSelectedSignal.To;
                if (site.IsSelected)
                {
                    CurrentSelectedSignal.To = site.Model;
                }
                else
                {
                    CurrentSelectedSignal.To = CoreUtilities.DummySiteCoordinatesModel;
                }
            }
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
        public bool IsFromSelected { get; set; }
    }
}
