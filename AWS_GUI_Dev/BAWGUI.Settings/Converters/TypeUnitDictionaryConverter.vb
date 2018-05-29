Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data

Namespace Converters
    Public Class TypeUnitDictionaryConverter
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            Dim key = values(1)
            Dim dict = values(0)
            'If key Is DependencyProperty.UnsetValue OrElse dict Is DependencyProperty.UnsetValue OrElse String.IsNullOrEmpty(key) Then
            '    Return DependencyProperty.UnsetValue
            'Else
            '    Return dict(key)
            'End If
            If TypeOf (key) Is String AndAlso Not String.IsNullOrEmpty(key) AndAlso TypeOf (dict) Is Dictionary(Of String, List(Of String)) Then
                Return dict(key)
            Else
                Return DependencyProperty.UnsetValue
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