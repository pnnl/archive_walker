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
    public class DataConfigModel
    {
        private XElement _xElement;
        //private ReaderProperties _readerProperties;
        public ReaderPropertiesModel ReaderProperty { get; set; }
        //private List<Object> _collectionOfSteps;
        public List<object> CollectionOfSteps { get; set; }

        public DataConfigModel(XElement xElement)
        {
            this._xElement = xElement;
            ReaderProperty = new ReaderPropertiesModel(_xElement.Element("Configuration").Element("ReaderProperties"));
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

        public DataConfigModel()
        {
            CollectionOfSteps = new List<object>();
            ReaderProperty = new ReaderPropertiesModel();
        }
    }

    public class ReaderPropertiesModel
    {
        private XElement _xElement;
        public ReaderPropertiesModel() { }
        public ReaderPropertiesModel(XElement xElement)
        {
            this._xElement = xElement;
            InputFileInfos = new List<InputFileInfoModel>();
            var serializer = new XmlSerializer(typeof(InputFileInfoModel), new XmlRootAttribute("FilePath"));
            foreach (var item in xElement.Elements("FilePath"))
            {
                var b = item.CreateReader();
                var a = (InputFileInfoModel)serializer.Deserialize(b);
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

        public List<InputFileInfoModel> InputFileInfos { get; set; }
        public ModeType ModeName { get; set; }
        public string DateTimeStart { get; set; }
        public string DateTimeEnd { get; set; }
        public string NoFutureWait { get; set; }
        public string MaxNoFutureCount { get; set; }
        public string FutureWait { get; set; }
        public string MaxFutureCount { get; set; }
        public string RealTimeRange { get; set; }
    }

    public class InputFileInfoModel
    {
        public InputFileInfoModel()
        {
            FileDirectory = "";
            FileType = "pdat";
            Mnemonic = "";
        }
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
    public class PMUElementModel
    {
        public PMUElementModel()
        {
        }
        public PMUElementModel(string pmu, string signal)
        {
            PMUName = pmu;
            SignalName = signal;
        }
        public string PMUName { get; set; }
        public string SignalName { get; set; }
    }

    public class PMUElementForUnaryCustModel
    {
        public PMUElementForUnaryCustModel()
        {
        }

        public PMUElementForUnaryCustModel(string pmu, string signalName, string custSignalName)
        {
            PMUName = pmu;
            SignalName = signalName;
            CustSignalName = custSignalName;
        }

        public string PMUName { get; set; }
        public string SignalName { get; set; }
        public string CustSignalName { get; set; }
    }

    public class PMUElementPairModel
    {
        public PMUElementModel PMUElement1 { get; set; }
        public PMUElementModel PMUElement2 { get; set; }
        public string CustSignalName { get; set; }
    }

    public class PhasorToPower : PMUElementPairModel
    {
    }
    public class MagAngToPower : PMUElementPairModel
    {
        public PMUElementModel PMUElement3 { get; set; }
        public PMUElementModel PMUElement4 { get; set; }
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
        public static List<PMUElementModel> ReadPMUElements(XElement item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            else
            {
                var newList = new List<PMUElementModel>();
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
                                var newElement = new PMUElementModel(pmuName, channelName);
                            newList.Add(newElement);
                            }
                        }
                        else
                        {
                            var newElement = new PMUElementModel();
                            newElement.PMUName = pmuName;
                        newList.Add(newElement);
                        }
                    }
                return newList;
            }
        }
    }
}
