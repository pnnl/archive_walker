Imports System.Drawing
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data

Namespace Converters

    'Public Class SelectionStatusBackgroundConverter
    '    Implements IValueConverter

    '    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    '        If value Then
    '            Return "White"
    '        Else
    '            Return "WhiteSmoke"
    '        End If
    '    End Function

    '    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    '        Return DependencyProperty.UnsetValue
    '    End Function
    'End Class

    Public Class ErrorStatusBorderColorConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value Then
                'Return Brushes.LightGray
                Return Brushes.Black
            Else
                Return "Maroon"
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class

End Namespace
