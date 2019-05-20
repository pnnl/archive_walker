using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
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

namespace BAWGUI.SignalManagement.Views
{
    /// <summary>
    /// Interaction logic for SignalSelectionPanel.xaml
    /// </summary>
    public partial class SignalSelectionPanel : UserControl
    {
        public SignalSelectionPanel()
        {
            InitializeComponent();
        }

        //private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    var dc = sender as TreeView;
        //    var sltd = e.NewValue as SignalTypeHierachy;
        //    var smgr = dc.DataContext as SignalManager;
        //    if (sltd != null && smgr != null && sltd.SignalList.Count == 0)
        //    {
        //        smgr.SelectedSignalToBeViewed = sltd.SignalSignature;
        //    }
        //}
    }
}
