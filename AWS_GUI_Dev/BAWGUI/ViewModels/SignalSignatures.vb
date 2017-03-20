Imports System.Collections.ObjectModel

Public Class SignalTypeHierachy
    Inherits ViewModelBase
    Public Sub New()
        _signalSignature = New SignalSignatures
        _signalList = New ObservableCollection(Of SignalTypeHierachy)
    End Sub
    Public Sub New(signature As SignalSignatures)
        _signalSignature = signature
        _signalList = New ObservableCollection(Of SignalTypeHierachy)
    End Sub
    Public Sub New(signature As SignalSignatures, list As ObservableCollection(Of SignalTypeHierachy))
        _signalSignature = signature
        _signalList = list
    End Sub
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
    'Implements IDisposable
    Public Sub New()
        _isEnabled = True
    End Sub
    Public Sub New(name As String)
        _signalName = name
        _isEnabled = True
    End Sub
    Public Sub New(name As String, pmu As String)
        _signalName = name
        _PMUName = pmu
        _isEnabled = True
    End Sub
    Public Sub New(name As String, pmu As String, type As String)
        _signalName = name
        _PMUName = pmu
        _typeAbbreviation = type
        _isEnabled = True
    End Sub
    'Public Sub Dispose() Implements IDisposable.Dispose
    '    'Dispose(True)
    '    GC.SuppressFinalize(Me)
    'End Sub
    'Private managedResource As System.ComponentModel.Component
    'Private unmanagedResource As IntPtr
    'Protected disposed As Boolean = False
    'Private Sub Dispose(v As Boolean)
    '    If Not Me.disposed Then
    '        If v Then
    '            managedResource.Dispose()
    '        End If
    '        ' Add code here to release the unmanaged resource.
    '        unmanagedResource = IntPtr.Zero
    '        ' Note that this is not thread safe. 
    '    End If
    '    Me.disposed = True
    'End Sub

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
    Private _isEnabled As Boolean
    Public Property IsEnabled As Boolean
        Get
            Return _isEnabled
        End Get
        Set(ByVal value As Boolean)
            _isEnabled = value
            OnPropertyChanged()
        End Set
    End Property
End Class
