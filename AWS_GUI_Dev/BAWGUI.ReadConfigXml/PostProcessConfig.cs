using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class PostProcessConfig
    {
        private XElement _xElement;
        public List<object> CollectionOfSteps { get; set; }
        public PostProcessConfig(XElement xElement)
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
                            CollectionOfSteps.Add(new StatusFlagsDQFilter(item));
                            break;
                        case "DropOutZeroFilt":
                            CollectionOfSteps.Add(new ZerosDQFilter(item));
                            break;
                        case "DropOutMissingFilt":
                            CollectionOfSteps.Add(new MissingDQFilter(item));
                            break;
                        case "VoltPhasorFilt":
                            CollectionOfSteps.Add(new VoltPhasorDQFilter(item));
                            break;
                        case "FreqFilt":
                            CollectionOfSteps.Add(new FreqDQFilter(item));
                            break;
                        case "OutlierFilt":
                            CollectionOfSteps.Add(new OutlierDQFilter(item));
                            break;
                        case "StaleFilt":
                            CollectionOfSteps.Add(new StaleDQFilter(item));
                            break;
                        case "DataFrameFilt":
                            CollectionOfSteps.Add(new DataFrameDQFilter(item));
                            break;
                        case "PMUchanFilt":
                            CollectionOfSteps.Add(new PMUchanDQFilter(item));
                            break;
                        case "PMUallFilt":
                            CollectionOfSteps.Add(new PMUallDQFilter(item));
                            break;
                        case "WrappingFailureFilt":
                            CollectionOfSteps.Add(new WrappingFailureDQFilter(item));
                            break;
                        case "ScalarRep":
                            CollectionOfSteps.Add(new ScalarRepCust(item));
                            break;
                        case "Addition":
                            CollectionOfSteps.Add(new AdditionCust(item));
                            break;
                        case "Subtraction":
                            CollectionOfSteps.Add(new SubtractionCust(item));
                            break;
                        case "Multiplication":
                            CollectionOfSteps.Add(new MultiplicationCust(item));
                            break;
                        case "Division":
                            CollectionOfSteps.Add(new DivisionCust(item));
                            break;
                        case "Exponent":
                            CollectionOfSteps.Add(new ExponentialCust(item));
                            break;
                        case "SignReversal":
                            CollectionOfSteps.Add(new SignReversalCust(item));
                            break;
                        case "AbsVal":
                            CollectionOfSteps.Add(new AbsValCust(item));
                            break;
                        case "RealComponent":
                            CollectionOfSteps.Add(new RealComponentCust(item));
                            break;
                        case "ImagComponent":
                            CollectionOfSteps.Add(new ImagComponentCust(item));
                            break;
                        case "Angle":
                            CollectionOfSteps.Add(new AngleCust(item));
                            break;
                        case "ComplexConj":
                            CollectionOfSteps.Add(new ComplexConjCust(item));
                            break;
                        case "CreatePhasor":
                            CollectionOfSteps.Add(new CreatePhasorCust(item));
                            break;
                        case "PowerCalc":
                            CollectionOfSteps.Add(new PowerCalcCust(item));
                            break;
                        case "SpecTypeUnit":
                            CollectionOfSteps.Add(new SpecTypeUnitCust(item));
                            break;
                        case "MetricPrefix":
                            CollectionOfSteps.Add(new MetricPrefixCust(item));
                            break;
                        case "AngleConversion":
                            CollectionOfSteps.Add(new AngleConversionCust(item));
                            break;
                        default:
                            throw new Exception("Error in reading data config customization steps, customization not recognized: " + name);
                    }
                }
            }
        }
    }
}