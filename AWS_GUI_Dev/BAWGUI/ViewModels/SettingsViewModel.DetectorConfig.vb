Imports System.Collections.ObjectModel
Imports System.Windows.Forms

Partial Public Class SettingsViewModel
    Private _groupedSignalByDetectorInput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByDetectorInput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByDetectorInput
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByDetectorInput = value
            OnPropertyChanged()
        End Set
    End Property
#Region "Reading"
    Private Sub _readDetectorConfig()
        GroupedSignalByDetectorInput = New ObservableCollection(Of SignalTypeHierachy)
        Dim newDetectorList = New ObservableCollection(Of DetectorBase)
        Dim detectors = From el In _configData.<Config>.<DetectorConfig>.<Configuration>.Elements Select el
        For Each detector In detectors
            Select Case detector.Name
                Case "EventPath"
                    DetectorConfigure.EventPath = detector.Value
                Case "ResultUpdateInterval"
                    DetectorConfigure.ResultUpdateInterval = detector.Value
                Case "OutOfRangeGeneral"
                    _readOutOfRangeGeneral(detector, newDetectorList)
                Case "OutOfRangeFrequency"
                    _readOutOfRangeFrequency(detector, newDetectorList)
                Case "Ringdown"
                    _readRingdown(detector, newDetectorList)
                Case "WindRamp"
                    _readWindRamp(detector, newDetectorList)
                Case "Periodogram"
                    _readPeriodogram(detector, newDetectorList)
                Case "SpectralCoherence"
                    _readSpectralCoherence(detector, newDetectorList)
                Case "Alarming"
                    _readAlarming(detector)
                Case Else
                    Throw New Exception("Unknown element found in DetectorConfig in config file.")
            End Select
        Next
        DetectorConfigure.DetectorList = newDetectorList
    End Sub

    Private Sub _readAlarming(detector As XElement)
        Dim newAlarmingList = New ObservableCollection(Of AlarmingDetectorBase)
        For Each alarm In detector.Elements
            Select Case alarm.Name
                Case "SpectralCoherence"
                    Dim newDetector = New AlarmingSpectralCoherence
                    newDetector.CoherenceAlarm = alarm.<CoherenceAlarm>.Value
                    newDetector.CoherenceMin = alarm.<CoherenceMin>.Value
                    newDetector.TimeMin = alarm.<TimeMin>.Value
                    newDetector.CoherenceCorner = alarm.<CoherenceCorner>.Value
                    newDetector.TimeCorner = alarm.<TimeCorner>.Value
                    newAlarmingList.Add(newDetector)
                Case "Periodogram"
                    Dim newDetector = New AlarmingPeriodogram
                    newDetector.SNRalarm = alarm.<SNRalarm>.Value
                    newDetector.SNRmin = alarm.<SNRmin>.Value
                    newDetector.TimeMin = alarm.<TimeMin>.Value
                    newDetector.SNRcorner = alarm.<SNRcorner>.Value
                    newDetector.TimeCorner = alarm.<TimeCorner>.Value
                    newAlarmingList.Add(newDetector)
                Case "Ringdown"
                    Dim newDetector = New AlarmingRingdown
                    newDetector.MaxDuration = alarm.<MaxDuration>.Value
                    newAlarmingList.Add(newDetector)
                Case Else
                    Throw New Exception("Error! Unknown alarming detector elements found in config file.")
            End Select
            DetectorConfigure.AlarmingList = newAlarmingList
        Next
    End Sub

    Private Sub _readSpectralCoherence(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New SpectralCoherenceDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.Mode = [Enum].Parse(GetType(DetectorModeType), detector.<Mode>.Value)
        newDetector.AnalysisLength = detector.<AnalysisLength>.Value
        newDetector.Delay = detector.<Delay>.Value
        newDetector.NumberDelays = detector.<NumberDelays>.Value
        newDetector.ThresholdScale = detector.<ThresholdScale>.Value
        newDetector.WindowType = [Enum].Parse(GetType(DetectorWindowType), detector.<WindowType>.Value)
        newDetector.FrequencyInterval = detector.<FrequencyInterval>.Value
        newDetector.WindowLength = detector.<WindowLength>.Value
        newDetector.WindowOverlap = detector.<WindowOverlap>.Value
        newDetector.FrequencyMin = detector.<FrequencyMin>.Value
        newDetector.FrequencyMax = detector.<FrequencyMax>.Value
        newDetector.FrequencyTolerance = detector.<FrequencyTolerance>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In Spectral Coherence detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readPeriodogram(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New PeriodogramDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.Mode = [Enum].Parse(GetType(DetectorModeType), detector.<Mode>.Value)
        newDetector.AnalysisLength = detector.<AnalysisLength>.Value
        newDetector.WindowType = [Enum].Parse(GetType(DetectorWindowType), detector.<WindowType>.Value)
        newDetector.WindowLength = detector.<WindowLength>.Value
        newDetector.FrequencyInterval = detector.<FrequencyInterval>.Value
        newDetector.WindowOverlap = detector.<WindowOverlap>.Value
        newDetector.MedianFilterFrequencyWidth = detector.<MedianFilterFrequencyWidth>.Value
        newDetector.Pfa = detector.<Pfa>.Value
        newDetector.FrequencyMin = detector.<FrequencyMin>.Value
        newDetector.FrequencyMax = detector.<FrequencyMax>.Value
        newDetector.FrequencyTolerance = detector.<FrequencyTolerance>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In Periodogram detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readWindRamp(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New WindRampDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.Fpass = detector.<Fpass>.Value
        newDetector.Fstop = detector.<Fstop>.Value
        newDetector.Apass = detector.<Apass>.Value
        newDetector.Astop = detector.<Astop>.Value
        newDetector.ValMin = detector.<ValMin>.Value
        newDetector.TimeMin = detector.<TimeMin>.Value
        newDetector.ValMax = detector.<ValMax>.Value
        newDetector.TimeMax = detector.<TimeMax>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In Windramp detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readRingdown(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New RingdownDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.RMSlength = detector.<RMSlength>.Value
        newDetector.ForgetFactor = detector.<ForgetFactor>.Value
        newDetector.RingThresholdScale = detector.<RingThresholdScale>.Value
        newDetector.MaxDuration = detector.<MaxDuration>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In Ringdown detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readOutOfRangeFrequency(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New OutOfRangeFrequencyDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        Dim type = From el In detector.Elements Where el.Name = "AverageWindow" Select el
        If type.Count = 0 Then
            newDetector.Type = OutOfRangeFrequencyDetectorType.Nominal
            newDetector.Nominal = type.Value
        Else
            newDetector.Type = OutOfRangeFrequencyDetectorType.AvergeWindow
            newDetector.AverageWindow = type.Value
        End If
        newDetector.DurationMax = detector.<DurationMax>.Value
        newDetector.DurationMin = detector.<DurationMin>.Value
        newDetector.Duration = detector.<Duration>.Value
        newDetector.AnalysisWindow = detector.<AnalysisWindow>.Value
        newDetector.RateOfChangeMax = detector.<RateOfChangeMax>.Value
        newDetector.RateOfChangeMin = detector.<RateOfChangeMin>.Value
        newDetector.RateOfChange = detector.<RateOfChange>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In out of range frequency detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readOutOfRangeGeneral(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New OutOfRangeGeneralDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.Max = detector.<Max>.Value
        newDetector.Min = detector.<Min>.Value
        newDetector.AnalysisWindow = detector.<AnalysisWindow>.Value
        newDetector.Duration = detector.<Duration>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In out of range general detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub
#End Region
#Region "Add, Delete, Select a detector, DeSelect All detector"
    Private _detectorSelectedToAdd As ICommand
    Public Property DetectorSelectedToAdd As ICommand
        Get
            Return _detectorSelectedToAdd
        End Get
        Set(ByVal value As ICommand)
            _detectorSelectedToAdd = value
        End Set
    End Property
    Private Sub _addSelectedDetector(obj As String)
        Dim newDetector As Object
        Select Case obj
            Case "OutOfRangeGeneral"
                newDetector = New OutOfRangeGeneralDetector
            Case "OutOfRangeFrequency"
                newDetector = New OutOfRangeFrequencyDetector
            Case "Ringdown"
                newDetector = New RingdownDetector
            Case "WindRamp"
                newDetector = New WindRampDetector
            Case "Periodogram"
                newDetector = New PeriodogramDetector
            Case "SpectralCoherence"
                newDetector = New SpectralCoherenceDetector
            Case Else
                Throw New Exception("Unknown detector selected to add.")
        End Select
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        DetectorConfigure.DetectorList.Add(newDetector)
        _selectedADetector(newDetector)
    End Sub
    Private _alarmingDetectorSelectedToAdd As ICommand
    Public Property AlarmingDetectorSelectedToAdd As ICommand
        Get
            Return _alarmingDetectorSelectedToAdd
        End Get
        Set(ByVal value As ICommand)
            _alarmingDetectorSelectedToAdd = value
        End Set
    End Property
    Private Sub _addSelectedAlarmingDetector(obj As String)
        Select Case obj
            Case "Ringdown"
                For Each item In DetectorConfigure.AlarmingList
                    If TypeOf item Is AlarmingRingdown Then
                        MessageBox.Show("Ringdown alarming detector already added. Each alarming detector can only be added once.", "Error!", MessageBoxButtons.OK)
                        Exit Sub
                    End If
                Next
                Dim newDetector = New AlarmingRingdown
                DetectorConfigure.AlarmingList.Add(newDetector)
                _selectedADetector(newDetector)
            Case "Periodogram"
                For Each item In DetectorConfigure.AlarmingList
                    If TypeOf item Is AlarmingPeriodogram Then
                        MessageBox.Show("Periodogram alarming detector already added. Each alarming detector can only be added once.", "Error!", MessageBoxButtons.OK)
                        Exit Sub
                    End If
                Next
                Dim newDetector = New AlarmingPeriodogram
                DetectorConfigure.AlarmingList.Add(newDetector)
                _selectedADetector(newDetector)
            Case "SpectralCoherence"
                For Each item In DetectorConfigure.AlarmingList
                    If TypeOf item Is AlarmingSpectralCoherence Then
                        MessageBox.Show("Spectral coherence alarming detector already added. Each alarming detector can only be added once.", "Error!", MessageBoxButtons.OK)
                        Exit Sub
                    End If
                Next
                Dim newDetector = New AlarmingSpectralCoherence
                DetectorConfigure.AlarmingList.Add(newDetector)
                _selectedADetector(newDetector)
            Case Else
                Throw New Exception("Unknown detector selected to add.")
        End Select
    End Sub
    Private _detectorConfigStepDeSelected As ICommand
    Public Property DetectorConfigStepDeSelected As ICommand
        Get
            Return _detectorConfigStepDeSelected
        End Get
        Set(ByVal value As ICommand)
            _detectorConfigStepDeSelected = value
        End Set
    End Property
    Private Sub _deSelectAllDetectors()
        If CurrentSelectedStep IsNot Nothing Then
            If TypeOf CurrentSelectedStep Is DetectorBase Then
                For Each signal In _currentSelectedStep.InputChannels
                    signal.IsChecked = False
                Next
            End If
            _changeCheckStatusAllParentsOfGroupedSignal(AllProcessConfigOutputGroupedByType, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllProcessConfigOutputGroupedByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllDataConfigOutputGroupedByType, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllDataConfigOutputGroupedByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllPostProcessOutputGroupedByType, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllPostProcessOutputGroupedByPMU, False)

            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByDetectorInput, False)

            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByType, False)
            _currentSelectedStep.IsStepSelected = False
            CurrentSelectedStep = Nothing
            _determineFileDirCheckableStatus()
        End If
        SignalSelectionTreeViewVisibility = "Visible"
    End Sub
    Private _detectorSelected As ICommand
    Public Property DetectorSelected As ICommand
        Get
            Return _detectorSelected
        End Get
        Set(ByVal value As ICommand)
            _detectorSelected = value
        End Set
    End Property
    Private Sub _selectedADetector(detector As Object)
        ' if processStep is already selected, then the selection is not changed, nothing needs to be done.
        ' however, if processStep is not selected, which means a new selection, we need to find the old selection, unselect it and all it's input signal
        If Not detector.IsStepSelected Then
            Try
                Dim selectedFound = False
                For Each dt In DetectorConfigure.DetectorList
                    If dt.IsStepSelected Then
                        dt.IsStepSelected = False
                        For Each signal In dt.InputChannels
                            signal.IsChecked = False
                        Next
                        selectedFound = True
                        Exit For
                    End If
                Next
                If Not selectedFound Then
                    For Each alarm In DetectorConfigure.AlarmingList
                        If alarm.IsStepSelected Then
                            alarm.IsStepSelected = False
                        End If
                    Next
                End If
                _determineFileDirCheckableStatus()
                detector.IsStepSelected = True
                If TypeOf detector Is DetectorBase Then
                    For Each signal In detector.InputChannels
                        signal.IsChecked = True
                    Next
                End If

                _detectorConfigDetermineAllParentNodeStatus()

                _determineFileDirCheckableStatus()

                If TypeOf detector Is AlarmingDetectorBase Then
                    SignalSelectionTreeViewVisibility = "Collapsed"
                Else
                    SignalSelectionTreeViewVisibility = "Visible"
                End If

                CurrentSelectedStep = detector
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        End If
    End Sub

    Private Sub _detectorConfigDetermineAllParentNodeStatus()
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType)
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU)
        _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType)
        _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU)
        _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByType)
        _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByPMU)
        _determineParentGroupedByTypeNodeStatus(AllPostProcessOutputGroupedByType)
        _determineParentGroupedByTypeNodeStatus(AllPostProcessOutputGroupedByPMU)
        For Each stepInput In GroupedSignalByDetectorInput
            If stepInput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepInput.SignalList)
                _determineParentCheckStatus(stepInput)
            Else
                stepInput.SignalSignature.IsChecked = False
            End If
        Next
    End Sub

    Private _deleteDetector As ICommand
    Public Property DeleteDetector As ICommand
        Get
            Return _deleteDetector
        End Get
        Set(ByVal value As ICommand)
            _deleteDetector = value
        End Set
    End Property
    Private Sub _deleteADetector(obj As Object)
        Try
            If TypeOf obj Is DetectorBase Then
                Dim newlist = New ObservableCollection(Of DetectorBase)(DetectorConfigure.DetectorList)
                newlist.Remove(obj)
                DetectorConfigure.DetectorList = newlist
                _addLog("Detector " & obj.Name & " is deleted!")
                GroupedSignalByDetectorInput.Remove(obj.ThisStepInputsAsSignalHerachyByType)
                For index = 1 To GroupedSignalByDetectorInput.Count
                    GroupedSignalByDetectorInput(index - 1).SignalSignature.SignalName = index.ToString & " Detector " & DetectorConfigure.DetectorList(index - 1).Name
                Next
            Else
                Dim newlist = New ObservableCollection(Of AlarmingDetectorBase)(DetectorConfigure.AlarmingList)
                newlist.Remove(obj)
                DetectorConfigure.AlarmingList = newlist
                _addLog("Alarming detector " & obj.Name & " is deleted from alarming!")
            End If
            _deSelectAllDetectors()
        Catch ex As Exception

        End Try
    End Sub
#End Region


End Class
