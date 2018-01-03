Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports BAWGUI

Public Class ProcessConfig
    Inherits ViewModelBase
    Public Sub New()
        _unWrapList = New ObservableCollection(Of Unwrap)()
        _interpolateList = New ObservableCollection(Of Interpolate)()
        _collectionOfSteps = New ObservableCollection(Of Object)()
        _wrapList = New ObservableCollection(Of Wrap)()
        _nameTypeUnitElement = New NameTypeUnit()
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
            OnPropertyChanged
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

    Private _maxNaN As String
    Public Property MaxNaN As String
        Get
            Return _maxNaN
        End Get
        Set(value As String)
            _maxNaN = value
            OnPropertyChanged()
        End Set
    End Property

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
        Name = "Interpolate"
        IsExpanded = False
    End Sub
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
    HighPass
    LowPass
    Median
End Enum
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''Class Wrap''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class Wrap
    Inherits SignalProcessStep
    Public Sub New ()
        InputChannels = New ObservableCollection(Of SignalSignatures)
        OutputChannels = New ObservableCollection(Of SignalSignatures)
        ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
        ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
        Name = "Wrap"
        IsExpanded = False
    End Sub

    Public Overrides Property Name As String

End Class

Public Class TunableFilter
    Inherits Filter

    Public Sub New ()
        MyBase.New
        _filterParameterDictionary = New Dictionary(Of TunableFilterType, List(Of String)) From{{TunableFilterType.Rational, {"Numerator", "Denominator", "ZeroPhase"}.ToList()},
                                                                                                {TunableFilterType.HighPass, {"Order", "Cutoff", "ZeroPhase"}.ToList()},
                                                                                                {TunableFilterType.LowPass, {"PassRipple", "StopRipple", "PassCutoff", "StopCutoff", "ZeroPhase"}.ToList()},
                                                                                                {TunableFilterType.Median, {"Order", "Endpoints", "HandleNaN"}.ToList()}}
        InputChannels = New ObservableCollection(Of SignalSignatures)
        OutputChannels = New ObservableCollection(Of SignalSignatures)
        ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
        ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
        Name = "Filter"
        Type = TunableFilterType.Rational
        IsExpanded = False
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
            for Each p in _filterParameterDictionary(_type)
                dim newPair = New ParameterValuePair
                newPair.ParameterName = p
                If p = "ZeroPhase"
                    newPair.Value = False
                ElseIf p = "Endpoints"
                    newPair.Value = EndpointsType.zeropad
                ElseIf p = "HandleNaN"
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
    End Class

Public Class Multirate
    Inherits Filter
    Public Sub New ()
        InputChannels = New ObservableCollection(Of SignalSignatures)
        OutputChannels = New ObservableCollection(Of SignalSignatures)
        ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
        ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
        _name = "Multirate"
        _pElement = 1
        _qElement = 1
        _newRate = 1
        IsExpanded = False
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
            dim q = 0
            Integer.TryParse(_qElement, q)
            if q <> 0
                dim p = 0
                Integer.TryParse(_pElement, p)
                for index = 0 to OutputChannels.Count - 1
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
            dim q = 0
            Integer.TryParse(_qElement, q)
            if q <> 0
                dim p = 0
                Integer.TryParse(_pElement, p)
                for index = 0 to OutputChannels.Count - 1
                    OutputChannels(index).SamplingRate = InputChannels(index).SamplingRate * p / q
                Next
            End If
            OnPropertyChanged()
        End Set
    End Property

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
    Public Sub New ()
        _nameTypeUnitPMUList = New ObservableCollection(Of NameTypeUnitPMU)()
    End Sub
    Private _newUnit As String
    Public Property NewUnit As String
        Get
            Return _newUnit
        End Get
        Set(value As String)
            _newUnit = value
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
    Public Sub New ()
        InputChannels = New ObservableCollection(Of SignalSignatures)
        OutputChannels = New ObservableCollection(Of SignalSignatures)
        ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
        ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatures)
        _name = "Signal Type and Unit Specification"
        IsExpanded = False
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
            if value <> "" And OutputChannels.Count > 0
                OutputChannels(0).SignalName = value
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
            For Each signal In OutputChannels
                signal.Unit = value
            Next
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
            For Each signal In OutputChannels
                signal.TypeAbbreviation = value
            Next
            OnPropertyChanged()
        End Set
    End Property
End Class