Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data
Imports BAWGUI.Settings.Model
Imports BAWGUI.Core

Namespace Converters
    Public Class ExpanderHeaderConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value IsNot Nothing Then
                Return "Step " & value.StepCounter & " - " & value.Name
            Else
                Return "No Step Selected Yet!"
            End If
        End Function
        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class

    'Public Class DetectorExpanderHeaderConverter
    '    Implements IMultiValueConverter

    '    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
    '        If values(0) IsNot Nothing Then
    '            'Dim detectorList =
    '            Return (values(1).IndexOf(values(0)) + 1).ToString & " - " & values(0).Name
    '        Else
    '            Return "No detector Selected Yet!"
    '        End If
    '    End Function

    '    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
    '        Return DependencyProperty.UnsetValue
    '    End Function
    'End Class

    Public Class DetectorExpanderHeaderConverter2
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            Dim str = ""
            If values(0) IsNot Nothing Then
                If TypeOf values(0) Is DetectorBase Then
                    str = (values(1).IndexOf(values(0)) + 1).ToString & " - " & values(0).Name
                End If
                If TypeOf values(0) Is AlarmingDetectorBase Then
                    str = (values(2).IndexOf(values(0)) + 1).ToString & " - Alarm Configuration: " & values(0).Name
                End If
            Else
                str = "No detector Selected Yet!"
            End If
            Return str
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class

    Public Class TunableFilterExpanderHeaderConverter
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            If values IsNot Nothing Then
                Dim number = values(0)
                Dim type = values(1)
                Select Case type
                    Case TunableFilterType.HighPass
                        Return "Step" & number.ToString & " - High-Pass Filter"
                    Case TunableFilterType.LowPass
                        Return "Step" & number.ToString & " - Low-Pass Filter"
                    Case Else
                        Return "Step" & number.ToString & " - Rational Filter"
                End Select
            Else
                Return ""
            End If
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class

    Public Class EmptyHeaderConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value Is Nothing Then
                Return parameter
            Else
                Return value
            End If
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class
End Namespace
