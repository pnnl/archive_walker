using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core;
using BAWGUI.Utilities;
using MapService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.CoordinateMapping.ViewModels
{
    public class SiteCoordinatesViewModel : ViewModelBase
    {
        private SiteCoordinatesModel _model;
        public SiteCoordinatesModel Model { get { return _model; } }
        public SiteCoordinatesViewModel(SiteCoordinatesModel md)
        {
            _model = md;
        }

        public SiteCoordinatesViewModel()
        {
            _model = new SiteCoordinatesModel();
        }
        public int InternalID => _model.GetInternalID();

        public string SiteName
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnSitePropertyChanged();
                OnPropertyChanged();
            }
        }
        public string Latitude
        {
            get { return _model.Latitude; }
            set
            {
                _model.Latitude = value;
                OnSitePropertyChanged();
                OnPropertyChanged();
            }
        }
        public string Longitude
        {
            get { return _model.Longitude; }
            set
            {
                _model.Longitude = value;
                OnSitePropertyChanged();
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// to decide if show on the map when the site coordinates are set up 
        /// </summary>
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnCheckStatusChanged();
                OnPropertyChanged();
            }
        }
        public event EventHandler CheckStatusChanged;
        protected virtual void OnCheckStatusChanged()
        {
            CheckStatusChanged?.Invoke(this, EventArgs.Empty);
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler SitePropertyChanged;
        protected virtual void OnSitePropertyChanged()
        {
            SitePropertyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
