﻿using System;
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
            _forcedOscillationResultsViewModel = new ForcedOscillationResultsViewModel();
            _outOfRangeResultsViewModel = new OutOfRangeResultsViewModel();
            _ringdownResultsViewModel = new RingdownResultsViewModel();
            _windRampResultsViewModel = new WindRampResultsViewModel();
            _resultsModel = new ResultsModel();
            _availableDateDict = new Dictionary<string, Dictionary<string, List<string>>>();
            _yearList = new List<string>();
            _monthList = new List<string>();
            _dayList = new List<string>();
            OpenResultFile = new RelayCommand(_openResultFile);
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
            _forcedOscillationResultsViewModel.Models = _resultsModel.ForcedOscillationCombinedList;
            var startTime = DateTime.ParseExact(Enumerable.LastOrDefault(dates), "yyMMdd", CultureInfo.InvariantCulture);
            _forcedOscillationResultsViewModel.SelectedStartTime = startTime.ToString("MM/dd/yyyy HH:mm:ss");
            _forcedOscillationResultsViewModel.SelectedEndTime = startTime.AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
            if (_forcedOscillationResultsViewModel.FilteredResults.Count() == 0)
            {
                startTime = startTime.AddDays(-1);
                _forcedOscillationResultsViewModel.SelectedStartTime = startTime.ToString("MM/dd/yyyy HH:mm:ss");
                _forcedOscillationResultsViewModel.SelectedEndTime = startTime.AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
            }
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

        public ICommand OpenResultFile { get; set; }
        private void _openResultFile(object obj)
        {
            FolderBrowserDialog openDirectoryDialog = new FolderBrowserDialog();
            openDirectoryDialog.Description = "Select the directory that result xml files are located ";
            if (Directory.Exists(ResultsPath))
            {
                openDirectoryDialog.SelectedPath = ResultsPath;
            }
            else
            {
                openDirectoryDialog.SelectedPath = Environment.CurrentDirectory;
            }
            openDirectoryDialog.ShowNewFolderButton = false;
            DialogResult result = openDirectoryDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                ResultsPath = openDirectoryDialog.SelectedPath;
                DirectoryInfo folderName = new DirectoryInfo(ResultsPath);
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
                    if (files.Count()!=dates.Count())
                    {
                        dates.Sort();
                        string lastDate = dates.LastOrDefault();
                        dates.Add((Convert.ToInt32(lastDate) + 1).ToString());
                    }
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
                    MessageBox.Show("No valid file found in folder: " + openDirectoryDialog.SelectedPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private string _resultsPath;
        public string ResultsPath
        {
            get { return _resultsPath; }
            set
            {
                _resultsPath = value;
                OnPropertyChanged();
            }
        }
    }
}
