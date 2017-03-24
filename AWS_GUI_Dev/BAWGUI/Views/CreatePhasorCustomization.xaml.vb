Public Class CreatePhasorCustomization
    Private Sub CreatePhasorTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.Yellow
    End Sub

    Private Sub CreatePhasorTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.White
    End Sub
End Class
