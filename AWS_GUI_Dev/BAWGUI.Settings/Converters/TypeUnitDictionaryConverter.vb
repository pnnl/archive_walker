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
            Dim type = value.ToString
            Select Case type
                Case "VMP", "VMA", "VMB", "VMC", "VPP", "VPA", "VPB", "VPC"
                    Return New List(Of String) From {"kV", "V"}.ToList
                Case "IMP", "IMA", "IMB", "IMC", "IPP", "IPA", "IPB", "IPC"
                    Return New List(Of String) From {"A", "kA"}.ToList
                'Case "VAP", "VAA", "VAB", "VAC", "IAP", "IAA", "IAB", "IAC"
                '    Return New List(Of String) From {"RAD", "DEG"}.ToList
                Case "R"
                    Return New List(Of String) From {"Hz/sec", "mHz/sec"}.ToList
                Case "F"
                    Return New List(Of String) From {"Hz", "mHz"}.ToList
                Case "P"
                    Return New List(Of String) From {"W", "kW", "MW"}.ToList
                Case "Q"
                    Return New List(Of String) From {"VAR", "kVAR", "MVAR"}.ToList
                Case "CP", "S"
                    Return New List(Of String) From {"VA", "kVA", "MVA"}.ToList
                    'Case "D"
                    '    Return New List(Of String) From {"D"}.ToList
                    'Case "SC"
                    '    Return New List(Of String) From {"SC"}.ToList
                    'Case "OTHER"
                    '    Return New List(Of String) From {"O", "kV", "V", "A", "kA", "RAD", "DEG", "Hz/sec", "mHz/sec", "Hz", "mHz", "W", "kW", "MW", "VAR", "kVAR", "MVAR", "VA", "kVA", "MVA", "D", "SC"}.ToList
                Case Else
                    Return DependencyProperty.UnsetValue
            End Select
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DependencyProperty.UnsetValue
        End Function
    End Class
End Namespace