Imports System.Collections.ObjectModel

Public Class PostProcessCustomizationConfig
    Inherits ViewModelBase
    Public Sub New()
        _customizationNameDictionary = New Dictionary(Of String, String) From {{"Scalar Repetition Customization", "ScalarRep"},
                                                                            {"Addition Customization", "Addition"},
                                                                            {"Subtraction Customization", "Subtraction"},
                                                                            {"Multiplication Customization", "Multiplication"},
                                                                            {"Division Customization", "Division"},
                                                                            {"Raise signals to an exponent", "Exponent"},
                                                                            {"Reverse sign of signals", "SignReversal"},
                                                                            {"Take absolute value of signals", "AbsVal"},
                                                                            {"Return real component of signals", "RealComponent"},
                                                                            {"Return imaginary component of signals", "ImagComponent"},
                                                                            {"Return angle of complex valued signals", "Angle"},
                                                                            {"Take complex conjugate of signals", "ComplexConj"},
                                                                            {"Phasor Creation Customization", "CreatePhasor"},
                                                                            {"Power Calculation Customization", "PowerCalc"},
                                                                            {"Specify Signal Type and Unit Customization", "SpecTypeUnit"},
                                                                            {"Metric Prefix Customization", "MetricPrefix"},
                                                                            {"Angle Conversion Customization", "AngleConversion"}}
        _customizationReverseNameDictionary = _customizationNameDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
        _customizationList = _customizationNameDictionary.Keys.ToList
        _customizationNameParemetersDictionary = New Dictionary(Of String, List(Of String)) From {{"Scalar Repetition Customization", {"CustPMUname", "scalar", "SignalName", "SignalType", "SignalUnit", "TimeSourcePMU"}.ToList},
                                                                            {"Addition Customization", {"CustPMUname", "SignalName", "term"}.ToList},
                                                                            {"Subtraction Customization", {"CustPMUname", "SignalName", "minuend", "subtrahend"}.ToList},
                                                                            {"Multiplication Customization", {"CustPMUname", "SignalName", "factor"}.ToList},
                                                                            {"Division Customization", {"CustPMUname", "SignalName", "dividend", "divisor"}.ToList},
                                                                            {"Raise signals to an exponent", {"CustPMUname", "signal", "exponent"}.ToList},
                                                                            {"Reverse sign of signals", {"CustPMUname", "signal"}.ToList},
                                                                            {"Take absolute value of signals", {"CustPMUname", "signal"}.ToList},
                                                                            {"Return real component of signals", {"CustPMUname", "signal"}.ToList},
                                                                            {"Return imaginary component of signals", {"CustPMUname", "signal"}.ToList},
                                                                            {"Return angle of complex valued signals", {"CustPMUname", "signal"}.ToList},
                                                                            {"Take complex conjugate of signals", {"CustPMUname", "signal"}.ToList},
                                                                            {"Phasor Creation Customization", {"CustPMUname", "phasor ", "mag", "ang"}.ToList},
                                                                            {"Power Calculation Customization", {"CustPMUname", "PowType", "power"}.ToList},
                                                                            {"Specify Signal Type and Unit Customization", {"CustPMUname", "CustName", "SigType", "SigUnit", "PMU", "Channel"}.ToList},
                                                                            {"Metric Prefix Customization", {"CustPMUname", "ToConvert"}.ToList},
                                                                            {"Angle Conversion Customization", {"CustPMUname", "ToConvert"}.ToList}}

        _collectionOfSteps = New ObservableCollection(Of Customization)
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
End Class
