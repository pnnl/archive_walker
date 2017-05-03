Public Class PowerCalculationsCustomization
    Private Sub VTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "VmagPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "VangPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "VmagChannel" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "VangChannel" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "VphasorPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "VphasorChannel" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub VTextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "VmagPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "VangPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "VmagChannel" Then
                item.Background = Brushes.White
            End If
            If item.Name = "VangChannel" Then
                item.Background = Brushes.White
            End If
            If item.Name = "VphasorPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "VphasorChannel" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
    Private Sub ITextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "ImagPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "IangPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "ImagChannel" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "IangChannel" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "IphasorPMU" Then
                item.Background = Application.HighlightColor
            End If
            If item.Name = "IphasorChannel" Then
                item.Background = Application.HighlightColor
            End If
        Next
    End Sub

    Private Sub ITextBoxLostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "ImagPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "IangPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "ImagChannel" Then
                item.Background = Brushes.White
            End If
            If item.Name = "IangChannel" Then
                item.Background = Brushes.White
            End If
            If item.Name = "IphasorPMU" Then
                item.Background = Brushes.White
            End If
            If item.Name = "IphasorChannel" Then
                item.Background = Brushes.White
            End If
        Next
    End Sub
End Class
