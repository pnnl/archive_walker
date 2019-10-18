Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Input
Imports BAWGUI.Core
Imports DissipationEnergyFlow.ViewModels
Imports ModeMeter.ViewModels
Imports VoltageStability.ViewModels

Namespace ViewModels
    Partial Public Class SettingsViewModel
        'Private _groupedSignalByDetectorInput As ObservableCollection(Of SignalTypeHierachy)
        'Public Property GroupedSignalByDetectorInput As ObservableCollection(Of SignalTypeHierachy)
        '    Get
        '        Return _groupedSignalByDetectorInput
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
        '        _groupedSignalByDetectorInput = value
        '        OnPropertyChanged()
        '    End Set
        'End Property


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
            _aDetectorStepDeSelected()
            Dim newDetector As DetectorBase
            Select Case obj
                'Case "Out-of-Range"
                '    newDetector = New OutOfRangeGeneralDetector
                Case "Out-of-Range Detector"
                    newDetector = New OutOfRangeFrequencyDetector
                Case "Ringdown Detector"
                    newDetector = New RingdownDetector
                Case "Wind Ramp Detector"
                    newDetector = New WindRampDetector
                Case "Periodogram Forced Oscillation Detector"
                    newDetector = New PeriodogramDetector
                    DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Visible
                Case "Spectral Coherence Forced Oscillation Detector"
                    newDetector = New SpectralCoherenceDetector
                    DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Visible
                Case "Voltage Stability"
                    If _isVoltageStabilityDetectorExist(DetectorConfigure.DetectorList) Then
                        Forms.MessageBox.Show("Only one Voltage Stability detector can be added!", "Error!", MessageBoxButtons.OK)
                        Exit Sub
                    Else
                        newDetector = New VoltageStabilityDetectorViewModel(_signalMgr)
                        DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Visible
                    End If
                    'newDetector.DetectorGroupID = (DetectorConfigure.DetectorList.Count + 1).ToString
                Case "Mode Meter Tool"
                    If Not _isModeMeterResultFolderExists() Then
                        _generateModeMeterResultFolder()
                    End If
                    newDetector = New SmallSignalStabilityToolViewModel(_signalMgr)
                    DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Visible
                Case "Dissipation Energy Flow Detector"
                    If _isDEFDetectorExist(DetectorConfigure.DetectorList) Then
                        Forms.MessageBox.Show("Only one Dissipation Energy Flow Detector can be added!", "Error!", MessageBoxButtons.OK)
                        Exit Sub
                    Else
                        newDetector = New DEFDetectorViewModel(_signalMgr)
                    End If
                Case Else
                    Throw New Exception("Unknown detector selected to add.")
            End Select
            newDetector.IsExpanded = True
            If TypeOf (newDetector) Is VoltageStabilityDetectorViewModel OrElse TypeOf (newDetector) Is SmallSignalStabilityToolViewModel Then
            Else
                newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & newDetector.Name
                newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(newDetector.InputChannels)
                _signalMgr.GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            End If

            DetectorConfigure.DetectorList.Add(newDetector)
            _selectedADetector(newDetector)
        End Sub

        Private Sub _generateModeMeterResultFolder()
        End Sub

        Private Function _isModeMeterResultFolderExists() As Boolean
            Return Directory.Exists(Run.Model.EventPath & "\MM")
        End Function

        Private Function _isVoltageStabilityDetectorExist(detectorList As ObservableCollection(Of DetectorBase)) As Boolean
            For Each dt In detectorList
                If TypeOf dt Is VoltageStabilityDetectorViewModel Then
                    Return True
                End If
            Next
            Return False
        End Function
        Private Function _isDEFDetectorExist(detectorList As ObservableCollection(Of DetectorBase)) As Boolean
            For Each dt In detectorList
                If TypeOf dt Is DEFDetectorViewModel Then
                    Return True
                End If
            Next
            Return False
        End Function

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
            _aDetectorStepDeSelected()
            Select Case obj
                Case "Ringdown Detector"
                    For Each item In DetectorConfigure.AlarmingList
                        If TypeOf item Is AlarmingRingdown Then
                            Forms.MessageBox.Show("Ringdown alarm detector already added. Each alarming detector can only be added once.", "Error!", MessageBoxButtons.OK)
                            Exit Sub
                        End If
                    Next
                    Dim newDetector = New AlarmingRingdown
                    newDetector.IsExpanded = True
                    DetectorConfigure.AlarmingList.Add(newDetector)
                    _selectedADetector(newDetector)
                Case "Periodogram Forced Oscillation Detector"
                    For Each item In DetectorConfigure.AlarmingList
                        If TypeOf item Is AlarmingPeriodogram Then
                            Forms.MessageBox.Show("Periodogram alarm detector already added. Each alarming detector can only be added once.", "Error!", MessageBoxButtons.OK)
                            Exit Sub
                        End If
                    Next
                    Dim newDetector = New AlarmingPeriodogram
                    newDetector.IsExpanded = True
                    DetectorConfigure.AlarmingList.Add(newDetector)
                    _selectedADetector(newDetector)
                Case "Spectral Coherence Forced Oscillation Detector"
                    For Each item In DetectorConfigure.AlarmingList
                        If TypeOf item Is AlarmingSpectralCoherence Then
                            Forms.MessageBox.Show("Spectral coherence alarm detector already added. Each alarming detector can only be added once.", "Error!", MessageBoxButtons.OK)
                            Exit Sub
                        End If
                    Next
                    Dim newDetector = New AlarmingSpectralCoherence
                    newDetector.IsExpanded = True
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
        Public Sub DeSelectAllDetectors()
            _aDetectorStepDeSelected()
            If CurrentSelectedStep IsNot Nothing Then
                If TypeOf CurrentSelectedStep Is DetectorBase Then
                    For Each signal In _currentSelectedStep.InputChannels
                        signal.IsChecked = False
                    Next
                End If
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllProcessConfigOutputGroupedByType, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllProcessConfigOutputGroupedByPMU, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllDataConfigOutputGroupedByType, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllDataConfigOutputGroupedByPMU, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllPostProcessOutputGroupedByType, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllPostProcessOutputGroupedByPMU, False)

                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedSignalByDetectorInput, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedSignalByDataWriterDetectorInput, False)

                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedRawSignalsByPMU, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedRawSignalsByType, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.ReGroupedRawSignalsByType, False)
                _currentSelectedStep.IsStepSelected = False
                CurrentSelectedStep = Nothing
                '_signalMgr.DetermineFileDirCheckableStatus()
                _determineSamplingRateCheckableStatus()
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
            If detector IsNot CurrentSelectedStep AndAlso CurrentSelectedStep IsNot Nothing Then
                If Not CurrentSelectedStep.CheckStepIsComplete() Then
                    'here need to check if the currentSelectedStep is complete, if not, cannot switch
                    Forms.MessageBox.Show("Missing field(s) in this step, please double check!", "Error!", MessageBoxButtons.OK)
                    Exit Sub
                End If
                'here do all the stuff that is needed such as sort signals to make sure the step is set up.
                If TypeOf CurrentSelectedStep IsNot AlarmingDetectorBase Then
                    CurrentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(_currentSelectedStep.InputChannels)
                End If
            End If
            ' if detector is already selected, then the selection is not changed, nothing needs to be done.
            ' however, if detector is not selected, which means a new selection, we need to find the old selection, unselect it and all it's input signal
            If Not detector.IsStepSelected Then
                _aDetectorStepDeSelected()
                'If CurrentSelectedStep IsNot Nothing AndAlso TypeOf (CurrentSelectedStep) Is DetectorBase AndAlso CurrentSelectedStep.InputChannels.Count = 0 Then
                '    'Forms.MessageBox.Show("Detectors have to have input signals!", "Error!", MessageBoxButtons.OK)
                'Else
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
                    For Each dt In DetectorConfigure.DataWriterDetectorList
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
                    '_signalMgr.DetermineFileDirCheckableStatus()
                    '_determineSamplingRateCheckableStatus()
                    detector.IsStepSelected = True
                    If TypeOf detector Is DetectorBase Then
                        For Each signal In detector.InputChannels
                            signal.IsChecked = True
                        Next
                    End If

                    _signalMgr.DetectorConfigDetermineAllParentNodeStatus()

                    '_signalMgr.DetermineFileDirCheckableStatus()
                    '_determineSamplingRateCheckableStatus()

                    If TypeOf detector Is AlarmingDetectorBase Then
                        SignalSelectionTreeViewVisibility = "Collapsed"
                    Else
                        SignalSelectionTreeViewVisibility = "Visible"
                    End If

                    CurrentSelectedStep = detector
                    If TypeOf CurrentSelectedStep IsNot AlarmingDetectorBase Then
                        _determineSamplingRateCheckableStatus()
                    End If
                Catch ex As Exception
                    Forms.MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
                End Try

                'End If

            End If
        End Sub

        'Private Sub _detectorConfigDetermineAllParentNodeStatus()
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.GroupedRawSignalsByType)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.GroupedRawSignalsByPMU)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.ReGroupedRawSignalsByType)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllDataConfigOutputGroupedByType)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllDataConfigOutputGroupedByPMU)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllProcessConfigOutputGroupedByType)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllProcessConfigOutputGroupedByPMU)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllPostProcessOutputGroupedByType)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllPostProcessOutputGroupedByPMU)
        '    For Each stepInput In _signalMgr.GroupedSignalByDetectorInput
        '        If stepInput.SignalList.Count > 0 Then
        '            _determineParentGroupedByTypeNodeStatus(stepInput.SignalList)
        '            _determineParentCheckStatus(stepInput)
        '        Else
        '            stepInput.SignalSignature.IsChecked = False
        '        End If
        '    Next
        'End Sub

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
            If CurrentSelectedStep IsNot obj Then
                _aDetectorStepDeSelected()
            End If
            Dim result = Forms.MessageBox.Show("Delete detector " & obj.Name & " in Detector Configuration?", "Warning!", MessageBoxButtons.OKCancel)
            If result = DialogResult.OK Then
                Try
                    If TypeOf obj Is DetectorBase Then
                        Dim newlist = New ObservableCollection(Of DetectorBase)(DetectorConfigure.DetectorList)
                        newlist.Remove(obj)
                        DetectorConfigure.DetectorList = newlist
                        _addLog("Detector " & obj.Name & " is deleted!")
                        _signalMgr.GroupedSignalByDetectorInput.Remove(obj.ThisStepInputsAsSignalHerachyByType)
                        For index = 1 To _signalMgr.GroupedSignalByDetectorInput.Count
                            _signalMgr.GroupedSignalByDetectorInput(index - 1).SignalSignature.SignalName = "Detector " & index.ToString & " " & DetectorConfigure.DetectorList(index - 1).Name
                        Next
                        If DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Visible Then
                            Dim updateResultInterval = False
                            For Each dtr In DetectorConfigure.DetectorList
                                If TypeOf dtr Is SpectralCoherenceDetector Or TypeOf dtr Is PeriodogramDetector Or TypeOf dtr Is VoltageStabilityDetectorViewModel Then
                                    updateResultInterval = True
                                    Exit For
                                End If
                            Next
                            If Not updateResultInterval Then
                                DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Collapsed
                            End If
                        End If
                    Else
                        Dim newlist = New ObservableCollection(Of AlarmingDetectorBase)(DetectorConfigure.AlarmingList)
                        newlist.Remove(obj)
                        DetectorConfigure.AlarmingList = newlist
                        _addLog("Alarming detector " & obj.Name & " is deleted from alarming!")
                    End If
                    'If obj Is CurrentSelectedStep Then
                    '    CurrentSelectedStep = Nothing
                    'Else
                    DeSelectAllDetectors()
                    'End If
                Catch ex As Exception
                    Forms.MessageBox.Show("Error deleting detector " & obj.Name & " in Detector Configuration.", "Error!", MessageBoxButtons.OK)
                End Try
            Else
                Exit Sub
            End If
        End Sub
        Private _detectorStepDeSelected As ICommand
        Public Property DetectorStepDeSelected As ICommand
            Get
                Return _detectorStepDeSelected
            End Get
            Set(ByVal value As ICommand)
                _detectorStepDeSelected = value
            End Set
        End Property

        Private Sub _aDetectorStepDeSelected()
            If CurrentSelectedStep IsNot Nothing AndAlso TypeOf (CurrentSelectedStep) Is DetectorBase AndAlso Not CurrentSelectedStep.CheckStepIsComplete Then
                Forms.MessageBox.Show("Detectors have to have input signals!", "Error!", MessageBoxButtons.OK)
            End If
        End Sub

#End Region

#Region "Data writer detector tab controls"
        Private _addDataWriterDetector As ICommand
        Public Property AddDataWriterDetector As ICommand
            Get
                Return _addDataWriterDetector
            End Get
            Set(value As ICommand)
                _addDataWriterDetector = value
            End Set
        End Property
        Private Sub _addADataWriterDetector(obj As Object)
            _aDetectorStepDeSelected()
            Dim newDetector = New DataWriterDetectorViewModel
            newDetector.IsExpanded = True
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Detector " & (_signalMgr.GroupedSignalByDataWriterDetectorInput.Count + 1).ToString & " " & newDetector.Name
            _signalMgr.GroupedSignalByDataWriterDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            DetectorConfigure.DataWriterDetectorList.Add(newDetector)
            _selectedADetector(newDetector)
        End Sub
        Private _deleteDataWriterDetector As ICommand
        Public Property DeleteDataWriterDetector As ICommand
            Get
                Return _deleteDataWriterDetector
            End Get
            Set(ByVal value As ICommand)
                _deleteDataWriterDetector = value
            End Set
        End Property
        Private Sub _deleteADataWriterDetector(obj As Object)
            If CurrentSelectedStep IsNot obj Then
                _aDetectorStepDeSelected()
            End If
            Dim result = Forms.MessageBox.Show("Delete this data writer?", "Warning!", MessageBoxButtons.OKCancel)
            If result = DialogResult.OK Then
                Try
                    Dim newlist = New ObservableCollection(Of DetectorBase)(DetectorConfigure.DataWriterDetectorList)
                    newlist.Remove(obj)
                    DetectorConfigure.DataWriterDetectorList = newlist
                    _addLog("Detector " & obj.Name & " is deleted!")
                    _signalMgr.GroupedSignalByDataWriterDetectorInput.Remove(obj.ThisStepInputsAsSignalHerachyByType)
                    For index = 1 To _signalMgr.GroupedSignalByDataWriterDetectorInput.Count
                        _signalMgr.GroupedSignalByDataWriterDetectorInput(index - 1).SignalSignature.SignalName = "Detector " & index.ToString & " " & DetectorConfigure.DataWriterDetectorList(index - 1).Name
                    Next
                    'If obj Is CurrentSelectedStep Then
                    '    CurrentSelectedStep = Nothing
                    'Else
                    DeSelectAllDetectors()
                    'End If
                Catch ex As Exception
                    Forms.MessageBox.Show("Error deleting detector " & obj.Name & " in Detector Configuration.", "Error!", MessageBoxButtons.OK)
                End Try
            Else
                Exit Sub
            End If
        End Sub
#End Region
    End Class

End Namespace
