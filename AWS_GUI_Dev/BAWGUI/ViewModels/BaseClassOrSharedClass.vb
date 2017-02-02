Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class ParameterValuePair
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Raise property changed event
    ''' </summary>
    ''' <param name="sender">The event sender</param>
    ''' <param name="e">The event</param>
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Private Sub OnPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub
    Public Sub New()

    End Sub
    Public Sub New(para As String, value As String)
        _parameterName = para
        _value = value
    End Sub
    Private _parameterName As String
    Public Property ParameterName As String
        Get
            Return _parameterName
        End Get
        Set(ByVal value As String)
            _parameterName = value
            OnPropertyChanged("ParameterName")
        End Set
    End Property
    Private _value As Object
    Public Property Value As Object
        Get
            Return _value
        End Get
        Set(ByVal value As Object)
            _value = value
            OnPropertyChanged("Value")
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
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Raise property changed event
    ''' </summary>
    ''' <param name="sender">The event sender</param>
    ''' <param name="e">The event</param>
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Private Sub OnPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub
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
            OnPropertyChanged("PMUName")
        End Set
    End Property
    Private _channels As ObservableCollection(Of String)
    Public Property Channels As ObservableCollection(Of String)
        Get
            Return _channels
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            _channels = value
            OnPropertyChanged("Channels")
        End Set
    End Property
End Class

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''Class Filter''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class Filter
    Inherits SignalProcessStep
    'Implements INotifyPropertyChanged
    '''' <summary>
    '''' Raise property changed event
    '''' </summary>
    '''' <param name="sender">The event sender</param>
    '''' <param name="e">The event</param>
    'Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    'Protected Sub OnPropertyChanged(ByVal info As String)
    '    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    'End Sub
    Public Sub New()
        _filterParameters = New ObservableCollection(Of ParameterValuePair)
        _parameters = New ObservableCollection(Of ParameterValuePair)
    End Sub
    Private _name As String
    Public Overrides Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            OnPropertyChanged("Name")
        End Set
    End Property

    Private _filterName As String
    Public Property FilterName As String
        Get
            Return _filterName
        End Get
        Set(ByVal value As String)
            _filterName = value
            OnPropertyChanged("FilterName")
        End Set
    End Property

    Private _parameters As ObservableCollection(Of ParameterValuePair)
    Public Overrides Property Parameters As ObservableCollection(Of ParameterValuePair)
        Get
            Return _parameters
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _parameters = value
            OnPropertyChanged("Parameters")
        End Set
    End Property

    Private _filterParameters As ObservableCollection(Of ParameterValuePair)
    Public Property FilterParameters As ObservableCollection(Of ParameterValuePair)
        Get
            Return _filterParameters
        End Get
        Set(ByVal value As ObservableCollection(Of ParameterValuePair))
            _filterParameters = value
            OnPropertyChanged("FilterParameters")
        End Set
    End Property
End Class

Public MustInherit Class SignalProcessStep
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Raise property changed event
    ''' </summary>
    ''' <param name="sender">The event sender</param>
    ''' <param name="e">The event</param>
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub

    Public MustOverride Property Name As String
    Public MustOverride Property Parameters As ObservableCollection(Of ParameterValuePair)
    Private _stepCounter As Integer
    Public Property StepCounter As Integer
        Get
            Return _stepCounter
        End Get
        Set(ByVal value As Integer)
            _stepCounter = value
            OnPropertyChanged("StepCounter")
        End Set
    End Property
End Class