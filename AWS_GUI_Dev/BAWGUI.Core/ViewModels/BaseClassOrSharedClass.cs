using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using BAWGUI.Utilities;
using BAWGUI.SignalManagement.ViewModels;

namespace BAWGUI.Core
{
    //public class ParameterValuePair : ViewModelBase
    //{
    //    public ParameterValuePair()
    //    {
    //        _isRequired = true;
    //    }
    //    public ParameterValuePair(string para, object value, bool required)
    //    {
    //        _parameterName = para;
    //        _value = value;
    //        _isRequired = required;
    //    }
    //    public ParameterValuePair(string para, object value)
    //    {
    //        _parameterName = para;
    //        _value = value;
    //        _isRequired = true;
    //    }
    //    private string _parameterName;
    //    public string ParameterName
    //    {
    //        get
    //        {
    //            return _parameterName;
    //        }
    //        set
    //        {
    //            _parameterName = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    private object _value;
    //    public object Value
    //    {
    //        get
    //        {
    //            return _value;
    //        }
    //        set
    //        {
    //            _value = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    private bool _isRequired;
    //    public bool IsRequired
    //    {
    //        get
    //        {
    //            return _isRequired;
    //        }
    //        set
    //        {
    //            _isRequired = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    private string _toolTip;
    //    public string ToolTip
    //    {
    //        get
    //        {
    //            return _toolTip;
    //        }
    //        set
    //        {
    //            _toolTip = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}

     //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
     //'''''''''''''''''''''''''''''''Class SignalProcessStep''''''''''''''''''''''''''''''''''''''''
     //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    public abstract class SignalProcessStep : ViewModelBase
    {
        private int _stepCounter;
        public int StepCounter
        {
            get
            {
                return _stepCounter;
            }
            set
            {
                _stepCounter = value;

                OnPropertyChanged();
            }
        }

        private bool _isStepSelected;
        public bool IsStepSelected
        {
            get
            {
                return _isStepSelected;
            }
            set
            {
                _isStepSelected = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SignalSignatureViewModel> _inputChannels;
        public ObservableCollection<SignalSignatureViewModel> InputChannels
        {
            get
            {
                return _inputChannels;
            }
            set
            {
                _inputChannels = value;
                OnPropertyChanged();
            }
        }

        private SignalTypeHierachy _thisStepInputsAsSignalHierachyByType;
        public SignalTypeHierachy ThisStepInputsAsSignalHerachyByType
        {
            get
            {
                return _thisStepInputsAsSignalHierachyByType;
            }
            set
            {
                _thisStepInputsAsSignalHierachyByType = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SignalSignatureViewModel> _outputChannels;
        public ObservableCollection<SignalSignatureViewModel> OutputChannels
        {
            get
            {
                return _outputChannels;
            }
            set
            {
                _outputChannels = value;
                OnPropertyChanged();
            }
        }

        private SignalTypeHierachy _thisStepOutputsAsSignalHierachyByPMU;
        public SignalTypeHierachy ThisStepOutputsAsSignalHierachyByPMU
        {
            get
            {
                return _thisStepOutputsAsSignalHierachyByPMU;
            }
            set
            {
                _thisStepOutputsAsSignalHierachyByPMU = value;
                OnPropertyChanged();
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public abstract bool CheckStepIsComplete();
    }
    public class Filter : SignalProcessStep
    {
        public Filter()
        {
            //_fileterParameters = new ObservableCollection<ParameterValuePair>();
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
            ThisStepInputsAsSignalHerachyByType = new SignalTypeHierachy(new SignalSignatureViewModel());

            OutputChannels = new ObservableCollection<SignalSignatureViewModel>();
            ThisStepOutputsAsSignalHierachyByPMU = new SignalTypeHierachy(new SignalSignatureViewModel());
            IsExpanded = false;
        }

        public override bool CheckStepIsComplete()
        {
            return true;
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        // Private _filterName As String
        // Public Property FilterName As String
        // Get
        // Return _filterName
        // End Get
        // Set(ByVal value As String)
        // _filterName = value
        // OnPropertyChanged()
        // End Set
        // End Property

        //private ObservableCollection<ParameterValuePair> _fileterParameters;
        //public ObservableCollection<ParameterValuePair> FilterParameters
        //{
        //    get
        //    {
        //        return _fileterParameters;
        //    }
        //    set
        //    {
        //        _fileterParameters = value;
        //        OnPropertyChanged();
        //    }
        //}
    }

    public abstract class DetectorBase : ViewModelBase
    {
        public abstract string Name { get; }

        private bool _isStepSelected;
        public bool IsStepSelected
        {
            get
            {
                return _isStepSelected;
            }
            set
            {
                _isStepSelected = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SignalSignatureViewModel> _inputChannels;
        public ObservableCollection<SignalSignatureViewModel> InputChannels
        {
            get
            {
                return _inputChannels;
            }
            set
            {
                _inputChannels = value;
                OnPropertyChanged();
            }
        }
        private SignalTypeHierachy _thisStepInputsAsSignalHierachyByType;
        public SignalTypeHierachy ThisStepInputsAsSignalHerachyByType
        {
            get
            {
                return _thisStepInputsAsSignalHierachyByType;
            }
            set
            {
                _thisStepInputsAsSignalHierachyByType = value;
                OnPropertyChanged();
            }
        }
        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
    }

    public abstract class AlarmingDetectorBase : ViewModelBase
    {
        public abstract string Name { get; }

        private bool _isStepSelected;
        public bool IsStepSelected
        {
            get
            {
                return _isStepSelected;
            }
            set
            {
                _isStepSelected = value;
                OnPropertyChanged();
            }
        }
        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
    }
}
