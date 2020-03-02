using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core.Models
{
    public enum DetectorWindowType
    {
        [Description("Hann")]
        hann,
        [Description("Rectangular")]
        rectwin,
        [Description("Bartlett")]
        bartlett,
        [Description("Hamming")]
        hamming,
        [Description("Blackman")]
        blackman
    }

    public enum DetectorModeType
    {
        [Description("Single Channel")]
        SingleChannel,
        [Description("Multichannel")]
        MultiChannel
    }

    public enum OutOfRangeFrequencyDetectorType
    {
        [Description("Nominal Value")]
        Nominal,
        [Description("History for Baseline (seconds)")]
        AvergeWindow
    }

    public enum DataFileType
    {
        [Description("PDAT")]
        pdat,
        [Description("JSIS CSV")]
        csv,
        [Description("HQ Point on Wave")]
        powHQ,
        [Description("PI Database")]
        PI,
        [Description("openHistorian")]
        OpenHistorian,
        [Description("openPDC")]
        OpenPDC
    }

    public enum ModeType
    {
        Archive,
        [Description("Real Time")]
        RealTime,
        Hybrid
    }
    public enum PowerType
    {
        [Description("Complex")]
        CP,
        [Description("Apparent")]
        S,
        [Description("Active")]
        P,
        [Description("Reactive")]
        Q
    }
    public enum TunableFilterType
    {
        Rational,
        [Description("High-Pass")]
        HighPass,
        [Description("Low-Pass")]
        LowPass,
        [Description("Frequency Derivation")]
        FrequencyDerivation,
        [Description("Running Average")]
        RunningAverage,
        [Description("Point on Wave Power Calculation")]
        PointOnWavePower,
        [Description("RMS Energy")]
        RMSenergyFilt
    }
    public enum InterpolateType
    {
        [Description("Linear")]
        Linear,
        [Description("Constant")]
        Constant,
        [Description("Cubic")]
        Cubic
    }
    public enum OutputSignalStorageType
    {
        [Description("Create Custom PMU")]
        CreateCustomPMU,
        [Description("Replace Input")]
        ReplaceInput
    }
    public enum SignalMapPlotType
    {
        [Description("Dot")]
        Dot,
        [Description("Line")]
        Line,
        [Description("Area")]
        Area
    }
    public enum CurveDesignOptions
    {
        [Description("Option 1")]
        Option1,
        [Description("Option 2")]
        Option2
    }
    public enum RMSEnergyBandOptions
    {
        [Description("Band 1")]
        Band1,
        [Description("Band 2")]
        Band2,
        [Description("Band 3")]
        Band3,
        [Description("Band 4")]
        Band4
    }
}
