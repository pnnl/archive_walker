using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Xml.EventClasses
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ForcedOscillation
    {
        private decimal idField;

        private string overallStartField;

        private string overallEndField;

        private ForcedOscillationOccurrence[] occurrenceField;

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
        public ForcedOscillationOccurrence[] Occurrence
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
    public partial class ForcedOscillationOccurrence
    {

        private decimal occurrenceIDField;

        private decimal frequencyField;

        private string startField;

        private string endField;

        private ushort persistenceField;

        private byte alarmFlagField;

        private ForcedOscillationOccurrenceChannel[] channelField;

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
        public ushort Persistence
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
        public ForcedOscillationOccurrenceChannel[] Channel
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
    public partial class ForcedOscillationOccurrenceChannel
    {

        private string nameField;

        private float amplitudeField;

        private float sNRField;

        private float coherenceField;

        /// <remarks/>
        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Ringdown
    {

        private decimal idField;

        private string startField;

        private string endField;

        private RingdownChannel[] channelField;

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
        public RingdownChannel[] Channel
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
    public partial class RingdownChannel
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class OutOfRangeFrequency
    {

        private decimal idField;

        private string startField;

        private string endField;

        private OutOfRangeFrequencyChannel[] channelField;

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
        public OutOfRangeFrequencyChannel[] Channel
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
    public partial class OutOfRangeFrequencyChannel
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class WindRamp
    {
        private decimal idField;

        private string pMUField;

        private string channelField;

        private string trendStartField;

        private string trendEndField;

        private decimal trendValueField;

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
    }
}
