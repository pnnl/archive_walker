using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.RunMATLAB.Models
{
    public class AWRun
    {
        public string RunName { get; set; }
        public string ConfigFilePath { get; set; }
        public string ConfigFilename { get; set; }
        public string EventPath { get; set; }
        public string InitializationPath { get; set; }
        public string ControlRunPath { get; set; }
        public string ControlRerunPath { get; set; }
    }
}
