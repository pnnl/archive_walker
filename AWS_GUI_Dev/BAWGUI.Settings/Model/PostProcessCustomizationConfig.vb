Imports System.Collections.ObjectModel
Imports BAWGUI.Core
Imports BAWGUI.Settings.ViewModels

Namespace Model
    Public Class PostProcessCustomizationConfig
        Inherits ViewModelBase
        Public Sub New()
            _customizationNameDictionary = New Dictionary(Of String, String) From {{"Scalar Repetition", "ScalarRep"},
                                                                            {"Addition", "Addition"},
                                                                            {"Subtraction", "Subtraction"},
                                                                            {"Multiplication", "Multiplication"},
                                                                            {"Division", "Division"},
                                                                            {"Exponential", "Exponent"},
                                                                            {"Sign Reversal", "SignReversal"},
                                                                            {"Absolute Value", "AbsVal"},
                                                                            {"Real Component", "RealComponent"},
                                                                            {"Imaginary Component", "ImagComponent"},
                                                                            {"Angle Calculation", "Angle"},
                                                                            {"Complex Conjugate", "ComplexConj"},
                                                                            {"Phasor Creation", "CreatePhasor"},
                                                                            {"Power Calculation", "PowerCalc"},
                                                                            {"Signal Type/Unit", "SpecTypeUnit"},
                                                                            {"Metric Prefix", "MetricPrefix"},
                                                                            {"Angle Conversion", "AngleConversion"}}
            _customizationReverseNameDictionary = _customizationNameDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
            _customizationList = _customizationNameDictionary.Keys.ToList
            _customizationNameParemetersDictionary = New Dictionary(Of String, List(Of String)) From {{"Scalar Repetition", {"CustPMUname", "scalar", "SignalName", "SignalType", "SignalUnit", "TimeSourcePMU"}.ToList},
                                                                            {"Addition", {"CustPMUname", "SignalName", "term"}.ToList},
                                                                            {"Subtraction", {"CustPMUname", "SignalName", "minuend", "subtrahend"}.ToList},
                                                                            {"Multiplication", {"CustPMUname", "SignalName", "factor"}.ToList},
                                                                            {"Division", {"CustPMUname", "SignalName", "dividend", "divisor"}.ToList},
                                                                            {"Exponential", {"CustPMUname", "signal", "exponent"}.ToList},
                                                                            {"Sign Reversal", {"CustPMUname", "signal"}.ToList},
                                                                            {"Absolute Value", {"CustPMUname", "signal"}.ToList},
                                                                            {"Real Component", {"CustPMUname", "signal"}.ToList},
                                                                            {"Imaginary Component", {"CustPMUname", "signal"}.ToList},
                                                                            {"Angle Calculation", {"CustPMUname", "signal"}.ToList},
                                                                            {"Complex Conjugate", {"CustPMUname", "signal"}.ToList},
                                                                            {"Phasor Creation", {"CustPMUname", "phasor ", "mag", "ang"}.ToList},
                                                                            {"Power Calculation", {"CustPMUname", "PowType", "power"}.ToList},
                                                                            {"Signal Type/Unit", {"CustPMUname", "CustName", "SigType", "SigUnit", "PMU", "Channel"}.ToList},
                                                                            {"Metric Prefix", {"CustPMUname", "ToConvert"}.ToList},
                                                                            {"Angle Conversion", {"CustPMUname", "ToConvert"}.ToList}}
            _typeUnitDictionary = New Dictionary(Of String, List(Of String)) From {{"VMP", {"kV", "V"}.ToList},
                                                                                   {"VMA", {"kV", "V"}.ToList},
                                                                                   {"VMB", {"kV", "V"}.ToList},
                                                                                   {"VMC", {"kV", "V"}.ToList},
                                                                                   {"VAP", {"DEG", "RAD"}.ToList},
                                                                                   {"VAA", {"DEG", "RAD"}.ToList},
                                                                                   {"VAB", {"DEG", "RAD"}.ToList},
                                                                                   {"VAC", {"DEG", "RAD"}.ToList},
                                                                                   {"VPP", {"kV", "V"}.ToList},
                                                                                   {"VPA", {"kV", "V"}.ToList},
                                                                                   {"VPB", {"kV", "V"}.ToList},
                                                                                   {"VPC", {"kV", "V"}.ToList},
                                                                                   {"IMP", {"A", "kA"}.ToList},
                                                                                   {"IMA", {"A", "kA"}.ToList},
                                                                                   {"IMB", {"A", "kA"}.ToList},
                                                                                   {"IMC", {"A", "kA"}.ToList},
                                                                                   {"IAP", {"DEG", "RAD"}.ToList},
                                                                                   {"IAA", {"DEG", "RAD"}.ToList},
                                                                                   {"IAB", {"DEG", "RAD"}.ToList},
                                                                                   {"IAC", {"DEG", "RAD"}.ToList},
                                                                                   {"IPP", {"A", "kA"}.ToList},
                                                                                   {"IPA", {"A", "kA"}.ToList},
                                                                                   {"IPB", {"A", "kA"}.ToList},
                                                                                   {"IPC", {"A", "kA"}.ToList},
                                                                                   {"P", {"MW"}.ToList},
                                                                                   {"Q", {"MVAR"}.ToList},
                                                                                   {"CP", {"MVA"}.ToList},
                                                                                   {"S", {"MVA"}.ToList},
                                                                                   {"F", {"Hz", "mHz"}.ToList},
                                                                                   {"RCF", {"mHz/sec", "Hz/sec"}.ToList},
                                                                                   {"D", {"D"}.ToList},
                                                                                   {"SC", {"SC"}.ToList},
                                                                                   {"OTHER", {"O"}.ToList}}

            _collectionOfSteps = New ObservableCollection(Of Customization)
            _unitList = New List(Of String)
        End Sub
        Private _collectionOfSteps As ObservableCollection(Of Customization)
        Public Property CollectionOfSteps As ObservableCollection(Of Customization)
            Get
                Return _collectionOfSteps
            End Get
            Set(ByVal value As ObservableCollection(Of Customization))
                _collectionOfSteps = value
                OnPropertyChanged()
            End Set
        End Property
        Private ReadOnly _customizationList As List(Of String)
        Public ReadOnly Property CustomizationList As List(Of String)
            Get
                Return _customizationList
            End Get
        End Property
        Private _customizationNameDictionary As Dictionary(Of String, String)
        Public Property CustomizationNameDictionary As Dictionary(Of String, String)
            Get
                Return _customizationNameDictionary
            End Get
            Set(ByVal value As Dictionary(Of String, String))
                _customizationNameDictionary = value
                OnPropertyChanged()
            End Set
        End Property

        Private _customizationReverseNameDictionary As Dictionary(Of String, String)
        Public Property CustomizationReverseNameDictionary As Dictionary(Of String, String)
            Get
                Return _customizationReverseNameDictionary
            End Get
            Set(ByVal value As Dictionary(Of String, String))
                _customizationReverseNameDictionary = value
                OnPropertyChanged()
            End Set
        End Property

        Private _customizationNameParemetersDictionary As Dictionary(Of String, List(Of String))
        Public Property CustomizationNameParemetersDictionary As Dictionary(Of String, List(Of String))
            Get
                Return _customizationNameParemetersDictionary
            End Get
            Set(ByVal value As Dictionary(Of String, List(Of String)))
                _customizationNameParemetersDictionary = value
                OnPropertyChanged()
            End Set
        End Property
        Private _typeUnitDictionary As Dictionary(Of String, List(Of String))
        Public Property TypeUnitDictionary As Dictionary(Of String, List(Of String))
            Get
                Return _typeUnitDictionary
            End Get
            Set(ByVal value As Dictionary(Of String, List(Of String)))
                _typeUnitDictionary = value
                OnPropertyChanged()
            End Set
        End Property
        Private _unitList As List(Of String)
        Public Property UnitList As List(Of String)
            Get
                Return _unitList
            End Get
            Set(ByVal value As List(Of String))
                _unitList = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

End Namespace
