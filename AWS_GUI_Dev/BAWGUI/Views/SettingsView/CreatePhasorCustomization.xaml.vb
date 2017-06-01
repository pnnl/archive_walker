Public Class CreatePhasorCustomization
    Private Sub CreatePhasorTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "Mag" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "Ang" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "MagPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "AngPMU" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub CreatePhasorTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "Mag" Then
                item.Background = Brushes.White
            End If
            If item.Name = "Ang" Then
                item.Background = Brushes.White
            End If
        Next
        For Each item In sender.Parent.Children
            If item.Name = "MagPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "AngPMU" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
End Class
