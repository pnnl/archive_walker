using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltageStability.Models
{
    public class Site
    {
        public Site()
        {
            Frequency = new Signal();
            VoltageBuses = new List<VoltageBus>();
            //VoltageBuses.Add(new VoltageBus());
            BranchesAndShunts = new List<object>();
            //BranchesAndShunts.Add(new Branch());
        }
        public string Name { get; set; }
        public string StabilityThreshold { get; set; }
        public Signal Frequency { get; set; }
        public List<VoltageBus> VoltageBuses { get; set; }
        public List<object> BranchesAndShunts { get; set; }
    }
    public class VoltageBus
    {
        public VoltageBus()
        {
            Magnitude = new Signal();
            Angle = new Signal();
        }

        public VoltageBus(Signal s1, Signal s2)
        {
            Magnitude = s1;
            Angle = s2;
        }
        public Signal Magnitude { get; set; }
        public Signal Angle { get; set; }
        public string Name { get { return "Voltage Bus"; } }
    }
    public class Branch
    {
        public Branch()
        {
            ActivePower = new Signal();
            ReactivePower = new Signal();
            CurrentMagnitude = new Signal();
            CurrentAngle = new Signal();
        }
        public Signal ActivePower { get; set; }
        public Signal ReactivePower { get; set; }
        public Signal CurrentMagnitude { get; set; }
        public Signal CurrentAngle { get; set; }
        public string Name { get { return "Branch"; } }
    }
    public class Shunt : Branch
    {
        public new string Name { get { return "Shunt"; } }
    }
}
