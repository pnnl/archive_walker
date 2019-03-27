using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using BAWGUI.Views;
using BAWGUI.ViewModels;
using BAWGUI.Core;
using BAWGUI.Utilities;
using BAWGUI.Settings.ViewModels;
using VoltageStability.Models;
using VoltageStability.ViewModels;
using ModeMeter.ViewModels;
using ModeMeter.Models;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BAWGUI.RunMATLAB.ViewModels
{
    public class ProjectsControlViewModel : ViewModelBase
    {
        private ObservableCollection<AWProjectViewModel> _awProjects;
        public ObservableCollection<AWProjectViewModel> AWProjects
        {
            get { return _awProjects; }
            set
            {
                _awProjects = value;
                OnPropertyChanged();
            }
        }
        //private string _resultsStoragePath;

        public ProjectsControlViewModel()
        {
            _resultsStoragePath = BAWGUI.Properties.Settings.Default.ResultStoragePath;
            if (!Directory.Exists(_resultsStoragePath))
            {
                _resultsStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            _awProjects = new ObservableCollection<AWProjectViewModel>();
            BrowseResultsStorage = new RelayCommand(_browseResultsStorage);
            AddAProject = new RelayCommand(_addAAWProject);
            DeleteProject = new RelayCommand(_deleteAProject);
            _generateProjectTree(ResultsStoragePath);
            SaveConfigFile = new RelayCommand(_saveConfigFile);
            AddRun = new RelayCommand(_addARun);
            //_addTaskVM = new AddTaskViewModel();
            _addRundialogbox = new AddARunPopup();
            _saveConfigFileFlag = true;
            _generatedNewRun = new AWRunViewModel();
            _isMatlabEngineRunning = false;
            _runCommands = new RunMATLABViewModel();
            DeleteRun = new RelayCommand(_deleteARun);
            _canRun = true;
        }

        //public ProjectsControlViewModel(string resultsStoragePath)
        //{
        //    _resultsStoragePath = resultsStoragePath;
        //    _awProjects = new ObservableCollection<AWProjectViewModel>();
        //    foreach (var dir in Directory.GetDirectories(resultsStoragePath))
        //    {
        //        var projectNameFrac = Path.GetFileName(dir).Split('_');
        //        if (projectNameFrac[0] == "Project" && Directory.Exists(dir))
        //        {
        //            var aNewPoject = new AWProjectViewModel(dir);
        //            aNewPoject.ProjectSelected += OnOneOfTheProjectSelected;
        //            _awProjects.Add(aNewPoject);
        //        }
        //    }
        //}
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
        //private AWProjectViewModel _selectedProject;
        //public AWProjectViewModel SelectedProject
        //{
        //    get { return _selectedProject; }
        //    set
        //    {
        //        _selectedProject = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private void OnOneOfTheProjectSelected(object sender, AWProjectViewModel e)
        //{
        //    SelectedProject = e;
        //    //OnProjectSelected(e);
        //}
        //public event EventHandler<AWProjectViewModel> ProjectSelected;
        //protected virtual void OnProjectSelected(AWProjectViewModel e)
        //{
        //    ProjectSelected?.Invoke(this, e);
        //}
        //private ProjectsControlViewModel _projectControl = new ProjectsControlViewModel();
        //public ProjectsControlViewModel ProjectControl
        //{
        //    get { return _projectControl; }
        //    set
        //    {
        //        _projectControl = value;
        //        OnPropertyChanged();
        //    }
        //}
        private string _resultsStoragePath;
        public string ResultsStoragePath
        {
            get { return _resultsStoragePath; }
            set
            {
                _resultsStoragePath = value;
                BAWGUI.Properties.Settings.Default.ResultStoragePath = value;
                BAWGUI.Properties.Settings.Default.Save();
                try
                {
                    _generateProjectTree(_resultsStoragePath);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error reading project folder.", "Error!", MessageBoxButtons.OK);
                }
                OnPropertyChanged();
            }
        }
        public ICommand BrowseResultsStorage { get; set; }
        private void _browseResultsStorage(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = ResultsStoragePath;
                fbd.IsFolderPicker = true;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = ResultsStoragePath;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                CommonFileDialogResult result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    ResultsStoragePath = fbd.FileName;
                    try
                    {
                        _generateProjectTree(ResultsStoragePath);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error reading project folder. Original error: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                    //string[] files = Directory.GetFiles(ResultsStoragePath);
                }
            }
        }

        private void _generateProjectTree(string ResultsStoragePath)
        {
            if (Directory.Exists(ResultsStoragePath))
            {
                //ProjectControl = new ProjectsControlViewModel(ResultsStoragePath);
                _resultsStoragePath = ResultsStoragePath;
                var awProjects = new ObservableCollection<AWProjectViewModel>();
                foreach (var dir in Directory.GetDirectories(ResultsStoragePath))
                {
                    var projectNameFrac = Path.GetFileName(dir).Split('_');
                    if (projectNameFrac[0] == "Project" && Directory.Exists(dir))
                    {
                        var aNewPoject = new AWProjectViewModel(dir);
                        aNewPoject.ProjectSelected += _onProjectSelected;
                        awProjects.Add(aNewPoject);
                    }
                }
                AWProjects = awProjects;
                //ProjectControl.ProjectSelected += OnProjectSelected;
            }
            else
            {
                throw new Exception("Directory does not exists.");
            }
        }

        private void _onProjectSelected(object sender, AWProjectViewModel e)
        {
            SelectedProject = e;
            SelectedRun = e.SelectedRun;
            OnRunSelected(e);
        }
        public event EventHandler<AWProjectViewModel> RunSelected;
        protected virtual void OnRunSelected(AWProjectViewModel e)
        {
            RunSelected?.Invoke(this, e);
        }

        private AWRunViewModel _selectedRun;
        public AWRunViewModel SelectedRun
        {
            get { return _selectedRun; }
            set
            {
                _selectedRun = value;
#if !DEBUG
                CanRun = !_findRunGeneratedFile(value.Model.RunPath);
#endif
                OnPropertyChanged();
            }
        }
        private AddAProjectPopup _addProjectdialogbox;
        public ICommand AddAProject { get; set; }
        private void _addAAWProject(object obj)
        {
            var _addProjectVM = new AddProjectViewModel();
            _addProjectVM.NameAccepted += _generateNewProjectStructureOnDisk;
            _addProjectVM.NewTaskCancelled += _newProjectCancelled;
            _addProjectdialogbox = new AddAProjectPopup
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = _addProjectVM
            };
            _addProjectdialogbox.ShowDialog();
        }

        private void _newProjectCancelled(object sender, EventArgs e)
        {
            _addProjectdialogbox.Close();
        }

        private void _generateNewProjectStructureOnDisk(object sender, EventArgs e)
        {
            _addProjectdialogbox.Close();
            var newProjectName = ((AddProjectViewModel)sender).NewProjectName;
            var nameExistsFlag = false;
            foreach (var prj in _awProjects)
            {
                if (prj.ProjectName == newProjectName)
                {
                    nameExistsFlag = true;
                    break;
                }
            }
            if (nameExistsFlag)
            {
                System.Windows.Forms.MessageBox.Show("Project exists, please give a new name!", "ERROR!", MessageBoxButtons.OK);
                var _addProjectVM = new AddProjectViewModel();
                _addProjectVM.NameAccepted += _generateNewProjectStructureOnDisk;
                _addProjectVM.NewTaskCancelled += _newProjectCancelled;
                _addProjectdialogbox = new AddAProjectPopup
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                    DataContext = _addProjectVM
                };
                _addProjectdialogbox.ShowDialog();
            }
            else
            {
                var newProjectDir = ResultsStoragePath + "\\Project_" + newProjectName;
                Directory.CreateDirectory(newProjectDir);
                _generateProjectTree(ResultsStoragePath);
            }
        }

        public ICommand DeleteProject { get; set; }
        private void _deleteAProject(object obj)
        {
            var tobeDeleted = (AWProjectViewModel)obj;
            var dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete the whole project: " + tobeDeleted.ProjectName + " ?", "Warning!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (var prj in _awProjects)
                {
                    if (prj.ProjectName == tobeDeleted.Model.ProjectName)
                    {
                        _awProjects.Remove(prj);
                        break;
                    }
                }
                try
                {
                    Directory.Delete(tobeDeleted.Model.Projectpath, true);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error deleting the task directories. Error message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                }
                _generateProjectTree(ResultsStoragePath);
            }
        }

        public ICommand SaveConfigFile { get; set; }
        private void _saveConfigFile(object obj)
        {
            SettingsViewModel settingNeedsToBeSaved = null;
            if (obj is MainViewModel)
            {
                var mvm = (MainViewModel)obj;
                settingNeedsToBeSaved = mvm.SettingsVM;
            }
            else if (obj is SettingsViewModel)
            {
                settingNeedsToBeSaved = (SettingsViewModel)obj;
            }
            //var mvm = (MainViewModel)obj;
            //var settingNeedsToBeSaved = mvm.SettingsVM;
            var isTaskRan = false;
            if (_selectedRun != null)
            {
                isTaskRan = _findRunGeneratedFile(_selectedRun.Model.RunPath);
            }
            if (isTaskRan || _selectedRun == null)
            {
                //show add task window
                _addARun(null);
                //_selectedRun = SelectedRun;
            }
            else
            {
                _generatedNewRun = SelectedRun;
            }
            if (_saveConfigFileFlag)
            {
                var configWriter = new ConfigFileWriter(settingNeedsToBeSaved, _generatedNewRun.Model);
                try
                {
                    configWriter.WriteXmlConfigFile(_generatedNewRun.Model.ConfigFilePath);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error writing config.xml file!\n" + ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK);
                }
                var detectorList = settingNeedsToBeSaved.DetectorConfigure.DetectorList.Where(x => x is VoltageStabilityDetectorViewModel).Select(x=>(VoltageStabilityDetectorViewModel)x).ToList();
                if (detectorList.Count > 0)
                {
                    var MMDir = _generatedNewRun.Model.EventPath + "\\MM\\";
                    if (!Directory.Exists(MMDir))
                    {
                        Directory.CreateDirectory(MMDir);
                    }
                    var vsWriter = new VoltageStabilityDetectorGroupWriter();
                    try
                    {
                        vsWriter.WriteXmlCofigFile(_generatedNewRun.Model.ConfigFilePath, detectorList);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error writing voltage stability detector(s). Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                }
                var modeMeterList = settingNeedsToBeSaved.DetectorConfigure.DetectorList.Where(x => x is SmallSignalStabilityToolViewModel).Select(x => (SmallSignalStabilityToolViewModel)x).ToList();
                if (modeMeterList.Count > 0)
                {
                    var mmWriter = new ModeMeterXmlWriter();
                    try
                    {
                        mmWriter.WriteXmlCofigFile(_generatedNewRun.Model, modeMeterList);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error writing voltage stability detector(s). Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                }
                if (SelectedRun != null)
                //if matlab engine is running, only save the new config file, but not switch selected run so it won't trigger possible matlab engine to read pdat file in the newly selected run
                if (IsMatlabEngineRunning)
                {
                    _generatedNewRun.IsEnabled = false;
                }
                else
                {
                    if (SelectedRun != null)
                    {
                        SelectedRun.IsSelected = false;
                    }
                    SelectedRun = _generatedNewRun;
                    SelectedRun.IsSelected = true;
                }
            }
            _saveConfigFileFlag = true;
            //OnWriteSettingsConfigFile(SelectedRun.Model.ConfigFilePath);
        }
        //public event EventHandler<string> WriteSettingsConfigFile;
        //protected virtual void OnWriteSettingsConfigFile(string e)
        //{
        //    WriteSettingsConfigFile?.Invoke(this, e);
        //}
        public void CreateNewTask(ref SettingsViewModel svm)
        {
            _saveConfigFile(svm);
        }
        private bool _findRunGeneratedFile(string runPath)
        {
            foreach (var file in Directory.GetFiles(runPath))
            {
                var ext = Path.GetExtension(file).ToLower();
                var filename = Path.GetFileNameWithoutExtension(file);
                if (ext == "mat" || filename.Contains("EventList") || filename.Contains("Pause"))
                {
                    return true;
                }
            }
            foreach (var dir in Directory.GetDirectories(runPath))
            {
               var result = _findRunGeneratedFile(dir);
                if (result)
                {
                    return result;
                }
            }
            return false;
        }

        public ICommand AddRun { get; set; }
        private void _addARun(object obj)
        {
            var thisProject = (AWProjectViewModel)obj;
            var _addTaskVM = new AddTaskViewModel(_awProjects);
            _addTaskVM.NameAccepted += _generateNewTaskStructureOnDisk;
            _addTaskVM.NewTaskCancelled += _newTaskCancelled;
            _addTaskVM.CurrentProject = thisProject;
            _addRundialogbox = new AddARunPopup
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = _addTaskVM
            };
            _addRundialogbox.ShowDialog();
            //var dialogResult = dialogbox.ShowDialog();
            //if (true)
            //{

            //}
        }
        //private AddTaskViewModel _addTaskVM;
        private AddARunPopup _addRundialogbox;
        private bool _saveConfigFileFlag;
        private AWRunViewModel _generatedNewRun;

        private void _generateNewTaskStructureOnDisk(object sender, AWProjectViewModel e)
        {
            _addRundialogbox.Close();
            var newtaskName = ((AddTaskViewModel)sender).NewRunName;
            var RunExistsFlag = false;
            var NameExistsFlag = false;

            //nameExistsFlag = _findRunGeneratedFile(_selectedRun.Model.RunPath);

            foreach (var task in e.AWRuns)
            {
                if (task.Model.RunName == newtaskName)
                {
                    RunExistsFlag = _findRunGeneratedFile(task.Model.RunPath);
                    break;
                }
            }

            if (RunExistsFlag)
            {
                System.Windows.Forms.MessageBox.Show("Task exists, please give a new name!", "ERROR!", MessageBoxButtons.OK);
                var _addTaskVM = new AddTaskViewModel(_awProjects);
                _addTaskVM.NameAccepted += _generateNewTaskStructureOnDisk;
                _addTaskVM.NewTaskCancelled += _newTaskCancelled;
                _addTaskVM.CurrentProject = e;
                _addRundialogbox = new AddARunPopup
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                    DataContext = _addTaskVM
                };
                _addRundialogbox.ShowDialog();
            }
            else
            {
                foreach (var run in e.AWRuns)
                {
                    if (run.AWRunName == newtaskName)
                    {
                        NameExistsFlag = true;
                        break;
                    }
                }
                if (!NameExistsFlag)
                {
                    e.AddANewTask(newtaskName);
                    //AddANewTask(e, newtaskName);
                    //_generateProjectTree(ResultsStoragePath);
                }
                SelectedProject = e;
                foreach (var run in e.AWRuns)
                {
                    if (run.AWRunName == newtaskName)
                    {
                        _generatedNewRun = run;
                        //if (SelectedRun != null)
                        //{
                        //    SelectedRun.IsSelected = false;
                        //}
                        //run.IsSelected = true;
                        //SelectedRun = run;
                        break;
                    }
                }
                if (IsMatlabEngineRunning)
                {
                    _generatedNewRun.IsEnabled = false;
                }
                //var newRunVm = new AWRunViewModel(taskDir);
                //AWRuns.Add(new AWRunViewModel(taskDir));
            }
        }

        //private static void AddANewTask(AWProjectViewModel e, string newtaskName)
        //{
        //    var taskDir = e.Model.Projectpath + "Run_" + newtaskName;
        //    //DirectoryInfo dir = Directory.CreateDirectory(taskDir);
        //    var controlRunPath = taskDir + "\\ControlRun";
        //    Directory.CreateDirectory(controlRunPath);
        //    var controlReRunPath = taskDir + "\\ControlRerun";
        //    Directory.CreateDirectory(controlReRunPath);
        //    var eventPath = taskDir + "\\Event";
        //    Directory.CreateDirectory(eventPath);
        //    var initPath = taskDir + "\\Init";
        //    Directory.CreateDirectory(initPath);

        //    var newTask = new AWRun();
        //    newTask.ConfigFilePath = taskDir + "\\Config.xml";
        //    newTask.ControlRerunPath = controlReRunPath;
        //    newTask.ControlRunPath = controlRunPath;
        //    newTask.EventPath = eventPath;
        //    newTask.InitializationPath = initPath;
        //    newTask.RunName = newtaskName;
        //    newTask.RunPath = taskDir;
        //    e.Model.AWRuns.Add(newTask);
        //    e.AWRuns = e.GetAWRunViewModelCollection(e.Model.AWRuns);
        //}

        private void _newTaskCancelled(object sender, AWProjectViewModel e)
        {
            _addRundialogbox.Close();
            _saveConfigFileFlag = false;
        }
        private bool _isMatlabEngineRunning;
        public bool IsMatlabEngineRunning
        {
            get { return _isMatlabEngineRunning; }
            set
            {
                _isMatlabEngineRunning = value;
                OnPropertyChanged();
            }
        }
        private RunMATLABViewModel _runCommands;
        public RunMATLABViewModel RunCommands
        {
            get { return _runCommands; }
        }
        public ICommand DeleteRun { get; set; }
        private void _deleteARun(object obj)
        {
            var runToDelete = (AWRunViewModel)obj;
            SelectedProject.DeleteARun(runToDelete);
        }
        private bool _canRun;
        public bool CanRun
        {
            set
            {
                _canRun = value;
                OnPropertyChanged();
            }
            get
            {
                return _canRun;
            }
        }
    }
    public class AWProjectViewModel : ViewModelBase
    {
        private AWProject _model;
        public AWProject Model
        {
            get { return _model; }
        }
        public AWProjectViewModel(string dir)
        {
            _model = new AWProject(dir);
            DeleteRun = new RelayCommand(DeleteARun);
            AddRun = new RelayCommand(_addARun);
            //_addTaskVM = new AddTaskViewModel();
            _dialogbox = new AddARunPopup();
            IsEnabled = true;
            AWRuns = GetAWRunViewModelCollection(_model.AWRuns);
        }

        public AWProjectViewModel()
        {
        }

        public string ProjectName { get { return _model.ProjectName; } }
        private ObservableCollection<AWRunViewModel> _awRuns;
        public ObservableCollection<AWRunViewModel> AWRuns
        {
            set
            {
                _awRuns = value;
                OnPropertyChanged();
            }
            get
            {
                return _awRuns;
            }
        }

        public ObservableCollection<AWRunViewModel> GetAWRunViewModelCollection(List<AWRun> aWRuns)
        {
            var runs = new ObservableCollection<AWRunViewModel>();
            foreach (var run in _model.AWRuns)
            {
                var newRunViewModel = new AWRunViewModel(run);
                newRunViewModel.RunSelected += _onOneOfTheRunSelected;
                runs.Add(newRunViewModel);
            }
           return new ObservableCollection<AWRunViewModel>(runs.OrderBy(x => x.AWRunName).ToList());
        }

        private void _onOneOfTheRunSelected(object sender, AWRunViewModel e)
        {
            _checktaskdirintegrity(e);
            SelectedRun = e;
            OnProjectSelected(this);
        }
        /// <summary>
        /// in the situation where user created a task folder outside the GUI without create any of the subfolders,
        /// this function will create all necessary subfolders except the ones for mode meter usage
        /// </summary>
        private void _checktaskdirintegrity(AWRunViewModel task)
        {
            var taskDir = task.Model.RunPath;
            var runName = task.Model.RunName;
            var controlRunPath = taskDir + "\\ControlRun\\";
            if (!Directory.Exists(controlRunPath))
            {
                Directory.CreateDirectory(controlRunPath);
                task.Model.ControlRunPath = controlRunPath;
                System.Windows.Forms.MessageBox.Show("ControlRun subfolder was just created since it didn't exist.", "warning!", MessageBoxButtons.OK);
            }
            var controlReRunPath = taskDir + "\\ControlRerun\\";
            if (!Directory.Exists(controlReRunPath))
            {
                Directory.CreateDirectory(controlReRunPath);
                task.Model.ControlRerunPath = controlReRunPath;
                System.Windows.Forms.MessageBox.Show("ControlReRun subfolder was just created since it didn't exist.", "warning!", MessageBoxButtons.OK);
            }
            var eventPath = taskDir + "\\Event";
            if (!Directory.Exists(eventPath))
            {
                Directory.CreateDirectory(eventPath);
                task.Model.EventPath = eventPath;
                System.Windows.Forms.MessageBox.Show("Event subfolder was just created since it didn't exist.", "warning!", MessageBoxButtons.OK);
            }
            var initPath = taskDir + "\\Init";
            if (!Directory.Exists(initPath))
            {
                Directory.CreateDirectory(initPath);
                task.Model.InitializationPath = initPath;
                System.Windows.Forms.MessageBox.Show("Init subfolder was just created since it didn't exist.", "warning!", MessageBoxButtons.OK);
            }
            var configFilePath = taskDir + "\\Config.xml";
            if (!File.Exists(configFilePath))
            {
                System.IO.FileStream fs = System.IO.File.Create(configFilePath);
                fs.Close();
                var wr = new ConfigFileWriter(new Settings.ViewModels.SettingsViewModel(), task.Model);
                wr.WriteXmlConfigFile(configFilePath);
                task.Model.ConfigFilePath = configFilePath;
                System.Windows.Forms.MessageBox.Show("Config.xml was just created since it didn't exist.", "warning!", MessageBoxButtons.OK);
            }
        }

        public event EventHandler<AWProjectViewModel> ProjectSelected;
        protected virtual void OnProjectSelected(AWProjectViewModel e)
        {
            ProjectSelected?.Invoke(this, e);
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
        //private bool _isSelected;
        public bool IsSelected
        {
            get { return _model.IsSelected; }
            set
            {
                _model.IsSelected = value;
                //MessageBox.Show(ProjectName);
                OnPropertyChanged();
            }
        }
        public ICommand DeleteRun { get; set; }
        public void DeleteARun(object obj)
        {
            var runToDelete = (AWRunViewModel)obj;
            var dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete this task: " + runToDelete.AWRunName + " ?", "Warning!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (var task in _model.AWRuns)
                {
                    if (task.RunName == runToDelete.Model.RunName)
                    {
                        Model.AWRuns.Remove(task);
                        break;
                    }
                }
                //OnPropertyChanged("AWRuns");
                AWRuns.Remove(runToDelete);
                //AWRuns = GetAWRunViewModelCollection(_model.AWRuns);
                _deleteDirectoryRecursively(runToDelete.Model.RunPath);
                //_deleteARunFolder(runToDelete.Model.RunPath);
            }
        }

        private static void _deleteDirectoryRecursively(string path)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                _deleteDirectoryRecursively(dir);
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException ex)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException ex)
            {
                Directory.Delete(path, true);
            }
        }

        public ICommand AddRun { get; set; }
        private void _addARun(object obj)
        {
            var _addTaskVM = new AddTaskViewModel();
            _addTaskVM.NameAccepted2 += _generateNewTaskStructureOnDisk;
            _addTaskVM.NewTaskCancelled2 += _newTaskCancelled;
            _addTaskVM.CurrentProject = this;
            _dialogbox = new AddARunPopup
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = _addTaskVM
            };
            _dialogbox.ShowDialog();
            //var dialogResult = dialogbox.ShowDialog();
            //if (true)
            //{

            //}
        }
        //private AddTaskViewModel _addTaskVM;
        private AddARunPopup _dialogbox;
        private void _generateNewTaskStructureOnDisk(object sender, EventArgs e)
        {
            _dialogbox.Close();
            var newtaskName = ((AddTaskViewModel)sender).NewRunName;
            var nameExistsFlag = false;
            foreach (var task in AWRuns)
            {
                if (task.Model.RunName == newtaskName)
                {
                    nameExistsFlag = true;
                    break;
                }
            }
            if (nameExistsFlag)
            {
                System.Windows.Forms.MessageBox.Show("Task exists, please give a new name!", "ERROR!", MessageBoxButtons.OK);
                var _addTaskVM = new AddTaskViewModel();
                _addTaskVM.NameAccepted2 += _generateNewTaskStructureOnDisk;
                _addTaskVM.NewTaskCancelled2 += _newTaskCancelled;
                _addTaskVM.CurrentProject = this;
                _dialogbox = new AddARunPopup
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                    DataContext = _addTaskVM
                };
                _dialogbox.ShowDialog();
            }
            else
            {
                AddANewTask(newtaskName);
                //var newRunVm = new AWRunViewModel(taskDir);
                //AWRuns.Add(new AWRunViewModel(taskDir));
            }
        }
        public void AddANewTask(string newtaskName)
        {
            var taskDir = _model.Projectpath + "Task_" + newtaskName;
            //DirectoryInfo dir = Directory.CreateDirectory(taskDir);
            var controlRunPath = taskDir + "\\ControlRun\\";
            Directory.CreateDirectory(controlRunPath);
            var controlReRunPath = taskDir + "\\ControlRerun\\";
            Directory.CreateDirectory(controlReRunPath);
            var eventPath = taskDir + "\\Event";
            Directory.CreateDirectory(eventPath);
            var initPath = taskDir + "\\Init";
            Directory.CreateDirectory(initPath);

            var newTask = new AWRun();
            newTask.ConfigFilePath = taskDir + "\\Config.xml";
            newTask.ControlRerunPath = controlReRunPath;
            newTask.ControlRunPath = controlRunPath;
            newTask.EventPath = eventPath;
            newTask.InitializationPath = initPath;
            newTask.RunName = newtaskName;
            newTask.RunPath = taskDir;
            if (!File.Exists(newTask.ConfigFilePath))
            {
                FileStream fs = File.Create(newTask.ConfigFilePath);
                fs.Close();
                var wr = new ConfigFileWriter(new SettingsViewModel(), newTask);
                wr.WriteXmlConfigFile(newTask.ConfigFilePath);
            }
            _model.AWRuns.Add(newTask);
            var newTaskVieModel = new AWRunViewModel(newTask);
            newTaskVieModel.RunSelected += _onOneOfTheRunSelected;
            AWRuns.Add(newTaskVieModel);
            AWRuns = new ObservableCollection<AWRunViewModel>(AWRuns.OrderBy(x => x.AWRunName).ToList());
            //AWRuns = GetAWRunViewModelCollection(_model.AWRuns);
            //_model.AWRuns.OrderBy(x => x.RunName);
            //OnPropertyChanged("AWRuns");
        }
        private void _newTaskCancelled(object sender, EventArgs e)
        {
            _dialogbox.Close();
        }
        private void _deleteARunFolder(string runPath)
        {
            foreach (var file in Directory.GetFiles(runPath))
            {
                File.Delete(file);
            }
            foreach (var dir in Directory.GetDirectories(runPath))
            {
                _deleteARunFolder(dir);
            }
            Directory.Delete(runPath);
        }

        public bool IsEnabled
        {
            get { return _model.IsEnabled; }
            set
            {
                _model.IsEnabled = value;
                OnPropertyChanged();
            }
        }
        //public bool IsRunEnabled { get; set; }
    }

}
