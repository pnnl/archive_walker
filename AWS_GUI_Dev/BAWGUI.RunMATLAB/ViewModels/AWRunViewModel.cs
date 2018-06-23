using BAWGUI.RunMATLAB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Core;
using BAWGUI.Utilities;

namespace BAWGUI.RunMATLAB.ViewModels
{
    public class AWRunViewModel : ViewModelBase
    {
        private AWRun _model;
        public AWRun Model
        {
            get { return _model; }
        }
        public AWRunViewModel(string dir)
        {
            _model = new AWRun(dir);
            _runCommands = new RunMATLABViewModel();
            //_isNormalRunPaused = false;
            //_isSelected = false;
            //DeleteRun = new RelayCommand(_deleteARun);
        }
        public AWRunViewModel()
        {
            _runCommands = new RunMATLABViewModel();
            //_isNormalRunPaused = false;
        }
        public AWRunViewModel(AWRun run)
        {
            _model = run;
            _runCommands = new RunMATLABViewModel();
        }
        private bool _isTaskrunning;
        public bool IsTaskRunning
        {
            get { return _isTaskrunning; }
            set
            {
                _isTaskrunning = value;
                OnPropertyChanged();
            }
        }
        public string AWRunName { get { return _model.RunName; } }
        public string AWRunConfigPath { get { return _model.ConfigFilePath; } }
        //private bool _isSelected;
        public bool IsSelected
        {
            get { return _model.IsSelected; }
            set
            {
                _model.IsSelected = value;
                //MessageBox.Show(AWRunConfigPath);
                if (value)
                {
                    OnRunSelected(this);
                }
                OnPropertyChanged();
            }
        }
        public event EventHandler<AWRunViewModel> RunSelected;
        protected virtual void OnRunSelected(AWRunViewModel e)
        {
            RunSelected?.Invoke(this, e);
        }
        private RunMATLABViewModel _runCommands;
        public RunMATLABViewModel RunCommands
        {
            get { return _runCommands; }
        }
        //public ICommand DeleteRun { get; set; }
        //private void _deleteARun(object obj)
        //{
        //    System.Windows.Forms.MessageBox.Show("Not Implemented!", "Error!", MessageBoxButtons.OK);
        //}
        //private bool _isNormalRunPaused;
        public bool IsNormalRunPaused
        {
            get { return _model.IsRunPaused; }
            set
            {
                _model.IsRunPaused = value;
                OnPropertyChanged();
            }
        }
        //private bool _isReRunPaused;
        //public bool IsReRunPaused
        //{
        //    get { return _isNormalRunPaused; }
        //    set
        //    {
        //        _isReRunPaused = value;
        //        OnPropertyChanged();
        //    }
        //}
    }
}
