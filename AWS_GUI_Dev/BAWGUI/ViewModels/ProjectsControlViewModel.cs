﻿using BAWGUI.RunMATLAB.Models;
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
                OnPropertyChanged();
            }
        }
        public ICommand BrowseResultsStorage { get; set; }
        private void _browseResultsStorage(object obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = ResultsStoragePath;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ResultsStoragePath = fbd.SelectedPath;
                    BAWGUI.Properties.Settings.Default.ResultStoragePath = ResultsStoragePath;
                    BAWGUI.Properties.Settings.Default.Save();
                    _generateProjectTree(ResultsStoragePath);
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
            var mvm = (MainViewModel)obj;
            var settingNeedsToBeSaved = mvm.SettingsVM;
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
                var configWriter = new Settings.ConfigFileWriter(settingNeedsToBeSaved, _generatedNewRun.Model);
                try
                {
                    configWriter.WriteXmlConfigFile(_generatedNewRun.Model.ConfigFilePath);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error writing config.xml file!\n" + ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK);
                }
                if (SelectedRun != null)
                {
                    SelectedRun.IsSelected = false;
                }
                SelectedRun = _generatedNewRun;
                SelectedRun.IsSelected = true;
            }
            _saveConfigFileFlag = true;
            //OnWriteSettingsConfigFile(SelectedRun.Model.ConfigFilePath);
        }
        //public event EventHandler<string> WriteSettingsConfigFile;
        //protected virtual void OnWriteSettingsConfigFile(string e)
        //{
        //    WriteSettingsConfigFile?.Invoke(this, e);
        //}

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
                //var newRunVm = new AWRunViewModel(taskDir);
                //AWRuns.Add(new AWRunViewModel(taskDir));
            }
        }

        private static void AddANewTask(AWProjectViewModel e, string newtaskName)
        {
            var taskDir = e.Model.Projectpath + "Run_" + newtaskName;
            //DirectoryInfo dir = Directory.CreateDirectory(taskDir);
            var controlRunPath = taskDir + "\\ControlRun";
            Directory.CreateDirectory(controlRunPath);
            var controlReRunPath = taskDir + "\\ControlRerun";
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
            e.Model.AWRuns.Add(newTask);
        }

        private void _newTaskCancelled(object sender, AWProjectViewModel e)
        {
            _addRundialogbox.Close();
            _saveConfigFileFlag = false;
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
            DeleteRun = new RelayCommand(_deleteARun);
            AddRun = new RelayCommand(_addARun);
            //_addTaskVM = new AddTaskViewModel();
            _dialogbox = new AddARunPopup();
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
                var runs = new ObservableCollection<AWRunViewModel>();
                foreach (var run in _model.AWRuns)
                {
                    var newRunViewModel = new AWRunViewModel(run);
                    newRunViewModel.RunSelected += _onOneOfTheRunSelected;
                    runs.Add(newRunViewModel);
                }
                _awRuns = new ObservableCollection<AWRunViewModel>(runs.OrderBy(x=>x.AWRunName).ToList());
                return _awRuns;
            }
        }

        private void _onOneOfTheRunSelected(object sender, AWRunViewModel e)
        {
            SelectedRun = e;
            OnProjectSelected(this);
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
        private void _deleteARun(object obj)
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
                OnPropertyChanged("AWRuns");
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
            foreach (var task in _awRuns)
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
            var taskDir = _model.Projectpath + "Run_" + newtaskName;
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
            if (!System.IO.File.Exists(newTask.ConfigFilePath))
            {
                System.IO.FileStream fs = System.IO.File.Create(newTask.ConfigFilePath);
                fs.Close();
                var wr = new Settings.ConfigFileWriter(new Settings.ViewModels.SettingsViewModel(), newTask);
                wr.WriteXmlConfigFile(newTask.ConfigFilePath);
            }
            _model.AWRuns.Add(newTask);
            //_model.AWRuns.OrderBy(x => x.RunName);
            OnPropertyChanged("AWRuns");
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

    }

}
