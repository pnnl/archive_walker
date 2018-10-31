using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BAWGUI.Core;
using BAWGUI.Core.Models;
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
            _model = model;
            SignalList = new List<string>();
            TaggedSignals = new ObservableCollection<SignalSignatureViewModel>();
        }
        //private string _fileDirectory;
        public string FileDirectory
        {
            get { return _model.FileDirectory; }
            set
            {
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
                if (File.Exists(value))
                {
                    //try
                    //{
                        var ftyp = Path.GetExtension(value).Substring(1);
                        switch (ftyp.ToLower())
                        {
                            case "pdat":
                                FileType = DataFileType.pdat;
                                break;
                            case "csv":
                                FileType = DataFileType.csv;
                                break;
                            case "mat":
                                FileType = DataFileType.powHQ;
                                break;
                            default:
                                MessageBox.Show("Data file type " + ftyp + " not recognized.", "Error!", MessageBoxButtons.OK);
                            break;
                        }
                        //FileType = (DataFileType)Enum.Parse(typeof(DataFileType), );
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Data file type not recognized. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    //}
                    var filename = "";
                    try
                    {
                        filename = Path.GetFileNameWithoutExtension(value);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show("Data file path contains one or more of the invalid characters. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                    try
                    {
                        Mnemonic = filename.Substring(0, filename.Length - 16);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error extracting Mnemonic from selected data file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                    //this try block need to stay so the change would show up in the GUI, even though it's duplicating the work in DataConfigModel.cs tryi block on line 268 to 279.
                    try
                    {
                        var fullPath = Path.GetDirectoryName(value);
                        var oneLevelUp = fullPath.Substring(0, fullPath.LastIndexOf(@"\"));
                        var twoLevelUp = oneLevelUp.Substring(0, oneLevelUp.LastIndexOf(@"\"));
                        FileDirectory = twoLevelUp;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    // MessageBox.Show("Example input data file does not exist!", "Warning!", MessageBoxButtons.OK);
                    MessageBox.Show("The example file  " + Path.GetFileName(value) + "  could not be found in the directory  " + Path.GetDirectoryName(value) + ".\n" 
                                    + "Please go to the 'Data Source' tab, update the location of the example file, and click the 'Read File' button.", "Warning!", MessageBoxButtons.OK);

                }
                OnPropertyChanged();
            }
        }
        public List<string> SignalList { get; internal set; }
        public ObservableCollection<SignalSignatureViewModel> TaggedSignals { get; internal set; }
        public int SamplingRate { get; internal set; }
        public bool IsExpanded { get; set; }
    }
}
