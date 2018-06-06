using System.Collections.ObjectModel;

namespace BAWGUI.Core
{
    public class SignalTypeHierachy : ViewModelBase
    {
        public SignalTypeHierachy()
        {
            _signalSignature = new SignalSignatureViewModel();
            _signalList = new ObservableCollection<SignalTypeHierachy>();
        }
        public SignalTypeHierachy(SignalSignatureViewModel signature)
        {
            _signalSignature = signature;
            _signalList = new ObservableCollection<SignalTypeHierachy>();
        }
        public SignalTypeHierachy(SignalSignatureViewModel signature, ObservableCollection<SignalTypeHierachy> list)
        {
            _signalSignature = signature;
            _signalList = list;
        }
        private SignalSignatureViewModel _signalSignature;
        public SignalSignatureViewModel SignalSignature
        {
            get
            {
                return _signalSignature;
            }
            set
            {
                _signalSignature = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _signalList;
        public ObservableCollection<SignalTypeHierachy> SignalList
        {
            get
            {
                return _signalList;
            }
            set
            {
                _signalList = value;
                OnPropertyChanged();
            }
        }
    }
}
