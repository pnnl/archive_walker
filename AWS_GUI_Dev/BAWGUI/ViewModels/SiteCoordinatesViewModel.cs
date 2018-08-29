using BAWGUI.Models;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.ViewModels
{
    public class SiteCoordinatesViewModel : ViewModelBase
    {
        private PMUCoordinates _model;
        public SiteCoordinatesViewModel(PMUCoordinates md)
        {
            _model = md;
        }

        public SiteCoordinatesViewModel()
        {
            _model = new PMUCoordinates();
        }

        public string SiteName
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }
        public string Latitude
        {
            get { return _model.Latitude; }
            set
            {
                _model.Latitude = value;
                OnPropertyChanged();
            }
        }
        public string Longitude
        {
            get { return _model.Longitude; }
            set
            {
                _model.Longitude = value;
                OnPropertyChanged();
            }
        }
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }
    }
}
