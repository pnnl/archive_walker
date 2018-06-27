using BAWGUI.Core;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BAWGUI.ViewModels
{
    public class AddProjectViewModel:ViewModelBase
    {
        public AddProjectViewModel()
        {
            _newProjectName = "";
            AcceptName = new RelayCommand(_nameOK);
            CancelNewTask = new RelayCommand(_cancelNewTask);
        }
        private string _newProjectName;
        public string NewProjectName
        {
            get { return _newProjectName; }
            set
            {
                _newProjectName = value;
                OnPropertyChanged();
            }
        }
        public ICommand AcceptName { get; set; }
        private void _nameOK(object obj)
        {
            OnNameAccepted();
        }

        public ICommand CancelNewTask { get; set; }
        private void _cancelNewTask(object obj)
        {
            NewProjectName = "";
            OnNewTaskCancelled();
        }
        public event EventHandler NameAccepted;
        protected virtual void OnNameAccepted()
        {
            NameAccepted?.Invoke(this, EventArgs.Empty);
        }
        //public event Action NameAccepted;
        //protected virtual void OnNameAccepted()
        //{
        //    NameAccepted?.Invoke();
        //}
        public event EventHandler NewTaskCancelled;
        protected virtual void OnNewTaskCancelled()
        {
            NewTaskCancelled?.Invoke(this, EventArgs.Empty);
        }
        //public event Action NewTaskCancelled;
        //protected virtual void OnNewTaskCancelled()
        //{
        //    NewTaskCancelled?.Invoke();
        //}

    }
}
