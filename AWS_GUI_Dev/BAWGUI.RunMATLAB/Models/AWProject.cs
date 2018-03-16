using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class AWProject
    {
        private string _projectName;
        public string ProjectName
        {
            get { return _projectpath; }
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

        public AWProject(string dir)
        {
            _projectpath = dir;
            _projectpath = Path.GetFileName(dir).Split('_')[1];
            _awRun = new List<AWRun>();
            foreach (var run in Directory.GetDirectories(dir))
            {
                var runNameFrac = Path.GetFileName(run).Split('_');
                if (runNameFrac[0] == "Run" && Directory.Exists(run))
                {
                    _awRun.Add(new AWRun(run));
                }
            }
        }
    }
}
