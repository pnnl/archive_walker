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
        }
        private ObservableCollection<SiteCoordinatesViewModel> _siteCoords;
        public ObservableCollection<SiteCoordinatesViewModel> SiteCoords
        {
            get { return _siteCoords; }
            set {
                _siteCoords = value;
                OnPropertyChanged();
            }
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
        public ICommand LoadCoordinates { get; set; }
        private void _openCoordsFile(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FileName = "";
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (!string.IsNullOrEmpty(_locationCoordinatesFilePath))
            {
                openFileDialog.InitialDirectory = Path.GetFullPath(_locationCoordinatesFilePath);
            }
            if (!Directory.Exists(openFileDialog.InitialDirectory))
            {
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            }
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Please select a location coordinates file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _locationCoordinatesFilePath = openFileDialog.FileName;
                try
                {
                    var reader = new SiteCoordinatesReader();
                    reader.ReadCoordsFile(_locationCoordinatesFilePath, SiteCoords);
                    foreach (var item in SiteCoords)
                    {
                        item.CheckStatusChanged += _modifyMapAnnotation;
                        item.SitePropertyChanged += _sitePropertyChangedHandler;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
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


    }
}
