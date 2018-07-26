using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeMeter.Models
{
    public class SmallSignalStabilityTool
    {
        public List<Mode> Modes { get; set; }
        public List<SignalSignatures> BaseliningSignals { get; set; }
        public string ResultPath { get; set; }
        public string ModeMeterName { get; set; }
    }
}
