using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using ModeMeter.Models;

namespace ModeMeter.ViewModels
{
    public class SmallSignalStabilityToolViewModel:DetectorBase
    {
        private SmallSignalStabilityTool _model;
        private SignalManager _signalMgr;

        public SmallSignalStabilityToolViewModel(SignalManager signalMgr)
        {
            _model = new SmallSignalStabilityTool();
            // need to set up result path if not exists
            _signalMgr = signalMgr;
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
        }

        public SmallSignalStabilityToolViewModel(SmallSignalStabilityTool model, SignalManager signalMgr)
        {
            this._model = model;
            _signalMgr = signalMgr;
            InputChannels = new ObservableCollection<SignalSignatureViewModel>();
        }
        public override string Name
        {
            get { return "Small Signal Stability Tool"; }
        }
        public string ModeMeterName
        {
            get { return _model.ModeMeterName; }
            set
            {
                _model.ModeMeterName = value;
                OnPropertyChanged();
            }
        }
    }
}
