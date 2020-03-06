using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ModeMeter.ViewModels
{
    public class GenerateReportViewModel : ViewModelBase
    {
        private string _selectedStartTime;
        public string SelectedStartTime
        {
            get { return _selectedStartTime; }
            set { _selectedStartTime = value;
                OnPropertyChanged();
            }
        }
        private string _selectedEndTime;
        public string SelectedEndTime
        {
            get { return _selectedEndTime; }
            set { _selectedEndTime = value;
                OnPropertyChanged();
            }
        }
        private AWRunViewModel _run;
        private MatLabEngine _engine;
        public GenerateReportViewModel(string selectedStartTime, string selectedEndTime, AWRunViewModel run, MatLabEngine engine)
        {
            this._selectedStartTime = selectedStartTime;
            this._selectedEndTime = selectedEndTime;
            this._run = run;
            _engine = engine;
            CancelReport = new RelayCommand(_cancelReport);
            _reportType = "Graphical";
            ReportTypeIsGraphic = true;
            ReportTypeIsTabular = false;
            FindReportPath = new RelayCommand(_browseReportPath);
            GenerateReport = new RelayCommand(_saveReport);
        }
        public ICommand CancelReport { get; set; }
        private void _cancelReport(object obj)
        {
            OnGenerateReportCancelled();
        }
        public event EventHandler GenerateReportCancelled;
        protected virtual void OnGenerateReportCancelled()
        {
            GenerateReportCancelled?.Invoke(this, EventArgs.Empty);
        }
        private string _reportType;
        private bool _reportTypeIsGraphic;
        public bool ReportTypeIsGraphic
        {
            get { return _reportTypeIsGraphic; }
            set
            {
                _reportTypeIsGraphic = value;
                if (value)
                {
                    _reportType = "Graphical";
                }
                OnPropertyChanged();
            }
        }
        private bool _reportTypeIsTabular;
        public bool ReportTypeIsTabular
        {
            get { return _reportTypeIsTabular; }
            set
            {
                _reportTypeIsTabular = value;
                if (value)
                {
                    _reportType = "Tabular";
                }
                OnPropertyChanged();
            }
        }
        private int _dampThresh;
        public int DampThresh
        {
            get { return _dampThresh; }
            set
            {
                _dampThresh = value;
                OnPropertyChanged();
            }
        }
        private int _eventSepMinutes;
        public int EventSepMinutes
        {
            get { return _eventSepMinutes; }
            set
            {
                _eventSepMinutes = value;
                OnPropertyChanged();
            }
        }
        private string _reportPath;
        public string ReportPath
        {
            get { return _reportPath; }
            set
            {
                _reportPath = value;
                OnPropertyChanged();
            }
        }
        public ICommand FindReportPath { get; set; }
        private void _browseReportPath(object obj)
        {
            //using (var fbd = new CommonOpenFileDialog())
            //{
            //    fbd.InitialDirectory = _previousReportPath;
            //    fbd.IsFolderPicker = true;
            //    fbd.AddToMostRecentlyUsedList = true;
            //    fbd.AllowNonFileSystemItems = false;
            //    fbd.DefaultDirectory = _previousReportPath;
            //    fbd.EnsureFileExists = true;
            //    fbd.EnsurePathExists = true;
            //    fbd.EnsureReadOnly = false;
            //    fbd.EnsureValidNames = true;
            //    fbd.Multiselect = false;
            //    fbd.ShowPlacesList = true;
            //    CommonFileDialogResult result = fbd.ShowDialog();
            //    if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
            //    {
            //        _previousReportPath = fbd.FileName;
            //        ReportPath = fbd.FileName;
            //    }
            //}
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Select file to save generated mode meter report";
            saveFileDialog1.RestoreDirectory = true;
            if (_reportType == "Graphical")
            {
                saveFileDialog1.Filter = "Word files (*.doc)|*.doc|All files (*.*)|*.*";
            }
            else if (_reportType == "Tabular")
            {
                saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    ReportPath = saveFileDialog1.FileName;
                }
            }
        }
        public ICommand GenerateReport { get; set; }
        private void _saveReport(object obj)
        {
            var start = Convert.ToDateTime(SelectedStartTime, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            var end = Convert.ToDateTime(SelectedEndTime, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                _engine.GenerateMMreport(start, end, _run.Model.EventPath, _reportType, DampThresh, EventSepMinutes, ReportPath);
                GenerateReportCancelled?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK);
            }
        }
    }
}
