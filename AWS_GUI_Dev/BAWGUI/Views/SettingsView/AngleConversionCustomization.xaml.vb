Public Class AngleConversionCustomization
    Private Sub ExpTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "Channel" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub
    Private Sub ExpTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "Channel" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
End Class
