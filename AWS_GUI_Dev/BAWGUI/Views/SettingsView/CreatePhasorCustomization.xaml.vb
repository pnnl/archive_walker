Public Class CreatePhasorCustomization
    Private Sub CreatePhasorTextBoxGotFocusMag(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "Mag" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "MagPMU" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub CreatePhasorTextBoxLostFocusMag(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "Mag" Then
                item.Background = Brushes.White
            End If
        Next
        For Each item In sender.Parent.Children
            If item.Name = "MagPMU" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub

    Private Sub CreatePhasorTextBoxGotFocusAng(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "Ang" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "AngPMU" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub CreatePhasorTextBoxLostFocusAng(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "Ang" Then
                item.Background = Brushes.White
            End If
        Next
        For Each item In sender.Parent.Children
            If item.Name = "AngPMU" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
End Class
