using BAWGUI.Utilities;
using MapService.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core;

namespace BAWGUI.CoordinateMapping.ViewModels
{
    public class CoordinatesTableViewModel : ViewModelBase
    {
        public CoordinatesTableViewModel()
        {
            AddALocation = new RelayCommand(_addALocation);
            _siteCoords = new ObservableCollection<SiteCoordinatesViewModel>();
            DeleteARow = new RelayCommand(_deleteARow);
            SaveCoordinatesFiile = new RelayCommand(_saveCoords);
            SelectAllRow = new RelayCommand(_selectAllRows);
            DeSelectAllRow = new RelayCommand(_deselectAllRows);
            DeleteSelectedRows = new RelayCommand(_deleteSelectedRows);
            LoadCoordinates = new RelayCommand(_openCoordsFile);
            MapVM = new MapViewModel();
            MapVM.LocationSelected += MapVM_LocationSelected;
            _locationCoordinatesFilePath = Properties.Settings.Default.LocationCoordinatesFilePath;
            if (File.Exists(_locationCoordinatesFilePath))
            {
                _readLocationCoordsFile(_locationCoordinatesFilePath);
            }
        }

        private void MapVM_LocationSelected(object sender, PointLatLng e)
        {
            var newLocation = new SiteCoordinatesModel(e.Lat, e.Lng);
            var newLocationVM = new SiteCoordinatesViewModel(newLocation);
            newLocationVM.CheckStatusChanged += _modifyMapAnnotation;
            newLocationVM.SitePropertyChanged += _sitePropertyChangedHandler;
            newLocationVM.IsChecked = true;
            SiteCoords.Add(newLocationVM);
        }

        private ObservableCollection<SiteCoordinatesViewModel> _siteCoords;
        public ObservableCollection<SiteCoordinatesViewModel> SiteCoords
        {
            get { return _siteCoords; }
            set {
                _siteCoords = value;
                OnPropertyChanged();
                OnSiteCoordsDefinitionChanged(EventArgs.Empty);
            }
        }
        public event EventHandler SiteCoordsDefinitionChanged;
        protected virtual void OnSiteCoordsDefinitionChanged(EventArgs e)
        {
            SiteCoordsDefinitionChanged?.Invoke(this, e);
        }
        public ICommand AddALocation { get; set; }
        private void _addALocation(object obj)
        {
            var newLocation = new SiteCoordinatesModel();
            var newLocationVM = new SiteCoordinatesViewModel(newLocation);
            newLocationVM.CheckStatusChanged += _modifyMapAnnotation;
            newLocationVM.SitePropertyChanged += _sitePropertyChangedHandler;
            SiteCoords.Add(newLocationVM);
        }
        public ICommand DeleteARow { get; set; }
        private void _deleteARow(object obj)
        {
            var toBeDeleted = obj as SiteCoordinatesViewModel;
            foreach (var item in SiteCoords)
            {
                if (toBeDeleted == item)
                {
                    SiteCoords.Remove(toBeDeleted);
                    if (toBeDeleted.IsChecked)
                    {
                        toBeDeleted.IsChecked = false;
                    }
                    break;
                }
            }
        }
        public ICommand SaveCoordinatesFiile { get; set; }
        private void _saveCoords(object obj)
        {
            _locationCoordinatesFilePath = (string)obj;
            var filePath = _locationCoordinatesFilePath + "\\SiteCoordinatesConfig.xml";
            try
            {
                SiteCoordinatesWriter writer = new SiteCoordinatesWriter();
                writer.WriteCoordsFile(filePath, SiteCoords);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not write SiteCoordinatesConfig.xml file to disk. Original error: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }
        }

        public ICommand SelectAllRow { get; set; }
        private void _selectAllRows(object obj)
        {
            foreach (var item in SiteCoords)
            {
                item.IsChecked = true;
            }
        }
        public ICommand DeSelectAllRow { get; set; }
        private void _deselectAllRows(object obj)
        {
            foreach (var item in SiteCoords)
            {
                item.IsChecked = false;
            }
        }
        public ICommand DeleteSelectedRows { get; set; }
        private void _deleteSelectedRows(object obj)
        {
            for (int index = SiteCoords.Count - 1; index >= 0; index--)
            {
                if (SiteCoords[index].IsChecked)
                {
                    SiteCoords[index].IsChecked = false;
                    SiteCoords.RemoveAt(index);
                }
            }
        }
        private string _locationCoordinatesFilePath;
        public string LocationCoordinatesFilePath
        {
            get { return _locationCoordinatesFilePath; }
            set
            {
                if (_locationCoordinatesFilePath != value && File.Exists(value))
                {
                    _locationCoordinatesFilePath = value;
                    Properties.Settings.Default.LocationCoordinatesFilePath = value;
                    Properties.Settings.Default.Save();
                    _readLocationCoordsFile(value);
                    OnPropertyChanged();
                }
            }
        }
        public ICommand LoadCoordinates { get; set; }
        private void _openCoordsFile(object obj)
        {
            //Console.WriteLine("The current directory is {0}", Directory.GetCurrentDirectory());
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FileName = "";
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (!string.IsNullOrEmpty(_locationCoordinatesFilePath))
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(_locationCoordinatesFilePath);
            }
            if (!Directory.Exists(openFileDialog.InitialDirectory))
            {
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            }
            openFileDialog.Title = "Please select a location coordinates file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _locationCoordinatesFilePath = openFileDialog.FileName;
                Properties.Settings.Default.LocationCoordinatesFilePath = _locationCoordinatesFilePath;
                Properties.Settings.Default.Save();
                _readLocationCoordsFile(_locationCoordinatesFilePath);
            }
            //Console.WriteLine("The current directory is {0}", Directory.GetCurrentDirectory());
        }

        private void _readLocationCoordsFile(string path)
        {
            try
            {
                var reader = new SiteCoordinatesReader();
                SiteCoords = reader.ReadCoordsFile(path);
                MapVM.Annotations.Clear();
                foreach (var item in SiteCoords)
                {
                    item.CheckStatusChanged += _modifyMapAnnotation;
                    item.SitePropertyChanged += _sitePropertyChangedHandler;
                    item.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private MapViewModel _mapVM;
        public MapViewModel MapVM
        {
            get { return _mapVM; }
            set
            {
                _mapVM = value;
                OnPropertyChanged();
            }
        }
        private void _modifyMapAnnotation (object sender, EventArgs e)
        {
            var coords = sender as SiteCoordinatesViewModel;
            if (coords.IsChecked)
            {
                var lat = Convert.ToDouble(coords.Latitude);
                var lng = Convert.ToDouble(coords.Longitude);
                var location = new PointLatLng(lat, lng);
                MapVM.AddAnnotation(location, coords.InternalID, coords.SiteName);
            }
            else
            {
                MapVM.DeleteAnnotation(coords.InternalID);
            }
            MapVM.ModifyMapAnnotation();
        }

        private void _sitePropertyChangedHandler(object sender, EventArgs e)
        {
            var coords = sender as SiteCoordinatesViewModel;
            if (coords.IsChecked)
            {
                MapVM.DeleteAnnotation(coords.InternalID);
                var lat = Convert.ToDouble(coords.Latitude);
                var lng = Convert.ToDouble(coords.Longitude);
                var location = new PointLatLng(lat, lng);
                MapVM.AddAnnotation(location, coords.InternalID, coords.SiteName);
                MapVM.ModifyMapAnnotation();
            }
        }

        //public static SiteCoordinatesViewModel FindSite(string Lat, string Lng)
        //{
        //    foreach (var site in SiteCoords)
        //    {
        //        if (site.Latitude == Lat && site.Longitude == Lng) //instead of equal, we could give a certain percentage to decide if they mean the same location even if the number are not exact the same.
        //        {
        //            return site;
        //        }
        //    }
        //    return null;
        //}
    }
}
