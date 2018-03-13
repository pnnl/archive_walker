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
        }

        public OutOfRangeResultsViewModel OutOfRangeResultsViewModel
        {
            get { return this._outOfRangeResultsViewModel; }
        }

        public RingdownResultsViewModel RingdownResultsViewModel
        {
            get { return this._ringdownResultsViewModel; }
        }

        public WindRampResultsViewModel WindRampResultsViewModel
        {
            get { return this._windRampResultsViewModel; }
        }

        public ResultsViewModel()
        {
            _engine = RunMATLAB.ViewModels.MatLabEngine.Instance;
            _resultsModel = new ResultsModel();
            _availableDateDict = new Dictionary<string, Dictionary<string, List<string>>>();
            _yearList = new List<string>();
            _monthList = new List<string>();
            _dayList = new List<string>();
            OpenConfigFile = new RelayCommand(_openConfigFile);
            _configFilePath = "";
            _forcedOscillationResultsViewModel = new ForcedOscillationResultsViewModel();
            _outOfRangeResultsViewModel = new OutOfRangeResultsViewModel();
            _ringdownResultsViewModel = new RingdownResultsViewModel();
            _windRampResultsViewModel = new WindRampResultsViewModel();
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

            _forcedOscillationResultsViewModel.Models = _resultsModel.ForcedOscillationCombinedList;
            _forcedOscillationResultsViewModel.SelectedStartTime = startTimeStr;
            _forcedOscillationResultsViewModel.SelectedEndTime = endTimeStr;
            var findStartTimeHasEvents = startTime;
            if (_forcedOscillationResultsViewModel.Models.Count() != 0)
            {
                while (_forcedOscillationResultsViewModel.FilteredResults.Count() == 0)
                {
                    findStartTimeHasEvents = findStartTimeHasEvents.AddDays(-1);
                    _forcedOscillationResultsViewModel.SelectedStartTime = findStartTimeHasEvents.ToString("MM/dd/yyyy HH:mm:ss");
                }
            }

            _ringdownResultsViewModel.Models = _resultsModel.RingdownEvents;
            _ringdownResultsViewModel.SelectedStartTime = startTimeStr;
            _ringdownResultsViewModel.SelectedEndTime = endTimeStr;
            findStartTimeHasEvents = startTime;
            if(_ringdownResultsViewModel.Models.Count()!= 0)
            {
                while (_ringdownResultsViewModel.FilteredResults.Count() == 0)
                {
                    findStartTimeHasEvents = findStartTimeHasEvents.AddDays(-1);
                    _forcedOscillationResultsViewModel.SelectedStartTime = findStartTimeHasEvents.ToString("MM/dd/yyyy HH:mm:ss");
                }
            }
            _ringdownResultsViewModel.SparseResults = _engine.GetSparseData(_ringdownResultsViewModel.SelectedStartTime, _ringdownResultsViewModel.SelectedEndTime, _configFilePath, "Ringdown");
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

        public ICommand OpenConfigFile { get; set; }
        private void _openConfigFile(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FileName = "";
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (!string.IsNullOrEmpty(_configFilePath))
            {
                openFileDialog.InitialDirectory = Path.GetFullPath(_configFilePath);
            }
            if (!Directory.Exists(openFileDialog.InitialDirectory))
            {
                openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\ConfigFiles";
            }
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Please select a configuration file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ConfigFilePath = openFileDialog.FileName;
                try
                {
                    _readConfigFile(ConfigFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void _readConfigFile(string configFilePath)
        {
            var _configData = XDocument.Load(ConfigFilePath);
            var _resultPath = (from el in _configData.Descendants("EventPath") select (string)el).FirstOrDefault();
            _lastDateOfTheRun = System.DateTime.ParseExact((from el in _configData.Descendants("DateTimeEnd") select (string)el).FirstOrDefault(), "yyyy-MM-dd HH:mm:ss GMT", CultureInfo.InvariantCulture).ToUniversalTime().ToString("yyMMdd");
            _openResultFile(_resultPath);
        }

        private void _openResultFile(string resultsPath)
        {
            DirectoryInfo folderName = new DirectoryInfo(resultsPath);
            List<string> files = new List<string>();
            List<string> dates = new List<string>();
            foreach (var file in folderName.GetFiles().OrderBy(x=>x.Name).ToArray())
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
                //if (files.Count()!=dates.Count())
                //{
                if (!dates.Contains(_lastDateOfTheRun))
                {
                    dates.Add(_lastDateOfTheRun);
                }
                dates.Sort();
                //string lastDate = dates.LastOrDefault();
                //dates.Add((Convert.ToInt32(lastDate) + 1).ToString());
                //}
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
                MessageBox.Show("No valid file found in folder: " + resultsPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
        private string _configFilePath;
        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                _configFilePath = value;
                _forcedOscillationResultsViewModel.ConfigFilePath = _configFilePath;
                _outOfRangeResultsViewModel.ConfigFilePath = _configFilePath;
                _ringdownResultsViewModel.ConfigFilePath = _configFilePath;
                _windRampResultsViewModel.ConfigFilePath = _configFilePath;
                OnPropertyChanged();
            }
        }
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

    }
}
