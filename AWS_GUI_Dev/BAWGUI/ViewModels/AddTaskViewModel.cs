using BAWGUI.Core;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace BAWGUI.ViewModels
{
    public class AddTaskViewModel:ViewModelBase
    {
        public AddTaskViewModel(System.Collections.ObjectModel.ObservableCollection<AWProjectViewModel> awProjects)
        {
            _newRunName = "";
            _currentProject = new AWProjectViewModel();
            AcceptName = new RelayCommand(_nameOK);
            CancelNewTask = new RelayCommand(_cancelNewTask);
            _awProjects = awProjects;
        }

        public AddTaskViewModel()
        {
            _newRunName = "";
            _currentProject = new AWProjectViewModel();
            AcceptName = new RelayCommand(_nameOK);
            CancelNewTask = new RelayCommand(_cancelNewTask);
        }

        private System.Collections.ObjectModel.ObservableCollection<AWProjectViewModel> _awProjects;
        public System.Collections.ObjectModel.ObservableCollection<AWProjectViewModel> AWProjects
        {
            get { return _awProjects; }
            set
            {
                _awProjects = value;
                OnPropertyChanged();
            }
        }

        private AWProjectViewModel _currentProject;
        public AWProjectViewModel CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value;
                OnPropertyChanged();
            }
        }
        private string _newRunName;
        public string NewRunName
        {
            get { return _newRunName; }
            set
            {
                _newRunName = value;
                OnPropertyChanged();
            }
        }
        public ICommand AcceptName { get; set; }
        private void _nameOK(object obj)
        {
            OnNameAccepted(_currentProject);
            OnNameAccepted2();
        }

        public ICommand CancelNewTask { get; set; }
        private void _cancelNewTask(object obj)
        {
            NewRunName = "";
            OnNewTaskCancelled(_currentProject);
            OnNewTaskCancelled2();
        }
        public event EventHandler<AWProjectViewModel> NameAccepted;
        public event EventHandler NameAccepted2;
        protected virtual void OnNameAccepted(AWProjectViewModel currentProject)
        {
            NameAccepted?.Invoke(this, currentProject);
        }
        protected virtual void OnNameAccepted2()
        {
            NameAccepted2?.Invoke(this, EventArgs.Empty);
        }
        //public event Action NameAccepted;
        //protected virtual void OnNameAccepted()
        //{
        //    NameAccepted?.Invoke();
        //}
        public event EventHandler<AWProjectViewModel> NewTaskCancelled;
        public event EventHandler NewTaskCancelled2;
        protected virtual void OnNewTaskCancelled(AWProjectViewModel currentProject)
        {
            NewTaskCancelled?.Invoke(this, currentProject);
        }
        protected virtual void OnNewTaskCancelled2()
        {
            NewTaskCancelled2?.Invoke(this, EventArgs.Empty);
        }
        //public event Action NewTaskCancelled;
        //protected virtual void OnNewTaskCancelled()
        //{
        //    NewTaskCancelled?.Invoke();
        //}
    }
}
