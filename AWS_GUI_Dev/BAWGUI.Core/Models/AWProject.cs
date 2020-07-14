using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core
{
    public class AWProject
    {
        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
        }
        private string _projectpath;
        public string Projectpath
        {
            get {return _projectpath; }
        }

        private List<AWRun> _awRun;
        public List<AWRun> AWRuns
        {
            get { return _awRun; }
        }

        public AWProject()
        {
            _projectName = "";
            _projectpath = "";
            _awRun = new List<AWRun>();
            _isSelected = false;
            IsEnabled = true;
        }

        public AWProject(string dir)
        {
            _projectpath = dir + "\\";
            _projectName = Path.GetFileName(dir).Split(new[] { '_' }, 2)[1];
            _awRun = new List<AWRun>();
            var dirs = new DirectoryInfo(dir).GetDirectories().OrderBy(x => x.CreationTime).ToList();
            foreach (var run in dirs)
            {
                var dirName = run.FullName;
                var runNameFrac = Path.GetFileName(dirName).Split(new[] { '_' }, 2);
                //var newName = "";
                //if (runNameFrac[0] == "Run" && Directory.Exists(run))
                //{
                //    newName = Path.GetDirectoryName(run) + "\\Task_" + runNameFrac[1];
                //    Directory.Move(run, newName);
                //}
                //runNameFrac = Path.GetFileName(newName).Split(new[] { '_' }, 2);
                if (runNameFrac[0] == "Task" && Directory.Exists(dirName))
                {
                    _awRun.Add(new AWRun(dirName));
                }
            }
            _isSelected = false;
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
            }
        }
        public bool IsEnabled { get; set; }
    }
}
