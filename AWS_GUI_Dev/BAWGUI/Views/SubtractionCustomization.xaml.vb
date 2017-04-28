Public Class SubtractionCustomization
    Private Sub MinuendOrDividentTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "MinuendOrDividend"
        'sender.Background = Brushes.Yellow
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = Brushes.Yellow
            End If
            If item.Name = "MinuendOrDividend" Then
                item.Background = Brushes.Yellow
            End If
        Next
    End Sub

    Private Sub SubtrahendOrDivisorTextBoxGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        sender.datacontext.CurrentCursor = "SubtrahendOrDivisor"
        'sender.Background = Brushes.Yellow
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = Brushes.Yellow
            End If
            If item.Name = "SubtrahendOrDivisor" Then
                item.Background = Brushes.Yellow
            End If
        Next
    End Sub

    Private Sub MinuendOrDividentTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        'sender.Background = Brushes.White
        For Each item In sender.Parent.Children
            If item.Name = "MDPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "MinuendOrDividend" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
    Private Sub SubtrahendOrDivisorTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        'sender.Background = Brushes.White
        For Each item In sender.Parent.Children
            If item.Name = "SDPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "SubtrahendOrDivisor" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub

    'Private Sub StackPanel_LostFocus(sender As Object, e As RoutedEventArgs)
    '    For Each item In sender.Children
    '        If TypeOf item Is TextBox Then
    '            If item.Name = "MinuendOrDividend" Then
    '                If sender.datacontext.MinuendOrDividend.IsError Then
    '                    item.Background = Brushes.Red
    '                Else
    '                    item.Background = Brushes.White
    '                End If
    '            End If
    '            If item.Name = "SubtrahendOrDivisor" Then
    '                If sender.datacontext.SubtrahendOrDivisor.IsError Then
    '                    item.Background = Brushes.Red
    '                Else
    '                    item.Background = Brushes.White
    '                End If
    '            End If
    '        End If
    '    Next
    'End Sub
End Class
