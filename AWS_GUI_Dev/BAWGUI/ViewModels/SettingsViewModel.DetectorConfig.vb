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
