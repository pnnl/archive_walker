Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization

Public Class DataConfig
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Raise property changed event
    ''' </summary>
    ''' <param name="sender">The event sender</param>
    ''' <param name="e">The event</param>
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Private Sub OnPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub
    Public Sub New()
        _readerProperty = New ReaderProperties
        _dqfilterNameDictionary = New Dictionary(Of String, String) From {{"PMU Status Flags Data Quality Filter", "PMUflagFilt"},
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
        _dqfilterReverseNameDictionary = _dqfilterNameDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
        _dqFilterList = _dqfilterNameDictionary.Keys.ToList
        _customizationList = {"Scalar Repetition Customization",
                                "Addition Customization",
                                "Subtraction Customization",
                                "Multiplication Customization",
                                "Division Customization",
                                "Exponent, Sign Reversal, Absolute Value, Real Component, Imaginary Component, Angle, and Complex Conjugate Customizations",
                                "Phasor Creation Customization",
                                "Power Calculation Customization",
                                "Specify Signal Type and Unit Customization",
                                "Metric Prefix Customization",
                                "Angle Conversion Customization"}.ToList
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

    Private _collectionOfSteps As ObservableCollection(Of SignalProcessStep)
    Public Property CollectionOfSteps As ObservableCollection(Of SignalProcessStep)
        Get
            Return _collectionOfSteps
        End Get
        Set(ByVal value As ObservableCollection(Of SignalProcessStep))
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

    Private ReadOnly _dqfilterNameDictionary As Dictionary(Of String, String)
    Public ReadOnly Property DQFilterNameDictionary As Dictionary(Of String, String)
        Get
            Return _dqfilterNameDictionary
        End Get
    End Property

    Private ReadOnly _dqfilterReverseNameDictionary As Dictionary(Of String, String)
    Public ReadOnly Property DQFilterReverseNameDictionary() As Dictionary(Of String, String)
        Get
            Return _dqfilterReverseNameDictionary
        End Get
    End Property

    'Public ReadOnly Property stepList As Dictionary(Of String, List(Of String))
    '    Get
    '        Dim a As New Dictionary(Of String, List(Of String))
    '        a("Filter") = DQFilterList
    '        a("Customization") = CustomizationList
    '        Return a
    '    End Get
    'End Property

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
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Raise property changed event
    ''' </summary>
    ''' <param name="sender">The event sender</param>
    ''' <param name="e">The event</param>
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Private Sub OnPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub
    Public Sub New()
        _mode = New Dictionary(Of ModeType, Dictionary(Of String, String))
        '_modeParams = New ObservableCollection(Of ParameterValuePair)

        ArchiveModeVisibility = Visibility.Collapsed
        RealTimeModeVisibility = Visibility.Collapsed
        HybridModeVisibility = Visibility.Collapsed

        '_dateTimeStart = ""
        _selectedTimeZone = TimeZoneInfo.Local
        '_selectUTCTime = True
        '_selectAlternateTimeZone = False
    End Sub
    Private _fileDirectory As String
    Public Property FileDirectory As String
        Get
            Return _fileDirectory
        End Get
        Set(ByVal value As String)
            _fileDirectory = value
            OnPropertyChanged("FileDirectory")
        End Set
    End Property

    Private _fileType As DataFileType
    Public Property FileType As DataFileType
        Get
            Return _fileType
        End Get
        Set(ByVal value As DataFileType)
            _fileType = value
            OnPropertyChanged("FileType")
        End Set
    End Property

    Private _mnemonic As String
    Public Property Mnemonic As String
        Get
            Return _mnemonic
        End Get
        Set(ByVal value As String)
            _mnemonic = value
            OnPropertyChanged("Mnemonic")
        End Set
    End Property

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
            _changeModeParamsVisibility()
            OnPropertyChanged("ModeName")
        End Set
    End Property

    Private _realTimeModeVisibility As Visibility
    Public Property RealTimeModeVisibility As Visibility
        Get
            Return _realTimeModeVisibility
        End Get
        Set(ByVal value As Visibility)
            _realTimeModeVisibility = value
            OnPropertyChanged("RealTimeModeVisibility")
        End Set
    End Property

    Private _hybridModeVisibility As Visibility
    Public Property HybridModeVisibility As Visibility
        Get
            Return _hybridModeVisibility
        End Get
        Set(ByVal value As Visibility)
            _hybridModeVisibility = value
            OnPropertyChanged("HybridModeVisibility")
        End Set
    End Property

    Private _archiveModeVisibility As Visibility
    Public Property ArchiveModeVisibility As Visibility
        Get
            Return _archiveModeVisibility
        End Get
        Set(ByVal value As Visibility)
            _archiveModeVisibility = value
            OnPropertyChanged("ArchiveModeVisibility")
        End Set
    End Property

    Private Sub _changeModeParamsVisibility()
        'Dim newParams = New ObservableCollection(Of ParameterValuePair)
        Select Case _modeName
            Case ModeType.Archive
                ArchiveModeVisibility = Visibility.Visible
                RealTimeModeVisibility = Visibility.Collapsed
                HybridModeVisibility = Visibility.Collapsed
            Case ModeType.Hybrid
                ArchiveModeVisibility = Visibility.Collapsed
                RealTimeModeVisibility = Visibility.Collapsed
                HybridModeVisibility = Visibility.Visible
            Case ModeType.RealTime
                ArchiveModeVisibility = Visibility.Collapsed
                RealTimeModeVisibility = Visibility.Visible
                HybridModeVisibility = Visibility.Collapsed
        End Select
        'ModeParams = newParams
    End Sub

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
    'Implements INotifyPropertyChanged
    '''' <summary>
    '''' Raise property changed event
    '''' </summary>
    '''' <param name="sender">The event sender</param>
    '''' <param name="e">The event</param>
    'Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    'Private Sub OnPropertyChanged(ByVal info As String)
    '    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    'End Sub
    Public Sub New()
        _customizationParams = New ObservableCollection(Of ParameterValuePair)
    End Sub

    Private _customizationName As String
    Public Property CustomizationName As String
        Get
            Return _customizationName
        End Get
        Set(ByVal value As String)
            _customizationName = value
            OnPropertyChanged("CustomizationName")
        End Set
    End Property
    Private _name As String
    Public Overrides Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            OnPropertyChanged("Name")
        End Set
    End Property
    Private _parameters As ObservableCollection(Of ParameterValuePair)
    Public Overrides Property Parameters As ObservableCollection(Of ParameterValuePair)
        Get
            Return _parameters
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _parameters = value
            OnPropertyChanged("Parameters")
        End Set
    End Property
    Private _customizationParams As ObservableCollection(Of ParameterValuePair)
    Public Property CustomizationParams As ObservableCollection(Of ParameterValuePair)
        Get
            Return _customizationParams
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _customizationParams = value
            OnPropertyChanged("CustomizationParams")
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