using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace BAWGUI.Utilities
{
    public class GridColumnWidthReseterBehaviour : Behavior<Expander>
    {
        private Grid _parentGrid;
        private double _oldWidth;
        //public int TargetGridRowIndex { get; set; }
        public int TargetGridColumnIndex { get; set; }
        protected override void OnAttached()
        {
            AssociatedObject.Expanded += AssociatedObject_Expanded;
            AssociatedObject.Collapsed += AssociatedObject_Collapsed;
            FindParentGrid();
        }

        private void FindParentGrid()
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(AssociatedObject);
            while (parent != null)
            {
                if (parent is Grid)
                {
                    _parentGrid = parent as Grid;
                    return;
                }
                parent = LogicalTreeHelper.GetParent(AssociatedObject);
            }
        }

        void AssociatedObject_Collapsed(object sender, System.Windows.RoutedEventArgs e)
        {
            //_parentGrid.RowDefinitions[TargetGridRowIndex].Height = GridLength.Auto;
            _oldWidth = _parentGrid.ColumnDefinitions[TargetGridColumnIndex].Width.Value;
            _parentGrid.ColumnDefinitions[TargetGridColumnIndex].Width = GridLength.Auto;
        }

        void AssociatedObject_Expanded(object sender, System.Windows.RoutedEventArgs e)
        {
            //_parentGrid.RowDefinitions[TargetGridRowIndex].Height = GridLength.Auto;
            //_parentGrid.ColumnDefinitions[TargetGridColumnIndex].Width = GridLength.Auto;
            var currentWidth = _parentGrid.ColumnDefinitions[TargetGridColumnIndex].Width.Value;
            if (currentWidth <= 1d)
            {
                if (_oldWidth <= 1d)
                {
                    _parentGrid.ColumnDefinitions[TargetGridColumnIndex].Width = GridLength.Auto;
                }
                else
                {
                    _parentGrid.ColumnDefinitions[TargetGridColumnIndex].Width = new GridLength(_oldWidth, GridUnitType.Pixel);
                }
            }
            else
            {
                _parentGrid.ColumnDefinitions[TargetGridColumnIndex].Width = new GridLength(currentWidth, GridUnitType.Pixel);
            }
        }
    }
}
