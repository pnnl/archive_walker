Imports System.Collections.ObjectModel
Imports System.Windows.Forms
Imports BAWGUI.ReadConfigXml
Imports BAWGUI.SignalManagement.ViewModels
Imports BAWGUI.Utilities

Namespace ViewModels
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
                                                                            {"Angle Conversion", "AngleConversion"},
                                                                            {"Duplicate Signals", "ReplicateSignal"}}
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
                                                                                   {"P", {"W", "kW", "MW"}.ToList},
                                                                                   {"Q", {"VAR", "kVAR", "MVAR"}.ToList},
                                                                                   {"CP", {"VA", "kVA", "MVA"}.ToList},
                                                                                   {"S", {"VA", "kVA", "MVA"}.ToList},
                                                                                   {"F", {"Hz", "mHz"}.ToList},
                                                                                   {"RCF", {"mHz/sec", "Hz/sec"}.ToList},
                                                                                   {"D", {"D"}.ToList},
                                                                                   {"SC", {"SC"}.ToList},
                                                                                   {"OTHER", {"O"}.ToList},
                                                                                   {"VWA", {"V", "kV"}.ToList},
                                                                                   {"VWB", {"V", "kV"}.ToList},
                                                                                   {"VWC", {"V", "kV"}.ToList},
                                                                                   {"IWA", {"A", "kA"}.ToList},
                                                                                   {"IWB", {"A", "kA"}.ToList},
                                                                                   {"IWC", {"A", "kA"}.ToList}}
            _model = New PostProcessConfigModel()
            _collectionOfSteps = New ObservableCollection(Of Object)
            _unitList = New List(Of String)
        End Sub

        Public Sub New(postProcessConfigure As PostProcessConfigModel, signalsMgr As SignalManager)
            Me.New
            Me._model = postProcessConfigure
            Dim allSteps = New ObservableCollection(Of Object)
            Dim stepCounter As Integer = 0
            For Each stp In _model.CollectionOfSteps
                Dim name = stp.Name
                Try
                    Select Case name
                        Case "Scalar Repetition"
                            stepCounter += 1
                            Dim a = New ScalarRepCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Addition"
                            stepCounter += 1
                            Dim a = New AdditionCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Subtraction"
                            stepCounter += 1
                            Dim a = New SubtractionCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Multiplication"
                            stepCounter += 1
                            Dim a = New MultiplicationCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Division"
                            stepCounter += 1
                            Dim a = New DivisionCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Exponential"
                            stepCounter += 1
                            Dim a = New ExponentialCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Sign Reversal"
                            stepCounter += 1
                            Dim a = New SignReversalCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Absolute Value"
                            stepCounter += 1
                            Dim a = New AbsValCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Real Component"
                            stepCounter += 1
                            Dim a = New RealComponentCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Imaginary Component"
                            stepCounter += 1
                            Dim a = New ImagComponentCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Angle Calculation"
                            stepCounter += 1
                            Dim a = New AngleCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Complex Conjugate"
                            stepCounter += 1
                            Dim a = New ComplexConjCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Phasor Creation"
                            stepCounter += 1
                            Dim a = New CreatePhasorCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Power Calculation"
                            stepCounter += 1
                            Dim a = New PowerCalcCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Signal Type/Unit"
                            stepCounter += 1
                            Dim a = New SpecifySignalTypeUnitCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Metric Prefix"
                            stepCounter += 1
                            Dim a = New MetricPrefixCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Angle Conversion"
                            stepCounter += 1
                            Dim a = New AngleConversionCust(stp, stepCounter, signalsMgr, True)
                            allSteps.Add(a)
                        Case "Duplicate Signals"
                            stepCounter += 1
                            Dim a = New SignalReplicationCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case Else
                            Throw New Exception(String.Format("Wrong stage name found in Config.xml file: {0}", name))
                    End Select
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
                End Try
            Next
            CollectionOfSteps = allSteps
        End Sub


        Private _model As PostProcessConfigModel
        Public Property Model As PostProcessConfigModel
            Get
                Return _model
            End Get
            Set(ByVal value As PostProcessConfigModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property

        Private _collectionOfSteps As ObservableCollection(Of Object)
        Public Property CollectionOfSteps As ObservableCollection(Of Object)
            Get
                Return _collectionOfSteps
            End Get
            Set(ByVal value As ObservableCollection(Of Object))
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
