Public Class MetricPrefixCustomization
    Private Sub ExpTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        'For Each child In sender.Child
        '    child.Background = Brushes.Yellow
        'Next
        sender.Background = Brushes.Cyan
        'For Each item In sender.Parent.Children
        '    If item.Name = "PMU" Then
        '        item.Background = Brushes.Yellow
        '    End If
        '    If item.Name = "Channel" Then
        '        item.Background = Brushes.Yellow
        '    End If
        'Next
    End Sub
    Private Sub ExpTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        'For Each child In sender.Child
        '    child.Background = Brushes.LightGray
        'Next
        sender.Background = Brushes.LightGray
        'For Each item In sender.Parent.Children
        '    If item.Name = "PMU" Then
        '        item.Background = Brushes.White
        '    End If
        '    If item.Name = "Channel" Then
        '        item.Background = Brushes.White
        '    End If
        'Next
    End Sub

    Private Sub StackPanel_LostFocus(sender As Object, e As RoutedEventArgs)
        For Each child In sender.Children
            child.Background = Brushes.White
        Next
        sender.Background = Brushes.White
    End Sub

    Private Sub StackPanel_GotFocus(sender As Object, e As RoutedEventArgs)
        For Each child In sender.Children
            child.Background = Application.HighlightColor
        Next
        sender.Background = Application.HighlightColor
    End Sub

    Private Sub PMU_GotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "Channel" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub Channel_GotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "Channel" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub PMU_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        For Each item In sender.Parent.Children
            'If item.Name = "PMU" Then
            item.Background = Application.HighlightColor
            'End If
            'If item.Name = "Channel" Then
            '    item.Background = Brushes.Yellow
            'End If
        Next
    End Sub

    Private Sub Channel_MouseDown(sender As Object, e As MouseButtonEventArgs)
        For Each item In sender.Parent.Children
            'If item.Name = "PMU" Then
            item.Background = Application.HighlightColor
            'End If
            'If item.Name = "Channel" Then
            '    item.Background = Brushes.Yellow
            'End If
        Next
    End Sub

    Private Sub PMU_GotFocus_1(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = Application.HighlightColor
        Next
    End Sub

    Private Sub Channel_GotFocus_1(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = Application.HighlightColor
        Next
    End Sub

    Private Sub PMU_LostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = Brushes.White
        Next
    End Sub

    Private Sub Channel_LostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = Brushes.White
        Next
    End Sub
End Class
