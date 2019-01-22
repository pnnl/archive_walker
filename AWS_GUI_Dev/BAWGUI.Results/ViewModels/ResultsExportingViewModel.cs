using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.Results.Models;
using BAWGUI.Results.Views;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using JSISCSVWriter;

namespace BAWGUI.Results.ViewModels
{
    public class ResultsExportingViewModel : ViewModelBase
    {
        private string _detectorType;
        public string DetectorType
        {
            get { return _detectorType; }
            set
            {
                _detectorType = value;
                OnPropertyChanged();
            }
        }
        private AWRunViewModel _run;
        public AWRunViewModel Run
        {
            get { return _run; }
            set
            {
                _run = value;
                OnPropertyChanged();
            }
        }
        private UpdateOBATPresetPopup _updateOBATPresetPopup;
        public List<RerunResultDetectGeneral> RerunDetectors { get; set; }
        public List<Signal> SignalsToBeSaved { get; set; }
        public List<Signal> UniqueSignalsToBeSaved { get; set; }
        public ResultsExportingViewModel()
        {
            ExportData = new RelayCommand(_exportData);
            CancelExportData = new RelayCommand(_cancelExport);
            SignalsToBeSaved = new List<Signal>();
            UniqueSignalsToBeSaved = new List<Signal>();
            _detectorType = "";
        }
        public ResultsExportingViewModel(List<OutOfRangeDetector> reRunResult, AWRunViewModel run) : this()
        {
            DetectorType = "OutOfRangeGeneral";
            Run = run;
            RerunDetectors = new List<RerunResultDetectGeneral>();
            foreach (var detector in reRunResult)
            {
                var newDetector = new RerunResultDetectGeneral();
                newDetector.Label = detector.Label;
                newDetector.SamplingRate = detector.SamplingRate;
                newDetector.Unit = detector.Unit;
                newDetector.Type = detector.Type;
                RerunDetectors.Add(newDetector);
                foreach (var signal in detector.OORSignals)
                {
                    var newSignal = new Signal();
                    newSignal.Data = signal.Data;
                    newSignal.TimeStampInSeconds = signal.TimeStampInSeconds;
                    newSignal.PMUname = signal.PMUname;
                    newSignal.SignalName = signal.SignalName;
                    newSignal.Type = signal.Type;
                    newSignal.Unit = signal.Unit;
                    newSignal.SamplingRate = signal.SamplingRate;
                    newDetector.SignalsList.Add(newSignal);
                }
            }
        }

        public ResultsExportingViewModel(List<RingdownDetector> reRunResult, AWRunViewModel run) : this()
        {
            DetectorType = "Ringdown";
            Run = run;
            RerunDetectors = new List<RerunResultDetectGeneral>();
            foreach (var detector in reRunResult)
            {
                var newDetector = new RerunResultDetectGeneral();
                newDetector.Label = detector.Label;
                newDetector.SamplingRate = detector.SamplingRate;
                newDetector.Unit = detector.Unit;
                newDetector.Type = detector.Type;
                RerunDetectors.Add(newDetector);
                foreach (var signal in detector.RingdownSignals)
                {
                    var newSignal = new Signal();
                    newSignal.Data = signal.Data;
                    newSignal.TimeStampInSeconds = signal.TimeStampInSeconds;
                    newSignal.PMUname = signal.PMUname;
                    newSignal.SignalName = signal.SignalName;
                    newSignal.Type = signal.Type;
                    newSignal.Unit = signal.Unit;
                    newSignal.SamplingRate = signal.SamplingRate;
                    newDetector.SignalsList.Add(newSignal);
                }
            }
        }

        public ICommand CancelExportData { get; set; }
        private void _cancelExport(object obj)
        {
            OnExportDataCancelled();
        }
        public event EventHandler ExportDataCancelled;
        protected virtual void OnExportDataCancelled()
        {
            ExportDataCancelled?.Invoke(this, EventArgs.Empty);
        }
        public ICommand ExportData { get; set; }
        private void _exportData(object obj)
        {
            var checkedDetectorStatus = _checkSelectedDetectorSamplingRate();
            if (checkedDetectorStatus == 1)
            {
                _consolidateDataToExport();
                var filename = _exportDateToCSV();
                var result = MessageBox.Show("Do you want to update the OBAT preset file?", "", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    //pop up window to browse and save the preset file, show NewPreset, Detector, AWconfigFile path to let the user confirem they are right.
                    //check reRunResult type to know the detector type.
                    //the AWconfig file path need to be  find out by the run.
                    var updateOBATPreset = new OBATPresetUpdateViewModel(filename, DetectorType, Run);
                    updateOBATPreset.UpdateOBATPresetCancelled += _cancelUpateOBATPreset;
                    _updateOBATPresetPopup = new UpdateOBATPresetPopup
                    {
                        Owner = System.Windows.Application.Current.MainWindow,
                        DataContext = updateOBATPreset
                    };
                    _updateOBATPresetPopup.ShowDialog();
                }
            }
            else if(checkedDetectorStatus == 0)
            {
                MessageBox.Show("No detector selected.", "Warning!", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Selected detectors have to have the same sampling rate.", "Error!", MessageBoxButtons.OK);
            }
        }

        private void _cancelUpateOBATPreset(object sender, EventArgs e)
        {
            _updateOBATPresetPopup.Close();
        }

        private void _consolidateDataToExport()
        {
            SignalsToBeSaved = new List<Signal>();
            foreach (var item in RerunDetectors)
            {
                if (item.IsChecked)
                {
                    SignalsToBeSaved.AddRange(item.SignalsList);
                }
            }
            UniqueSignalsToBeSaved = SignalsToBeSaved.GroupBy(p => new { p.PMUname, p.SignalName }).Select(g => g.First()).ToList();
        }

        private string _exportDateToCSV()
        {
            string fullname = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JSIS_CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.Title = "Export Result Data File";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    var time = UniqueSignalsToBeSaved.FirstOrDefault().TimeStampInSeconds[0];
                    var timeStr = Utility.SecondsToDateTimeString(time);
                    var path = Path.GetFullPath(saveFileDialog1.FileName);
                    fullname = path.Split('.')[0] + "_" + timeStr + ".csv";
                    var index = 1;
                    while (File.Exists(fullname))
                    {
                        fullname = fullname.Split(new char[] { '(', '.' })[0] + "(" + index.ToString() + ").csv";
                        index++;
                    }
                    _saveData(fullname);
                    ExportDataCancelled?.Invoke(this, EventArgs.Empty);
                }
            }
            return fullname;
        }

        private void _saveData(string fullname)
        {
            var writer = new JSIS_CSV_Writer();
            writer.Signals = UniqueSignalsToBeSaved;
            writer.FileToBeSaved = fullname;
            writer.WriteJSISCSV();
        }

        private int _checkSelectedDetectorSamplingRate()
        {
            var selectedSamplingRate = -1d;
            var samplingRateAreTheSame = 0;
            foreach (var item in RerunDetectors)
            {
                if (item.IsChecked && selectedSamplingRate == -1)
                {
                    selectedSamplingRate = item.SamplingRate;
                    samplingRateAreTheSame = 1;
                }
                if (item.IsChecked && selectedSamplingRate != -1 && item.SamplingRate != selectedSamplingRate)
                {
                    samplingRateAreTheSame = 2;
                    break;
                }
            }
            return samplingRateAreTheSame;
        }
    }
}
