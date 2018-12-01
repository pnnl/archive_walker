using System;
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
using BAWGUI.ReadConfigXml;

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
            //_settingsVM.SaveNewTasl += _settingsVM_SaveNewTasl;
            _settingsVM.SaveNewTask += _projectControlVM.CreateNewTask;
            //_projectControlVM.WriteSettingsConfigFile += _projectControlVM_WriteSettingsConfigFile;
            _runMatlabVM.MatlabRunning += _matlabEngineStatusChanged;
            InspectRawSignal = new RelayCommand(_inpsectRawInputSignals);
            InspectSignalByTimeRange = new RelayCommand(_inpsectAllSignalsByTimeRange);
        }
        //private void _settingsVM_SaveNewTasl(ref SettingsViewModel svm)
        //{
        //    throw new NotImplementedException();
        //}

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
            else if((string)obj == "Results")
            {
                CurrentView = ResultsVM;
            }
            else
            {
                CurrentView = _signalMgr;
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
        public SignalManager SignalMgr
        {
            get { return _signalMgr; }
        }
        private void _onRunSelected(object sender, AWProjectViewModel e)
        {
            if (e != null)
            {
                //SettingsVM = new SettingsViewModel();
                //RunMatlabVM = new RunMATLABViewModel();
                //ResultsVM = new ResultsViewModel();
                //SettingsVM.SaveNewTask += _projectControlVM.CreateNewTask;
                //RunMatlabVM.MatlabRunning += _matlabEngineStatusChanged;
                //if (CurrentView is SettingsViewModel)
                //{
                //    CurrentView = SettingsVM;
                //}
                //else
                //{
                //    CurrentView = ResultsVM;
                //}

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
                    var readingDataSourceSuccess = _signalMgr.AddRawSignals(config.DataConfigure.ReaderProperty.InputFileInfos);
                    if (readingDataSourceSuccess)
                    {
                        //pass signal manager into settings.
                        SettingsVM.SignalMgr = _signalMgr;
                        //read config files
                        SettingsVM.DataConfigure = new DataConfig(config.DataConfigure, _signalMgr);
                        SettingsVM.ProcessConfigure = new ProcessConfig(config.ProcessConfigure, _signalMgr);
                        SettingsVM.PostProcessConfigure = new PostProcessCustomizationConfig(config.PostProcessConfigure, _signalMgr);
                        SettingsVM.DetectorConfigure = new DetectorConfig(config.DetectorConfigure, _signalMgr);
                        var cti = SettingsVM.CurrentTabIndex;
                        SettingsVM.CurrentTabIndex = cti;
                        SettingsVM.CurrentSelectedStep = null;
                        e.SelectedRun.Model.DataFileDirectories = new List<string>();
                        foreach (var info in _signalMgr.FileInfo)
                        {
                            e.SelectedRun.Model.DataFileDirectories.Add(info.FileDirectory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in reading config file.\n" + ex.Message, "Error!", MessageBoxButtons.OK);
                }
            }
        }

        //private void _projectControlVM_RunSelected(object sender, AWRunViewModel e)
        //{
        //    throw new NotImplementedException();
        //}
        public ICommand InspectRawSignal { get; set; }
        private void _inpsectRawInputSignals(object obj)
        {
            CurrentView = _signalMgr;
            var info = (InputFileInfoViewModel)obj;
            _signalMgr.GetRawSignalData(info);
        }
        public ICommand InspectSignalByTimeRange { get; set; }
        private void _inpsectAllSignalsByTimeRange(object obj)
        {
            CurrentView = _signalMgr;
            var pm = obj as ViewResolvingPlotModel;
            _signalMgr.GetSignalDataByTimeRange(pm, ResultsVM.Run);
            //foreach (var ax in pm.Axes)
            //{
            //    if (ax.IsHorizontal())
            //    {
            //        var start = DateTime.FromOADate(ax.ActualMinimum).ToString("MM/dd/yyyy HH:mm:ss");
            //        var end = DateTime.FromOADate(ax.ActualMaximum).ToString("MM/dd/yyyy HH:mm:ss");
            //        _signalMgr.GetSignalDataByTimeRange(start, end, ResultsVM.Run);
            //        break;
            //    }
            //}
        }

    }
}
