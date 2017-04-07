Imports System.Globalization

Namespace Converters
    Public Class VisibleIfNonZeroConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim val = Double.Parse(value.ToString())
            If val = 0 Xor parameter = "HideIfNonZero" Then
                Return Visibility.Hidden
            Else
                Return Visibility.Visible
            End If
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
