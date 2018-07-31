Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data

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


    'Public Class EmptyTextBoxConverter
    '    Implements IValueConverter

    '    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
    '        If value Is Nothing Then
    '            Return parameter
    '        Else
    '            Return value
    '        End If
    '    End Function

    '    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    '        Return DependencyProperty.UnsetValue
    '    End Function
    'End Class

End Namespace
