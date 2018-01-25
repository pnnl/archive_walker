using BAWGUI.RunMATLAB.ViewModels;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class SparsePlot:ViewModelBase
    {
        private PlotModel _sparsePlotModel;
        public PlotModel SparsePlotModel
        {
            get { return _sparsePlotModel; }
            set
            {
                _sparsePlotModel = value;
                OnPropertyChanged();
            }
        }
    }
}
