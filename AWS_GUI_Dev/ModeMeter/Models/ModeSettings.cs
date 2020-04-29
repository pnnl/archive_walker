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
            FODetectorParas = new FOdetectorParameters();
            AnalysisLength = 600;
            EventDetectionPara = new EventDetectionParameters();
            ShowRMSEnergyTransientParameters = false;
            ShowFOParameters = false;
        }
        public string ModeName { get; set; }
        public List<SignalSignatures> PMUs { get; set; }
        public int AnalysisLength { get; set; }
        //public string DampRatioThreshold { get; set; }
        public RetroactiveContinuity RetConTracking { get; set; }
        public DesiredModeAttributes DesiredModes { get; set; }
        public List<ModeMethod> AlgNames { get; set; }
        public EventDetectionParameters EventDetectionPara { get; set; }
        public bool ShowFOParameters { get; set; }
        public bool ShowRMSEnergyTransientParameters { get; set; }
        public FOdetectorParameters FODetectorParas { get; set; }
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
        public string NaNomitLimit { get; set; } = "0";
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
    public class EventDetectionParameters
    {
        public EventDetectionParameters()
        {
            //RMSlength = "15";
            //RMSmedianFilterTime = "120";
            //RingThresholdScale = "5";
            //MinAnalysisLength = "600";
            PMUs = new List<SignalSignatures>();
        }
        //public string RMSlength { get; set; }
        //public string RMSmedianFilterTime { get; set; }
        public string Threshold { get; set; }
        public string MinAnalysisLength { get; set; }
        public bool RingdownID { get; set; }
        public ForgetFactor1Type ForgetFactor1 { get; set; }
        public ForgetFactor2Type ForgetFactor2 { get; set; }
        public PostEventWinAdjType PostEventWinAdj { get; set; }
        public List<SignalSignatures> PMUs { get; set; }
    }
    public enum ForgetFactor1Type
    {
        [Description("Enable")]
        TRUE,
        [Description("Disable")]
        FALSE
    }
    public enum ForgetFactor2Type
    {
        [Description("Match Full Analysis Window")]
        MATCH,
        [Description("Match Smallest Analysis Window")]
        TRUE,
        [Description("Disable")]
        FALSE
    }
    public enum PostEventWinAdjType
    {
        [Description("None")]
        NONE,
        [Description("Shorten")]
        SHORTEN,
        [Description("Diminish")]
        DIMINISH
    }
    public class FOtimeLocParameters
    {
        public bool PerformTimeLoc { get; set; }
        public string LocMinLength { get; set; }
        public string LocLengthStep { get; set; }
        public string LocRes { get; set; }
    }
    public class FOdetectorParameters
    {
        public FOtimeLocParameters FOtimeLocParams { get; set; } = new FOtimeLocParameters();
        public PeriodogramDetectorModel FODetectorParams { get; set; } = new PeriodogramDetectorModel();
        public List<SignalSignatures> PMUs { get; set; } = new List<SignalSignatures>();
        public string MinTestStatWinLength { get; set; }
    }
}
