using BAWGUI.Core.Views;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace ModeMeter.Views
{
    /// <summary>
    /// Interaction logic for ModeSignalSelectionView.xaml
    /// </summary>
    public partial class ModeSignalSelectionView : UserControl
    {
        public ModeSignalSelectionView()
        {
            InitializeComponent();
        }

        //private void FilterListBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    var b = sender as FilterListBox;
        //    b.Background = Utility.HighlightColor;
        //    //var a = b.Parent as UserControl;
        //    //var cc = a.Parent as GroupBox;
        //    //cc.Background = Utility.HighlightColor;
        //    //foreach (var item in b.Parent)
        //    //{
        //    //    Console.WriteLine(item);
        //    //}
        //}

        //private void FilterListBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    var b = sender as FilterListBox;
        //    b.Background = new SolidColorBrush(Colors.White);
        //    //var a = b.Parent as UserControl;
        //    //var cc = a.Parent as GroupBox;
        //    //cc.Background = new SolidColorBrush(Colors.White);
        //    var c = b.Content as Grid;
        //    //var showText = false;
        //    foreach (var ch in c.Children)
        //    {
        //        //if (ch is ListView)
        //        //{
        //        //    var child = ch as ListView;
        //        //    if (child.Items.Count == 0)
        //        //    {
        //        //        //child.Visibility = Visibility.Collapsed;
        //        //        showText = true;
        //        //    }
        //        //}
        //        if (ch is TextBlock)
        //        {
        //            var child = ch as TextBlock;
        //            child.Foreground = new SolidColorBrush(Colors.Black);
        //            //if (showText)
        //            //{
        //            //    //child.Visibility = Visibility.Visible;
        //            //}
        //        }
        //    }

        //}

        //private void FilterListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    var b = sender as FilterListBox;
        //    var c = b.Content as Grid;
        //    foreach (var ch in c.Children)
        //    {
        //        if (ch is ListView)
        //        {
        //            var child = ch as ListView;
        //            child.Visibility = Visibility.Visible;
        //            child.Focus();
        //        }
        //        if (ch is TextBlock)
        //        {
        //            var child = ch as TextBlock;
        //            child.Foreground = new SolidColorBrush(Colors.Red);
        //            //child.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //}

        private void UnaryTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as TextBox;
            b.Background = Utility.HighlightColor;
            //foreach (var item in sender.Parent.Children)
            //{
            //    if (item.Name == "PMU")
            //        item.Background = Utility.HighlightColor;
            //    if (item.Name == "Channel")
            //        item.Background = Utility.HighlightColor;
            //    if (item.Name == "CustName")
            //        item.Background = Utility.HighlightColor;
            //}
        }

        private void UnaryTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as TextBox;
            b.Background = new SolidColorBrush(Colors.White);
            //foreach (var item in sender.Parent.Children)
            //{
            //    if (item.Name == "PMU")
            //        item.Background = new System.Media.SolidColorBrush(Colors.White);
            //    if (item.Name == "Channel")
            //        item.Background = new System.Media.SolidColorBrush(Colors.White);
            //    if (item.Name == "CustName")
            //        item.Background = new System.Media.SolidColorBrush(Colors.White);
            //}
        }


    }
}
