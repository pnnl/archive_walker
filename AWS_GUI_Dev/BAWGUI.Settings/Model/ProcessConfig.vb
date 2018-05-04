Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports BAWGUI.Settings.ViewModels

Namespace Model
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
                                                                                   {"AP", {"DEG", "RAD"}.ToList},
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
                                                                                   {"P", {"MW"}.ToList},
                                                                                   {"Q", {"MVAR"}.ToList},
                                                                                   {"CP", {"MVA"}.ToList},
                                                                                   {"S", {"MVA"}.ToList},
                                                                                   {"F", {"Hz", "mHz"}.ToList},
                                                                                   {"RCF", {"mHz/sec", "Hz/sec"}.ToList},
                                                                                   {"D", {"D"}.ToList},
                                                                                   {"SC", {"SC"}.ToList},
                                                                                   {"OTHER", {"O"}.ToList}}
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
                Return _initializationPath
            End Get
            Set(ByVal value As String)
                _initializationPath = value
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
            InputChannels = New ObservableCollection(Of SignalSignatures)
            OutputChannels = New ObservableCollection(Of SignalSignatures)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
            Name = "Unwrap"
            IsExpanded = False
        End Sub
        Public Overrides Property Name As String

        Public Overrides Function CheckStepIsComplete() As Boolean
            Throw New NotImplementedException()
        End Function
        'Private _inputChannels As ObservableCollection(Of SignalSignatures)
        'Public Property InputChannels As ObservableCollection(Of SignalSignatures)
        '    Get
        '        Return _inputChannels
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of SignalSignatures))
        '        _inputChannels = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

        'Private _maxNaN As String
        'Public Property MaxNaN As String
        '    Get
        '        Return _maxNaN
        '    End Get
        '    Set(value As String)
        '        _maxNaN = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

    End Class

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''Class Interpolate''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class Interpolate
        Inherits SignalProcessStep
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatures)
            OutputChannels = New ObservableCollection(Of SignalSignatures)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
            Name = "Interpolation"
            IsExpanded = False
            _flagInterp = False
        End Sub

        Public Overrides Function CheckStepIsComplete() As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Property Name As String

        Private _limit As String
        Public Property Limit As String
            Get
                Return _limit
            End Get
            Set(value As String)
                _limit = value
                OnPropertyChanged()
            End Set
        End Property

        Private _type As InterpolateType
        Public Property Type As InterpolateType
            Get
                Return _type
            End Get
            Set(value As InterpolateType)
                _type = value
                ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & StepCounter & " - " & value.ToString() & " " & Name
                ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter & " - " & value.ToString() & " " & Name
                OnPropertyChanged()
            End Set
        End Property

        Private _flagInterp As Boolean
        Public Property FlagInterp As Boolean
            Get
                Return _flagInterp
            End Get
            Set(value As Boolean)
                _flagInterp = value
                OnPropertyChanged()
            End Set
        End Property

        'Private _inputChannels As ObservableCollection(Of SignalSignatures)
        'Public Property InputChannels As ObservableCollection(Of SignalSignatures)
        '    Get
        '        Return _inputChannels
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of SignalSignatures))
        '        _inputChannels = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

    End Class

    Public Enum InterpolateType
        <Description("Linear")>
        Linear
        <Description("Constant")>
        Constant
    End Enum

    Public Enum TunableFilterType
        Rational
        <Description("High-Pass")>
        HighPass
        <Description("Low-Pass")>
        LowPass
        'Median
    End Enum
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''Class Wrap''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class Wrap
        Inherits SignalProcessStep
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatures)
            OutputChannels = New ObservableCollection(Of SignalSignatures)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
            Name = "Wrap"
            IsExpanded = False
        End Sub

        Public Overrides Property Name As String

        Public Overrides Function CheckStepIsComplete() As Boolean
            Throw New NotImplementedException()
        End Function
    End Class

    Public Class TunableFilter
        Inherits Filter

        Public Sub New()
            MyBase.New
            _filterParameterDictionary = New Dictionary(Of TunableFilterType, List(Of String)) From {{TunableFilterType.Rational, {"Numerator", "Denominator"}.ToList()},
                                                                                                    {TunableFilterType.HighPass, {"Order", "Cutoff"}.ToList()},
                                                                                                    {TunableFilterType.LowPass, {"PassRipple", "StopRipple", "PassCutoff", "StopCutoff"}.ToList()}}
            '{TunableFilterType.Median, {"Order", "Endpoints", "HandleNaN"}.ToList()}}
            InputChannels = New ObservableCollection(Of SignalSignatures)
            OutputChannels = New ObservableCollection(Of SignalSignatures)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
            Name = "Filter"
            Type = TunableFilterType.Rational
            IsExpanded = False
            _order = 1
            _cutoff = 0.1
            _passRipple = 0.1
            _stopRipple = 50
            _passCutoff = 1.5
            _stopCutoff = 2.5
        End Sub

        Private _type As TunableFilterType
        Public Property Type As TunableFilterType
            Get
                Return _type
            End Get
            Set(value As TunableFilterType)
                _type = value
                ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & StepCounter & " - " & value.ToString() & " " & Name
                ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & StepCounter & " - " & value.ToString() & " " & Name
                Dim newParameterList = New ObservableCollection(Of ParameterValuePair)
                For Each p In _filterParameterDictionary(_type)
                    Dim newPair = New ParameterValuePair
                    newPair.ParameterName = p
                    'If p = "ZeroPhase" Then
                    '    newPair.Value = False
                    If p = "Endpoints" Then
                        newPair.Value = EndpointsType.zeropad
                    ElseIf p = "HandleNaN" Then
                        newPair.Value = HandleNaNType.includenan
                    Else
                        newPair.Value = ""
                    End If
                    newParameterList.Add(newPair)
                Next
                Me.FilterParameters = newParameterList
                OnPropertyChanged()
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
                Return _numberator
            End Get
            Set(ByVal value As String)
                _numberator = value
                OnPropertyChanged()
            End Set
        End Property

        Private _denominator As String
        Public Property Denominator As String
            Get
                Return _denominator
            End Get
            Set(ByVal value As String)
                _denominator = value
                OnPropertyChanged()
            End Set
        End Property

        Private _order As String
        Public Property Order As String
            Get
                Return _order
            End Get
            Set(ByVal value As String)
                _order = value
                OnPropertyChanged()
            End Set
        End Property

        Private _cutoff As String
        Public Property Cutoff As String
            Get
                Return _cutoff
            End Get
            Set(ByVal value As String)
                _cutoff = value
                OnPropertyChanged()
            End Set
        End Property

        Private _passRipple As String
        Public Property PassRipple As String
            Get
                Return _passRipple
            End Get
            Set(ByVal value As String)
                _passRipple = value
                OnPropertyChanged()
            End Set
        End Property

        Private _stopRipple As String
        Public Property StopRipple As String
            Get
                Return _stopRipple
            End Get
            Set(ByVal value As String)
                _stopRipple = value
                OnPropertyChanged()
            End Set
        End Property

        Private _passCutoff As String
        Public Property PassCutoff As String
            Get
                Return _passCutoff
            End Get
            Set(ByVal value As String)
                _passCutoff = value
                OnPropertyChanged()
            End Set
        End Property

        Private _stopCutoff As String
        Public Property StopCutoff As String
            Get
                Return _stopCutoff
            End Get
            Set(ByVal value As String)
                _stopCutoff = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public Class Multirate
        Inherits Filter
        Public Sub New()
            InputChannels = New ObservableCollection(Of SignalSignatures)
            OutputChannels = New ObservableCollection(Of SignalSignatures)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
            _name = "Multirate"
            '_pElement = 1
            '_qElement = 1
            _newRate = 1
            IsExpanded = False
            _filterChoice = "0"
        End Sub

        Private _name As String
        Public Overrides Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
                OnPropertyChanged()
            End Set
        End Property

        Private _multiRatePMU As String
        Public Property MultiRatePMU As String
            Get
                Return _multiRatePMU
            End Get
            Set(value As String)
                _multiRatePMU = value
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
                Return _newRate
            End Get
            Set(value As String)
                _newRate = value
                If Not String.IsNullOrEmpty(value) Then
                    For Each signal In OutputChannels
                        signal.SamplingRate = value
                    Next
                End If
                'ThisStepOutputsAsSignalHierachyByPMU.SignalList =
                OnPropertyChanged()
            End Set
        End Property

        Private _pElement As String
        Public Property PElement As String
            Get
                Return _pElement
            End Get
            Set(value As String)
                _pElement = value
                Dim q = 1
                Integer.TryParse(_qElement, q)
                If q = 0 Then
                    q = 1
                End If
                Dim p = 1
                Integer.TryParse(_pElement, p)
                If p <> 0 Then
                    For index = 0 To OutputChannels.Count - 1
                        OutputChannels(index).SamplingRate = InputChannels(index).SamplingRate * p / q
                    Next
                End If
                OnPropertyChanged()
            End Set
        End Property

        Private _qElement As String
        Public Property QElement As String
            Get
                Return _qElement
            End Get
            Set(value As String)
                _qElement = value
                Dim p = 1
                Integer.TryParse(_pElement, p)
                If p = 0 Then
                    p = 1
                End If
                Dim q = 1
                Integer.TryParse(_qElement, q)
                If q <> 0 Then
                    For index = 0 To OutputChannels.Count - 1
                        OutputChannels(index).SamplingRate = InputChannels(index).SamplingRate * p / q
                    Next
                End If
                OnPropertyChanged()
            End Set
        End Property

        Private _filterChoice As String
        Public Property FilterChoice As String
            Get
                Return _filterChoice
            End Get
            Set(ByVal value As String)
                _filterChoice = value
                OnPropertyChanged()
            End Set
        End Property

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return Not String.IsNullOrEmpty(_multiRatePMU)
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
            InputChannels = New ObservableCollection(Of SignalSignatures)
            OutputChannels = New ObservableCollection(Of SignalSignatures)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
            _name = "Signal Type and Unit Specification"
            IsExpanded = False
        End Sub

        Public Overrides Function CheckStepIsComplete() As Boolean
            Throw New NotImplementedException()
        End Function

        Private _name As String
        Public Overrides Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
                OnPropertyChanged()
            End Set
        End Property

        'Private _pmuName As String
        'Public Property PmuName As String
        '    Get
        '        Return _pmuName
        '    End Get
        '    Set(value As String)
        '        _pmuName = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

        'Private _currentChannel As String
        'Public Property CurrentChannel As String
        '    Get
        '        Return _currentChannel
        '    End Get
        '    Set(value As String)
        '        _currentChannel = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

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
                            OutputChannels(0).OldSignalName = ""
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
                        signal.OldUnit = value
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
                        signal.OldTypeAbbreviation = value
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
