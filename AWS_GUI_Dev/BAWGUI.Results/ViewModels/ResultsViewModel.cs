using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Results.Models;
using BAWGUI.Xml;
using System.Globalization;
//using System.Windows;
using System.Windows.Forms;
using BAWGUI.RunMATLAB.ViewModels;
using System.Windows.Input;
using System.IO;
using System.Xml.Linq;
using System.ComponentModel;
using System.Threading;
using BAWGUI.Core;
using BAWGUI.Utilities;
using System.Collections.ObjectModel;
using BAWGUI.MATLABRunResults.Models;

namespace BAWGUI.Results.ViewModels
{
    public class ResultsViewModel:ViewModelBase
    {
        //private ForcedOscillationResultsViewModel _forcedOscillationResultsViewModel = new ForcedOscillationResultsViewModel();
        //private OutOfRangeResultsViewModel _outOfRangeResultsViewModel = new OutOfRangeResultsViewModel();
        //private RingdownResultsViewModel _ringdownResultsViewModel = new RingdownResultsViewModel();
        //private WindRampResultsViewModel _windRampResultsViewModel = new WindRampResultsViewModel();
        //private ResultsModel _resultsModel = new ResultsModel();
        private ForcedOscillationResultsViewModel _forcedOscillationResultsViewModel;
        private OutOfRangeResultsViewModel _outOfRangeResultsViewModel;
        private RingdownResultsViewModel _ringdownResultsViewModel;
        private WindRampResultsViewModel _windRampResultsViewModel;
        private ResultsModel _resultsModel;
        private RunMATLAB.ViewModels.MatLabEngine _engine;
        public ForcedOscillationResultsViewModel ForcedOscillationResultsViewModel
        {
            get { return this._forcedOscillationResultsViewModel; }
            set { _forcedOscillationResultsViewModel = value; OnPropertyChanged(); }
        }

        public OutOfRangeResultsViewModel OutOfRangeResultsViewModel
        {
            get { return this._outOfRangeResultsViewModel; }
            set { _outOfRangeResultsViewModel = value; OnPropertyChanged(); }
        }

        public RingdownResultsViewModel RingdownResultsViewModel
        {
            get { return this._ringdownResultsViewModel; }
            set { _ringdownResultsViewModel = value; OnPropertyChanged(); }
        }

        public WindRampResultsViewModel WindRampResultsViewModel
        {
            get { return this._windRampResultsViewModel; }
            set { _windRampResultsViewModel = value; OnPropertyChanged(); }
        }

        public ResultsViewModel()
        {
            _engine = RunMATLAB.ViewModels.MatLabEngine.Instance;
            _engine.RunSelected += _engine_RunSelected;
            _resultsModel = new ResultsModel();
            _availableDateDict = new Dictionary<string, Dictionary<string, List<string>>>();
            _yearList = new List<string>();
            _monthList = new List<string>();
            _dayList = new List<string>();
            //OpenConfigFile = new RelayCommand(_openConfigFile);
            //_configFilePath = "";
            _forcedOscillationResultsViewModel = new ForcedOscillationResultsViewModel();
            _outOfRangeResultsViewModel = new OutOfRangeResultsViewModel();
            _ringdownResultsViewModel = new RingdownResultsViewModel();
            _windRampResultsViewModel = new WindRampResultsViewModel();
            _run = new AWRunViewModel();
            _resultsExist = true;
        }

        private void _engine_RunSelected(object sender, AWRunViewModel e)
        {
            _openResultFile(e.Model.EventPath);
        }

        private AWRunViewModel _run;
        public AWRunViewModel Run
        {
            get { return _run; }
            set
            {
                _run = value;
                if (File.Exists(_run.Model.ConfigFilePath))
                {
                    //ConfigFilePath = _run.Model.ConfigFilePath;
                    try
                    {
                        _readConfigFile();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read config file for results. Original error: " + ex.Message);
                    }
                }
                _forcedOscillationResultsViewModel.Run = _run;
                _outOfRangeResultsViewModel.Run = _run;
                _ringdownResultsViewModel.Run = _run;
                _windRampResultsViewModel.Run = _run;
                OnPropertyChanged();
            }
        }
        private AWProject _project;
        public AWProject Project
        {
            get { return _project; }
            set
            {
                _project = value;
                OnPropertyChanged();
            }
        }
        //public void LoadResults(List<string> filenames, string startDate, string endDate)
        public void LoadResults(List<string> filenames, List<string> dates)
        {
            try
            {
                _getAvailableDates(dates);
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing available dates from result files. " + ex.Message);
            }
            //if (filenames.Count() != dates.Count())
            //{

            //}
            //string startDate, endDate;
            //If Not String.IsNullOrEmpty(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeStart) Then
            //    startDate = Date.Parse(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeStart).ToString("yyMMdd")
            //Else
            //    startDate = dates.FirstOrDefault
            //End If
            //If Not String.IsNullOrEmpty(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeEnd) Then
            //    endDate = Date.Parse(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeEnd).ToString("yyMMdd")
            //Else
            //    endDate = dates.LastOrDefault
            //End If
            this._resultsModel.LoadResults(filenames, dates);
            var startTime = DateTime.ParseExact(Enumerable.LastOrDefault(dates), "yyMMdd", CultureInfo.InvariantCulture);
            //var endTime = startTime.AddDays(1).AddSeconds(-1);
            var startTimeStr = startTime.ToString("MM/dd/yyyy HH:mm:ss");
            var endTimeStr = startTime.AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
            _forcedOscillationResultsViewModel.FOPlotModel = new OxyPlot.PlotModel();
            _forcedOscillationResultsViewModel.Models = _resultsModel.ForcedOscillationCombinedList;
            _forcedOscillationResultsViewModel.SelectedEndTime = endTimeStr;
            _forcedOscillationResultsViewModel.SelectedStartTime = startTimeStr;
            var findStartTimeHasEvents = startTime;
            if (_forcedOscillationResultsViewModel.Models.Count() != 0)
            {
                while (_forcedOscillationResultsViewModel.FilteredResults.Count() == 0)
                {
                    findStartTimeHasEvents = findStartTimeHasEvents.AddDays(-1);
                    _forcedOscillationResultsViewModel.SelectedStartTime = findStartTimeHasEvents.ToString("MM/dd/yyyy HH:mm:ss");
                }
            }
            _ringdownResultsViewModel.SparsePlotModels = new ObservableCollection<SparsePlot>();
            _ringdownResultsViewModel.RdReRunPlotModels = new ObservableCollection<RDreRunPlot>();
            _ringdownResultsViewModel.Models = _resultsModel.RingdownEvents;
            _ringdownResultsViewModel.SelectedEndTime = endTimeStr;
            _ringdownResultsViewModel.SelectedStartTime = startTimeStr;
            _ringdownResultsViewModel.ReRunResult = new List<RingdownDetector>();
            findStartTimeHasEvents = startTime;
            if(_ringdownResultsViewModel.Models.Count()!= 0)
            {
                while (_ringdownResultsViewModel.FilteredResults.Count() == 0)
                {
                    findStartTimeHasEvents = findStartTimeHasEvents.AddDays(-1);
                    _ringdownResultsViewModel.SelectedStartTime = findStartTimeHasEvents.ToString("MM/dd/yyyy HH:mm:ss");
                }
            }
            _outOfRangeResultsViewModel.SparsePlotModels = new System.Collections.ObjectModel.ObservableCollection<SparsePlot>();
            _outOfRangeResultsViewModel.OORReRunPlotModels = new System.Collections.ObjectModel.ObservableCollection<OORReRunPlot>();
            _outOfRangeResultsViewModel.Models = _resultsModel.OutOfRangeEvents;
            _outOfRangeResultsViewModel.SelectedEndTime = endTimeStr;
            _outOfRangeResultsViewModel.SelectedStartTime = startTimeStr;
            _outOfRangeResultsViewModel.ReRunResult = new List<OutOfRangeDetector>();
            findStartTimeHasEvents = startTime;
            if (_outOfRangeResultsViewModel.Models.Count() != 0)
            {
                while (_outOfRangeResultsViewModel.FilteredResults.Count() == 0)
                {
                    findStartTimeHasEvents = findStartTimeHasEvents.AddDays(-1);
                    _outOfRangeResultsViewModel.SelectedStartTime = findStartTimeHasEvents.ToString("MM/dd/yyyy HH:mm:ss");
                }
            }
            _windRampResultsViewModel.SparsePlotModels = new System.Collections.ObjectModel.ObservableCollection<SparsePlot>();
            _windRampResultsViewModel.Models = _resultsModel.WindRampEvents;
            _windRampResultsViewModel.SelectedEndTime = endTimeStr;
            _windRampResultsViewModel.SelectedStartTime = startTimeStr;
            findStartTimeHasEvents = startTime;
            if (_windRampResultsViewModel.Models.Count() != 0)
            {
                while (_windRampResultsViewModel.FilteredResults.Count() == 0)
                {
                    findStartTimeHasEvents = findStartTimeHasEvents.AddDays(-1);
                    _windRampResultsViewModel.SelectedStartTime = findStartTimeHasEvents.ToString("MM/dd/yyyy HH:mm:ss");
                }
            }
            //_ringdownResultsViewModel.SparseResults = _engine.GetSparseData(_ringdownResultsViewModel.SelectedStartTime, _ringdownResultsViewModel.SelectedEndTime, _configFilePath, "Ringdown");
            //if(_ringdownResultsViewModel.FilteredResults.Count() != 0)
            //{
            //if (_engine.IsMatlabEngineRunning)
            //{
            //    var oldControlPath = _engine.ControlPath;
            //    var oldConfigFilename = _engine.ConfigFilePath;
            //    var pauseFlag = oldControlPath + "PauseFlag.txt";
            //    var runFlag = oldControlPath + "RunFlag.txt";
            //    System.IO.FileStream fs = System.IO.File.Create(pauseFlag);
            //    fs.Close();
            //    File.Delete(runFlag);
            //    while (_engine.worker.IsBusy)
            //    {
            //        Application.DoEvents();
            //        System.Threading.Thread.Sleep(500);
            //    }
            //    _ringdownResultsViewModel.SparseResults = _engine.GetSparseData(_ringdownResultsViewModel.SelectedStartTime, _ringdownResultsViewModel.SelectedEndTime, _configFilePath, "Ringdown");
            //    File.Delete(pauseFlag);
            //    //fs = System.IO.File.Create(runFlag);
            //    //fs.Close();
            //    _engine.RuNormalModeByBackgroundWorker(oldControlPath, oldConfigFilename);
            //    //_runAWNormal();
            //    //_engine.RunNormalMode(controlPath, _configFilePath);
            //}
            //else
            //{
            //    _ringdownResultsViewModel.SparseResults = _engine.GetSparseData(_ringdownResultsViewModel.SelectedStartTime, _ringdownResultsViewModel.SelectedEndTime, _configFilePath, "Ringdown");
            //}
            ////}
        }

        private Dictionary<string, Dictionary<string, List<string>>> _availableDateDict;
        public Dictionary<string, Dictionary<string, List<string>>> AvailableDatesDict
        {
            get { return _availableDateDict; }
            set
            {
                _availableDateDict = value;
                OnPropertyChanged();
            }
        }

        private List<DateStruct> _availableDateList;
        public List<DateStruct> AvailableDateList
        {
            get { return _availableDateList; }
            set
            {
                _availableDateList = value;
                OnPropertyChanged();
            }
        }
        private string _firstAvailableDate;
        public string FirstAvailableDate
        {
            get { return _firstAvailableDate; }
            set
            {
                _firstAvailableDate = value;
                OnPropertyChanged();
            }
        }
        private string _lastAvailableDate;
        public string LastAvailableDate
        {
            get { return _lastAvailableDate; }
            set
            {
                _lastAvailableDate = value;
                OnPropertyChanged();
            }
        }
        private string _selectedDateFromCalendar;
        public string SelectedDateFromCalendar
        {
            get { return _selectedDateFromCalendar; }
            set
            {
                _selectedDateFromCalendar = value;
                MessageBox.Show("Date selection NOT implemented yet!", "Error!", MessageBoxButtons.OK);
                //_forcedOscillationResultsViewModel.SelectedStartTime = DateTime.ParseExact(value, "yyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
                //_forcedOscillationResultsViewModel.SelectedEndTime = DateTime.ParseExact(value, "yyMMdd", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
                OnPropertyChanged();
            }
        }
        private void _getAvailableDates(List<string> dates)
        {
            List<DateStruct> dateList = new List<DateStruct>();
            string year, month, day;
            foreach(var date in dates)
            {
                year = "20" + date.Substring(0, 2);
                month = date.Substring(2, 2);
                day = date.Substring(4, 2);

                dateList.Add(new DateStruct(year, month, day));
                if (!_availableDateDict.ContainsKey(year))
                {
                    _availableDateDict[year] = new Dictionary<string, List<string>>();
                }
                if (!_availableDateDict[year].ContainsKey(month))
                {
                    _availableDateDict[year][month] = new List<string>();
                }
                _availableDateDict[year][month].Add(day);
            }
            YearList = _availableDateDict.Keys.ToList();
            DateStruct date1 = dateList.FirstOrDefault();
            string d1 = date1.Year.Substring(2, 2) + date1.Month + date1.Day;
            DateStruct date2 = dateList.LastOrDefault();
            string d2= date2.Year.Substring(2, 2) + date2.Month + date2.Day;
            FirstAvailableDate = DateTime.ParseExact(d1, "yyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            LastAvailableDate = DateTime.ParseExact(d2, "yyMMdd", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
            
            //AvailableDates.Values.
        }
        private List<string> _yearList;
        public List<string> YearList
        {
            get { return _yearList; }
            set
            {
                _yearList = value;
                OnPropertyChanged();
            }
        }
        private string _selectedYear;
        public string SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                MonthList = _availableDateDict[value].Keys.ToList();
                OnPropertyChanged();
            }
        }
        private List<string> _monthList;
        public List<string> MonthList
        {
            get { return _monthList; }
            set
            {
                _monthList = value;
                OnPropertyChanged();
            }
        }
        private string _selectedMonth;
        public string SelectedMonth
        {
            get { return _selectedMonth;}
            set
            {
                _selectedMonth = value;
                DayList = _availableDateDict[SelectedYear][value];
                OnPropertyChanged();
            }
        }
        private List<string> _dayList;
        public List<string> DayList
        {
            get { return _dayList; }
            set
            {
                _dayList = value;
                OnPropertyChanged();
            }
        }
        private string _selectedDay;
        public string SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                _selectedDay = value;
                string date = SelectedYear.Substring(2, 2) + SelectedMonth + SelectedDay;
                _forcedOscillationResultsViewModel.SelectedStartTime = DateTime.ParseExact(date, "yyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
                _forcedOscillationResultsViewModel.SelectedEndTime = DateTime.ParseExact(date, "yyMMdd", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
                OnPropertyChanged();
            }
        }

        //public ICommand OpenConfigFile { get; set; }
        //private void _openConfigFile(object obj)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.RestoreDirectory = true;
        //    openFileDialog.FileName = "";
        //    openFileDialog.DefaultExt = ".xml";
        //    openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
        //    if (!string.IsNullOrEmpty(_configFilePath))
        //    {
        //        openFileDialog.InitialDirectory = Path.GetFullPath(_configFilePath);
        //    }
        //    if (!Directory.Exists(openFileDialog.InitialDirectory))
        //    {
        //        openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\ConfigFiles";
        //    }
        //    openFileDialog.RestoreDirectory = true;
        //    openFileDialog.Title = "Please select a configuration file";
        //    if (openFileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        ConfigFilePath = openFileDialog.FileName;
        //        try
        //        {
        //            _readConfigFile(ConfigFilePath);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
        //        }
        //    }
        //}

        private void _readConfigFile()
        {
            var _configData = XDocument.Load(Run.Model.ConfigFilePath);
            var _resultPath = Run.Model.EventPath;
            //var _resultPath = (from el in _configData.Descendants("EventPath") select (string)el).FirstOrDefault();
            var dateTimeNode = from el in _configData.Descendants("DateTimeEnd") select(string)el;
            if (dateTimeNode.Any())
            {
                _lastDateOfTheRun = System.DateTime.ParseExact(dateTimeNode.FirstOrDefault(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToUniversalTime().ToString("yyMMdd");
            }
            //_lastDateOfTheRun = System.DateTime.ParseExact((from el in _configData.Descendants("DateTimeEnd") select (string)el).FirstOrDefault(), "yyyy-MM-dd HH:mm:ss GMT", CultureInfo.InvariantCulture).ToUniversalTime().ToString("yyMMdd");
            if (!String.IsNullOrEmpty(_resultPath) && Directory.Exists(_resultPath))
            {
                _openResultFile(_resultPath);
            }
            else
            {
                _cleanResults();
            }
        }

        private void _cleanResults()
        {
            ResultsExist = false;
            //MessageBox.Show("No valid result file found in project: " + _project.ProjectName +", in run: " + _run.RunName + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _resultsModel = new ResultsModel();
            ForcedOscillationResultsViewModel = new ForcedOscillationResultsViewModel();
            OutOfRangeResultsViewModel = new OutOfRangeResultsViewModel();
            OutOfRangeResultsViewModel.OORReRunPlotModels = new System.Collections.ObjectModel.ObservableCollection<OORReRunPlot>();
            OutOfRangeResultsViewModel.SparsePlotModels = new System.Collections.ObjectModel.ObservableCollection<SparsePlot>();
            RingdownResultsViewModel = new RingdownResultsViewModel();
            RingdownResultsViewModel.RdReRunPlotModels = new System.Collections.ObjectModel.ObservableCollection<RDreRunPlot>();
            RingdownResultsViewModel.SparsePlotModels = new System.Collections.ObjectModel.ObservableCollection<SparsePlot>();
            WindRampResultsViewModel = new WindRampResultsViewModel();
        }

        private void _openResultFile(string resultsPath)
        {
            DirectoryInfo folderName = new DirectoryInfo(resultsPath);
            List<string> files = new List<string>();
            List<string> dates = new List<string>();
            IEnumerable<FileInfo> resultFiles = null;
            try
            {
                resultFiles = folderName.GetFiles().OrderBy(x => x.Name).ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error finding result file in project: " + _project.ProjectName + ", in run: " + _run.AWRunName + ".\nOriginal error message:\n" + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            foreach (var file in resultFiles)
            {
                List<string> nameFragment = file.Name.Split(new Char [] { '.', '_' }).ToList();
                if (file.Extension.ToLower() == ".xml" && nameFragment.Count() == 3 && nameFragment[0].ToLower() == "eventlist")
                {
                    string dateStr = nameFragment[1];
                    try
                    {
                        System.DateTime.ParseExact(dateStr, "yyMMdd", CultureInfo.InvariantCulture);
                        dates.Add(dateStr);
                        files.Add(file.FullName);
                    }
                    catch (Exception ex)
                    {
                        if (file.Name.ToLower() == "eventlist_current.xml")
                        {
                            files.Add(file.FullName);
                        }
                    }
                }
            }
            if (files.Count()>0)
            {
                ResultsExist = true;
                //if (files.Count()!=dates.Count())
                //{
                //_lastDateOfTheRun = System.DateTime.ParseExact((from el in _configData.Descendants("DateTimeEnd") select (string)el).FirstOrDefault(), "yyyy-MM-dd HH:mm:ss GMT", CultureInfo.InvariantCulture).ToUniversalTime().ToString("yyMMdd");
                if (_lastDateOfTheRun != null && !dates.Contains(_lastDateOfTheRun))
                {
                    dates.Add(_lastDateOfTheRun);
                }
                //when there's more file than date, that means we have a current file and we don't know the end date of the data
                //for the current file, we don't know what the date of the events in the file would be
                //so we just add one more date in the dates assuming there could be an event happen in that last date, even if not, it is ok since we later check and make sure we have event to display for the last day.
                if (files.Count() != dates.Count())
                {
                    string lastDate = dates.LastOrDefault();
                    dates.Add((Convert.ToInt32(lastDate) + 1).ToString());
                }
                dates.Sort();
                try
                {
                    LoadResults(files, dates);
                }
                catch (Exception ex)
                {
                    string errorStr = "Error loading results: " + ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorStr = errorStr + "\n" + ex.InnerException.Message;
                    }
                    MessageBox.Show(errorStr, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _cleanResults();
            }            
        }
        //private string _configFilePath;
        //public string ConfigFilePath
        //{
        //    get { return _configFilePath; }
        //    set
        //    {
        //        _configFilePath = value;
        //        try
        //        {
        //            _readConfigFile(_configFilePath);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error: Could not read config file for results. Original error: " + ex.Message);
        //        }
        //        OnPropertyChanged();
        //    }
        //}
        private string _resultPath;
        public string ResultPath
        {
            get { return _resultPath; }
            set
            {
                _resultPath = value;
                OnPropertyChanged();
            }
        }
        private string _lastDateOfTheRun;
        public string LastDateOfTheRun
        {
            get { return _lastDateOfTheRun; }
            set
            {
                _lastDateOfTheRun = value;
                OnPropertyChanged();
            }
        }
        //private readonly BackgroundWorker _worker = new BackgroundWorker();
        //private void _runNormalMode(object sender, DoWorkEventArgs e)
        //{
        //    if (Thread.CurrentThread.Name == null)
        //    {
        //        Thread.CurrentThread.Name = "normalRunThread";
        //    }
        //    //var controlPath = @"C:\Users\wang690\Desktop\projects\ArchiveWalker\RerunTest\RerunTest\";
        //    var runFlag = _engineWrapper.Engine.ControlPath + "RunFlag.txt";
        //    if (!System.IO.File.Exists(runFlag))
        //    {
        //        System.IO.FileStream fs = System.IO.File.Create(runFlag);
        //        fs.Close();
        //    }
        //    _engineWrapper.IsMatlabEngineRunning = true;
        //    _engineWrapper.Engine.RunNormalMode(_engineWrapper.Engine.ControlPath, ConfigFilePath);
        //}
        //private void _runAWNormal()
        //{
        //    try
        //    {
        //        _worker.DoWork += _runNormalMode;
        //        _worker.ProgressChanged += _worker_ProgressChanged;
        //        _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        //        _worker.WorkerReportsProgress = true;
        //        _worker.WorkerSupportsCancellation = true;
        //        _worker.RunWorkerAsync();
        //        //System.Threading.Thread t1 = new System.Threading.Thread(() => { _engine.RunNormalMode(controlPath, ConfigFileName); });
        //        //t1.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK);
        //    }
        //}

        //private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    _engineWrapper.IsMatlabEngineRunning = false;
        //}

        //private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        private int _currentTabIndex;
        public int CurrentTabIndex
        {
            get { return _currentTabIndex; }
            set
            {
                _currentTabIndex = value;
                OnPropertyChanged();
            }
        }
        private bool _resultsExist;
        public bool ResultsExist
        {
            get { return _resultsExist; }
            set
            {
                _resultsExist = value;
                OnPropertyChanged();
            }
        }
    }
}
