Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data
Imports BAWGUI.Settings.Model

Namespace Converters
    Public Class SignalSignatureListStringConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            'Dim b = New ObservableCollection(Of SignalSignatures)(value)
            Dim a = New List(Of String)
            For Each item In value
                a.Add(item.SignalName)
            Next
            Return String.Join(vbCrLf, a)
            'Return String.Join(Of String)(vbCrLf, a)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class

    Public Class CurrentStepNameConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If value IsNot Nothing Then
                Return value.Name
            Else
                Return DependencyProperty.UnsetValue
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class


    Public Class SignalSelectionDropDownConverter
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            Dim value1 = values(0)
            Dim index = values(1)
            If TypeOf index Is Integer Then
                Select Case index
                    Case 0
                    Case 1
                        value1 = {"All Initial Input Channels by Signal Type",
                                           "All Initial Input Channels by PMU",
                                           "Input Channels by Step",
                                           "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "All Initial Input Channels by Signal Type"
                    Case 2
                        value1 = {"All Initial Input Channels by Signal Type",
                                           "All Initial Input Channels by PMU",
                                           "Output from Data Quality and Customization by Signal Type",
                                           "Output from Data Quality and Customization by PMU",
                                           "Input to MultiRate steps",
                                           "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "All Initial Input Channels by Signal Type"
                    Case 3
                        value1 = {"All Initial Input Channels by Signal Type",
                                           "All Initial Input Channels by PMU",
                                           "Output from Data Quality and Customization by Signal Type",
                                           "Output from Data Quality and Customization by PMU",
                                           "Output from Signal Processing by Signal Type",
                                           "Output from Signal Processing by PMU",
                                           "Input Channels by Step",
                                           "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "NOT Implemented Yet!"
                    Case 4
                        value1 = {"All Initial Input Channels by Signal Type",
                                           "All Initial Input Channels by PMU",
                                           "Output from Data Quality and Customization by Signal Type",
                                           "Output from Data Quality and Customization by PMU",
                                           "Output from Signal Processing by Signal Type",
                                           "Output from Signal Processing by PMU",
                                           "Output from Post-Processing Customization by Signal Type",
                                           "Output from Post-Processing Customization by PMU",
                                           "Input Channels by Step"}.ToList
                End Select
            End If
            Return value1
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class


    Public Class ValueTypeConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If TypeOf value Is Boolean Then
                Return "Boolean"
            ElseIf TypeOf value Is String Then
                Return "String"
            ElseIf TypeOf value Is EndpointsType Then
                Return "EndpointsType"
            ElseIf TypeOf value Is HandleNaNType Then
                Return "HandleNaNType"
            Else
                Return DependencyProperty.UnsetValue
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class


    Public Class AddCustomizationParameters
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            Return values.ToList
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class

    'Public Class TreeViewItemLabelConverter
    '    Implements IMultiValueConverter

    '    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
    '        If values(2) = 4 Then
    '            Return Nothing
    '        Else
    '            Return values(0)
    '        End If
    '    End Function

    '    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
    '        Return DependencyProperty.UnsetValue
    '    End Function
    'End Class

    Public Class PhasorCreationCommandParameterConverter
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            Return values.ToList
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class
End Namespace
