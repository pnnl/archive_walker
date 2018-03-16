using BAWGUI.RunMATLAB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BAWGUI.RunMATLAB.ViewModels
{
    public class ProjectsControlViewModel:ViewModelBase
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
        private string _resultsStoragePath;

        public ProjectsControlViewModel()
        {
            _resultsStoragePath = "";
            _awProjects = new ObservableCollection<AWProjectViewModel>();
        }

        public ProjectsControlViewModel(string resultsStoragePath)
        {
            _resultsStoragePath = resultsStoragePath;
            _awProjects = new ObservableCollection<AWProjectViewModel>();
            foreach (var dir in Directory.GetDirectories(resultsStoragePath))
            {
                var projectNameFrac = Path.GetFileName(dir).Split('_');
                if (projectNameFrac[0] == "Project" && Directory.Exists(dir))
                {
                    var aNewPoject = new AWProjectViewModel(dir);
                    aNewPoject.ProjectSelected += OnOneOfTheProjectSelected;
                    _awProjects.Add(aNewPoject);
                }
            }
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
        private void OnOneOfTheProjectSelected(object sender, AWProjectViewModel e)
        {
            SelectedProject = e;
            OnProjectSelected(e);
        }
        public event EventHandler<AWProjectViewModel> ProjectSelected;
        protected virtual void OnProjectSelected(AWProjectViewModel e)
        {
            ProjectSelected?.Invoke(this, e);
        }
    }
    public class AWProjectViewModel : ViewModelBase
    {
        private AWProject _model;
        public AWProjectViewModel(string dir)
        {
            _model = new AWProject(dir);
        }
        public string ProjectName { get { return _model.ProjectName; } }
        public ObservableCollection<AWRunViewModel> AWRuns
        {
            get
            {
                var runs = new ObservableCollection<AWRunViewModel>();
                foreach (var run in _model.AWRuns)
                {
                    var newRunViewModel = new AWRunViewModel(run.RunPath);
                    newRunViewModel.RunSelected += OnOneOfTheRunSelected;
                    runs.Add(newRunViewModel);
                }
                return runs;
            }
        }

        private void OnOneOfTheRunSelected(object sender, AWRunViewModel e)
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
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                //MessageBox.Show(ProjectName);
                OnPropertyChanged();
            }
        }

    }
    public class AWRunViewModel : ViewModelBase
    {
        private AWRun _model;
        public AWRunViewModel(string dir)
        {
            _model = new AWRun(dir);
        }
        public string AWRunName { get { return _model.RunName; } }
        public string AWRunConfigPath { get { return _model.ConfigFilePath; } }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                //MessageBox.Show(AWRunConfigPath);
                OnRunSelected(this);
                OnPropertyChanged();
            }
        }
        public event EventHandler<AWRunViewModel> RunSelected;
        protected virtual void OnRunSelected(AWRunViewModel e)
        {
            RunSelected?.Invoke(this, e);
        }
    }
}
