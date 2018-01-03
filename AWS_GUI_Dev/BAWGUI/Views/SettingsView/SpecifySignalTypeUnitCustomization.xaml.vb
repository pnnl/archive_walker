Public Class SpecifySignalTypeUnitCustomization
    Private Sub WatermarkTextBox_GotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMUnameBox" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "SignalNameBox" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub WatermarkTextBox_LostKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMUnameBox" Then
                item.Background = Brushes.White
            End If
            If item.Name = "SignalNameBox" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
End Class
