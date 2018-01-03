Imports BAWGUI

Public Class PMUWithSamplingRate
    Inherits ViewModelBase
    Public Sub New(pmu As String, rate As Integer)
        _pmu = pmu
        _samplingRate = rate
    End Sub
    Public Sub New()

    End Sub
    Private _pmu As String
    Public Property PMU As String
        Get
            Return _pmu
        End Get
        Set(ByVal value As String)
            _pmu = value
            OnPropertyChanged()
        End Set
    End Property
    Private _samplingRate As Integer
    Public Property SamplingRate As Integer
        Get
            Return _samplingRate
        End Get
        Set(ByVal value As Integer)
            _samplingRate = value
            OnPropertyChanged()
        End Set
    End Property
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing OrElse Not Me.GetType Is obj.GetType Then
            Return False
        End If
        Dim p As PMUWithSamplingRate = CType(obj, PMUWithSamplingRate)
        Return Me.PMU = p.PMU ' AndAlso Me.SamplingRate = p.SamplingRate
    End Function
    Public Overrides Function GetHashCode() As Integer
        'If item Is Nothing Then Return 0
        'Dim hashItemName = If(item.PMU Is Nothing, 0, item.PMU.GetHashCode())
        'Dim hashRate = item.SamplingRate.GetHashCode()
        Return PMU.GetHashCode() ' Xor SamplingRate.GetHashCode()
    End Function
    Public Shared Operator =(ByVal x As PMUWithSamplingRate, ByVal y As PMUWithSamplingRate) As Boolean
        Return x.PMU = y.PMU ' AndAlso x.SamplingRate = y.SamplingRate
    End Operator
    Public Shared Operator <>(ByVal x As PMUWithSamplingRate, ByVal y As PMUWithSamplingRate) As Boolean
        Return x.PMU <> y.PMU ' OrElse x.SamplingRate <> y.SamplingRate
    End Operator
End Class

Public Class PMUWithSamplingRateComparer
    Implements IEqualityComparer(Of PMUWithSamplingRate)

    Public Overloads Function Equals(ByVal x As PMUWithSamplingRate, ByVal y As PMUWithSamplingRate) As Boolean Implements IEqualityComparer(Of PMUWithSamplingRate).Equals
        If x Is y Then Return True
        If x Is Nothing OrElse y Is Nothing Then Return False
        Return (x.PMU = y.PMU) ' AndAlso (x.SamplingRate = y.SamplingRate)
    End Function
    Public Overloads Function GetHashCode(ByVal item As PMUWithSamplingRate) As Integer Implements IEqualityComparer(Of PMUWithSamplingRate).GetHashCode
        If item Is Nothing Then Return 0
        Dim hashItemName = If(item.PMU Is Nothing, 0, item.PMU.GetHashCode())
        'Dim hashRate = item.SamplingRate.GetHashCode()
        Return hashItemName ' Xor hashRate
    End Function
End Class
