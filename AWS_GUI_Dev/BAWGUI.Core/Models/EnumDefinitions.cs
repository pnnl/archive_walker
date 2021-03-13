using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        RMSenergyFilt,
        [Description("PMU")]
        POWpmuFilt
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
    public enum POWPMUFilterInputType
    {
        Voltage,
        Current
    }
    public static class EnumExtencsionMethod
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
        public static string ToStringEnums(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
            // or return default(T);
        }
    }
}
