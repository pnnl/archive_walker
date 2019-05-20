using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
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
                //var a = Enum.Parse(typeof(DataFileType), _model.FileType);
                return _model.FileType; }
            set
            {
                _model.FileType = (DataFileType)value;
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
                if (!string.IsNullOrEmpty(value))
                {
                    if (File.Exists(value) && Model.CheckDataFileMatch())
                    {
                        var filename = "";
                        try
                        {
                            filename = Path.GetFileNameWithoutExtension(value);
                        }
                        catch (ArgumentException ex)
                        {
                            MessageBox.Show("Data file path contains one or more of the invalid characters. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                        if (FileType == DataFileType.PI || FileType == DataFileType.OpenHistorian)
                        {
                            Mnemonic = "";
                            //this try block need to stay so the change would show up in the GUI, even though it's duplicating the work in DataConfigModel.cs tryi block on line 268 to 279.
                            try
                            {
                                FileDirectory = Path.GetDirectoryName(value);
                                var type = Path.GetExtension(value);
                                if (type == ".xml")
                                {
                                    PresetList = _model.GetPresets(value);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
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
                    }
                    else
                    {
                        // MessageBox.Show("Example input data file does not exist!", "Warning!", MessageBoxButtons.OK);
                        MessageBox.Show("The example file  " + Path.GetFileName(value) + "  could not be found in the directory  " + Path.GetDirectoryName(value) + ".\n"
                                        + "Please go to the 'Data Source' tab, update the location of the example file, and click the 'Read File' button.", "Warning!", MessageBoxButtons.OK);

                    }
                }
                OnPropertyChanged();
            }
        }
        public List<string> SignalList { get; internal set; }
        public ObservableCollection<SignalSignatureViewModel> TaggedSignals { get; internal set; }
        public int SamplingRate { get; internal set; }
        public bool IsExpanded { get; set; }
        private List<string> _presetList;
        public List<string> PresetList
        {
            set { _presetList = value;
                OnPropertyChanged();
            }
            get { return _presetList; }
        }
        //public List<string> GetPresets(string filename)
        //{
        //    var newPresets = new List<string>();
        //    var doc = XDocument.Load(filename);
        //    var presets = doc.Element("Presets");
        //    if (presets != null)
        //    {
        //        var pts = presets.Elements("Preset");
        //        if (pts != null)
        //        {
        //            foreach (var item in pts)
        //            {
        //                if (item.HasAttributes)
        //                {
        //                    var nm = item.Attribute("name");
        //                    if (nm != null)
        //                    {
        //                        newPresets.Add(nm.Value.ToString());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return newPresets;
        //}
    }
}
