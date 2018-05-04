Imports System.Drawing
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media
Imports Resources

Public Class SpecifySignalTypeUnitCustomization
    Private Sub WatermarkTextBox_GotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMUnameBox" Then
                item.Background = Utility.Utility.HighlightColor
            End If
            If item.Name = "SignalNameBox" Then
                item.Background = Utility.Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub WatermarkTextBox_LostKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMUnameBox" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
            If item.Name = "SignalNameBox" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
        Next
    End Sub
End Class
