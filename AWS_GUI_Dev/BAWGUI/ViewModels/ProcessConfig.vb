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