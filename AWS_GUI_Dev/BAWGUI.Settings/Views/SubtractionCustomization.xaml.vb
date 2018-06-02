Imports System.Drawing
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media
Imports BAWGUI.Core.Resources

Public Class SubtractionCustomization
    Private Sub MinuendOrDividentTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "MinuendOrDividend"
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "MinuendOrDividend" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub SubtrahendOrDivisorTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "SubtrahendOrDivisor"
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "SubtrahendOrDivisor" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub MinuendOrDividentTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        'sender.Background = New Media.SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
            If item.Name = "MinuendOrDividend" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
        Next
    End Sub
    Private Sub SubtrahendOrDivisorTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        'sender.Background = New Media.SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White)
            End If
            If item.Name = "SubtrahendOrDivisor" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
        Next
    End Sub

    'Private Sub StackPanel_LostFocus(sender As Object, e As RoutedEventArgs)
    '    For Each item In sender.Children
    '        If TypeOf item Is TextBox Then
    '            If item.Name = "MinuendOrDividend" Then
    '                If sender.datacontext.MinuendOrDividend.IsError Then
    '                    item.Background = Brushes.Red
    '                Else
    '                    item.Background = New Media.SolidColorBrush(Colors.White)
    '                End If
    '            End If
    '            If item.Name = "SubtrahendOrDivisor" Then
    '                If sender.datacontext.SubtrahendOrDivisor.IsError Then
    '                    item.Background = Brushes.Red
    '                Else
    '                    item.Background = New Media.SolidColorBrush(Colors.White)
    '                End If
    '            End If
    '        End If
    '    Next
    'End Sub
End Class
