using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class Customization
    {
        public string Name { get; set; }
    }
    public class ScalarRepCust : Customization
    {
        private XElement item;

        public ScalarRepCust(XElement item)
        {
            this.item = item;
            Name = "Scalar Repetition";
            var par = item.Element("Parameters").Element("CustPMUname");
            if (par != null)
            {
                CustPMUname = par.Value;
            }
            par = item.Element("Parameters").Element("scalar");
            if (par != null)
            {
                Scalar = par.Value;
            }
            par = item.Element("Parameters").Element("SignalName");
            if (par != null)
            {
                SignalName = par.Value;
            }
            par = item.Element("Parameters").Element("TimeSourcePMU");
            if (par != null)
            {
                TimeSourcePMU = par.Value;
            }
            par = item.Element("Parameters").Element("SignalType");
            if (par != null)
            {
                Type = par.Value;
            }
            par = item.Element("Parameters").Element("SignalUnit");
            if (par != null)
            {
                Unit = par.Value;
            }
        }

        public string CustPMUname { get; set; }
        public string Scalar { get; set; }
        public string SignalName { get; set; }
        public string TimeSourcePMU { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
    }
    public class AdditionCust : Customization
    {
        private XElement item;

        public AdditionCust(XElement item)
        {
            this.item = item;
            Name = "Addition";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            par = item.Element("Parameters").Element("SignalName").Value;
            if (par != null)
            {
                SignalName = par;
            }
            var terms = item.Element("Parameters").Elements("term");
            PMUElementList = new List<PMUElement>();
            foreach (var term in terms)
            {
                var newTerm = new PMUElement(term.Element("PMU").Value, term.Element("Channel").Value);
                PMUElementList.Add(newTerm);
            }
        }

        public string CustPMUname { get; set; }
        public string SignalName { get; set; }
        public List<PMUElement> PMUElementList { get; set; }
    }
    public class SubtractionCust : Customization
    {
        private XElement item;

        public SubtractionCust(XElement item)
        {
            this.item = item;
            Name = "Subtraction";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            par = item.Element("Parameters").Element("SignalName").Value;
            if (par != null)
            {
                SignalName = par;
            }
            Minuend = new PMUElement(item.Element("Parameters").Element("minuend").Element("PMU").Value, item.Element("Parameters").Element("minuend").Element("Channel").Value);
            Subtrahend = new PMUElement(item.Element("Parameters").Element("subtrahend").Element("PMU").Value, item.Element("Parameters").Element("subtrahend").Element("Channel").Value);
        }

        public string CustPMUname { get; set; }
        public string SignalName { get; set; }
        public PMUElement Minuend { get; set; }
        public PMUElement Subtrahend { get; set; }
    }
    public class MultiplicationCust : Customization
    {
        private XElement item;

        public MultiplicationCust(XElement item)
        {
            this.item = item;
            Name = "Multiplication";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            par = item.Element("Parameters").Element("SignalName").Value;
            if (par != null)
            {
                SignalName = par;
            }
            var factors = item.Element("Parameters").Elements("factor");
            PMUElementList = new List<PMUElement>();
            foreach (var factor in factors)
            {
                var newFactor = new PMUElement(factor.Element("PMU").Value, factor.Element("Channel").Value);
                PMUElementList.Add(newFactor);
            }
        }

        public string CustPMUname { get; set; }
        public string SignalName { get; set; }
        public List<PMUElement> PMUElementList { get; set; }
    }
    public class DivisionCust : Customization
    {
        private XElement item;

        public DivisionCust(XElement item)
        {
            this.item = item;
            Name = "Division";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            par = item.Element("Parameters").Element("SignalName").Value;
            if (par != null)
            {
                SignalName = par;
            }
            Dividend = new PMUElement(item.Element("Parameters").Element("dividend").Element("PMU").Value, item.Element("Parameters").Element("dividend").Element("Channel").Value);
            Divisor = new PMUElement(item.Element("Parameters").Element("divisor").Element("PMU").Value, item.Element("Parameters").Element("divisor").Element("Channel").Value);
        }
        public string CustPMUname { get; set; }
        public string SignalName { get; set; }
        public PMUElement Dividend { get; set; }
        public PMUElement Divisor { get; set; }
    }
    public class ExponentialCust : Customization
    {
        private XElement item;

        public ExponentialCust(XElement item)
        {
            this.item = item;
            Name = "Exponential";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            par = item.Element("Parameters").Element("exponent").Value;
            if (par != null)
            {
                Exponent = par;
            }
            var signals = item.Element("Parameters").Elements("signal");
            PMUElementList = new List<PMUElementForUnaryCust>();
            foreach (var signal in signals)
            {
                var newSignal = new PMUElementForUnaryCust(signal.Element("PMU").Value, signal.Element("Channel").Value, signal.Element("CustName").Value);
                PMUElementList.Add(newSignal);
            }
        }

        public string CustPMUname { get; set; }
        public string Exponent { get; set; }
        public List<PMUElementForUnaryCust> PMUElementList { get; set; }
    }
    public class SignReversalCust : Customization
    {
        private XElement item;

        public SignReversalCust(XElement item)
        {
            this.item = item;
            Name = "Sign Reversal";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            var signals = item.Element("Parameters").Elements("signal");
            PMUElementList = new List<PMUElementForUnaryCust>();
            foreach (var signal in signals)
            {
                var newSignal = new PMUElementForUnaryCust(signal.Element("PMU").Value, signal.Element("Channel").Value, signal.Element("CustName").Value);
                PMUElementList.Add(newSignal);
            }
        }

        public string CustPMUname { get; set; }
        public List<PMUElementForUnaryCust> PMUElementList { get; set; }
    }
    public class AbsValCust : SignReversalCust
    {
        public AbsValCust(XElement item) : base(item)
        {
            Name = "Absolute Value";
        }
    }
    public class RealComponentCust : SignReversalCust
    {
        public RealComponentCust(XElement item) : base(item)
        {
            Name = "Real Component";
        }
    }
    public class ImagComponentCust : SignReversalCust
    {
        public ImagComponentCust(XElement item) : base(item)
        {
            Name = "Imaginary Component";
        }
    }
    public class AngleCust : SignReversalCust
    {
        public AngleCust(XElement item) : base(item)
        {
            Name = "Angle Calculation";
        }
    }
    public class ComplexConjCust : SignReversalCust
    {
        public ComplexConjCust(XElement item) : base(item)
        {
            Name = "Complex Conjugate";
        }
    }
    public class CreatePhasorCust : Customization
    {
        private XElement item;

        public CreatePhasorCust(XElement item)
        {
            this.item = item;
            Name = "Phasor Creation";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            var phasors = item.Element("Parameters").Elements("phasor");
            MagAngPairList = new List<PMUElementPair>();
            foreach (var phasor in phasors)
            {
                var newPair = new PMUElementPair();
                newPair.PMUElement1 = new PMUElement(phasor.Element("mag").Element("PMU").Value, phasor.Element("mag").Element("Channel").Value);
                newPair.PMUElement2 = new PMUElement(phasor.Element("ang").Element("PMU").Value, phasor.Element("ang").Element("Channel").Value);
                newPair.CustSignalName = phasor.Element("CustName").Value;
                MagAngPairList.Add(newPair);
            }
        }
        public string CustPMUname { get; set; }
        public List<PMUElementPair> MagAngPairList { get; set; }
    }

    public class PowerCalcCust : Customization
    {
        private XElement item;
        private Dictionary<string, string> _powerTypeDictionary = new Dictionary<string, string>{{"Complex", "CP"}, {"Apparent", "S"}, {"Active", "P"}, {"Reactive", "Q"}};

    public PowerCalcCust(XElement item)
        {
            this.item = item;
            Name = "Power Calculation";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            par = item.Element("Parameters").Element("PowType").Value;
            if (par != null)
            {
                PowType = (PowerType)Enum.Parse(typeof(PowerType), _powerTypeDictionary[par]);
            }
            if (item.Element("Parameters").Element("power").Element("Vphasor") != null)
            {
                IsFromPhasor = true;
                PhasorPair = new PhasorToPower();
                PhasorPair.PMUElement1 = new PMUElement(item.Element("Parameters").Element("power").Element("Vphasor").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Vphasor").Element("Channel").Value);
                PhasorPair.PMUElement2 = new PMUElement(item.Element("Parameters").Element("power").Element("Iphasor").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Iphasor").Element("Channel").Value);
                PhasorPair.CustSignalName = item.Element("Parameters").Element("power").Element("CustName").Value;
            }
            else
            {
                IsFromPhasor = false;
                MagAngQuad = new MagAngToPower();
                MagAngQuad.PMUElement1 = new PMUElement(item.Element("Parameters").Element("power").Element("Vmag").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Vmag").Element("Channel").Value);
                MagAngQuad.PMUElement2 = new PMUElement(item.Element("Parameters").Element("power").Element("Vang").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Vang").Element("Channel").Value);
                MagAngQuad.PMUElement3 = new PMUElement(item.Element("Parameters").Element("power").Element("Imag").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Imag").Element("Channel").Value);
                MagAngQuad.PMUElement4 = new PMUElement(item.Element("Parameters").Element("power").Element("Iang").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Iang").Element("Channel").Value);
                MagAngQuad.CustSignalName = item.Element("Parameters").Element("power").Element("CustName").Value;
            }
        }

        public string CustPMUname { get; set; }
        public PowerType PowType { get; set; }
        public bool IsFromPhasor { get; set; }
        public PhasorToPower PhasorPair { get; set; }
        public MagAngToPower MagAngQuad { get; set; }
    }
    public class SpecTypeUnitCust : Customization
    {
        private XElement item;

        public SpecTypeUnitCust(XElement item)
        {
            this.item = item;
            Name = "Signal Type/Unit";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            par = item.Element("Parameters").Element("CustName").Value;
            if (par != null)
            {
                SignalName = par;
            }
            par = item.Element("Parameters").Element("SigType").Value;
            if (par != null)
            {
                Type = par;
            }
            par = item.Element("Parameters").Element("SigUnit").Value;
            if (par != null)
            {
                Unit = par;
            }
            Input = new PMUElement(item.Element("Parameters").Element("PMU").Value, item.Element("Parameters").Element("Channel").Value);
        }

        public PMUElement Input { get; set; }
        public string CustPMUname { get; set; }
        public string SignalName { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
    }
    public class MetricPrefixCust : Customization
    {
        private XElement item;

        public MetricPrefixCust(XElement item)
        {
            this.item = item;
            Name = "Metric Prefix";
            var par = item.Element("Parameters").Element("CustPMUname");
            ToConverts = new List<ToConvert>();
            if (par != null)
            {
                CustPMUname = par.Value;
                UseCustomPMU = true;
                var toConverts = item.Element("Parameters").Elements("ToConvert");
                foreach (var convert in toConverts)
                {
                    var newConvert = new ToConvert();
                    newConvert.PMU = convert.Element("PMU").Value;
                    newConvert.Channel = convert.Element("Channel").Value;
                    newConvert.NewUnit = convert.Element("NewUnit").Value;
                    newConvert.SignalName = convert.Element("CustName").Value;
                    ToConverts.Add(newConvert);
                }
            }
            else
            {
                UseCustomPMU = false;
                var toConverts = item.Element("Parameters").Elements("ToConvert");
                foreach (var convert in toConverts)
                {
                    var newConvert = new ToConvert();
                    newConvert.PMU = convert.Element("PMU").Value;
                    newConvert.Channel = convert.Element("Channel").Value;
                    newConvert.NewUnit = convert.Element("NewUnit").Value;
                    ToConverts.Add(newConvert);
                }
            }
        }

        public bool UseCustomPMU { get; set; }
        public string CustPMUname { get; set; }
        public List<ToConvert> ToConverts { get; set; }
    }
    public class AngleConversionCust : Customization
    {
        private XElement item;

        public AngleConversionCust(XElement item)
        {
            this.item = item;
            Name = "Angle Conversion";
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            ToConverts = new List<ToConvert>();
            var toConverts = item.Element("Parameters").Elements("ToConvert");
            foreach (var convert in toConverts)
            {
                var newConvert = new ToConvert();
                newConvert.PMU = convert.Element("PMU").Value;
                newConvert.Channel = convert.Element("Channel").Value;
                newConvert.SignalName = convert.Element("CustName").Value;
                ToConverts.Add(newConvert);
            }
        }

        public string CustPMUname { get; set; }
        public List<ToConvert> ToConverts { get; set; }
    }

    public class ToConvert
    {
        public string PMU { get; set; }
        public string Channel { get; set; }
        public string NewUnit { get; set; }
        public string SignalName { get; set; }
    }
}
