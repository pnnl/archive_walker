using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class AWProject
    {
        public string ProjectName;
        public string Projectpath;
        public List<AWRun> AWRuns = new List<AWRun>();
    }
}
