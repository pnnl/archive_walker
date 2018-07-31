Imports System.Drawing
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media
Imports BAWGUI.Utilities

Public Class MetricPrefixCustomization
    Private Sub ExpTextBoxGotFocus(sender As Object, e As RoutedEventArgs)
        'For Each child In sender.Child
        '    child.Background = Brushes.Yellow
        'Next
        sender.Background = Utility.HighlightColor
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
        sender.Background = New Media.SolidColorBrush(Colors.White)
        'For Each item In sender.Parent.Children
        '    If item.Name = "PMU" Then
        '        item.Background = New Media.SolidColorBrush(Colors.White)
        '    End If
        '    If item.Name = "Channel" Then
        '        item.Background = New Media.SolidColorBrush(Colors.White)
        '    End If
        'Next
    End Sub

    Private Sub StackPanel_LostFocus(sender As Object, e As RoutedEventArgs)
        For Each child In sender.Children
            child.Background = New Media.SolidColorBrush(Colors.White)
        Next
        sender.Background = New Media.SolidColorBrush(Colors.White)
    End Sub

    Private Sub StackPanel_GotFocus(sender As Object, e As RoutedEventArgs)
        For Each child In sender.Children
            child.Background = Utility.HighlightColor
        Next
        sender.Background = Utility.HighlightColor
    End Sub

    Private Sub PMU_GotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "Channel" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub Channel_GotFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            If item.Name = "PMU" Then
                item.Background = Utility.HighlightColor
            End If
            If item.Name = "Channel" Then
                item.Background = Utility.HighlightColor
            End If
        Next
    End Sub

    Private Sub PMU_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        For Each item In sender.Parent.Children
            'If item.Name = "PMU" Then
            item.Background = Utility.HighlightColor
            'End If
            'If item.Name = "Channel" Then
            '    item.Background = Brushes.Yellow
            'End If
        Next
    End Sub

    Private Sub Channel_MouseDown(sender As Object, e As MouseButtonEventArgs)
        For Each item In sender.Parent.Children
            'If item.Name = "PMU" Then
            item.Background = Utility.HighlightColor
            'End If
            'If item.Name = "Channel" Then
            '    item.Background = Brushes.Yellow
            'End If
        Next
    End Sub

    Private Sub PMU_GotFocus_1(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = Utility.HighlightColor
        Next
    End Sub

    Private Sub Channel_GotFocus_1(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = Utility.HighlightColor
        Next
    End Sub

    Private Sub PMU_LostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = New Media.SolidColorBrush(Colors.White)
        Next
    End Sub

    Private Sub Channel_LostFocus(sender As Object, e As RoutedEventArgs)
        For Each item In sender.Parent.Children
            item.Background = New Media.SolidColorBrush(Colors.White)
        Next
    End Sub
End Class
