using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BAWGUI.Results.Views
{
    /// <summary>
    /// Interaction logic for EventTableView.xaml
    /// </summary>
    public partial class FOEventTableView : UserControl
    {
        public FOEventTableView()
        {
            InitializeComponent();
        }

        private void DataGridControl_SelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            var s = sender as Xceed.Wpf.DataGrid.DataGridControl;
            //Xceed.Wpf.DataGrid.DataRow rowOld = s.GetContainerFromItem(s.CurrentItem) as Xceed.Wpf.DataGrid.DataRow;
            //if (rowOld != null)
            //{
            //    rowOld.UpdateDefaultStyle();
            //}
            if (s.SelectedItem != null)
            {
                s.CurrentItem = s.SelectedItem;
            }
            //Xceed.Wpf.DataGrid.DataRow row = s.GetContainerFromItem(s.CurrentItem) as Xceed.Wpf.DataGrid.DataRow;
            //if (row != null)
            //{
            //    row.InactiveSelectionBackground = Brushes.Green;
            //    row.SelectionForeground = New Media.SolidColorBrush(Colors.White);
            //}
        }
    }
    //public partial class DraggablePopup : Popup
    //{
    //    public DraggablePopup()
    //    {
    //        var thumb = new Thumb
    //        {
    //            Width = 0,
    //            Height = 0,
    //        };
    //        ContentCanvas.Children.Add(thumb);

    //        MouseDown += (sender, e) =>
    //        {
    //            thumb.RaiseEvent(e);
    //        };

    //        thumb.DragDelta += (sender, e) =>
    //        {
    //            HorizontalOffset += e.HorizontalChange;
    //            VerticalOffset += e.VerticalChange;
    //        };
    //    }
    //}
}
