﻿using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeMeter.Models
{
    public class SmallSignalStabilityTool
    {
        public SmallSignalStabilityTool()
        {
            Modes = new List<Mode>();
            BaseliningSignals = new List<SignalSignatures>();
            CalcDEF = false;
        }
        public string Name
        {
            get
            {
                return "Mode Meter Tool";
            }
        }
        public List<Mode> Modes { get; set; }
        public List<SignalSignatures> BaseliningSignals { get; set; }
        public string ResultPath { get; set; }
        public string ModeMeterName { get; set; }
        public bool CalcDEF { get; set; }
    }
}
