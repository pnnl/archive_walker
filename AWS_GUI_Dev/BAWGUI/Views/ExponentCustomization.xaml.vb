Public Class ExponentCustomization
    Private Sub ExpTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.Yellow
    End Sub
    Private Sub ExpTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.White
    End Sub
End Class
