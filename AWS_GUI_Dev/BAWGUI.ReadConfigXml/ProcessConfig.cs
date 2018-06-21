using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.ComponentModel;

namespace BAWGUI.ReadConfigXml
{
    public class ProcessConfig
    {
        private XElement _xElement;

        public ProcessConfig(XElement xElement)
        {
            this._xElement = xElement;
            var par = xElement.Element("Configuration").Element("InitializationPath").Value;
            if (par != null)
            {
                InitializationPath = par;
            }
            var unWraps = xElement.Element("Configuration").Element("Processing").Elements("Unwrap");
            UnWrapList = new List<Unwrap>();
            foreach (var unwrap in unWraps)
            {
                UnWrapList.Add(new Unwrap(unwrap));
            }
            var interpolates = xElement.Element("Configuration").Element("Processing").Elements("Interpolate");
            InterpolateList = new List<Interpolate>();
            foreach (var interplt in interpolates)
            {
                InterpolateList.Add(new Interpolate(interplt));
            }
            var stages = xElement.Element("Configuration").Element("Processing").Elements("Stages");
            CollectionOfSteps = new List<object>();
            foreach (var stage in stages)
            {
                var steps = stage.Elements();
                foreach (var step in steps)
                {
                    if (step.Name == "Filter")
                    {
                        CollectionOfSteps.Add(new TunableFilter(step));
                    }
                    else
                    {
                        CollectionOfSteps.Add(new Multirate(step));
                    }
                }
            }
            var wraps = xElement.Element("Configuration").Element("Processing").Elements("Wrap");
            WrapList = new List<Wrap>();
            foreach (var wrap in wraps)
            {
                WrapList.Add(new Wrap(wrap));

            }
            var ntu = xElement.Element("Configuration").Element("NameTypeUnit").Elements("PMU");
            NameTypeUnitList = new NameTypeUnit();
            foreach (var pmu in ntu)
            {
                NameTypeUnitList.NameTypeUnitPMUList.Add(new NameTypeUnitPMU(pmu));
            }
        }

        public string InitializationPath { get; set; }
        public List<Unwrap> UnWrapList { get; set; }
        public List<Interpolate> InterpolateList { get; set; }
        public List<object> CollectionOfSteps { get; set; }
        public List<Wrap> WrapList { get; set; }
        public NameTypeUnit NameTypeUnitList { get; set; }
    }

    public class Unwrap
    {
        private XElement _item;
        public Unwrap(XElement item)
        {
            Name = "Unwrap";
            this._item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(_item);
        }
        public string Name { get; set; }
        public List<PMUElement> PMUElementList { get; set; }
    }
    public class Interpolate : Unwrap
    {
        public Interpolate(XElement intplt) : base(intplt)
        {
            base.Name = "Interpolation";
            var par = intplt.Element("Parameters").Element("Limit");
            if (par != null)
            {
                Limit = par.Value;
            }
            var tp = intplt.Element("Parameters").Element("Type");
            if (tp != null)
            {
                Type = (InterpolateType)Enum.Parse(typeof(InterpolateType), tp.Value);
            }
            par = intplt.Element("Parameters").Element("FlagInterp");
            if (par != null)
            {
                if (par.Value.ToLower() == "true")
                {
                    FlagInterp = true;
                }
                else
                {
                    FlagInterp = false;
                }
            }
        }

        public string Limit { get; set; }
        public InterpolateType Type { get; set; }
        public bool FlagInterp { get; set; }
    }
    public enum InterpolateType
    {
        [System.ComponentModel.Description("Linear")]
        Linear,
        [Description("Constant")]
        Constant
    }
    public class TunableFilter : Unwrap
    {
        public TunableFilter(XElement filter) : base(filter)
        {
            base.Name = "Filter";
            var par = filter.Element("Type");
            if (par != null)
            {
                Type = (TunableFilterType)Enum.Parse(typeof(TunableFilterType), par.Value);
            }
            switch (Type)
            {
                case TunableFilterType.Rational:
                    var value = filter.Element("Parameters").Element("Numerator");
                    if (value != null)
                    {
                        Numerator = value.Value;
                    }
                    value = filter.Element("Parameters").Element("Denominator");
                    if (value != null)
                    {
                        Denominator = value.Value;
                    }
                    break;
                case TunableFilterType.HighPass:
                    value = filter.Element("Parameters").Element("Order");
                    if (value != null)
                    {
                        Order = value.Value;
                    }
                    value = filter.Element("Parameters").Element("Cutoff");
                    if (value != null)
                    {
                        Cutoff = value.Value;
                    }
                    break;
                case TunableFilterType.LowPass:
                    value = filter.Element("Parameters").Element("PassRipple");
                    if (value != null)
                    {
                        PassRipple = value.Value;
                    }
                    value = filter.Element("Parameters").Element("StopRipple");
                    if (value != null)
                    {
                        StopRipple = value.Value;
                    }
                    value = filter.Element("Parameters").Element("PassCutoff");
                    if (value != null)
                    {
                        PassCutoff = value.Value;
                    }
                    value = filter.Element("Parameters").Element("StopCutoff");
                    if (value != null)
                    {
                        StopCutoff = value.Value;
                    }
                    break;
                default:
                    throw new Exception("Unknow tunable filter type!");
            }
        }
        public TunableFilterType Type { get; set; }
        public string Numerator { get; set; }
        public string Denominator { get; set; }
        public string Order { get; set; }
        public string Cutoff { get; set; }
        public string PassRipple { get; set; }
        public string StopRipple { get; set; }
        public string PassCutoff { get; set; }
        public string StopCutoff { get; set; }
    }
    public enum TunableFilterType
    {
        Rational,
        [Description("High-Pass")]
        HighPass,
        [Description("Low-Pass")]
        LowPass
    }
    public class Multirate : Unwrap
    {
        public Multirate(XElement mRate) : base(mRate)
        {
            base.Name = "Multirate";
            FilterChoice = 0;
            var par = mRate.Element("Parameters").Element("MultiRatePMU");
            if (par != null)
            {
                MultiRatePMU = par.Value;
            }
            par = mRate.Element("Parameters").Element("NewRate");
            if (par != null)
            {
                NewRate = par.Value;
                FilterChoice = 1;
            }
            else
            {
                par = mRate.Element("Parameters").Element("p");
                if (par != null)
                {
                    PElement = par.Value;
                }
                par = mRate.Element("Parameters").Element("q");
                if (par != null)
                {
                    QElement = par.Value;
                    FilterChoice = 2;
                }
            }
        }
        public string MultiRatePMU { get; set; }
        public string NewRate { get; set; }
        public string PElement { get; set; }
        public string QElement { get; set; }
        public int FilterChoice { get; set; }
    }
    public class Wrap : Unwrap
    {
        public Wrap(XElement wrap) : base(wrap)
        {
            base.Name = "Wrap";
        }
    }
    public class NameTypeUnit
    {
        public NameTypeUnit()
        {
            Name = "Signal Type and Unit Specification";
            NameTypeUnitPMUList = new List<NameTypeUnitPMU>();
        }
        public string Name { get; set; }
        public List<NameTypeUnitPMU> NameTypeUnitPMUList { get; set; }
    }
    public class NameTypeUnitPMU
    {
        private XElement pmu;

        public NameTypeUnitPMU(XElement pmu)
        {
            this.pmu = pmu;
            var par = pmu.Element("NewChannel");
            if (par != null)
            {
                NewChannel = par.Value;
            }
            par = pmu.Element("NewUnit");
            if (par != null)
            {
                NewUnit = par.Value;
            }
            par = pmu.Element("NewType");
            if (par != null)
            {
                NewType = par.Value;
            }
            Input = new PMUElement(pmu.Element("Name").Value, pmu.Element("CurrentChannel").Value);
        }

        public PMUElement Input { get; set; }
        public string NewChannel { get; set; }
        public string NewUnit { get; set; }
        public string NewType { get; set; }
    }

}