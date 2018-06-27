using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class PostProcessConfigModel
    {
        private XElement _xElement;
        public List<object> CollectionOfSteps { get; set; }
        public PostProcessConfigModel(XElement xElement)
        {
            this._xElement = xElement;
            CollectionOfSteps = new List<object>();
            var stages = _xElement.Element("Configuration").Elements("Stages");
            foreach (var stage in stages)
            {
                foreach (var item in stage.Elements())
                {
                    var name = item.Element("Name").Value;
                    switch (name)
                    {
                        case "PMUflagFilt":
                            CollectionOfSteps.Add(new StatusFlagsDQFilterModel(item));
                            break;
                        case "DropOutZeroFilt":
                            CollectionOfSteps.Add(new ZerosDQFilterModel(item));
                            break;
                        case "DropOutMissingFilt":
                            CollectionOfSteps.Add(new MissingDQFilterModel(item));
                            break;
                        case "VoltPhasorFilt":
                            CollectionOfSteps.Add(new VoltPhasorDQFilterModel(item));
                            break;
                        case "FreqFilt":
                            CollectionOfSteps.Add(new FreqDQFilterModel(item));
                            break;
                        case "OutlierFilt":
                            CollectionOfSteps.Add(new OutlierDQFilterModel(item));
                            break;
                        case "StaleFilt":
                            CollectionOfSteps.Add(new StaleDQFilterModel(item));
                            break;
                        case "DataFrameFilt":
                            CollectionOfSteps.Add(new DataFrameDQFilterModel(item));
                            break;
                        case "PMUchanFilt":
                            CollectionOfSteps.Add(new PMUchanDQFilterModel(item));
                            break;
                        case "PMUallFilt":
                            CollectionOfSteps.Add(new PMUallDQFilterModel(item));
                            break;
                        case "WrappingFailureFilt":
                            CollectionOfSteps.Add(new WrappingFailureDQFilterModel(item));
                            break;
                        case "ScalarRep":
                            CollectionOfSteps.Add(new ScalarRepCustModel(item));
                            break;
                        case "Addition":
                            CollectionOfSteps.Add(new AdditionCustModel(item));
                            break;
                        case "Subtraction":
                            CollectionOfSteps.Add(new SubtractionCustModel(item));
                            break;
                        case "Multiplication":
                            CollectionOfSteps.Add(new MultiplicationCustModel(item));
                            break;
                        case "Division":
                            CollectionOfSteps.Add(new DivisionCustModel(item));
                            break;
                        case "Exponent":
                            CollectionOfSteps.Add(new ExponentialCustModel(item));
                            break;
                        case "SignReversal":
                            CollectionOfSteps.Add(new SignReversalCustModel(item));
                            break;
                        case "AbsVal":
                            CollectionOfSteps.Add(new AbsValCustModel(item));
                            break;
                        case "RealComponent":
                            CollectionOfSteps.Add(new RealComponentCustModel(item));
                            break;
                        case "ImagComponent":
                            CollectionOfSteps.Add(new ImagComponentCustModel(item));
                            break;
                        case "Angle":
                            CollectionOfSteps.Add(new AngleCustModel(item));
                            break;
                        case "ComplexConj":
                            CollectionOfSteps.Add(new ComplexConjCustModel(item));
                            break;
                        case "CreatePhasor":
                            CollectionOfSteps.Add(new CreatePhasorCustModel(item));
                            break;
                        case "PowerCalc":
                            CollectionOfSteps.Add(new PowerCalcCustModel(item));
                            break;
                        case "SpecTypeUnit":
                            CollectionOfSteps.Add(new SpecTypeUnitCustModel(item));
                            break;
                        case "MetricPrefix":
                            CollectionOfSteps.Add(new MetricPrefixCustModel(item));
                            break;
                        case "AngleConversion":
                            CollectionOfSteps.Add(new AngleConversionCustModel(item));
                            break;
                        default:
                            throw new Exception("Error in reading data config customization steps, customization not recognized: " + name);
                    }
                }
            }
        }
    }
}