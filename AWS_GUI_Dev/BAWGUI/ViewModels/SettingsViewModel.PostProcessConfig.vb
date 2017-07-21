Imports System.Collections.ObjectModel
Imports System.Windows.Forms

Partial Public Class SettingsViewModel
    Private _groupedSignalByPostProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByPostProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByPostProcessConfigStepsInput
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByPostProcessConfigStepsInput = value
            OnPropertyChanged()
        End Set
    End Property

    Private _groupedSignalByPostProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByPostProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByPostProcessConfigStepsOutput
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByPostProcessConfigStepsOutput = value
            OnPropertyChanged()
        End Set
    End Property

    Private _allPostProcessOutputGroupedByType As ObservableCollection(Of SignalTypeHierachy)
    Public Property AllPostProcessOutputGroupedByType As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _allPostProcessOutputGroupedByType
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _allPostProcessOutputGroupedByType = value
            OnPropertyChanged()
        End Set
    End Property

    Private _allPostProcessOutputGroupedByPMU As ObservableCollection(Of SignalTypeHierachy)
    Public Property AllPostProcessOutputGroupedByPMU As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _allPostProcessOutputGroupedByPMU
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _allPostProcessOutputGroupedByPMU = value
            OnPropertyChanged()
        End Set
    End Property

    Private _postProcessConfigStepSelected As ICommand
    Public Property PostProcessConfigStepSelected As ICommand
        Get
            Return _postProcessConfigStepSelected
        End Get
        Set(ByVal value As ICommand)
            _postProcessConfigStepSelected = value
        End Set
    End Property
    Private Sub _postProcessConfigureStepSelected(processStep As Object)
        ' if processStep is already selected, then the selection is not changed, nothing needs to be done.
        ' however, if processStep is not selected, which means a new selection, we need to find the old selection, unselect it and all it's input signal
        If Not processStep.IsStepSelected Then
            Try
                Dim lastNumberOfSteps = processStep.StepCounter
                Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                Dim selectedFound = False
                For Each stp In PostProcessConfigure.CollectionOfSteps
                    If stp.IsStepSelected Then
                        stp.IsStepSelected = False
                        For Each signal In stp.InputChannels
                            signal.IsChecked = False
                        Next
                        selectedFound = True
                    End If
                    If stp.StepCounter < lastNumberOfSteps Then
                        stp.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(stp.InputChannels)
                        stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                        stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(stp.OutputChannels)
                        stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                    End If
                    If stp.StepCounter >= lastNumberOfSteps AndAlso selectedFound Then
                        Exit For
                    End If
                Next
                _determineFileDirCheckableStatus()
                processStep.IsStepSelected = True

                For Each signal In processStep.InputChannels
                    signal.IsChecked = True
                Next

                If processStep.Name = "Scalar Repetition Customization" Then
                    SignalSelectionTreeViewVisibility = "Collapsed"
                Else
                    SignalSelectionTreeViewVisibility = "Visible"
                End If

                If CurrentSelectedStep IsNot Nothing Then
                    If CurrentSelectedStep.Name = "Phasor Creation Customization" Then
                        _disableEnableAllButMagnitudeSignalsInPostProcessConfig(True)
                    ElseIf CurrentSelectedStep.Name = "Power Calculation Customization" Then
                        If CurrentSelectedStep.OutputInputMappingPair.Count > 0 Then
                            Dim situation = CurrentSelectedStep.OutputInputMappingPair(0).Value.Count
                            If situation = 4 Then
                                _disableEnableAllButMagnitudeSignalsInPostProcessConfig(True)
                            ElseIf situation = 2 Then
                                _disableEnableAllButPhasorSignalsInPostProcessConfig(True)
                            End If
                        End If
                    ElseIf CurrentSelectedStep.Name = "Metric Prefix Customization" Then
                        _disableEnableAllButMagnitudeFrequencyROCOFSignalsInPostProcessConfig(True)
                    ElseIf CurrentSelectedStep.Name = "Angle Conversion Customization" Then
                        _disableEnableAllButAngleSignalsInPostProcessConfig(True)
                    End If
                End If

                GroupedSignalByPostProcessConfigStepsInput = stepsInputAsSignalHierachy
                GroupedSignalByPostProcessConfigStepsOutput = stepsOutputAsSignalHierachy
                _postProcessDetermineAllParentNodeStatus()
                _determineFileDirCheckableStatus()

                If processStep.Name = "Phasor Creation Customization" Then
                    _disableEnableAllButMagnitudeSignalsInPostProcessConfig(False)
                ElseIf processStep.Name = "Power Calculation Customization" Then
                    If processStep.OutputInputMappingPair.Count > 0 Then
                        Dim situation = processStep.OutputInputMappingPair(0).Value.Count
                        If situation = 4 Then
                            _disableEnableAllButMagnitudeSignalsInPostProcessConfig(False)
                        ElseIf situation = 2 Then
                            _disableEnableAllButPhasorSignalsInPostProcessConfig(False)
                        End If
                    End If
                ElseIf processStep.Name = "Metric Prefix Customization" Then
                    _disableEnableAllButMagnitudeFrequencyROCOFSignalsInPostProcessConfig(False)
                ElseIf processStep.Name = "Angle Conversion Customization" Then
                    _disableEnableAllButAngleSignalsInPostProcessConfig(False)
                End If
                CurrentSelectedStep = processStep
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        End If

    End Sub

    Private Sub _postProcessDetermineAllParentNodeStatus()
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType)
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU)
        _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType)
        _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU)
        _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByType)
        _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByPMU)
        For Each stepInput In GroupedSignalByPostProcessConfigStepsInput
            If stepInput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepInput.SignalList)
                _determineParentCheckStatus(stepInput)
            Else
                stepInput.SignalSignature.IsChecked = False
            End If
        Next
        For Each stepOutput In GroupedSignalByPostProcessConfigStepsOutput
            If stepOutput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList)
                _determineParentCheckStatus(stepOutput)
            Else
                stepOutput.SignalSignature.IsChecked = False
            End If
        Next
    End Sub

    Private _postProcessConfigStepDeSelected As ICommand
    Public Property PostProcessConfigStepDeSelected As ICommand
        Get
            Return _postProcessConfigStepDeSelected
        End Get
        Set(ByVal value As ICommand)
            _postProcessConfigStepDeSelected = value
        End Set
    End Property
    Private Sub _deSelectAllPostProcessConfigSteps()
        If _currentSelectedStep IsNot Nothing Then
            For Each signal In _currentSelectedStep.InputChannels
                signal.IsChecked = False
            Next
            Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
            Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
            For Each stp In PostProcessConfigure.CollectionOfSteps
                stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
            Next
            GroupedSignalByPostProcessConfigStepsInput = stepsInputAsSignalHierachy
            GroupedSignalByPostProcessConfigStepsOutput = stepsOutputAsSignalHierachy
            If CurrentSelectedStep.Name = "Phasor Creation Customization" Then
                _disableEnableAllButMagnitudeSignalsInPostProcessConfig(True)
            ElseIf CurrentSelectedStep.Name = "Power Calculation Customization" Then
                If CurrentSelectedStep.OutputInputMappingPair.Count > 0 Then
                    Dim situation = CurrentSelectedStep.OutputInputMappingPair(0).Value.Count
                    If situation = 4 Then
                        _disableEnableAllButMagnitudeSignalsInPostProcessConfig(True)
                    Else
                        _disableEnableAllButPhasorSignalsInPostProcessConfig(True)
                    End If
                End If
            ElseIf CurrentSelectedStep.Name = "Metric Prefix Customization" Then
                _disableEnableAllButMagnitudeFrequencyROCOFSignalsInPostProcessConfig(True)
            ElseIf CurrentSelectedStep.Name = "Angle Conversion Customization" Then
                _disableEnableAllButAngleSignalsInPostProcessConfig(True)
            End If
            _changeCheckStatusAllParentsOfGroupedSignal(AllProcessConfigOutputGroupedByType, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllProcessConfigOutputGroupedByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllDataConfigOutputGroupedByType, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllDataConfigOutputGroupedByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByPostProcessConfigStepsInput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByPostProcessConfigStepsOutput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByType, False)
            _currentSelectedStep.IsStepSelected = False
            CurrentSelectedStep = Nothing
            _determineFileDirCheckableStatus()
        End If
        SignalSelectionTreeViewVisibility = "Visible"
    End Sub

    Private Sub _disableEnableAllButAngleSignalsInPostProcessConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "A" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedRawSignalsByPMU
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
        For Each group In AllDataConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subsubgroup In group.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "A" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllDataConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse subgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next
        For Each group In AllProcessConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subsubgroup In group.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "A" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllProcessConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse subgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "A" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
    End Sub
    Private Sub _disableEnableAllButMagnitudeFrequencyROCOFSignalsInPostProcessConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "R" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 2 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M") Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedRawSignalsByPMU
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "R") OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M") Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
        For Each group In AllDataConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" AndAlso group.SignalSignature.TypeAbbreviation <> "F" AndAlso group.SignalSignature.TypeAbbreviation <> "R" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subgroup In group.SignalList
                    If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse (subgroup.SignalSignature.TypeAbbreviation.Length = 2 AndAlso subgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M") Then
                        subgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllDataConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse (subgroup.SignalSignature.TypeAbbreviation.Length <> 3 AndAlso subgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "R") OrElse (subgroup.SignalSignature.TypeAbbreviation.Length = 3 AndAlso subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M") Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next
        For Each group In AllProcessConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" AndAlso group.SignalSignature.TypeAbbreviation <> "F" AndAlso group.SignalSignature.TypeAbbreviation <> "R" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subgroup In group.SignalList
                    If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse (subgroup.SignalSignature.TypeAbbreviation.Length = 2 AndAlso subgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M") Then
                        subgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllProcessConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse (subgroup.SignalSignature.TypeAbbreviation.Length <> 3 AndAlso subgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "R") OrElse (subgroup.SignalSignature.TypeAbbreviation.Length = 3 AndAlso subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M") Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "R" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 2 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M") Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "R") OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M") Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
    End Sub
    Private Sub _disableEnableAllButPhasorSignalsInPostProcessConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            group.SignalSignature.IsEnabled = isEnabled
        Next
        For Each group In GroupedRawSignalsByPMU
            group.SignalSignature.IsEnabled = isEnabled
        Next
        For Each group In AllDataConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subsubgroup In group.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "P" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllDataConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse subgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "P" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next
        For Each group In AllProcessConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subsubgroup In group.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "P" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllProcessConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse subgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "P" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "P" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "P" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
    End Sub
    Private Sub _disableEnableAllButMagnitudeSignalsInPostProcessConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedRawSignalsByPMU
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next




        For Each group In AllDataConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subsubgroup In group.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllDataConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse subgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next
        For Each group In AllProcessConfigOutputGroupedByType
            If group.SignalSignature.TypeAbbreviation <> "I" AndAlso group.SignalSignature.TypeAbbreviation <> "V" Then
                group.SignalSignature.IsEnabled = isEnabled
            Else
                For Each subsubgroup In group.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            End If
        Next
        For Each group In AllProcessConfigOutputGroupedByPMU
            For Each subgroup In group.SignalList
                If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse subgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                End If
            Next
        Next







        For Each group In GroupedSignalByPostProcessConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next

    End Sub

    Private _deletePostProcessStep As ICommand
    Public Property DeletePostProcessStep As ICommand
        Get
            Return _deletePostProcessStep
        End Get
        Set(ByVal value As ICommand)
            _deletePostProcessStep = value
        End Set
    End Property
    Private Sub _deleteAPostProcessStep(obj As Object)
        Dim result = MessageBox.Show("Delete step " & obj.StepCounter.ToString & " in Post Process Configuration: " & obj.Name & " ?", "Warning!", MessageBoxButtons.OKCancel)
        If result = DialogResult.OK Then
            Try
                PostProcessConfigure.CollectionOfSteps.Remove(obj)
                Dim steps = New ObservableCollection(Of Customization)(PostProcessConfigure.CollectionOfSteps)
                For Each stp In steps
                    If stp.StepCounter > obj.StepCounter Then
                        stp.StepCounter -= 1
                        If TypeOf (stp) Is Customization Then
                            stp.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stp.StepCounter.ToString & "-" & stp.Name
                        End If
                        stp.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stp.StepCounter.ToString & "-" & stp.Name
                    End If
                Next
                _deSelectAllPostProcessConfigSteps()
                _addLog("Step " & obj.StepCounter & ", " & obj.Name & " is deleted!")
                PostProcessConfigure.CollectionOfSteps = steps
                SignalSelectionTreeViewVisibility = "Visible"
            Catch ex As Exception
                MessageBox.Show("Error deleting step " & obj.StepCounter.ToString & " in Post Process Configuration, " & obj.Name & ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        Else
            Exit Sub
        End If
    End Sub

    Private Sub _groupAllPostProcessConfigOutputSignal()
        Dim allOutputSignals = New ObservableCollection(Of SignalSignatures)
        For Each stp In PostProcessConfigure.CollectionOfSteps
            For Each signal In stp.OutputChannels
                If Not allOutputSignals.Contains(signal) Then
                    allOutputSignals.Add(signal)
                End If
            Next
        Next
        AllPostProcessOutputGroupedByType = SortSignalByType(allOutputSignals)
        AllPostProcessOutputGroupedByPMU = SortSignalByPMU(allOutputSignals)
    End Sub
End Class
