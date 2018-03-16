using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class AWRun
    {
        public AWRun(string run)
        {
            this._runPath = run;
            _runName = Path.GetFileName(run).Split('_')[1];
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
                    _eventPath = dir;
                }
                if (Path.GetFileName(dir) == "Init")
                {
                    _initializationPath = dir;
                }
                if (Path.GetFileName(dir) == "ControlRun")
                {
                    _controlRunPath = dir;
                }
                if (Path.GetFileName(dir) == "ControlRerun")
                {
                    _controlRerunPath = dir;
                }
            }
        }
        private string _runPath;
        public string RunPath { get { return _runPath; } }
        private string _runName;
        public string RunName { get { return _runName; } }
        private string _configFilePath;
        public string ConfigFilePath { get { return _configFilePath; } }
        private string _eventPath;
        public string EventPath { get { return _eventPath; } }
        private string _initializationPath;
        public string InitializationPath { get { return _initializationPath; } }
        private string _controlRunPath;
        public string ControlRunPath { get { return _controlRunPath; } }
        private string _controlRerunPath;
        public string ControlRerunPath { get { return _controlRerunPath; } }
    }
}
