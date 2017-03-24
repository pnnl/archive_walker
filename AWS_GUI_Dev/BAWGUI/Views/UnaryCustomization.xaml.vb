Public Class UnaryCustomization
    Private Sub UnaryTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.Yellow
    End Sub

    Private Sub UnaryTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.White
    End Sub
End Class
