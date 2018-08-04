using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.ReadConfigXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeMeter.Models
{
    public class Mode
    {
        public Mode()
        {
            PMUs = new List<SignalSignatures>();
            RetConTracking = new RetroactiveContinuity();
            DesiredModes = new DesiredModeAttributes();
            AlgNames = new List<ModeMethod>();
            FODetectorParameters = new PeriodogramDetectorModel();
            AnalysisLength = 600;
        }
        public string ModeName { get; set; }
        public List<SignalSignatures> PMUs { get; set; }
        public int AnalysisLength { get; set; }
        public string DampRatioThreshold { get; set; }
        public RetroactiveContinuity RetConTracking { get; set; }
        public DesiredModeAttributes DesiredModes { get; set; }
        public List<ModeMethod> AlgNames { get; set; }
        public PeriodogramDetectorModel FODetectorParameters { get; set; }
    }
    public class RetroactiveContinuity
    {
        public RetroactiveContinuityStatusType Status { get; set; }
        public string MaxLength { get; set; }
    }
    public class DesiredModeAttributes
    {
        public string LowF { get; set; }
        public string HighF { get; set; }
        public string GuessF { get; set; }
        public string DampMax { get; set; }
    }
    public class ModeMethod
    {
        public ModeMethods Name { get; set; }
        public string ARModelOrder { get; set; }
        public string MAModelOrder { get; set; }
        public string NumberOfEquations { get; set; }
        public string ExaggeratedARModelOrder { get; set; }
        public string NumberOfEquationsWithFOpresent { get; set; }
    }
    //public class YWARMA : ModeMethodBase
    //{
    //    public string NumberOfEquations { get; set; }
    //    public new string Name { get => "YW-ARMA"; }
    //}
    //public class LSARMA : ModeMethodBase
    //{
    //    public string ExaggeratedARModelOrder { get; set; }
    //    public new string Name { get => "LS-ARMA"; }
    //}
    //public class YWARMAS : ModeMethodBase
    //{
    //    public string NumberOfEquations { get; set; }
    //    public string NumberOfEquationsWithFOpresent { get; set; }
    //    public new string Name { get => "YW-ARMA+S"; }
    //}
    //public class LSARMAS : ModeMethodBase
    //{
    //    public string ExaggeratedARModelOrder { get; set; }
    //    public new string Name { get => "LS-ARMA+S"; }
    //}
    public enum ModeMethods
    {
        [Description("YW-ARMA")]
        YWARMA,
        [Description("LS-ARMA")]
        LSARMA,
        [Description("YW-ARMA+S")]
        YWARMAS,
        [Description("LS-ARMA+S")]
        LSARMAS
    }
    public enum RetroactiveContinuityStatusType
    {
        [Description("ON")]
        ON,
        [Description("OFF")]
        OFF
    }
}
