using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using ModeMeter.Models;
using System.Collections.ObjectModel;

namespace ModeMeter.ViewModels
{
    public class ModeViewModel : ViewModelBase
    {
        private Mode _model;

        public ModeViewModel(Mode mode, SignalManager _signalMgr)
        {
            this._model = mode;
            PMUs = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var signal in _model.PMUs)
            {

            }
        }
        public string Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SignalSignatureViewModel> PMUs { get; set; }
    }
}