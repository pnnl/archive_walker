﻿using System.Xml.Linq;
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
using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Core.Converters;

namespace BAWGUI.ReadConfigXml
{
    public class ProcessConfigModel
    {
        private XElement _xElement;

        public ProcessConfigModel(XElement xElement)
        {
            this._xElement = xElement;
            //var par = xElement.Element("Configuration").Element("InitializationPath").Value;
            //if (par != null)
            //{
            //    InitializationPath = par;
            //}
            var unWraps = xElement.Element("Configuration").Element("Processing").Elements("Unwrap");
            UnWrapList = new List<UnwrapModel>();
            foreach (var unwrap in unWraps)
            {
                UnWrapList.Add(new UnwrapModel(unwrap));
            }
            var interpolates = xElement.Element("Configuration").Element("Processing").Elements("Interpolate");
            InterpolateList = new List<InterpolateModel>();
            foreach (var interplt in interpolates)
            {
                InterpolateList.Add(new InterpolateModel(interplt));
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
                        CollectionOfSteps.Add(new TunableFilterModel(step));
                    }
                    else
                    {
                        CollectionOfSteps.Add(new MultirateModel(step));
                    }
                }
            }
            var wraps = xElement.Element("Configuration").Element("Processing").Elements("Wrap");
            WrapList = new List<WrapModel>();
            foreach (var wrap in wraps)
            {
                WrapList.Add(new WrapModel(wrap));

            }
            var ntu = xElement.Element("Configuration").Element("NameTypeUnit").Elements("PMU");
            NameTypeUnitList = new NameTypeUnitModel();
            foreach (var pmu in ntu)
            {
                NameTypeUnitList.NameTypeUnitPMUList.Add(new NameTypeUnitPMUModel(pmu));
            }
        }

        public string InitializationPath { get; set; }
        public List<UnwrapModel> UnWrapList { get; set; }
        public List<InterpolateModel> InterpolateList { get; set; }
        public List<object> CollectionOfSteps { get; set; }
        public List<WrapModel> WrapList { get; set; }
        public NameTypeUnitModel NameTypeUnitList { get; set; }
    }

    public class UnwrapModel
    {
        private XElement _item;

        public UnwrapModel()
        {
        }

        public UnwrapModel(XElement item)
        {
            //Name = "Unwrap";
            this._item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(_item);
        }
        public string Name
        {
            get { return "Unwrap"; }
        }
        public List<SignalSignatures> PMUElementList { get; set; }
    }
    public class InterpolateModel : UnwrapModel
    {
        public InterpolateModel() { }
        public InterpolateModel(XElement intplt) : base(intplt)
        {
            //base.Name = "Interpolation";
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

        public new string Name
        {
            get { return "Interpolation"; }
        }
        public string Limit { get; set; }
        public InterpolateType Type { get; set; }
        public bool FlagInterp { get; set; }
    }
    //public enum InterpolateType
    //{
    //    [System.ComponentModel.Description("Linear")]
    //    Linear,
    //    [Description("Constant")]
    //    Constant
    //}
    public class TunableFilterModel
    {
        public TunableFilterModel()
        {
            PointOnWavePowerCalculationFilterParam = new PointOnWavePowerCalculationFilterParameters();
            POWPMUFilterParameters = new PointOnWavePMUFilterParameters();
            PMUElementList = new List<PMUElementForUnaryCustModel>();
            ReturnABCPhases = false;
            ReturnPositiveSequence = false;
            CalculateFandROCOF = false;
        }
        public TunableFilterModel(XElement filter) : this()
        {
            //base.Name = "Filter";
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
                case TunableFilterType.FrequencyDerivation:
                    break;
                case TunableFilterType.RunningAverage:
                    value = filter.Element("Parameters").Element("RemoveAve");
                    if (value != null)
                    {
                        RemoveAve = bool.Parse(value.Value);
                    }
                    value = filter.Element("Parameters").Element("WindowLength");
                    if (value != null)
                    {
                        WindowLength = value.Value;
                    }
                    break;
                case TunableFilterType.PointOnWavePower:
                    value = filter.Element("Parameters").Element("Pname");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.Pname = value.Value;
                    }
                    value = filter.Element("Parameters").Element("Qname");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.Qname = value.Value;
                    }
                    value = filter.Element("Parameters").Element("Fname");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.Fname = value.Value;
                    }
                    value = filter.Element("Parameters").Element("WindowLength");
                    if (value != null)
                    {
                        WindowLength = value.Value;
                    }
                    value = filter.Element("Parameters").Element("VA");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.VA = value.Value;
                    }
                    value = filter.Element("Parameters").Element("VB");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.VB = value.Value;
                    }
                    value = filter.Element("Parameters").Element("VC");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.VC = value.Value;
                    }
                    value = filter.Element("Parameters").Element("IA");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.IA = value.Value;
                    }
                    value = filter.Element("Parameters").Element("IB");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.IB = value.Value;
                    }
                    value = filter.Element("Parameters").Element("IC");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.IC = value.Value;
                    }
                    value = filter.Element("Parameters").Element("PhaseShiftV");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.PhaseShiftV = value.Value;
                    }
                    value = filter.Element("Parameters").Element("PhaseShiftI");
                    if (value != null)
                    {
                        PointOnWavePowerCalculationFilterParam.PhaseShiftI = value.Value;
                    }
                    break;
                case TunableFilterType.RMSenergyFilt:
                    value = filter.Element("Parameters").Element("Band");
                    if (value != null)
                    {
                        BandType = EnumExtencsionMethod.GetValueFromDescription<RMSEnergyBandOptions>(value.Value);
                    }
                    break;
                case TunableFilterType.POWpmuFilt:
                    value = filter.Element("Parameters").Element("PmagName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.PmagName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("PangName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.PangName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("AmagName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.AmagName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("AangName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.AangName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("AfitName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.AfitName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("BmagName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.BmagName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("BangName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.BangName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("BfitName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.BfitName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("CmagName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.CmagName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("CangName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.CangName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("CfitName");
                    if (value != null)
                    {
                        POWPMUFilterParameters.CfitName = value.Value;
                    }
                    value = filter.Element("Parameters").Element("Fname");
                    if (value != null)
                    {
                        POWPMUFilterParameters.Fname = value.Value;
                    }
                    value = filter.Element("Parameters").Element("ROCOFname");
                    if (value != null)
                    {
                        POWPMUFilterParameters.ROCOFname = value.Value;
                    }
                    double v = 0d;
                    value = filter.Element("Parameters").Element("ReportRate");
                    if (value != null && double.TryParse(value.Value, out v))
                    {
                        POWPMUFilterParameters.ReportRate = v;
                    }
                    value = filter.Element("Parameters").Element("WinLength");
                    if (value != null && double.TryParse(value.Value, out v))
                    {
                        POWPMUFilterParameters.WinLength = v;
                    }
                    value = filter.Element("Parameters").Element("SynchFreq");
                    if (value != null && double.TryParse(value.Value, out v))
                    {
                        POWPMUFilterParameters.SynchFreq = v;
                    }
                    value = filter.Element("Parameters").Element("PA");
                    if (value != null)
                    {
                        POWPMUFilterParameters.PA = value.Value;
                    }
                    value = filter.Element("Parameters").Element("PB");
                    if (value != null)
                    {
                        POWPMUFilterParameters.PB = value.Value;
                    }
                    value = filter.Element("Parameters").Element("PC");
                    if (value != null)
                    {
                        POWPMUFilterParameters.PC = value.Value;
                    }
                    value = filter.Element("InputType");
                    if (value != null)
                    {
                        if (value.Value.ToUpper() == "VOLTAGE")
                        {
                            PMUFilterInputType = POWPMUFilterInputType.Voltage;
                        }
                        else if (value.Value.ToUpper() == "CURRENT")
                        {
                            PMUFilterInputType = POWPMUFilterInputType.Current;
                        }
                        else
                        {
                        }
                    }
                    value = filter.Element("ReturnABCPhase");
                    if (value != null)
                    {
                        if (value.Value.ToUpper() == "TRUE")
                        {
                            ReturnABCPhases = true;
                        }
                        else
                        {
                            ReturnABCPhases = false;
                        }
                    }
                    value = filter.Element("ReturnPositiveSequence");
                    if (value != null)
                    {
                        if (value.Value.ToUpper() == "TRUE")
                        {
                            ReturnPositiveSequence = true;
                        }
                        else
                        {
                            ReturnPositiveSequence = false;
                        }
                    }
                    value = filter.Element("CalculateFandROCOF");
                    if (value != null)
                    {
                        if (value.Value.ToUpper() == "TRUE")
                        {
                            CalculateFandROCOF = true;
                        }
                        else
                        {
                            CalculateFandROCOF = false;
                        }
                    }
                    break;
                default:
                    throw new Exception("Unknow tunable filter type!");
            }
            par = filter.Element("CustPMU");
            if (par != null)
            {
                CustPMUName = par.Value;
                UseCustomPMU = true;
                OutputSignalStorage = OutputSignalStorageType.CreateCustomPMU;
            }
            else
            {
                UseCustomPMU = false;
                OutputSignalStorage = OutputSignalStorageType.ReplaceInput;
            }
            var pmus = filter.Elements("PMU");
            //PMUElementList = new List<PMUElementForUnaryCustModel>();
            foreach (var pmu in pmus)
            {
                var pmuName = pmu.Element("Name").Value;
                var channels = pmu.Elements("Channel");
                if (channels.Count() > 0)
                {
                    foreach (var channel in channels)
                    {
                        var channelName = channel.Element("Name").Value;
                        var custname = "";
                        if (UseCustomPMU && Type != TunableFilterType.PointOnWavePower && Type != TunableFilterType.POWpmuFilt)
                        {
                            custname = channel.Element("CustName").Value;
                        }
                        var newSignal = new PMUElementForUnaryCustModel(pmuName, channelName, custname);
                        PMUElementList.Add(newSignal);
                    }
                }
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
        public bool RemoveAve { get; set; }
        public string WindowLength { get; set; }
        public string Name
        {
            get { return "Filter"; }
        }
        public bool UseCustomPMU { get; set; }
        public OutputSignalStorageType OutputSignalStorage { get; set; }
        public string CustPMUName { get; set; }
        public List<PMUElementForUnaryCustModel> PMUElementList { get; set; }
        public PointOnWavePowerCalculationFilterParameters PointOnWavePowerCalculationFilterParam { get; set; }
        public RMSEnergyBandOptions BandType { get; set; }
        public PointOnWavePMUFilterParameters POWPMUFilterParameters { get; set; }
        public POWPMUFilterInputType PMUFilterInputType { get; set; }
        public bool ReturnABCPhases { get; set; }
        public bool ReturnPositiveSequence { get; set; }
        public bool CalculateFandROCOF { get; set; }
        //public string Pname { get; private set; }
        //public string Qname { get; private set; }
        //public string VA { get; private set; }
        //public string IC { get; private set; }
        //public string IB { get; private set; }
        //public string IA { get; private set; }
        //public string VC { get; private set; }
        //public string VB { get; private set; }
        //public string PhaseShiftV { get; private set; }
        //public string PhaseShiftI { get; private set; }
    }
    //public enum TunableFilterType
    //{
    //    Rational,
    //    [Description("High-Pass")]
    //    HighPass,
    //    [Description("Low-Pass")]
    //    LowPass
    //}
    public class MultirateModel : UnwrapModel
    {
        public MultirateModel() { }
        public MultirateModel(XElement mRate) : base(mRate)
        {
            //base.Name = "Multirate";
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
        public new string Name
        {
            get { return "Multirate"; }
        }
    }
    public class WrapModel : UnwrapModel
    {
        public WrapModel() { }
        public WrapModel(XElement wrap) : base(wrap)
        {
            //base.Name = "Wrap";
        }
        public new string Name
        {
            get { return "Wrap"; }
        }
    }
    public class NameTypeUnitModel
    {
        //public NameTypeUnitModel() { }
        public NameTypeUnitModel()
        {
            Name = "Signal Type and Unit Specification";
            NameTypeUnitPMUList = new List<NameTypeUnitPMUModel>();
        }
        public string Name { get; set; }
        public List<NameTypeUnitPMUModel> NameTypeUnitPMUList { get; set; }
    }
    public class NameTypeUnitPMUModel
    {
        public NameTypeUnitPMUModel() { }
        private XElement pmu;

        public NameTypeUnitPMUModel(XElement pmu)
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
            Input = new SignalSignatures(pmu.Element("Name").Value, pmu.Element("CurrentChannel").Value);
        }

        public SignalSignatures Input { get; set; }
        public string NewChannel { get; set; }
        public string NewUnit { get; set; }
        public string NewType { get; set; }
    }
    public class PointOnWavePowerCalculationFilterParameters
    {
        public string Pname { get; set; }
        public string Qname { get; set; }
        public string Fname { get; set; }
        public string VA { get; set; }
        public string VB { get; set; }
        public string VC { get; set; }
        public string IA { get; set; }
        public string IB { get; set; }
        public string IC { get; set; }
        public string PhaseShiftV { get; set; }
        public string PhaseShiftI { get; set; }
    }
    public class PointOnWavePMUFilterParameters
    {
        public PointOnWavePMUFilterParameters()
        {
            SynchFreq = 60;
            ReportRate = 30;
        }
        public string PmagName { get; set; }
        public string PangName { get; set; }
        public string AmagName { get; set; }
        public string AangName { get; set; }
        public string AfitName { get; set; }
        public string BmagName { get; set; }
        public string BangName { get; set; }
        public string BfitName { get; set; }
        public string CmagName { get; set; }
        public string CangName { get; set; }
        public string CfitName { get; set; }
        public string Fname { get; set; }
        public string ROCOFname { get; set; }
        public double ReportRate { get; set; }
        public double WinLength { get; set; }
        public double SynchFreq { get; set; }
        public string PA { get; set; }
        public string PB { get; set; }
        public string PC { get; set; }
    }
}