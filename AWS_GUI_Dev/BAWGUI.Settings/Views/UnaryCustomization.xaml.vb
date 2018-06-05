Imports System.Drawing
Imports System.Windows
Imports System.Windows.Media
Imports BAWGUI.Core.Resources

Public Class UnaryCustomization
    Private Sub UnaryTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "Channel" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub UnaryTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
            If item.Name = "Channel" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
        Next
    End Sub
End Class
