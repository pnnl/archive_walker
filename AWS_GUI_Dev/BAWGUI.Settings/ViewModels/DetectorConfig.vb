Imports System.Collections.ObjectModel
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Input
Imports BAWGUI.Core
Imports BAWGUI.Core.Models
Imports BAWGUI.ReadConfigXml
Imports BAWGUI.SignalManagement.ViewModels
Imports BAWGUI.Utilities
Imports Microsoft.WindowsAPICodePack.Dialogs

Namespace ViewModels
    Public Class DetectorConfig
        Inherits ViewModelBase
        Public Sub New()
            _detectorList = New ObservableCollection(Of DetectorBase)
            _dataWriterDetectorList = New ObservableCollection(Of DetectorBase)
            _alarmingList = New ObservableCollection(Of AlarmingDetectorBase)
            _model = New DetectorConfigModel()
            _resultUpdateIntervalVisibility = Visibility.Collapsed
            _detectorNameList = New List(Of String) From {"Periodogram Forced Oscillation Detector",
                                                          "Spectral Coherence Forced Oscillation Detector",
                                                          "Ringdown Detector",
                                                          "Out-of-Range Detector",
                                                          "Wind Ramp Detector",
                                                          "Voltage Stability",
                                                          "Mode Meter Tool"}
            _alarmingDetectorNameList = New List(Of String) From {"Periodogram Forced Oscillation Detector",
                                                                  "Spectral Coherence Forced Oscillation Detector",
                                                                  "Ringdown Detector"}
            '_addDataWriterDetector = New DelegateCommand(AddressOf _addADataWriterDetector, AddressOf CanExecute)
            _signalsMgr = SignalManager.Instance
            _browseSavePath = New DelegateCommand(AddressOf _openSavePath, AddressOf CanExecute)
        End Sub
        Public Sub New(detectorConfigure As ReadConfigXml.DetectorConfigModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detectorConfigure
            Dim newDetectorList = New ObservableCollection(Of DetectorBase)
            Dim newDataWriterDetectorList = New ObservableCollection(Of DetectorBase)
            For Each detector In _model.DetectorList
                Select Case detector.Name
                    Case "Out-Of-Range Detector"
                        newDetectorList.Add(New OutOfRangeFrequencyDetector(detector, signalsMgr))
                    Case "Ringdown Detector"
                        newDetectorList.Add(New RingdownDetector(detector, signalsMgr))
                    Case "Wind Ramp Detector"
                        newDetectorList.Add(New WindRampDetector(detector, signalsMgr))
                    Case "Periodogram Forced Oscillation Detector"
                        newDetectorList.Add(New PeriodogramDetector(detector, signalsMgr))
                        ResultUpdateIntervalVisibility = Visibility.Visible
                    Case "Spectral Coherence Forced Oscillation Detector"
                        newDetectorList.Add(New SpectralCoherenceDetector(detector, signalsMgr))
                        ResultUpdateIntervalVisibility = Visibility.Visible
                    Case "Data Writer"
                        newDataWriterDetectorList.Add(New DataWriterDetectorViewModel(detector, signalsMgr))
                    Case Else
                        Throw New Exception("Unknown element found in DetectorConfig in config file.")
                End Select
            Next
            DetectorList = newDetectorList
            DataWriterDetectorList = newDataWriterDetectorList
            Dim newAlarmingList = New ObservableCollection(Of AlarmingDetectorBase)
            For Each alarm In _model.AlarmingList
                Select Case alarm.Name
                    Case "Spectral Coherence Forced Oscillation Detector"
                        newAlarmingList.Add(New AlarmingSpectralCoherence(alarm, signalsMgr))
                    Case "Periodogram Forced Oscillation Detector"
                        newAlarmingList.Add(New AlarmingPeriodogram(alarm, signalsMgr))
                    Case "Ringdown Detector"
                        newAlarmingList.Add(New AlarmingRingdown(alarm, signalsMgr))
                    Case Else
                        Throw New Exception("Error! Unknown alarming detector elements found in config file.")
                End Select

            Next
            AlarmingList = newAlarmingList
            _signalsMgr = signalsMgr
        End Sub
        Private _signalsMgr As SignalManager
        Private _model As DetectorConfigModel
        Public Property Model As DetectorConfigModel
            Get
                Return _model
            End Get
            Set(ByVal value As DetectorConfigModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Private _eventPath As String
        Public Property EventPath As String
            Get
                Return _model.EventPath
            End Get
            Set(ByVal value As String)
                _model.EventPath = value
                OnPropertyChanged()
            End Set
        End Property
        Private _resultUpdateInterval As String
        Public Property ResultUpdateInterval As String
            Get
                Return _model.ResultUpdateInterval
            End Get
            Set(ByVal value As String)
                _model.ResultUpdateInterval = value
                OnPropertyChanged()
            End Set
        End Property
        Private _detectorList As ObservableCollection(Of DetectorBase)
        Public Property DetectorList As ObservableCollection(Of DetectorBase)
            Get
                Return _detectorList
            End Get
            Set(ByVal value As ObservableCollection(Of DetectorBase))
                _detectorList = value
                OnPropertyChanged()
            End Set
        End Property
        Private _dataWriterDetectorList As ObservableCollection(Of DetectorBase)
        Public Property DataWriterDetectorList As ObservableCollection(Of DetectorBase)
            Get
                Return _dataWriterDetectorList
            End Get
            Set(ByVal value As ObservableCollection(Of DetectorBase))
                _dataWriterDetectorList = value
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
        Private _resultUpdateIntervalVisibility As Visibility
        Public Property ResultUpdateIntervalVisibility As Visibility
            Get
                Return _resultUpdateIntervalVisibility
            End Get
            Set(ByVal value As Visibility)
                _resultUpdateIntervalVisibility = value
                OnPropertyChanged()
            End Set
        End Property
        'Private _addDataWriterDetector As ICommand
        'Public Property AddDataWriterDetector As ICommand
        '    Get
        '        Return _addDataWriterDetector
        '    End Get
        '    Set(value As ICommand)
        '        _addDataWriterDetector = value
        '    End Set
        'End Property
        'Private Sub _addADataWriterDetector(obj As Object)
        '    Dim newDetector = New DataWriterDetectorViewModel
        '    newDetector.IsExpanded = True
        '    newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (_signalsMgr.GroupedSignalByDataWriterDetectorInput.Count + 1).ToString & " " & newDetector.Name
        '    _signalsMgr.GroupedSignalByDataWriterDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        '    DataWriterDetectorList.Add(newDetector)
        'End Sub
        Private _lastSavePath As String
        Private _browseSavePath As ICommand
        Public Property BrowseSavePath As ICommand
            Get
                Return _browseSavePath
            End Get
            Set(ByVal value As ICommand)
                _browseSavePath = value
            End Set
        End Property
        Private Sub _openSavePath(obj As Object)
            'Dim openDirectoryDialog As New FolderBrowserDialog()
            'openDirectoryDialog.Description = "Select the Save Path"
            'If _lastSavePath Is Nothing Then
            '    openDirectoryDialog.SelectedPath = Environment.CurrentDirectory
            'Else
            '    openDirectoryDialog.SelectedPath = _lastSavePath
            'End If
            'openDirectoryDialog.ShowNewFolderButton = True
            'If (openDirectoryDialog.ShowDialog = DialogResult.OK) Then
            '    _lastSavePath = openDirectoryDialog.SelectedPath
            '    obj.SavePath = openDirectoryDialog.SelectedPath
            'End If
            Dim openDirectoryDialog As New CommonOpenFileDialog
            openDirectoryDialog.Title = "Select the Save Path"
            openDirectoryDialog.IsFolderPicker = True
            If _lastSavePath Is Nothing Then
                openDirectoryDialog.InitialDirectory = Environment.CurrentDirectory
            Else
                openDirectoryDialog.InitialDirectory = _lastSavePath
            End If
            openDirectoryDialog.AddToMostRecentlyUsedList = True
            openDirectoryDialog.AllowNonFileSystemItems = False
            openDirectoryDialog.DefaultDirectory = Environment.CurrentDirectory
            openDirectoryDialog.EnsureFileExists = True
            openDirectoryDialog.EnsurePathExists = True
            openDirectoryDialog.EnsureReadOnly = False
            openDirectoryDialog.EnsureValidNames = True
            openDirectoryDialog.Multiselect = False
            openDirectoryDialog.ShowPlacesList = True
            If openDirectoryDialog.ShowDialog = CommonFileDialogResult.Ok Then
                _lastSavePath = openDirectoryDialog.FileName
                obj.SavePath = openDirectoryDialog.FileName
            End If
        End Sub
    End Class

    Public Class PeriodogramDetector
        Inherits DetectorBase
        Public Sub New()
            _pfa = "0.01"
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New PeriodogramDetectorModel()
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

        Public Sub New(detector As PeriodogramDetectorModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (signalsMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & Name
            Try
                InputChannels = signalsMgr.FindSignals(detector.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByDetectorInput.Add(ThisStepInputsAsSignalHerachyByType)
        End Sub

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return InputChannels.Count > 0
        End Function

        Private _model As PeriodogramDetectorModel
        Public Property Model As PeriodogramDetectorModel
            Get
                Return _model
            End Get
            Set(ByVal value As PeriodogramDetectorModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Periodogram Forced Oscillation Detector"
            End Get
        End Property
        Private _mode As DetectorModeType
        Public Property Mode As DetectorModeType
            Get
                Return _model.Mode
            End Get
            Set(ByVal value As DetectorModeType)
                _model.Mode = value
                OnPropertyChanged()
            End Set
        End Property
        Private _analysisLength As Integer
        Public Property AnalysisLength As Integer
            Get
                Return _model.AnalysisLength
            End Get
            Set(ByVal value As Integer)
                _model.AnalysisLength = value
                WindowLength = Math.Floor(value / 3)
                OnPropertyChanged()
            End Set
        End Property
        Private _windowType As DetectorWindowType
        Public Property WindowType As DetectorWindowType
            Get
                Return _model.WindowType
            End Get
            Set(ByVal value As DetectorWindowType)
                _model.WindowType = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyInterval As String
        Public Property FrequencyInterval As String
            Get
                Return _model.FrequencyInterval
            End Get
            Set(ByVal value As String)
                _model.FrequencyInterval = value
                OnPropertyChanged()
            End Set
        End Property
        Private _windowLength As Integer
        Public Property WindowLength As Integer
            Get
                Return _model.WindowLength
            End Get
            Set(ByVal value As Integer)
                _model.WindowLength = value
                WindowOverlap = Math.Floor(value / 2)
                OnPropertyChanged()
            End Set
        End Property
        Private _windowOverlap As Integer
        Public Property WindowOverlap As Integer
            Get
                Return _model.WindowOverlap
            End Get
            Set(ByVal value As Integer)
                _model.WindowOverlap = value
                OnPropertyChanged()
            End Set
        End Property
        Private _medianFilterFrequencyWidth As String
        Public Property MedianFilterFrequencyWidth As String
            Get
                Return _model.MedianFilterFrequencyWidth
            End Get
            Set(ByVal value As String)
                _model.MedianFilterFrequencyWidth = value
                OnPropertyChanged()
            End Set
        End Property
        Private _pfa As String
        Public Property Pfa As String
            Get
                Return _model.Pfa
            End Get
            Set(ByVal value As String)
                _model.Pfa = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMin As String
        Public Property FrequencyMin As String
            Get
                Return _model.FrequencyMin
            End Get
            Set(ByVal value As String)
                _model.FrequencyMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMax As String
        Public Property FrequencyMax As String
            Get
                Return _model.FrequencyMax
            End Get
            Set(ByVal value As String)
                _model.FrequencyMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyTolerance As String
        Public Property FrequencyTolerance As String
            Get
                Return _model.FrequencyTolerance
            End Get
            Set(ByVal value As String)
                _model.FrequencyTolerance = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class SpectralCoherenceDetector
        Inherits DetectorBase
        Public Sub New()
            _model = New SpectralCoherenceDetectorModel
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

        Public Sub New(detector As SpectralCoherenceDetectorModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (signalsMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & Name
            Try
                InputChannels = signalsMgr.FindSignals(detector.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByDetectorInput.Add(ThisStepInputsAsSignalHerachyByType)
        End Sub
        Private _model As SpectralCoherenceDetectorModel
        Public Property Model As SpectralCoherenceDetectorModel
            Get
                Return _model
            End Get
            Set(ByVal value As SpectralCoherenceDetectorModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Spectral Coherence Forced Oscillation Detector"
            End Get
        End Property
        Private _mode As DetectorModeType
        Public Property Mode As DetectorModeType
            Get
                Return _model.Mode
            End Get
            Set(ByVal value As DetectorModeType)
                _model.Mode = value
                OnPropertyChanged()
            End Set
        End Property
        Private _analysisLength As Integer
        Public Property AnalysisLength() As Integer
            Get
                Return _model.AnalysisLength
            End Get
            Set(ByVal value As Integer)
                _model.AnalysisLength = value
                'Delay = _analysisLength / 10
                WindowLength = Math.Floor(value / 5)
                OnPropertyChanged()
            End Set
        End Property
        Private _delay As Double
        Public Property Delay As Double
            Get
                Return _model.Delay
            End Get
            Set(ByVal value As Double)
                _model.Delay = value
                OnPropertyChanged()
            End Set
        End Property
        Private _numberDelays As Integer
        Public Property NumberDelays As Integer
            Get
                Return _model.NumberDelays
            End Get
            Set(ByVal value As Integer)
                _model.NumberDelays = value
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
                Return _model.ThresholdScale
            End Get
            Set(ByVal value As Integer)
                _model.ThresholdScale = value
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
                Return _model.WindowType
            End Get
            Set(ByVal value As DetectorWindowType)
                _model.WindowType = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyInterval As String
        Public Property FrequencyInterval As String
            Get
                Return _model.FrequencyInterval
            End Get
            Set(ByVal value As String)
                _model.FrequencyInterval = value
                OnPropertyChanged()
            End Set
        End Property
        Private _windowLength As Integer
        Public Property WindowLength As Integer
            Get
                Return _model.WindowLength
            End Get
            Set(ByVal value As Integer)
                _model.WindowLength = value
                WindowOverlap = Math.Floor(value / 2)
                OnPropertyChanged()
            End Set
        End Property
        Private _windowOverlap As Integer
        Public Property WindowOverlap As Integer
            Get
                Return _model.WindowOverlap
            End Get
            Set(ByVal value As Integer)
                _model.WindowOverlap = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMin As String
        Public Property FrequencyMin As String
            Get
                Return _model.FrequencyMin
            End Get
            Set(ByVal value As String)
                _model.FrequencyMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyMax As String
        Public Property FrequencyMax As String
            Get
                Return _model.FrequencyMax
            End Get
            Set(ByVal value As String)
                _model.FrequencyMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _frequencyTolerance As String
        Public Property FrequencyTolerance As String
            Get
                Return _model.FrequencyTolerance
            End Get
            Set(ByVal value As String)
                _model.FrequencyTolerance = value
                OnPropertyChanged()
            End Set
        End Property

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return InputChannels.Count > 0
        End Function
    End Class

    Public Class RingdownDetector
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New RingdownDetectorModel()
            IsExpanded = False
            RMSlength = "15"
            RMSmedianFilterTime = "120"
            RingThresholdScale = "3"
        End Sub

        Public Sub New(detector As RingdownDetectorModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (signalsMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & Name
            Try
                InputChannels = signalsMgr.FindSignals(detector.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByDetectorInput.Add(ThisStepInputsAsSignalHerachyByType)
        End Sub
        Private _model As RingdownDetectorModel
        Public Property Model As RingdownDetectorModel
            Get
                Return _model
            End Get
            Set(ByVal value As RingdownDetectorModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Ringdown Detector"
            End Get
        End Property
        Private _rmsLength As String
        Public Property RMSlength As String
            Get
                Return _model.RMSlength
            End Get
            Set(ByVal value As String)
                _model.RMSlength = value
                OnPropertyChanged()
            End Set
        End Property
        Private _rmsmedianFilterTime As String
        Public Property RMSmedianFilterTime As String
            Get
                Return _model.RMSmedianFilterTime
            End Get
            Set(ByVal value As String)
                _model.RMSmedianFilterTime = value
                OnPropertyChanged()
            End Set
        End Property
        Private _ringThresholdScale As String
        Public Property RingThresholdScale As String
            Get
                Return _model.RingThresholdScale
            End Get
            Set(ByVal value As String)
                _model.RingThresholdScale = value
                OnPropertyChanged()
            End Set
        End Property
        Private _maxDuration As String
        Public Property MaxDuration As String
            Get
                Return _model.MaxDuration
            End Get
            Set(ByVal value As String)
                _model.MaxDuration = value
                OnPropertyChanged()
            End Set
        End Property

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return InputChannels.Count > 0
        End Function
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

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return InputChannels.Count > 0
        End Function
    End Class

    Public Class OutOfRangeFrequencyDetector
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New OutOfRangeFrequencyDetectorModel()
            IsExpanded = False
        End Sub

        Public Sub New(detector As OutOfRangeFrequencyDetectorModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (signalsMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & Name
            Try
                InputChannels = signalsMgr.FindSignals(detector.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByDetectorInput.Add(ThisStepInputsAsSignalHerachyByType)
        End Sub

        Private _model As OutOfRangeFrequencyDetectorModel
        Public Property Model As OutOfRangeFrequencyDetectorModel
            Get
                Return _model
            End Get
            Set(ByVal value As OutOfRangeFrequencyDetectorModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Out-Of-Range Detector"
            End Get
        End Property
        Private _type As OutOfRangeFrequencyDetectorType
        Public Property Type As OutOfRangeFrequencyDetectorType
            Get
                Return _model.Type
            End Get
            Set(ByVal value As OutOfRangeFrequencyDetectorType)
                _model.Type = value
                OnPropertyChanged()
            End Set
        End Property
        Private _averageWindow As String
        Public Property AverageWindow As String
            Get
                Return _model.AverageWindow
            End Get
            Set(ByVal value As String)
                _model.AverageWindow = value
                OnPropertyChanged()
            End Set
        End Property
        Private _nominal As String
        Public Property Nominal As String
            Get
                Return _model.Nominal
            End Get
            Set(ByVal value As String)
                _model.Nominal = value
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
                Return _model.DurationMax
            End Get
            Set(ByVal value As String)
                _model.DurationMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _durationMin As String
        Public Property DurationMin As String
            Get
                Return _model.DurationMin
            End Get
            Set(ByVal value As String)
                _model.DurationMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _duration As String
        Public Property Duration As String
            Get
                Return _model.Duration
            End Get
            Set(ByVal value As String)
                _model.Duration = value
                OnPropertyChanged()
            End Set
        End Property
        Private _analysisWindow As String
        Public Property AnalysisWindow As String
            Get
                Return _model.AnalysisWindow
            End Get
            Set(ByVal value As String)
                _model.AnalysisWindow = value
                OnPropertyChanged()
            End Set
        End Property
        Private _rateOfChangeMax As String
        Public Property RateOfChangeMax As String
            Get
                Return _model.RateOfChangeMax
            End Get
            Set(ByVal value As String)
                _model.RateOfChangeMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _rateOfChangeMin As String
        Public Property RateOfChangeMin As String
            Get
                Return _model.RateOfChangeMin
            End Get
            Set(ByVal value As String)
                _model.RateOfChangeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _rateOfChange As String
        Public Property RateOfChange As String
            Get
                Return _model.RateOfChange
            End Get
            Set(ByVal value As String)
                _model.RateOfChange = value
                OnPropertyChanged()
            End Set
        End Property
        Private _eventMergeWindow As String
        Public Property EventMergeWindow As String
            Get
                Return _model.EventMergeWindow
            End Get
            Set(ByVal value As String)
                _model.EventMergeWindow = value
                OnPropertyChanged()
            End Set
        End Property

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return InputChannels.Count > 0
        End Function
    End Class

    Public Class WindRampDetector
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New WindRampDetectorModel()
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

        Public Sub New(detector As WindRampDetectorModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (signalsMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & Name
            Try
                InputChannels = signalsMgr.FindSignals(detector.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByDetectorInput.Add(ThisStepInputsAsSignalHerachyByType)
        End Sub
        Private _model As WindRampDetectorModel
        Public Property Model As WindRampDetectorModel
            Get
                Return _model
            End Get
            Set(ByVal value As WindRampDetectorModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
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
                Return _model.IsLongTrend
            End Get
            Set(ByVal value As Boolean)
                _model.IsLongTrend = value
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
                Return _model.Fpass
            End Get
            Set(ByVal value As String)
                _model.Fpass = value
                OnPropertyChanged()
            End Set
        End Property
        Private _fstop As String
        Public Property Fstop As String
            Get
                Return _model.Fstop
            End Get
            Set(ByVal value As String)
                _model.Fstop = value
                OnPropertyChanged()
            End Set
        End Property
        Private _apass As String
        Public Property Apass As String
            Get
                Return _model.Apass
            End Get
            Set(ByVal value As String)
                _model.Apass = value
                OnPropertyChanged()
            End Set
        End Property
        Private _astop As String
        Public Property Astop As String
            Get
                Return _model.Astop
            End Get
            Set(ByVal value As String)
                _model.Astop = value
                OnPropertyChanged()
            End Set
        End Property
        Private _valMin As String
        Public Property ValMin As String
            Get
                Return _model.ValMin
            End Get
            Set(ByVal value As String)
                _model.ValMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _valMax As String
        Public Property ValMax As String
            Get
                Return _model.ValMax
            End Get
            Set(ByVal value As String)
                _model.ValMax = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMin As String
        Public Property TimeMin As String
            Get
                Return _model.TimeMin
            End Get
            Set(ByVal value As String)
                _model.TimeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMax As String
        Public Property TimeMax As String
            Get
                Return _model.TimeMax
            End Get
            Set(ByVal value As String)
                _model.TimeMax = value
                OnPropertyChanged()
            End Set
        End Property

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return InputChannels.Count > 0
        End Function
    End Class

    Public Class DataWriterDetectorViewModel
        Inherits DetectorBase
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New DataWriterDetectorModel()
            IsExpanded = False
            '_browseSavePath = New DelegateCommand(AddressOf _openSavePath, AddressOf CanExecute)
        End Sub
        Public Sub New(detector As DataWriterDetectorModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (signalsMgr.GroupedSignalByDataWriterDetectorInput.Count + 1).ToString & " " & Name
            Try
                InputChannels = signalsMgr.FindSignals(detector.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error sorting output signals by PMU in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByDataWriterDetectorInput.Add(ThisStepInputsAsSignalHerachyByType)
        End Sub
        Private _model As DataWriterDetectorModel
        Public Property Model As DataWriterDetectorModel
            Get
                Return _model
            End Get
            Set(ByVal value As DataWriterDetectorModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Data Writer"
            End Get
        End Property
        Private _savePath As String
        Public Property SavePath As String
            Get
                Return _model.SavePath
            End Get
            Set(ByVal value As String)
                _model.SavePath = value
                OnPropertyChanged()
            End Set
        End Property
        Private _separatePMUs As Boolean
        Public Property SeparatePMUs As Boolean
            Get
                Return _model.SeparatePMUs
            End Get
            Set(ByVal value As Boolean)
                _model.SeparatePMUs = value
                OnPropertyChanged()
            End Set
        End Property
        Private _mnemonic As String
        Public Property Mnemonic As String
            Get
                Return _model.Mnemonic
            End Get
            Set(ByVal value As String)
                _model.Mnemonic = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return InputChannels.Count > 0
        End Function
        'Private _lastSavePath As String
        'Private _browseSavePath As ICommand
        'Public Property BrowseSavePath As ICommand
        '    Get
        '        Return _browseSavePath
        '    End Get
        '    Set(ByVal value As ICommand)
        '        _browseSavePath = value
        '    End Set
        'End Property
        'Private Sub _openSavePath(obj As Object)
        '    Dim openDirectoryDialog As New FolderBrowserDialog()
        '    openDirectoryDialog.Description = "Select the save path"
        '    If _lastSavePath Is Nothing Then
        '        openDirectoryDialog.SelectedPath = Environment.CurrentDirectory
        '    Else
        '        openDirectoryDialog.SelectedPath = _lastSavePath
        '    End If
        '    openDirectoryDialog.ShowNewFolderButton = True
        '    If (openDirectoryDialog.ShowDialog = DialogResult.OK) Then
        '        _lastSavePath = openDirectoryDialog.SelectedPath
        '        SavePath = openDirectoryDialog.SelectedPath
        '    End If
        'End Sub
    End Class

    'Public Enum DetectorModeType
    '    <ComponentModel.Description("Single Channel")>
    '    SingleChannel
    '    <ComponentModel.Description("Multichannel")>
    '    MultiChannel
    'End Enum

    'Public Enum DetectorWindowType
    '    <ComponentModel.Description("Hann")>
    '    hann
    '    <ComponentModel.Description("Rectangular")>
    '    rectwin
    '    <ComponentModel.Description("Bartlett")>
    '    bartlett
    '    <ComponentModel.Description("Hamming")>
    '    hamming
    '    <ComponentModel.Description("Blackman")>
    '    blackman
    'End Enum

    'Public Enum OutOfRangeFrequencyDetectorType
    '    <ComponentModel.Description("Nominal Value")>
    '    Nominal
    '    <ComponentModel.Description("History for Baseline (seconds)")>
    '    AvergeWindow
    'End Enum

    Public Class AlarmingSpectralCoherence
        Inherits AlarmingDetectorBase
        Public Sub New()
            IsExpanded = False
            _model = New AlarmingSpectralCoherenceModel()
        End Sub

        Public Sub New(detector As AlarmingSpectralCoherenceModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
        End Sub

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function

        Private _model As AlarmingSpectralCoherenceModel
        Public Property Model As AlarmingSpectralCoherenceModel
            Get
                Return _model
            End Get
            Set(ByVal value As AlarmingSpectralCoherenceModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Spectral Coherence Forced Oscillation Detector"
            End Get
        End Property
        Private _coherenceAlarm As String
        Public Property CoherenceAlarm As String
            Get
                Return _model.CoherenceAlarm
            End Get
            Set(ByVal value As String)
                _model.CoherenceAlarm = value
                OnPropertyChanged()
            End Set
        End Property
        Private _coherenceMin As String
        Public Property CoherenceMin As String
            Get
                Return _model.CoherenceMin
            End Get
            Set(ByVal value As String)
                _model.CoherenceMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMin As String
        Public Property TimeMin As String
            Get
                Return _model.TimeMin
            End Get
            Set(ByVal value As String)
                _model.TimeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _coherenceCorner As String
        Public Property CoherenceCorner As String
            Get
                Return _model.CoherenceCorner
            End Get
            Set(ByVal value As String)
                _model.CoherenceCorner = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeCorner As String
        Public Property TimeCorner As String
            Get
                Return _model.TimeCorner
            End Get
            Set(ByVal value As String)
                _model.TimeCorner = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class AlarmingPeriodogram
        Inherits AlarmingDetectorBase
        Public Sub New()
            IsExpanded = False
            _model = New AlarmingPeriodogramModel()
        End Sub

        Public Sub New(detector As AlarmingPeriodogramModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Private _model As AlarmingPeriodogramModel
        Public Property Model As AlarmingPeriodogramModel
            Get
                Return _model
            End Get
            Set(ByVal value As AlarmingPeriodogramModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Periodogram Forced Oscillation Detector"
            End Get
        End Property
        Private _snrAlarm As String
        Public Property SNRalarm As String
            Get
                Return _model.SNRalarm
            End Get
            Set(ByVal value As String)
                _model.SNRalarm = value
                OnPropertyChanged()
            End Set
        End Property
        Private _snrMin As String
        Public Property SNRmin As String
            Get
                Return _model.SNRmin
            End Get
            Set(ByVal value As String)
                _model.SNRmin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeMin As String
        Public Property TimeMin As String
            Get
                Return _model.TimeMin
            End Get
            Set(ByVal value As String)
                _model.TimeMin = value
                OnPropertyChanged()
            End Set
        End Property
        Private _snrCorner As String
        Public Property SNRcorner As String
            Get
                Return _model.SNRcorner
            End Get
            Set(ByVal value As String)
                _model.SNRcorner = value
                OnPropertyChanged()
            End Set
        End Property
        Private _timeCorner As String
        Public Property TimeCorner As String
            Get
                Return _model.TimeCorner
            End Get
            Set(ByVal value As String)
                _model.TimeCorner = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class AlarmingRingdown
        Inherits AlarmingDetectorBase
        Public Sub New()
            IsExpanded = False
            _model = New AlarmingRingdownModel
        End Sub

        Public Sub New(detector As AlarmingRingdownModel, signalsMgr As SignalManager)
            Me.New
            Me._model = detector
        End Sub
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Private _model As AlarmingRingdownModel
        Public Property Model As AlarmingRingdownModel
            Get
                Return _model
            End Get
            Set(ByVal value As AlarmingRingdownModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public Overrides ReadOnly Property Name As String
            Get
                Return "Ringdown Detector"
            End Get
        End Property
        Private _maxDuration As String
        Public Property MaxDuration As String
            Get
                Return _model.MaxDuration
            End Get
            Set(ByVal value As String)
                _model.MaxDuration = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
End Namespace
