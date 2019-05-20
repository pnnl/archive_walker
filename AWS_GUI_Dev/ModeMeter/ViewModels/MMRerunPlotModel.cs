using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeMeter.ViewModels
{
    public class MMRerunPlotModel : ViewModelBase
    {
        private ObservableCollection<Legend> _mmReRunPlotLegend;
        public ObservableCollection<Legend> MMReRunPlotLegend
        {
            get { return _mmReRunPlotLegend; }
            set
            {
                _mmReRunPlotLegend = value;
                OnPropertyChanged();
            }
        }
        private ViewResolvingPlotModel _mmReRunSignalPlotModel;
        public ViewResolvingPlotModel MMReRunAllSignalsPlotModel
        {
            get { return _mmReRunSignalPlotModel; }
            set
            {
                _mmReRunSignalPlotModel = value;
                OnPropertyChanged();
            }
        }
    }
}
