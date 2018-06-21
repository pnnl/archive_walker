using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BAWGUI.ReadConfigXml
{
    public class DataConfig
    {
        private XElement _xElement;
        //private ReaderProperties _readerProperties;
        public ReaderProperties ReaderProperty { get; set; }
        //private List<Object> _collectionOfSteps;
        public List<object> CollectionOfSteps { get; set; }

        public DataConfig(XElement xElement)
        {
            this._xElement = xElement;
            ReaderProperty = new ReaderProperties(_xElement.Element("Configuration").Element("ReaderProperties"));
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

    public class ReaderProperties
    {
        private XElement _xElement;

        public ReaderProperties(XElement xElement)
        {
            this._xElement = xElement;
            InputFileInfos = new List<InputFileInfo>();
            var serializer = new XmlSerializer(typeof(InputFileInfo), new XmlRootAttribute("FilePath"));
            foreach (var item in xElement.Elements("FilePath"))
            {
                var b = item.CreateReader();
                var a = (InputFileInfo)serializer.Deserialize(b);
                InputFileInfos.Add(a);
            }
            var mode = xElement.Element("Mode");
            //var modeName = mode.Element("Name").Value;
            ModeName = (ModeType)Enum.Parse(typeof(ModeType), mode.Element("Name").Value);
            switch (ModeName)
            {
                case ModeType.Archive:
                    DateTimeStart = mode.Element("Params").Element("DateTimeStart").Value;
                    DateTimeEnd = mode.Element("Params").Element("DateTimeEnd").Value;
                    break;
                case ModeType.RealTime:
                    NoFutureWait = mode.Element("Params").Element("NoFutureWait").Value;
                    MaxNoFutureCount = mode.Element("Params").Element("MaxNoFutureCount").Value;
                    FutureWait = mode.Element("Params").Element("FutureWait").Value;
                    MaxFutureCount = mode.Element("Params").Element("MaxFutureCount").Value;
                    break;
                case ModeType.Hybrid:
                    DateTimeStart = mode.Element("Params").Element("DateTimeStart").Value;
                    NoFutureWait = mode.Element("Params").Element("NoFutureWait").Value;
                    MaxNoFutureCount = mode.Element("Params").Element("MaxNoFutureCount").Value;
                    FutureWait = mode.Element("Params").Element("FutureWait").Value;
                    MaxFutureCount = mode.Element("Params").Element("MaxFutureCount").Value;
                    RealTimeRange = mode.Element("Params").Element("RealTimeRange").Value;
                    break;
                default:
                    throw new Exception("Mode type not recognized.");
            }
        }

        public List<InputFileInfo> InputFileInfos { get; set; }
        public ModeType ModeName { get; set; }
        public string DateTimeStart { get; set; }
        public string DateTimeEnd { get; set; }
        public string NoFutureWait { get; set; }
        public string MaxNoFutureCount { get; set; }
        public string FutureWait { get; set; }
        public string MaxFutureCount { get; set; }
        public string RealTimeRange { get; set; }
    }

    public class InputFileInfo
    {
        public string FileDirectory { get; set; }
        public string FileType { get; set; }
        public string Mnemonic { get; set; }
    }

    public enum DataFileType
    {
        [Description("PDAT")]
        pdat,
        [Description("JSIS CSV")]
        csv
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
    public class PMUElement
    {
        public PMUElement()
        {
        }
        public PMUElement(string pmu, string signal)
        {
            PMUName = pmu;
            SignalName = signal;
        }
        public string PMUName { get; set; }
        public string SignalName { get; set; }
    }

    public class PMUElementForUnaryCust
    {
        public PMUElementForUnaryCust()
        {
        }

        public PMUElementForUnaryCust(string pmu, string signalName, string custSignalName)
        {
            PMUName = pmu;
            SignalName = signalName;
            CustSignalName = custSignalName;
        }

        public string PMUName { get; set; }
        public string SignalName { get; set; }
        public string CustSignalName { get; set; }
    }

    public class PMUElementPair
    {
        public PMUElement PMUElement1 { get; set; }
        public PMUElement PMUElement2 { get; set; }
        public string CustSignalName { get; set; }
    }

    public class PhasorToPower : PMUElementPair
    {
    }
    public class MagAngToPower : PMUElementPair
    {
        public PMUElement PMUElement3 { get; set; }
        public PMUElement PMUElement4 { get; set; }
    }
    public class PMUWithSamplingRate
    {
    }

    //namespace BAWGUI.Readers
    //{
    //    public class DataConfig
    //    {
    //        public DataConfig()
    //        {
    //            DQFilterNameDictionary = new Dictionary<string, string>() { { "Status Flags", "PMUflagFilt" }, { "Zeros", "DropOutZeroFilt" }, { "Missing", "DropOutMissingFilt" }, { "Nominal Voltage", "VoltPhasorFilt" }, { "Nominal Frequency", "FreqFilt" }, { "Outliers", "OutlierFilt" }, { "Stale Data", "StaleFilt" }, { "Data Frame", "DataFrameFilt" }, { "Channel", "PMUchanFilt" }, { "Entire PMU", "PMUallFilt" }, { "Angle Wrapping", "WrappingFailureFilt" } };
    //            DQFilterReverseNameDictionary = DQFilterNameDictionary.ToDictionary(x => x.Value, x => x.Key);
    //            CustomizationNameDictionary = new Dictionary<string, string>() { { "Scalar Repetition", "ScalarRep" }, { "Addition", "Addition" }, { "Subtraction", "Subtraction" }, { "Multiplication", "Multiplication" }, { "Division", "Division" }, { "Exponential", "Exponent" }, { "Sign Reversal", "SignReversal" }, { "Absolute Value", "AbsVal" }, { "Real Component", "RealComponent" }, { "Imaginary Component", "ImagComponent" }, { "Angle Calculation", "Angle" }, { "Complex Conjugate", "ComplexConj" }, { "Phasor Creation", "CreatePhasor" }, { "Power Calculation", "PowerCalc" }, { "Signal Type/Unit", "SpecTypeUnit" }, { "Metric Prefix", "MetricPrefix" }, { "Angle Conversion", "AngleConversion" } };
    //            CustomizationReverseNameDictionary = CustomizationNameDictionary.ToDictionary(x => x.Value, x => x.Key);
    //        }
    //        public new List<object> CollectionOfSteps { get; set; }
    //        public Dictionary<string, string> DQFilterNameDictionary { get; set; }
    //        public Dictionary<string, string> DQFilterReverseNameDictionary { get; set; }
    //        public Dictionary<string, string> CustomizationNameDictionary { get; set; }
    //        public Dictionary<string, string> CustomizationReverseNameDictionary { get; set; }
    //    }
    //}
    public static class PMUElementReader
    {
        public static List<PMUElement> ReadPMUElements(XElement item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            else
            {
                var newList = new List<PMUElement>();
                var inputs = item.Elements("PMU");
                    foreach (var aInput in inputs)
                    {
                        var pmuName = aInput.Element("Name").Value;
                        var channels = aInput.Elements("Channel");
                        if (channels.Count() > 0)
                        {
                            foreach (var channel in channels)
                            {
                                var channelName = channel.Element("Name").Value;
                                var newElement = new PMUElement(pmuName, channelName);
                            newList.Add(newElement);
                            }
                        }
                        else
                        {
                            var newElement = new PMUElement();
                            newElement.PMUName = pmuName;
                        newList.Add(newElement);
                        }
                    }
                return newList;
            }
        }
    }
}
