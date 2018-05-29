﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace BAWGUI.ViewModels
{
    public static class ComboBoxItemsSourceDecorator
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
            "ItemsSource", typeof(IEnumerable), typeof(ComboBoxItemsSourceDecorator), new PropertyMetadata(null, ItemsSourcePropertyChanged)
        );

        public static void SetItemsSource(UIElement element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(UIElement element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        static void ItemsSourcePropertyChanged(DependencyObject element,
                        DependencyPropertyChangedEventArgs e)
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
}
