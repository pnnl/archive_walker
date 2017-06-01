Imports System.Globalization

Namespace Converters
    Public Class ValueIfEmptyConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If String.IsNullOrEmpty(value) Then
                Return parameter
            Else
                Return value
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class
End Namespace
