using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace BAWGUI.Utilities
{
    public static class ComboBoxItemsSourceDecorator
    {
        public readonly static DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
            "ItemsSource", typeof(IEnumerable), typeof(ComboBoxItemsSourceDecorator), new PropertyMetadata(null, ItemsSourcePropertyChanged));

        public static void SetItemsSource(UIElement element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(UIElement element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        private static void ItemsSourcePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var target = element as Selector;
            if (element == null)
                return;
            // Save original binding 
            var originalBinding = BindingOperations.GetBinding(target, Selector.SelectedValueProperty);
            BindingOperations.ClearBinding(target, Selector.SelectedValueProperty);

            try
            {
                target.ItemsSource = e.NewValue as IEnumerable;
            }
            finally
            {
                if (originalBinding != null)
                    BindingOperations.SetBinding(target, Selector.SelectedValueProperty, originalBinding);
            }
        }
    }

    public class SelectorBehavior
    {
        public static bool GetKeepSelection(DependencyObject obj)
        {
            return System.Convert.ToBoolean(obj.GetValue(KeepSelectionProperty));
        }

        public static void SetKeepSelection(DependencyObject obj, bool value)
        {
            obj.SetValue(KeepSelectionProperty, value);
        }

        public static readonly DependencyProperty KeepSelectionProperty = DependencyProperty.RegisterAttached("KeepSelection", typeof(bool), typeof(SelectorBehavior), new UIPropertyMetadata(false, new PropertyChangedCallback(onKeepSelectionChanged)));

        private static void onKeepSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as Selector;
            var value = System.Convert.ToBoolean(e.NewValue);

            if (value)
                selector.SelectionChanged += selector_SelectionChanged;
            else
                selector.SelectionChanged -= selector_SelectionChanged;
        }

        private static void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as Selector;

            if (e.RemovedItems.Count > 0)
            {
                var deselectedItem = e.RemovedItems[0];

                if (selector.SelectedItem == null)
                {
                    selector.SelectedItem = deselectedItem;
                    e.Handled = true;
                }
            }
        }
    }

}


