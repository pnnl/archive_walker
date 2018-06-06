Imports System.Collections.ObjectModel
Imports BAWGUI.Core
Imports BAWGUI.Settings.ViewModel
Imports BAWGUI.Settings.ViewModels

Namespace Model
    Public Class DetectorConfig
        Inherits ViewModelBase
        Public Sub New()
            _detectorList = New ObservableCollection(Of Object)
            _alarmingList = New ObservableCollection(Of AlarmingDetectorBase)
            _detectorNameList = New List(Of String) From {"Periodogram Forced Oscillation Detector",
                                                          "Spectral Coherence Forced Oscillation Detector",
                                                          "Ringdown Detector",
                                                          "Out-of-Range Detector",
                                                          "Wind Ramp Detector",
                                                          "Voltage Stability"}
            _alarmingDetectorNameList = New List(Of String) From {"Periodogram Forced Oscillation Detector",
                                                                  "Spectral Coherence Forced Oscillation Detector",
                                                                  "Ringdown Detector"}
        End Sub
        Private _eventPath As String
        Public Property EventPath As String
            Get
                Return _eventPath
            End Get
            Set(ByVal value As String)
                _eventPath = value
                OnPropertyChanged()
            End Set
        End Property
        Private _resultUpdateInterval As String
        Public Property ResultUpdateInterval As String
            Get
                Return _resultUpdateInterval
            End Get
            Set(ByVal value As String)
                _resultUpdateInterval = value
                OnPropertyChanged()
            End Set
        End Property
        Private _detectorList As ObservableCollection(Of Object)
        Public Property DetectorList As ObservableCollection(Of Object)
            Get
                Return _detectorList
            End Get
            Set(ByVal value As ObservableCollection(Of Object))
                _detectorList = value
                OnPropertyChanged()
            End Set
        End Property
        Private _alarmingList As ObservableCollection(Of AlarmingDetectorBase)
        Public Property AlarmingList As ObservableCollection(Of AlarmingDetectorBase)
            Get
                Return _alarmingList
            End Get
            Set(ByVal value As ObservableCollection(Of AlarmingDetectorBase))
                _alarmingList = value
                OnPropertyChanged()
            End Set
        End Property
        Private _detectorNameList As List(Of String)
        Public Property DetectorNameList As List(Of String)
            Get
                Return _detectorNameList
            End Get
            Set(ByVal value As List(Of String))
                _detectorNameList = value
                OnPropertyChanged()
            End Set
        End Property
        Private _alarmingDetectorNameList As List(Of String)
        Public Property AlarmingDetectorNameList As List(Of String)
            Get
                Return _alarmingDetectorNameList
            End Get
            Set(ByVal value As List(Of String))
                _alarmingDetectorNameList = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class PeriodogramDetector
        Inherits DetectorBase
        Public Sub New()
            _pfa = "0.01"
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
            _analysisLength = 600
            _windowType = DetectorWindowType.hann
            _windowLength = 200
            _windowOverlap = 100
            _pfa = "0.001"
            _frequencyMin = "0.1"
            _frequencyMax = "15"
            _frequencyTolerance = "0.05"
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Periodogram Forced Oscillation Detector"
            End Get
        End Property
        Private _mode As DetectorModeType
        Public Property Mode As DetectorModeType
            Get
                Return _mode
            End Get
            Set(ByVal value As DetectorModeType)
                _mode = value
                OnPropertyChanged()
            End Set
        End Property
        Private _analysisLength As Integer
        Public Property AnalysisLength As Integer
            Get
                Return _analysisLength
            End Get
            Set(ByVal value As Integer)
                _analysisLength = value
                WindowLength = Math.Floor(value / 3)
                OnPropertyChanged()
            End Set
        End Property
        Private _windowType As DetectorWindowType
        Public Property WindowType As DetectorWindowType
            Get
                Return _windowType
            End Get
            Set(ByVal value As DetectorWindowType)
                _windowType = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyInterval As String
        Public Property FrequencyInterval As String
            Get
                Return _frequencyInterval
            End Get
            Set(ByVal value As String)
                _frequencyInterval = value
                OnPropertyChanged()
            End Set
        End Property
        Private _windowLength As Integer
        Public Property WindowLength As Integer
            Get
                Return _windowLength
            End Get
            Set(ByVal value As Integer)
                _windowLength = value
                WindowOverlap = Math.Floor(value / 2)
                OnPropertyChanged()
            End Set
        End Property
        Private _windowOverlap As Integer
        Public Property WindowOverlap As Integer
            Get
                Return _windowOverlap
            End Get
            Set(ByVal value As Integer)
                _windowOverlap = value
                OnPropertyChanged()
            End Set
        End Property
        Private _medianFilterFrequencyWidth As String
        Public Property MedianFilterFrequencyWidth As String
            Get
                Return _medianFilterFrequencyWidth
            End Get
            Set(ByVal value As String)
                _medianFilterFrequencyWidth = value
                OnPropertyChanged()
            End Set
        End Property
        Private _pfa As String
        Public Property Pfa As String
            Get
                Return _pfa
            End Get
            Set(ByVal value As String)
                _pfa = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMin As String
        Public Property FrequencyMin As String
            Get
                Return _frequencyMin
            End Get
            Set(ByVal value As String)
                _frequencyMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMax As String
        Public Property FrequencyMax As String
            Get
                Return _frequencyMax
            End Get
            Set(ByVal value As String)
                _frequencyMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyTolerance As String
        Public Property FrequencyTolerance As String
            Get
                Return _frequencyTolerance
            End Get
            Set(ByVal value As String)
                _frequencyTolerance = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class SpectralCoherenceDetector
        Inherits DetectorBase
        Public Sub New()
            _analysisLength = 60
            _delay = 10
            _numberDelays = 2
            _thresholdScale = 3
            _windowType = DetectorWindowType.hann
            _windowLength = 12
            _windowOverlap = 6
            _frequencyMin = "0.1"
            _frequencyMax = "15"
            _frequencyTolerance = "0.05"
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
            '_windowLength = _analysisLength / 5
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Spectral Coherence Forced Oscillation Detector"
            End Get
        End Property
        Private _mode As DetectorModeType
        Public Property Mode As DetectorModeType
            Get
                Return _mode
            End Get
            Set(ByVal value As DetectorModeType)
                _mode = value
                OnPropertyChanged()
            End Set
        End Property
        Private _analysisLength As Integer
        Public Property AnalysisLength() As Integer
            Get
                Return _analysisLength
            End Get
            Set(ByVal value As Integer)
                _analysisLength = value
                'Delay = _analysisLength / 10
                WindowLength = Math.Floor(value / 5)
                OnPropertyChanged()
            End Set
        End Property
        Private _delay As Double
        Public Property Delay As Double
            Get
                Return _delay
            End Get
            Set(ByVal value As Double)
                _delay = value
                OnPropertyChanged()
            End Set
        End Property
        Private _numberDelays As Integer
        Public Property NumberDelays As Integer
            Get
                Return _numberDelays
            End Get
            Set(ByVal value As Integer)
                _numberDelays = value
                If _numberDelays < 2 Then
                    _numberDelays = 2
                    Throw New Exception("Error! Number of delays in Spectral Coherence detector must be an integer greater than or equal to 2.")
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _thresholdScale As Integer
        Public Property ThresholdScale As Integer
            Get
                Return _thresholdScale
            End Get
            Set(ByVal value As Integer)
                _thresholdScale = value
                If _thresholdScale <= 1 Then
                    _thresholdScale = 3
                    Throw New Exception("Error! ThresholdScale in Spectral Coherence detector must be greater than1.")
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _windowType As DetectorWindowType
        Public Property WindowType As DetectorWindowType
            Get
                Return _windowType
            End Get
            Set(ByVal value As DetectorWindowType)
                _windowType = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyInterval As String
        Public Property FrequencyInterval As String
            Get
                Return _frequencyInterval
            End Get
            Set(ByVal value As String)
                _frequencyInterval = value
                OnPropertyChanged()
            End Set
        End Property
        Private _windowLength As Integer
        Public Property WindowLength As Integer
            Get
                Return _windowLength
            End Get
            Set(ByVal value As Integer)
                _windowLength = value
                WindowOverlap = Math.Floor(value / 2)
                OnPropertyChanged()
            End Set
        End Property
        Private _windowOverlap As Integer
        Public Property WindowOverlap As Integer
            Get
                Return _windowOverlap
            End Get
            Set(ByVal value As Integer)
                _windowOverlap = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMin As String
        Public Property FrequencyMin As String
            Get
                Return _frequencyMin
            End Get
            Set(ByVal value As String)
                _frequencyMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMax As String
        Public Property FrequencyMax As String
            Get
                Return _frequencyMax
            End Get
            Set(ByVal value As String)
                _frequencyMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyTolerance As String
        Public Property FrequencyTolerance As String
            Get
                Return _frequencyTolerance
            End Get
            Set(ByVal value As String)
                _frequencyTolerance = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class RingdownDetector
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
            RMSlength = "15"
            RMSmedianFilterTime = "120"
            RingThresholdScale = "3"
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Ringdown Detector"
            End Get
        End Property
        Private _rmsLength As String
        Public Property RMSlength As String
            Get
                Return _rmsLength
            End Get
            Set(ByVal value As String)
                _rmsLength = value
                OnPropertyChanged()
            End Set
        End Property
        'Private _maxDuration As String
        'Public Property MaxDuration As String
        '    Get
        '        Return _maxDuration
        '    End Get
        '    Set(ByVal value As String)
        '        _maxDuration = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        'Private _forgetFactor As String
        'Public Property ForgetFactor As String
        '    Get
        '        Return _forgetFactor
        '    End Get
        '    Set(ByVal value As String)
        '        _forgetFactor = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        Private _rmsmedianFilterTime As String
        Public Property RMSmedianFilterTime As String
            Get
                Return _rmsmedianFilterTime
            End Get
            Set(ByVal value As String)
                _rmsmedianFilterTime = value
                OnPropertyChanged()
            End Set
        End Property
        Private _ringThresholdScale As String
        Public Property RingThresholdScale As String
            Get
                Return _ringThresholdScale
            End Get
            Set(ByVal value As String)
                _ringThresholdScale = value
                OnPropertyChanged()
            End Set
        End Property
        Private _maxDuration As String
        Public Property MaxDuration As String
            Get
                Return _maxDuration
            End Get
            Set(ByVal value As String)
                _maxDuration = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class OutOfRangeGeneralDetector
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Out-Of-Range Detector"
            End Get
        End Property
        Private _max As Double
        Public Property Max As Double
            Get
                Return _max
            End Get
            Set(ByVal value As Double)
                _max = value
                OnPropertyChanged()
            End Set
        End Property
        Private _min As Double
        Public Property Min As Double
            Get
                Return _min
            End Get
            Set(ByVal value As Double)
                _min = value
                OnPropertyChanged()
            End Set
        End Property
        Private _duration As String
        Public Property Duration As String
            Get
                Return _duration
            End Get
            Set(ByVal value As String)
                _duration = value
                OnPropertyChanged()
            End Set
        End Property
        Private _analysisWindow As String
        Public Property AnalysisWindow As String
            Get
                Return _analysisWindow
            End Get
            Set(ByVal value As String)
                _analysisWindow = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class OutOfRangeFrequencyDetector
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Out-Of-Range Detector"
            End Get
        End Property
        Private _type As OutOfRangeFrequencyDetectorType
        Public Property Type As OutOfRangeFrequencyDetectorType
            Get
                Return _type
            End Get
            Set(ByVal value As OutOfRangeFrequencyDetectorType)
                _type = value
                OnPropertyChanged()
            End Set
        End Property
        Private _averageWindow As String
        Public Property AverageWindow As String
            Get
                Return _averageWindow
            End Get
            Set(ByVal value As String)
                _averageWindow = value
                OnPropertyChanged()
            End Set
        End Property
        Private _nominal As String
        Public Property Nominal As String
            Get
                Return _nominal
            End Get
            Set(ByVal value As String)
                _nominal = value
                OnPropertyChanged()
            End Set
        End Property
        ''' <summary>
        ''' This value holds either the nominal or averageWindow value depends on the selected baseline creation type
        ''' </summary>
        'Private _baselineCreationValue As String
        'Public Property BaselineCreationValue As String
        '    Get
        '        Return _baselineCreationValue
        '    End Get
        '    Set(ByVal value As String)
        '        _baselineCreationValue = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        Private _durationMax As String
        Public Property DurationMax As String
            Get
                Return _durationMax
            End Get
            Set(ByVal value As String)
                _durationMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _durationMin As String
        Public Property DurationMin As String
            Get
                Return _durationMin
            End Get
            Set(ByVal value As String)
                _durationMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _duration As String
        Public Property Duration As String
            Get
                Return _duration
            End Get
            Set(ByVal value As String)
                _duration = value
                OnPropertyChanged()
            End Set
        End Property
        Private _analysisWindow As String
        Public Property AnalysisWindow As String
            Get
                Return _analysisWindow
            End Get
            Set(ByVal value As String)
                _analysisWindow = value
                OnPropertyChanged()
            End Set
        End Property
        Private _rateOfChangeMax As String
        Public Property RateOfChangeMax As String
            Get
                Return _rateOfChangeMax
            End Get
            Set(ByVal value As String)
                _rateOfChangeMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _rateOfChangeMin As String
        Public Property RateOfChangeMin As String
            Get
                Return _rateOfChangeMin
            End Get
            Set(ByVal value As String)
                _rateOfChangeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _rateOfChange As String
        Public Property RateOfChange As String
            Get
                Return _rateOfChange
            End Get
            Set(ByVal value As String)
                _rateOfChange = value
                OnPropertyChanged()
            End Set
        End Property
        Private _eventMergeWindow As String
        Public Property EventMergeWindow As String
            Get
                Return _eventMergeWindow
            End Get
            Set(ByVal value As String)
                _eventMergeWindow = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class WindRampDetector
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
            _isLongTrend = True
            _apass = "1"
            _astop = "60"
            _fpass = "0.00005"
            _fstop = "0.0002"
            _valMin = "400"
            _valMax = "1000"
            _timeMin = "14400"
            _timeMax = "45000"
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Wind Ramp Detector"
            End Get
        End Property
        ''' <summary>
        ''' if false, is short trend
        ''' </summary>
        Private _isLongTrend As Boolean
        Public Property IsLongTrend As Boolean
            Get
                Return _isLongTrend
            End Get
            Set(ByVal value As Boolean)
                _isLongTrend = value
                If value Then
                    _fpass = "0.00005"
                    _fstop = "0.0002"
                    ValMin = "400"
                    ValMax = "1000"
                    TimeMin = "14400"
                    TimeMax = "45000"
                Else
                    _fpass = "0.03"
                    _fstop = "0.05"
                    ValMin = "50"
                    ValMax = "300"
                    TimeMin = "30"
                    TimeMax = "300"
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _fpass As String
        Public Property Fpass As String
            Get
                Return _fpass
            End Get
            Set(ByVal value As String)
                _fpass = value
                OnPropertyChanged()
            End Set
        End Property
        Private _fstop As String
        Public Property Fstop As String
            Get
                Return _fstop
            End Get
            Set(ByVal value As String)
                _fstop = value
                OnPropertyChanged()
            End Set
        End Property
        Private _apass As String
        Public Property Apass As String
            Get
                Return _apass
            End Get
            Set(ByVal value As String)
                _apass = value
                OnPropertyChanged()
            End Set
        End Property
        Private _astop As String
        Public Property Astop As String
            Get
                Return _astop
            End Get
            Set(ByVal value As String)
                _astop = value
                OnPropertyChanged()
            End Set
        End Property
        Private _valMin As String
        Public Property ValMin As String
            Get
                Return _valMin
            End Get
            Set(ByVal value As String)
                _valMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _valMax As String
        Public Property ValMax As String
            Get
                Return _valMax
            End Get
            Set(ByVal value As String)
                _valMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMin As String
        Public Property TimeMin As String
            Get
                Return _timeMin
            End Get
            Set(ByVal value As String)
                _timeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMax As String
        Public Property TimeMax As String
            Get
                Return _timeMax
            End Get
            Set(ByVal value As String)
                _timeMax = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Enum DetectorModeType
        <ComponentModel.Description("Single Channel")>
        SingleChannel
        <ComponentModel.Description("Multichannel")>
        MultiChannel
    End Enum

    Public Enum DetectorWindowType
        <ComponentModel.Description("Hann")>
        hann
        <ComponentModel.Description("Rectangular")>
        rectwin
        <ComponentModel.Description("Bartlett")>
        bartlett
        <ComponentModel.Description("Hamming")>
        hamming
        <ComponentModel.Description("Blackman")>
        blackman
    End Enum

    Public Enum OutOfRangeFrequencyDetectorType
        <ComponentModel.Description("Nominal Value")>
        Nominal
        <ComponentModel.Description("History for Baseline (seconds)")>
        AvergeWindow
    End Enum

    Public Class AlarmingSpectralCoherence
        Inherits AlarmingDetectorBase
        Public Sub New()
            IsExpanded = False
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Spectral Coherence Forced Oscillation Detector"
            End Get
        End Property
        Private _coherenceAlarm As String
        Public Property CoherenceAlarm As String
            Get
                Return _coherenceAlarm
            End Get
            Set(ByVal value As String)
                _coherenceAlarm = value
                OnPropertyChanged()
            End Set
        End Property
        Private _coherenceMin As String
        Public Property CoherenceMin As String
            Get
                Return _coherenceMin
            End Get
            Set(ByVal value As String)
                _coherenceMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMin As String
        Public Property TimeMin As String
            Get
                Return _timeMin
            End Get
            Set(ByVal value As String)
                _timeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _coherenceCorner As String
        Public Property CoherenceCorner As String
            Get
                Return _coherenceCorner
            End Get
            Set(ByVal value As String)
                _coherenceCorner = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeCorner As String
        Public Property TimeCorner As String
            Get
                Return _timeCorner
            End Get
            Set(ByVal value As String)
                _timeCorner = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class AlarmingPeriodogram
        Inherits AlarmingDetectorBase
        Public Sub New()
            IsExpanded = False
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Periodogram Forced Oscillation Detector"
            End Get
        End Property
        Private _snrAlarm As String
        Public Property SNRalarm As String
            Get
                Return _snrAlarm
            End Get
            Set(ByVal value As String)
                _snrAlarm = value
                OnPropertyChanged()
            End Set
        End Property
        Private _snrMin As String
        Public Property SNRmin As String
            Get
                Return _snrMin
            End Get
            Set(ByVal value As String)
                _snrMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMin As String
        Public Property TimeMin As String
            Get
                Return _timeMin
            End Get
            Set(ByVal value As String)
                _timeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _snrCorner As String
        Public Property SNRcorner As String
            Get
                Return _snrCorner
            End Get
            Set(ByVal value As String)
                _snrCorner = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeCorner As String
        Public Property TimeCorner As String
            Get
                Return _timeCorner
            End Get
            Set(ByVal value As String)
                _timeCorner = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class AlarmingRingdown
        Inherits AlarmingDetectorBase
        Public Sub New()
            IsExpanded = False
        End Sub
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Ringdown Detector"
            End Get
        End Property
        Private _maxDuration As String
        Public Property MaxDuration As String
            Get
                Return _maxDuration
            End Get
            Set(ByVal value As String)
                _maxDuration = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
End Namespace
