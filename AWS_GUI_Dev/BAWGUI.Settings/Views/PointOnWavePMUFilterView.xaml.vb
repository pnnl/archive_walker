Imports System.Windows.Media
Imports BAWGUI.Utilities

Public Class PointOnWavePMUFilterView
    Private Sub PAPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PAChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PAPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PAChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PAChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PAPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PAChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PAPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PBPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PBChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PBPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PBChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PBChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PBPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PBChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PBPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PCPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PCChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PCPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PCChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PCChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PCPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PCChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PCPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
End Class
