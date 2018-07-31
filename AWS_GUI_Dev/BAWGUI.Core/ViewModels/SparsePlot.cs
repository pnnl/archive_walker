using BAWGUI.Core;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BAWGUI.Core
{
    public class SparsePlot:ViewModelBase
    {
        public SparsePlot()
        {
            _sparsePlotLegend = new List<string>();
            _sparsePlotModel = new ViewResolvingPlotModel();
        }
        private ViewResolvingPlotModel _sparsePlotModel;
        public ViewResolvingPlotModel SparsePlotModel
        {
            get { return _sparsePlotModel; }
            set
            {
                _sparsePlotModel = value;
                OnPropertyChanged();
            }
        }
        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged();
            }
        }
        private List<string> _sparsePlotLegend;
        public List<string> SparsePlotLegend
        {
            get { return _sparsePlotLegend; }
            set
            {
                _sparsePlotLegend = value;
                OnPropertyChanged();
            }
        }
    }
}
