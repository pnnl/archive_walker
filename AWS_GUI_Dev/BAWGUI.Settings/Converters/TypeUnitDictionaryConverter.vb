Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data

Namespace Converters
    Public Class TypeUnitDictionaryConverter
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            Dim key = values(1)
            If String.IsNullOrEmpty(key) Then
                Return DependencyProperty.UnsetValue
            Else
                Return values(0)(key)
            End If
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class
    Public Class UnitMetricConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim type = value.ToString.ToList.FirstOrDefault
            Select Case type
                Case "V"
                    Return New List(Of String) From {"kV", "V"}.ToList
                Case "I"
                    Return New List(Of String) From {"A", "kA"}.ToList
                Case "R"
                    Return New List(Of String) From {"Hz/sec", "mHz/sec"}.ToList
                Case "F"
                    Return New List(Of String) From {"Hz", "mHz"}.ToList
                Case Else
                    Return DependencyProperty.UnsetValue
            End Select
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class
End Namespace