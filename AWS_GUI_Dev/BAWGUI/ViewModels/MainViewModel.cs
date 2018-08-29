﻿using System;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Results.ViewModels;
using BAWGUI.Settings;
using BAWGUI.Settings.ViewModels;
using System.Windows.Input;
using System.IO;
using BAWGUI.Core;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BAWGUI.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        public MainViewModel()
        {
            _settingsVM = new SettingsViewModel();
            _runMatlabVM = new RunMATLABViewModel();
            _resultsVM = new ResultsViewModel();
            _projectControlVM = new ProjectsControlViewModel();
            CoordsTableVM = new CoordinatesTableViewModel();
            _currentView = _resultsVM;
            MainViewSelected = new RelayCommand(_switchView);
            _projectControlVM.RunSelected += _onRunSelected;
            _signalMgr = SignalManager.Instance;
            //_projectControlVM.WriteSettingsConfigFile += _projectControlVM_WriteSettingsConfigFile;
            _runMatlabVM.MatlabRunning += _matlabEngineStatusChanged;
        }

        private void _matlabEngineStatusChanged(object sender, bool e)
        {
            var sd = sender as RunMATLABViewModel;
            if (e)
            {
                ProjectControlVM.IsMatlabEngineRunning = true;
                SettingsVM.IsMatlabEngineRunning = true;
                foreach (var pjt in ProjectControlVM.AWProjects)
                {
                    pjt.IsEnabled = false;
                    //if (pjt.ProjectName != sd.Project.ProjectName)
                    //{
                    //    pjt.IsProjectEnabled = false;
                    //    foreach (var run in pjt.AWRuns)
                    //    {
                    //        run.IsRunEnabled = false;
                    //    }
                    //}
                    //else
                    //{
                    //    foreach (var run in pjt.AWRuns)
                    //    {
                    //        if (run.AWRunName != sd.Run.AWRunName)
                    //        {
                    //            run.IsRunEnabled = false;
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                ProjectControlVM.IsMatlabEngineRunning = false;
                SettingsVM.IsMatlabEngineRunning = false;
                foreach (var pjt in ProjectControlVM.AWProjects)
                {
                    pjt.IsEnabled = true;
                    foreach (var run in pjt.AWRuns)
                    {
                        run.IsEnabled = true;
                    }
                }
            }
        }

        //private void _projectControlVM_WriteSettingsConfigFile(object sender, string e)
        //{
        //    var configWriter = new ConfigFileWriter(_settingsVM);
        //    try
        //    {
        //        configWriter.WriteXmlConfigFile(e);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show("Error writing config.xml file!\n" + ex.Message, "Error!", System.Windows.Forms.MessageBoxButtons.OK);
        //    }
        //}

        //private SettingsWindow _settingsWin = new SettingsWindow();
        private ResultsViewModel _resultsVM;
        public ResultsViewModel ResultsVM
        {
            get { return _resultsVM; }
            set
            {
                _resultsVM = value;
                OnPropertyChanged();
            }
        }
        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        private SettingsViewModel _settingsVM;
        public SettingsViewModel SettingsVM
        {
            get { return _settingsVM; }
            set
            {
                _settingsVM = value;
                OnPropertyChanged();
            }
        }
        private RunMATLABViewModel _runMatlabVM;
        public RunMATLABViewModel RunMatlabVM
        {
            get { return _runMatlabVM; }
            set
            {
                _runMatlabVM = value;
                OnPropertyChanged();
            }
        }
        public ICommand MainViewSelected { get; set; }
        private void _switchView(object obj)
        {
            if ((string)obj == "Settings")
            {
                CurrentView = SettingsVM;
            }
            else if((string)obj == "Coordinates")
            {
                CurrentView = CoordsTableVM;
            }
            else
            {
                CurrentView = ResultsVM;
            }
        }
        private ProjectsControlViewModel _projectControlVM;
        public ProjectsControlViewModel ProjectControlVM
        {
            get { return _projectControlVM; }
            set
            {
                _projectControlVM = value;
                OnPropertyChanged();
            }
        }
        private SignalManager _signalMgr;
        private void _onRunSelected(object sender, AWProjectViewModel e)
        {
            if (e != null)
            {
                SettingsVM.Project = e.Model;
                //SettingsVM.Run = e.SelectedRun.Model;
                ResultsVM.Project = e.Model;
                //ResultsVM.Run = e.SelectedRun.Model;
                //SettingsVM.Project = e;
                SettingsVM.Run = e.SelectedRun;
                //ResultsVM.Project = e;
                ResultsVM.Run = e.SelectedRun;
                RunMatlabVM.Run = e.SelectedRun;
                RunMatlabVM.Project = e.Model;
                try
                {
                    var config = new ReadConfigXml.ConfigFileReader(e.SelectedRun.Model.ConfigFilePath);
                    _signalMgr.cleanUp();
                    _signalMgr.AddRawSignals(config.DataConfigure.ReaderProperty.InputFileInfos);
                    SettingsVM.SignalMgr = _signalMgr;
                    SettingsVM.DataConfigure = new DataConfig(config.DataConfigure, _signalMgr);
                    SettingsVM.ProcessConfigure = new ProcessConfig(config.ProcessConfigure, _signalMgr);
                    SettingsVM.PostProcessConfigure = new PostProcessCustomizationConfig(config.PostProcessConfigure, _signalMgr);
                    SettingsVM.DetectorConfigure = new DetectorConfig(config.DetectorConfigure, _signalMgr);
                    e.SelectedRun.Model.DataFileDirectories = new List<string>();
                    foreach (var info in _signalMgr.FileInfo)
                    {
                        e.SelectedRun.Model.DataFileDirectories.Add(info.FileDirectory);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error in reading config file.\n" + ex.Message, "Error!", MessageBoxButtons.OK);
                }
            }
        }
        public CoordinatesTableViewModel CoordsTableVM { get; set; }
        //private void _projectControlVM_RunSelected(object sender, AWRunViewModel e)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
