using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.ViewModels
{
    public static class FocusExtension
    {
        public static bool GetIsFocused(System.Windows.DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(System.Windows.DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly System.Windows.DependencyProperty IsFocusedProperty =
            System.Windows.DependencyProperty.RegisterAttached(
                "IsFocused", typeof(bool), typeof(FocusExtension),
                new System.Windows.UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        private static void OnIsFocusedPropertyChanged(
            System.Windows.DependencyObject d,
            System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var uie = (System.Windows.UIElement)d;
            if ((bool)e.NewValue)
            {
                uie.Focus(); // Don't care about false values.
            }
        }
    }
}
