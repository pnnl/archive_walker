﻿using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Core.Utilities;
using BAWGUI.Utilities;
using BAWGUI.Xml;
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
    public class EnergyFlowAreaCoordsMappingViewModel : ViewModelBase
    {
        private EnergyFlowAreaCoordsMappingModel _model;

        public EnergyFlowAreaCoordsMappingViewModel()
        {
            _model = new EnergyFlowAreaCoordsMappingModel();
            _locations = new ObservableCollection<SiteCoordinatesModel>();
            _locations.Add(CoreUtilities.DummySiteCoordinatesModel);
            DeleteASite = new RelayCommand(_deleteASite);
            AddASite = new RelayCommand(_addASite);
            SelectedLocationChanged = new RelayCommand(_changeSelectedLocation);
            _selectedLocation = Locations.FirstOrDefault();
        }
        public EnergyFlowAreaCoordsMappingViewModel(EnergyFlowAreaCoordsMappingModel area) : this()
        {
            _model = area;
            //_locations = new ObservableCollection<SiteCoordinatesModel>(_model.Locations);
        }
        public EnergyFlowAreaCoordsMappingViewModel(string areaName) : this()
        {
            AreaName = areaName;
        }
        public string AreaName 
        {
            get { return _model.AreaName; }
            set
            {
                try
                {
                    _model.AreaName = value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                OnPropertyChanged();
            }
        }
        public SignalMapPlotType Type 
        {
            get { return _model.Type; }
            set
            {
                if (_model.Type != value)
                {
                    _model.Type = value;
                    if (value == SignalMapPlotType.Line)
                    {
                        for (int index = Locations.Count; index < 2; index++)
                        {
                            Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                        }
                    }
                    if (value == SignalMapPlotType.Area)
                    {
                        for (int index = Locations.Count; index < 2; index++)
                        {
                            Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                        }
                    }
                    if (value == SignalMapPlotType.Dot)
                    {
                        if (Locations.Count == 0)
                        {
                            Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                        }
                        else
                        {
                            var keep = Locations[0];
                            Locations.Clear();
                            Locations.Add(keep);
                        }
                    }
                    SelectedLocation = Locations.FirstOrDefault();
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<SiteCoordinatesModel> _locations;
        public ObservableCollection<SiteCoordinatesModel> Locations 
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }
        public ICommand DeleteASite { get; set; }
        private void _deleteASite(object obj)
        {
            var values = (object[])obj;
            var currentLocation = (SiteCoordinatesModel)values[0];
            var selectedTextboxIndex = (int)values[1];
            Locations.RemoveAt(selectedTextboxIndex);
            SelectedLocation = Locations.LastOrDefault();
            //foreach (var item in Locations)
            //{

            //}
        }
        public ICommand AddASite { get; set; }
        private void _addASite(object obj)
        {
            Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
            SelectedLocation = Locations.LastOrDefault();
            //OnLocationSelectionChanged(EventArgs.Empty);
        }
        private SiteCoordinatesModel _selectedLocation;
        public SiteCoordinatesModel SelectedLocation 
        {
            get { return _selectedLocation; }
            set
            {
                _selectedLocation = value;
                OnPropertyChanged();
                OnLocationSelectionChanged(EventArgs.Empty);
            }
        }
        public ICommand SelectedLocationChanged { get; set; }
        private void _changeSelectedLocation(object obj)
        {
            SelectedLocation = (SiteCoordinatesModel)obj;
            //OnLocationSelectionChanged(EventArgs.Empty);
        }
        public event EventHandler LocationSelectionChanged;
        protected virtual void OnLocationSelectionChanged(EventArgs e)
        {
            LocationSelectionChanged?.Invoke(this, e);
        }
        public event EventHandler DEFAreaSelected;
        protected virtual void OnDEFAreaSelected(EventArgs e)
        {
            DEFAreaSelected?.Invoke(this, e);
        }
    }
}
