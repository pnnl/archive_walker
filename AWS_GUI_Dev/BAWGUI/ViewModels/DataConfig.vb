Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization

Public Class DataConfig
    Inherits ViewModelBase
    Public Sub New()
        _readerProperty = New ReaderProperties
        _dqFilterNameDictionary = New Dictionary(Of String, String) From {{"PMU Status Flags Data Quality Filter", "PMUflagFilt"},
                                                                        {"Replaced-by-Zero Dropout Data Quality Filter", "DropOutZeroFilt"},
                                                                        {"Replaced-by-Missing Dropout Data Quality Filter", "DropOutMissingFilt"},
                                                                        {"Nominal-Value Voltage Phasor Data Quality Filter", "VoltPhasorFilt"},
                                                                        {"Nominal-Value Frequency Data Quality Filter", "FreqFilt"},
                                                                        {"Outlier Data Quality Filter", "OutlierFilt"},
                                                                        {"Stale Measurements Data Quality Filter", "StaleFilt"},
                                                                        {"Measurement Frame Data Quality Filter", "DataFrameFilt"},
                                                                        {"Entire Channel Data Quality Filter", "PMUchanFilt"},
                                                                        {"Entire PMU Data Quality Filter", "PMUallFilt"},
                                                                        {"Angle Wrapping Failure Filter", "WrappingFailureFilt"}}
        _dqFilterReverseNameDictionary = _dqFilterNameDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
        _dqFilterList = _dqFilterNameDictionary.Keys.ToList

        _dqFilterNameParametersDictionary = New Dictionary(Of String, List(Of String)) From {{"PMU Status Flags Data Quality Filter", {"SetToNaN", "FlagBit"}.ToList},
                                                                        {"Replaced-by-Zero Dropout Data Quality Filter", {"SetToNaN", "FlagBit"}.ToList},
                                                                        {"Replaced-by-Missing Dropout Data Quality Filter", {"SetToNaN", "FlagBit"}.ToList},
                                                                        {"Nominal-Value Voltage Phasor Data Quality Filter", {"SetToNaN", "FlagBit", "VoltMin", "VoltMax", "NomVoltage"}.ToList},
                                                                        {"Nominal-Value Frequency Data Quality Filter", {"SetToNaN", "FlagBit", "FreqMinChan", "FreqMaxChan", "FreqPctChan", "FreqMinSamp", "FreqMaxSamp", "FlagBitChan", "FlagBitSamp"}.ToList},
                                                                        {"Outlier Data Quality Filter", {"SetToNaN", "FlagBit", "StdDevMult"}.ToList},
                                                                        {"Stale Measurements Data Quality Filter", {"SetToNaN", "FlagBit", "StaleThresh", "FlagAllByFreq", "FlagBitFreq"}.ToList},
                                                                        {"Measurement Frame Data Quality Filter", {"SetToNaN", "FlagBit", "PercentBadThresh"}.ToList},
                                                                        {"Entire Channel Data Quality Filter", {"SetToNaN", "FlagBit", "PercentBadThresh"}.ToList},
                                                                        {"Entire PMU Data Quality Filter", {"SetToNaN", "FlagBit", "PercentBadThresh"}.ToList},
                                                                        {"Angle Wrapping Failure Filter", {"SetToNaN", "FlagBit", "AngleThresh"}.ToList}}

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
                                                                            {"Specify Signal Type and Unit Customization", {"CustPMUname", "SigType", "SigUnit", "PMU", "Channel"}.ToList},
                                                                            {"Metric Prefix Customization", {"CustPMUname", "ToConvert"}.ToList},
                                                                            {"Angle Conversion Customization", {"CustPMUname", "ToConvert"}.ToList}}

        _collectionOfSteps = New ObservableCollection(Of Object)
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

    Private _collectionOfSteps As ObservableCollection(Of Object)
    Public Property CollectionOfSteps As ObservableCollection(Of Object)
        Get
            Return _collectionOfSteps
        End Get
        Set(ByVal value As ObservableCollection(Of Object))
            _collectionOfSteps = value
            OnPropertyChanged("CollectionOfSteps")
        End Set
    End Property

    Private ReadOnly _dqFilterList As List(Of String)
    Public ReadOnly Property DQFilterList As List(Of String)
        Get
            Return _dqFilterList
        End Get
    End Property
    Private ReadOnly _customizationList As List(Of String)
    Public ReadOnly Property CustomizationList As List(Of String)
        Get
            Return _customizationList
        End Get
    End Property

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

Public Enum DataFileType
    <Description("PDAT")>
    pdat
    <Description("JSIS CSV")>
    csv
End Enum

Public Enum ModeType
    Archive
    <Description("Real Time")>
    RealTime
    Hybrid
End Enum

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''Class ReaderProperties'''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''' <summary>
''' 
''' </summary>
Public Class ReaderProperties
    Inherits ViewModelBase
    Public Sub New()
        _mode = New Dictionary(Of ModeType, Dictionary(Of String, String))
        '_modeParams = New ObservableCollection(Of ParameterValuePair)

        'ArchiveModeVisibility = Visibility.Collapsed
        'RealTimeModeVisibility = Visibility.Collapsed
        'HybridModeVisibility = Visibility.Collapsed

        '_dateTimeStart = ""
        _selectedTimeZone = TimeZoneInfo.Local

        _inputFileInfos = New ObservableCollection(Of InputFileInfo)
    End Sub

    Private _inputFileInfos As ObservableCollection(Of InputFileInfo)
    Public Property InputFileInfos As ObservableCollection(Of InputFileInfo)
        Get
            Return _inputFileInfos
        End Get
        Set(ByVal value As ObservableCollection(Of InputFileInfo))
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

    Private _mode As Dictionary(Of ModeType, Dictionary(Of String, String))
    Public Property Mode As Dictionary(Of ModeType, Dictionary(Of String, String))
        Get
            Return _mode
        End Get
        Set(ByVal value As Dictionary(Of ModeType, Dictionary(Of String, String)))
            _mode = value
            OnPropertyChanged("Mode")
        End Set
    End Property

    Private _modeName As ModeType
    Public Property ModeName As ModeType
        Get
            Return _modeName
        End Get
        Set(ByVal value As ModeType)
            'Can set up a dictionary with ModeName as key, and ModeParams as value to save the old values
            'Then test the new value to see if it exists in the dictionary
            'If yes, set ModeParams to the old values
            'If not, call _constructParamTable()
            'This way we can show old mode parameters when switch among different mode name.
            _modeName = value
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
            Return _dateTimeStart
        End Get
        Set(ByVal value As String)
            _dateTimeStart = value
            'If SelectUTCTime Then
            _convertStartTimeToSelectedTimeZone()
            'End If
            OnPropertyChanged("DateTimeStart")
        End Set
    End Property

    Private Sub _convertStartTimeToSelectedTimeZone()
        'If SelectUTCTime Then
        'Dim TimeZone As TimeZoneInfo = DirectCast(SelectedTimeZone, TimeZoneInfo)
        If DateTimeStart IsNot Nothing Then
            Dim UTCTimeStart = DateTime.Parse(DateTimeStart, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
            Dim newConvertedStartTime = TimeZoneInfo.ConvertTimeFromUtc(UTCTimeStart, SelectedTimeZone)
            If newConvertedStartTime.ToString <> ConvertedStartTime Then
                ConvertedStartTime = newConvertedStartTime
            End If
        End If
        'Else
        '    Throw New Exception("Error! Bad settings in start time or converted start time.")
        'End If
    End Sub

    Private Sub _convertEndTimeToSelectedTimeZone()
        If DateTimeEnd IsNot Nothing Then
            Dim UTCTimeEnd = DateTime.Parse(DateTimeEnd, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
            Dim newConvertedEndTime = TimeZoneInfo.ConvertTimeFromUtc(UTCTimeEnd, SelectedTimeZone)
            If newConvertedEndTime.ToString <> ConvertedEndTime Then
                ConvertedEndTime = newConvertedEndTime
            End If
        End If
    End Sub

    Private _dateTimeEnd As String
    Public Property DateTimeEnd As String
        Get
            Return _dateTimeEnd
        End Get
        Set(ByVal value As String)
            _dateTimeEnd = value
            _convertEndTimeToSelectedTimeZone()
            OnPropertyChanged("DateTimeEnd")
        End Set
    End Property

    Private _selectedTimeZone As TimeZoneInfo
    Public Property SelectedTimeZone As TimeZoneInfo
        Get
            Return _selectedTimeZone
        End Get
        Set(ByVal value As TimeZoneInfo)
            _selectedTimeZone = value
            'If SelectAlternateTimeZone Then
            _convertStartTimeToUTCTime()
            _convertEndTimeToUTCTime()
            'ElseIf SelectUTCTime Then
            '    _convertTimeToSelectedTimeZone()
            'End If
            OnPropertyChanged("SelectedTimeZone")
        End Set
    End Property

    Private _convertedStartTime As String
    Public Property ConvertedStartTime As String
        Get
            Return _convertedStartTime
        End Get
        Set(ByVal value As String)
            _convertedStartTime = value
            'If SelectAlternateTimeZone Then
            _convertStartTimeToUTCTime()
            'End If
            OnPropertyChanged("ConvertedStartTime")
        End Set
    End Property

    Private Sub _convertStartTimeToUTCTime()
        'Dim TimeZone As TimeZoneInfo = DirectCast(SelectedTimeZone, TimeZoneInfo)
        If ConvertedStartTime IsNot Nothing Then
            Dim localTimeStart = DateTime.Parse(ConvertedStartTime)
            Dim newDateTimeStart = TimeZoneInfo.ConvertTimeToUtc(localTimeStart, SelectedTimeZone)
            If newDateTimeStart.ToString <> DateTimeStart Then
                DateTimeStart = newDateTimeStart
            End If
        End If
    End Sub

    Private Sub _convertEndTimeToUTCTime()
        If ConvertedEndTime IsNot Nothing Then
            Dim localTimeEnd = DateTime.Parse(ConvertedEndTime)
            Dim newDateTimeEnd = TimeZoneInfo.ConvertTimeToUtc(localTimeEnd, SelectedTimeZone)
            If newDateTimeEnd.ToString <> DateTimeEnd Then
                DateTimeEnd = newDateTimeEnd
            End If
        End If
    End Sub

    Private _convertedEndTime As String
    Public Property ConvertedEndTime As String
        Get
            Return _convertedEndTime
        End Get
        Set(ByVal value As String)
            _convertedEndTime = value
            _convertEndTimeToUTCTime()
            OnPropertyChanged("ConvertedEndTime")
        End Set
    End Property

    Private _noFutureWait As String
    Public Property NoFutureWait As String
        Get
            Return _noFutureWait
        End Get
        Set(ByVal value As String)
            _noFutureWait = value
            OnPropertyChanged("NoFutureWait")
        End Set
    End Property

    Private _maxNoFutureCount As String
    Public Property MaxNoFutureCount As String
        Get
            Return _maxNoFutureCount
        End Get
        Set(ByVal value As String)
            _maxNoFutureCount = value
            OnPropertyChanged("MaxNoFutureCount")
        End Set
    End Property

    Private _futureWait As String
    Public Property FutureWait As String
        Get
            Return _futureWait
        End Get
        Set(ByVal value As String)
            _futureWait = value
            OnPropertyChanged("FutureWait")
        End Set
    End Property

    Private _maxFutureCount As String
    Public Property MaxFutureCount As String
        Get
            Return _maxFutureCount
        End Get
        Set(ByVal value As String)
            _maxFutureCount = value
            OnPropertyChanged("MaxFutureCount")
        End Set
    End Property

    Private _realTimeRange As String
    Public Property RealTimeRange As String
        Get
            Return _realTimeRange
        End Get
        Set(ByVal value As String)
            _realTimeRange = value
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

    Private _selectUTCTime As Boolean
    Public Property SelectUTCTime As Boolean
        Get
            Return _selectUTCTime
        End Get
        Set(ByVal value As Boolean)
            _selectUTCTime = value
            If _selectUTCTime Then
                SelectAlternateTimeZone = False
            Else
                SelectAlternateTimeZone = True
            End If
            OnPropertyChanged("SelectUTCTime")
        End Set
    End Property

    Private _selectAlternateTimeZone As Boolean
    Public Property SelectAlternateTimeZone As Boolean
        Get
            Return _selectAlternateTimeZone
        End Get
        Set(ByVal value As Boolean)
            _selectAlternateTimeZone = value
            If _selectAlternateTimeZone Then
                SelectUTCTime = False
            Else
                SelectUTCTime = True
            End If
            OnPropertyChanged("SelectAlternateTimeZone")
        End Set
    End Property

End Class
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''Class InputFileInfo''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class InputFileInfo
    Inherits ViewModelBase
    Public Sub New()
        _inputFileTree = New ObservableCollection(Of Folder)
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
    Private _taggedSignals As ObservableCollection(Of SignalSignatures)
    Public Property TaggedSignals As ObservableCollection(Of SignalSignatures)
        Get
            Return _taggedSignals
        End Get
        Set(ByVal value As ObservableCollection(Of SignalSignatures))
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
Public Class DQFilter
    Inherits Filter

    Public Sub New()
        MyBase.New
    End Sub

    Private _PMUs As ObservableCollection(Of PMU)
    Public Property PMUs As ObservableCollection(Of PMU)
        Get
            Return _PMUs
        End Get
        Set(ByVal value As ObservableCollection(Of PMU))
            _PMUs = value
            OnPropertyChanged("PMUs")
        End Set
    End Property



End Class

Public Class Customization
    Inherits SignalProcessStep
    Public Sub New()
        '_customizationParams = New ObservableCollection(Of ParameterValuePair)
        _parameters = New ObservableCollection(Of ParameterValuePair)
        InputChannels = New ObservableCollection(Of SignalSignatures)
        OutputChannels = New ObservableCollection(Of SignalSignatures)
        ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
        ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
        _custSignalName = New ObservableCollection(Of String)
        _outputInputMappingDictionary = New ObservableCollection(Of KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures)))
    End Sub

    Private _customizationName As String
    Public Property CustomizationName As String
        Get
            Return _customizationName
        End Get
        Set(ByVal value As String)
            _customizationName = value
            OnPropertyChanged()
        End Set
    End Property
    Private _name As String
    Public Overrides Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            OnPropertyChanged()
        End Set
    End Property
    Private _parameters As ObservableCollection(Of ParameterValuePair)
    Public Overrides Property Parameters As ObservableCollection(Of ParameterValuePair)
        Get
            Return _parameters
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _parameters = value
            OnPropertyChanged()
        End Set
    End Property
    Private _customizationParams As ObservableCollection(Of ParameterValuePair)
    Public Property CustomizationParams As ObservableCollection(Of ParameterValuePair)
        Get
            Return _customizationParams
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _customizationParams = value
            OnPropertyChanged()
        End Set
    End Property
    Private _minuendOrDivident As SignalSignatures
    Public Property MinuendOrDivident As SignalSignatures
        Get
            Return _minuendOrDivident
        End Get
        Set(ByVal value As SignalSignatures)
            _minuendOrDivident = value
            OnPropertyChanged()
        End Set
    End Property
    Private _subtrahendOrDivisor As SignalSignatures
    Public Property SubtrahendOrDivisor As SignalSignatures
        Get
            Return _subtrahendOrDivisor
        End Get
        Set(ByVal value As SignalSignatures)
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
            Return _custPMUname
        End Get
        Set(ByVal value As String)
            _custPMUname = value
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
    Private _custSignalName As ObservableCollection(Of String)
    Public Property CustSignalName As ObservableCollection(Of String)
        Get
            Return _custSignalName
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            _custSignalName = value
            OnPropertyChanged()
        End Set
    End Property
    Private _outputChannels As ObservableCollection(Of SignalSignatures)
    Public Property OutputChannels As ObservableCollection(Of SignalSignatures)
        Get
            Return _outputChannels
        End Get
        Set(ByVal value As ObservableCollection(Of SignalSignatures))
            _outputChannels = value
            OnPropertyChanged()
        End Set
    End Property
    Private _thisStepOutputsAsSignalHierachyByPMU As SignalTypeHierachy
    Public Property ThisStepOutputsAsSignalHierachyByPMU As SignalTypeHierachy
        Get
            Return _thisStepOutputsAsSignalHierachyByPMU
        End Get
        Set(ByVal value As SignalTypeHierachy)
            _thisStepOutputsAsSignalHierachyByPMU = value
            OnPropertyChanged()
        End Set
    End Property
    Private _outputInputMappingDictionary As ObservableCollection(Of KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures)))
    Public Property OutputInputMappingDictionary As ObservableCollection(Of KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures)))
        Get
            Return _outputInputMappingDictionary
        End Get
        Set(ByVal value As ObservableCollection(Of KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))))
            _outputInputMappingDictionary = value
            OnPropertyChanged()
        End Set
    End Property
End Class

'Public Class DataConfigStage
'    Implements INotifyPropertyChanged
'    ''' <summary>
'    ''' Raise property changed event
'    ''' </summary>
'    ''' <param name="sender">The event sender</param>
'    ''' <param name="e">The event</param>
'    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
'    Private Sub OnPropertyChanged(ByVal info As String)
'        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
'    End Sub
'    Public Sub New()

'    End Sub

'    Private _filters As ObservableCollection(Of DQFilter)
'    Public Property Filters As ObservableCollection(Of DQFilter)
'        Get
'            Return _filters
'        End Get
'        Set(ByVal value As ObservableCollection(Of DQFilter))
'            _filters = value
'            OnPropertyChanged("Filters")
'        End Set
'    End Property

'    Private _customizations As ObservableCollection(Of Customization)
'    Public Property Customizations As ObservableCollection(Of Customization)
'        Get
'            Return _customizations
'        End Get
'        Set(ByVal value As ObservableCollection(Of Customization))
'            _customizations = value
'            OnPropertyChanged("Customizations")
'        End Set
'    End Property

'End Class