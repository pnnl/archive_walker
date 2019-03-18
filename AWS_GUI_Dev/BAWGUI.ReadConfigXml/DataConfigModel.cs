using BAWGUI.Core;
using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                    try
                    {
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
                            case "ReplicateSignal":
                                CollectionOfSteps.Add(new SignalReplicationCustModel(item));
                                break;
                            default:
                                throw new Exception("Error in reading data config customization steps, customization not recognized: " + name);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
        public ReaderPropertiesModel()
        {
            //DateTimeStart = "01/01/0001 00:00:00";
            //DateTimeEnd = "01/01/0001 00:00:00";
            DateTimeStart = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            DateTimeEnd = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        }
        public ReaderPropertiesModel(XElement xElement)
        {
            this._xElement = xElement;
            InputFileInfos = new List<InputFileInfoModel>();
            XmlSerializer serializer = null;
            try
            {
                serializer = new XmlSerializer(typeof(InputFileInfoModel), new XmlRootAttribute("FilePath"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (serializer != null)
            {
                foreach (var item in xElement.Elements("FilePath"))
                {
                    var b = item.CreateReader();
                    InputFileInfoModel a = null;
                    try
                    {
                        a = (InputFileInfoModel)serializer.Deserialize(b);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Data Source Error.\nException message: " + ex.Message + "\nInner exception message: " + ex.InnerException.Message);
                    }
                    if (a != null)
                    {
                        InputFileInfos.Add(a);
                    }
                }
            }
            var mode = xElement.Element("Mode");
            //var modeName = mode.Element("Name").Value;
            ModeName = (ModeType)Enum.Parse(typeof(ModeType), mode.Element("Name").Value);
            switch (ModeName)
            {
                case ModeType.Archive:
                    DateTimeStart = mode.Element("Params").Element("DateTimeStart").Value.Substring(0,19);
                    DateTimeEnd = mode.Element("Params").Element("DateTimeEnd").Value.Substring(0, 19);
                    break;
                case ModeType.RealTime:
                    NoFutureWait = mode.Element("Params").Element("NoFutureWait").Value;
                    MaxNoFutureCount = mode.Element("Params").Element("MaxNoFutureCount").Value;
                    FutureWait = mode.Element("Params").Element("FutureWait").Value;
                    MaxFutureCount = mode.Element("Params").Element("MaxFutureCount").Value;
                    UTCoffset = mode.Element("Params").Element("UTCoffset").Value;
                    break;
                case ModeType.Hybrid:
                    DateTimeStart = mode.Element("Params").Element("DateTimeStart").Value;
                    NoFutureWait = mode.Element("Params").Element("NoFutureWait").Value;
                    MaxNoFutureCount = mode.Element("Params").Element("MaxNoFutureCount").Value;
                    FutureWait = mode.Element("Params").Element("FutureWait").Value;
                    MaxFutureCount = mode.Element("Params").Element("MaxFutureCount").Value;
                    RealTimeRange = mode.Element("Params").Element("RealTimeRange").Value;
                    UTCoffset = mode.Element("Params").Element("UTCoffset").Value;
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
        public string UTCoffset { get; set; }
    }

    public class InputFileInfoModel
    {
        public InputFileInfoModel()
        {
            FileDirectory = "";
            FileType = DataFileType.pdat;
            Mnemonic = "";
            _exampleFile = "";
        }
        public string FileDirectory { get; set; }
        public DataFileType FileType { get; set; }
        public string Mnemonic { get; set; }
        private string _exampleFile;
        public string ExampleFile
        {
            get { return _exampleFile; }
            set
            {
                _exampleFile = value;
                if (File.Exists(value))
                {
                    //try
                    //{
                    //    FileType = Path.GetExtension(value).Substring(1);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Data file type not recognized. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    //}
                    //var filename = "";
                    //try
                    //{
                    //    filename = Path.GetFileNameWithoutExtension(value);
                    //}
                    //catch (ArgumentException ex)
                    //{
                    //    MessageBox.Show("Data file path contains one or more of the invalid characters. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    //}
                    //try
                    //{
                    //    Mnemonic = filename.Substring(0, filename.Length - 16);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Error extracting Mnemonic from selected data file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    //}
                    try
                    {
                        var fullPath = Path.GetDirectoryName(value);
                        var oneLevelUp = fullPath.Substring(0, fullPath.LastIndexOf(@"\"));
                        var twoLevelUp = oneLevelUp.Substring(0, oneLevelUp.LastIndexOf(@"\"));
                        FileDirectory = twoLevelUp;
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                }
                //else
                //{
                //    MessageBox.Show("Example input data file does not exist!", "Warning!", MessageBoxButtons.OK);
                //}
            }
        }
    }

    //public enum DataFileType
    //{
    //    [Description("PDAT")]
    //    pdat,
    //    [Description("JSIS CSV")]
    //    csv
    //}

    //public enum ModeType
    //{
    //    Archive,
    //    [Description("Real Time")]
    //    RealTime,
    //    Hybrid
    //}
    //public enum PowerType
    //{
    //    [Description("Complex")]
    //    CP,
    //    [Description("Apparent")]
    //    S,
    //    [Description("Active")]
    //    P,
    //    [Description("Reactive")]
    //    Q
    //}
    //public class SignalSignatures
    //{
    //    public SignalSignatures()
    //    {
    //    }
    //    public SignalSignatures(string pmu, string signal)
    //    {
    //        PMUName = pmu;
    //        SignalName = signal;
    //    }
    //    public string PMUName { get; set; }
    //    public string SignalName { get; set; }
    //}

    public class ToReplicate
    {
        public string PMUName { get; set; }
        public string Channel { get; set; }
    }
    public class ToConvert : ToReplicate
    {
        public string NewUnit { get; set; }
        public string SignalName { get; set; }
    }
    public class PMUElementForUnaryCustModel : ToReplicate
    {
        public PMUElementForUnaryCustModel()
        {
        }

        public PMUElementForUnaryCustModel(string pmu, string signalName, string custSignalName)
        {
            PMUName = pmu;
            Channel = signalName;
            CustSignalName = custSignalName;
        }
        public string CustSignalName { get; set; }
    }

    public class PMUElementPairModel
    {
        public SignalSignatures PMUElement1 { get; set; }
        public SignalSignatures PMUElement2 { get; set; }
        public string CustSignalName { get; set; }
    }

    public class PhasorToPower : PMUElementPairModel
    {
    }
    public class MagAngToPower : PMUElementPairModel
    {
        public SignalSignatures PMUElement3 { get; set; }
        public SignalSignatures PMUElement4 { get; set; }
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
        public static List<SignalSignatures> ReadPMUElements(XElement item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            else
            {
                var newList = new List<SignalSignatures>();
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
                            var newElement = new SignalSignatures(pmuName, channelName);
                            newList.Add(newElement);
                        }
                    }
                    else
                    {
                        var newElement = new SignalSignatures();
                        newElement.PMUName = pmuName;
                        newList.Add(newElement);
                    }
                }
                return newList;
            }
        }
    }
}
