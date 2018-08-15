Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows.Forms
Imports BAWGUI.Core
Imports BAWGUI.Core.Models
Imports BAWGUI.ReadConfigXml
Imports BAWGUI.SignalManagement.ViewModels
Imports BAWGUI.Utilities

Namespace ViewModels
    Public Class DataConfig
        Inherits PostProcessCustomizationConfig
        Public Sub New()
            MyBase.New
            _readerProperty = New ReaderProperties
            '_collectionOfSteps = New ObservableCollection(Of Object)
            _dqFilterNameDictionary = New Dictionary(Of String, String) From {{"Status Flags", "PMUflagFilt"},
                                                                            {"Zeros", "DropOutZeroFilt"},
                                                                            {"Missing", "DropOutMissingFilt"},
                                                                            {"Nominal Voltage", "VoltPhasorFilt"},
                                                                            {"Nominal Frequency", "FreqFilt"},
                                                                            {"Outliers", "OutlierFilt"},
                                                                            {"Stale Data", "StaleFilt"},
                                                                            {"Data Frame", "DataFrameFilt"},
                                                                            {"Channel", "PMUchanFilt"},
                                                                            {"Entire PMU", "PMUallFilt"},
                                                                            {"Angle Wrapping", "WrappingFailureFilt"}}
            _dqFilterReverseNameDictionary = _dqFilterNameDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
            _dqFilterList = _dqFilterNameDictionary.Keys.ToList
            _model = New DataConfigModel()
            _dqFilterNameParametersDictionary = New Dictionary(Of String, List(Of String)) From {{"Status Flags", {"SetToNaN", "FlagBit"}.ToList},
                                                                            {"Zeros", {"SetToNaN", "FlagBit"}.ToList},
                                                                            {"Missing", {"SetToNaN", "FlagBit"}.ToList},
                                                                            {"Nominal Voltage", {"SetToNaN", "FlagBit", "VoltMin", "VoltMax", "NomVoltage"}.ToList},
                                                                            {"Nominal Frequency", {"SetToNaN", "FreqMinChan", "FreqMaxChan", "FreqPctChan", "FreqMinSamp", "FreqMaxSamp", "FlagBitChan", "FlagBitSamp"}.ToList},
                                                                            {"Outliers", {"SetToNaN", "FlagBit", "StdDevMult"}.ToList},
                                                                            {"Stale Data", {"SetToNaN", "FlagBit", "StaleThresh", "FlagAllByFreq", "FlagBitFreq"}.ToList},
                                                                            {"Data Frame", {"SetToNaN", "FlagBit", "PercentBadThresh"}.ToList},
                                                                            {"Channel", {"SetToNaN", "FlagBit", "PercentBadThresh"}.ToList},
                                                                            {"Entire PMU", {"SetToNaN", "FlagBit", "PercentBadThresh"}.ToList},
                                                                            {"Angle Wrapping", {"SetToNaN", "FlagBit", "AngleThresh"}.ToList}}

            '_customizationNameDictionary = New Dictionary(Of String, String) From {{"Scalar Repetition", "ScalarRep"},
            '                                                                    {"Addition", "Addition"},
            '                                                                    {"Subtraction", "Subtraction"},
            '                                                                    {"Multiplication", "Multiplication"},
            '                                                                    {"Division", "Division"},
            '                                                                    {"Exponential", "Exponent"},
            '                                                                    {"Sign Reversal", "SignReversal"},
            '                                                                    {"Absolute Value", "AbsVal"},
            '                                                                    {"Real Component", "RealComponent"},
            '                                                                    {"Imaginary Component", "ImagComponent"},
            '                                                                    {"Angle Calculation", "Angle"},
            '                                                                    {"Complex Conjugate", "ComplexConj"},
            '                                                                    {"Phasor Creation", "CreatePhasor"},
            '                                                                    {"Power Calculation", "PowerCalc"},
            '                                                                    {"Signal Type/Unit", "SpecTypeUnit"},
            '                                                                    {"Metric Prefix", "MetricPrefix"},
            '                                                                    {"Angle Conversion", "AngleConversion"}}
            '_customizationReverseNameDictionary = _customizationNameDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
            '_customizationList = _customizationNameDictionary.Keys.ToList
            '_customizationNameParemetersDictionary = New Dictionary(Of String, List(Of String)) From {{"Scalar Repetition", {"CustPMUname", "scalar", "SignalName", "SignalType", "SignalUnit", "TimeSourcePMU"}.ToList},
            '                                                                    {"Addition", {"CustPMUname", "SignalName", "term"}.ToList},
            '                                                                    {"Subtraction", {"CustPMUname", "SignalName", "minuend", "subtrahend"}.ToList},
            '                                                                    {"Multiplication", {"CustPMUname", "SignalName", "factor"}.ToList},
            '                                                                    {"Division", {"CustPMUname", "SignalName", "dividend", "divisor"}.ToList},
            '                                                                    {"Exponential", {"CustPMUname", "signal", "exponent"}.ToList},
            '                                                                    {"Sign Reversal", {"CustPMUname", "signal"}.ToList},
            '                                                                    {"Absolute Value", {"CustPMUname", "signal"}.ToList},
            '                                                                    {"Real Component", {"CustPMUname", "signal"}.ToList},
            '                                                                    {"Imaginary Component", {"CustPMUname", "signal"}.ToList},
            '                                                                    {"Angle Calculation", {"CustPMUname", "signal"}.ToList},
            '                                                                    {"Complex Conjugate", {"CustPMUname", "signal"}.ToList},
            '                                                                    {"Phasor Creation", {"CustPMUname", "phasor ", "mag", "ang"}.ToList},
            '                                                                    {"Power Calculation", {"CustPMUname", "PowType", "power"}.ToList},
            '                                                                    {"Signal Type/Unit", {"CustPMUname", "CustName", "SigType", "SigUnit", "PMU", "Channel"}.ToList},
            '                                                                    {"Metric Prefix", {"CustPMUname", "ToConvert"}.ToList},
            '                                                                    {"Angle Conversion", {"CustPMUname", "ToConvert"}.ToList}}

            '_collectionOfSteps = New ObservableCollection(Of Object)
        End Sub

        Public Sub New(dataConfigure As ReadConfigXml.DataConfigModel, signalsMgr As SignalManager)
            Me.New
            _model = dataConfigure
            ReaderProperty = New ReaderProperties(_model.ReaderProperty, signalsMgr)
            Dim allSteps = New ObservableCollection(Of Object)
            Dim stepCounter As Integer = 0
            For Each stp In _model.CollectionOfSteps
                Dim name = stp.Name
                Try
                    Select Case name
                        Case "Status Flags"
                            stepCounter += 1
                            Dim a = New StatusFlagsDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Zeros"
                            stepCounter += 1
                            Dim a = New ZerosDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Missing"
                            stepCounter += 1
                            Dim a = New MissingDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Angle Wrapping"
                            stepCounter += 1
                            Dim a = New WrappingFailureDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Data Frame"
                            stepCounter += 1
                            Dim a = New DataFrameDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Channel"
                            stepCounter += 1
                            Dim a = New PMUchanDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Entire PMU"
                            stepCounter += 1
                            Dim a = New PMUallDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Stale Data"
                            stepCounter += 1
                            Dim a = New StaleDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Outliers"
                            stepCounter += 1
                            Dim a = New OutlierDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Nominal Frequency"
                            stepCounter += 1
                            Dim a = New FreqDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Nominal Voltage"
                            stepCounter += 1
                            Dim a = New VoltPhasorDQFilter(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Scalar Repetition"
                            stepCounter += 1
                            Dim a = New ScalarRepCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Addition"
                            stepCounter += 1
                            Dim a = New AdditionCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Subtraction"
                            stepCounter += 1
                            Dim a = New SubtractionCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Multiplication"
                            stepCounter += 1
                            Dim a = New MultiplicationCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Division"
                            stepCounter += 1
                            Dim a = New DivisionCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Exponential"
                            stepCounter += 1
                            Dim a = New ExponentialCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Sign Reversal"
                            stepCounter += 1
                            Dim a = New SignReversalCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Absolute Value"
                            stepCounter += 1
                            Dim a = New AbsValCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Real Component"
                            stepCounter += 1
                            Dim a = New RealComponentCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Imaginary Component"
                            stepCounter += 1
                            Dim a = New ImagComponentCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Angle Calculation"
                            stepCounter += 1
                            Dim a = New AngleCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Complex Conjugate"
                            stepCounter += 1
                            Dim a = New ComplexConjCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Phasor Creation"
                            stepCounter += 1
                            Dim a = New CreatePhasorCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Power Calculation"
                            stepCounter += 1
                            Dim a = New PowerCalcCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Signal Type/Unit"
                            stepCounter += 1
                            Dim a = New SpecifySignalTypeUnitCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Metric Prefix"
                            stepCounter += 1
                            Dim a = New MetricPrefixCust(stp, stepCounter, signalsMgr)
                            allSteps.Add(a)
                        Case "Angle Conversion"
                            stepCounter += 1
                            Dim a = New AngleConversionCust(stp, stepCounter, signalsMgr)
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

        Private _readerProperty As ReaderProperties
        Public Property ReaderProperty As ReaderProperties
            Get
                Return _readerProperty
            End Get
            Set(ByVal value As ReaderProperties)
                _readerProperty = value
                OnPropertyChanged("ReaderProperty")
            End Set
        End Property

        'Private _collectionOfSteps As ObservableCollection(Of Object)
        'Public Overloads Property CollectionOfSteps As ObservableCollection(Of Object)
        '    Get
        '        Return _collectionOfSteps
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of Object))
        '        _collectionOfSteps = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

        Private ReadOnly _dqFilterList As List(Of String)
        Public ReadOnly Property DQFilterList As List(Of String)
            Get
                Return _dqFilterList
            End Get
        End Property
        'Private ReadOnly _customizationList As List(Of String)
        'Public ReadOnly Property CustomizationList As List(Of String)
        '    Get
        '        Return _customizationList
        '    End Get
        'End Property

        Private ReadOnly _dqFilterNameDictionary As Dictionary(Of String, String)
        Public ReadOnly Property DQFilterNameDictionary As Dictionary(Of String, String)
            Get
                Return _dqFilterNameDictionary
            End Get
        End Property

        Private ReadOnly _dqFilterReverseNameDictionary As Dictionary(Of String, String)
        Public ReadOnly Property DQFilterReverseNameDictionary() As Dictionary(Of String, String)
            Get
                Return _dqFilterReverseNameDictionary
            End Get
        End Property

        Private ReadOnly _dqFilterNameParametersDictionary As Dictionary(Of String, List(Of String))
        Public ReadOnly Property DQFilterNameParametersDictionary As Dictionary(Of String, List(Of String))
            Get
                Return _dqFilterNameParametersDictionary
            End Get
        End Property

        Private _model As ReadConfigXml.DataConfigModel
        Public Property Model As ReadConfigXml.DataConfigModel
            Set
                _model = Value
                OnPropertyChanged()
            End Set
            Get
                Return _model
            End Get
        End Property

        'Private _customizationNameDictionary As Dictionary(Of String, String)
        'Public Property CustomizationNameDictionary As Dictionary(Of String, String)
        '    Get
        '        Return _customizationNameDictionary
        '    End Get
        '    Set(ByVal value As Dictionary(Of String, String))
        '        _customizationNameDictionary = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

        'Private _customizationReverseNameDictionary As Dictionary(Of String, String)
        'Public Property CustomizationReverseNameDictionary As Dictionary(Of String, String)
        '    Get
        '        Return _customizationReverseNameDictionary
        '    End Get
        '    Set(ByVal value As Dictionary(Of String, String))
        '        _customizationReverseNameDictionary = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

        'Private _customizationNameParemetersDictionary As Dictionary(Of String, List(Of String))
        'Public Property CustomizationNameParemetersDictionary As Dictionary(Of String, List(Of String))
        '    Get
        '        Return _customizationNameParemetersDictionary
        '    End Get
        '    Set(ByVal value As Dictionary(Of String, List(Of String)))
        '        _customizationNameParemetersDictionary = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
    End Class
    'Public Enum DataFileType
    '    <Description("PDAT")>
    '    pdat
    '    <Description("JSIS CSV")>
    '    csv
    'End Enum

    'Public Enum ModeType
    '    Archive
    '    <Description("Real Time")>
    '    RealTime
    '    Hybrid
    'End Enum

    'Public Enum PowerType
    '    <Description("Complex")>
    '    CP
    '    <Description("Apparent")>
    '    S
    '    <Description("Active")>
    '    P
    '    <Description("Reactive")>
    '    Q
    'End Enum

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''Class ReaderProperties'''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class ReaderProperties
        Inherits ViewModelBase
        Public Sub New()
            '_mode = New Dictionary(Of ModeType, Dictionary(Of String, String))
            '_modeParams = New ObservableCollection(Of ParameterValuePair)

            'ArchiveModeVisibility = Visibility.Collapsed
            'RealTimeModeVisibility = Visibility.Collapsed
            'HybridModeVisibility = Visibility.Collapsed
            _model = New ReadConfigXml.ReaderPropertiesModel
            '_dateTimeStart = "01/01/0001 00:00:00"
            '_dateTimeEnd = "01/01/0001 00:00:00"
            '_selectedStartTime = "01/01/0001 00:00:00"
            '_selectedEndTime = "01/01/0001 00:00:00"
            '_selectedTimeZone = TimeZoneInfo.Utc

            _inputFileInfos = New ObservableCollection(Of InputFileInfoViewModel)
        End Sub

        Public Sub New(readerProperty As ReadConfigXml.ReaderPropertiesModel, signalsMgr As SignalManager)
            Me.New
            Me._model = readerProperty
            'look for input file info in signal manager first, 
            'If exist Then, the file info Is sound And can be added To the reader Property To be displayed
            'if cannot be found, then, the file info is bad, add the file info only to the reader property to be displayed
            For Each info In _model.InputFileInfos
                Dim infoFound = False
                For Each existingInfo In signalsMgr.FileInfo
                    If info.ExampleFile = existingInfo.ExampleFile Then
                        _inputFileInfos.Add(existingInfo)
                        infoFound = True
                        Exit For
                    End If
                Next
                If Not infoFound Then
                    _inputFileInfos.Add(New InputFileInfoViewModel(info))
                End If
            Next
        End Sub

        Private _inputFileInfos As ObservableCollection(Of InputFileInfoViewModel)
        Public Property InputFileInfos As ObservableCollection(Of InputFileInfoViewModel)
            Get
                Return _inputFileInfos
            End Get
            Set(ByVal value As ObservableCollection(Of InputFileInfoViewModel))
                _inputFileInfos = value
                OnPropertyChanged()
            End Set
        End Property
        'Private _fileDirectory As String
        'Public Property FileDirectory As String
        '    Get
        '        Return _fileDirectory
        '    End Get
        '    Set(ByVal value As String)
        '        _fileDirectory = value
        '        OnPropertyChanged("FileDirectory")
        '    End Set
        'End Property

        'Private _fileType As DataFileType
        'Public Property FileType As DataFileType
        '    Get
        '        Return _fileType
        '    End Get
        '    Set(ByVal value As DataFileType)
        '        _fileType = value
        '        OnPropertyChanged("FileType")
        '    End Set
        'End Property

        'Private _mnemonic As String
        'Public Property Mnemonic As String
        '    Get
        '        Return _mnemonic
        '    End Get
        '    Set(ByVal value As String)
        '        _mnemonic = value
        '        OnPropertyChanged("Mnemonic")
        '    End Set
        'End Property

        'Private _mode As Dictionary(Of ModeType, Dictionary(Of String, String))
        'Public Property Mode As Dictionary(Of ModeType, Dictionary(Of String, String))
        '    Get
        '        Return _mode
        '    End Get
        '    Set(ByVal value As Dictionary(Of ModeType, Dictionary(Of String, String)))
        '        _mode = value
        '        OnPropertyChanged("Mode")
        '    End Set
        'End Property

        Private _modeName As ModeType
        Public Property ModeName As ModeType
            Get
                Return _model.ModeName
            End Get
            Set(ByVal value As ModeType)
                'Can set up a dictionary with ModeName as key, and ModeParams as value to save the old values
                'Then test the new value to see if it exists in the dictionary
                'If yes, set ModeParams to the old values
                'If not, call _constructParamTable()
                'This way we can show old mode parameters when switch among different mode name.
                _model.ModeName = value
                '_changeModeParamsVisibility()
                OnPropertyChanged("ModeName")
            End Set
        End Property

        'Private _realTimeModeVisibility As Visibility
        'Public Property RealTimeModeVisibility As Visibility
        '    Get
        '        Return _realTimeModeVisibility
        '    End Get
        '    Set(ByVal value As Visibility)
        '        _realTimeModeVisibility = value
        '        OnPropertyChanged("RealTimeModeVisibility")
        '    End Set
        'End Property

        'Private _hybridModeVisibility As Visibility
        'Public Property HybridModeVisibility As Visibility
        '    Get
        '        Return _hybridModeVisibility
        '    End Get
        '    Set(ByVal value As Visibility)
        '        _hybridModeVisibility = value
        '        OnPropertyChanged("HybridModeVisibility")
        '    End Set
        'End Property

        'Private _archiveModeVisibility As Visibility
        'Public Property ArchiveModeVisibility As Visibility
        '    Get
        '        Return _archiveModeVisibility
        '    End Get
        '    Set(ByVal value As Visibility)
        '        _archiveModeVisibility = value
        '        OnPropertyChanged("ArchiveModeVisibility")
        '    End Set
        'End Property

        'Private Sub _changeModeParamsVisibility()
        '    'Dim newParams = New ObservableCollection(Of ParameterValuePair)
        '    Select Case _modeName
        '        Case ModeType.Archive
        '            ArchiveModeVisibility = Visibility.Visible
        '            RealTimeModeVisibility = Visibility.Collapsed
        '            HybridModeVisibility = Visibility.Collapsed
        '        Case ModeType.Hybrid
        '            ArchiveModeVisibility = Visibility.Collapsed
        '            RealTimeModeVisibility = Visibility.Collapsed
        '            HybridModeVisibility = Visibility.Visible
        '        Case ModeType.RealTime
        '            ArchiveModeVisibility = Visibility.Collapsed
        '            RealTimeModeVisibility = Visibility.Visible
        '            HybridModeVisibility = Visibility.Collapsed
        '    End Select
        '    'ModeParams = newParams
        'End Sub

        Private _dateTimeStart As String
        Public Property DateTimeStart As String
            Get
                Return _model.DateTimeStart
            End Get
            Set(ByVal value As String)
                _model.DateTimeStart = value
                'If SelectUTCTime Then
                '_convertStartTimeToSelectedTimeZone()
                'End If
                Dim StackTrace = New StackTrace()
                Dim stackFrames = StackTrace.GetFrames()

                For Each sf In stackFrames
                    Dim a = sf.GetMethod().Name
                Next
                OnPropertyChanged("DateTimeStart")
            End Set
        End Property

        'Private Sub _convertStartTimeToSelectedTimeZone()
        '    'If SelectUTCTime Then
        '    'Dim TimeZone As TimeZoneInfo = DirectCast(SelectedTimeZone, TimeZoneInfo)
        '    If DateTimeStart IsNot Nothing Then
        '        Dim UTCTimeStart = DateTime.Parse(DateTimeStart, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
        '        Dim newConvertedStartTime = TimeZoneInfo.ConvertTimeFromUtc(UTCTimeStart, SelectedTimeZone)
        '        If newConvertedStartTime.ToString <> ConvertedStartTime Then
        '            ConvertedStartTime = newConvertedStartTime
        '        End If
        '    End If
        '    'Else
        '    '    Throw New Exception("Error! Bad settings in start time or converted start time.")
        '    'End If
        'End Sub

        'Private Sub _convertEndTimeToSelectedTimeZone()
        '    If DateTimeEnd IsNot Nothing Then
        '        Dim UTCTimeEnd = DateTime.Parse(DateTimeEnd, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
        '        Dim newConvertedEndTime = TimeZoneInfo.ConvertTimeFromUtc(UTCTimeEnd, SelectedTimeZone)
        '        If newConvertedEndTime.ToString <> ConvertedEndTime Then
        '            ConvertedEndTime = newConvertedEndTime
        '        End If
        '    End If
        'End Sub

        Private _dateTimeEnd As String
        Public Property DateTimeEnd As String
            Get
                Return _model.DateTimeEnd
            End Get
            Set(ByVal value As String)
                _model.DateTimeEnd = value
                '_convertEndTimeToSelectedTimeZone()
                OnPropertyChanged("DateTimeEnd")
            End Set
        End Property

        'Private _selectedTimeZone As TimeZoneInfo
        'Public Property SelectedTimeZone As TimeZoneInfo
        '    Get
        '        Return _selectedTimeZone
        '    End Get
        '    Set(ByVal value As TimeZoneInfo)
        '        _selectedTimeZone = value
        '        'If SelectAlternateTimeZone Then
        '        _convertStartTimeToUTCTime()
        '        _convertEndTimeToUTCTime()
        '        'ElseIf SelectUTCTime Then
        '        '    _convertTimeToSelectedTimeZone()
        '        'End If
        '        OnPropertyChanged("SelectedTimeZone")
        '    End Set
        'End Property

        'Private _convertedStartTime As String
        'Public Property ConvertedStartTime As String
        '    Get
        '        Return _convertedStartTime
        '    End Get
        '    Set(ByVal value As String)
        '        _convertedStartTime = value
        '        'If SelectAlternateTimeZone Then
        '        _convertStartTimeToUTCTime()
        '        'End If
        '        OnPropertyChanged("ConvertedStartTime")
        '    End Set
        'End Property

        'Private Sub _convertStartTimeToUTCTime()
        '    'Dim TimeZone As TimeZoneInfo = DirectCast(SelectedTimeZone, TimeZoneInfo)
        '    If ConvertedStartTime IsNot Nothing Then
        '        Dim localTimeStart = DateTime.Parse(ConvertedStartTime)
        '        Dim newDateTimeStart = TimeZoneInfo.ConvertTimeToUtc(localTimeStart, SelectedTimeZone)
        '        If newDateTimeStart.ToString <> DateTimeStart Then
        '            DateTimeStart = newDateTimeStart
        '        End If
        '    End If
        'End Sub

        'Private Sub _convertEndTimeToUTCTime()
        '    If ConvertedEndTime IsNot Nothing Then
        '        Dim localTimeEnd = DateTime.Parse(ConvertedEndTime)
        '        Dim newDateTimeEnd = TimeZoneInfo.ConvertTimeToUtc(localTimeEnd, SelectedTimeZone)
        '        If newDateTimeEnd.ToString <> DateTimeEnd Then
        '            DateTimeEnd = newDateTimeEnd
        '        End If
        '    End If
        'End Sub

        'Private _convertedEndTime As String
        'Public Property ConvertedEndTime As String
        '    Get
        '        Return _convertedEndTime
        '    End Get
        '    Set(ByVal value As String)
        '        _convertedEndTime = value
        '        _convertEndTimeToUTCTime()
        '        OnPropertyChanged("ConvertedEndTime")
        '    End Set
        'End Property

        Private _noFutureWait As String
        Public Property NoFutureWait As String
            Get
                Return _model.NoFutureWait
            End Get
            Set(ByVal value As String)
                _model.NoFutureWait = value
                OnPropertyChanged("NoFutureWait")
            End Set
        End Property

        Private _maxNoFutureCount As String
        Public Property MaxNoFutureCount As String
            Get
                Return _model.MaxNoFutureCount
            End Get
            Set(ByVal value As String)
                _model.MaxNoFutureCount = value
                OnPropertyChanged("MaxNoFutureCount")
            End Set
        End Property

        Private _futureWait As String
        Public Property FutureWait As String
            Get
                Return _model.FutureWait
            End Get
            Set(ByVal value As String)
                _model.FutureWait = value
                OnPropertyChanged("FutureWait")
            End Set
        End Property

        Private _maxFutureCount As String
        Public Property MaxFutureCount As String
            Get
                Return _model.MaxFutureCount
            End Get
            Set(ByVal value As String)
                _model.MaxFutureCount = value
                OnPropertyChanged("MaxFutureCount")
            End Set
        End Property

        Private _realTimeRange As String
        Public Property RealTimeRange As String
            Get
                Return _model.RealTimeRange
            End Get
            Set(ByVal value As String)
                _model.RealTimeRange = value
                OnPropertyChanged("RealTimeRange")
            End Set
        End Property


        'Private _modeParams As ObservableCollection(Of ParameterValuePair)
        'Public Property ModeParams As ObservableCollection(Of ParameterValuePair)
        '    Get
        '        Return _modeParams
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of ParameterValuePair))
        '        _modeParams = value
        '        OnPropertyChanged("ModeParams")
        '    End Set
        'End Property

        'Private _selectUTCTime As Boolean
        'Public Property SelectUTCTime As Boolean
        '    Get
        '        Return _selectUTCTime
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _selectUTCTime = value
        '        If _selectUTCTime Then
        '            SelectAlternateTimeZone = False
        '        Else
        '            SelectAlternateTimeZone = True
        '        End If
        '        OnPropertyChanged("SelectUTCTime")
        '    End Set
        'End Property

        'Private _selectAlternateTimeZone As Boolean
        'Public Property SelectAlternateTimeZone As Boolean
        '    Get
        '        Return _selectAlternateTimeZone
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _selectAlternateTimeZone = value
        '        If _selectAlternateTimeZone Then
        '            SelectUTCTime = False
        '        Else
        '            SelectUTCTime = True
        '        End If
        '        OnPropertyChanged("SelectAlternateTimeZone")
        '    End Set
        'End Property

        Public Model As ReadConfigXml.ReaderPropertiesModel
        Private _model As ReadConfigXml.ReaderPropertiesModel

    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''Class InputFileInfo''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class InputFileInfo
        Inherits ViewModelBase
        Public Sub New()
            _inputFileTree = New ObservableCollection(Of Folder)
            _groupedSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
            _groupedSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
            _taggedSignals = New ObservableCollection(Of SignalSignatureViewModel)
            _isExpanded = False
        End Sub
        Private _fileDirectory As String
        Public Property FileDirectory As String
            Get
                Return _fileDirectory
            End Get
            Set(ByVal value As String)
                _fileDirectory = value
                OnPropertyChanged()
            End Set
        End Property
        Private _samplingRate As String
        Public Property SamplingRate As String
            Get
                Return _samplingRate
            End Get
            Set(ByVal value As String)
                _samplingRate = value
                OnPropertyChanged()
            End Set
        End Property
        Private _fileType As DataFileType
        Public Property FileType As DataFileType
            Get
                Return _fileType
            End Get
            Set(ByVal value As DataFileType)
                _fileType = value
                OnPropertyChanged()
            End Set
        End Property

        Private _mnemonic As String
        Public Property Mnemonic As String
            Get
                Return _mnemonic
            End Get
            Set(ByVal value As String)
                _mnemonic = value
                OnPropertyChanged()
            End Set
        End Property
        Private _inputFileTree As ObservableCollection(Of Folder)
        Public Property InputFileTree As ObservableCollection(Of Folder)
            Get
                Return _inputFileTree
            End Get
            Set(ByVal value As ObservableCollection(Of Folder))
                _inputFileTree = value
                OnPropertyChanged()
            End Set
        End Property
        Private _signalList As List(Of String)
        Public Property SignalList As List(Of String)
            Get
                Return _signalList
            End Get
            Set(value As List(Of String))
                _signalList = value
                OnPropertyChanged()
            End Set
        End Property
        Private _taggedSignals As ObservableCollection(Of SignalSignatureViewModel)
        Public Property TaggedSignals As ObservableCollection(Of SignalSignatureViewModel)
            Get
                Return _taggedSignals
            End Get
            Set(ByVal value As ObservableCollection(Of SignalSignatureViewModel))
                _taggedSignals = value
                OnPropertyChanged()
            End Set
        End Property
        Private _groupedSignalsByType As ObservableCollection(Of SignalTypeHierachy)
        Public Property GroupedSignalsByType As ObservableCollection(Of SignalTypeHierachy)
            Get
                Return _groupedSignalsByType
            End Get
            Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
                _groupedSignalsByType = value
                OnPropertyChanged()
            End Set
        End Property
        Private _isExpanded As Boolean
        Public Property IsExpanded As Boolean
            Get
                Return _isExpanded
            End Get
            Set(ByVal value As Boolean)
                _isExpanded = value
                OnPropertyChanged()
            End Set
        End Property
        'Private _pmuSignalDictionary As Dictionary(Of String, List(Of SignalSignatures))
        'Public Property PMUSignalDictionary As Dictionary(Of String, List(Of SignalSignatures))
        '    Get
        '        Return _pmuSignalDictionary
        '    End Get
        '    Set(ByVal value As Dictionary(Of String, List(Of SignalSignatures)))
        '        _pmuSignalDictionary = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        Private _groupedSignalsByPMU As ObservableCollection(Of SignalTypeHierachy)
        Public Property GroupedSignalsByPMU As ObservableCollection(Of SignalTypeHierachy)
            Get
                Return _groupedSignalsByPMU
            End Get
            Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
                _groupedSignalsByPMU = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''Class ModeParameter'''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Public Class ModeParameter
    '    Inherits ParameterValuePair
    '    Public Sub New()
    '        MyBase.New()
    '    End Sub
    '    Public Sub New(para As String, value As String)
    '        MyBase.New(para, value)
    '    End Sub
    'End Class

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''Class DQFilterParameter'''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Public Class DQFilterParameter
    '    Inherits ParameterValuePair
    '    Public Sub New()
    '        MyBase.New()
    '    End Sub
    '    Public Sub New(para As String, value As String)
    '        MyBase.New(para, value)
    '    End Sub
    'End Class

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''Class DQFilter'''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region "DQ Filters"
    Public Class DQFilter
        Inherits SignalProcessStep

        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)

            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
            '_flagBitCounter = _flagBitCounter + 1
            'FlagBit = _flagBitCounter.ToString
            '_setToNaN = True
        End Sub


        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function


        'Private Shared _flagBitCounter As Integer = 0
        'Public ReadOnly Property FlagBit As String

        Private _setToNaN As String

        Public Property SetToNaN As String
            Get
                Return _setToNaN
            End Get
            Set(ByVal value As String)
                _setToNaN = value
                OnPropertyChanged()
            End Set
        End Property
        'Private _PMUs As ObservableCollection(Of PMU)
        'Public Property PMUs As ObservableCollection(Of PMU)
        '    Get
        '        Return _PMUs
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of PMU))
        '        _PMUs = value
        '        OnPropertyChanged()
        '    End Set
        'End Property



    End Class
    Public Class StatusFlagsDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.StatusFlagsDQFilterModel()
        End Sub

        Private _model As ReadConfigXml.StatusFlagsDQFilterModel
        Public Sub New(stp As ReadConfigXml.StatusFlagsDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                'InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
                InputChannels = signalsMgr.FindSignalsEntirePMU(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
    End Class
    Public Class ZerosDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.ZerosDQFilterModel()
        End Sub

        Private _model As ReadConfigXml.ZerosDQFilterModel

        Public Sub New(stp As ReadConfigXml.ZerosDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
    End Class
    Public Class MissingDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.MissingDQFilterModel()
        End Sub

        Private _model As ReadConfigXml.MissingDQFilterModel
        Public Sub New(stp As ReadConfigXml.MissingDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
    End Class

    Public Class VoltPhasorDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.VoltPhasorDQFilterModel()
        End Sub
        Private _model As ReadConfigXml.VoltPhasorDQFilterModel
        Public Sub New(stp As ReadConfigXml.VoltPhasorDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Public Property VoltMin As String
            Get
                Return _model.VoltMin
            End Get
            Set(ByVal value As String)
                _model.VoltMin = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property VoltMax As String
            Get
                Return _model.VoltMax
            End Get
            Set(ByVal value As String)
                _model.VoltMax = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property NomVoltage As String
            Get
                Return _model.NomVoltage
            End Get
            Set(ByVal value As String)
                _model.NomVoltage = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class FreqDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.FreqDQFilterModel()
        End Sub
        Public Property _model As ReadConfigXml.FreqDQFilterModel
        Public Sub New(stp As ReadConfigXml.FreqDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Public Property FreqMaxChan As String
            Get
                Return _model.FreqMaxChan
            End Get
            Set(ByVal value As String)
                _model.FreqMaxChan = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property FreqMinChan As String
            Get
                Return _model.FreqMinChan
            End Get
            Set(ByVal value As String)
                _model.FreqMinChan = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property FreqPctChan As String
            Get
                Return _model.FreqPctChan
            End Get
            Set(ByVal value As String)
                _model.FreqPctChan = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property FreqMinSamp As String
            Get
                Return _model.FreqMinSamp
            End Get
            Set(ByVal value As String)
                _model.FreqMinSamp = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property FreqMaxSamp As String
            Get
                Return _model.FreqMaxSamp
            End Get
            Set(ByVal value As String)
                _model.FreqMaxSamp = value
                OnPropertyChanged()
            End Set
        End Property
        'Public ReadOnly Property FlagBitChan As String
        'Public ReadOnly Property FlagBitSamp As String
    End Class

    Public Class OutlierDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.OutlierDQFilterModel()
        End Sub
        Public Property _model As ReadConfigXml.OutlierDQFilterModel
        Public Sub New(stp As ReadConfigXml.OutlierDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Public Property StdDevMult As String
            Get
                Return _model.StdDevMult
            End Get
            Set(ByVal value As String)
                _model.StdDevMult = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class StaleDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.StaleDQFilterModel()
        End Sub
        Public Property _model As ReadConfigXml.StaleDQFilterModel
        Public Sub New(stp As ReadConfigXml.StaleDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Public Property StaleThresh As String
            Get
                Return _model.StaleThresh
            End Get
            Set(ByVal value As String)
                _model.StaleThresh = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property FlagAllByFreq As Boolean
            Get
                Return _model.FlagAllByFreq
            End Get
            Set(ByVal value As Boolean)
                _model.FlagAllByFreq = value
                OnPropertyChanged()
            End Set
        End Property
        'Public ReadOnly Property FlagBitFreq As String
    End Class
    Public Class DataFrameDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.DataFrameDQFilterModel()
        End Sub

        Public Property _model As ReadConfigXml.DataFrameDQFilterModel
        Public Sub New(stp As ReadConfigXml.DataFrameDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Public Property PercentBadThresh As String
            Get
                Return _model.PercentBadThresh
            End Get
            Set(ByVal value As String)
                _model.PercentBadThresh = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class PMUchanDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.PMUchanDQFilterModel()
        End Sub

        Public Property _model As ReadConfigXml.PMUchanDQFilterModel
        Public Sub New(stp As ReadConfigXml.PMUchanDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Public Property PercentBadThresh As String
            Get
                Return _model.PercentBadThresh
            End Get
            Set(ByVal value As String)
                _model.PercentBadThresh = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class PMUallDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.PMUallDQFilterModel()
        End Sub

        Public Property _model As ReadConfigXml.PMUallDQFilterModel
        Public Sub New(stp As ReadConfigXml.PMUallDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignalsEntirePMU(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Public Property PercentBadThresh As String
            Get
                Return _model.PercentBadThresh
            End Get
            Set(ByVal value As String)
                _model.PercentBadThresh = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class WrappingFailureDQFilter
        Inherits DQFilter
        Public Sub New()
            MyBase.New
            _model = New ReadConfigXml.WrappingFailureDQFilterModel()
        End Sub
        Public Property _model As ReadConfigXml.WrappingFailureDQFilterModel
        Public Sub New(stp As ReadConfigXml.WrappingFailureDQFilterModel, stepCounter As Integer, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & stp.Name)
            End Try
            signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Public Property AngleThresh As String
            Get
                Return _model.AngleThresh
            End Get
            Set(ByVal value As String)
                _model.AngleThresh = value
                OnPropertyChanged()
            End Set
        End Property

    End Class
#End Region
#Region "Customization"
    Public Class Customization
        Inherits SignalProcessStep
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            _outputInputMappingPair = New ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
            _exponent = "1"
            IsExpanded = False
            _model = New CustomizationModel
            '_timeSourcePMU = New PMUWithSamplingRate()
            '_outputInputMultipleMappingPair = New ObservableCollection(Of KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures)))
        End Sub

        Public Sub New(cStep As Customization)
            Me.New
            Name = cStep.Name
            CustPMUname = cStep.CustPMUname
            PowType = cStep.PowType
        End Sub
        Private _model As ReadConfigXml.CustomizationModel
        Public Sub New(cStep As ReadConfigXml.CustomizationModel)
            Me.New
            _model = cStep
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            'Throw New NotImplementedException()
        End Function

        'Private _customizationName As String
        'Public Property CustomizationName As String
        '    Get
        '        Return _customizationName
        '    End Get
        '    Set(ByVal value As String)
        '        _customizationName = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        Public ReadOnly Property Name As String

        'Private _parameters As ObservableCollection(Of ParameterValuePair)
        'Public Property Parameters As ObservableCollection(Of ParameterValuePair)
        '    Get
        '        Return _parameters
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of ParameterValuePair))
        '        _parameters = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        'Private _customizationParams As ObservableCollection(Of ParameterValuePair)
        'Public Property CustomizationParams As ObservableCollection(Of ParameterValuePair)
        '    Get
        '        Return _customizationParams
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of ParameterValuePair))
        '        _customizationParams = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        Private _minuendOrDividend As SignalSignatureViewModel
        Public Property MinuendOrDividend As SignalSignatureViewModel
            Get
                Return _minuendOrDividend
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                _minuendOrDividend = value
                OnPropertyChanged()
            End Set
        End Property
        Private _subtrahendOrDivisor As SignalSignatureViewModel
        Public Property SubtrahendOrDivisor As SignalSignatureViewModel
            Get
                Return _subtrahendOrDivisor
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                _subtrahendOrDivisor = value
                OnPropertyChanged()
            End Set
        End Property
        Private _currentCursor As String
        Public Property CurrentCursor As String
            Get
                Return _currentCursor
            End Get
            Set(ByVal value As String)
                _currentCursor = value
                OnPropertyChanged()
            End Set
        End Property
        Private _custPMUname As String
        Public Property CustPMUname As String
            Get
                Return _model.CustPMUname
            End Get
            Set(ByVal value As String)
                _model.CustPMUname = value
                For Each out In OutputChannels
                    out.PMUName = value
                Next
                Dim theOnlyPMUHierachy = ThisStepOutputsAsSignalHierachyByPMU.SignalList.FirstOrDefault
                If theOnlyPMUHierachy IsNot Nothing Then
                    theOnlyPMUHierachy.SignalSignature.PMUName = value
                    theOnlyPMUHierachy.SignalSignature.SignalName = value
                End If
                OnPropertyChanged()
            End Set
        End Property
        'Private _custSignalName As ObservableCollection(Of String)
        'Public Property CustSignalName As ObservableCollection(Of String)
        '    Get
        '        Return _custSignalName
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of String))
        '        _custSignalName = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        'Private _outputChannels As ObservableCollection(Of SignalSignatures)
        'Public Property OutputChannels As ObservableCollection(Of SignalSignatures)
        '    Get
        '        Return _outputChannels
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of SignalSignatures))
        '        _outputChannels = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        ''' <summary>
        ''' The key of each item is the output and the value of the item is a collection of input(s)
        ''' </summary>
        Private _outputInputMappingPair As ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
        Public Property OutputInputMappingPair As ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
            Get
                Return _outputInputMappingPair
            End Get
            Set(ByVal value As ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))))
                _outputInputMappingPair = value
                OnPropertyChanged()
            End Set
        End Property
        Private _powType As PowerType
        Public Property PowType As PowerType
            Get
                Return _powType
            End Get
            Set(ByVal value As PowerType)
                _powType = value
                For Each out In OutputChannels
                    out.TypeAbbreviation = value.ToString
                Next
                OnPropertyChanged()
            End Set
        End Property
        Private _exponent As String

        Public Property Exponent As String
            Get
                Return _exponent
            End Get
            Set(ByVal value As String)
                _exponent = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class ScalarRepCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _timeSourcePMU = New PMUWithSamplingRate
            _model = New ScalarRepCustModel
        End Sub

        Public Sub New(cStep As ReadConfigXml.ScalarRepCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(cStep)
            _model = cStep
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            _timeSourcePMU = signalsMgr.AllPMUs.Where(Function(x) x.PMU = _model.TimeSourcePMU).FirstOrDefault()
            Dim output = New SignalSignatureViewModel(_model.SignalName, _model.CustPMUname, _model.Type)
            output.IsCustomSignal = True
            output.Unit = Unit
            If _timeSourcePMU IsNot Nothing Then
                output.SamplingRate = _timeSourcePMU.SamplingRate
            Else
                Throw New Exception("PMU for time source is required for Scalar Repetition Customization.")
            End If
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            OutputChannels.Add(output)

            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return TimeSourcePMU IsNot Nothing
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As ReadConfigXml.ScalarRepCustModel
        Public Property Model As ReadConfigXml.ScalarRepCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As ReadConfigXml.ScalarRepCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Private _scalar As String
        Public Property Scalar As String
            Get
                Return _model.Scalar
            End Get
            Set(ByVal value As String)
                _model.Scalar = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeSourcePMU As PMUWithSamplingRate
        Public Property TimeSourcePMU As PMUWithSamplingRate
            Get
                Return _timeSourcePMU
            End Get
            Set(ByVal value As PMUWithSamplingRate)
                _timeSourcePMU = value
                'OutputChannels.FirstOrDefault.SamplingRate = GroupedRawSignalsByPMU.SelectMany(Function(x) x.SignalList).Distinct.Select(Function(y) y.SignalSignature).Where(Function(z) z.PMUName = aStep.TimeSourcePMU).Select(Function(n) n.SamplingRate).FirstOrDefault()
                If _timeSourcePMU IsNot Nothing AndAlso OutputChannels.Count > 0 Then
                    OutputChannels.FirstOrDefault.SamplingRate = _timeSourcePMU.SamplingRate
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _type As String
        Public Property Type As String
            Get
                Return _model.Type
            End Get
            Set(ByVal value As String)
                _model.Type = value
                If OutputChannels.Count > 0 AndAlso Not String.IsNullOrEmpty(_model.Type) Then
                    OutputChannels.FirstOrDefault.TypeAbbreviation = _model.Type
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _unit As String
        Public Property Unit As String
            Get
                Return _model.Unit
            End Get
            Set(ByVal value As String)
                _model.Unit = value
                If OutputChannels.Count > 0 AndAlso Not String.IsNullOrEmpty(_model.Unit) Then
                    OutputChannels.FirstOrDefault.Unit = _model.Unit
                End If
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class AdditionCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New AdditionCustModel
        End Sub
        Public Sub New(stp As AdditionCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try

            Dim type = ""
            Dim unit = ""
            Dim samplingRate = -1
            Dim output = New SignalSignatureViewModel(_model.SignalName, _model.CustPMUname, "OTHER")
            For Each signal In InputChannels
                If String.IsNullOrEmpty(type) Then
                    type = signal.TypeAbbreviation
                ElseIf type <> signal.TypeAbbreviation Then
                    Throw New Exception("All terms of addition customization have to be the same signal type! Different signal type found in addition customization step: " & stepCounter & ", with types: " & type & " and " & signal.TypeAbbreviation & ".")
                End If
                If String.IsNullOrEmpty(unit) Then
                    unit = signal.Unit
                ElseIf unit <> signal.Unit Then
                    Throw New Exception("All terms of addition customization have to have the same unit! Different unit found in addition customization step: " & stepCounter & ", with unit: " & unit & " and " & signal.Unit & ".")
                End If
                If samplingRate = -1 Then
                    samplingRate = signal.SamplingRate
                ElseIf samplingRate <> signal.SamplingRate Then
                    Throw New Exception("All terms of addition customization have to have the same sampling rate! Different sampling rate found in addition customization step: " & stepCounter & ", with sampling rate: " & samplingRate & " and " & signal.SamplingRate & ".")
                End If
            Next
            If Not String.IsNullOrEmpty(type) Then
                output.TypeAbbreviation = type
            End If
            If Not String.IsNullOrEmpty(unit) Then
                output.Unit = unit
            End If
            If samplingRate <> -1 Then
                output.SamplingRate = samplingRate
            End If
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            OutputChannels.Add(output)


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If



            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As AdditionCustModel
        Public Property Model As AdditionCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As AdditionCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class SubtractionCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New SubtractionCustModel
        End Sub
        Public Sub New(stp As SubtractionCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Minuend = signalsMgr.SearchForSignalInTaggedSignals(_model.Minuend.PMUName, _model.Minuend.SignalName)
            If Minuend Is Nothing Then
                Minuend = New SignalSignatureViewModel()
                Minuend.IsValid = False
            End If
            InputChannels.Add(Minuend)
            Subtrahend = signalsMgr.SearchForSignalInTaggedSignals(_model.Subtrahend.PMUName, _model.Subtrahend.SignalName)
            If Subtrahend Is Nothing Then
                Subtrahend = New SignalSignatureViewModel()
                Subtrahend.IsValid = False
            End If
            InputChannels.Add(Subtrahend)
            Dim output = New SignalSignatureViewModel(_model.SignalName, _model.CustPMUname)
            If Minuend.IsValid AndAlso Subtrahend.IsValid Then
                If Minuend.TypeAbbreviation = Subtrahend.TypeAbbreviation Then
                    output.TypeAbbreviation = Minuend.TypeAbbreviation
                    output.Unit = Minuend.Unit
                    output.SamplingRate = Minuend.SamplingRate
                Else
                    Throw New Exception("In step: " & stepCounter & " ," & _model.Name & ", the types of Minuend and Subtrahend do not match!")
                End If
            End If
            output.IsCustomSignal = True
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            OutputChannels.Add(output)


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As SubtractionCustModel
        Public Property Model As SubtractionCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As SubtractionCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Private _minuend As SignalSignatureViewModel
        Public Property Minuend As SignalSignatureViewModel
            Get
                Return _minuend
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                _minuend = value
                OnPropertyChanged()
            End Set
        End Property
        Private _subtrahend As SignalSignatureViewModel
        Public Property Subtrahend As SignalSignatureViewModel
            Get
                Return _subtrahend
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                _subtrahend = value
                OnPropertyChanged()
            End Set
        End Property

    End Class
    Public Class MultiplicationCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New MultiplicationCustModel
        End Sub
        Public Sub New(stp As MultiplicationCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & stp.Name)
            End Try

            Dim type = ""
            Dim unit = ""
            Dim samplingRate = -1
            Dim countNonScalarType = 0
            Dim output = New SignalSignatureViewModel(_model.SignalName, _model.CustPMUname)
            For Each signal In InputChannels
                If signal.TypeAbbreviation <> "SC" Then
                    countNonScalarType += 1
                    If String.IsNullOrEmpty(type) Then
                        type = signal.TypeAbbreviation
                    End If
                    If String.IsNullOrEmpty(unit) Then
                        unit = signal.Unit
                    End If
                End If
                If samplingRate = -1 Then
                    samplingRate = signal.SamplingRate
                ElseIf samplingRate <> signal.SamplingRate Then
                    Throw New Exception("All factors of multiplication customization have to have the same sampling rate! Different sampling rate found in multiplication customization step: " & stepCounter & ", with sampling rate: " & samplingRate & " and " & signal.SamplingRate & ".")
                End If
            Next
            If countNonScalarType = 0 Then
                output.TypeAbbreviation = "SC"
                output.Unit = "SC"
            ElseIf countNonScalarType = 1 Then
                output.TypeAbbreviation = type
                output.Unit = unit
                'TODO: ElseIf countNonScalarType == 2 AndAlso one of them is current magnitude and one of the is voltage magnitude, then we should get power P
                'TODO: Are there any other multiplication result in legal signal?
            Else
                output.TypeAbbreviation = "OTHER"
                output.Unit = "OTHER"
            End If
            output.SamplingRate = samplingRate
            output.IsCustomSignal = True
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            OutputChannels.Add(output)


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As MultiplicationCustModel
        Public Property Model As MultiplicationCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As MultiplicationCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class DivisionCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New DivisionCustModel
        End Sub
        Public Sub New(stp As DivisionCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Dividend = signalsMgr.SearchForSignalInTaggedSignals(_model.Dividend.PMUName, _model.Dividend.SignalName)
            If Dividend Is Nothing Then
                Dividend = New SignalSignatureViewModel()
                Dividend.IsValid = False
            End If
            InputChannels.Add(Dividend)
            Divisor = signalsMgr.SearchForSignalInTaggedSignals(_model.Divisor.PMUName, _model.Divisor.SignalName)
            If Divisor Is Nothing Then
                Divisor = New SignalSignatureViewModel()
                Divisor.IsValid = False
            End If
            InputChannels.Add(Divisor)
            Dim output = New SignalSignatureViewModel(_model.SignalName, _model.CustPMUname)
            If Dividend.IsValid AndAlso Divisor.IsValid Then
                If Dividend.TypeAbbreviation = Divisor.TypeAbbreviation Then
                    output.TypeAbbreviation = Dividend.TypeAbbreviation
                    output.Unit = Dividend.Unit
                    output.SamplingRate = Dividend.SamplingRate
                Else
                    Throw New Exception("In step: " & stepCounter & " ," & _model.Name & ", the types of divisor and dividend do not match!")
                End If
            End If
            output.IsCustomSignal = True
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            OutputChannels.Add(output)


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As DivisionCustModel
        Public Property Model As DivisionCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As DivisionCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Private _dividend As SignalSignatureViewModel
        Public Property Dividend As SignalSignatureViewModel
            Get
                Return _dividend
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                _dividend = value
                OnPropertyChanged()
            End Set
        End Property
        Private _divisor As SignalSignatureViewModel
        Public Property Divisor As SignalSignatureViewModel
            Get
                Return _divisor
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                _divisor = value
                OnPropertyChanged()
            End Set
        End Property

    End Class
    Public Class ExponentialCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New ExponentialCustModel
        End Sub
        Public Sub New(stp As ExponentialCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & stepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.SignalName & " in PMU " & signal.PMUName & " not found!")
                End If
                Dim output = New SignalSignatureViewModel(signal.CustSignalName, CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    If Exponent = 1 OrElse input.TypeAbbreviation = "SC" Then
                        output.TypeAbbreviation = input.TypeAbbreviation
                        output.Unit = input.Unit
                    Else
                        output.TypeAbbreviation = "OTHER"
                        output.Unit = "OTHER"
                    End If
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As ExponentialCustModel
        Public Property Model As ExponentialCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As ExponentialCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property Exponent As String
            Get
                Return _model.Exponent
            End Get
            Set(ByVal value As String)
                _model.Exponent = value
                OnPropertyChanged()
            End Set
        End Property

    End Class
    Public Class SignReversalCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New SignReversalCustModel
        End Sub
        Public Sub New(stp As SignReversalCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & stepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.SignalName & " in PMU " & signal.PMUName & " not found!")
                End If
                Dim output = New SignalSignatureViewModel(signal.CustSignalName, CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    output.TypeAbbreviation = input.TypeAbbreviation
                    output.Unit = input.Unit
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As SignReversalCustModel
        Public Property Model As SignReversalCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As SignReversalCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class AbsValCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New AbsValCustModel
        End Sub
        Public Sub New(stp As AbsValCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & stepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.SignalName & " in PMU " & signal.PMUName & " not found!")
                End If
                Dim output = New SignalSignatureViewModel(signal.CustSignalName, CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    output.TypeAbbreviation = input.TypeAbbreviation
                    output.Unit = input.Unit
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As AbsValCustModel
        Public Property Model As AbsValCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As AbsValCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class RealComponentCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New RealComponentCustModel
        End Sub
        Public Sub New(stp As RealComponentCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & stepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.SignalName & " in PMU " & signal.PMUName & " not found!")
                End If
                Dim output = New SignalSignatureViewModel(signal.CustSignalName, CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    output.TypeAbbreviation = input.TypeAbbreviation
                    output.Unit = input.Unit
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next

            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If

            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As RealComponentCustModel
        Public Property Model As RealComponentCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As RealComponentCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class ImagComponentCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New ImagComponentCustModel
        End Sub
        Public Sub New(stp As ImagComponentCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & stepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.SignalName & " in PMU " & signal.PMUName & " not found!")
                End If
                Dim output = New SignalSignatureViewModel(signal.CustSignalName, CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    output.TypeAbbreviation = input.TypeAbbreviation
                    output.Unit = input.Unit
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next

            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As ImagComponentCustModel
        Public Property Model As ImagComponentCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As ImagComponentCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class AngleCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New AngleCustModel
        End Sub
        Public Sub New(stp As AngleCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & stepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.SignalName & " in PMU " & signal.PMUName & " not found!")
                End If
                Dim output = New SignalSignatureViewModel(signal.CustSignalName, CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    output.TypeAbbreviation = input.TypeAbbreviation
                    output.Unit = input.Unit
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next

            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If

            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As AngleCustModel
        Public Property Model As AngleCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As AngleCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class ComplexConjCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New ComplexConjCustModel
        End Sub
        Public Sub New(stp As ComplexConjCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & stepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.SignalName & " in PMU " & signal.PMUName & " not found!")
                End If
                Dim output = New SignalSignatureViewModel(signal.CustSignalName, CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    output.TypeAbbreviation = input.TypeAbbreviation
                    output.Unit = input.Unit
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If

            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As ComplexConjCustModel
        Public Property Model As ComplexConjCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As ComplexConjCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class CreatePhasorCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New CreatePhasorCustModel
        End Sub
        Public Sub New(stp As CreatePhasorCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name

            For Each phasor In _model.MagAngPairList
                Dim magSignal = signalsMgr.SearchForSignalInTaggedSignals(phasor.PMUElement1.PMUName, phasor.PMUElement1.SignalName)
                If magSignal IsNot Nothing Then
                    InputChannels.Add(magSignal)
                Else
                    magSignal = New SignalSignatureViewModel("")
                    magSignal.IsValid = False
                    Throw New Exception("Error reading config file! Signal in step: " & stepCounter & ", channel name: " & phasor.PMUElement1.SignalName & " in PMU " & phasor.PMUElement1.PMUName & " not found!")
                End If
                Dim angSignal = signalsMgr.SearchForSignalInTaggedSignals(phasor.PMUElement2.PMUName, phasor.PMUElement2.SignalName)
                If angSignal IsNot Nothing Then
                    InputChannels.Add(angSignal)
                Else
                    angSignal = New SignalSignatureViewModel("")
                    angSignal.IsValid = False
                    Throw New Exception("Error reading config file! Signal in step: " & stepCounter & ", channel name: " & phasor.PMUElement2.SignalName & " in PMU " & phasor.PMUElement2.PMUName & " not found!")
                End If
                If String.IsNullOrEmpty(phasor.CustSignalName) Then
                    phasor.CustSignalName = "CustomSignalNameRequired"
                End If
                Dim output = New SignalSignatureViewModel(phasor.CustSignalName, CustPMUname, "OTHER")
                output.IsCustomSignal = True
                If magSignal.IsValid AndAlso angSignal.IsValid Then
                    Dim mtype = magSignal.TypeAbbreviation.ToArray
                    Dim atype = angSignal.TypeAbbreviation.ToArray
                    If mtype(0) = atype(0) AndAlso mtype(2) = atype(2) AndAlso mtype(1) = "M" AndAlso atype(1) = "A" Then
                        output.TypeAbbreviation = mtype(0) & "P" & mtype(2)
                    Else
                        Throw New Exception("In step: " & stepCounter & ", type of input magnitude siganl: " & magSignal.SignalName & ", does not match angle signal: " & angSignal.SignalName & ".")
                    End If
                    output.SamplingRate = magSignal.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(magSignal)
                newPair.Value.Add(angSignal)
                OutputInputMappingPair.Add(newPair)
            Next

            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As CreatePhasorCustModel
        Public Property Model As CreatePhasorCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As CreatePhasorCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class PowerCalcCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New PowerCalcCustModel
        End Sub
        Public Sub New(stp As PowerCalcCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            Dim custSignalName = ""
            Dim samplingRate = -1

            If _model.IsFromPhasor Then
                custSignalName = _model.PhasorPair.CustSignalName
                Dim Pmag = signalsMgr.SearchForSignalInTaggedSignals(_model.PhasorPair.PMUElement1.PMUName, _model.PhasorPair.PMUElement1.SignalName)
                If Pmag Is Nothing Then
                    Pmag = New SignalSignatureViewModel("SignalNotFound")
                    Pmag.IsValid = False
                    Pmag.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & _model.PhasorPair.PMUElement1.PMUName & ", and Channel: " & _model.PhasorPair.PMUElement1.SignalName & " not found!")
                Else
                    If samplingRate = -1 Then
                        samplingRate = Pmag.SamplingRate
                    End If
                End If
                InputChannels.Add(Pmag)
                Dim Pang = signalsMgr.SearchForSignalInTaggedSignals(_model.PhasorPair.PMUElement2.PMUName, _model.PhasorPair.PMUElement2.SignalName)
                If Pang Is Nothing Then
                    Pang = New SignalSignatureViewModel("SignalNotFound")
                    Pang.IsValid = False
                    Pang.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & _model.PhasorPair.PMUElement2.PMUName & ", and Channel: " & _model.PhasorPair.PMUElement2.SignalName & " not found!")
                Else
                    If samplingRate = -1 Then
                        samplingRate = Pang.SamplingRate
                    End If
                End If
                InputChannels.Add(Pang)

            Else
                custSignalName = _model.MagAngQuad.CustSignalName
                Dim Vmag = signalsMgr.SearchForSignalInTaggedSignals(_model.MagAngQuad.PMUElement1.PMUName, _model.MagAngQuad.PMUElement1.SignalName)
                If Vmag Is Nothing Then
                    Vmag = New SignalSignatureViewModel("SignalNotFound")
                    Vmag.IsValid = False
                    Vmag.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & _model.MagAngQuad.PMUElement1.PMUName & ", and Channel: " & _model.MagAngQuad.PMUElement1.SignalName & " not found!")
                Else
                    If samplingRate = -1 Then
                        samplingRate = Vmag.SamplingRate
                    End If
                End If
                InputChannels.Add(Vmag)
                Dim Vang = signalsMgr.SearchForSignalInTaggedSignals(_model.MagAngQuad.PMUElement2.PMUName, _model.MagAngQuad.PMUElement2.SignalName)
                If Vang Is Nothing Then
                    Vang = New SignalSignatureViewModel("SignalNotFound")
                    Vang.IsValid = False
                    Vang.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & _model.MagAngQuad.PMUElement2.PMUName & ", and Channel: " & _model.MagAngQuad.PMUElement2.SignalName & " not found!")
                Else
                    If samplingRate = -1 Then
                        samplingRate = Vang.SamplingRate
                    End If
                End If
                InputChannels.Add(Vang)
                Dim Imag = signalsMgr.SearchForSignalInTaggedSignals(_model.MagAngQuad.PMUElement3.PMUName, _model.MagAngQuad.PMUElement3.SignalName)
                If Imag Is Nothing Then
                    Imag = New SignalSignatureViewModel("SignalNotFound")
                    Imag.IsValid = False
                    Imag.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & _model.MagAngQuad.PMUElement3.PMUName & ", and Channel: " & _model.MagAngQuad.PMUElement3.SignalName & " not found!")
                Else
                    If samplingRate = -1 Then
                        samplingRate = Imag.SamplingRate
                    End If
                End If
                InputChannels.Add(Imag)
                Dim Iang = signalsMgr.SearchForSignalInTaggedSignals(_model.MagAngQuad.PMUElement4.PMUName, _model.MagAngQuad.PMUElement4.SignalName)
                If Iang Is Nothing Then
                    Iang = New SignalSignatureViewModel("SignalNotFound")
                    Iang.IsValid = False
                    Iang.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & _model.MagAngQuad.PMUElement4.PMUName & ", and Channel: " & _model.MagAngQuad.PMUElement4.SignalName & " not found!")
                Else
                    If samplingRate = -1 Then
                        samplingRate = Iang.SamplingRate
                    End If
                End If
                InputChannels.Add(Iang)
            End If
            If String.IsNullOrEmpty(custSignalName) Then
                custSignalName = "CustomSignalNameRequired"
            End If
            Dim typeAbbre = Me.PowType.ToString
            Dim output = New SignalSignatureViewModel(custSignalName, CustPMUname, typeAbbre)
            output.IsCustomSignal = True
            Select Case output.TypeAbbreviation
                Case "CP", "S"
                    output.Unit = "MVA"
                Case "Q"
                    output.Unit = "MVAR"
                Case "P"
                    output.Unit = "MW"
            End Select
            output.SamplingRate = samplingRate
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
            For Each signal In InputChannels
                newPair.Value.Add(signal)
            Next
            OutputInputMappingPair.Add(newPair)


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If

            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As PowerCalcCustModel
        Public Property Model As PowerCalcCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As PowerCalcCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Private _powType As PowerType
        Public Property PowType As PowerType
            Get
                Return _model.PowType
            End Get
            Set(ByVal value As PowerType)
                _model.PowType = value
                For Each out In OutputChannels
                    out.TypeAbbreviation = value.ToString
                Next
                OnPropertyChanged()
            End Set
        End Property

    End Class
    Public Class SpecifySignalTypeUnitCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New SpecTypeUnitCustModel
        End Sub

        Public Sub New(cStep As ReadConfigXml.SpecTypeUnitCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(cStep)
            _model = cStep
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name

            Dim inputSignal = signalsMgr.SearchForSignalInTaggedSignals(_model.Input.PMUName, _model.Input.SignalName)
            Dim samplingRate = -1
            If inputSignal Is Nothing Then
                inputSignal = New SignalSignatureViewModel("SignalNotFound")
                inputSignal.IsValid = False
                inputSignal.TypeAbbreviation = "C"
                Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & _model.Input.PMUName & ", and Channel: " & _model.Input.SignalName & " not found!")
            Else
                samplingRate = inputSignal.SamplingRate
            End If
            InputChannels.Add(inputSignal)
            Dim outputName = _model.SignalName
            If outputName Is Nothing Then
                outputName = inputSignal.SignalName
            End If
            Dim output = New SignalSignatureViewModel(_model.SignalName, _model.CustPMUname, _model.Type)
            output.IsCustomSignal = True
            output.Unit = _model.Unit
            output.SamplingRate = samplingRate
            output.OldSignalName = output.SignalName
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldUnit = output.Unit
            OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
            newPair.Value.Add(inputSignal)
            OutputInputMappingPair.Add(newPair)

            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If


            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As ReadConfigXml.SpecTypeUnitCustModel
        Public Property Model As ReadConfigXml.SpecTypeUnitCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As ReadConfigXml.SpecTypeUnitCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Private _type As String
        Public Property Type As String
            Get
                Return _model.Type
            End Get
            Set(ByVal value As String)
                _model.Type = value
                If OutputChannels.Count > 0 AndAlso Not String.IsNullOrEmpty(_model.Type) Then
                    OutputChannels.FirstOrDefault.TypeAbbreviation = _model.Type
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _unit As String
        Public Property Unit As String
            Get
                Return _model.Unit
            End Get
            Set(ByVal value As String)
                _model.Unit = value
                If OutputChannels.Count > 0 AndAlso Not String.IsNullOrEmpty(_model.Unit) Then
                    OutputChannels.FirstOrDefault.Unit = _model.Unit
                End If
                OnPropertyChanged()
            End Set
        End Property
    End Class
    Public Class MetricPrefixCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New MetricPrefixCustModel
            '_useCustomPMU = True
        End Sub
        Public Sub New(stp As MetricPrefixCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            '_useCustomPMU = True
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.ToConverts
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMU, signal.Channel)
                If input IsNot Nothing Then
                    InputChannels.Add(input)
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    input.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.Channel & " in PMU " & signal.PMU & " not found!")
                End If
                If signal.SignalName Is Nothing Then
                    signal.SignalName = input.SignalName
                End If
                'Dim output = input
                'If UseCustomPMU Then
                Dim output = New SignalSignatureViewModel(signal.SignalName, CustPMUname, input.TypeAbbreviation)
                output.SamplingRate = input.SamplingRate
                output.Unit = signal.NewUnit
                output.OldSignalName = output.SignalName
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldUnit = output.Unit
                'Else
                '    output.OldUnit = output.Unit
                '    output.Unit = signal.NewUnit
                'End If
                output.IsCustomSignal = True
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next

            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name & " , " & ex.Message)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If

            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name & ". Original messages: " & ex.Message)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name & ". Original messages: " & ex.Message)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As MetricPrefixCustModel
        Public Property Model As MetricPrefixCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As MetricPrefixCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        'Private _useCustomPMU As Boolean
        'Public Property UseCustomPMU As Boolean
        '    Get
        '        Return _model.UseCustomPMU
        '    End Get
        '    Set(ByVal value As Boolean)
        '        _model.UseCustomPMU = value
        '        If value Then
        '            'use custom PMU, generate new PMU and new signal as the output signal
        '            Dim newOutputInputMappingPairs = New ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
        '            OutputChannels.Clear()
        '            For Each pair In OutputInputMappingPair
        '                Dim input = pair.Value.FirstOrDefault
        '                Dim output = New SignalSignatureViewModel(input.SignalName, CustPMUname, input.TypeAbbreviation)
        '                output.Unit = input.Unit
        '                output.IsCustomSignal = True
        '                output.OldUnit = output.Unit
        '                output.SamplingRate = input.SamplingRate
        '                Dim aNewPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
        '                input.Unit = input.OldUnit
        '                aNewPair.Value.Add(input)
        '                newOutputInputMappingPairs.Add(aNewPair)
        '                OutputChannels.Add(output)
        '            Next
        '            OutputInputMappingPair = newOutputInputMappingPairs
        '            'ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(OutputChannels)
        '        Else
        '            'use current PMU, so overwrite the current signal
        '            Dim newOutputInputMappingPairs = New ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
        '            OutputChannels.Clear()
        '            For Each pair In OutputInputMappingPair
        '                Dim output = pair.Key
        '                Dim input = pair.Value.FirstOrDefault
        '                'Dim newUnit = ""
        '                If output IsNot Nothing AndAlso Not String.IsNullOrEmpty(output.Unit) Then
        '                    Dim newUnit = output.Unit
        '                    input.OldUnit = input.Unit
        '                    input.Unit = newUnit
        '                End If
        '                OutputChannels.Add(input)
        '                Dim aNewPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(input, New ObservableCollection(Of SignalSignatureViewModel))
        '                aNewPair.Value.Add(input)
        '                newOutputInputMappingPairs.Add(aNewPair)
        '            Next
        '            OutputInputMappingPair = newOutputInputMappingPairs
        '        End If
        '        OnPropertyChanged()
        '    End Set
        'End Property
        'Private _newUnit As String
        'Public Property NewUnit As String
        '    Get
        '        Return _newUnit
        '    End Get
        '    Set(ByVal value As String)
        '        _newUnit = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
    End Class
    Public Class AngleConversionCust
        Inherits Customization
        Public Sub New()
            MyBase.New
            _model = New AngleConversionCustModel
        End Sub
        Public Sub New(stp As AngleConversionCustModel, stepCounter As Integer, signalsMgr As SignalManager, Optional postProcess As Boolean = False)
            MyBase.New(stp)
            Me._model = stp
            Me.StepCounter = stepCounter
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stepCounter.ToString & " - " & Name
            For Each signal In _model.ToConverts
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMU, signal.Channel)
                If input IsNot Nothing Then
                    InputChannels.Add(input)
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    input.TypeAbbreviation = "C"
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.Channel & " in PMU " & signal.PMU & " not found!")
                End If
                If signal.SignalName Is Nothing Then
                    signal.SignalName = input.SignalName
                End If
                Dim output = New SignalSignatureViewModel(signal.SignalName, CustPMUname, input.TypeAbbreviation)
                output.SamplingRate = input.SamplingRate
                output.IsCustomSignal = True
                If input.Unit.ToLower = "deg" Then
                    output.Unit = "RAD"
                Else
                    output.Unit = "DEG"
                End If
                output.OldSignalName = output.SignalName
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldUnit = output.Unit
                OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                OutputInputMappingPair.Add(newPair)
            Next


            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by PMU in step: " & Name)
            End Try
            If postProcess Then
                signalsMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            Else
                signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            End If

            'Try
            '    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by type in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            'Try
            '    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            'Catch ex As Exception
            '    Throw New Exception("Error when sort signals by PMU in step: " & Name)
            'End Try
            'signalsMgr.GroupedSignalByDataConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As AngleConversionCustModel
        Public Property Model As AngleConversionCustModel
            Get
                Return _model
            End Get
            Set(ByVal value As AngleConversionCustModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

#End Region
End Namespace
