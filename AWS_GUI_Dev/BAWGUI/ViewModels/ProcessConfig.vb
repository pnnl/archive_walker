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
            for each signal in OutputChannels
                signal.SamplingRate = value
            Next
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