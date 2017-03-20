Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class ViewModelBase
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Raise property changed event
    ''' </summary>
    ''' <param name="sender">The event sender</param>
    ''' <param name="e">The event</param>
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Protected Overridable Sub OnPropertyChanged(<Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Protected Overridable Function CanExecute(ByVal param As Object) As Boolean
        Return True
    End Function
End Class

Public Class ParameterValuePair
    Inherits ViewModelBase
    Public Sub New()
        _isRequired = True
    End Sub
    Public Sub New(para As String, value As Object, required As Boolean)
        _parameterName = para
        _value = value
        _isRequired = required
    End Sub
    Public Sub New(para As String, value As Object)
        _parameterName = para
        _value = value
        _isRequired = True
    End Sub
    Private _parameterName As String
    Public Property ParameterName As String
        Get
            Return _parameterName
        End Get
        Set(ByVal value As String)
            _parameterName = value
            OnPropertyChanged()
        End Set
    End Property
    Private _value As Object
    Public Property Value As Object
        Get
            Return _value
        End Get
        Set(ByVal value As Object)
            _value = value
            OnPropertyChanged()
        End Set
    End Property
    Private _isRequired As Boolean
    Public Property IsRequired As Boolean
        Get
            Return _isRequired
        End Get
        Set(ByVal value As Boolean)
            _isRequired = value
            OnPropertyChanged()
        End Set
    End Property
    Private _toolTip As String
    Public Property ToolTip As String
        Get
            Return _toolTip
        End Get
        Set(ByVal value As String)
            _toolTip = value
            OnPropertyChanged()
        End Set
    End Property
    'Private _value As String
    'Public Property Value As String
    '    Get
    '        Return _value
    '    End Get
    '    Set(ByVal value As String)
    '        _value = value
    '    End Set
    'End Property
End Class

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''Class PMU''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class PMU
    Inherits ViewModelBase
    Public Sub New()
        _channels = New ObservableCollection(Of String)
    End Sub
    Private _PMUName As String
    Public Property PMUName As String
        Get
            Return _PMUName
        End Get
        Set(ByVal value As String)
            _PMUName = value
            OnPropertyChanged()
        End Set
    End Property
    Private _channels As ObservableCollection(Of String)
    Public Property Channels As ObservableCollection(Of String)
        Get
            Return _channels
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            _channels = value
            OnPropertyChanged()
        End Set
    End Property
End Class

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''Class Filter''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class Filter
    Inherits SignalProcessStep
    Public Sub New()
        '_filterParameters = New ObservableCollection(Of ParameterValuePair)
        _parameters = New ObservableCollection(Of ParameterValuePair)
        InputChannels = New ObservableCollection(Of SignalSignatures)
        ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatures)
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

    Private _filterName As String
    Public Property FilterName As String
        Get
            Return _filterName
        End Get
        Set(ByVal value As String)
            _filterName = value
            OnPropertyChanged()
        End Set
    End Property

    Private _parameters As ObservableCollection(Of ParameterValuePair)
    Public Overrides Property Parameters As ObservableCollection(Of ParameterValuePair)
        Get
            Return _parameters
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _parameters = value
            OnPropertyChanged()
        End Set
    End Property

    Private _filterParameters As ObservableCollection(Of ParameterValuePair)
    Public Property FilterParameters As ObservableCollection(Of ParameterValuePair)
        Get
            Return _filterParameters
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _filterParameters = value
            OnPropertyChanged()
        End Set
    End Property
End Class

Public MustInherit Class SignalProcessStep
    Inherits ViewModelBase

    Public MustOverride Property Name As String
    Public MustOverride Property Parameters As ObservableCollection(Of ParameterValuePair)
    Private _stepCounter As Integer
    Public Property StepCounter As Integer
        Get
            Return _stepCounter
        End Get
        Set(ByVal value As Integer)
            _stepCounter = value

            OnPropertyChanged()
        End Set
    End Property

    Private _isStepSelected As Boolean
    Public Property IsStepSelected As Boolean
        Get
            Return _isStepSelected
        End Get
        Set(ByVal value As Boolean)
            _isStepSelected = value
            OnPropertyChanged()
        End Set
    End Property

    'Private _areSignalSelected As Boolean
    'Public Property AreSignalSelected As Boolean
    '    Get
    '        Return _areSignalSelected
    '    End Get
    '    Set(ByVal value As Boolean)
    '        _areSignalSelected = value
    '        OnPropertyChanged()
    '    End Set
    'End Property

    Private _inputChannels As ObservableCollection(Of SignalSignatures)
    Public Property InputChannels As ObservableCollection(Of SignalSignatures)
        Get
            Return _inputChannels
        End Get
        Set(ByVal value As ObservableCollection(Of SignalSignatures))
            _inputChannels = value
            OnPropertyChanged()
        End Set
    End Property

    'Private _inputChannelsSortedByType As ObservableCollection(Of SignalTypeHierachy)
    'Public Property InputChannelsSortedByType As ObservableCollection(Of SignalTypeHierachy)
    '    Get
    '        Return _inputChannelsSortedByType
    '    End Get
    '    Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
    '        _inputChannelsSortedByType = value
    '        OnPropertyChanged()
    '    End Set
    'End Property
    'ThisStepAsSignalHerachy
    Private _thisStepInputsAsSignalHierachyByType As SignalTypeHierachy
    Public Property ThisStepInputsAsSignalHerachyByType As SignalTypeHierachy
        Get
            Return _thisStepInputsAsSignalHierachyByType
        End Get
        Set(ByVal value As SignalTypeHierachy)
            _thisStepInputsAsSignalHierachyByType = value
            OnPropertyChanged()
        End Set
    End Property

    'Private _inputSignalFromPreviousStepOutput As ObservableCollection(Of String)
    'Public Property InputSignalFromPreviousStepOutput As ObservableCollection(Of String)
    '    Get
    '        Return _inputSignalFromPreviousStepOutput
    '    End Get
    '    Set(ByVal value As ObservableCollection(Of String))
    '        _inputSignalFromPreviousStepOutput = value
    '        OnPropertyChanged()
    '    End Set
    'End Property
End Class