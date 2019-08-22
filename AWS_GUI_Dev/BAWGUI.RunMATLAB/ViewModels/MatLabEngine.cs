using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using BAWSengine;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using BAWGUI.Core;
using BAWGUI.Utilities;
using BAWGUI.MATLABRunResults.Models;
using VoltageStability.MATLABRunResults.Models;
using ModeMeter.MATLABRunResults.Models;
using System.Collections.ObjectModel;
using BAWGUI.Core.ViewModels;
using System.Globalization;

//[assembly: NOJVM(true)]
namespace BAWGUI.RunMATLAB.ViewModels
{
    public class MatLabEngine : ViewModelBase
    {
        private bool _isMatlabEngineRunning;
        public bool IsMatlabEngineRunning
        {
            get { return _isMatlabEngineRunning; }
            set
            {
                if (_isMatlabEngineRunning != value)
                {
                    _isMatlabEngineRunning = value;
                    OnMatlabEngineStatusChange(value);
                    OnPropertyChanged();
                }
            }
        }
        public event EventHandler<bool> MatlabRunning;
        protected virtual void OnMatlabEngineStatusChange(bool v)
        {
            MatlabRunning?.Invoke(this, v);
        }

        private bool _isNormalRunPaused;
        public bool IsNormalRunPaused
        {
            get { return _isNormalRunPaused; }
            set
            {
                _isNormalRunPaused = value;
                OnPropertyChanged();
            }
        }
        private bool _isReRunRunning;
        public bool IsReRunRunning
        {
            get { return _isReRunRunning; }
            set
            {
                _isReRunRunning = value;
                OnPropertyChanged();
            }
        }
        private string _controlPath;
        public string ControlPath
        {
            get { return _controlPath; }
            //set { _controlPath = value; }
        }
        private string _configFilePath;
        public string ConfigFilePath
        {
            get { return _configFilePath; }
        }
        private MatLabEngine()
        {
            _isMatlabEngineRunning = false;
            _isNormalRunPaused = false;
            _isReRunRunning = false;
            //_run = new AWRunViewModel();
            try
            {
                //_matlabEngine = new BAWSengine.GUI2MAT();
                _matlabEngine = new BAWSengine.GUI2MAT();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error getting MATLAB engine! Error message: " + ex.Message, "ERROR!", MessageBoxButtons.OK);
            }
        }
    
        private static MatLabEngine _instance = null;
        public static MatLabEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MatLabEngine();
                }
                return _instance;
            }
        }
        //private BAWSengine.GUI2MAT _matlabEngine;
        private BAWSengine.GUI2MAT _matlabEngine;
        //public void RunNormalMode(string controlPath, string configFile)
        //{
        //    _controlPath = controlPath;
        //    IsMatlabEngineRunning = true;
        //    _matlabEngine.RunNormalMode(controlPath, configFile);
        //    //TODO: ????????????maybe check if run flag exist, if yes, delete it.??????????????????
        //    IsMatlabEngineRunning = false;
        //}
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

        public void RingDownRerun(string start, string end, AWRunViewModel run)
        {

            if (IsMatlabEngineRunning)
            {
                //Here the running engine could be running a rerun instead of the normal run,
                //need to talk to Jim as how to distinguish normal or rerun,
                //I might need to set a separate flag for them or a class of flags with individual flag for each situation.
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();
            //IsNormalRunPaused = false;
            Run = run;
            _controlPath = run.Model.ControlRerunPath;
            _configFilePath = run.Model.ConfigFilePath;
            try
            {
                //_worker.DoWork += _runNormalMode;
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runRDReRunMode);
                worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _workerRDReRun_RunWorkerCompleted;
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                worker2.DoWork += _progressReporter;
                worker2.ProgressChanged += _worker_ProgressChanged;
                worker2.RunWorkerCompleted += _progressReportsDone;
                worker2.WorkerReportsProgress = true;
                worker2.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                object[] parameters = new object[] { start, end, run.Model.ConfigFilePath, run.Model.ControlRerunPath, run.Model.EventPath, run.Model.InitializationPath, run.Model.DataFileDirectories };
                worker.RunWorkerAsync(parameters);
                worker2.RunWorkerAsync();
                IsReRunRunning = true;
                Run.IsTaskRunning = true;
                //System.Threading.Thread t1 = new System.Threading.Thread(() => { _engine.RunNormalMode(controlPath, ConfigFileName); });
                //t1.Start();
            }
            catch (Exception ex)
            {
                //ex.Message = "Error in running normal mode by background walker." + ex.Message;
                throw ex;
            }
            //return e.RingdownRerunResults.RingdownDetectorList;
        }

        //private List<RingdownDetector> _rdReRunResults = new List<RingdownDetector>();
        //public List<RingdownDetector> RDReRunResults
        //{
        //    get { return _rdReRunResults; }
        //    set
        //    {
        //        _rdReRunResults = value;
        //        OnPropertyChanged();
        //    }
        //}
        private void _workerRDReRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker2.CancelAsync();
            IsMatlabEngineRunning = false;
            IsReRunRunning = false;
            Run.IsTaskRunning = false;
            //RDReRunResults = e.Result as List<RingdownDetector>;
            OnRDReRunCompletedEvent(e.Result as List<RingdownDetector>);
        }

        private void _runRDReRunMode(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "RDReRunThread";
            }
            object[] parameters = e.Argument as object[];
            var start = parameters[0] as string;
            var end = parameters[1] as string;
            var configFilename = parameters[2] as string;
            var controlPath = parameters[3] as string;
            var eventPath = parameters[4] as string;
            var initPath = parameters[5] as string;
            var dataFileDir = parameters[6] as List<string>;

            MWCellArray dataFileDirs = new MWCellArray(dataFileDir.Count);
            try
            {
                for (int index = 0; index < dataFileDir.Count; index++)
                {
                    dataFileDirs[index + 1] = new MWCharArray(dataFileDir[index]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            start = Convert.ToDateTime(start, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            end = Convert.ToDateTime(end, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            var runFlag = controlPath + "RunFlag.txt";
            if (!System.IO.File.Exists(runFlag))
            {
                System.IO.FileStream fs = System.IO.File.Create(runFlag);
                fs.Close();
            }
            var RingdownRerunResults = new RDRerunResults();
            IsMatlabEngineRunning = true;
            try
            {
                RingdownRerunResults = new RDRerunResults((MWStructArray)_matlabEngine.RerunRingdown(start, end, configFilename, controlPath, eventPath, initPath, dataFileDirs));
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                Run.IsTaskRunning = false;
                IsReRunRunning = false;
                MessageBox.Show("Error in running matlab ringdown re-run mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }
            e.Result = RingdownRerunResults.RingdownDetectorList;
        }

        public event EventHandler<List<RingdownDetector>> RDReRunCompletedEvent;
        protected virtual void OnRDReRunCompletedEvent(List<RingdownDetector> e)
        {
            RDReRunCompletedEvent?.Invoke(this, e);
        }

        //private void _runRDReRunMode(string start, string end, string configFilename, string controlPath)
        //{
        //    if (Thread.CurrentThread.Name == null)
        //    {
        //        Thread.CurrentThread.Name = "RDReRunThread";
        //    }
        //    start = Convert.ToDateTime(start, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
        //    end = Convert.ToDateTime(end, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
        //    var runFlag = controlPath + "RunFlag.txt";
        //    if (!System.IO.File.Exists(runFlag))
        //    {
        //        System.IO.FileStream fs = System.IO.File.Create(runFlag);
        //        fs.Close();
        //    }

        //    IsMatlabEngineRunning = true;
        //    var RingdownRerunResults = new RDRerunResults((MWStructArray)_matlabEngine.RerunRingdown(start, end, configFilename, controlPath));

        //    IsMatlabEngineRunning = false;
        //    //return RingdownRerunResults.RingdownDetectorList;
        //}

        public List<SparseDetector> GetSparseData(string start, string end, AWRunViewModel run, string detector)
        {
            start = Convert.ToDateTime(start, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            end = Convert.ToDateTime(end, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            if (IsMatlabEngineRunning)
            {
                PauseMatlabNormalRun();
            }
            IsMatlabEngineRunning = true;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            run.IsTaskRunning = true;
            SparseResults sparseResults = null;
            try
            {
                sparseResults = new SparseResults((MWStructArray)_matlabEngine.GetSparseData(start, end, run.Model.InitializationPath, detector));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK);
            }
            run.IsTaskRunning = false;
            Mouse.OverrideCursor = null;
            IsMatlabEngineRunning = false;
            //MessageBox.Show("Resuming normal run in the background.", "Notification", MessageBoxButtons.OK);
            //File.Delete(pauseFlag);
            //RuNormalModeByBackgroundWorker(ControlPath, ControlPath);
            return sparseResults.SparseDetectorList;
        }
        //private System.DateTime _numbTimeConvert(double item)
        //{
        //    System.DateTime dtDateTime = new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        //    System.DateTime bbb = dtDateTime.AddSeconds((item - 367) * 86400);
        //    return bbb;
        //}
        //public void RuNormalModeByBackgroundWalker()
        //{
        //    _runNormalModeByBackgroundWalker();
        //}
        public BackgroundWorker worker = new BackgroundWorker();
        public BackgroundWorker worker2 = new BackgroundWorker();

        //private void _runNormalMode(object sender, DoWorkEventArgs e)
        //{
        //    if (Thread.CurrentThread.Name == null)
        //    {
        //        Thread.CurrentThread.Name = "normalRunThread";
        //    }
        //    //var controlPath = @"C:\Users\wang690\Desktop\projects\ArchiveWalker\RerunTest\RerunTest\";
        //    var runFlag = controlPath + "RunFlag.txt";
        //    if (!System.IO.File.Exists(runFlag))
        //    {
        //        System.IO.FileStream fs = System.IO.File.Create(runFlag);
        //        fs.Close();
        //    }
        //    IsMatlabEngineRunning = true;
        //    RunNormalMode(controlPath, ConfigFileName);
        //    IsMatlabEngineRunning = true;
        //    _matlabEngine.RunNormalMode(controlPath, configFile);
        //    //TODO: ????????????maybe check if run flag exist, if yes, delete it.??????????????????
        //    IsMatlabEngineRunning = false;
        //}
        public void RuNormalModeByBackgroundWorker(AWRunViewModel run)
        {
            Run = run;
            worker = new BackgroundWorker();
            IsNormalRunPaused = false;
            IsNormalRunPaused = false;
            //_controlPath = controlPath;
            //_configFilePath = configFilename;
            try
            {
                //_worker.DoWork += _runNormalMode;
                worker.DoWork += (obj, e) => _runNormalMode(run.Model.ControlRunPath, run.Model.EventPath, run.Model.InitializationPath, run.Model.DataFileDirectories, run.Model.ConfigFilePath);
                worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                worker.RunWorkerAsync();
                //System.Threading.Thread t1 = new System.Threading.Thread(() => { _engine.RunNormalMode(controlPath, ConfigFileName); });
                //t1.Start();
            }
            catch (Exception ex)
            {
                //ex.Message = "Error in running normal mode by background walker." + ex.Message;
                throw ex;
            }
        }

        private void _runNormalMode(string controlPath, string eventPath, string initPath, List<string> dataFileDir, string configFilename)
        {
            MWCellArray dataFileDirs = new MWCellArray(dataFileDir.Count);
            try
            {
                for (int index = 0; index < dataFileDir.Count; index++)
                {
                    dataFileDirs[index + 1] = new MWCharArray(dataFileDir[index]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "normalRunThread";
            }
            var runFlag = controlPath + "RunFlag.txt";
            if (!System.IO.File.Exists(runFlag))
            {
                System.IO.FileStream fs = System.IO.File.Create(runFlag);
                fs.Close();
            }
            IsMatlabEngineRunning = true;
            Run.IsTaskRunning = true;
            IsNormalRunPaused = false;
            Run.IsNormalRunPaused = false;
            try
            {
                _matlabEngine.RunNormalMode(controlPath, eventPath, initPath, dataFileDirs, configFilename);
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                Run.IsTaskRunning = false;
                MessageBox.Show("Error in running matlab normal mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }
            //IsMatlabEngineRunning = false;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsMatlabEngineRunning = false;
            Run.IsTaskRunning = false;
            //need to see if the run that is done is selected, if yes, read in results
            if (_run.IsSelected)
            {
                OnRunSelected(_run);
            }
        }
        public event EventHandler<AWRunViewModel> RunSelected;
        protected virtual void OnRunSelected(AWRunViewModel e)
        {
            RunSelected?.Invoke(this, e);
        }
        private int _reRunProgress;
        public int ReRunProgress
        {
            get { return _reRunProgress; }
            set
            {
                _reRunProgress = value;
                OnPropertyChanged();
            }
        }

        private void _progressReporter(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            while (!bwAsync.CancellationPending)
            {
                foreach (var f in Directory.GetFiles(Run.Model.ControlRerunPath))
                {
                    if (Path.GetExtension(f) == ".csv")
                    {
                        bwAsync.ReportProgress(Int32.Parse(Path.GetFileNameWithoutExtension(f).Split('_')[1]));
                        break;
                    }
                }
                Thread.Sleep(100);
            }
            //var i = 0;
            //while (i < 101)
            //{
            //    bwAsync.ReportProgress(i);
            //    i++;
            //    Thread.Sleep(100);
            //}
            bwAsync.ReportProgress(100);
            Thread.Sleep(200);
        }
        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReRunProgress = e.ProgressPercentage;
        }

        private void _progressReportsDone(object sender, RunWorkerCompletedEventArgs e)
        {
            //Thread.Sleep(10000);
            ReRunProgress = 0;
        }

        public void StopMatlabNormalRun()
        {
            if (IsMatlabEngineRunning || IsNormalRunPaused)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to stop the matlab engine?", "Warning!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    IsMatlabEngineRunning = false;
                    IsNormalRunPaused = false;
                    Run.IsNormalRunPaused = false;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    System.IO.DirectoryInfo dir = new DirectoryInfo(Run.Model.ControlRunPath);
                    try
                    {
                        foreach (FileInfo file in dir.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error in deleting files in : " + ControlPath + ".\nOriginal exception message is: " + ex.Message);
                    }
                    //var runFlag = ControlPath + "RunFlag.txt";
                    //File.Delete(runFlag);
                    //if (Run.IsTaskRunning)
                    //{
                    //    Run.IsTaskRunning = false;
                    //}
                    while (worker.IsBusy)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(500);
                    }
                    Mouse.OverrideCursor = null;

                }
            }
            else
            {
                throw new Exception("Matlab engine is not running.");
            }

        }
        public void PauseMatlabNormalRun()
        {
            if (IsReRunRunning)
            {
                MessageBox.Show("Pausing re-run in the background.", "Notification", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Pausing normal run in the background.", "Notification", MessageBoxButtons.OK);
            }
            IsNormalRunPaused = true;
            Run.IsNormalRunPaused = true;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            var pauseFlag = "";
            var runFlag = "";
            if (IsReRunRunning)
            {
                pauseFlag = Run.Model.ControlRerunPath + "PauseFlag.txt";
                runFlag = Run.Model.ControlRerunPath + "RunFlag.txt";
            }
            else
            {
                pauseFlag = Run.Model.ControlRunPath + "PauseFlag.txt";
                runFlag = Run.Model.ControlRunPath + "RunFlag.txt";
            }
            System.IO.FileStream fs = System.IO.File.Create(pauseFlag);
            fs.Close();
            File.Delete(runFlag);
            //if (Run.IsTaskRunning)
            //{
            //    Run.IsTaskRunning = false;
            //}
            while (worker.IsBusy)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }
            IsMatlabEngineRunning = false;
            Mouse.OverrideCursor = null;
        }

        public void CancelRDReRun(AWRunViewModel run)
        {
            var result = MessageBox.Show("Cancel Ringdown re-run?", "Warning!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                System.IO.DirectoryInfo di = new DirectoryInfo(run.Model.ControlRerunPath);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete();
                }
                worker.CancelAsync();
                worker2.CancelAsync();
                //var pauseFlag = run.Model.ControlRerunPath + "PauseFlag.txt";
                //var runFlag = run.Model.ControlRerunPath + "RunFlag.txt";
                //System.IO.FileStream fs = System.IO.File.Create(pauseFlag);
                //fs.Close();
                //File.Delete(runFlag);
                while (worker.IsBusy)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                while (worker2.IsBusy)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                run.IsTaskRunning = false;
                Mouse.OverrideCursor = null;
            }
        }
        public void OutOfRangeRerun(string start, string end, AWRunViewModel run)
        {
            if (IsMatlabEngineRunning)
            {
                //Here the running engine could be running a rerun instead of the normal run,
                //need to talk to Jim as how to distinguish normal or rerun,
                //I might need to set a separate flag for them or a class of flags with individual flag for each situation.
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();
            worker2 = new BackgroundWorker();
            Run = run;
            _controlPath = run.Model.ControlRerunPath;
            _configFilePath = run.Model.ConfigFilePath;

            try
            {
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runOORReRunMode);
                worker.RunWorkerCompleted += _workerOORReRun_RunWorkerCompleted;
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                worker2.DoWork += _progressReporter;
                worker2.ProgressChanged += _worker_ProgressChanged;
                worker2.RunWorkerCompleted += _progressReportsDone;
                worker2.WorkerReportsProgress = true;
                worker2.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                object[] parameters = new object[] { start, end, run.Model.ConfigFilePath, run.Model.ControlRerunPath, run.Model.EventPath, run.Model.InitializationPath, run.Model.DataFileDirectories };
                worker.RunWorkerAsync(parameters);
                worker2.RunWorkerAsync();
                IsReRunRunning = true;
                Run.IsTaskRunning = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void _workerOORReRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker2.CancelAsync();
            IsMatlabEngineRunning = false;
            IsReRunRunning = false;
            Run.IsTaskRunning = false;
            //RDReRunResults = e.Result as List<RingdownDetector>;
            OnOORReRunCompletedEvent(e.Result as List<OutOfRangeDetector>);
        }

        private void OnOORReRunCompletedEvent(List<OutOfRangeDetector> e)
        {
            OORReRunCompletedEvent?.Invoke(this, e);
        }

        private void _runOORReRunMode(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "OORReRunThread";
            }
            object[] parameters = e.Argument as object[];
            var start = parameters[0] as string;
            var end = parameters[1] as string;
            var configFilename = parameters[2] as string;
            var controlPath = parameters[3] as string;
            var eventPath = parameters[4] as string;
            var initPath = parameters[5] as string;
            var dataFileDir = parameters[6] as List<string>;

            MWCellArray dataFileDirs = new MWCellArray(dataFileDir.Count);
            try
            {
                for (int index = 0; index < dataFileDir.Count; index++)
                {
                    dataFileDirs[index + 1] = new MWCharArray(dataFileDir[index]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            start = Convert.ToDateTime(start, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            end = Convert.ToDateTime(end, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            var runFlag = controlPath + "RunFlag.txt";
            if (!System.IO.File.Exists(runFlag))
            {
                System.IO.FileStream fs = System.IO.File.Create(runFlag);
                fs.Close();
            }
            var OORRerunResults = new OORRerunResults();
            IsMatlabEngineRunning = true;
            try
            {
                OORRerunResults = new OORRerunResults((MWStructArray)_matlabEngine.RerunOutOfRange(start, end, configFilename, controlPath, eventPath, initPath, dataFileDirs));
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                Run.IsTaskRunning = false;
                IsReRunRunning = false;
                MessageBox.Show("Error in running matlab out of range re-run mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }

            e.Result = OORRerunResults.OORDetectorList;
        }

        public Action<object, List<OutOfRangeDetector>> OORReRunCompletedEvent { get; set; }

        public void CancelOORReRun(AWRunViewModel run)
        {
            var result = MessageBox.Show("Cancel Out-Of-Range re-run?", "Warning!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                System.IO.DirectoryInfo di = new DirectoryInfo(run.Model.ControlRerunPath);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete();
                }
                worker.CancelAsync();
                worker2.CancelAsync();
                //var pauseFlag = run.Model.ControlRerunPath + "PauseFlag.txt";
                //var runFlag = run.Model.ControlRerunPath + "RunFlag.txt";
                //System.IO.FileStream fs = System.IO.File.Create(pauseFlag);
                //fs.Close();
                //File.Delete(runFlag);
                while (worker.IsBusy)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                while (worker2.IsBusy)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                run.IsTaskRunning = false;
                Mouse.OverrideCursor = null;
            }
        }
        public void VSRerun(string start, string end, AWRunViewModel run, int predictionDelay)
        {
            if (IsMatlabEngineRunning)
            {
                //Here the running engine could be running a rerun instead of the normal run,
                //need to talk to Jim as how to distinguish normal or rerun,
                //I might need to set a separate flag for them or a class of flags with individual flag for each situation.
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();
            Run = run;
            _controlPath = run.Model.ControlRerunPath;
            _configFilePath = run.Model.ConfigFilePath;

            try
            {
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runVSReRunMode);
                worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _workerVSReRun_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                object[] parameters = new object[] { start, end, run.Model.ConfigFilePath, run.Model.ControlRerunPath, run.Model.EventPath, run.Model.InitializationPath, run.Model.DataFileDirectories, predictionDelay };
                worker.RunWorkerAsync(parameters);
                IsReRunRunning = true;
                Run.IsTaskRunning = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _workerVSReRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsMatlabEngineRunning = false;
            IsReRunRunning = false;
            Run.IsTaskRunning = false;
            OnVSReRunCompletedEvent(e.Result as List<TheveninDetector>);
        }

        private void OnVSReRunCompletedEvent(List<TheveninDetector> e)
        {
            VSReRunCompletedEvent?.Invoke(this, e);
        }
        public Action<object, List<TheveninDetector>> VSReRunCompletedEvent { get; set; }

        private void _runVSReRunMode(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "VSReRunThread";
            }
            object[] parameters = e.Argument as object[];
            var start = parameters[0] as string;
            var end = parameters[1] as string;
            var configFilename = parameters[2] as string;
            var controlPath = parameters[3] as string;
            var eventPath = parameters[4] as string;
            var initPath = parameters[5] as string;
            var dataFileDir = parameters[6] as List<string>;
            var predictionDelay = parameters[7] as int?;

            MWCellArray dataFileDirs = new MWCellArray(dataFileDir.Count);
            try
            {
                for (int index = 0; index < dataFileDir.Count; index++)
                {
                    dataFileDirs[index + 1] = new MWCharArray(dataFileDir[index]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //start = Convert.ToDateTime(start, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            //end = Convert.ToDateTime(end, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            var runFlag = controlPath + "RunFlag.txt";
            if (!System.IO.File.Exists(runFlag))
            {
                System.IO.FileStream fs = System.IO.File.Create(runFlag);
                fs.Close();
            }
            var vsRerunResults = new TheveninReRunResults();
            IsMatlabEngineRunning = true;
            try
            {
                vsRerunResults = new TheveninReRunResults((MWStructArray)_matlabEngine.RerunThevenin(start, end, configFilename, controlPath, eventPath, initPath, dataFileDirs, predictionDelay));
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                MessageBox.Show("Error in running matlab voltage stability re-run mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }

            e.Result = vsRerunResults.VSDetectorList;
        }
        public void CancelVSReRun(AWRunViewModel run)
        {
            var result = MessageBox.Show("Cancel Voltage Stability re-run?", "Warning!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                System.IO.DirectoryInfo di = new DirectoryInfo(run.Model.ControlRerunPath);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete();
                }
                //var pauseFlag = run.Model.ControlRerunPath + "PauseFlag.txt";
                //var runFlag = run.Model.ControlRerunPath + "RunFlag.txt";
                //System.IO.FileStream fs = System.IO.File.Create(pauseFlag);
                //fs.Close();
                //File.Delete(runFlag);
                while (worker.IsBusy)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                run.IsTaskRunning = false;
                Mouse.OverrideCursor = null;
            }
        }


        //public PDATExampleResults ReadPDATSampleFile(string filename)
        public ReadExampleFileResults GetFileExample(string filename, int fileType)
        {
            var FileReadingResults = new ReadExampleFileResults();

            if (IsMatlabEngineRunning)
            {
                PauseMatlabNormalRun();
            }
            IsMatlabEngineRunning = true;
            try
            {
                FileReadingResults.GetSignals((MWStructArray)_matlabEngine.GetFileExample(filename, fileType, 1));
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                MessageBox.Show("Error in running matlab GetFileExample: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }
            IsMatlabEngineRunning = false;
            return FileReadingResults;
        }

        public ReadExampleFileResults GetDBFileExample(string starttime, string preset, string filename, string dbtype)
        {
            var FileReadingResults = new ReadExampleFileResults();
            var start = "";
            //MessageBox.Show("The time string is: " + starttime);
            try
            {
                start = Convert.ToDateTime(starttime, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            }
            catch (Exception)
            {
                MessageBox.Show("Time string convert to datetime object failed.");
            }

            if (IsMatlabEngineRunning)
            {
                PauseMatlabNormalRun();
            }
            IsMatlabEngineRunning = true;
            try
            {
                if (dbtype == "PI")
                {
                    FileReadingResults.GetSignals((MWStructArray)_matlabEngine.GetFileExampleDB(start, preset, filename, 1, "PI"));
                }
                else if (dbtype == "openHistorian")
                {
                    FileReadingResults.GetSignals((MWStructArray)_matlabEngine.GetFileExampleDB(start, preset, filename, 1, "OpenHistorian"));
                }
                else { }            
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                //MessageBox.Show("Error in running matlab GetFileExampleDB: " + ex.Message, "Error!", MessageBoxButtons.OK);
                throw new Exception("\nError in running matlab GetFileExampleDB: " + ex.Message);
            }
            IsMatlabEngineRunning = false;
            return FileReadingResults;
        }

        public event EventHandler<ReadExampleFileResults> RetrieveDataCompletedEvent;
        protected virtual void OnRetrieveDataCompletedEvent(ReadExampleFileResults e)
        {
            RetrieveDataCompletedEvent?.Invoke(this, e);
        }
        public void RetrieveData(string start, string end, AWRunViewModel run)
        {
            if (IsMatlabEngineRunning)
            {
                //Here the running engine could be running a rerun instead of the normal run,
                //need to talk to Jim as how to distinguish normal or rerun,
                //I might need to set a separate flag for them or a class of flags with individual flag for each situation.
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();
            //IsNormalRunPaused = false;
            Run = run;
            _controlPath = run.Model.ControlRerunPath;
            _configFilePath = run.Model.ConfigFilePath;
            try
            {
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runRetrieveDataMode);
                worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _workerRetrieveData_RunWorkerCompleted;
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                worker2.DoWork += _progressReporter;
                worker2.ProgressChanged += _worker_ProgressChanged;
                worker2.RunWorkerCompleted += _progressReportsDone;
                worker2.WorkerReportsProgress = true;
                worker2.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                object[] parameters = new object[] { start, end, run.Model.ConfigFilePath, run.Model.ControlRerunPath, run.Model.EventPath, run.Model.InitializationPath, run.Model.DataFileDirectories };
                worker.RunWorkerAsync(parameters);
                worker2.RunWorkerAsync();
                IsReRunRunning = true;
                Run.IsTaskRunning = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _workerRetrieveData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker2.CancelAsync();
            IsMatlabEngineRunning = false;
            IsReRunRunning = false;
            if (Run != null)
            {
                Run.IsTaskRunning = false;
            }
            OnRetrieveDataCompletedEvent(e.Result as ReadExampleFileResults);
        }

        private void _runRetrieveDataMode(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "RetrieveDataThread";
            }
            object[] parameters = e.Argument as object[];
            var start = parameters[0] as string;
            var end = parameters[1] as string;
            var configFilename = parameters[2] as string;
            var controlPath = parameters[3] as string;
            var eventPath = parameters[4] as string;
            var initPath = parameters[5] as string;
            var dataFileDir = parameters[6] as List<string>;

            MWCellArray dataFileDirs = new MWCellArray(dataFileDir.Count);
            try
            {
                for (int index = 0; index < dataFileDir.Count; index++)
                {
                    dataFileDirs[index + 1] = new MWCharArray(dataFileDir[index]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            var runFlag = controlPath + "RunFlag.txt";
            if (!System.IO.File.Exists(runFlag))
            {
                System.IO.FileStream fs = System.IO.File.Create(runFlag);
                fs.Close();
            }
            var FileReadingResults = new ReadExampleFileResults();
            IsMatlabEngineRunning = true;
            try
            {
                FileReadingResults.GetSignalsWithData((MWStructArray)_matlabEngine.RetrieveData(start, end, configFilename, controlPath, eventPath, initPath, dataFileDirs));
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                MessageBox.Show("Error in running matlab retrieve data mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
            }

            e.Result = FileReadingResults;
        }
        public void GetFileExampleSignalData(string filename, int fileType)
        {
            if (IsMatlabEngineRunning)
            {
                //Here the running engine could be running a rerun instead of the normal run,
                //need to talk to Jim as how to distinguish normal or rerun,
                //I might need to set a separate flag for them or a class of flags with individual flag for each situation.
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();
            try
            {
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runRetrieveExampleFileDataMode);
                worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _workerRetrieveData_RunWorkerCompleted;
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                worker2.DoWork += _progressReporter;
                worker2.ProgressChanged += _worker_ProgressChanged;
                worker2.RunWorkerCompleted += _progressReportsDone;
                worker2.WorkerReportsProgress = true;
                worker2.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                object[] parameters = new object[] { filename, fileType };
                worker.RunWorkerAsync(parameters);
                worker2.RunWorkerAsync();
                IsReRunRunning = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void _runRetrieveExampleFileDataMode(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "RetrieveDataThread";
            }
            object[] parameters = e.Argument as object[];
            var filename = parameters[0] as string;
            var type = (int)parameters[1];
            var FileReadingResults = new ReadExampleFileResults();
            IsMatlabEngineRunning = true;
            try
            {
                FileReadingResults.GetSignalsWithData((MWStructArray)_matlabEngine.GetFileExample(filename, type, 0));
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                //MessageBox.Show("Error in running matlab retrieve data mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
                throw new Exception("\nError in running matlab retrieve data mode on background worker thread: " + ex.Message);
            }

            e.Result = FileReadingResults;
        }
     
        public void MMRerun(string selectedStartTime, string selectedEndTime, AWRunViewModel run)
        {
            if (IsMatlabEngineRunning)
            {
                //Here the running engine could be running a rerun instead of the normal run,
                //need to talk to Jim as how to distinguish normal or rerun,
                //I might need to set a separate flag for them or a class of flags with individual flag for each situation.
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();
            Run = run;
            _controlPath = run.Model.ControlRerunPath;
            _configFilePath = run.Model.ConfigFilePath;

            try
            {
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runMMReRunMode);
                worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _workerMMReRun_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                object[] parameters = new object[] { selectedStartTime, selectedEndTime, run.Model.EventPath, run.Model.ControlRerunPath };
                worker.RunWorkerAsync(parameters);
                IsReRunRunning = true;
                Run.IsTaskRunning = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _runMMReRunMode(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "MMReRunThread";
            }
            object[] parameters = e.Argument as object[];
            var start = parameters[0] as string;
            var end = parameters[1] as string;
            var eventPath = parameters[2] as string;
            var controlPath = parameters[3] as string;
            var runFlag = controlPath + "RunFlag.txt";
            if (!System.IO.File.Exists(runFlag))
            {
                System.IO.FileStream fs = System.IO.File.Create(runFlag);
                fs.Close();
            }
            var mmRerunResults = new ModeMeterReRunResults();
            IsMatlabEngineRunning = true;
            try
            {
                mmRerunResults = new ModeMeterReRunResults((MWStructArray)_matlabEngine.ReadMMdata(start, end, eventPath));
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                Run.IsTaskRunning = false;
                IsReRunRunning = false;
                //MessageBox.Show("Error in running matlab mode meter re-run mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
                throw new Exception("Error in running matlab mode meter re-run mode on background worker thread: " + ex.Message);
            }
            e.Result = mmRerunResults.PlotData;
        }
        private void _workerMMReRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsMatlabEngineRunning = false;
            IsReRunRunning = false;
            Run.IsTaskRunning = false;
            OnMMReRunCompletedEvent(e.Result as List<ModeMeterPlotData>);
        }

        public Action<object, List<ModeMeterPlotData>> MMReRunCompletedEvent { get; set; }
        private void OnMMReRunCompletedEvent(List<ModeMeterPlotData> list)
        {
            MMReRunCompletedEvent?.Invoke(this, list);
        }

        public InspectionAnalysisResults InspectionAnalysis(string func, double[][] allData, double[] time, ObservableCollection<SignalSignatureViewModel> signals, InspectionAnalysisParametersViewModel inspectionAnalysisParams)
        {
            InspectionAnalysisResults result = null;
            if (signals.Count > 0)
            {
                MWCellArray signalNames = new MWCellArray(signals.Count);
                MWNumericArray data = new MWNumericArray(allData);
                MWNumericArray t = new MWNumericArray(time);
                var listOfParameters = new List<string>(new string[] { "AnalysisLength", "WindowLength", "Window", "WindowOverlap", "fs", "LogScale", "SigNames" });
                if (inspectionAnalysisParams.ZeroPadding != null)
                {
                    listOfParameters.Add("ZeroPadding");
                }
                if (inspectionAnalysisParams.FreqMin != null)
                {
                    listOfParameters.Add("FreqMin");
                }
                if (inspectionAnalysisParams.FreqMax != null)
                {
                    listOfParameters.Add("FreqMax");
                }
                MWStructArray parameters = new MWStructArray(1, 1, listOfParameters.ToArray());
                try
                {
                    for (int index = 0; index < signals.Count; index++)
                    {
                        signalNames[index + 1] = new MWCharArray(signals[index].SignalName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                parameters["SigNames", 1] = signalNames;
                parameters["AnalysisLength", 1] = inspectionAnalysisParams.AnalysisLength;
                parameters["WindowLength", 1] = inspectionAnalysisParams.WindowLength;
                parameters["Window", 1] = inspectionAnalysisParams.WindowType.ToString();
                parameters["WindowOverlap", 1] = inspectionAnalysisParams.WindowOverlap;
                if (inspectionAnalysisParams.ZeroPadding != null)
                {
                    parameters["ZeroPadding", 1] = (int)inspectionAnalysisParams.ZeroPadding;
                }
                parameters["LogScale", 1] = inspectionAnalysisParams.LogScale.ToString().ToUpper();
                if (inspectionAnalysisParams.FreqMin != null)
                {
                    parameters["FreqMin", 1] = (double)inspectionAnalysisParams.FreqMin;
                }
                if (inspectionAnalysisParams.FreqMax != null)
                {
                    parameters["FreqMax", 1] = (double)inspectionAnalysisParams.FreqMax;
                }
                parameters["fs", 1] = signals[0].SamplingRate;

                if (IsMatlabEngineRunning)
                {
                    PauseMatlabNormalRun();
                }
                IsMatlabEngineRunning = true;
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Run.IsTaskRunning = true;
                try
                {
                    result = new InspectionAnalysisResults((MWStructArray)_matlabEngine.InspectionAnalysis(func, data, t, parameters));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK);
                }
                Run.IsTaskRunning = false;
                Mouse.OverrideCursor = null;
                IsMatlabEngineRunning = false;
            }
            return result;
        }
        public void UpdateOBATPreset(string newPresetName, string detectorName, string configFilePath, string oBATPresetFilePath)
        {
            if (IsMatlabEngineRunning)
            {
                PauseMatlabNormalRun();
            }
            IsMatlabEngineRunning = true;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Run.IsTaskRunning = true;
            // save a copy of the OBAT preset file
            var bckOBATFile = Path.GetDirectoryName(oBATPresetFilePath) + "\\backup." + Path.GetExtension(oBATPresetFilePath);
            //var number = 1;
            //while (File.Exists(bckOBATFile))
            //{
            //    bckOBATFile = Path.GetDirectoryName(oBATPresetFilePath) + "\\backup" + number.ToString() + "." + Path.GetExtension(oBATPresetFilePath);
            //    number++;
            //}
            try
            {
                File.Copy(oBATPresetFilePath, bckOBATFile, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var updateStatus = "";
            try
            {
                updateStatus = _matlabEngine.UpdateOBATpreset(newPresetName, detectorName, configFilePath, oBATPresetFilePath).ToString();
            }
            catch (Exception ex)
            {
                //copy the copied OBAT file back to it's original PATH
                File.Copy(bckOBATFile, oBATPresetFilePath, true);
                //delete the copied OBAT prest file.
                File.Delete(bckOBATFile);
                Run.IsTaskRunning = false;
                Mouse.OverrideCursor = null;
                IsMatlabEngineRunning = false;
                throw ex;
            }
            //need to show the returned string updateSuccess here as a message.
            MessageBox.Show(updateStatus.ToString(), "Done", MessageBoxButtons.OK);
            if (updateStatus.ToString() == "Success") //if success
            {
                //delete the copied OBAT prest file.
                File.Delete(bckOBATFile);
            }
            else
            {
                //copy the copied OBAT file back to it's original PATH
                File.Copy(bckOBATFile, oBATPresetFilePath, true);
                //delete the copied OBAT prest file.
                File.Delete(bckOBATFile);
            }
            Run.IsTaskRunning = false;
            Mouse.OverrideCursor = null;
            IsMatlabEngineRunning = false;
        }

        public void GenerateMMreport(string start, string end, string eventPath, string reportType, int dampThresh, int eventSepMinutes, string reportPath)
        {
            if (IsMatlabEngineRunning)
            {
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();

            try
            {
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runGenerateMMReport);
                //worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _workerGenerateMMreport_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                object[] parameters = new object[] { start, end, eventPath, reportType, dampThresh, eventSepMinutes, reportPath };
                worker.RunWorkerAsync(parameters);
                IsReRunRunning = true;
                Run.IsTaskRunning = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _runGenerateMMReport(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "GenerateMMreportThread";
            }
            object[] parameters = e.Argument as object[];
            var start = parameters[0] as string;
            var end = parameters[1] as string;
            var eventPath = parameters[2] as string;
            var reportType = parameters[3] as string;
            var dampThresh = (int)parameters[4];
            var eventSepMinutes = (int)parameters[5];
            var reportPath = parameters[6] as string;
            IsMatlabEngineRunning = true;
            try
            {
                var result = _matlabEngine.WriteMMreport(start, end, eventPath, reportType, dampThresh, eventSepMinutes, reportPath);
                MessageBox.Show(result.ToString(), "", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                IsReRunRunning = false;
                Run.IsTaskRunning = false;
                //MessageBox.Show("Error in running matlab retrieve data mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
                throw new Exception("\nError in running matlab retrieve data mode on background worker thread: " + ex.Message);
            }
        }

        private void _workerGenerateMMreport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsMatlabEngineRunning = false;
            IsReRunRunning = false;
            Run.IsTaskRunning = false;
        }

        public void GetDBExampleSignals(string starttime, string mnemonic, string exampleFile, string dbtype)
        {
            if (IsMatlabEngineRunning)
            {
                //Here the running engine could be running a rerun instead of the normal run,
                //need to talk to Jim as how to distinguish normal or rerun,
                //I might need to set a separate flag for them or a class of flags with individual flag for each situation.
                PauseMatlabNormalRun();
            }
            worker = new BackgroundWorker();
            try
            {
                var start = Convert.ToDateTime(starttime, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
                worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_runRetrieveDBDataMode);
                worker.ProgressChanged += _worker_ProgressChanged;
                worker.RunWorkerCompleted += _workerRetrieveData_RunWorkerCompleted;
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                worker2.DoWork += _progressReporter;
                worker2.ProgressChanged += _worker_ProgressChanged;
                worker2.RunWorkerCompleted += _progressReportsDone;
                worker2.WorkerReportsProgress = true;
                worker2.WorkerSupportsCancellation = true;
                while (worker.IsBusy)
                {
                    Thread.Sleep(500);
                }
                object[] parameters = new object[] { start, mnemonic, exampleFile, dbtype };
                worker.RunWorkerAsync(parameters);
                worker2.RunWorkerAsync();
                IsReRunRunning = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _runRetrieveDBDataMode(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "RetrieveDataThread";
            }
            object[] parameters = e.Argument as object[];
            var starttime = parameters[0] as string;
            var preset = parameters[1] as string;
            var filename = parameters[2] as string;
            var dbtype = parameters[3] as string;
            var FileReadingResults = new ReadExampleFileResults();
            IsMatlabEngineRunning = true;
            try
            {
                if (dbtype == "PI")
                {
                    FileReadingResults.GetSignalsWithData((MWStructArray)_matlabEngine.GetFileExampleDB(starttime, preset, filename, 0, "PI"));
                }
                else if (dbtype == "openHistorian")
                {
                    FileReadingResults.GetSignalsWithData((MWStructArray)_matlabEngine.GetFileExampleDB(starttime, preset, filename, 0, "OpenHistorian"));
                }
            }
            catch (Exception ex)
            {
                IsMatlabEngineRunning = false;
                IsReRunRunning = false;
                //MessageBox.Show("\nError in running matlab retrieve data mode on background worker thread: " + ex.Message, "Error!", MessageBoxButtons.OK);
                throw new Exception("\nError in running matlab retrieve data mode on background worker thread: " + ex.Message);
            }

            e.Result = FileReadingResults;
        }
    }
}
