Public Class DivisionCustomization
    Private Sub MinuendOrDividentTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "MinuendOrDivident"
        sender.Background = Brushes.Yellow
    End Sub

    Private Sub SubtrahendOrDivisorTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "SubtrahendOrDivisor"
        sender.Background = Brushes.Yellow
    End Sub

    Private Sub MinuendOrDividentTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.White
        'If sender.datacontext.MinuendOrDivident IsNot Nothing AndAlso sender.datacontext.MinuendOrDivident.IsValid Then
        '    sender.BorderBrush = Brushes.White
        'Else
        '    sender.BorderBrush = Brushes.Red
        'End If
    End Sub
    Private Sub SubtrahendOrDivisorTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        sender.Background = Brushes.White
        'If sender.datacontext.SubtrahendOrDivisor IsNot Nothing AndAlso sender.datacontext.SubtrahendOrDivisor.IsValid Then
        '    sender.BorderBrush = Brushes.White
        'Else
        '    sender.BorderBrush = Brushes.Red
        'End If
    End Sub
End Class
