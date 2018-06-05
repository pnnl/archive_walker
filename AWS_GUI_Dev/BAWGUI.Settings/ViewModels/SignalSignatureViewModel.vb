Imports BAWGUI.Core
Imports BAWGUI.Settings.ViewModels

Namespace ViewModel
    Public Class SignalSignatureViewModel
        Inherits ViewModelBase
        'Implements IDisposable
        Private _model As BAWGUI.Core.SignalSignatures
        Public Sub New()
            _model = New Core.SignalSignatures
            '_model.IsEnabled = True
            '_model.IsValid = True
            '_model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0
            _model.PassedThroughProcessor = 0
            '_model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1
            _model.Unit = "O"
        End Sub
        Public Sub New(name As String)
            _model = New Core.SignalSignatures
            _model.SignalName = name
            '_model.IsEnabled = True
            '_model.IsValid = True
            '_model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0
            _model.PassedThroughProcessor = 0
            '_model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1
            _model.Unit = "O"
        End Sub
        Public Sub New(name As String, pmu As String)
            _model = New Core.SignalSignatures
            _model.SignalName = name
            _model.PMUName = pmu
            '_model.IsEnabled = True
            '_model.IsValid = True
            '_model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0
            _model.PassedThroughProcessor = 0
            '_model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1
            _model.Unit = "O"
        End Sub
        Public Sub New(name As String, pmu As String, type As String)
            _model = New Core.SignalSignatures
            _model.SignalName = name
            _model.PMUName = pmu
            _model.TypeAbbreviation = type
            '_model.IsEnabled = True
            '_model.IsValid = True
            '_model.IsCustomSignal = False
            _model.PassedThroughDQFilter = 0
            _model.PassedThroughProcessor = 0
            '_model.IsNameTypeUnitChanged = False
            _model.SamplingRate = -1
            _model.Unit = "O"
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
                Return _model.IsValid
            End Get
            Set(ByVal value As Boolean)
                _model.IsValid = value
                OnPropertyChanged()
            End Set
        End Property
        Private _PMUName As String
        Public Property PMUName As String
            Get
                Return _model.PMUName
            End Get
            Set(ByVal value As String)
                _model.PMUName = value
                OnPropertyChanged()
            End Set
        End Property
        Private _signalName As String
        Public Property SignalName As String
            Get
                Return _model.SignalName
            End Get
            Set(ByVal value As String)
                _model.SignalName = value
                If Not String.IsNullOrEmpty(value) AndAlso String.IsNullOrEmpty(OldSignalName) Then
                    OldSignalName = value
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _typeAbbreviation As String
        Public Property TypeAbbreviation As String
            Get
                Return _model.TypeAbbreviation
            End Get
            Set(ByVal value As String)
                _model.TypeAbbreviation = value
                If Not String.IsNullOrEmpty(value) AndAlso String.IsNullOrEmpty(OldTypeAbbreviation) Then
                    OldTypeAbbreviation = value
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _unit As String
        Public Property Unit As String
            Get
                Return _model.Unit
            End Get
            Set(ByVal value As String)
                _model.Unit = value
                If Not String.IsNullOrEmpty(value) AndAlso String.IsNullOrEmpty(OldUnit) Then
                    OldUnit = value
                End If
                OnPropertyChanged()
            End Set
        End Property
        Private _isChecked? As Boolean = False
        Public Property IsChecked As Boolean?
            Get
                Return _model.IsChecked
            End Get
            Set(ByVal value As Boolean?)
                _model.IsChecked = value
                OnPropertyChanged()
            End Set
        End Property
        Private _isEnabled As Boolean
        Public Property IsEnabled As Boolean
            Get
                Return _model.IsEnabled
            End Get
            Set(ByVal value As Boolean)
                _model.IsEnabled = value
                OnPropertyChanged()
            End Set
        End Property
        Private _isCustomSignal As Boolean
        Public Property IsCustomSignal As Boolean
            Get
                Return _model.IsCustomSignal
            End Get
            Set(ByVal value As Boolean)
                _model.IsCustomSignal = value
                OnPropertyChanged()
            End Set
        End Property

        Private _samplingRate As Integer
        Public Property SamplingRate As Integer
            Get
                Return _model.SamplingRate
            End Get
            Set(value As Integer)
                _model.SamplingRate = value
                OnPropertyChanged()
            End Set
        End Property

        Private _passedThroughDQFilter As Integer
        Public Property PassedThroughDQFilter As Integer
            Get
                Return _model.PassedThroughDQFilter
            End Get
            Set(value As Integer)
                _model.PassedThroughDQFilter = value
                OnPropertyChanged()
            End Set
        End Property

        Private _passedThroughProcessor As Integer
        Public Property PassedThroughProcessor As Integer
            Get
                Return _model.PassedThroughProcessor
            End Get
            Set(value As Integer)
                _model.PassedThroughProcessor = value
                OnPropertyChanged()
            End Set
        End Property

        Private _isNameTypeUnitChanged As Boolean
        Public Property IsNameTypeUnitChanged As Boolean
            Get
                Return _model.IsNameTypeUnitChanged
            End Get
            Set(ByVal value As Boolean)
                _model.IsNameTypeUnitChanged = value
                OnPropertyChanged()
            End Set
        End Property

        Public Overrides Function Equals(obj As Object) As Boolean
            If obj Is Nothing OrElse Not Me.GetType Is obj.GetType Then
                Return False
            End If
            Dim p As SignalSignatureViewModel = CType(obj, SignalSignatureViewModel)
            Return Me.PMUName = p.PMUName AndAlso Me.SignalName = p.SignalName AndAlso Me.TypeAbbreviation = p.TypeAbbreviation AndAlso Me.Unit = p.Unit AndAlso Me.OldSignalName = p.OldSignalName AndAlso Me.OldTypeAbbreviation = p.OldTypeAbbreviation AndAlso Me.OldUnit = p.OldUnit AndAlso Me.SamplingRate = p.SamplingRate
        End Function
        'Public Overrides Function GetHashCode() As Integer
        '    Return MyBase.GetHashCode()
        'End Function
        Public Shared Operator =(ByVal x As SignalSignatureViewModel, ByVal y As SignalSignatureViewModel) As Boolean
            Return x.PMUName = y.PMUName AndAlso x.SignalName = y.SignalName AndAlso x.TypeAbbreviation = y.TypeAbbreviation AndAlso x.Unit = y.Unit AndAlso x.OldSignalName = y.OldSignalName AndAlso x.OldTypeAbbreviation = y.OldTypeAbbreviation AndAlso x.OldUnit = y.OldUnit AndAlso x.SamplingRate = y.SamplingRate
        End Operator
        Public Shared Operator <>(ByVal x As SignalSignatureViewModel, ByVal y As SignalSignatureViewModel) As Boolean
            Return x.PMUName <> y.PMUName OrElse x.SignalName <> y.SignalName OrElse x.TypeAbbreviation <> y.TypeAbbreviation OrElse x.Unit <> y.Unit OrElse x.OldSignalName <> y.OldSignalName OrElse x.OldTypeAbbreviation <> y.OldTypeAbbreviation OrElse x.OldUnit <> y.OldUnit OrElse x.SamplingRate <> y.SamplingRate
        End Operator

        Public Function IsSignalInformationComplete() As Boolean
            Return Not String.IsNullOrEmpty(PMUName) AndAlso Not String.IsNullOrEmpty(SignalName) AndAlso Not String.IsNullOrEmpty(TypeAbbreviation) AndAlso Not String.IsNullOrEmpty(Unit) AndAlso (SamplingRate > 0)
        End Function

        Private _oldSignalName As String
        Public Property OldSignalName As String
            Get
                Return _model.OldSignalName
            End Get
            Set(ByVal value As String)
                _model.OldSignalName = value
                OnPropertyChanged()
            End Set
        End Property

        Private _oldUnit As String
        Public Property OldUnit As String
            Get
                Return _model.OldUnit
            End Get
            Set(ByVal value As String)
                _model.OldUnit = value
                OnPropertyChanged()
            End Set
        End Property

        Private _oldTypeAbbreviation As String
        Public Property OldTypeAbbreviation As String
            Get
                Return _model.OldTypeAbbreviation
            End Get
            Set(ByVal value As String)
                _model.OldTypeAbbreviation = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

End Namespace
