using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

namespace BAWGUI.RunMATLAB.ViewModels
{
    public class RunMATLABViewModel : ViewModelBase
    {
        public RunMATLABViewModel()
        {
            _configFileName = "";
            _initialConfigFilePath = "";
            OpenConfigFile = new RelayCommand(_openConfigFile);
            RunArchiveWalkerNormal = new RelayCommand(_runAWNormal);
            _engine = MatLabEngine.Instance;
            //_isNormalModeRunning = false;
        }
        private string _configFileName;
        public string ConfigFileName
        {
            get { return _configFileName; }
            set
            {
                _configFileName = value;
                OnPropertyChanged();
            }
        }
        private string _initialConfigFilePath;
        private MatLabEngine _engine;
        public MatLabEngine Engine
        {
            get { return _engine; }
        }
        //private bool _isNormalModeRunning;
        public bool IsNormalModeRunning
        {
            get { return Engine.IsMatlabEngineRunning; }
            set
            {
                Engine.IsMatlabEngineRunning = value;
                OnPropertyChanged();
            }
        }
        public ICommand RunArchiveWalkerNormal { get; set; }
        private void _runAWNormal(object obj)
        {
            try
            {
                IsNormalModeRunning = true;
                var controlPath = @"C:\Users\wang690\Desktop\projects\ArchiveWalker\RerunTest\";
                var runFlag = controlPath + "RunFlag.txt";
                if (!System.IO.File.Exists(runFlag))
                {
                    System.IO.FileStream fs = System.IO.File.Create(runFlag);
                    fs.Close();
                }
                System.Threading.Thread t1 = new System.Threading.Thread(() => { _engine.RunNormalMode(controlPath, ConfigFileName); });
                t1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK);
            }  
        }
        public ICommand OpenConfigFile { get; set; }
        private void _openConfigFile(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Please select a .xml configure file";
            openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (string.IsNullOrEmpty(_initialConfigFilePath))
            {
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            }
            else
            {
                openFileDialog.InitialDirectory = _initialConfigFilePath;
            }
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ConfigFileName = openFileDialog.FileName;
                _initialConfigFilePath = openFileDialog.InitialDirectory;
            }
        }
        //public void RingDownRerun(string start, string end, string configFile)
        //{

        //}
    }
}
