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
        _isValid = True
        _isCustomSignal = False
        _passedThroughDQFilter = False
        _passedThroughProcessor = False
        _samplingRate = -1
        _unit = "O"
    End Sub
    Public Sub New(name As String)
        _signalName = name
        _isEnabled = True
        _isValid = True
        _isCustomSignal = False
        _passedThroughDQFilter = False
        _passedThroughProcessor = False
        _samplingRate = -1
        _unit = "O"
    End Sub
    Public Sub New(name As String, pmu As String)
        _signalName = name
        _PMUName = pmu
        _isEnabled = True
        _isValid = True
        _isCustomSignal = False
        _passedThroughDQFilter = False
        _passedThroughProcessor = False
        _samplingRate = -1
        _unit = "O"
    End Sub
    Public Sub New(name As String, pmu As String, type As String)
        _signalName = name
        _PMUName = pmu
        _typeAbbreviation = type
        _isEnabled = True
        _isValid = True
        _isCustomSignal = False
        _passedThroughDQFilter = False
        _passedThroughProcessor = False
        _samplingRate = -1
        _unit = "O"
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
    Private _isValid As Boolean
    Public Property IsValid As Boolean
        Get
            Return _isValid
        End Get
        Set(ByVal value As Boolean)
            _isValid = value
            OnPropertyChanged()
        End Set
    End Property
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
    Private _unit As String
    Public Property Unit As String
        Get
            Return _unit
        End Get
        Set(ByVal value As String)
            _unit = value
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
    Private _isCustomSignal As Boolean
    Public Property IsCustomSignal As Boolean
        Get
            Return _isCustomSignal
        End Get
        Set(ByVal value As Boolean)
            _isCustomSignal = value
            OnPropertyChanged()
        End Set
    End Property
    
    Private _samplingRate As Integer
    Public Property SamplingRate As Integer
        Get
            Return _samplingRate
        End Get
        Set(value As Integer)
            _samplingRate = value
            OnPropertyChanged()
        End Set
    End Property
    
    Private _passedThroughDQFilter As Boolean
    Public Property PassedThroughDQFilter As Boolean
        Get
            Return _passedThroughDQFilter
        End Get
        Set(value As Boolean)
            _passedThroughDQFilter = value
            OnPropertyChanged()
        End Set
    End Property
    
    Private _passedThroughProcessor As Boolean
    Public Property PassedThroughProcessor As Boolean
        Get
            Return _passedThroughProcessor
        End Get
        Set(value As Boolean)
            _passedThroughProcessor = value
            OnPropertyChanged()
        End Set
    End Property

    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing OrElse Not Me.GetType Is obj.GetType Then
            Return False
        End If
        Dim p As SignalSignatures = CType(obj, SignalSignatures)
        Return Me.PMUName = p.PMUName AndAlso Me.SignalName = p.SignalName AndAlso Me.TypeAbbreviation = p.TypeAbbreviation
    End Function
    'Public Overrides Function GetHashCode() As Integer
    '    Return MyBase.GetHashCode()
    'End Function
    Public Shared Operator =(ByVal x As SignalSignatures, ByVal y As SignalSignatures) As Boolean
        Return x.PMUName = y.PMUName AndAlso x.SignalName = y.SignalName AndAlso x.TypeAbbreviation = y.TypeAbbreviation
    End Operator
    Public Shared Operator <>(ByVal x As SignalSignatures, ByVal y As SignalSignatures) As Boolean
        Return x.PMUName <> y.PMUName OrElse x.SignalName <> y.SignalName OrElse x.TypeAbbreviation <> y.TypeAbbreviation
    End Operator

    Public Function IsSignalInformationComplete() As Boolean
        Return Not String.IsNullOrEmpty(PMUName) AndAlso Not String.IsNullOrEmpty(SignalName) AndAlso Not String.IsNullOrEmpty(TypeAbbreviation) AndAlso Not String.IsNullOrEmpty(Unit) AndAlso (SamplingRate > 0)
    End Function
End Class
