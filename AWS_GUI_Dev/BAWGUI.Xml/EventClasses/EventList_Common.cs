﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 

namespace BAWGUI.Xml
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("Events", Namespace = "", IsNullable = false)]
    public partial class EventSequenceType
    {
        private ForcedOscillationType[] forcedOscillationField;

        private WindRampType[] windRampField;

        private RingdownType[] ringdownField;

        private OutOfRangeFrequencyType[] outOfRangeFrequencyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ForcedOscillation")]
        public ForcedOscillationType[] ForcedOscillation
        {
            get { return this.forcedOscillationField; }
            set { this.forcedOscillationField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("WindRamp")]
        public WindRampType[] WindRamp
        {
            get { return this.windRampField; }
            set { this.windRampField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Ringdown")]
        public RingdownType[] Ringdown
        {
            get { return this.ringdownField; }
            set { this.ringdownField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OutOfRangeGeneral")]
        public OutOfRangeFrequencyType[] OutOfRangeFrequency
        {
            get { return this.outOfRangeFrequencyField; }
            set { this.outOfRangeFrequencyField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ForcedOscillationType
    {
        private decimal idField;

        private string overallStartField;

        private string overallEndField;

        private ForcedOscillationTypeOccurrence[] occurrenceField;

        /// <remarks/>
        public decimal ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        /// <remarks/>
        public string OverallStart
        {
            get { return this.overallStartField; }
            set { this.overallStartField = value; }
        }

        /// <remarks/>
        public string OverallEnd
        {
            get { return this.overallEndField; }
            set { this.overallEndField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Occurrence")]
        public ForcedOscillationTypeOccurrence[] Occurrence
        {
            get { return this.occurrenceField; }
            set { this.occurrenceField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ForcedOscillationTypeOccurrence
    {
        private decimal occurrenceIDField;

        private decimal frequencyField;

        private string startField;

        private string endField;

        private uint persistenceField;

        private byte alarmFlagField;

        private ForcedOscillationTypeOccurrenceChannel[] channelField;

        /// <remarks/>
        public decimal OccurrenceID
        {
            get { return this.occurrenceIDField; }
            set { this.occurrenceIDField = value; }
        }

        /// <remarks/>
        public decimal Frequency
        {
            get { return this.frequencyField; }
            set { this.frequencyField = value; }
        }

        /// <remarks/>
        public string Start
        {
            get { return this.startField; }
            set { this.startField = value; }
        }

        /// <remarks/>
        public string End
        {
            get { return this.endField; }
            set { this.endField = value; }
        }

        /// <remarks/>
        public uint Persistence
        {
            get { return this.persistenceField; }
            set { this.persistenceField = value; }
        }

        /// <remarks/>
        public byte AlarmFlag
        {
            get { return this.alarmFlagField; }
            set { this.alarmFlagField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Channel")]
        public ForcedOscillationTypeOccurrenceChannel[] Channel
        {
            get { return this.channelField; }
            set { this.channelField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ForcedOscillationTypeOccurrenceChannel
    {
        private string nameField;

        private string pmuField;

        private string unitField;

        private float amplitudeField;

        private float sNRField;

        private float coherenceField;

        /// <remarks/>
        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public string PMU
        {
            get { return this.pmuField; }
            set { this.pmuField = value; }
        }
        public string Unit
        {
            get { return this.unitField; }
            set { this.unitField = value; }
        }
        /// <remarks/>
        public float Amplitude
        {
            get { return this.amplitudeField; }
            set { this.amplitudeField = value; }
        }

        /// <remarks/>
        public float SNR
        {
            get { return this.sNRField; }
            set { this.sNRField = value; }
        }

        /// <remarks/>
        public float Coherence
        {
            get { return this.coherenceField; }
            set { this.coherenceField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class OutOfRangeFrequencyType
    {
        private decimal idField;

        private string startField;

        private string endField;

        private string extremaFiled;

        private string extremaFactorField;

        private OutOfRangeFrequencyTypeChannel[] channelField;

        /// <remarks/>
        public decimal ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        /// <remarks/>
        public string Start
        {
            get { return this.startField; }
            set { this.startField = value; }
        }

        /// <remarks/>
        public string End
        {
            get { return this.endField; }
            set { this.endField = value; }
        }

        public string Extrema
        {
            get { return this.extremaFiled; }
            set { this.extremaFiled = value; }
        }

        public string ExtremaFactor
        {
            get { return this.extremaFactorField; }
            set { this.extremaFactorField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Channel")]
        public OutOfRangeFrequencyTypeChannel[] Channel
        {
            get { return this.channelField; }
            set { this.channelField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OutOfRangeFrequencyTypeChannel
    {
        private string pMUField;

        private string nameField;

        /// <remarks/>
        public string PMU
        {
            get { return this.pMUField; }
            set { this.pMUField = value; }
        }

        /// <remarks/>
        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RingdownType
    {
        private decimal idField;

        private string startField;

        private string endField;

        private RingdownTypeChannel[] channelField;

        /// <remarks/>
        public decimal ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        /// <remarks/>
        public string Start
        {
            get { return this.startField; }
            set { this.startField = value; }
        }

        /// <remarks/>
        public string End
        {
            get { return this.endField; }
            set { this.endField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Channel")]
        public RingdownTypeChannel[] Channel
        {
            get { return this.channelField; }
            set { this.channelField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RingdownTypeChannel
    {

        private string pMUField;

        private string nameField;

        /// <remarks/>
        public string PMU
        {
            get { return this.pMUField; }
            set { this.pMUField = value; }
        }

        /// <remarks/>
        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class WindRampType
    {
        private decimal idField;

        private string pMUField;

        private string channelField;

        private string trendStartField;

        private string trendEndField;

        private decimal trendValueField;

        private decimal valueStartField;

        private decimal valueEndField;

        /// <remarks/>
        public decimal ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        /// <remarks/>
        public string PMU
        {
            get { return this.pMUField; }
            set { this.pMUField = value; }
        }

        /// <remarks/>
        public string Channel
        {
            get { return this.channelField; }
            set { this.channelField = value; }
        }

        /// <remarks/>
        public string TrendStart
        {
            get { return this.trendStartField; }
            set { this.trendStartField = value; }
        }

        /// <remarks/>
        public string TrendEnd
        {
            get { return this.trendEndField; }
            set { this.trendEndField = value; }
        }

        /// <remarks/>
        public decimal TrendValue
        {
            get { return this.trendValueField; }
            set { this.trendValueField = value; }
        }

        /// <remarks/>
        public decimal ValueStart
        {
            get { return this.valueStartField; }
            set { this.valueStartField = value; }
        }

        /// <remarks/>
        public decimal ValueEnd
        {
            get { return this.valueEndField; }
            set { this.valueEndField = value; }
        }
    }

}
