using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.ReadConfigXml;
using BAWGUI.Utilities;

namespace BAWGUI.SignalManagement.ViewModels
{
    public class InputFileInfoViewModel:ViewModelBase
    {
        private InputFileInfo _model;

        public InputFileInfoViewModel(InputFileInfo model)
        {
            this._model = model;
            SignalList = new List<string>();
            TaggedSignals = new ObservableCollection<SignalSignatureViewModel>();
        }
        public string FileDirectory { get { return _model.FileDirectory; } }
        public string FileType { get { return _model.FileType; } }
        public string Mnemonic { get { return _model.Mnemonic; } }

        public List<string> SignalList { get; internal set; }
        public ObservableCollection<SignalSignatureViewModel> TaggedSignals { get; internal set; }
        public int SamplingRate { get; internal set; }
    }
}
