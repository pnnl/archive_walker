Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data
Imports BAWGUI.Core.Models

Namespace Converters
    Public Class PMUFilterInputTypeBooleanConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value = POWPMUFilterInputType.Current Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class
End Namespace
