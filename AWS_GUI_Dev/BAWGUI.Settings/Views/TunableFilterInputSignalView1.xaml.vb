Imports System.Windows
Imports System.Windows.Media
Imports BAWGUI.Utilities
Public Class TunableFilterInputSignalView1
    Private Sub TextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "Channel" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "CustName" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub TextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
            If item.Name = "Channel" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
            If item.Name = "CustName" Then
                item.Background = New Media.SolidColorBrush(Colors.White)
            End If
        Next
    End Sub
End Class
