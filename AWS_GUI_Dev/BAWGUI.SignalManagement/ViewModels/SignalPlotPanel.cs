using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.SignalManagement.ViewModels
{
    public class SignalPlotPanel:ViewModelBase
    {
        public SignalPlotPanel()
        {
            _signalViewPlotModel = new ViewResolvingPlotModel();
            Signals = new ObservableCollection<SignalSignatureViewModel>();
            _isPlotSelected = false;
        }
        private ViewResolvingPlotModel _signalViewPlotModel;
        public ViewResolvingPlotModel SignalViewPlotModel
        {
            get { return _signalViewPlotModel; }
            set
            {
                _signalViewPlotModel = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalSignatureViewModel> _signals;
        public ObservableCollection<SignalSignatureViewModel> Signals
        {
            get { return _signals; }
            set
            {
                _signals = value;
                OnPropertyChanged();
            }
        }
        private bool _isPlotSelected;
        public bool IsPlotSelected
        {
            get { return _isPlotSelected; }
            set
            {
                _isPlotSelected = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Legend> _legends;
        public ObservableCollection<Legend> Legends
        {
            get { return _legends; }
            set
            {
                _legends = value;
                OnPropertyChanged();
            }
        }
    }
}
