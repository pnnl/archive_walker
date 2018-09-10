using BAWGUI.Utilities;

namespace BAWGUI.Core
{
    public class SignalSignatureViewModel : ViewModelBase
    {
        // Implements IDisposable
        private SignalSignatures _model;
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
        }
        public SignalSignatureViewModel(string name)
        {
            _model = new SignalSignatures();
            _model.SignalName = name;
            // _model.IsEnabled = True
            // _model.IsValid = True
            // _model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0;
            _model.PassedThroughProcessor = 0;
            // _model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1;
            _model.Unit = "O";
        }
        public SignalSignatureViewModel(string name, string pmu)
        {
            _model = new SignalSignatures();
            _model.SignalName = name;
            _model.PMUName = pmu;
            // _model.IsEnabled = True
            // _model.IsValid = True
            // _model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0;
            _model.PassedThroughProcessor = 0;
            // _model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1;
            _model.Unit = "O";
        }
        public SignalSignatureViewModel(string name, string pmu, string type)
        {
            _model = new SignalSignatures();
            _model.SignalName = name;
            _model.PMUName = pmu;
            _model.TypeAbbreviation = type;
            // _model.IsEnabled = True
            // _model.IsValid = True
            // _model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0;
            _model.PassedThroughProcessor = 0;
            // _model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1;
            _model.Unit = "O";
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
        private string _typeAbbreviation;
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

        private int _samplingRate;
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

        private int _passedThroughDQFilter;
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

        private int _passedThroughProcessor;
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
            return x.PMUName == y.PMUName && x.SignalName == y.SignalName && x.TypeAbbreviation == y.TypeAbbreviation && x.Unit == y.Unit && x.OldSignalName == y.OldSignalName && x.OldTypeAbbreviation == y.OldTypeAbbreviation && x.OldUnit == y.OldUnit && x.SamplingRate == y.SamplingRate;
        }
        public static bool operator !=(SignalSignatureViewModel x, SignalSignatureViewModel y)
        {
            return x.PMUName != y.PMUName || x.SignalName != y.SignalName || x.TypeAbbreviation != y.TypeAbbreviation || x.Unit != y.Unit || x.OldSignalName != y.OldSignalName || x.OldTypeAbbreviation != y.OldTypeAbbreviation || x.OldUnit != y.OldUnit || x.SamplingRate != y.SamplingRate;
        }

        public bool IsSignalInformationComplete()
        {
            return !string.IsNullOrEmpty(PMUName) && !string.IsNullOrEmpty(SignalName) && !string.IsNullOrEmpty(TypeAbbreviation) && !string.IsNullOrEmpty(Unit) && (SamplingRate > 0);
        }

        private string _oldSignalName;
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

        private string _oldUnit;
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

        private string _oldTypeAbbreviation;
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
    }
}
