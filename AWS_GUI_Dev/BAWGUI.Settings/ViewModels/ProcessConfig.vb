Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports BAWGUI.Core
Imports BAWGUI.Core.Models
Imports BAWGUI.ReadConfigXml
Imports BAWGUI.SignalManagement.ViewModels
Imports BAWGUI.Utilities

Namespace ViewModels
    Public Class ProcessConfig
        Inherits ViewModelBase
        Public Sub New()
            _unWrapList = New ObservableCollection(Of Unwrap)()
            _interpolateList = New ObservableCollection(Of Interpolate)()
            _collectionOfSteps = New ObservableCollection(Of Object)()
            _wrapList = New ObservableCollection(Of Wrap)()
            _nameTypeUnitElement = New NameTypeUnit()
            _typeUnitDictionary = New Dictionary(Of String, List(Of String)) From {{"VMP", {"kV", "V"}.ToList},
                                                                                   {"VMA", {"kV", "V"}.ToList},
                                                                                   {"VMB", {"kV", "V"}.ToList},
                                                                                   {"VMC", {"kV", "V"}.ToList},
                                                                                   {"VAP", {"DEG", "RAD"}.ToList},
                                                                                   {"VAA", {"DEG", "RAD"}.ToList},
                                                                                   {"VAB", {"DEG", "RAD"}.ToList},
                                                                                   {"VAC", {"DEG", "RAD"}.ToList},
                                                                                   {"VPP", {"kV", "V"}.ToList},
                                                                                   {"VPA", {"kV", "V"}.ToList},
                                                                                   {"VPB", {"kV", "V"}.ToList},
                                                                                   {"VPC", {"kV", "V"}.ToList},
                                                                                   {"IMP", {"A", "kA"}.ToList},
                                                                                   {"IMA", {"A", "kA"}.ToList},
                                                                                   {"IMB", {"A", "kA"}.ToList},
                                                                                   {"IMC", {"A", "kA"}.ToList},
                                                                                   {"IAP", {"DEG", "RAD"}.ToList},
                                                                                   {"IAA", {"DEG", "RAD"}.ToList},
                                                                                   {"IAB", {"DEG", "RAD"}.ToList},
                                                                                   {"IAC", {"DEG", "RAD"}.ToList},
                                                                                   {"IPP", {"A", "kA"}.ToList},
                                                                                   {"IPA", {"A", "kA"}.ToList},
                                                                                   {"IPB", {"A", "kA"}.ToList},
                                                                                   {"IPC", {"A", "kA"}.ToList},
                                                                                   {"P", {"W", "kW", "MW"}.ToList},
                                                                                   {"Q", {"VAR", "kVAR", "MVAR"}.ToList},
                                                                                   {"CP", {"VA", "kVA", "MVA"}.ToList},
                                                                                   {"S", {"VA", "kVA", "MVA"}.ToList},
                                                                                   {"F", {"Hz", "mHz"}.ToList},
                                                                                   {"RCF", {"mHz/sec", "Hz/sec"}.ToList},
                                                                                   {"D", {"D"}.ToList},
                                                                                   {"SC", {"SC"}.ToList},
                                                                                   {"OTHER", {"O"}.ToList},
                                                                                   {"VWA", {"V", "kV"}.ToList},
                                                                                   {"VWB", {"V", "kV"}.ToList},
                                                                                   {"VWC", {"V", "kV"}.ToList},
                                                                                   {"IWA", {"A", "kA"}.ToList},
                                                                                   {"IWB", {"A", "kA"}.ToList},
                                                                                   {"IWC", {"A", "kA"}.ToList}}
        End Sub

        Public Sub New(processConfigure As ReadConfigXml.ProcessConfigModel, signalsMgr As SignalManager)
            Me.New
            Me._model = processConfigure
            Dim newUnWrapList = New ObservableCollection(Of Unwrap)
            For Each unWrapM In _model.UnWrapList
                newUnWrapList.Add(New Unwrap(unWrapM, signalsMgr))
            Next
            UnWrapList = newUnWrapList
            Dim newInterpolateList = New ObservableCollection(Of Interpolate)
            For Each interpolateM In _model.InterpolateList
                newInterpolateList.Add(New Interpolate(interpolateM, signalsMgr))
            Next
            InterpolateList = newInterpolateList
            Dim newCollectionOfSteps As New ObservableCollection(Of Object)
            For Each stp In _model.CollectionOfSteps
                If stp.Name = "Filter" Then
                    newCollectionOfSteps.Add(New TunableFilter(stp, signalsMgr))
                Else
                    newCollectionOfSteps.Add(New Multirate(stp, signalsMgr))
                End If
            Next
            CollectionOfSteps = newCollectionOfSteps
            Dim newWrapList = New ObservableCollection(Of Wrap)
            For Each unWrapM In _model.WrapList
                newWrapList.Add(New Wrap(unWrapM, signalsMgr))
            Next
            WrapList = newWrapList
            NameTypeUnitElement = New NameTypeUnit(_model.NameTypeUnitList, signalsMgr)
        End Sub

        Private _unWrapList As ObservableCollection(Of Unwrap)
        Public Property UnWrapList As ObservableCollection(Of Unwrap)
            Get
                Return _unWrapList
            End Get
            Set(value As ObservableCollection(Of Unwrap))
                _unWrapList = value
                OnPropertyChanged()
            End Set
        End Property

        Private _interpolateList As ObservableCollection(Of Interpolate)
        Public Property InterpolateList As ObservableCollection(Of Interpolate)
            Get
                Return _interpolateList
            End Get
            Set(value As ObservableCollection(Of Interpolate))
                _interpolateList = value
                OnPropertyChanged()
            End Set
        End Property

        Private _wrapList As ObservableCollection(Of Wrap)
        Public Property WrapList As ObservableCollection(Of Wrap)
            Get
                Return _wrapList
            End Get
            Set(value As ObservableCollection(Of Wrap))
                _wrapList = value
                OnPropertyChanged()
            End Set
        End Property

        Private _collectionOfSteps As ObservableCollection(Of Object)
        Public Property CollectionOfSteps As ObservableCollection(Of Object)
            Get
                Return _collectionOfSteps
            End Get
            Set(ByVal value As ObservableCollection(Of Object))
                _collectionOfSteps = value
                OnPropertyChanged()
            End Set
        End Property

        Private _nameTypeUnitElement As NameTypeUnit
        Public Property NameTypeUnitElement As NameTypeUnit
            Get
                Return _nameTypeUnitElement
            End Get
            Set(value As NameTypeUnit)
                _nameTypeUnitElement = value
                OnPropertyChanged()
            End Set
        End Property

        Private _initializationPath As String
        Public Property InitializationPath As String
            Get
                Return _model.InitializationPath
            End Get
            Set(ByVal value As String)
                _model.InitializationPath = value
                OnPropertyChanged()
            End Set
        End Property
        Private _typeUnitDictionary As Dictionary(Of String, List(Of String))

        Public Property TypeUnitDictionary As Dictionary(Of String, List(Of String))
            Get
                Return _typeUnitDictionary
            End Get
            Set(ByVal value As Dictionary(Of String, List(Of String)))
                _typeUnitDictionary = value
                OnPropertyChanged()
            End Set
        End Property
        Private _model As ReadConfigXml.ProcessConfigModel
        Public Property Model As ReadConfigXml.ProcessConfigModel
            Get
                Return _model
            End Get
            Set(value As ReadConfigXml.ProcessConfigModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''Class Unwrap'''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class Unwrap
        Inherits SignalProcessStep
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New UnwrapModel()
            IsExpanded = False
        End Sub
        Public Sub New(unWrapM As UnwrapModel, signalsMgr As SignalManager)
            Me.New
            Me._model = unWrapM
            StepCounter = signalsMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(unWrapM.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As UnwrapModel
        Public Property Model As UnwrapModel
            Get
                Return _model
            End Get
            Set(ByVal value As UnwrapModel)
                _model = value
            End Set
        End Property
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
    End Class

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''Class Interpolate''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class Interpolate
        Inherits SignalProcessStep
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New InterpolateModel()
            IsExpanded = False
            _flagInterp = False
        End Sub

        Public Sub New(interpolateM As InterpolateModel, signalsMgr As SignalManager)
            Me.New
            Me._model = interpolateM
            StepCounter = signalsMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter.ToString & " - " & Type.ToString() & " " & Name
            Try
                InputChannels = signalsMgr.FindSignals(interpolateM.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As InterpolateModel
        Public Property Model As InterpolateModel
            Get
                Return _model
            End Get
            Set(ByVal value As InterpolateModel)
                _model = value
            End Set
        End Property
        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function

        Private _limit As String
        Public Property Limit As String
            Get
                Return _model.Limit
            End Get
            Set(value As String)
                _model.Limit = value
                OnPropertyChanged()
            End Set
        End Property

        Private _type As InterpolateType
        Public Property Type As InterpolateType
            Get
                Return _model.Type
            End Get
            Set(value As InterpolateType)
                _model.Type = value
                ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & StepCounter & " - " & value.ToString() & " " & Name
                ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter & " - " & value.ToString() & " " & Name
                OnPropertyChanged()
            End Set
        End Property

        Private _flagInterp As Boolean
        Public Property FlagInterp As Boolean
            Get
                Return _model.FlagInterp
            End Get
            Set(value As Boolean)
                _model.FlagInterp = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    'Public Enum InterpolateType
    '    <Description("Linear")>
    '    Linear
    '    <Description("Constant")>
    '    Constant
    'End Enum

    'Public Enum TunableFilterType
    '    Rational
    '    <Description("High-Pass")>
    '    HighPass
    '    <Description("Low-Pass")>
    '    LowPass
    '    'Median
    'End Enum
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''Class Wrap''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class Wrap
        Inherits SignalProcessStep
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            _model = New WrapModel()
            IsExpanded = False
        End Sub
        Public Sub New(wrapM As WrapModel, signalsMgr As SignalManager)
            Me.New
            Me._model = wrapM
            StepCounter = signalsMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter.ToString & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(wrapM.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            For Each signal In InputChannels
                signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
                OutputChannels.Add(signal)
            Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _model As WrapModel
        Public Property Model As WrapModel
            Get
                Return _model
            End Get
            Set(ByVal value As WrapModel)
                _model = value
            End Set
        End Property

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
    End Class

    Public Class TunableFilter
        Inherits Filter

        Public Sub New()
            MyBase.New
            _filterParameterDictionary = New Dictionary(Of TunableFilterType, List(Of String)) From {{TunableFilterType.Rational, {"Numerator", "Denominator"}.ToList()},
                                                                                                    {TunableFilterType.HighPass, {"Order", "Cutoff"}.ToList()},
                                                                                                    {TunableFilterType.LowPass, {"PassRipple", "StopRipple", "PassCutoff", "StopCutoff"}.ToList()},
                                                                                                    {TunableFilterType.FrequencyDerivation, New List(Of String)()},
                                                                                                    {TunableFilterType.RunningAverage, {"RemoveAve", "WindowLength"}.ToList()},
                                                                                                    {TunableFilterType.PointOnWavePower, {"WindowLength"}.ToList()}}
            '{TunableFilterType.Median, {"Order", "Endpoints", "HandleNaN"}.ToList()}}
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            _outputInputMappingPair = New ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
            _model = New TunableFilterModel()
            _powInputSignals = New PointOnWaveCalFilterInputSignals
            Type = TunableFilterType.Rational
            IsExpanded = False
            _order = 1
            _cutoff = 0.1
            _passRipple = 0.1
            _stopRipple = 50
            _passCutoff = 1.5
            _stopCutoff = 2.5
            UseCustomPMU = False
        End Sub

        Public Sub New(stp As TunableFilterModel, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            StepCounter = signalsMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & StepCounter & " - " & Type.ToString() & " " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter & " - " & Type.ToString() & " " & Name
            For Each signal In _model.PMUElementList
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(signal.PMUName, signal.Channel)
                If input IsNot Nothing Then
                    If InputChannels.Contains(input) Then
                        Throw New Exception("Duplicate input signal found in step: " & StepCounter.ToString & " ," & Name & ".")
                    Else
                        InputChannels.Add(input)
                    End If
                Else
                    Throw New Exception("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.Channel & " in PMU " & signal.PMUName & " not found!")
                End If
                If _model.Type <> TunableFilterType.PointOnWavePower Then
                    Dim output As SignalSignatureViewModel = Nothing
                    If _model.UseCustomPMU Then
                        output = New SignalSignatureViewModel(signal.CustSignalName, _model.CustPMUName)
                        output.IsCustomSignal = True
                        If input.IsValid Then
                            output.TypeAbbreviation = input.TypeAbbreviation
                            output.Unit = input.Unit
                            output.SamplingRate = input.SamplingRate
                        End If
                        output.OldUnit = output.Unit
                        output.OldTypeAbbreviation = output.TypeAbbreviation
                        output.OldSignalName = output.SignalName
                    Else
                        input.PassedThroughProcessor = input.PassedThroughProcessor + 1
                        output = input
                    End If
                    OutputChannels.Add(output)
                    Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                    newPair.Value.Add(input)
                    OutputInputMappingPair.Add(newPair)
                End If
            Next
            If _model.Type = TunableFilterType.PointOnWavePower Then
                PhaseShiftV = _model.PointOnWavePowerCalculationFilterParam.PhaseShiftV
                PhaseShiftI = _model.PointOnWavePowerCalculationFilterParam.PhaseShiftI
                Dim freq = -1
                For Each signal In InputChannels
                    If freq = -1 AndAlso signal.SamplingRate <> -1 Then
                        freq = signal.SamplingRate
                    End If
                    Select Case signal.SignalName
                        Case _model.PointOnWavePowerCalculationFilterParam.VA
                            POWInputSignals.PhaseAVoltage = signal
                        Case _model.PointOnWavePowerCalculationFilterParam.VB
                            POWInputSignals.PhaseBVoltage = signal
                        Case _model.PointOnWavePowerCalculationFilterParam.VC
                            POWInputSignals.PhaseCVoltage = signal
                        Case _model.PointOnWavePowerCalculationFilterParam.IA
                            POWInputSignals.PhaseACurrent = signal
                        Case _model.PointOnWavePowerCalculationFilterParam.IB
                            POWInputSignals.PhaseBCurrent = signal
                        Case _model.PointOnWavePowerCalculationFilterParam.IC
                            POWInputSignals.PhaseCCurrent = signal
                        Case Else

                    End Select
                Next
                Dim output1 = New SignalSignatureViewModel(_model.PointOnWavePowerCalculationFilterParam.Pname, CustPMUName, "P")
                output1.SamplingRate = freq
                Dim output2 = New SignalSignatureViewModel(_model.PointOnWavePowerCalculationFilterParam.Qname, CustPMUName, "Q")
                output2.SamplingRate = freq
                Dim output3 = New SignalSignatureViewModel(_model.PointOnWavePowerCalculationFilterParam.Fname, CustPMUName, "F")
                output3.SamplingRate = freq
                OutputChannels.Add(output1)
                OutputChannels.Add(output2)
                OutputChannels.Add(output3)
                Dim PhAVUnit = POWInputSignals.PhaseAVoltage.Unit
                Dim PhAIUnit = POWInputSignals.PhaseACurrent.Unit
                If PhAVUnit = "V" And PhAIUnit = "A" Then
                    output1.Unit = "W"
                    output2.Unit = "VAR"
                    output1.OldUnit = output1.Unit
                    output2.OldUnit = output2.Unit
                ElseIf PhAVUnit = "kV" And PhAIUnit = "A" Then
                    output1.Unit = "kW"
                    output2.Unit = "kVAR"
                    output1.OldUnit = output1.Unit
                    output2.OldUnit = output2.Unit
                ElseIf PhAVUnit = "V" And PhAIUnit = "kA" Then
                    output1.Unit = "kW"
                    output2.Unit = "kVAR"
                    output1.OldUnit = output1.Unit
                    output2.OldUnit = output2.Unit
                ElseIf PhAVUnit = "kV" And PhAIUnit = "kA" Then
                    output1.Unit = "MW"
                    output2.Unit = "MVAR"
                    output1.OldUnit = output1.Unit
                    output2.OldUnit = output2.Unit
                End If
            End If
            'Try
            '    InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            'Catch ex As Exception
            '    Throw New Exception("Error finding signal in step: " & Name)
            'End Try
            'For Each signal In InputChannels
            '    signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
            '    OutputChannels.Add(signal)
            'Next
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
            If _model.UseCustomPMU Then
                Try
                    ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
                Catch ex As Exception
                    Throw New Exception("Error when sort signals by type in step: " & Name)
                End Try
                signalsMgr.GroupedSignalByProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            End If
        End Sub
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property

        Private _model As TunableFilterModel
        Public Property Model As TunableFilterModel
            Get
                Return _model
            End Get
            Set(ByVal value As TunableFilterModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property

        Private _type As TunableFilterType
        Public Property Type As TunableFilterType
            Get
                Return _model.Type
            End Get
            Set(value As TunableFilterType)
                If _model.Type <> value Then
                    'if switching from point wave to other type, it must be using custom pmu already, no need to reduce passthroughfilter count, but need to clear output to get rid of p and q, and establish input out put pairs
                    If _model.Type = TunableFilterType.PointOnWavePower Then
                        OutputChannels.Clear()
                        OutputInputMappingPair.Clear()
                        For Each signal In InputChannels
                            Dim newOutput = New SignalSignatureViewModel(signal.SignalName)
                            If String.IsNullOrEmpty(CustPMUName) Then
                                'Throw New Exception("Please enter a PMU name for this multirate step.")
                            Else
                                newOutput.PMUName = CustPMUName
                            End If
                            newOutput.TypeAbbreviation = signal.TypeAbbreviation
                            newOutput.IsCustomSignal = True
                            newOutput.Unit = signal.Unit
                            newOutput.SamplingRate = signal.SamplingRate
                            newOutput.OldSignalName = newOutput.SignalName
                            newOutput.OldTypeAbbreviation = newOutput.TypeAbbreviation
                            newOutput.OldUnit = newOutput.Unit
                            OutputChannels.Add(newOutput)
                            Dim kvp = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(newOutput, New ObservableCollection(Of SignalSignatureViewModel))
                            kvp.Value.Add(signal)
                            OutputInputMappingPair.Add(kvp)
                        Next
                    End If
                    _model.Type = value
                    'if switching to freq derivation and point on wave, they must be custom pmu
                    If _model.Type = TunableFilterType.FrequencyDerivation OrElse _model.Type = TunableFilterType.PointOnWavePower Then
                        OutputSignalStorage = OutputSignalStorageType.CreateCustomPMU
                    End If
                    'if switching to point on wave, need to establish input from point on wave input structure and output from p and q
                    If _model.Type = TunableFilterType.PointOnWavePower Then
                        For Each signal In InputChannels
                            signal.IsChecked = False
                        Next
                        InputChannels.Clear()
                        OutputChannels.Clear()
                        OutputInputMappingPair.Clear()
                        If POWInputSignals.PhaseACurrent.SamplingRate <> -1 Then
                            InputChannels.Add(POWInputSignals.PhaseACurrent)
                        End If
                        If POWInputSignals.PhaseAVoltage.SamplingRate <> -1 Then
                            InputChannels.Add(POWInputSignals.PhaseAVoltage)
                        End If
                        If POWInputSignals.PhaseBCurrent.SamplingRate <> -1 Then
                            InputChannels.Add(POWInputSignals.PhaseBCurrent)
                        End If
                        If POWInputSignals.PhaseBVoltage.SamplingRate <> -1 Then
                            InputChannels.Add(POWInputSignals.PhaseBVoltage)
                        End If
                        If POWInputSignals.PhaseCCurrent.SamplingRate <> -1 Then
                            InputChannels.Add(POWInputSignals.PhaseCCurrent)
                        End If
                        If POWInputSignals.PhaseCVoltage.SamplingRate <> -1 Then
                            InputChannels.Add(POWInputSignals.PhaseCVoltage)
                        End If
                        Dim freq = -1
                        For Each signal In InputChannels
                            If signal.SamplingRate <> -1 Then
                                freq = signal.SamplingRate
                                Exit For
                            End If
                        Next
                        Dim output1 = New SignalSignatureViewModel(Pname, CustPMUName, "P")
                        output1.SamplingRate = freq
                        Dim output2 = New SignalSignatureViewModel(Qname, CustPMUName, "Q")
                        output2.SamplingRate = freq
                        Dim output3 = New SignalSignatureViewModel(Fname, CustPMUName, "F")
                        output3.SamplingRate = freq
                        Dim PhAVUnit = POWInputSignals.PhaseAVoltage.Unit
                        Dim PhAIUnit = POWInputSignals.PhaseACurrent.Unit
                        If PhAVUnit = "V" And PhAIUnit = "A" Then
                            output1.Unit = "W"
                            output2.Unit = "VAR"
                            output1.OldUnit = output1.Unit
                            output2.OldUnit = output2.Unit
                        ElseIf PhAVUnit = "kV" And PhAIUnit = "A" Then
                            output1.Unit = "kW"
                            output2.Unit = "kVAR"
                            output1.OldUnit = output1.Unit
                            output2.OldUnit = output2.Unit
                        ElseIf PhAVUnit = "V" And PhAIUnit = "kA" Then
                            output1.Unit = "kW"
                            output2.Unit = "kVAR"
                            output1.OldUnit = output1.Unit
                            output2.OldUnit = output2.Unit
                        ElseIf PhAVUnit = "kV" And PhAIUnit = "kA" Then
                            output1.Unit = "MW"
                            output2.Unit = "MVAR"
                            output1.OldUnit = output1.Unit
                            output2.OldUnit = output2.Unit
                        End If
                        OutputChannels.Add(output1)
                        OutputChannels.Add(output2)
                        OutputChannels.Add(output3)
                    End If
                    ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & StepCounter.ToString & " - " & value.ToString() & " " & Name
                    ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter.ToString & " - " & value.ToString() & " " & Name
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Private _filterParameterDictionary As Dictionary(Of TunableFilterType, List(Of String))
        Public ReadOnly Property FilterParameterDictionary As Dictionary(Of TunableFilterType, List(Of String))
            Get
                Return _filterParameterDictionary
            End Get
        End Property

        Private _numberator As String
        Public Property Numerator As String
            Get
                Return _model.Numerator
            End Get
            Set(ByVal value As String)
                _model.Numerator = value
                OnPropertyChanged()
            End Set
        End Property

        Private _denominator As String
        Public Property Denominator As String
            Get
                Return _model.Denominator
            End Get
            Set(ByVal value As String)
                _model.Denominator = value
                OnPropertyChanged()
            End Set
        End Property

        Private _order As String
        Public Property Order As String
            Get
                Return _model.Order
            End Get
            Set(ByVal value As String)
                _model.Order = value
                OnPropertyChanged()
            End Set
        End Property

        Private _cutoff As String
        Public Property Cutoff As String
            Get
                Return _model.Cutoff
            End Get
            Set(ByVal value As String)
                _model.Cutoff = value
                OnPropertyChanged()
            End Set
        End Property

        Private _passRipple As String
        Public Property PassRipple As String
            Get
                Return _model.PassRipple
            End Get
            Set(ByVal value As String)
                _model.PassRipple = value
                OnPropertyChanged()
            End Set
        End Property

        Private _stopRipple As String
        Public Property StopRipple As String
            Get
                Return _model.StopRipple
            End Get
            Set(ByVal value As String)
                _model.StopRipple = value
                OnPropertyChanged()
            End Set
        End Property

        Private _passCutoff As String
        Public Property PassCutoff As String
            Get
                Return _model.PassCutoff
            End Get
            Set(ByVal value As String)
                _model.PassCutoff = value
                OnPropertyChanged()
            End Set
        End Property

        Private _stopCutoff As String
        Public Property StopCutoff As String
            Get
                Return _model.StopCutoff
            End Get
            Set(ByVal value As String)
                _model.StopCutoff = value
                OnPropertyChanged()
            End Set
        End Property
        Private _useCustomPMU As Boolean
        Public Property UseCustomPMU As Boolean
            Get
                Return _model.UseCustomPMU
            End Get
            Set(ByVal value As Boolean)
                If _model.UseCustomPMU <> value Then
                    _model.UseCustomPMU = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _outputStorage As OutputSignalStorageType
        Public Property OutputSignalStorage As OutputSignalStorageType
            Get
                Return _model.OutputSignalStorage
            End Get
            Set(ByVal value As OutputSignalStorageType)
                If _model.OutputSignalStorage <> value Then
                    _model.OutputSignalStorage = value
                    'switch from overwrite original to create custom pmu, need to reduce passthroughfilter count and create output for each input signal
                    If value = OutputSignalStorageType.CreateCustomPMU Then
                        UseCustomPMU = True
                        OutputChannels.Clear()
                        OutputInputMappingPair.Clear()
                        For Each signal In InputChannels
                            signal.PassedThroughProcessor -= 1
                            Dim newOutput = New SignalSignatureViewModel(signal.SignalName)
                            If String.IsNullOrEmpty(CustPMUName) Then
                                'Throw New Exception("Please enter a PMU name for this multirate step.")
                            Else
                                newOutput.PMUName = CustPMUName
                            End If
                            newOutput.TypeAbbreviation = signal.TypeAbbreviation
                            newOutput.IsCustomSignal = True
                            newOutput.Unit = signal.Unit
                            newOutput.SamplingRate = signal.SamplingRate
                            newOutput.OldSignalName = newOutput.SignalName
                            newOutput.OldTypeAbbreviation = newOutput.TypeAbbreviation
                            newOutput.OldUnit = newOutput.Unit
                            OutputChannels.Add(newOutput)
                            Dim kvp = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(newOutput, New ObservableCollection(Of SignalSignatureViewModel))
                            kvp.Value.Add(signal)
                            OutputInputMappingPair.Add(kvp)
                        Next
                    Else
                        'switch from custom pmu to overwrite original, need to increase passthroughfilter count and add the input signals to output so they will be over written.
                        UseCustomPMU = False
                        OutputChannels.Clear()
                        OutputInputMappingPair.Clear()
                        For Each signal In InputChannels
                            signal.PassedThroughProcessor += 1
                            OutputChannels.Add(signal)
                            Dim kvp = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(signal, New ObservableCollection(Of SignalSignatureViewModel))
                            kvp.Value.Add(signal)
                            OutputInputMappingPair.Add(kvp)
                        Next
                    End If
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _outputInputMappingPair As ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
        Public Property OutputInputMappingPair As ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel)))
            Get
                Return _outputInputMappingPair
            End Get
            Set(ByVal value As ObservableCollection(Of KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))))
                _outputInputMappingPair = value
                OnPropertyChanged()
            End Set
        End Property
        Private _customPMUName As String
        Public Property CustPMUName As String
            Get
                Return _model.CustPMUName
            End Get
            Set(ByVal value As String)
                _model.CustPMUName = value
                For Each signal In OutputChannels
                    signal.PMUName = value
                Next
                OnPropertyChanged()
            End Set
        End Property
        Private _removeAve As Boolean
        Public Property RemoveAve As Boolean
            Get
                Return _model.RemoveAve
            End Get
            Set(ByVal value As Boolean)
                If _model.RemoveAve <> value Then
                    _model.RemoveAve = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _windowLength As String
        Public Property WindowLength As String
            Get
                Return _model.WindowLength
            End Get
            Set(ByVal value As String)
                If _model.WindowLength <> value Then
                    _model.WindowLength = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Public Overrides Function CheckStepIsComplete() As Boolean
            If UseCustomPMU AndAlso String.IsNullOrEmpty(CustPMUName) Then
                'Throw New Exception("Please fill in custom PMU name.")
                Return False
            Else
                Return True
            End If
        End Function
        Private _powInputSignals As PointOnWaveCalFilterInputSignals
        Public Property POWInputSignals As PointOnWaveCalFilterInputSignals
            Get
                Return _powInputSignals
            End Get
            Set(ByVal value As PointOnWaveCalFilterInputSignals)
                _powInputSignals = value
                OnPropertyChanged()
            End Set
        End Property
        Private _phaseShiftV As String
        Public Property PhaseShiftV As String
            Get
                Return _model.PointOnWavePowerCalculationFilterParam.PhaseShiftV
            End Get
            Set(ByVal value As String)
                If _model.PointOnWavePowerCalculationFilterParam.PhaseShiftV <> value Then
                    _model.PointOnWavePowerCalculationFilterParam.PhaseShiftV = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _phaseShiftI As String
        Public Property PhaseShiftI As String
            Get
                Return _model.PointOnWavePowerCalculationFilterParam.PhaseShiftI
            End Get
            Set(ByVal value As String)
                If _model.PointOnWavePowerCalculationFilterParam.PhaseShiftI <> value Then
                    _model.PointOnWavePowerCalculationFilterParam.PhaseShiftI = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _pName As String
        Public Property Pname As String
            Get
                Return _model.PointOnWavePowerCalculationFilterParam.Pname
            End Get
            Set(ByVal value As String)
                If _model.PointOnWavePowerCalculationFilterParam.Pname <> value Then
                    _model.PointOnWavePowerCalculationFilterParam.Pname = value
                    OutputChannels(0).SignalName = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _qName As String
        Public Property Qname As String
            Get
                Return _model.PointOnWavePowerCalculationFilterParam.Qname
            End Get
            Set(ByVal value As String)
                If _model.PointOnWavePowerCalculationFilterParam.Qname <> value Then
                    _model.PointOnWavePowerCalculationFilterParam.Qname = value
                    OutputChannels(1).SignalName = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _fName As String
        Public Property Fname As String
            Get
                Return _model.PointOnWavePowerCalculationFilterParam.Fname
            End Get
            Set(ByVal value As String)
                If _model.PointOnWavePowerCalculationFilterParam.Fname <> value Then
                    _model.PointOnWavePowerCalculationFilterParam.Fname = value
                    OutputChannels(2).SignalName = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
    End Class

    Public Class PointOnWaveCalFilterInputSignals
        Inherits ViewModelBase
        Public Sub New()
            _phaseAVoltage = New SignalSignatureViewModel
            _phaseBVoltage = New SignalSignatureViewModel
            _phaseCVoltage = New SignalSignatureViewModel
            _phaseACurrent = New SignalSignatureViewModel
            _phaseBCurrent = New SignalSignatureViewModel
            _phaseCCurrent = New SignalSignatureViewModel
        End Sub
        Private _phaseAVoltage As SignalSignatureViewModel
        Public Property PhaseAVoltage As SignalSignatureViewModel
            Get
                Return _phaseAVoltage
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                If _phaseAVoltage <> value Then
                    _phaseAVoltage = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _phaseBVoltage As SignalSignatureViewModel
        Public Property PhaseBVoltage As SignalSignatureViewModel
            Get
                Return _phaseBVoltage
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                If _phaseBVoltage <> value Then
                    _phaseBVoltage = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _phaseCVoltage As SignalSignatureViewModel
        Public Property PhaseCVoltage As SignalSignatureViewModel
            Get
                Return _phaseCVoltage
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                If _phaseCVoltage <> value Then
                    _phaseCVoltage = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _phaseACurrent As SignalSignatureViewModel
        Public Property PhaseACurrent As SignalSignatureViewModel
            Get
                Return _phaseACurrent
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                If _phaseACurrent <> value Then
                    _phaseACurrent = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _phaseBCurrent As SignalSignatureViewModel
        Public Property PhaseBCurrent As SignalSignatureViewModel
            Get
                Return _phaseBCurrent
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                If _phaseBCurrent <> value Then
                    _phaseBCurrent = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private _phaseCCurrent As SignalSignatureViewModel
        Public Property PhaseCCurrent As SignalSignatureViewModel
            Get
                Return _phaseCCurrent
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                If _phaseCCurrent <> value Then
                    _phaseCCurrent = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
    End Class

    Public Class Multirate
        Inherits Filter
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(signature:=New SignalSignatureViewModel)
            _model = New MultirateModel()
            '_pElement = 1
            '_qElement = 1
            '_newRate = 1
            IsExpanded = False
            _filterChoice = "0"
        End Sub

        Private Property signalsMgr As SignalManager
        Public Sub New(stp As MultirateModel, signalsMgr As SignalManager)
            Me.New
            Me._model = stp
            signalsMgr = signalsMgr
            StepCounter = signalsMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
            ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & StepCounter & " - " & Name
            ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter & " - " & Name
            Try
                InputChannels = signalsMgr.FindSignals(stp.PMUElementList)
            Catch ex As Exception
                Throw New Exception("Error finding signal in step: " & Name)
            End Try
            For Each signal In InputChannels
                Dim output = New SignalSignatureViewModel(signal.SignalName, MultiRatePMU, signal.TypeAbbreviation)
                If NewRate IsNot Nothing Then
                    output.SamplingRate = NewRate
                Else
                    If PElement IsNot Nothing Then
                        output.SamplingRate = signal.SamplingRate * PElement / QElement
                    Else
                        output.SamplingRate = signal.SamplingRate * 1 / QElement
                    End If
                End If
                output.Unit = signal.Unit
                output.IsCustomSignal = True
                output.OldSignalName = output.SignalName
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldUnit = output.Unit
                OutputChannels.Add(output)
            Next
            Try
                ThisStepInputsAsSignalHerachyByType.SignalList = signalsMgr.SortSignalByType(InputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByProcessConfigStepsInput.Add(ThisStepInputsAsSignalHerachyByType)
            Try
                ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
            Catch ex As Exception
                Throw New Exception("Error when sort signals by type in step: " & Name)
            End Try
            signalsMgr.GroupedSignalByProcessConfigStepsOutput.Add(ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Private _model As MultirateModel
        Public Property Model As MultirateModel
            Get
                Return _model
            End Get
            Set(ByVal value As MultirateModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _multiRatePMU As String
        Public Property MultiRatePMU As String
            Get
                Return _model.MultiRatePMU
            End Get
            Set(value As String)
                _model.MultiRatePMU = value
                'FilterParameters(0).Value = value
                For Each out In OutputChannels
                    out.PMUName = value
                Next
                Dim theOnlyPMUHierachy = ThisStepOutputsAsSignalHierachyByPMU.SignalList.FirstOrDefault
                If theOnlyPMUHierachy IsNot Nothing Then
                    theOnlyPMUHierachy.SignalSignature.PMUName = value
                    theOnlyPMUHierachy.SignalSignature.SignalName = value
                End If
                OnPropertyChanged()
            End Set
        End Property

        Private _newRate As String
        Public Property NewRate As String
            Get
                Return _model.NewRate
            End Get
            Set(value As String)
                If _model.NewRate <> value Then
                    _model.NewRate = value
                    If Not String.IsNullOrEmpty(value) Then
                        For Each signal In OutputChannels
                            signal.SamplingRate = value
                        Next
                    End If
                    ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
                    'ThisStepOutputsAsSignalHierachyByPMU.SignalList =
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Private _pElement As String
        Public Property PElement As String
            Get
                Return _model.PElement
            End Get
            Set(value As String)
                If _model.PElement <> value Then
                    _model.PElement = value
                    Dim q = 1
                    Integer.TryParse(_model.QElement, q)
                    If q = 0 Then
                        q = 1
                    End If
                    Dim p = 1
                    Integer.TryParse(value, p)
                    If p <> 0 Then
                        For index = 0 To OutputChannels.Count - 1
                            OutputChannels(index).SamplingRate = InputChannels(index).SamplingRate * p / q
                        Next
                    End If
                    'ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Private _qElement As String
        Public Property QElement As String
            Get
                Return _model.QElement
            End Get
            Set(value As String)
                If _model.QElement <> value Then
                    _model.QElement = value
                    Dim p = 1
                    Integer.TryParse(_model.PElement, p)
                    If p = 0 Then
                        p = 1
                    End If
                    Dim q = 1
                    Integer.TryParse(value, q)
                    If q <> 0 Then
                        For index = 0 To OutputChannels.Count - 1
                            OutputChannels(index).SamplingRate = InputChannels(index).SamplingRate * p / q
                        Next
                    End If
                    'ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(OutputChannels)
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Private _filterChoice As String

        Public Property FilterChoice As String
            Get
                Return _model.FilterChoice
            End Get
            Set(ByVal value As String)
                _model.FilterChoice = value
                OnPropertyChanged()
            End Set
        End Property

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return Not String.IsNullOrEmpty(_model.MultiRatePMU)
        End Function

    End Class

    Public Enum EndpointsType
        zeropad
        truncate
    End Enum

    Public Enum HandleNaNType
        includenan
        omitnan
    End Enum

    Public Class NameTypeUnit
        Inherits ViewModelBase
        Public Sub New()
            _nameTypeUnitPMUList = New ObservableCollection(Of NameTypeUnitPMU)()
        End Sub

        Public Sub New(nameTypeUnitList As NameTypeUnitModel, signalsMgr As SignalManager)
            Me.New
            Me._model = nameTypeUnitList
            Dim newPMUlist = New ObservableCollection(Of NameTypeUnitPMU)
            Dim newPMU As NameTypeUnitPMU
            For Each pmu In _model.NameTypeUnitPMUList
                Dim pmuFound = False

                If pmu.Input.SignalName = pmu.NewChannel Then
                    For Each pmuItem In newPMUlist
                        If pmuItem.NewType = pmu.NewType AndAlso pmuItem.NewUnit = pmu.NewUnit Then
                            pmuFound = True
                            newPMU = pmuItem
                            newPMU.NewChannel = ""
                            Exit For
                        End If
                    Next
                End If
                If Not pmuFound Then
                    newPMU = New NameTypeUnitPMU()
                    newPMU.NewType = pmu.NewType
                    newPMU.NewUnit = pmu.NewUnit
                    newPMU.NewChannel = pmu.NewChannel
                    newPMUlist.Add(newPMU)
                End If
                Dim input = signalsMgr.SearchForSignalInTaggedSignals(pmu.Input.PMUName, pmu.Input.SignalName)
                If input IsNot Nothing Then
                    If input.IsNameTypeUnitChanged Then
                        Throw New Exception("Error reading config file! Signal in a NameTypeUnit step : " & pmu.Input.SignalName & " in PMU " & pmu.Input.PMUName & " has already gone through another NameTypeUnit step, a signal is not allow to go through NameTypeUnit step twice.")
                    Else
                        If Not String.IsNullOrEmpty(pmu.NewChannel) Then
                            input.OldSignalName = input.SignalName
                            input.SignalName = pmu.NewChannel
                        End If
                        input.OldTypeAbbreviation = input.TypeAbbreviation
                        input.TypeAbbreviation = pmu.NewType
                        input.OldUnit = input.Unit
                        input.Unit = pmu.NewUnit
                        input.PassedThroughProcessor = input.PassedThroughProcessor + 1
                        input.IsNameTypeUnitChanged = True
                        newPMU.InputChannels.Add(input)
                        newPMU.OutputChannels.Add(input)
                    End If
                Else
                    Throw New Exception("Error reading config file! Signal in a NameTypeUnit step of processing with channel name: " & pmu.Input.SignalName & " in PMU " & pmu.Input.PMUName & " not found!")
                End If
            Next
            For Each pmuItem In newPMUlist
                pmuItem.StepCounter = signalsMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
                pmuItem.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & pmuItem.StepCounter.ToString & " - " & pmuItem.Name
                Try
                    pmuItem.ThisStepOutputsAsSignalHierachyByPMU.SignalList = signalsMgr.SortSignalByPMU(pmuItem.OutputChannels)
                Catch ex As Exception
                    Throw New Exception("Error when sort signals by type in step: " & pmuItem.Name)
                End Try
                signalsMgr.GroupedSignalByProcessConfigStepsOutput.Add(pmuItem.ThisStepOutputsAsSignalHierachyByPMU)
            Next
            NameTypeUnitPMUList = newPMUlist
        End Sub
        Private _model As NameTypeUnitModel
        Public Property Model As NameTypeUnitModel
            Get
                Return _model
            End Get
            Set(ByVal value As NameTypeUnitModel)
                _model = value
                OnPropertyChanged()
            End Set
        End Property
        Public ReadOnly Property Name As String
            Get
                Return _model.Name
            End Get
        End Property
        Private _newUnit As String
        Public Property NewUnit As String
            Get
                Return _newUnit
            End Get
            Set(value As String)
                _newUnit = value
                'TODO: how to make all initial signal and all output signal change their unit?
                '      this is needed for the first approach of NameTypeUnit which is a shortcut from Jim in the matlab code
                '      but might not be necessary anymore with the GUI
                OnPropertyChanged()
            End Set
        End Property

        Private _newType As String
        Public Property NewType As String
            Get
                Return _newType
            End Get
            Set(value As String)
                _newType = value
                'TODO: how to make all initial signal and all output signal change their type?
                '      this is needed for the first approach of NameTypeUnit which is a shortcut from Jim in the matlab code
                '      but might not be necessary anymore with the GUI
                OnPropertyChanged()
            End Set
        End Property

        Private _nameTypeUnitPMUList As ObservableCollection(Of NameTypeUnitPMU)
        Public Property NameTypeUnitPMUList As ObservableCollection(Of NameTypeUnitPMU)
            Get
                Return _nameTypeUnitPMUList
            End Get
            Set(value As ObservableCollection(Of NameTypeUnitPMU))
                _nameTypeUnitPMUList = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class NameTypeUnitPMU
        Inherits SignalProcessStep
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
        End Sub

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function
        Public ReadOnly Property Name As String
            Get
                Return "Signal Type and Unit Specification"
            End Get
        End Property
        Private _newChannel As String
        Public Property NewChannel As String
            Get
                Return _newChannel
            End Get
            Set(value As String)
                _newChannel = value
                If OutputChannels.Count = 1 Then
                    If String.IsNullOrEmpty(value) Then
                        If Not String.IsNullOrEmpty(OutputChannels(0).OldSignalName) Then
                            OutputChannels(0).SignalName = OutputChannels(0).OldSignalName
                            'OutputChannels(0).OldSignalName = ""
                        End If
                    Else
                        OutputChannels(0).OldSignalName = OutputChannels(0).SignalName
                        OutputChannels(0).SignalName = value
                    End If
                End If
                OnPropertyChanged()
            End Set
        End Property

        Private _newUnit As String
        Public Property NewUnit As String
            Get
                Return _newUnit
            End Get
            Set(value As String)
                _newUnit = value
                If String.IsNullOrEmpty(value) Then
                    For Each signal In OutputChannels
                        signal.Unit = signal.OldUnit
                        'signal.OldUnit = value
                    Next
                Else
                    For Each signal In OutputChannels
                        signal.OldUnit = signal.Unit
                        signal.Unit = value
                    Next
                End If
                OnPropertyChanged()
            End Set
        End Property

        Private _newType As String
        Public Property NewType As String
            Get
                Return _newType
            End Get
            Set(value As String)
                _newType = value
                If String.IsNullOrEmpty(value) Then
                    For Each signal In OutputChannels
                        signal.TypeAbbreviation = signal.OldTypeAbbreviation
                        'signal.OldTypeAbbreviation = value
                    Next
                Else
                    For Each signal In OutputChannels
                        signal.OldTypeAbbreviation = signal.TypeAbbreviation
                        signal.TypeAbbreviation = value
                    Next
                End If
                OnPropertyChanged()
            End Set
        End Property
    End Class
End Namespace
