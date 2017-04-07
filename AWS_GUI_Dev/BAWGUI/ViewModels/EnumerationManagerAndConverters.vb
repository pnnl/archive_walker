Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Linq

Public Class EnumerationManager
    Public Shared Function GetValues(enumeration As Type) As Array
        Dim wArray As Array = [Enum].GetValues(enumeration)
        Dim wFinalArray As New ArrayList()
        For Each wValue As [Enum] In wArray
            Dim fi As FieldInfo = enumeration.GetField(wValue.ToString())
            If fi IsNot Nothing Then
                Dim wBrowsableAttributes As BrowsableAttribute() = TryCast(fi.GetCustomAttributes(GetType(BrowsableAttribute), True), BrowsableAttribute())
                If wBrowsableAttributes.Length > 0 Then
                    '  If the Browsable attribute is false
                    If wBrowsableAttributes(0).Browsable = False Then
                        ' Do not add the enumeration to the list.
                        Continue For
                    End If
                End If

                Dim wDescriptions As DescriptionAttribute() = TryCast(fi.GetCustomAttributes(GetType(DescriptionAttribute), True), DescriptionAttribute())
                If wDescriptions.Length > 0 Then
                    wFinalArray.Add(wDescriptions(0).Description)
                Else
                    wFinalArray.Add(wValue)
                End If
            End If
        Next

        Return wFinalArray.ToArray()
    End Function
End Class

Public Class EnumToStringConverter1
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case DataFileType.csv
                Return "JSIS CSV"
            Case DataFileType.pdat
                Return "PDAT"
            Case Else
                Throw New Exception("Data file type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "JSIS CSV"
                Return DataFileType.csv
            Case "PDAT"
                Return DataFileType.pdat
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter2
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case ModeType.RealTime
                Return "Real Time"
            Case Else
                Return value
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "Real Time"
                Return ModeType.RealTime
            Case Else
                Return value
        End Select
    End Function
End Class

Public Class EnumToStringConverter3
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case PowerType.Active
                Return "Active"
            Case PowerType.Apparent
                Return "Apparent"
            Case PowerType.Complex
                Return "Complex"
            Case PowerType.Reactive
                Return "Reactive"
            Case Else
                Throw New Exception("Power type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "Reactive"
                Return PowerType.Reactive
            Case "Apparent"
                Return PowerType.Apparent
            Case "Complex"
                Return PowerType.Complex
            Case "Active"
                Return PowerType.Active
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class SelectionStatusBackgroundConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Then
            Return "White"
        Else
            Return "WhiteSmoke"
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function
End Class

Public Class ErrorStatusBorderColorConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Then
            Return Brushes.LightGray
        Else
            Return "Red"
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function
End Class

Public Class ExpanderHeaderConverter
    'Implements IMultiValueConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value IsNot Nothing Then
            Return "Step " & value.StepCounter & " - " & value.Name
        Else
            Return "No Step Selected Yet!"
        End If
    End Function

    'Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
    '    Return "Step " & values(0) & " - " & values(1)
    'End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function

    'Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
    '    Return DependencyProperty.UnsetValue
    'End Function
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
        Throw New NotImplementedException()
    End Function
End Class

Public Class InverseBooleanConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Return Not value
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return Not value
    End Function
End Class

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