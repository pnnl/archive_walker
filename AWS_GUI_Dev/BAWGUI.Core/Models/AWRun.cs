using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core
{
    public class AWRun
    {
        public AWRun()
        {
            _runPath = "";
            _runName = "";
            _configFilePath = "";
            _eventPath = "";
            _initializationPath = "";
            _controlRunPath = "";
            _controlRerunPath = "";
            _isSelected = false;
            _isRunPaused = false;
        }
        public AWRun(string run)
        {
            this._runPath = run;
            _runName = Path.GetFileName(run).Split(new[] { '_' }, 2)[1];
            foreach (var file in Directory.GetFiles(run))
            {
                if (Path.GetFileNameWithoutExtension(file) == "Config" && Path.GetExtension(file).ToLower() == ".xml")
                {
                    _configFilePath = file;
                }
            }
            foreach (var dir in Directory.GetDirectories(run))
            {
                if (Path.GetFileName(dir) == "Event")
                {
                    _eventPath = dir + "\\";
                }
                if (Path.GetFileName(dir) == "Init")
                {
                    _initializationPath = dir + "\\";
                }
                if (Path.GetFileName(dir) == "ControlRun")
                {
                    _controlRunPath = dir + "\\";
                    var runFlag = _controlRunPath + "RunFlag.txt";
                    var pauseFlag = _controlRunPath + "PauseFlag.txt";
                    if (!System.IO.File.Exists(runFlag) && System.IO.File.Exists(pauseFlag) && System.IO.File.Exists(_controlRunPath + "PauseData.mat"))
                    {
                        _isRunPaused = true;
                    }
                    else
                    {
                        _isRunPaused = false;
                    }
                }
                if (Path.GetFileName(dir) == "ControlRerun")
                {
                    _controlRerunPath = dir + "\\";
                }
            }
            _isSelected = false;
        }
        private string _runPath;
        public string RunPath { get { return _runPath; } set { _runPath = value; } }
        private string _runName;
        public string RunName { get { return _runName; } set { _runName = value; } }
        private string _configFilePath;
        public string ConfigFilePath { get { return _configFilePath; } set { _configFilePath = value; } }
        private string _eventPath;
        public string EventPath { get { return _eventPath; } set { _eventPath = value; } }
        private string _initializationPath;
        public string InitializationPath { get { return _initializationPath; } set { _initializationPath = value; } }
        private string _controlRunPath;
        public string ControlRunPath { get { return _controlRunPath; } set { _controlRunPath = value; } }
        private string _controlRerunPath;
        public string ControlRerunPath { get { return _controlRerunPath; } set { _controlRerunPath = value; } }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
            }
        }
        private bool _isRunPaused;
        public bool IsRunPaused
        {
            get { return _isRunPaused; }
            set
            {
                _isRunPaused = value;
            }
        }
    }
}
