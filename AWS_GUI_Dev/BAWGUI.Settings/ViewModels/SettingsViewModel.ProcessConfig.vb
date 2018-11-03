Imports System.Collections.ObjectModel
Imports System.Windows.Forms
Imports System.Windows.Input
Imports BAWGUI.Core

Namespace ViewModels
    Partial Public Class SettingsViewModel
        'Private _groupedSignalByProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
        'Public Property GroupedSignalByProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
        '    Get
        '        Return _groupedSignalByProcessConfigStepsInput
        '    End Get
        '    Set(value As ObservableCollection(Of SignalTypeHierachy))
        '        _groupedSignalByProcessConfigStepsInput = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        'Private _groupedSignalByProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
        'Public Property GroupedSignalByProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
        '    Get
        '        Return _groupedSignalByProcessConfigStepsOutput
        '    End Get
        '    Set(value As ObservableCollection(Of SignalTypeHierachy))
        '        _groupedSignalByProcessConfigStepsOutput = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        Private Function _getAllprocessOutputSignals() As ObservableCollection(Of SignalSignatureViewModel)
            Dim allOutputSignals = New ObservableCollection(Of Core.SignalSignatureViewModel)
            For Each uwrp In ProcessConfigure.UnWrapList
                For Each signal In uwrp.OutputChannels
                    If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
                        allOutputSignals.Add(signal)
                    End If
                Next
            Next
            For Each itpl In ProcessConfigure.InterpolateList
                For Each signal In itpl.OutputChannels
                    If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
                        allOutputSignals.Add(signal)
                    End If
                Next
            Next
            For Each stp In ProcessConfigure.CollectionOfSteps
                For Each signal In stp.OutputChannels
                    If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
                        allOutputSignals.Add(signal)
                    End If
                Next
            Next
            For Each wrp In ProcessConfigure.WrapList
                For Each signal In wrp.OutputChannels
                    If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
                        allOutputSignals.Add(signal)
                    End If
                Next
            Next
            If NameTypeUnitStatusFlag Then
                'TODO: NameTypeUnit approach 1 that is obsolete where all signal in this system will be changed
            Else
                For Each change In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                    For Each signal In change.OutputChannels
                        If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
                            allOutputSignals.Add(signal)
                        End If
                    Next
                Next
            End If
            Return allOutputSignals
        End Function
        'Private _allProcessConfigOutputGroupedByType As ObservableCollection(Of SignalTypeHierachy)
        'Public Property AllProcessConfigOutputGroupedByType As ObservableCollection(Of SignalTypeHierachy)
        '    Get
        '        Return _allProcessConfigOutputGroupedByType
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
        '        _allProcessConfigOutputGroupedByType = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        ''Private Function _getAllProcessConfigOutputGroupedByPMU() As ObservableCollection(Of SignalTypeHierachy)
        ''    Dim allOutputSignals = New ObservableCollection(Of SignalSignatures)
        ''    For Each uwrp In ProcessConfigure.UnWrapList
        ''        For Each signal In uwrp.OutputChannels
        ''            If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
        ''                allOutputSignals.Add(signal)
        ''            End If
        ''        Next
        ''    Next
        ''    For Each itpl In ProcessConfigure.InterpolateList
        ''        For Each signal In itpl.OutputChannels
        ''            If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
        ''                allOutputSignals.Add(signal)
        ''            End If
        ''        Next
        ''    Next
        ''    For Each stp In ProcessConfigure.CollectionOfSteps
        ''        For Each signal In stp.OutputChannels
        ''            If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
        ''                allOutputSignals.Add(signal)
        ''            End If
        ''        Next
        ''    Next
        ''    For Each wrp In ProcessConfigure.WrapList
        ''        For Each signal In wrp.OutputChannels
        ''            If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
        ''                allOutputSignals.Add(signal)
        ''            End If
        ''        Next
        ''    Next
        ''    If NameTypeUnitStatusFlag Then
        ''        'TODO: NameTypeUnit approach 1 that is obsolete where all signal in this system will be changed
        ''    Else
        ''        For Each change In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
        ''            For Each signal In change.OutputChannels
        ''                If Not allOutputSignals.Contains(signal) AndAlso signal.IsSignalInformationComplete Then
        ''                    allOutputSignals.Add(signal)
        ''                End If
        ''            Next
        ''        Next
        ''    End If
        ''    Return SortSignalByPMU(allOutputSignals)
        ''End Function
        'Private _allProcessConfigOutputGroupedByPMU As ObservableCollection(Of SignalTypeHierachy)
        'Public Property AllProcessConfigOutputGroupedByPMU As ObservableCollection(Of SignalTypeHierachy)
        '    Get
        '        Return _allProcessConfigOutputGroupedByPMU
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
        '        _allProcessConfigOutputGroupedByPMU = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
        Private _nameTypeUnitStatusFlag As Integer
        ''' <summary>
        ''' This Flag denotes the rare situation(might never happend since obsolete) when there is NameTypeUnit Element exists and use the first approach(1) in the Processor XML Configuration File specifications, otherwise, 2nd approach(0)
        ''' </summary>
        ''' <returns></returns>
        Public Property NameTypeUnitStatusFlag As Integer
            Get
                Return _nameTypeUnitStatusFlag
            End Get
            Set(ByVal value As Integer)
                _nameTypeUnitStatusFlag = value
                OnPropertyChanged()
            End Set
        End Property

#Region "Add Or Delete Process Config Steps"
        Private _addUnwrap As ICommand
        Public Property AddUnwrap As ICommand
            Get
                Return _addUnwrap
            End Get
            Set(value As ICommand)
                _addUnwrap = value
            End Set
        End Property
        Private Sub _addAUnwrap(obj As Object)
            Dim aNewUnwrap = New Unwrap
            aNewUnwrap.IsExpanded = True
            aNewUnwrap.StepCounter = ProcessConfigure.UnWrapList.Count + 1
            aNewUnwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aNewUnwrap.StepCounter.ToString & " - " & aNewUnwrap.Name
            aNewUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aNewUnwrap.StepCounter.ToString & " - " & aNewUnwrap.Name
            'GroupedSignalByProcessConfigStepsOutput.Insert(aNewUnwrap.StepCounter - 1, aNewUnwrap.ThisStepOutputsAsSignalHierachyByPMU)
            ProcessConfigure.UnWrapList.Add(aNewUnwrap)

            Dim intpltList = New ObservableCollection(Of Interpolate)(ProcessConfigure.InterpolateList)
            For Each interpolate In intpltList
                interpolate.StepCounter += 1
                interpolate.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
                interpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
            Next
            ProcessConfigure.InterpolateList = intpltList
            Dim steps = New ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
            For Each aStep In steps
                aStep.StepCounter += 1
                If TypeOf aStep Is TunableFilter Then
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                Else
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                End If
            Next
            ProcessConfigure.CollectionOfSteps = steps
            Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
            For Each wrap In wraps
                wrap.StepCounter += 1
                wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
                wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            Next
            ProcessConfigure.WrapList = wraps
            If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                For Each newTypeUnit In newTypeUnits
                    newTypeUnit.StepCounter += 1
                    newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                    newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                Next
                ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
            End If
            _processStepSelectedToEdit(aNewUnwrap)
        End Sub

        Private _deleteUnwrapStep As ICommand
        Public Property DeleteUnwrapStep As ICommand
            Get
                Return _deleteUnwrapStep
            End Get
            Set(value As ICommand)
                _deleteUnwrapStep = value
            End Set
        End Property
        Private Sub _deleteAUnwrap(obj As Unwrap)
            Dim result = MessageBox.Show("Delete an Unwrap step " & obj.StepCounter.ToString & " in Process Configuration: " & obj.Name & " ?", "Warning!", MessageBoxButtons.OKCancel)
            If result = DialogResult.OK Then
                Try
                    ProcessConfigure.UnWrapList.Remove(obj)
                    Dim unwraps = New ObservableCollection(Of Unwrap)(ProcessConfigure.UnWrapList)
                    For Each unwrap In unwraps
                        If unwrap.StepCounter > obj.StepCounter Then
                            unwrap.StepCounter -= 1
                            unwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & unwrap.StepCounter.ToString & " - " & unwrap.Name
                            unwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & unwrap.StepCounter.ToString & " - " & unwrap.Name
                        End If
                    Next
                    ProcessConfigure.UnWrapList = unwraps
                    Dim intplts = New ObservableCollection(Of Interpolate)(ProcessConfigure.InterpolateList)
                    For Each interpolate In intplts
                        interpolate.StepCounter -= 1
                        interpolate.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
                        interpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
                    Next
                    ProcessConfigure.InterpolateList = intplts
                    Dim steps = New ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
                    For Each aStep In steps
                        aStep.StepCounter -= 1
                        If TypeOf aStep Is TunableFilter Then
                            aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                        Else
                            aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                        End If
                    Next
                    ProcessConfigure.CollectionOfSteps = steps
                    Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
                    For Each wrap In wraps
                        wrap.StepCounter -= 1
                        wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
                        wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
                    Next
                    ProcessConfigure.WrapList = wraps
                    If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                        Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                        For Each newTypeUnit In newTypeUnits
                            newTypeUnit.StepCounter -= 1
                            newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                            newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                        Next
                        ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
                    End If
                    For Each signal In obj.OutputChannels
                        signal.PassedThroughProcessor = signal.PassedThroughProcessor - 1
                    Next
                    _deSelectAllProcessConfigSteps()
                    _addLog("Unwrap step " & obj.StepCounter & " is deleted!")
                Catch ex As Exception
                    MessageBox.Show("Error deleting a unwrap step " & obj.StepCounter.ToString & ", " & obj.Name & ex.Message, "Error!", MessageBoxButtons.OK)
                End Try
            Else
                Exit Sub
            End If
        End Sub

        Private _addInterpolate As ICommand
        Public Property AddInterpolate As ICommand
            Get
                Return _addInterpolate
            End Get
            Set(value As ICommand)
                _addInterpolate = value
            End Set
        End Property
        Private Sub _addAnInterpolate(obj As Object)
            Dim anInterpolate = New Interpolate
            anInterpolate.IsExpanded = True
            anInterpolate.StepCounter = ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + 1
            anInterpolate.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & anInterpolate.StepCounter.ToString & " - " & anInterpolate.Name
            anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & anInterpolate.StepCounter.ToString & " - " & anInterpolate.Name
            'GroupedSignalByProcessConfigStepsOutput.Insert(ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count, anInterpolate.ThisStepOutputsAsSignalHierachyByPMU)
            ProcessConfigure.InterpolateList.Add(anInterpolate)
            Dim steps = New ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
            For Each aStep In steps
                aStep.StepCounter += 1
                If TypeOf aStep Is TunableFilter Then
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                Else
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                End If
            Next
            ProcessConfigure.CollectionOfSteps = steps
            Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
            For Each wrap In wraps
                wrap.StepCounter += 1
                wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
                wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            Next
            ProcessConfigure.WrapList = wraps
            If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                For Each newTypeUnit In newTypeUnits
                    newTypeUnit.StepCounter += 1
                    newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                    newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                Next
                ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
            End If
            _processStepSelectedToEdit(anInterpolate)
        End Sub

        Private _deleteInterpolateStep As ICommand
        Public Property DeleteInterpolateStep As ICommand
            Get
                Return _deleteInterpolateStep
            End Get
            Set(value As ICommand)
                _deleteInterpolateStep = value
            End Set
        End Property
        Private Sub _deleteAnInterpolate(obj As Interpolate)
            Dim result = MessageBox.Show("Delete an Interpolate step " & obj.StepCounter.ToString & " in Process Configuration: " & obj.Name & " ?", "Warning!", MessageBoxButtons.OKCancel)
            If result = DialogResult.OK Then
                Try
                    ProcessConfigure.InterpolateList.Remove((obj))
                    Dim intplts = New ObservableCollection(Of Interpolate)(ProcessConfigure.InterpolateList)
                    For Each intplt In intplts
                        If intplt.StepCounter > obj.StepCounter Then
                            intplt.StepCounter -= 1
                            intplt.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & intplt.StepCounter.ToString & " - " & intplt.Type.ToString() & " " & intplt.Name
                            intplt.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & intplt.StepCounter.ToString & " - " & intplt.Type.ToString() & " " & intplt.Name
                        End If
                    Next
                    ProcessConfigure.InterpolateList = intplts
                    Dim steps = New ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
                    For Each aStep In steps
                        aStep.StepCounter -= 1
                        If TypeOf aStep Is TunableFilter Then
                            aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                        Else
                            aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                        End If
                    Next
                    ProcessConfigure.CollectionOfSteps = steps
                    Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
                    For Each wrap In wraps
                        wrap.StepCounter -= 1
                        wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
                        wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
                    Next
                    ProcessConfigure.WrapList = wraps
                    If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                        Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                        For Each newTypeUnit In newTypeUnits
                            newTypeUnit.StepCounter -= 1
                            newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                            newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                        Next
                        ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
                    End If
                    For Each signal In obj.OutputChannels
                        signal.PassedThroughProcessor = signal.PassedThroughProcessor - 1
                    Next
                    _deSelectAllProcessConfigSteps()
                    _addLog("Interpolate step " & obj.StepCounter & " is deleted!")
                Catch ex As Exception
                    MessageBox.Show("Error deleting an interpolate step " & obj.StepCounter.ToString & ", " & obj.Name & ex.Message, "Error!", MessageBoxButtons.OK)
                End Try
            Else
                Exit Sub
            End If
        End Sub

        Private _addTunableFilterOrMultirate As ICommand
        Public Property AddTunableFilterOrMultirate As ICommand
            Get
                Return _addTunableFilterOrMultirate
            End Get
            Set(value As ICommand)
                _addTunableFilterOrMultirate = value
            End Set
        End Property
        Private Sub _addATunableFilterOrMultirate(obj As String)
            Dim aStep As Object
            Select Case obj
                Case "Filter"
                    aStep = New TunableFilter
                Case "Multirate"
                    aStep = New Multirate
                Case Else
                    _addLog("Can only add tunable Filter or Multirate in the stages. But " & obj & " requested.")
            End Select
            aStep.IsExpanded = True
            aStep.StepCounter = ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + ProcessConfigure.CollectionOfSteps.Count + 1
            If TypeOf aStep Is TunableFilter Then
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
            Else
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                'GroupedSignalByProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            End If
            'GroupedSignalByProcessConfigStepsOutput.Insert(ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + ProcessConfigure.CollectionOfSteps.Count, aStep.ThisStepOutputsAsSignalHierachyByPMU)
            ProcessConfigure.CollectionOfSteps.Add(aStep)
            Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
            For Each wrap In wraps
                wrap.StepCounter += 1
                wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
                wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            Next
            ProcessConfigure.WrapList = wraps
            If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                For Each newTypeUnit In newTypeUnits
                    newTypeUnit.StepCounter += 1
                    newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                    newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                Next
                ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
            End If
            _processStepSelectedToEdit(aStep)
        End Sub

        Private _deleteTunableFilterOrMultirate As ICommand
        Public Property DeleteTunableFilterOrMultirate As ICommand
            Get
                Return _deleteTunableFilterOrMultirate
            End Get
            Set(value As ICommand)
                _deleteTunableFilterOrMultirate = value
            End Set
        End Property
        Private Sub _deleteATunableFilterOrMultirate(obj As Object)
            Dim result = MessageBox.Show("Delete step " & obj.StepCounter.ToString & " in Process Configuration: " & obj.Name & " ?", "Warning!", MessageBoxButtons.OKCancel)
            If result = DialogResult.OK Then
                Try
                    ProcessConfigure.CollectionOfSteps.Remove((obj))
                    Dim steps = New ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
                    For Each aStep In steps
                        If aStep.StepCounter > obj.StepCounter Then
                            aStep.StepCounter -= 1
                            If TypeOf aStep Is TunableFilter Then
                                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                            Else
                                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                            End If
                        End If
                    Next
                    ProcessConfigure.CollectionOfSteps = steps
                    Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
                    For Each wrap In wraps
                        wrap.StepCounter -= 1
                        wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
                        wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
                    Next
                    ProcessConfigure.WrapList = wraps
                    If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                        Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                        For Each newTypeUnit In newTypeUnits
                            newTypeUnit.StepCounter -= 1
                            newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                            newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                        Next
                        ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
                    End If
                    If TypeOf obj Is TunableFilter Then
                        For Each signal In obj.OutputChannels
                            signal.PassedThroughProcessor = signal.PassedThroughProcessor - 1
                        Next
                    End If
                    _deSelectAllProcessConfigSteps()
                    _addLog("Step " & obj.StepCounter.ToString & ", " & obj.Name & " is deleted!")
                Catch ex As Exception
                    MessageBox.Show("Error deleting a step " & obj.StepCounter.ToString & ", " & obj.Name & ex.Message, "Error!", MessageBoxButtons.OK)
                End Try
            Else
                Exit Sub
            End If
        End Sub

        Private _addWrap As ICommand
        Public Property AddWrap As ICommand
            Get
                Return _addWrap
            End Get
            Set(value As ICommand)
                _addWrap = value
            End Set
        End Property
        Private Sub _addAWrap(obj As Object)
            Dim wrap = New Wrap
            wrap.IsExpanded = True
            wrap.StepCounter = ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + ProcessConfigure.CollectionOfSteps.Count + ProcessConfigure.WrapList.Count + 1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            'GroupedSignalByProcessConfigStepsOutput.Add(wrap.ThisStepOutputsAsSignalHierachyByPMU)
            ProcessConfigure.WrapList.Add(wrap)
            If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                For Each newTypeUnit In newTypeUnits
                    newTypeUnit.StepCounter += 1
                    newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                    newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                Next
                ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
            End If
            _processStepSelectedToEdit(wrap)
        End Sub

        Private _deleteWrap As ICommand
        Public Property DeleteWrap As ICommand
            Get
                Return _deleteWrap
            End Get
            Set(value As ICommand)
                _deleteWrap = value
            End Set
        End Property
        Private Sub _deleteAWrap(obj As Wrap)
            Dim result = MessageBox.Show("Delete a Wrap step " & obj.StepCounter.ToString & " in Process Configuration: " & obj.Name & " ?", "Warning!", MessageBoxButtons.OKCancel)
            If result = DialogResult.OK Then
                Try
                    ProcessConfigure.WrapList.Remove((obj))
                    Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
                    For Each wrap In wraps
                        If wrap.StepCounter > obj.StepCounter Then
                            wrap.StepCounter -= 1
                            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
                            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
                        End If
                    Next
                    ProcessConfigure.WrapList = wraps
                    If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                        Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                        For Each newTypeUnit In newTypeUnits
                            newTypeUnit.StepCounter -= 1
                            newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                            newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
                        Next
                        ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
                    End If
                    For Each signal In obj.OutputChannels
                        signal.PassedThroughProcessor = signal.PassedThroughProcessor - 1
                    Next
                    _deSelectAllProcessConfigSteps()
                    _addLog("Wrap step " & obj.StepCounter & " is deleted!")
                Catch ex As Exception
                    MessageBox.Show("Error deleting a wrap step " & obj.StepCounter.ToString & ", " & obj.Name & ex.Message, "Error!", MessageBoxButtons.OK)
                End Try
            Else
                Exit Sub
            End If
        End Sub

        Private _addNameTypeUnit As ICommand
        Public Property AddNameTypeUnit As ICommand
            Get
                Return _addNameTypeUnit
            End Get
            Set(value As ICommand)
                _addNameTypeUnit = value
            End Set
        End Property
        Private Sub _addANameTypeUnit(obj As Object)
            Dim newTypeUnit = New NameTypeUnitPMU
            newTypeUnit.IsExpanded = True
            newTypeUnit.StepCounter = ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + ProcessConfigure.CollectionOfSteps.Count + ProcessConfigure.WrapList.Count + ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count + 1
            newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
            newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & " - " & newTypeUnit.Name
            ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Add(newTypeUnit)
            _processStepSelectedToEdit(newTypeUnit)
        End Sub

        Private _deleteNameTypeUnit As ICommand
        Public Property DeleteNameTypeUnit As ICommand
            Get
                Return _deleteNameTypeUnit
            End Get
            Set(value As ICommand)
                _deleteNameTypeUnit = value
            End Set
        End Property
        Private Sub _deleteANameTypeUnit(obj As Object)
            Dim result = MessageBox.Show("Delete a NameTypeUnit step " & obj.StepCounter.ToString & " in Process Configuration: " & obj.Name & " ?", "Warning!", MessageBoxButtons.OKCancel)
            If result = DialogResult.OK Then
                Try
                    ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Remove((obj))
                    Dim newTypeUnits = New ObservableCollection(Of NameTypeUnitPMU)(ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList)
                    For Each newTypeUnit In newTypeUnits
                        If newTypeUnit.StepCounter > obj.StepCounter Then
                            newTypeUnit.StepCounter -= 1
                            newTypeUnit.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & "-" & newTypeUnit.Name
                            newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newTypeUnit.StepCounter.ToString & "-" & newTypeUnit.Name
                        End If
                    Next
                    ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList = newTypeUnits
                    _deSelectAllProcessConfigSteps()
                    If NameTypeUnitStatusFlag Then
                        'TODO: NameTypeUnit approach one, obsolete
                    Else
                        If obj.OutputChannels.Count = 1 AndAlso Not String.IsNullOrEmpty(obj.OutputChannels(0).OldSignalName) Then
                            obj.OutputChannels(0).SignalName = obj.OutputChannels(0).OldSignalName
                            obj.OutputChannels(0).OldSignalName = ""
                        End If
                        For Each signal In obj.OutputChannels
                            signal.TypeAbbreviation = signal.OldTypeAbbreviation
                            signal.OldTypeAbbreviation = ""
                            signal.Unit = signal.OldUnit
                            signal.OldUnit = ""
                            signal.IsNameTypeUnitChanged = False
                            signal.PassedThroughProcessor = signal.PassedThroughProcessor - 1
                        Next
                    End If
                    _addLog("NameTypeUnit step " & obj.StepCounter & " is deleted!")
                Catch ex As Exception
                    MessageBox.Show("Error deleting a NameTypeUnit step " & obj.StepCounter.ToString & ", " & obj.Name & ex.Message, "Error!", MessageBoxButtons.OK)
                End Try
            Else
                Exit Sub
            End If
        End Sub

        Private _multirateParameterChoice As ICommand
        Public Property MultirateParameterChoice As ICommand
            Get
                Return _multirateParameterChoice
            End Get
            Set(value As ICommand)
                _multirateParameterChoice = value
            End Set
        End Property
        Private Sub _chooseParameterForMultirate(obj As String)
            Select Case obj
                Case "newrate"
                    CurrentSelectedStep.FilterChoice = 1
                Case "pqratio"
                    CurrentSelectedStep.FilterChoice = 2
            End Select
        End Sub

#End Region


        Private _processConfigStepSelected As ICommand
        Public Property ProcessConfigStepSelected As ICommand
            Get
                Return _processConfigStepSelected
            End Get
            Set(value As ICommand)
                _processConfigStepSelected = value
            End Set
        End Property
        Private Sub _processStepSelectedToEdit(processStep As Object)
            If processStep IsNot CurrentSelectedStep AndAlso CurrentSelectedStep IsNot Nothing Then
                If Not CurrentSelectedStep.CheckStepIsComplete() Then
                    'here need to check if the currentSelectedStep is complete, if not, cannot switch
                    MessageBox.Show("Missing field(s) in this step, please double check!", "Error!", MessageBoxButtons.OK)
                    Exit Sub
                Else
                    'here do all the stuff that is needed such as sort signals to make sure the step is set up.

                    CurrentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(_currentSelectedStep.OutputChannels)
                    If TypeOf CurrentSelectedStep Is Multirate Then
                        CurrentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(_currentSelectedStep.InputChannels)
                    End If
                End If
            End If
            If Not processStep.IsStepSelected Then
                Try
                    Dim lastNumberOfSteps = processStep.StepCounter
                    Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                    Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                    'if TypeOf CurrentSelectedStep Is UnWrap
                    Dim selectedFound = False
                    For Each unwrap In ProcessConfigure.UnWrapList
                        If unwrap.IsStepSelected Then
                            unwrap.IsStepSelected = False
                            For Each signal In unwrap.InputChannels
                                signal.IsChecked = False
                            Next
                            selectedFound = True
                        End If
                        If unwrap.StepCounter < lastNumberOfSteps Then
                            unwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(unwrap.OutputChannels)
                            stepsOutputAsSignalHierachy.Add(unwrap.ThisStepOutputsAsSignalHierachyByPMU)
                        End If
                        If unwrap.StepCounter >= lastNumberOfSteps AndAlso selectedFound Then
                            Exit For
                        End If
                    Next
                    'ElseIf TypeOf CurrentSelectedStep Is Interpolate
                    For Each intplt In ProcessConfigure.InterpolateList
                        If intplt.IsStepSelected Then
                            intplt.IsStepSelected = False
                            For Each signal In intplt.InputChannels
                                signal.IsChecked = False
                            Next
                            selectedFound = True
                        End If
                        If intplt.StepCounter < lastNumberOfSteps Then
                            intplt.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(intplt.OutputChannels)
                            stepsOutputAsSignalHierachy.Add(intplt.ThisStepOutputsAsSignalHierachyByPMU)
                        End If
                        If intplt.StepCounter >= lastNumberOfSteps AndAlso selectedFound Then
                            Exit For
                        End If
                    Next
                    'ElseIf TypeOf CurrentSelectedStep Is TunableFilter OrElse TypeOf CurrentSelectedStep Is Multirate
                    For Each stp In ProcessConfigure.CollectionOfSteps
                        If stp.IsStepSelected Then
                            stp.IsStepSelected = False
                            For Each signal In stp.InputChannels
                                signal.IsChecked = False
                            Next
                            selectedFound = True
                        End If
                        If stp.StepCounter < lastNumberOfSteps Then
                            stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(stp.OutputChannels)
                            stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                            If TypeOf stp Is Multirate Then
                                stp.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(stp.InputChannels)
                                stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                            End If
                        End If
                        If stp.StepCounter >= lastNumberOfSteps AndAlso selectedFound Then
                            Exit For
                        End If
                    Next
                    'ElseIf TypeOf CurrentSelectedStep Is Wrap
                    For Each wrap In ProcessConfigure.WrapList
                        If wrap.IsStepSelected Then
                            wrap.IsStepSelected = False
                            For Each signal In wrap.InputChannels
                                signal.IsChecked = False
                            Next
                            selectedFound = True
                        End If
                        If wrap.StepCounter < lastNumberOfSteps Then
                            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(wrap.OutputChannels)
                            stepsOutputAsSignalHierachy.Add(wrap.ThisStepOutputsAsSignalHierachyByPMU)
                        End If
                        If wrap.StepCounter >= lastNumberOfSteps AndAlso selectedFound Then
                            Exit For
                        End If
                    Next
                    'End If
                    If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then

                        For Each newTypeUnit In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                            If newTypeUnit.IsStepSelected Then
                                newTypeUnit.IsStepSelected = False
                                For Each signal In newTypeUnit.InputChannels
                                    signal.IsChecked = False
                                Next
                                selectedFound = True
                            End If
                            If newTypeUnit.StepCounter < lastNumberOfSteps Then
                                newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(newTypeUnit.OutputChannels)
                                stepsOutputAsSignalHierachy.Add(newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU)
                            End If
                            If newTypeUnit.StepCounter >= lastNumberOfSteps AndAlso selectedFound Then
                                Exit For
                            End If
                        Next
                    End If
                    '_signalMgr.DetermineFileDirCheckableStatus()
                    '_determineSamplingRateCheckableStatus()
                    For Each signal In processStep.InputChannels
                        signal.IsChecked = True
                    Next
                    processStep.IsStepSelected = True
                    If CurrentSelectedStep IsNot Nothing Then
                        If TypeOf CurrentSelectedStep Is Unwrap OrElse TypeOf CurrentSelectedStep Is Wrap Then
                            _disableEnableAllButAngleSignalsInProcessConfig(True)
                        End If
                        If TypeOf CurrentSelectedStep Is NameTypeUnitPMU Then
                            _disableEnableSignalPassedThroughNameTypeUnit(True, CurrentSelectedStep)
                        End If
                    End If

                    _signalMgr.GroupedSignalByProcessConfigStepsInput = stepsInputAsSignalHierachy
                    _signalMgr.GroupedSignalByProcessConfigStepsOutput = stepsOutputAsSignalHierachy
                    _signalMgr.ProcessConfigDetermineAllParentNodeStatus()
                    '_signalMgr.DetermineFileDirCheckableStatus()
                    '_determineSamplingRateCheckableStatus()

                    If TypeOf processStep Is Unwrap OrElse TypeOf processStep Is Wrap Then
                        _disableEnableAllButAngleSignalsInProcessConfig(False)
                    End If
                    If TypeOf processStep Is NameTypeUnitPMU Then
                        _disableEnableSignalPassedThroughNameTypeUnit(False, processStep)
                    End If

                    CurrentSelectedStep = processStep
                    _determineSamplingRateCheckableStatus()
                Catch ex As Exception
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
                End Try
            End If
        End Sub

        Private Sub _disableEnableSignalPassedThroughNameTypeUnit(isEnabled As Boolean, thisStep As NameTypeUnitPMU)
            If NameTypeUnitStatusFlag Then
                'TODO: NameTypeUnit approach 1 that is obsolete where all signal in this system will be changed
            Else
                For Each change In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                    For Each signal In change.OutputChannels
                        If Not thisStep.OutputChannels.Contains(signal) Then
                            signal.IsEnabled = isEnabled
                        End If
                    Next
                Next
            End If
        End Sub

        Private Sub _reverseSignalPassedThroughNameTypeUnit()
            If NameTypeUnitStatusFlag Then
                'TODO: NameTypeUnit approach 1 that is obsolete where all signal in this system will be changed
            Else
                For Each change In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                    For Each signal In change.OutputChannels
                        If signal.IsNameTypeUnitChanged Then
                            Dim temp = ""
                            If Not String.IsNullOrEmpty(signal.OldSignalName) Then
                                temp = signal.SignalName
                                signal.SignalName = signal.OldSignalName
                                signal.OldSignalName = temp
                            End If
                            temp = signal.Unit
                            signal.Unit = signal.OldUnit
                            signal.OldUnit = temp
                            temp = signal.TypeAbbreviation
                            signal.TypeAbbreviation = signal.OldTypeAbbreviation
                            signal.OldTypeAbbreviation = temp
                        End If
                    Next
                Next
            End If
        End Sub

        Private Sub _disableEnableAllButAngleSignalsInProcessConfig(isEnabled As Boolean)
            For Each group In _signalMgr.GroupedRawSignalsByType
                For Each subgroupBySamplingRate In group.SignalList
                    For Each subgroup In subgroupBySamplingRate.SignalList
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
            Next
            For Each group In _signalMgr.GroupedRawSignalsByPMU
                For Each subgroupBySamplingRate In group.SignalList
                    For Each subgroup In subgroupBySamplingRate.SignalList
                        For Each subsubgroup In subgroup.SignalList
                            If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                                subsubgroup.SignalSignature.IsEnabled = isEnabled
                            End If
                        Next
                    Next
                Next
            Next
            For Each group In _signalMgr.GroupedSignalByProcessConfigStepsInput
                For Each subgroupBySamplingRate In group.SignalList
                    For Each subgroup In subgroupBySamplingRate.SignalList
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
            Next
            For Each group In _signalMgr.GroupedSignalByProcessConfigStepsOutput
                For Each subgroupBySamplingRate In group.SignalList
                    For Each subgroup In subgroupBySamplingRate.SignalList
                        For Each subsubgroup In subgroup.SignalList
                            If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                                subsubgroup.SignalSignature.IsEnabled = isEnabled
                            End If
                        Next
                    Next
                Next
            Next
            For Each group In _signalMgr.AllDataConfigOutputGroupedByType
                For Each subgroupBySamplingRate In group.SignalList
                    If subgroupBySamplingRate.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroupBySamplingRate.SignalSignature.TypeAbbreviation <> "V" Then
                        subgroupBySamplingRate.SignalSignature.IsEnabled = isEnabled
                    Else
                        For Each subsubgroup In subgroupBySamplingRate.SignalList
                            If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "A" Then
                                subsubgroup.SignalSignature.IsEnabled = isEnabled
                            End If
                        Next
                    End If
                Next
            Next
            For Each group In _signalMgr.AllDataConfigOutputGroupedByPMU
                For Each subgroupBySamplingRate In group.SignalList
                    For Each subgroup In subgroupBySamplingRate.SignalList
                        If String.IsNullOrEmpty(subgroup.SignalSignature.TypeAbbreviation) OrElse subgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                            subgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                Next
            Next
        End Sub

        Private _processConfigStepDeSelected As ICommand
        Public Property ProcessConfigStepDeSelected As ICommand
            Get
                Return _processConfigStepDeSelected
            End Get
            Set(value As ICommand)
                _processConfigStepDeSelected = value
            End Set
        End Property
        Private Sub _deSelectAllProcessConfigSteps()
            If CurrentSelectedStep IsNot Nothing Then
                For Each signal In CurrentSelectedStep.InputChannels
                    signal.IsChecked = False
                Next
                'If _currentSelectedDataConfigStep.OutputChannels IsNot Nothing Then
                '    For Each signal In _currentSelectedDataConfigStep.OutputChannels
                '        signal.IsChecked = False
                '    Next
                'End If
                Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                For Each unwrap In ProcessConfigure.UnWrapList
                    stepsOutputAsSignalHierachy.Add(unwrap.ThisStepOutputsAsSignalHierachyByPMU)
                Next
                For Each intplt In ProcessConfigure.InterpolateList
                    stepsOutputAsSignalHierachy.Add(intplt.ThisStepOutputsAsSignalHierachyByPMU)
                Next
                For Each stp In ProcessConfigure.CollectionOfSteps
                    stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(stp.OutputChannels)
                    stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                    If TypeOf stp Is Multirate Then
                        stp.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(stp.InputChannels)
                        stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                    End If
                Next
                For Each wrap In ProcessConfigure.WrapList
                    stepsOutputAsSignalHierachy.Add(wrap.ThisStepOutputsAsSignalHierachyByPMU)
                Next
                If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then

                    For Each newTypeUnit In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                        stepsOutputAsSignalHierachy.Add(newTypeUnit.ThisStepOutputsAsSignalHierachyByPMU)
                    Next
                End If
                _signalMgr.GroupedSignalByProcessConfigStepsInput = stepsInputAsSignalHierachy
                _signalMgr.GroupedSignalByProcessConfigStepsOutput = stepsOutputAsSignalHierachy

                If TypeOf CurrentSelectedStep Is Unwrap OrElse TypeOf CurrentSelectedStep Is Wrap Then
                    _disableEnableAllButAngleSignalsInProcessConfig(True)
                End If
                If TypeOf CurrentSelectedStep Is NameTypeUnitPMU Then
                    _disableEnableSignalPassedThroughNameTypeUnit(True, CurrentSelectedStep)
                End If
                'If CurrentSelectedDataConfigStep.Name = "Phasor Creation" Then
                '    _disableEnableAllButMagnitudeSignals(True)
                'ElseIf CurrentSelectedDataConfigStep.Name = "Power Calculation" Then
                '    If CurrentSelectedDataConfigStep.OutputInputMappingPair.Count > 0 Then
                '        Dim situation = CurrentSelectedDataConfigStep.OutputInputMappingPair(0).Value.Count
                '        If situation = 4 Then
                '            _disableEnableAllButMagnitudeSignals(True)
                '        Else
                '            _disableEnableAllButPhasorSignals(True)
                '        End If
                '    End If
                'ElseIf CurrentSelectedDataConfigStep.Name = "Metric Prefix" Then
                '    _disableEnableAllButMagnitudeFrequencyROCOFSignals(True)
                'ElseIf CurrentSelectedDataConfigStep.Name = "Angle Conversion" Then
                '    _disableEnableAllButAngleSignals(True)
                'End If
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedSignalByProcessConfigStepsInput, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedSignalByProcessConfigStepsOutput, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedRawSignalsByPMU, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.GroupedRawSignalsByType, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllDataConfigOutputGroupedByType, False)
                _changeCheckStatusAllParentsOfGroupedSignal(_signalMgr.AllDataConfigOutputGroupedByPMU, False)
                If TypeOf CurrentSelectedStep Is TunableFilter Then
                    _currentInputOutputPair = Nothing
                End If
                CurrentSelectedStep.IsStepSelected = False
                CurrentSelectedStep = Nothing
                '_signalMgr.DetermineFileDirCheckableStatus()
                _determineSamplingRateCheckableStatus()
            End If
        End Sub

        'Private Sub _processConfigDetermineAllParentNodeStatus()
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.GroupedRawSignalsByType)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.GroupedRawSignalsByPMU)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllDataConfigOutputGroupedByType)
        '    _determineParentGroupedByTypeNodeStatus(_signalMgr.AllDataConfigOutputGroupedByPMU)
        '    For Each stepInput In _signalMgr.GroupedSignalByProcessConfigStepsInput
        '        If stepInput.SignalList.Count > 0 Then
        '            _determineParentGroupedByTypeNodeStatus(stepInput.SignalList)
        '            _determineParentCheckStatus(stepInput)
        '        Else
        '            stepInput.SignalSignature.IsChecked = False
        '        End If
        '    Next
        '    For Each stepOutput In _signalMgr.GroupedSignalByProcessConfigStepsOutput
        '        If stepOutput.SignalList.Count > 0 Then
        '            _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList)
        '            _determineParentCheckStatus(stepOutput)
        '        Else
        '            stepOutput.SignalSignature.IsChecked = False
        '        End If
        '    Next
        'End Sub

    End Class

End Namespace
