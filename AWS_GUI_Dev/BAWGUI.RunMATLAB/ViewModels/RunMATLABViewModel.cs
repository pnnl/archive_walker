using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using System.Threading;
using System.IO;

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
            PauseArchiveWalkerNormal = new RelayCommand(_pauseArchiveWalkerNormal);
            ResumeArchiveWalkerNormal = new RelayCommand(_resumeArchiveWalkerNormal);
            StopArchiveWalkerNormal = new RelayCommand(_stopArchiveWalkerNormal);
            BrowseResultsStorage = new RelayCommand(_browseResultsStorage);
            _engine = MatLabEngine.Instance;
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

        string RunPath = @"C:\Users\wang690\Desktop\projects\ArchiveWalker\RerunTest\RerunTestRD\";
        public ICommand RunArchiveWalkerNormal { get; set; }
        private void _runAWNormal(object obj)
        {
            if (File.Exists(ConfigFileName))
            {
                var controlPath = RunPath + "ControlRun\\";
                System.IO.Directory.CreateDirectory(controlPath);
                try
                {
                    Engine.RuNormalModeByBackgroundWorker(controlPath, ConfigFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Config file does not exist. Please specify a valid config file.", "Error!", MessageBoxButtons.OK);
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
                _initialConfigFilePath = Path.GetDirectoryName(ConfigFileName); ;
            }
        }

        public ICommand PauseArchiveWalkerNormal { get; set; }
        public ICommand ResumeArchiveWalkerNormal { get; set; }
        public ICommand StopArchiveWalkerNormal { get; set; }
        private void _stopArchiveWalkerNormal(object obj)
        {
            try
            {
                _engine.StopMatlabNormalRun();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in stop matlab normal run: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }
        }
        private void _pauseArchiveWalkerNormal(object obj)
        {
            _engine.PauseMatlabNormalRun();
        }
        private void _resumeArchiveWalkerNormal(object obj)
        {
            var pauseFlag = RunPath + "ControlRun\\PauseFlag.txt";
            File.Delete(pauseFlag);
            _runAWNormal(obj);
            //_engine.IsNormalRunPaused = false;
        }


        private ProjectsControlViewModel _projectControl = new ProjectsControlViewModel();
        public ProjectsControlViewModel ProjectControl
        {
            get { return _projectControl; }
            set
            {
                _projectControl = value;
                OnPropertyChanged();
            }
        }
        private string _resultsStoragePath;
        public string ResultsStoragePath
        {
            get { return _resultsStoragePath; }
            set
            {
                _resultsStoragePath = value;
                OnPropertyChanged();
            }
        }
        public ICommand BrowseResultsStorage { get; set; }
        private void _browseResultsStorage(object obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ResultsStoragePath = fbd.SelectedPath;
                    if (Directory.Exists(ResultsStoragePath))
                    {
                        ProjectControl = new ProjectsControlViewModel(ResultsStoragePath);
                        ProjectControl.ProjectSelected += OnProjectSelected;
                    }
                    //string[] files = Directory.GetFiles(ResultsStoragePath);
                }
            }
        }

        private void OnProjectSelected(object sender, AWProjectViewModel e)
        {
            SelectedProject = e;
            SelectedRun = e.SelectedRun;
        }

        private AWProjectViewModel _selectedProject;
        public AWProjectViewModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
            }
        }
        private AWRunViewModel _selectedRun;
        public AWRunViewModel SelectedRun
        {
            get { return _selectedRun; }
            set
            {
                _selectedRun = value;
                OnPropertyChanged();
            }
        }
    }
}
