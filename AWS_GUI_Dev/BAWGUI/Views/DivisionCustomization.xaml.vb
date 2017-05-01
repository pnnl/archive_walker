﻿Public Class DivisionCustomization

    Private Sub MinuendOrDividentTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "MinuendOrDividend"
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "MinuendOrDividend" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub SubtrahendOrDivisorTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "SubtrahendOrDivisor"
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "SubtrahendOrDivisor" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub MinuendOrDividentTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "MinuendOrDividend" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
    Private Sub SubtrahendOrDivisorTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "SubtrahendOrDivisor" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
End Class
