using BAWGUI.Core.Models;
using BAWGUI.Core.Utilities;
using BAWGUI.Utilities;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace BAWGUI.Core
{
    public class SignalSignatureViewModel : ViewModelBase
    {
        // Implements IDisposable
        private SignalSignatures _model;
        public SignalSignatures Model
        {
            get { return _model; }
        }
        public SignalSignatureViewModel()
        {
            _model = new SignalSignatures();
            // _model.IsEnabled = True
            // _model.IsValid = True
            // _model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0;
            _model.PassedThroughProcessor = 0;
            // _model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1;
            _model.Unit = "O";
            DeleteASite = new RelayCommand(_deleteASite);
            AddASite = new RelayCommand(_addASite);
            _selectedLocation = Locations.FirstOrDefault();
            SelectedLocationIndex = 0;
        }
        public SignalSignatureViewModel(string name) : this()
        {
            _model.SignalName = name;
        }
        public SignalSignatureViewModel(string name, string pmu) : this(name)
        {
            _model.PMUName = pmu;
        }
        public SignalSignatureViewModel(string name, string pmu, string type) : this (name, pmu)
        {
            _model.TypeAbbreviation = type;
        }

        public bool? IsValid
        {
            get
            {
                return _model.IsValid;
            }
            set
            {
                _model.IsValid = value;
                OnPropertyChanged();
            }
        }
        public string PMUName
        {
            get
            {
                return _model.PMUName;
            }
            set
            {
                _model.PMUName = value;
                OnPropertyChanged();
            }
        }
        public string SignalName
        {
            get
            {
                return _model.SignalName;
            }
            set
            {
                _model.SignalName = value;
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(OldSignalName))
                    OldSignalName = value;
                OnPropertyChanged();
            }
        }
        //private string _typeAbbreviation;
        public string TypeAbbreviation
        {
            get
            {
                return _model.TypeAbbreviation;
            }
            set
            {
                _model.TypeAbbreviation = value;
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(OldTypeAbbreviation))
                    OldTypeAbbreviation = value;
                OnPropertyChanged();
            }
        }
        public string Unit
        {
            get
            {
                return _model.Unit;
            }
            set
            {
                _model.Unit = value;
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(OldUnit))
                    OldUnit = value;
                OnPropertyChanged();
            }
        }
        public bool? IsChecked
        {
            get
            {
                return _model.IsChecked;
            }
            set
            {
                _model.IsChecked = value;
                OnPropertyChanged();
            }
        }
        public bool? IsEnabled
        {
            get
            {
                return _model.IsEnabled;
            }
            set
            {
                _model.IsEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool? IsCustomSignal
        {
            get
            {
                return _model.IsCustomSignal;
            }
            set
            {
                _model.IsCustomSignal = value;
                OnPropertyChanged();
            }
        }

        //private int _samplingRate;
        public int SamplingRate
        {
            get
            {
                return _model.SamplingRate;
            }
            set
            {
                _model.SamplingRate = value;
                OnPropertyChanged();
            }
        }

        //private int _passedThroughDQFilter;
        public int PassedThroughDQFilter
        {
            get
            {
                return _model.PassedThroughDQFilter;
            }
            set
            {
                _model.PassedThroughDQFilter = value;
                OnPropertyChanged();
            }
        }

        //private int _passedThroughProcessor;
        public int PassedThroughProcessor
        {
            get
            {
                return _model.PassedThroughProcessor;
            }
            set
            {
                _model.PassedThroughProcessor = value;
                OnPropertyChanged();
            }
        }

        public bool? IsNameTypeUnitChanged
        {
            get
            {
                return _model.IsNameTypeUnitChanged;
            }
            set
            {
                _model.IsNameTypeUnitChanged = value;
                OnPropertyChanged();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            SignalSignatureViewModel p = (SignalSignatureViewModel)obj;
            return this.PMUName == p.PMUName && this.SignalName == p.SignalName && this.TypeAbbreviation == p.TypeAbbreviation && this.Unit == p.Unit && this.OldSignalName == p.OldSignalName && this.OldTypeAbbreviation == p.OldTypeAbbreviation && this.OldUnit == p.OldUnit && this.SamplingRate == p.SamplingRate;
        }
        // Public Overrides Function GetHashCode() As Integer
        // Return MyBase.GetHashCode()
        // End Function
        public static bool operator ==(SignalSignatureViewModel x, SignalSignatureViewModel y)
        {
            if ((object)x == null)
            {
                return (object)y == null;
            }else if((object)y == null)
            {
                return false;
            }
            return x.PMUName == y.PMUName && x.SignalName == y.SignalName && x.TypeAbbreviation == y.TypeAbbreviation && x.Unit == y.Unit && x.OldSignalName == y.OldSignalName && x.OldTypeAbbreviation == y.OldTypeAbbreviation && x.OldUnit == y.OldUnit && x.SamplingRate == y.SamplingRate;
            //if (x is null && y is null)
            //{
            //    return true;
            //}
            //else if (x is null ^ y is null)
            //{
            //    return false;
            //}
            //else
            //{

            //    return x.PMUName == y.PMUName && x.SignalName == y.SignalName && x.TypeAbbreviation == y.TypeAbbreviation && x.Unit == y.Unit && x.OldSignalName == y.OldSignalName && x.OldTypeAbbreviation == y.OldTypeAbbreviation && x.OldUnit == y.OldUnit && x.SamplingRate == y.SamplingRate;
            //}
        }
        public static bool operator !=(SignalSignatureViewModel x, SignalSignatureViewModel y)
        {
            return !(x == y);
            //if ((object)x == null)
            //{
            //    return y != null;
            //}
            //return x.PMUName != y.PMUName || x.SignalName != y.SignalName || x.TypeAbbreviation != y.TypeAbbreviation || x.Unit != y.Unit || x.OldSignalName != y.OldSignalName || x.OldTypeAbbreviation != y.OldTypeAbbreviation || x.OldUnit != y.OldUnit || x.SamplingRate != y.SamplingRate;
            //if (x is null && y is null)
            //{
            //    return false;
            //}else if(x is null ^ y is null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return x.PMUName != y.PMUName || x.SignalName != y.SignalName || x.TypeAbbreviation != y.TypeAbbreviation || x.Unit != y.Unit || x.OldSignalName != y.OldSignalName || x.OldTypeAbbreviation != y.OldTypeAbbreviation || x.OldUnit != y.OldUnit || x.SamplingRate != y.SamplingRate;
            //}
        }

        public bool IsSignalInformationComplete()
        {
            return !string.IsNullOrEmpty(PMUName) && !string.IsNullOrEmpty(SignalName) && !string.IsNullOrEmpty(TypeAbbreviation) && !string.IsNullOrEmpty(Unit) && (SamplingRate > 0);
        }

        //private string _oldSignalName;
        public string OldSignalName
        {
            get
            {
                return _model.OldSignalName;
            }
            set
            {
                _model.OldSignalName = value;
                OnPropertyChanged();
            }
        }

        //private string _oldUnit;
        public string OldUnit
        {
            get
            {
                return _model.OldUnit;
            }
            set
            {
                _model.OldUnit = value;
                OnPropertyChanged();
            }
        }

        //private string _oldTypeAbbreviation;
        public string OldTypeAbbreviation
        {
            get
            {
                return _model.OldTypeAbbreviation;
            }
            set
            {
                _model.OldTypeAbbreviation = value;
                OnPropertyChanged();
            }
        }
        //private List<double> _data;
        public List<double> Data
        {
            get { return _model.Data; }
            set
            {
                _model.Data = value;
                OnPropertyChanged();
            }
        }
        //private List<double> _timeStampNumber;
        public List<double> TimeStampNumber
        {
            get { return _model.TimeStampNumber; }
            set
            {
                _model.TimeStampNumber = value;
                OnPropertyChanged();
            }
        }
        //private List<double> _matlabTimeStampNumber;
        public List<double> MATLABTimeStampNumber
        {
            get { return _model.MATLABTimeStampNumber; }
            set
            {
                _model.MATLABTimeStampNumber = value;
                OnPropertyChanged();
            }
        }
        public SiteCoordinatesModel From
        {
            get { return _model.From; }
            set
            {
                _model.From = value;
                OnPropertyChanged();
            }
        }
        public SiteCoordinatesModel To
        {
            get { return _model.To; }
            set
            {
                _model.To = value;
                OnPropertyChanged();
            }
        }
        public SignalMapPlotType MapPlotType
        {
            get { return _model.MapPlotType; }
            set
            {
                if (_model.MapPlotType != value)
                {
                    _model.MapPlotType = value;
                    if (value == SignalMapPlotType.Line)
                    {
                        for (int index = Locations.Count; index < 2; index++)
                        {
                            Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
                        }
                    }
                    if (value == SignalMapPlotType.Area)
                    {
                        for (int index = Locations.Count; index < 3; index++)
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
                    SelectedLocation = Locations.LastOrDefault();
                    SelectedLocationIndex = Locations.Count - 1;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<SiteCoordinatesModel> Locations
        {
            get { return _model.Locations; }
            set
            {
                _model.Locations = value;
                SelectedLocation = _model.Locations.FirstOrDefault();
                SelectedLocationIndex = 0;
                OnPropertyChanged();
            }
        }
        public ICommand DeleteASite { get; set; }
        private void _deleteASite(object obj)
        {
            var values = (object[])obj;
            var currentLocation = (SiteCoordinatesModel)values[0];
            SelectedLocationIndex = (int)values[1];
            _model.Locations.RemoveAt(SelectedLocationIndex);
            if (SelectedLocationIndex >= Locations.Count)
            {
                SelectedLocationIndex = Locations.Count - 1;
            }
            SelectedLocation = Locations[SelectedLocationIndex];
        }
        public ICommand AddASite { get; set; }
        private void _addASite(object obj)
        {
            _model.Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
            SelectedLocation = Locations.LastOrDefault();
            SelectedLocationIndex = Locations.Count - 1;
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
        public int SelectedLocationIndex { get; set; }
        public event EventHandler LocationSelectionChanged;
        protected virtual void OnLocationSelectionChanged(EventArgs e)
        {
            LocationSelectionChanged?.Invoke(this, e);
        }
    }
}
