using BAWGUI.Core;
using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class CustomizationModel
    {
        public string Name { get; set; }
        public string CustPMUname { get; set; }
    }
    public class ScalarRepCustModel : CustomizationModel
    {
        public ScalarRepCustModel()
        {

        }
        private XElement item;

        public ScalarRepCustModel(XElement item)
        {
            this.item = item;
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
        public new string Name { get => "Scalar Repetition"; }
        public string Scalar { get; set; }
        public string SignalName { get; set; }
        public string TimeSourcePMU { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
    }
    public class AdditionCustModel : CustomizationModel
    {
        public AdditionCustModel() { }
        private XElement item;

        public AdditionCustModel(XElement item)
        {
            this.item = item;
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
            PMUElementList = new List<SignalSignatures>();
            foreach (var term in terms)
            {
                var newTerm = new SignalSignatures(term.Element("PMU").Value, term.Element("Channel").Value);
                PMUElementList.Add(newTerm);
            }
        }
        public new string Name { get => "Addition"; }
        public string SignalName { get; set; }
        public List<SignalSignatures> PMUElementList { get; set; }
    }
    public class SubtractionCustModel : CustomizationModel
    {
        public SubtractionCustModel() { }
        private XElement item;

        public SubtractionCustModel(XElement item)
        {
            this.item = item;
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
            Minuend = new SignalSignatures(item.Element("Parameters").Element("minuend").Element("PMU").Value, item.Element("Parameters").Element("minuend").Element("Channel").Value);
            Subtrahend = new SignalSignatures(item.Element("Parameters").Element("subtrahend").Element("PMU").Value, item.Element("Parameters").Element("subtrahend").Element("Channel").Value);
        }
        public new string Name { get => "Subtraction"; }
        public string SignalName { get; set; }
        public SignalSignatures Minuend { get; set; }
        public SignalSignatures Subtrahend { get; set; }
    }
    public class MultiplicationCustModel : CustomizationModel
    {
        public MultiplicationCustModel() { }
        private XElement item;

        public MultiplicationCustModel(XElement item)
        {
            this.item = item;
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
            PMUElementList = new List<SignalSignatures>();
            foreach (var factor in factors)
            {
                var newFactor = new SignalSignatures(factor.Element("PMU").Value, factor.Element("Channel").Value);
                PMUElementList.Add(newFactor);
            }
        }
        public new string Name { get => "Multiplication"; }
        public string SignalName { get; set; }
        public List<SignalSignatures> PMUElementList { get; set; }
    }
    public class DivisionCustModel : CustomizationModel
    {
        public DivisionCustModel() { }
        private XElement item;

        public DivisionCustModel(XElement item)
        {
            this.item = item;
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
            Dividend = new SignalSignatures(item.Element("Parameters").Element("dividend").Element("PMU").Value, item.Element("Parameters").Element("dividend").Element("Channel").Value);
            Divisor = new SignalSignatures(item.Element("Parameters").Element("divisor").Element("PMU").Value, item.Element("Parameters").Element("divisor").Element("Channel").Value);
        }
        public new string Name { get => "Division"; }
        public string SignalName { get; set; }
        public SignalSignatures Dividend { get; set; }
        public SignalSignatures Divisor { get; set; }
    }
    public class ExponentialCustModel : CustomizationModel
    {
        public ExponentialCustModel() { }
        private XElement item;

        public ExponentialCustModel(XElement item)
        {
            this.item = item;
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
            PMUElementList = new List<PMUElementForUnaryCustModel>();
            foreach (var signal in signals)
            {
                var newSignal = new PMUElementForUnaryCustModel(signal.Element("PMU").Value, signal.Element("Channel").Value, signal.Element("CustName").Value);
                PMUElementList.Add(newSignal);
            }
        }
        public new string Name { get => "Exponential"; }
        public string Exponent { get; set; }
        public List<PMUElementForUnaryCustModel> PMUElementList { get; set; }
    }
    public class SignReversalCustModel : CustomizationModel
    {
        public SignReversalCustModel() { }
        private XElement item;

        public SignReversalCustModel(XElement item)
        {
            this.item = item;
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            var signals = item.Element("Parameters").Elements("signal");
            PMUElementList = new List<PMUElementForUnaryCustModel>();
            foreach (var signal in signals)
            {
                var newSignal = new PMUElementForUnaryCustModel(signal.Element("PMU").Value, signal.Element("Channel").Value, signal.Element("CustName").Value);
                PMUElementList.Add(newSignal);
            }
        }
        public new string Name { get => "Sign Reversal"; }
        public List<PMUElementForUnaryCustModel> PMUElementList { get; set; }
    }
    public class AbsValCustModel : SignReversalCustModel
    {
        public AbsValCustModel() { }
        public AbsValCustModel(XElement item) : base(item)
        {
        }
        public new string Name { get => "Absolute Value"; }
    }
    public class RealComponentCustModel : SignReversalCustModel
    {
        public RealComponentCustModel() { }
        public RealComponentCustModel(XElement item) : base(item)
        {
        }
        public new string Name { get => "Real Component"; }
    }
    public class ImagComponentCustModel : SignReversalCustModel
    {
        public ImagComponentCustModel() { }
        public ImagComponentCustModel(XElement item) : base(item)
        {
        }
        public new string Name { get => "Imaginary Component"; }
    }
    public class AngleCustModel : SignReversalCustModel
    {
        public AngleCustModel() { }
        public AngleCustModel(XElement item) : base(item)
        {
        }
        public new string Name { get => "Angle Calculation"; }
    }
    public class ComplexConjCustModel : SignReversalCustModel
    {
        public ComplexConjCustModel() { }
        public ComplexConjCustModel(XElement item) : base(item)
        {
        }
        public new string Name { get => "Complex Conjugate"; }
    }
    public class CreatePhasorCustModel : CustomizationModel
    {
        public CreatePhasorCustModel() { }
        private XElement item;

        public CreatePhasorCustModel(XElement item)
        {
            this.item = item;
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            var phasors = item.Element("Parameters").Elements("phasor");
            MagAngPairList = new List<PMUElementPairModel>();
            foreach (var phasor in phasors)
            {
                var newPair = new PMUElementPairModel();
                newPair.PMUElement1 = new SignalSignatures(phasor.Element("mag").Element("PMU").Value, phasor.Element("mag").Element("Channel").Value);
                newPair.PMUElement2 = new SignalSignatures(phasor.Element("ang").Element("PMU").Value, phasor.Element("ang").Element("Channel").Value);
                newPair.CustSignalName = phasor.Element("CustName").Value;
                MagAngPairList.Add(newPair);
            }
        }
        public new string Name { get => "Phasor Creation"; }
        public List<PMUElementPairModel> MagAngPairList { get; set; }
    }

    public class PowerCalcCustModel : CustomizationModel
    {
        public PowerCalcCustModel() { }
        private XElement item;
        private Dictionary<string, string> _powerTypeDictionary = new Dictionary<string, string>{{"Complex", "CP"}, {"Apparent", "S"}, {"Active", "P"}, {"Reactive", "Q"}};

        public PowerCalcCustModel(XElement item)
        {
            this.item = item;
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
                PhasorPair.PMUElement1 = new SignalSignatures(item.Element("Parameters").Element("power").Element("Vphasor").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Vphasor").Element("Channel").Value);
                PhasorPair.PMUElement2 = new SignalSignatures(item.Element("Parameters").Element("power").Element("Iphasor").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Iphasor").Element("Channel").Value);
                PhasorPair.CustSignalName = item.Element("Parameters").Element("power").Element("CustName").Value;
            }
            else
            {
                IsFromPhasor = false;
                MagAngQuad = new MagAngToPower();
                MagAngQuad.PMUElement1 = new SignalSignatures(item.Element("Parameters").Element("power").Element("Vmag").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Vmag").Element("Channel").Value);
                MagAngQuad.PMUElement2 = new SignalSignatures(item.Element("Parameters").Element("power").Element("Vang").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Vang").Element("Channel").Value);
                MagAngQuad.PMUElement3 = new SignalSignatures(item.Element("Parameters").Element("power").Element("Imag").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Imag").Element("Channel").Value);
                MagAngQuad.PMUElement4 = new SignalSignatures(item.Element("Parameters").Element("power").Element("Iang").Element("PMU").Value, item.Element("Parameters").Element("power").Element("Iang").Element("Channel").Value);
                MagAngQuad.CustSignalName = item.Element("Parameters").Element("power").Element("CustName").Value;
            }
        }
        public new string Name { get => "Power Calculation"; }
        public PowerType PowType { get; set; }
        public bool IsFromPhasor { get; set; }
        public PhasorToPower PhasorPair { get; set; }
        public MagAngToPower MagAngQuad { get; set; }
    }
    public class SpecTypeUnitCustModel : CustomizationModel
    {
        public SpecTypeUnitCustModel() { }
        private XElement item;

        public SpecTypeUnitCustModel(XElement item)
        {
            this.item = item;
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
            Input = new SignalSignatures(item.Element("Parameters").Element("PMU").Value, item.Element("Parameters").Element("Channel").Value);
        }
        public new string Name { get => "Signal Type/Unit"; }
        public SignalSignatures Input { get; set; }
        public string SignalName { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
    }
    public class MetricPrefixCustModel : CustomizationModel
    {
        public MetricPrefixCustModel() { }
        private XElement item;

        public MetricPrefixCustModel(XElement item)
        {
            this.item = item;
            var par = item.Element("Parameters").Element("CustPMUname");
            ToConverts = new List<ToConvert>();
            if (par != null)
            {
                CustPMUname = par.Value;
                //UseCustomPMU = true;
            }
            var toConverts = item.Element("Parameters").Elements("ToConvert");
            foreach (var convert in toConverts)
            {
                var newConvert = new ToConvert();
                var va = convert.Element("PMU");
                if (va != null)
                {
                    newConvert.PMUName = va.Value;
                }
                va = convert.Element("Channel");
                if (va != null)
                {
                    newConvert.Channel = va.Value;
                }
                va = convert.Element("NewUnit");
                if (va != null)
                {
                    newConvert.NewUnit = va.Value;
                }
                va = convert.Element("CustName");
                if (va != null)
                {
                    newConvert.SignalName = va.Value;
                }
                //newConvert.Channel = convert.Element("Channel").Value;
                //newConvert.NewUnit = convert.Element("NewUnit").Value;
                //newConvert.SignalName = convert.Element("CustName").Value;
                ToConverts.Add(newConvert);
            }
            //else
            //{
            //    //UseCustomPMU = false;
            //    var toConverts = item.Element("Parameters").Elements("ToConvert");
            //    foreach (var convert in toConverts)
            //    {
            //        var newConvert = new ToConvert();
            //        newConvert.PMU = convert.Element("PMU").Value;
            //        newConvert.Channel = convert.Element("Channel").Value;
            //        newConvert.NewUnit = convert.Element("NewUnit").Value;
            //        ToConverts.Add(newConvert);
            //    }
            //}
        }
        public new string Name { get => "Metric Prefix"; }
        //public bool UseCustomPMU { get; set; }
        public List<ToConvert> ToConverts { get; set; }
    }
    public class AngleConversionCustModel : CustomizationModel
    {
        public AngleConversionCustModel() { }
        private XElement item;

        public AngleConversionCustModel(XElement item)
        {
            this.item = item;
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
                newConvert.PMUName = convert.Element("PMU").Value;
                newConvert.Channel = convert.Element("Channel").Value;
                newConvert.SignalName = convert.Element("CustName").Value;
                ToConverts.Add(newConvert);
            }
        }
        public new string Name { get => "Angle Conversion"; }
        public List<ToConvert> ToConverts { get; set; }
    }
    public class SignalReplicationCustModel : CustomizationModel
    {
        public SignalReplicationCustModel() { }
        private XElement item;

        public SignalReplicationCustModel(XElement item)
        {
            this.item = item;
            var par = item.Element("Parameters").Element("CustPMUname").Value;
            if (par != null)
            {
                CustPMUname = par;
            }
            ToRep = new List<ToReplicate>();
            var toReps = item.Element("Parameters").Elements("ToReplicate");
            foreach (var rep in toReps)
            {
                var newRep = new ToReplicate();
                newRep.PMUName = rep.Element("PMU").Value;
                newRep.Channel = rep.Element("Channel").Value;
                ToRep.Add(newRep);
            }
        }

        public List<ToReplicate> ToRep { get; set; }
        public new string Name { get => "Duplicate Signals"; }
    }
    public class GraphEigenvalueCustModel : CustomizationModel
    {
        public GraphEigenvalueCustModel() { }
        private XElement item;

        public GraphEigenvalueCustModel(XElement item)
        {
            this.item = item;
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
            PMUElementList = new List<SignalSignatures>();
            foreach (var term in terms)
            {
                var newTerm = new SignalSignatures(term.Element("PMU").Value, term.Element("Channel").Value);
                PMUElementList.Add(newTerm);
            }
        }
        public new string Name { get => "Graph Eigenvalue"; }
        public string SignalName { get; set; }
        public List<SignalSignatures> PMUElementList { get; set; }
    }
}
