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
                    wFinalArray.Add(wValue.ToString())
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
            Case PowerType.P
                Return "Active"
            Case PowerType.S
                Return "Apparent"
            Case PowerType.CP
                Return "Complex"
            Case PowerType.Q
                Return "Reactive"
            Case Else
                Throw New Exception("Power type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "Reactive"
                Return PowerType.Q
            Case "Apparent"
                Return PowerType.S
            Case "Complex"
                Return PowerType.CP
            Case "Active"
                Return PowerType.P
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter4
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case InterpolateType.Constant
                Return "Constant"
            Case InterpolateType.Linear
                Return "Linear"
            Case Else
                Throw New Exception("Interpolate type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "Constant"
                Return InterpolateType.Constant
            Case "Linear"
                Return InterpolateType.Linear
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter5
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case EndpointsType.truncate
                Return "truncate"
            Case EndpointsType.zeropad
                Return "zeropad"
            Case Else
                Throw New Exception("Endpoints type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "truncate"
                Return EndpointsType.truncate
            Case "zeropad"
                Return EndpointsType.zeropad
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter6
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case HandleNaNType.includenan
                Return "includenan"
            Case HandleNaNType.omitnan
                Return "omitnan"
            Case Else
                Throw New Exception("HandleNaN type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "includenan"
                Return HandleNaNType.includenan
            Case "omitnan"
                Return HandleNaNType.omitnan
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter7
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case TunableFilterType.Rational
                Return "Rational"
            Case TunableFilterType.HighPass
                Return "HighPass"
            Case TunableFilterType.LowPass
                Return "LowPass"
            Case TunableFilterType.Median
                Return "Median"
            Case Else
                Throw New Exception("TunableFilter type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "Rational"
                Return TunableFilterType.Rational
            Case "HighPass"
                Return TunableFilterType.HighPass
            Case "LowPass"
                Return TunableFilterType.LowPass
            Case "Median"
                Return TunableFilterType.Median
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter8
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case DetectorModeType.SingleChannel
                Return "SingleChannel"
            Case DetectorModeType.MultiChannel
                Return "MultiChannel"
            Case Else
                Throw New Exception("Detector mode type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "SingleChannel"
                Return DetectorModeType.SingleChannel
            Case "MultiChannel"
                Return DetectorModeType.MultiChannel
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter9
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case DetectorWindowType.bartlett
                Return "bartlett"
            Case DetectorWindowType.blackman
                Return "blackman"
            Case DetectorWindowType.hamming
                Return "hamming"
            Case DetectorWindowType.hann
                Return "hann"
            Case DetectorWindowType.rectwin
                Return "rectwin"
            Case Else
                Throw New Exception("Detector window type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "bartlett"
                Return DetectorWindowType.bartlett
            Case "blackman"
                Return DetectorWindowType.blackman
            Case "hamming"
                Return DetectorWindowType.hamming
            Case "hann"
                Return DetectorWindowType.hann
            Case "rectwin"
                Return DetectorWindowType.rectwin
            Case Else
                Throw New Exception("Enum type not valid!")
        End Select
    End Function
End Class

Public Class EnumToStringConverter10
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Select Case value
            Case OutOfRangeFrequencyDetectorType.AvergeWindow
                Return "AvergeWindow"
            Case OutOfRangeFrequencyDetectorType.Nominal
                Return "Nominal"
            Case Else
                Throw New Exception("Out of range frequency detector type not valid!")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Select Case value
            Case "Nominal"
                Return OutOfRangeFrequencyDetectorType.Nominal
            Case "AvergeWindow"
                Return OutOfRangeFrequencyDetectorType.AvergeWindow
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

Public Class DQFilteredSignalFlagConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value
            Return Visibility.Visible
        else
            Return Visibility.Collapsed
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function
End Class

Public Class ProcessedSignalFlagConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value
            Return Visibility.Visible
        else
            Return Visibility.Collapsed
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function
End Class

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

Public Class DetectorExpanderHeaderConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        If values(0) IsNot Nothing Then
            'Dim detectorList =
            Return (values(1).IndexOf(values(0)) + 1).ToString & " - Detector " & values(0).Name
        Else
            Return "No detector Selected Yet!"
        End If
    End Function

    'Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    '    If value IsNot Nothing Then
    '        'Dim detectorList =
    '        Return value.Name & " Detector"
    '    Else
    '        Return "No detector Selected Yet!"
    '    End If
    'End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function

    'Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    '    Return DependencyProperty.UnsetValue
    'End Function
End Class

Public Class DetectorExpanderHeaderConverter2
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim str = ""
        If values(0) IsNot Nothing Then
            If TypeOf values(0) Is DetectorBase Then
                str = (values(1).IndexOf(values(0)) + 1).ToString & " - Detector " & values(0).Name
            End If
            If TypeOf values(0) Is AlarmingDetectorBase Then
                str = (values(2).IndexOf(values(0)) + 1).ToString & " - Alarming Detector " & values(0).Name
            End If
        Else
            str = "No detector Selected Yet!"
        End If
        Return str
    End Function

    'Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    '    If value IsNot Nothing Then
    '        'Dim detectorList =
    '        Return value.Name & " Detector"
    '    Else
    '        Return "No detector Selected Yet!"
    '    End If
    'End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function

    'Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
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
        Return DependencyProperty.UnsetValue
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

Public Class InVisibleIfNothingConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing Xor parameter = "VisibleIfNothing" Then
            Return Visibility.Collapsed
        Else
            Return Visibility.Visible
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
                                       "Output from SignalSelectionAndManipulation by Signal Type",
                                       "Output from SignalSelectionAndManipulation by PMU",
                                       "Input to MultiRate steps",
                                       "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "All Initial Input Channels by Signal Type"
                Case 3
                    value1 = {"All Initial Input Channels by Signal Type",
                                       "All Initial Input Channels by PMU",
                                       "Output from SignalSelectionAndManipulation by Signal Type",
                                       "Output from SignalSelectionAndManipulation by PMU",
                                       "Output from ProcessConfig by Signal Type",
                                       "Output from ProcessConfig by PMU",
                                       "Input Channels by Step",
                                       "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "NOT Implemented Yet!"
                Case 4
                    value1 = {"All Initial Input Channels by Signal Type",
                                       "All Initial Input Channels by PMU",
                                       "Output from SignalSelectionAndManipulation by Signal Type",
                                       "Output from SignalSelectionAndManipulation by PMU",
                                       "Output from ProcessConfig by Signal Type",
                                       "Output from ProcessConfig by PMU",
                                       "Output from Post ProcessConfig by Signal Type",
                                       "Output from Post ProcessConfig by PMU",
                                       "Input Channels by Step"}.ToList
            End Select
        End If
        Return value1
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Return DependencyProperty.UnsetValue
    End Function
End Class

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
        Return DependencyProperty.UnsetValue
    End Function
End Class

Public Class ValueTypeConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is Boolean
            Return "Boolean"
        ElseIf TypeOf value Is String
            Return "String"
        Elseif TypeOf value Is EndpointsType
            Return "EndpointsType"
        ElseIf TypeOf value Is HandleNaNType
            Return "HandleNaNType"
        else
            Return DependencyProperty.UnsetValue
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

Public Class NewChannelVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value = 1 Or value = 0 Then
            Return True
        Else
            Return False
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