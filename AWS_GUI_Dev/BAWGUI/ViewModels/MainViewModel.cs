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
using VoltageStability.Models;
using VoltageStability.ViewModels;
using ModeMeter.Models;

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
                    pjt.IsProjectEnabled = false;
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
                    pjt.IsProjectEnabled = true;
                    foreach (var run in pjt.AWRuns)
                    {
                        run.IsRunEnabled = true;
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
        private Object _currentView;
        public Object CurrentView
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
                    //clean up the signal manager
                    _signalMgr.cleanUp();
                    //read input data files and generate all the signal objects from the data files and put them in the signal manager.
                    _signalMgr.AddRawSignals(config.DataConfigure.ReaderProperty.InputFileInfos);
                    //pass signal manager into settings.
                    SettingsVM.SignalMgr = _signalMgr;
                    //read config files
                    SettingsVM.DataConfigure = new DataConfig(config.DataConfigure, _signalMgr);
                    SettingsVM.ProcessConfigure = new ProcessConfig(config.ProcessConfigure, _signalMgr);
                    SettingsVM.PostProcessConfigure = new PostProcessCustomizationConfig(config.PostProcessConfigure, _signalMgr);
                    SettingsVM.DetectorConfigure = new DetectorConfig(config.DetectorConfigure, _signalMgr);

                    //read voltage stability settings from config file.
                    var vsDetectors = new VoltageStabilityDetectorGroupReader(e.SelectedRun.Model.ConfigFilePath).GetDetector();
                    //add voltage stability detectors to te detector list in the settings.
                    if (vsDetectors.Count > 0)
                    {
                        SettingsVM.DetectorConfigure.ResultUpdateIntervalVisibility = System.Windows.Visibility.Visible;
                        foreach (var detector in vsDetectors)
                        {
                            SettingsVM.DetectorConfigure.DetectorList.Add(new VoltageStabilityDetectorViewModel(detector, _signalMgr));
                        }
                    }
                    var modeMeters = new ModeMeterReader(e.SelectedRun.Model.ConfigFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error in reading config file.\n" + ex.Message, "Error!", MessageBoxButtons.OK);
                }
                //need to read signal stability results

            }
        }

        //private void _projectControlVM_RunSelected(object sender, AWRunViewModel e)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
