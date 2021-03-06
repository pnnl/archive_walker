﻿using BAWGUI.Utilities;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModeMeter.Views
{
    /// <summary>
    /// Interaction logic for ModeAnalysisParametersView.xaml
    /// </summary>
    public partial class ModeAnalysisParametersView : UserControl
    {
        public ModeAnalysisParametersView()
        {
            InitializeComponent();
        }

        private void GroupBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as Border;
            var a = b.Parent as Grid;
            var c = a.Parent as GroupBox;
            c.Background = Utility.HighlightColor;
        }

        private void GroupBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as Border;
            var a = b.Parent as Grid;
            var c = a.Parent as GroupBox;
            c.Background = new SolidColorBrush(Colors.White);
        }

        private void GroupBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var c = sender as GroupBox;
        }
    }
}
