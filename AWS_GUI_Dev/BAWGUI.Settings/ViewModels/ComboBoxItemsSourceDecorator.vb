Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data


Module ComboBoxItemsSourceDecorator
    Public ReadOnly ItemsSourceProperty As DependencyProperty = DependencyProperty.RegisterAttached("ItemsSource", GetType(IEnumerable), GetType(ComboBoxItemsSourceDecorator), New PropertyMetadata(Nothing, AddressOf ItemsSourcePropertyChanged))

    Sub SetItemsSource(ByVal element As UIElement, ByVal value As IEnumerable)
        element.SetValue(ItemsSourceProperty, value)
    End Sub

    Function GetItemsSource(ByVal element As UIElement) As IEnumerable
        Return CType(element.GetValue(ItemsSourceProperty), IEnumerable)
    End Function

    Private Sub ItemsSourcePropertyChanged(ByVal element As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim target = TryCast(element, Selector)
        If element Is Nothing Then Return
        Dim originalBinding = BindingOperations.GetBinding(target, Selector.SelectedValueProperty)
        BindingOperations.ClearBinding(target, Selector.SelectedValueProperty)

        Try
            target.ItemsSource = TryCast(e.NewValue, IEnumerable)
        Finally
            If originalBinding IsNot Nothing Then BindingOperations.SetBinding(target, Selector.SelectedValueProperty, originalBinding)
        End Try
    End Sub
End Module

Public Class SelectorBehavior
    Public Shared Function GetKeepSelection(ByVal obj As DependencyObject) As Boolean
        Return CBool(obj.GetValue(KeepSelectionProperty))
    End Function

    Public Shared Sub SetKeepSelection(ByVal obj As DependencyObject, ByVal value As Boolean)
        obj.SetValue(KeepSelectionProperty, value)
    End Sub

    Public Shared ReadOnly KeepSelectionProperty As DependencyProperty = DependencyProperty.RegisterAttached("KeepSelection", GetType(Boolean), GetType(SelectorBehavior), New UIPropertyMetadata(False, New PropertyChangedCallback(AddressOf onKeepSelectionChanged)))

    Private Shared Sub onKeepSelectionChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim selector = TryCast(d, Selector)
        Dim value = CBool(e.NewValue)

        If value Then
            AddHandler selector.SelectionChanged, AddressOf selector_SelectionChanged
        Else
            RemoveHandler selector.SelectionChanged, AddressOf selector_SelectionChanged
        End If
    End Sub

    Private Shared Sub selector_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim selector = TryCast(sender, Selector)

        If e.RemovedItems.Count > 0 Then
            Dim deselectedItem = e.RemovedItems(0)

            If selector.SelectedItem Is Nothing Then
                selector.SelectedItem = deselectedItem
                e.Handled = True
            End If
        End If
    End Sub
End Class


