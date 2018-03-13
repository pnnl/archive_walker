using BAWGUI.RunMATLAB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.ViewModels
{
    public class ProjectsControlViewModel
    {
        public List<AWProject> AWProjects = new List<AWProject>();
        public string ResultsStoragePath;
    }
}
