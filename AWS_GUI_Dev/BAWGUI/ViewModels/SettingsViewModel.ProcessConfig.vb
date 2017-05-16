Imports System.Collections.ObjectModel
Imports System.Windows.Forms

Partial Public Class SettingsViewModel
    Private _groupedSignalByProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByProcessConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByProcessConfigStepsInput
        End Get
        Set(value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByProcessConfigStepsInput = value
            OnPropertyChanged()
        End Set
    End Property
    Private _groupedSignalByProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByProcessConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByProcessConfigStepsOutput
        End Get
        Set(value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByProcessConfigStepsOutput = value
            OnPropertyChanged()
        End Set
    End Property

#Region "Read Process Config"
    Private Sub _readProcessConfig()
        GroupedSignalByProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)()
        GroupedSignalByProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)()
        _readUnwrap()
        _readInterpolate()
        _readProcessConfigStages()
        _readWrap()
    End Sub
    Private Sub _readUnwrap()
        Dim newUnWrapList = New ObservableCollection(Of Unwrap)
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
            newUnWrapList.Add(aUnwrap)
        Next
        ProcessConfigure.UnWrapList = newUnWrapList
    End Sub
    Private Sub _readInterpolate()
        dim newInterpolateList = New ObservableCollection(Of Interpolate)
        Dim interpolates = From el In _configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Interpolate" Select el
        For Each interpolate As XElement In interpolates
            Dim anInterpolate = New Interpolate
            anInterpolate.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
            anInterpolate.Limit = interpolate.<Parameters>.<Limit>.Value
            anInterpolate.Type = [Enum].Parse(GetType(InterpolateType), interpolate.<Parameters>.<Type>.Value)
            anInterpolate.FlagInterp = Convert.ToBoolean(interpolate.<Parameters>.<FlagInterp>.Value)
            anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & anInterpolate.StepCounter.ToString & " - " & anInterpolate.Type.ToString() & " " & anInterpolate.Name
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
            newInterpolateList.Add(anInterpolate)
        Next
        ProcessConfigure.InterpolateList = newInterpolateList
    End Sub
    Private Sub _readProcessConfigStages()
        Dim CollectionOfSteps As New ObservableCollection(Of Object)
        dim stepCounter = GroupedSignalByProcessConfigStepsOutput.Count
        Dim stages = From el In _configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Stages" Select el
        For Each stage In stages
            Dim steps = From element In stage.Elements Select element
            For Each stp In steps
                Dim aStep As Object
                If stp.Name = "Filter"
                    aStep = New TunableFilter
                    stepCounter += 1
                    aStep.StepCounter = stepCounter
                    aStep.Type = [Enum].Parse(GetType(TunableFilterType), stp.<Type>.Value)
                    'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                    'Select Case aStep.Type
                    '    Case TunableFilterType.HighPass
                            
                    '        '_readHighPassFilter(aStep, stp.<Parameters>)
                    '    Case TunableFilterType.LowPass

                    '    Case TunableFilterType.Median

                    '    Case TunableFilterType.Rational

                    '    Case Else
                    '        Throw New Exception("Undefined tunable filter type found!")
                    'End Select
                    Dim params = From ps In stp.<Parameters>.Elements Select ps
                    For Each pair In params
                        'Dim aPair As New ParameterValuePair
                        Dim paraName = pair.Name.ToString
                        dim aPair = (From x in DirectCast(aStep, TunableFilter).FilterParameters Where x.ParameterName = paraName Select x).firstOrDefault
                        If pair.Value.ToLower = "false" Then
                            aPair.Value = False
                        ElseIf pair.Value.ToLower = "true" Then
                            aPair.Value = True
                        ElseIf paraName = "Endpoints" Then
                            aPair.Value = [Enum].Parse(GetType(EndpointsType), pair.Value)
                        ElseIf paraName = "HandleNaN"
                            aPair.Value = [Enum].Parse(GetType(HandleNaNType), pair.Value)
                        'ElseIf aStep.Name = "Nominal-Value Frequency Data Quality Filter" And paraName = "FlagBit" Then
                        '    aPair.IsRequired = False
                        '    aPair.Value = pair.Value
                        Else
                            aPair.Value = pair.Value
                        End If
                    Next
                ElseIf stp.Name = "Multirate"
                    aStep = New Multirate
                    stepCounter += 1
                    aStep.StepCounter = stepCounter
                    Dim params = From ps In stp.<Parameters>.Elements Select ps
                    For Each pair In params
                        Dim aPair As New ParameterValuePair
                        Dim paraName = pair.Name.ToString
                        aPair.ParameterName = paraName
                        aPair.Value = pair.Value
                        aStep.FilterParameters.Add(aPair)
                        if paraName = "MultiRatePMU"
                            aStep.MultiRatePMU = pair.Value()
                        ElseIf paraName = "NewRate"
                            aStep.NewRate = pair.Value()
                        ElseIf paraName = "p"
                            aStep.PElement = pair.Value()
                        ElseIf paraName = "q"
                            aStep.QElement = pair.Value()
                        End If
                    Next
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                End If
                aStep.InputChannels = _readPMUElements(stp)
                For Each signal In aStep.InputChannels
                    signal.PassedThroughProcessor = True
                    If TypeOf aStep Is Multirate
                        signal.PMUName = aStep.MultiRatePMU
                    End If
                    aStep.OutputChannels.Add(signal)
                Next
                If TypeOf aStep Is Multirate Then
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                    GroupedSignalByProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                End If
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                GroupedSignalByProcessConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                CollectionOfSteps.Add(aStep)
            Next
        Next
        ProcessConfigure.CollectionOfSteps = CollectionOfSteps
    End Sub
    Private Sub _readWrap()
        Dim newWrapList = New ObservableCollection(Of Wrap)
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
            newWrapList.Add(aWrap)
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
        ProcessConfigure.WrapList = newWrapList
    End Sub
#End Region

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
        dim aNewUnwrap = New Unwrap
        aNewUnwrap.StepCounter = ProcessConfigure.UnWrapList.Count + 1
        aNewUnwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aNewUnwrap.StepCounter.ToString & " - " & aNewUnwrap.Name
        aNewUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aNewUnwrap.StepCounter.ToString & " - " & aNewUnwrap.Name
        'GroupedSignalByProcessConfigStepsOutput.Insert(aNewUnwrap.StepCounter - 1, aNewUnwrap.ThisStepOutputsAsSignalHierachyByPMU)
        ProcessConfigure.UnWrapList.Add(aNewUnwrap)
        
        Dim intpltList = New ObservableCollection(Of Interpolate)(ProcessConfigure.InterpolateList)
        For Each interpolate In intpltList
            interpolate.StepCounter +=1
            interpolate.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
            interpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
        Next
        ProcessConfigure.InterpolateList = intpltList
        Dim steps = New  ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
        For Each aStep In steps
            aStep.StepCounter +=1
            If TypeOf aStep Is TunableFilter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
            else
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
            End If
        Next
        ProcessConfigure.CollectionOfSteps = steps
        dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
        For Each wrap In wraps
            wrap.StepCounter +=1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
        Next
        ProcessConfigure.WrapList = wraps
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
        GroupedSignalByProcessConfigStepsOutput.Remove(obj.ThisStepOutputsAsSignalHierachyByPMU)
        ProcessConfigure.UnWrapList.Remove((obj))
        dim unwraps = New ObservableCollection(Of Unwrap)(ProcessConfigure.UnWrapList)
        For Each unwrap In unwraps
            If unwrap.StepCounter > obj.StepCounter
                unwrap.StepCounter -= 1
                unwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & unwrap.StepCounter.ToString & " - " & unwrap.Name
                unwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & unwrap.StepCounter.ToString & " - " & unwrap.Name
            End If
        Next
        ProcessConfigure.UnWrapList=unwraps
        Dim intplts = new ObservableCollection(Of Interpolate)(ProcessConfigure.InterpolateList)
        For Each interpolate In intplts
            interpolate.StepCounter -=1
            interpolate.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
            interpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & interpolate.StepCounter.ToString & " - " & interpolate.Type.ToString() & " " & interpolate.Name
        Next
        ProcessConfigure.InterpolateList = intplts
        dim steps = New ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
        For Each aStep In steps
            aStep.StepCounter -=1
            If TypeOf aStep Is TunableFilter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
            else
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
            End If
        Next
        ProcessConfigure.CollectionOfSteps = steps
        dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
        For Each wrap In wraps
            wrap.StepCounter -=1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
        Next
        ProcessConfigure.WrapList = wraps
    End Sub

    Private _addInterpolate as ICommand
    Public Property AddInterpolate As ICommand
        Get
            Return _addInterpolate
        End Get
        Set(value As ICommand)
            _addInterpolate = value
        End Set
    End Property
    Private Sub _addAnInterpolate(obj As Object)
        dim anInterpolate = New Interpolate
        anInterpolate.StepCounter = ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + 1
        anInterpolate.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & anInterpolate.StepCounter.ToString & " - " & anInterpolate.Name
        anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & anInterpolate.StepCounter.ToString & " - " & anInterpolate.Name
        'GroupedSignalByProcessConfigStepsOutput.Insert(ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count, anInterpolate.ThisStepOutputsAsSignalHierachyByPMU)
        ProcessConfigure.InterpolateList.Add(anInterpolate)
        dim steps = New ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
        For Each aStep In steps
            aStep.StepCounter +=1
            If TypeOf aStep Is TunableFilter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
            else
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
            End If
        Next
        ProcessConfigure.CollectionOfSteps = steps
        Dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
        For Each wrap In wraps
            wrap.StepCounter +=1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
        Next
        ProcessConfigure.WrapList = wraps
        _processStepSelectedToEdit(anInterpolate)
    End Sub

    Private _deleteInterpolateStep as ICommand
    Public Property DeleteInterpolateStep As ICommand
        Get
            Return _deleteInterpolateStep
        End Get
        Set(value As ICommand)
            _deleteInterpolateStep = value
        End Set
    End Property
    Private Sub _deleteAnInterpolate(obj As Interpolate)
        GroupedSignalByProcessConfigStepsOutput.Remove(obj.ThisStepOutputsAsSignalHierachyByPMU)
        ProcessConfigure.InterpolateList.Remove((obj))
        Dim intplts = new ObservableCollection(Of Interpolate)(ProcessConfigure.InterpolateList)
        For Each intplt In intplts
            If intplt.StepCounter > obj.StepCounter
                intplt.StepCounter -= 1
                intplt.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & intplt.StepCounter.ToString & " - " & intplt.Type.ToString() & " " & intplt.Name
                intplt.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & intplt.StepCounter.ToString & " - " & intplt.Type.ToString() & " " & intplt.Name
            End If
        Next
        ProcessConfigure.InterpolateList = intplts
        dim steps = new ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
        For Each aStep In steps
            aStep.StepCounter -=1
            If TypeOf aStep Is TunableFilter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
            else
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
            End If
        Next
        ProcessConfigure.CollectionOfSteps = steps
        dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
        For Each wrap In wraps
            wrap.StepCounter -=1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
        Next
        ProcessConfigure.WrapList = wraps
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
        aStep.StepCounter = ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + ProcessConfigure.CollectionOfSteps.Count + 1
        If TypeOf aStep Is TunableFilter
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
            wrap.StepCounter +=1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
        Next
        ProcessConfigure.WrapList = wraps
        _processStepSelectedToEdit(aStep)
    End Sub

    Private _deleteTunableFilterOrMultirate as ICommand
    Public Property DeleteTunableFilterOrMultirate As ICommand
        Get
            Return _deleteTunableFilterOrMultirate
        End Get
        Set(value As ICommand)
            _deleteTunableFilterOrMultirate = value
        End Set
    End Property
    Private Sub _deleteATunableFilterOrMultirate(obj As Object)
        GroupedSignalByProcessConfigStepsOutput.Remove(obj.ThisStepOutputsAsSignalHierachyByPMU)
        if TypeOf obj Is Multirate
            GroupedSignalByProcessConfigStepsInput.Remove(obj.ThisStepInputsAsSignalHerachyByType)
        End If
        ProcessConfigure.CollectionOfSteps.Remove((obj))
        dim steps = new ObservableCollection(Of Object)(ProcessConfigure.CollectionOfSteps)
        For Each aStep In steps
            If aStep.StepCounter > obj.StepCounter
                aStep.StepCounter -=1
                If TypeOf aStep Is TunableFilter
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Type.ToString() & " " & aStep.Name
                else
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                End If
            End If
        Next
        ProcessConfigure.CollectionOfSteps = steps
        dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
        For Each wrap In wraps
            wrap.StepCounter -=1
            wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
            wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
        Next
        ProcessConfigure.WrapList = wraps
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
        dim wrap = New Wrap
        wrap.StepCounter = ProcessConfigure.InterpolateList.Count + ProcessConfigure.UnWrapList.Count + ProcessConfigure.CollectionOfSteps.Count + ProcessConfigure.WrapList.Count + 1
        wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
        wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & " - " & wrap.Name
        'GroupedSignalByProcessConfigStepsOutput.Add(wrap.ThisStepOutputsAsSignalHierachyByPMU)
        ProcessConfigure.WrapList.Add(wrap)
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
        GroupedSignalByProcessConfigStepsOutput.Remove(obj.ThisStepOutputsAsSignalHierachyByPMU)
        ProcessConfigure.WrapList.Remove((obj))
        dim wraps = New ObservableCollection(Of Wrap)(ProcessConfigure.WrapList)
        For Each wrap In wraps
            If wrap.StepCounter > obj.StepCounter
                wrap.StepCounter -=1
                wrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
                wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & wrap.StepCounter.ToString & "-" & wrap.Name
            End If
        Next
        ProcessConfigure.WrapList = wraps
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
        dim parameters = New ObservableCollection(of ParameterValuePair)
        parameters.Add(new ParameterValuePair("MultiRatePMU", ""))
        Select Case obj
            Case "newrate"
                parameters.Add(New ParameterValuePair("NewRate", ""))
            Case "pqratio"
                parameters.Add(New ParameterValuePair("p", ""))
                parameters.Add(New ParameterValuePair("q", ""))
        End Select
        CurrentSelectedStep.FilterParameters = parameters
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
        if Not processStep.IsStepSelected Then
            Try
                Dim lastNumberOfSteps = processStep.StepCounter
                Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                'if TypeOf CurrentSelectedStep Is UnWrap
                dim selectedFound = False
                For Each unwrap In ProcessConfigure.UnWrapList
                    If unwrap.IsStepSelected
                        unwrap.IsStepSelected = False
                        For Each signal In unwrap.InputChannels
                            signal.IsChecked = False
                        Next
                        selectedFound = True
                    End If
                    If unwrap.StepCounter < lastNumberOfSteps
                        unwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(unwrap.OutputChannels)
                        stepsOutputAsSignalHierachy.Add(unwrap.ThisStepOutputsAsSignalHierachyByPMU)
                    End If
                    If unwrap.StepCounter >= lastNumberOfSteps AndAlso selectedFound
                        Exit For
                    End If
                Next
                'ElseIf TypeOf CurrentSelectedStep Is Interpolate
                For Each intplt In ProcessConfigure.InterpolateList
                    If intplt.IsStepSelected
                        intplt.IsStepSelected = False
                        For Each signal In intplt.InputChannels
                            signal.IsChecked = False
                        Next
                        selectedFound = True
                    End If
                    If intplt.StepCounter < lastNumberOfSteps
                        intplt.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(intplt.OutputChannels)
                        stepsOutputAsSignalHierachy.Add(intplt.ThisStepOutputsAsSignalHierachyByPMU)
                    End If
                    If intplt.StepCounter >= lastNumberOfSteps AndAlso selectedFound
                        Exit For
                    End If
                Next
                'ElseIf TypeOf CurrentSelectedStep Is TunableFilter OrElse TypeOf CurrentSelectedStep Is Multirate
                For Each stp In ProcessConfigure.CollectionOfSteps
                    If stp.IsStepSelected
                        stp.IsStepSelected = False
                        For Each signal In stp.InputChannels
                            signal.IsChecked = False
                        Next
                        selectedFound = True
                    End If
                    If stp.StepCounter < lastNumberOfSteps
                        stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(stp.OutputChannels)
                        stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                        If TypeOf stp Is Multirate
                            stp.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(stp.InputChannels)
                            stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                        End If
                    End If
                    If stp.StepCounter >= lastNumberOfSteps AndAlso selectedFound
                        Exit For
                    End If
                Next
                'ElseIf TypeOf CurrentSelectedStep Is Wrap
                For Each wrap In ProcessConfigure.WrapList
                    If wrap.IsStepSelected
                        wrap.IsStepSelected = False
                        For Each signal In wrap.InputChannels
                            signal.IsChecked = False
                        Next
                        selectedFound = True
                    End If
                    If wrap.StepCounter < lastNumberOfSteps
                        wrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(wrap.OutputChannels)
                        stepsOutputAsSignalHierachy.Add(wrap.ThisStepOutputsAsSignalHierachyByPMU)
                    End If
                    If wrap.StepCounter >= lastNumberOfSteps AndAlso selectedFound
                        Exit For
                    End If
                Next
                'End If
                _determineFileDirCheckableStatus()
                For Each signal In processStep.InputChannels
                    signal.IsChecked = True
                Next
                processStep.IsStepSelected = True
                If CurrentSelectedStep IsNot Nothing Then
                    if TypeOf CurrentSelectedStep Is Unwrap OrElse TypeOf CurrentSelectedStep Is Wrap
                        _disableEnableAllButAngleSignals(True)
                    End If
                End If

                GroupedSignalByProcessConfigStepsInput = stepsInputAsSignalHierachy
                GroupedSignalByProcessConfigStepsOutput = stepsOutputAsSignalHierachy
                _processConfigDetermineAllParentNodeStatus()
                _determineFileDirCheckableStatus()
                
                if TypeOf processStep Is Unwrap OrElse TypeOf processStep Is Wrap
                    _disableEnableAllButAngleSignals(False)
                End If
                
                CurrentSelectedStep = processStep
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        End If
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
    Private Sub _deSelectAllProcessConfigSteps(obj As Object)
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
                stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                If TypeOf stp Is Multirate
                    stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                End If
            Next
            For Each wrap In ProcessConfigure.WrapList
                stepsOutputAsSignalHierachy.Add(wrap.ThisStepOutputsAsSignalHierachyByPMU)
            Next
            GroupedSignalByProcessConfigStepsInput = stepsInputAsSignalHierachy
            GroupedSignalByProcessConfigStepsOutput = stepsOutputAsSignalHierachy

            if TypeOf CurrentSelectedStep Is Unwrap OrElse TypeOf CurrentSelectedStep Is Wrap
                _disableEnableAllButAngleSignals(True)
            End If
            'If CurrentSelectedDataConfigStep.Name = "Phasor Creation Customization" Then
            '    _disableEnableAllButMagnitudeSignals(True)
            'ElseIf CurrentSelectedDataConfigStep.Name = "Power Calculation Customization" Then
            '    If CurrentSelectedDataConfigStep.OutputInputMappingPair.Count > 0 Then
            '        Dim situation = CurrentSelectedDataConfigStep.OutputInputMappingPair(0).Value.Count
            '        If situation = 4 Then
            '            _disableEnableAllButMagnitudeSignals(True)
            '        Else
            '            _disableEnableAllButPhasorSignals(True)
            '        End If
            '    End If
            'ElseIf CurrentSelectedDataConfigStep.Name = "Metric Prefix Customization" Then
            '    _disableEnableAllButMagnitudeFrequencyROCOFSignals(True)
            'ElseIf CurrentSelectedDataConfigStep.Name = "Angle Conversion Customization" Then
            '    _disableEnableAllButAngleSignals(True)
            'End If
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByProcessConfigStepsInput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByProcessConfigStepsOutput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByType, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllDataConfigOutputGroupedByType, False)
            _changeCheckStatusAllParentsOfGroupedSignal(AllDataConfigOutputGroupedByPMU, False)
            CurrentSelectedStep.IsStepSelected = False
            CurrentSelectedStep = Nothing
            _determineFileDirCheckableStatus()
        End If
    End Sub

    Private Sub _processConfigDetermineAllParentNodeStatus()
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType)
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU)
        _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType)
        _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU)
        'For Each group In GroupedSignalsByPMU
        '    _determineParentCheckStatus(group)
        'Next
        For Each stepInput In GroupedSignalByProcessConfigStepsInput
            If stepInput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepInput.SignalList)
                _determineParentCheckStatus(stepInput)
            Else
                stepInput.SignalSignature.IsChecked = False
            End If
        Next
        For Each stepOutput In GroupedSignalByProcessConfigStepsOutput
            If stepOutput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList)
                _determineParentCheckStatus(stepOutput)
            Else
                stepOutput.SignalSignature.IsChecked = False
            End If
        Next
    End Sub

End Class
