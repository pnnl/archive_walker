Imports System.Collections.ObjectModel

Partial Public Class SettingsViewModel
    Private _groupedSignalByProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByProcessConfigStepsInput
        End Get
        Set(value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByProcessConfigStepsInput = value
        End Set
    End Property
    Private _groupedSignalByProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByProcessConfigStepsOutput
        End Get
        Set(value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByProcessConfigStepsOutput = value
        End Set
    End Property

#Region "Read Process Config"
    Private Sub _readProcessConfig()
        _groupedSignalByProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)()
        _groupedSignalByProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)()
        _readUnwrap()
        _readInterpolate()
        _readProcessConfigStages()
        _readWrap()
    End Sub
    Private Sub _readUnwrap()
        Dim unWraps = From el In _configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Unwrap" Select el
        For Each unWrap As XElement In unWraps
            Dim aUnwrap = New Unwrap
            aUnwrap.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
            'aUnwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aUnwrap.StepCounter.ToString & "-" & aUnwrap.Name
            aUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aUnwrap.StepCounter.ToString & "-" & aUnwrap.Name
            aUnwrap.MaxNaN = unWrap.<MaxNaN>.Value()
            aUnwrap.InputChannels = _readPMUElements(unWrap)
            For Each signal In aUnwrap.InputChannels
                signal.PassedThroughProcessor = True
                aUnwrap.OutputChannels.Add(signal)
            Next
            'dim inputs = From el In unWrap.Elements Where el.Name="PMU" Select el
            'If Inputs.ToList.Count >0
            '    For Each aInput In inputs
            '        Dim pmuName = aInput.<Name>.Value()
            '        dim channels = from el in aInput.Elements where el.Name = "Channel" Select el
            '        If channels.ToList.Count > 0
            '            for Each channel in channels
            '                dim signalName = channel.<Name>.Value
            '                Dim signal = _searchForSignalInTaggedSignals(pmuName, signalName)
            '                if signal IsNot nothing
            '                    aUnwrap.InputChannels.Add(signal)
            '                else
            '                    _addLog("Error reading config file! Signal in unwrap of processing with channel name: " & signalName & " in PMU " & pmuName & " not found!")
            '                End If
            '            Next
            '        else
            '            For Each group In GroupedSignalsByPMU
            '                For Each subgroup In group.SignalList
            '                    if subgroup.SignalSignature.PMUName = pmuName
            '                        For Each signal In subgroup.SignalList
            '                            aUnwrap.InputChannels.Add(signal.SignalSignature)
            '                        Next
            '                    End If
            '                Next
            '            Next
            '        End If
            '    Next
            'else
            '    'For Each group In GroupedSignalsByType
            '    '    For Each subgroup In group.SignalList
            '    '        If subgroup.SignalSignature.TypeAbbreviation = "V" OrElse subgroup.SignalSignature.TypeAbbreviation = "I"
            '    '            For Each subsubgroup In subgroup.SignalList
            '    '                 if subsubgroup.SignalSignature.TypeAbbreviation.Substring(1,1) = "A"
            '    '                     For Each phase In subsubgroup.SignalList
            '    '                         For Each signal In phase.SignalList
            '    '                             aUnwrap.InputChannels.Add(signal.SignalSignature)
            '    '                         Next
            '    '                     Next
            '    '                 End If       
            '    '            Next
            '    '        End If
            '    '    Next
            '    'Next
            '    _addLog("Warning! No PMU specified in Unwrap, no channel or no PMU is included in this step.")
            'End If
            'aUnwrap.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aUnwrap.InputChannels)
            'GroupedSignalByProcessConfigStepsInput.Add(aUnwrap.ThisStepInputsAsSignalHerachyByType)
            aUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aUnwrap.OutputChannels)
            GroupedSignalByProcessConfigStepsOutput.Add(aUnwrap.ThisStepOutputsAsSignalHierachyByPMU)
            ProcessConfigure.UnWrapList.Add(aUnwrap)
        Next
    End Sub
    Private Sub _readInterpolate()
        Dim interpolates = From el In _configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Interpolate" Select el
        For Each interpolate As XElement In interpolates
            Dim anInterpolate = New Interpolate
            anInterpolate.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
            anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & anInterpolate.StepCounter.ToString & "-" & anInterpolate.Name
            anInterpolate.Limit = interpolate.<Limit>.Value()
            anInterpolate.Type = interpolate.<Type>.Value()
            anInterpolate.FlagInterp = Convert.ToBoolean(interpolate.<FlagInterp>.Value())
            anInterpolate.InputChannels = _readPMUElements(interpolate)
            For Each signal In anInterpolate.InputChannels
                signal.PassedThroughProcessor = True
                anInterpolate.OutputChannels.Add(signal)
            Next
            'dim inputs = From el In interpolate.Elements Where el.Name="PMU" Select el
            'If Inputs.ToList.Count >0
            '    For Each aInput In inputs
            '        Dim pmuName = aInput.<Name>.Value()
            '        dim channels = from el in aInput.Elements where el.Name = "Channel" Select el
            '        If channels.ToList.Count > 0
            '            for Each channel in channels
            '                dim signalName = channel.<Name>.Value
            '                Dim signal = _searchForSignalInTaggedSignals(pmuName, signalName)
            '                if signal IsNot nothing
            '                    anInterpolate.InputChannels.Add(signal)
            '                else
            '                    _addLog("Error reading config file! Signal in interpolate of processing with channel name: " & signalName & " in PMU " & pmuName & " not found!")
            '                End If
            '            Next
            '        else
            '            For Each group In GroupedSignalsByPMU
            '                For Each subgroup In group.SignalList
            '                    if subgroup.SignalSignature.PMUName = pmuName
            '                        For Each signal In subgroup.SignalList
            '                            anInterpolate.InputChannels.Add(signal.SignalSignature)
            '                        Next
            '                    End If
            '                Next
            '            Next
            '        End If
            '    Next
            'else
            '    'For Each group In GroupedSignalsByPMU
            '    '    For Each subgroup In group.SignalList
            '    '        For Each signal In subgroup.SignalList
            '    '            anInterpolate.InputChannels.Add(signal.SignalSignature)
            '    '        Next
            '    '    Next
            '    'Next
            '    _addLog("Warning! No PMU specified in interpolate, no channel or no PMU is included in this step.")
            'End If
            anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(anInterpolate.OutputChannels)
            GroupedSignalByProcessConfigStepsOutput.Add(anInterpolate.ThisStepOutputsAsSignalHierachyByPMU)
            ProcessConfigure.InterpolateList.Add(anInterpolate)
        Next
    End Sub
    Private Sub _readProcessConfigStages()

    End Sub
    Private Sub _readWrap()
        Dim Wraps = From el In _configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Wrap" Select el
        For Each Wrap As XElement In Wraps
            Dim aWrap = New Wrap
            aWrap.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
            aWrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aWrap.StepCounter.ToString & "-" & aWrap.Name
            aWrap.InputChannels = _readPMUElements(Wrap)
            For Each signal In aWrap.InputChannels
                signal.PassedThroughProcessor = True
                aWrap.OutputChannels.Add(signal)
            Next
            aWrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aWrap.OutputChannels)
            GroupedSignalByProcessConfigStepsOutput.Add(aWrap.ThisStepOutputsAsSignalHierachyByPMU)
            ProcessConfigure.WrapList.Add(aWrap)
            'dim inputs = From el In Wrap.Elements Where el.Name="PMU" Select el
            'If Inputs.ToList.Count >0
            '    For Each aInput In inputs
            '        Dim pmuName = aInput.<Name>.Value()
            '        dim channels = from el in aInput.Elements where el.Name = "Channel" Select el
            '        If channels.ToList.Count > 0
            '            for Each channel in channels
            '                dim signalName = channel.<Name>.Value
            '                Dim signal = _searchForSignalInTaggedSignals(pmuName, signalName)
            '                if signal IsNot nothing
            '                    ProcessConfigure.WrapSignalList.Add(signal)
            '                else
            '                    _addLog("Error reading config file! Signal in Wrap of processing with channel name: " & signalName & " in PMU " & pmuName & " not found!")
            '                End If
            '            Next
            '        else
            '            For Each group In GroupedSignalsByPMU
            '                For Each subgroup In group.SignalList
            '                    if subgroup.SignalSignature.PMUName = pmuName
            '                        For Each signal In subgroup.SignalList
            '                            ProcessConfigure.WrapSignalList.Add(signal.SignalSignature)
            '                        Next
            '                    End If
            '                Next
            '            Next
            '        End If
            '    Next
            'else
            '    'For Each group In GroupedSignalsByType
            '    '    For Each subgroup In group.SignalList
            '    '        If subgroup.SignalSignature.TypeAbbreviation = "V" OrElse subgroup.SignalSignature.TypeAbbreviation = "I"
            '    '            For Each subsubgroup In subgroup.SignalList
            '    '                if subsubgroup.SignalSignature.TypeAbbreviation.Substring(1,1) = "A"
            '    '                    For Each phase In subsubgroup.SignalList
            '    '                        For Each signal In phase.SignalList
            '    '                            ProcessConfigure.WrapSignalList.Add(signal.SignalSignature)
            '    '                        Next
            '    '                    Next
            '    '                End If       
            '    '            Next
            '    '        End If
            '    '    Next
            '    'Next
            '    _addLog("Warning! No PMU specified in Warp, no channel or no PMU is included in this step.")
            'End If
        Next
    End Sub
#End Region

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
        dim aNewUnwrap = New Unwrap
        aNewUnwrap.StepCounter = ProcessConfigure.UnWrapList.Count + 1
        aNewUnwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aNewUnwrap.StepCounter.ToString & "-" & aNewUnwrap.Name
        aNewUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aNewUnwrap.StepCounter.ToString & "-" & aNewUnwrap.Name
        ProcessConfigure.UnWrapList.Add(aNewUnwrap)
        
        dim newOutputTree = New ObservableCollection(Of SignalTypeHierachy)()
        For Each unwrap In ProcessConfigure.UnWrapList
            newOutputTree.Add(unwrap.ThisStepOutputsAsSignalHierachyByPMU)
        Next

        For Each interpolate In ProcessConfigure.InterpolateList
            interpolate.StepCounter +=1
            interpolate.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & "-" & interpolate.Name
            interpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & "-" & interpolate.Name
            newOutputTree.Add(interpolate.ThisStepOutputsAsSignalHierachyByPMU)
        Next
        For Each aStep In ProcessConfigure.CollectionOfSteps
            aStep.StepCounter +=1
            aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
            newOutputTree.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        Next
        For Each wrap In ProcessConfigure.WrapList
            wrap.StepCounter +=1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
            newOutputTree.Add(wrap.ThisStepOutputsAsSignalHierachyByPMU)
        Next

        GroupedSignalByProcessConfigStepsOutput= newOutputTree
    End Sub

End Class
