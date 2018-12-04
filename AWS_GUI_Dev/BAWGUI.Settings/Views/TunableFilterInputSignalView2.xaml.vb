Imports BAWGUI.Utilities
Imports System.Windows.Media

Public Class TunableFilterInputSignalView2
    Private Sub PAVPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PAVChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PAVPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PAVChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PAVChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PAVPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PAVChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PAVPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PBVPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PBVChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PBVPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PBVChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PBVChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PBVPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PBVChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PBVPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PCVPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PCVChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PCVPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PCVChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PCVChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PCVPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PCVChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PCVPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PAIPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PAIChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PAIPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PAIChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PAIChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PAIPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PAIChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PAIPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PBIPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PBIChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PBIPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PBIChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PBIChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PBIPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PBIChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PBIPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
    Private Sub PCIPMU_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PCIChannel" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PCIPMU_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PCIChannel" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub

    Private Sub PCIChannel_GotFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = Utility.HighlightColor
        For Each item In sender.Parent.Children
            If item.Name = "PCIPMU" Then
                item.Background = Utility.HighlightColor
                Exit For
            End If
        Next
    End Sub

    Private Sub PCIChannel_LostFocus(sender As Object, e As Windows.RoutedEventArgs)
        sender.Background = New SolidColorBrush(Colors.White)
        For Each item In sender.Parent.Children
            If item.Name = "PCIPMU" Then
                item.Background = New SolidColorBrush(Colors.White)
                Exit For
            End If
        Next
    End Sub
End Class
