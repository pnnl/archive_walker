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
            IsProjectEnabled = true;
        }

        public AWProject(string dir)
        {
            _projectpath = dir + "\\";
            _projectName = Path.GetFileName(dir).Split(new[] { '_' }, 2)[1];
            _awRun = new List<AWRun>();
            foreach (var run in Directory.GetDirectories(dir))
            {
                var runNameFrac = Path.GetFileName(run).Split(new[] { '_' }, 2);
                if (runNameFrac[0] == "Run" && Directory.Exists(run))
                {
                    _awRun.Add(new AWRun(run));
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
        public bool IsProjectEnabled { get; set; }
    }
}
