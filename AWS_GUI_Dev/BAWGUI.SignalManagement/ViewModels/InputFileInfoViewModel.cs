using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Core;
using BAWGUI.ReadConfigXml;
using BAWGUI.Utilities;

namespace BAWGUI.SignalManagement.ViewModels
{
    public class InputFileInfoViewModel:ViewModelBase
    {
        private InputFileInfoModel _model;
        public InputFileInfoModel Model
        {
            get { return _model; }
        }

        public InputFileInfoViewModel()
        {
            _model = new InputFileInfoModel();
            SignalList = new List<string>();
            TaggedSignals = new ObservableCollection<SignalSignatureViewModel>();
        }

        public InputFileInfoViewModel(InputFileInfoModel model)
        {
            this._model = model;
            SignalList = new List<string>();
            TaggedSignals = new ObservableCollection<SignalSignatureViewModel>();
        }
        public string FileDirectory
        {
            get { return _model.FileDirectory; }
            set {
                _model.FileDirectory = value;
                OnPropertyChanged();
            }
        }
        public DataFileType? FileType
        {
            get {
                var a = Enum.Parse(typeof(DataFileType), _model.FileType);
                return (DataFileType)a; }
            set
            {
                _model.FileType = value.ToString();
                OnPropertyChanged();
            }
        }
        public string Mnemonic
        {
            get { return _model.Mnemonic; }
            set
            {
                _model.Mnemonic = value;
                OnPropertyChanged();
            }
        }
        public string ExampleFile
        {
            get { return _model.ExampleFile; }
            set
            {
                _model.ExampleFile = value;
                OnPropertyChanged();
            }
        }
        public List<string> SignalList { get; internal set; }
        public ObservableCollection<SignalSignatureViewModel> TaggedSignals { get; internal set; }
        public int SamplingRate { get; internal set; }
        public bool IsExpanded { get; set; }
    }
}
