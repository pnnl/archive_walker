﻿Imports System.Drawing
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media
Imports BAWGUI.Utilities

Public Class DivisionCustomization

    Private Sub MinuendOrDividentTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "Dividend"
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "Dividend" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub SubtrahendOrDivisorTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "Divisor"
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "Divisor" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub MinuendOrDividentTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
            If item.Name = "Dividend" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
        Next
    End Sub
    Private Sub SubtrahendOrDivisorTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
            If item.Name = "Divisor" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
        Next
    End Sub
End Class
