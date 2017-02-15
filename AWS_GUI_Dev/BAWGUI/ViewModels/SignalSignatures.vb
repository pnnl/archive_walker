Imports System.Collections.ObjectModel

Public Class SignalTypeHierachy
    Inherits ViewModelBase
    Public Sub New(signature As SignalSignatures)
        _signalSignature = signature
        _signalList = New ObservableCollection(Of SignalTypeHierachy)
    End Sub
    'Private _name As String
    'Public Property Name As String
    '    Get
    '        Return _name
    '    End Get
    '    Set(ByVal value As String)
    '        _name = value
    '        OnPropertyChanged()
    '    End Set
    'End Property
    Private _signalSignature As SignalSignatures
    Public Property SignalSignature As SignalSignatures
        Get
            Return _signalSignature
        End Get
        Set(ByVal value As SignalSignatures)
            _signalSignature = value
            OnPropertyChanged()
        End Set
    End Property
    Private _signalList As ObservableCollection(Of SignalTypeHierachy)
    Public Property SignalList As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _signalList
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _signalList = value
            OnPropertyChanged()
        End Set
    End Property
End Class
Public Class SignalSignatures
    Inherits ViewModelBase
    Public Sub New()

    End Sub
    Public Sub New(name As String)
        _signalName = name
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
    Private _signalName As String
    Public Property SignalName As String
        Get
            Return _signalName
        End Get
        Set(ByVal value As String)
            _signalName = value
            OnPropertyChanged()
        End Set
    End Property
    Private _typeAbbreviation As String
    Public Property TypeAbbreviation As String
        Get
            Return _typeAbbreviation
        End Get
        Set(ByVal value As String)
            _typeAbbreviation = value
            OnPropertyChanged()
        End Set
    End Property
    Private _isChecked? As Boolean = False
    Public Property IsChecked As Boolean?
        Get
            Return _isChecked
        End Get
        Set(ByVal value As Boolean?)
            _isChecked = value
            OnPropertyChanged()
        End Set
    End Property
End Class
