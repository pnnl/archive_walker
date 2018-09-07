Imports System.Globalization
Imports System.Windows.Data
Imports BAWGUI.Core.Models
Imports BAWGUI.Settings.ViewModels

Namespace Converters
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
                Case InterpolateType.Cubic
                    Return "Cubic"
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
                Case "Cubic"
                    Return InterpolateType.Cubic
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
                    Return "High-Pass"
                Case TunableFilterType.LowPass
                    Return "Low-Pass"
                    'Case TunableFilterType.Median
                    '    Return "Median"
                Case Else
                    Throw New Exception("TunableFilter type not valid!")
            End Select
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Select Case value
                Case "Rational"
                    Return TunableFilterType.Rational
                Case "High-Pass"
                    Return TunableFilterType.HighPass
                Case "Low-Pass"
                    Return TunableFilterType.LowPass
                    'Case "Median"
                    '    Return TunableFilterType.Median
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
                    Return "Single Channel"
                Case DetectorModeType.MultiChannel
                    Return "Multichannel"
                Case Else
                    Throw New Exception("Detector mode type not valid!")
            End Select
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Select Case value
                Case "Single Channel"
                    Return DetectorModeType.SingleChannel
                Case "Multichannel"
                    Return DetectorModeType.MultiChannel
                Case Else
                    Throw New Exception("Enum type not valid!")
            End Select
        End Function
    End Class

    'Public Class EnumToStringConverter9
    '    Implements IValueConverter

    '    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    '        Select Case value
    '            Case DetectorWindowType.hann
    '                Return "Hann"
    '            Case DetectorWindowType.bartlett
    '                Return "Bartlett"
    '            Case DetectorWindowType.blackman
    '                Return "Blackman"
    '            Case DetectorWindowType.hamming
    '                Return "Hamming"
    '            Case DetectorWindowType.rectwin
    '                Return "Rectangular"
    '            Case Else
    '                Throw New Exception("Detector window type not valid!")
    '        End Select
    '    End Function

    '    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    '        Select Case value
    '            Case "Hann"
    '                Return DetectorWindowType.hann
    '            Case "Bartlett"
    '                Return DetectorWindowType.bartlett
    '            Case "Blackman"
    '                Return DetectorWindowType.blackman
    '            Case "Hamming"
    '                Return DetectorWindowType.hamming
    '            Case "Rectangular"
    '                Return DetectorWindowType.rectwin
    '            Case Else
    '                Throw New Exception("Enum type not valid!")
    '        End Select
    '    End Function
    'End Class

    Public Class EnumToStringConverter10
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Select Case value
                Case OutOfRangeFrequencyDetectorType.AvergeWindow
                    Return "History for Baseline (seconds)"
                Case OutOfRangeFrequencyDetectorType.Nominal
                    Return "Nominal Value"
                Case Else
                    Throw New Exception("Out of range frequency detector type not valid!")
            End Select
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Select Case value
                Case "Nominal Value"
                    Return OutOfRangeFrequencyDetectorType.Nominal
                Case "History for Baseline (seconds)"
                    Return OutOfRangeFrequencyDetectorType.AvergeWindow
                Case Else
                    Throw New Exception("Enum type not valid!")
            End Select
        End Function
    End Class
End Namespace
