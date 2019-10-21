using System;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Results.ViewModels;
using BAWGUI.Settings.ViewModels;
using System.Windows.Input;
using System.IO;
using BAWGUI.SignalManagement.ViewModels;
using BAWGUI.Utilities;
using System.Windows.Forms;
using System.Collections.Generic;
using VoltageStability.Models;
using VoltageStability.ViewModels;
using BAWGUI.CoordinateMapping.ViewModels;
using ModeMeter.Models;
using ModeMeter.ViewModels;
using BAWGUI.CoordinateMapping.Models;
using DissipationEnergyFlow.ViewModels;
using System.Collections.ObjectModel;

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
            _currentView = _settingsVM;
            MainViewSelected = new RelayCommand(_switchView);
            _projectControlVM.RunSelected += _onRunSelected;
            _signalMgr = SignalManager.Instance;
            //_settingsVM.SaveNewTasl += _settingsVM_SaveNewTasl;
            _settingsVM.SaveNewTask += _projectControlVM.CreateNewTask;
            //_projectControlVM.WriteSettingsConfigFile += _projectControlVM_WriteSettingsConfigFile;
            _runMatlabVM.MatlabRunning += _matlabEngineStatusChanged;
            InspectRawSignal = new RelayCommand(_inpsectRawInputSignals);
            InspectSignalByTimeRange = new RelayCommand(_inpsectAllSignalsByTimeRange);
            SiteMappingVM = new SiteMappingViewModel(CoordsTableVM.SiteCoords);
            //SiteMappingVM.AvailableSites = CoordsTableVM.SiteCoords;
            SiteMappingVM.SignalCoordsMappingVM = new SignalCoordsMappingViewModel(CoordsTableVM.SiteCoords, _signalMgr);
            _settingsVM.DEFAreasChanged += _settingsVM_DEFAreasChanged;
            _projectControlVM.ResultsStoragePathChanged += _projectControlVM_ResultsStoragePathChanged;
        }

        private void _settingsVM_DEFAreasChanged()
        {
            foreach (var dtr in SettingsVM.DetectorConfigure.DetectorList)
            {
                if (dtr is DEFDetectorViewModel)
                {
                    var thisdtr = dtr as DEFDetectorViewModel;
                    SiteMappingVM.DEFAreaSiteMappingVM.Areas = new ObservableCollection<EnergyFlowAreaCoordsMappingViewModel>(thisdtr.Areas.Values);
                    break;
                }
            }
        }

        private void _projectControlVM_ResultsStoragePathChanged(object sender, string e)
        {
            CoordsTableVM.LocationCoordinatesFilePath = e + "\\SiteCoordinatesConfig.xml";
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
                if (CurrentView == _signalMgr)
                {
                    _signalMgr.DeSelectAllPlots(null);
                }
                CurrentView = SettingsVM;
            }
            else if((string)obj == "Results")
            {
                if (CurrentView == _settingsVM)
                {
                    _deselectSettingsVMStep(_settingsVM);
                }
                if (CurrentView == _signalMgr)
                {
                    _signalMgr.DeSelectAllPlots(null);
                }
                CurrentView = ResultsVM;
            }
            else if((string)obj == "Coordinates")
            {
                if (CurrentView == _settingsVM)
                {
                    _deselectSettingsVMStep(_settingsVM);
                }
                if (CurrentView == _signalMgr)
                {
                    _signalMgr.DeSelectAllPlots(null);
                }
                CurrentView = CoordsTableVM;
                //CoordsTableVM.MapVM.GMap.InvalidateVisual();
            }
            else
            {
                if (CurrentView == _settingsVM)
                {
                    _deselectSettingsVMStep(_settingsVM);
                }
                CurrentView = _signalMgr;
            }
        }

        private void _deselectSettingsVMStep(SettingsViewModel settingsVM)
        {
            var tIndex = settingsVM.CurrentTabIndex;
            if (tIndex == 1)
            {
                settingsVM.DeSelectAllDataConfigSteps();
            }
            else if (tIndex == 2)
            {
                settingsVM.DeSelectAllProcessConfigSteps();
            }
            else if (tIndex == 3)
            {
                settingsVM.DeSelectAllPostProcessConfigSteps();
            }
            else if (tIndex == 4 || tIndex == 5)
            {
                settingsVM.DeSelectAllDetectors();
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
                SettingsVM.Run = e.SelectedRun;
                if (File.Exists(e.SelectedRun.Model.ConfigFilePath))
                {
                    try
                    {
                        var config = new ReadConfigXml.ConfigFileReader(e.SelectedRun.Model.ConfigFilePath);
                        //clean up the signal manager
                        _signalMgr.cleanUp();
                        //read input data files and generate all the signal objects from the data files and put them in the signal manager.
                        var readingDataSourceSuccess = _signalMgr.AddRawSignals(config.DataConfigure.ReaderProperty.InputFileInfos, config.DataConfigure.ReaderProperty.ExampleTime);
                        //pass signal manager into settings.
                        SettingsVM.SignalMgr = _signalMgr;
                        if (readingDataSourceSuccess)
                        {
                            //read config files
                            SettingsVM.DataConfigure = new DataConfig(config.DataConfigure, _signalMgr);
                            SettingsVM.ProcessConfigure = new ProcessConfig(config.ProcessConfigure, _signalMgr);
                            SettingsVM.PostProcessConfigure = new PostProcessCustomizationConfig(config.PostProcessConfigure, _signalMgr);
                            SettingsVM.DetectorConfigure = new DetectorConfig(config.DetectorConfigure, _signalMgr);
                            var cti = SettingsVM.CurrentTabIndex;
                            if (cti < 2)
                            {
                                SettingsVM.ReverseSignalPassedThroughNameTypeUnit();
                            }
                            SettingsVM.CurrentTabIndex = cti;
                            SettingsVM.CurrentSelectedStep = null;
                            e.SelectedRun.Model.DataFileDirectories = new List<string>();
                            foreach (var info in _signalMgr.FileInfo)
                            {
                                e.SelectedRun.Model.DataFileDirectories.Add(info.FileDirectory);
                            }

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
                            var modeMeters = new ModeMeterReader(e.SelectedRun.Model.ConfigFilePath).GetDetectors();
                            if (modeMeters.Count > 0)
                            {
                                foreach (var mm in modeMeters)
                                {
                                    SettingsVM.DetectorConfigure.DetectorList.Add(new SmallSignalStabilityToolViewModel(mm, _signalMgr));
                                }
                                ModeMeterXmlWriter.CheckMMDirsStatus(e.SelectedRun.Model, modeMeters);
                            }
                        }
                        else
                        {
                            SettingsVM.DataConfigure.ReaderProperty = new ReaderProperties(config.DataConfigure.ReaderProperty, _signalMgr);
                        }
                        //set up DEF area and detector signals on map
                        var signalSiteMappingConfig = new SignalMappingPlotConfigReader(e.SelectedRun.Model.ConfigFilePath);
                        _signalMgr.DistinctMappingSignal();
                        SiteMappingVM.AvailableSites = CoordsTableVM.SiteCoords;
                        SiteMappingVM.SignalCoordsMappingVM = new SignalCoordsMappingViewModel(CoordsTableVM.SiteCoords, _signalMgr, signalSiteMappingConfig.GetSignalCoordsMappingModel());
                        var DEFDetectorFound = false;
                        foreach (var dtr in SettingsVM.DetectorConfigure.DetectorList)
                        {
                            if (dtr is DEFDetectorViewModel)
                            {
                                var thisdtr = dtr as DEFDetectorViewModel;
                                //var areas = new ObservableCollection<EnergyFlowAreaCoordsMappingViewModel>(thisdtr.Areas.Values);
                                SiteMappingVM.DEFAreaSiteMappingVM.Areas = new ObservableCollection<EnergyFlowAreaCoordsMappingViewModel>(thisdtr.Areas.Values);
                                DEFDetectorFound = true;
                                break;
                            }
                        }
                        if (!DEFDetectorFound)
                        {
                            SiteMappingVM.DEFAreaSiteMappingVM.Areas = new ObservableCollection<EnergyFlowAreaCoordsMappingViewModel>();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in reading config file.\n" + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("No Config.xml file found in the task folder.", "Error!", MessageBoxButtons.OK);
                }
                //need to read signal stability results

                RunMatlabVM.Project = e.Model;
                RunMatlabVM.Run = e.SelectedRun;
                ResultsVM.Project = e.Model;
                ResultsVM.Run = e.SelectedRun;
            }
        }
        public CoordinatesTableViewModel CoordsTableVM { get; set; }
        public SiteMappingViewModel SiteMappingVM { get; set; }
        //public SignalCoordsMappingViewModel SignalCoordsMappingVM { get; set; }
        //private void _checkMMDirsStatus(AWRunViewModel task, List<SmallSignalStabilityTool> modeMeters)
        //{
        //    var eventPath = task.Model.EventPath;
        //    var mm = eventPath + "\\MM";
        //    if (!Directory.Exists(mm))
        //    {
        //        Directory.CreateDirectory(mm);
        //        System.Windows.Forms.MessageBox.Show("Modemeter event subfolder MM was just created since it didn't exist.", "warning!", MessageBoxButtons.OK);
        //    }
        //    foreach (var meter in modeMeters)
        //    {
        //        var meterDir = mm + "\\" + meter.ModeMeterName;
        //        if (!Directory.Exists(meterDir))
        //        {
        //            Directory.CreateDirectory(meterDir);
        //            System.Windows.Forms.MessageBox.Show("Subfolder for mode meter " + meter.ModeMeterName + " was just created since it didn't exist.", "warning!", MessageBoxButtons.OK);
        //        }
        //    }
        //}

        private void _checkMMDirsStatus(AWRunViewModel task)
        {
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
            //if ((info.FileType == Core.Models.DataFileType.piDatabase) && (SettingsVM.DataConfigure.ReaderProperty.Model != null))
            //{

            //}
            //else
            //{
                try
                {
                    _signalMgr.GetRawSignalData(info, SettingsVM.DataConfigure.ReaderProperty.ExampleTime);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving data for viewing from example file. " + ex.Message, "Error!", MessageBoxButtons.OK);
                }
            //}
        }
        public ICommand InspectSignalByTimeRange { get; set; }
        private void _inpsectAllSignalsByTimeRange(object obj)
        {
            CurrentView = _signalMgr;
            var pm = obj as ViewResolvingPlotModel;
            try
            {
                _signalMgr.GetSignalDataByTimeRange(pm, ResultsVM.Run);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data for viewing from matlab retrieve data function. " + ex.Message, "Error!", MessageBoxButtons.OK);
            }
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
