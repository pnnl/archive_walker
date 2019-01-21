using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace BAWGUI.Results.ViewModels
{
    public class OBATPresetUpdateViewModel : ViewModelBase
    {
        private string _newPresetName;
        public string NewPresetName
        {
            get { return _newPresetName; }
            set
            {
                _newPresetName = value;
                OnPropertyChanged();
            }
        }
        private string _detectorName;
        public string DetectorName
        {
            get { return _detectorName; }
            set
            {
                _detectorName = value;
                OnPropertyChanged();
            }
        }
        private AWRunViewModel _run;
        public ICommand CancelUpdateOBATPreset { get; set; }
        private void _cancelUpdateOBATPreset(object obj)
        {
            OnUpdateOBATPresetCancelled();
        }
        public event EventHandler UpdateOBATPresetCancelled;
        protected virtual void OnUpdateOBATPresetCancelled()
        {
            UpdateOBATPresetCancelled?.Invoke(this, EventArgs.Empty);
        }
        private string _configFilePath;
        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                _configFilePath = value;
                OnPropertyChanged();
            }
        }
        public OBATPresetUpdateViewModel(string filename, string detectorType, AWRunViewModel run)
        {
            this._newPresetName = Path.GetFileNameWithoutExtension(filename);
            this._detectorName = detectorType;
            this._run = run;
            if (run != null)
            {
                ConfigFilePath = run.AWRunConfigPath;
            }
            _obatPresetFilePath = "";
            _previousOBATPresetFilePath = "";
            CancelUpdateOBATPreset = new RelayCommand(_cancelUpdateOBATPreset);
            SelectAWConfigFile = new RelayCommand(_selectAWConfigFile);
            SelectOBATPresetFile = new RelayCommand(_selectOBATPresetFile);
            UpdateOBATpreset = new RelayCommand(_updateOBATpreset);
            _engine = RunMATLAB.ViewModels.MatLabEngine.Instance;
        }
        private RunMATLAB.ViewModels.MatLabEngine _engine;
        public RunMATLAB.ViewModels.MatLabEngine Engine
        {
            get { return _engine; }
        }
        private string _obatPresetFilePath;
        public string OBATPresetFilePath
        {
            get { return _obatPresetFilePath; }
            set
            {
                _obatPresetFilePath = value;
                OnPropertyChanged();
            }
        }
        public ICommand SelectAWConfigFile { get; set; }
        private void _selectAWConfigFile(object obj)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Please select the Archive Walker .xml configure file";
                if (_run != null && Directory.Exists(_run.Model.RunPath))
                {
                    openFileDialog.InitialDirectory = _run.Model.RunPath;
                }
                else
                {
                    openFileDialog.InitialDirectory = Environment.CurrentDirectory;
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    ConfigFilePath = openFileDialog.FileName;
                }
            }
        }
        public ICommand SelectOBATPresetFile { get; set; }
        private void _selectOBATPresetFile(object obj)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Please select the OBAT Preset.xml file";
                if (Directory.Exists(_previousOBATPresetFilePath))
                {
                    openFileDialog.InitialDirectory = _previousOBATPresetFilePath;
                }
                else
                {
                    openFileDialog.InitialDirectory = Environment.CurrentDirectory;
                }
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    OBATPresetFilePath = openFileDialog.FileName;
                    _previousOBATPresetFilePath = Path.GetDirectoryName(OBATPresetFilePath);
                }
            }
        }
        private string _previousOBATPresetFilePath;
        public ICommand UpdateOBATpreset { get; set; }
        private void _updateOBATpreset(object obj)
        {
            //close the popup first
            UpdateOBATPresetCancelled?.Invoke(this, EventArgs.Empty);
            //call engine to do the update
            _engine.UpdateOBATPreset(NewPresetName, DetectorName, ConfigFilePath, OBATPresetFilePath);
        }
    }
}
