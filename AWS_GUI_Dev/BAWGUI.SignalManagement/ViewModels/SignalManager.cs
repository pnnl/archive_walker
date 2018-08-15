using BAWGUI.Core;
using BAWGUI.CSVDataReader.CSVDataReader;
using BAWGUI.ReadConfigXml;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using PDAT_Reader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BAWGUI.SignalManagement.ViewModels
{
    public class SignalManager:ViewModelBase
    {
        private SignalManager()
        {
            FileInfo = new ObservableCollection<InputFileInfoViewModel>();
            _groupedRawSignalsByType = new ObservableCollection<SignalTypeHierachy>();
            _reGroupedRawSignalsByType = new ObservableCollection<SignalTypeHierachy>();
            _groupedRawSignalsByPMU = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByDataConfigStepsInput = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByDataConfigStepsOutput = new ObservableCollection<SignalTypeHierachy>();
            _allDataConfigOutputGroupedByType = new ObservableCollection<SignalTypeHierachy>();
            _allDataConfigOutputGroupedByPMU = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByProcessConfigStepsInput = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByProcessConfigStepsOutput = new ObservableCollection<SignalTypeHierachy>();
            _allProcessConfigOutputGroupedByType = new ObservableCollection<SignalTypeHierachy>();
            _allProcessConfigOutputGroupedByPMU = new ObservableCollection<SignalTypeHierachy>();

            _groupedSignalByPostProcessConfigStepsInput = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByPostProcessConfigStepsOutput = new ObservableCollection<SignalTypeHierachy>();
            _allPostProcessOutputGroupedByType = new ObservableCollection<SignalTypeHierachy>();
            _allPostProcessOutputGroupedByPMU = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByDetectorInput = new ObservableCollection<SignalTypeHierachy>();
            _engine = MatLabEngine.Instance;
        }

        public void cleanUp()
        {
            FileInfo = new ObservableCollection<InputFileInfoViewModel>();
            _groupedRawSignalsByType = new ObservableCollection<SignalTypeHierachy>();
            _reGroupedRawSignalsByType = new ObservableCollection<SignalTypeHierachy>();
            _groupedRawSignalsByPMU = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByDataConfigStepsInput = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByDataConfigStepsOutput = new ObservableCollection<SignalTypeHierachy>();
            _allDataConfigOutputGroupedByType = new ObservableCollection<SignalTypeHierachy>();
            _allDataConfigOutputGroupedByPMU = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByProcessConfigStepsInput = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByProcessConfigStepsOutput = new ObservableCollection<SignalTypeHierachy>();
            _allProcessConfigOutputGroupedByType = new ObservableCollection<SignalTypeHierachy>();
            _allProcessConfigOutputGroupedByPMU = new ObservableCollection<SignalTypeHierachy>();

            _groupedSignalByPostProcessConfigStepsInput = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByPostProcessConfigStepsOutput = new ObservableCollection<SignalTypeHierachy>();
            _allPostProcessOutputGroupedByType = new ObservableCollection<SignalTypeHierachy>();
            _allPostProcessOutputGroupedByPMU = new ObservableCollection<SignalTypeHierachy>();
            _groupedSignalByDetectorInput = new ObservableCollection<SignalTypeHierachy>();
        }
        private MatLabEngine _engine;

        private static SignalManager _instance = null;
        public static SignalManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SignalManager();
                }
                return _instance;
            }
        }
        public void AddRawSignals(List<InputFileInfoModel> inputFileInfos)
        {
            foreach (var item in inputFileInfos)
            {
                if (!File.Exists(item.ExampleFile))
                {
                    //item.ExampleFile = Utility.FindFirstInputFile(item.FileDirectory, item.FileType);
                    MessageBox.Show("Example input data file does not exist!", "Warning!", MessageBoxButtons.OK);
                }
                else
                {
                    var aFileInfo = new InputFileInfoViewModel(item);
                    if (item.FileType.ToLower() == "csv")
                    {
                        _readCSVFile(aFileInfo);
                    }
                    else
                    {
                        _readPDATFile(aFileInfo);
                    }
                    FileInfo.Add(aFileInfo);
                }
            }
            AllPMUs = _getAllPMU();
        }
        private void _readCSVFile(InputFileInfoViewModel aFileInfo)
        {
            var csvReader = new CSVReader(aFileInfo.ExampleFile);
            var pmuName = csvReader.pmuName;
            var SamplingRate = csvReader.SamplingRate;
            var signalNames = csvReader.signalNames;
            var signalTypes = csvReader.signalTypes;
            var signalUnits = csvReader.signalUnits;
            var signalList = new List<string>();
            var signalSignatureList = new ObservableCollection<SignalSignatureViewModel>();
            for (var index = 0; index <= signalNames.Count - 1; index++)
            {
                var newSignal = new SignalSignatureViewModel();
                newSignal.PMUName = pmuName;
                newSignal.Unit = signalUnits[index];
                newSignal.SignalName = signalNames[index];
                newSignal.SamplingRate = (int)SamplingRate;
                signalList.Add(signalNames[index]);
                switch (signalTypes[index])
                {
                    case "VPM":
                        {
                            // signalName = signalNames(index).Split(".")(0) & ".VMP"
                            // signalName = signalNames(index)
                            newSignal.TypeAbbreviation = "VMP";
                            break;
                        }

                    case "VPA":
                        {
                            // signalName = signalNames(index).Split(".")(0) & ".VAP"
                            // signalName = signalNames(index)
                            newSignal.TypeAbbreviation = "VAP";
                            break;
                        }

                    case "IPM":
                        {
                            // signalName = signalNames(index).Split(".")(0) & ".IMP"
                            // signalName = signalNames(index)
                            newSignal.TypeAbbreviation = "IMP";
                            break;
                        }

                    case "IPA":
                        {
                            // signalName = signalNames(index).Split(".")(0) & ".IAP"
                            // signalName = signalNames(index)
                            newSignal.TypeAbbreviation = "IAP";
                            break;
                        }

                    case "F":
                        {
                            // signalName = signalNames(index)
                            newSignal.TypeAbbreviation = "F";
                            break;
                        }

                    case "P":
                        {
                            // signalName = signalNames(index)
                            newSignal.TypeAbbreviation = "P";
                            break;
                        }

                    case "Q":
                        {
                            // signalName = signalNames(index)
                            newSignal.TypeAbbreviation = "Q";
                            break;
                        }

                    default:
                        {
                            throw new Exception("Error! Invalid signal type " + signalTypes[index] + " found in file: " + aFileInfo.ExampleFile + " !");
                        }
                }
                newSignal.OldSignalName = newSignal.SignalName;
                newSignal.OldTypeAbbreviation = newSignal.TypeAbbreviation;
                newSignal.OldUnit = newSignal.Unit;
                signalSignatureList.Add(newSignal);
            }
            aFileInfo.SignalList = signalList;
            aFileInfo.TaggedSignals = signalSignatureList;
            aFileInfo.SamplingRate = (int)SamplingRate;
            var newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
            newSig.SamplingRate = (int)SamplingRate;
            var a = new SignalTypeHierachy(newSig);
            a.SignalList = SortSignalByPMU(signalSignatureList);
            GroupedRawSignalsByPMU.Add(a);
            //newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
            //newSig.SamplingRate = (int)SamplingRate;
            var b = new SignalTypeHierachy(newSig);
            b.SignalList = SortSignalByType(signalSignatureList);
            GroupedRawSignalsByType.Add(b);
            ReGroupedRawSignalsByType = GroupedRawSignalsByType;
        }
        //private void _readPDATFile(InputFileInfoViewModel aFileInfo)
        //{
        //    PDATReader PDATSampleFile = new PDATReader();
        //    try
        //    {
        //        aFileInfo.SignalList = PDATSampleFile.GetPDATSignalNameList(aFileInfo.ExampleFile);
        //        aFileInfo.SamplingRate = PDATSampleFile.GetSamplingRate();
        //        TagSignals(aFileInfo, aFileInfo.SignalList);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("PDAT Reading error! " + ex.Message);
        //    }
        //}
        private void _readPDATFile(InputFileInfoViewModel aFileInfo)
        {
            var signalInformation = _engine.ReadPDATSampleFile(aFileInfo.ExampleFile);
            aFileInfo.SamplingRate = signalInformation.SamplingRate;
            ObservableCollection<SignalSignatureViewModel> newSignalList = new ObservableCollection<SignalSignatureViewModel>();
            for (int idx = 0; idx < signalInformation.PMUSignalsList.Count; idx++)
            {
                var thisPMU = signalInformation.PMUSignalsList[idx];
                var thisPMUName = signalInformation.PMUSignalsList[idx].PMUname;
                for (int idx2 = 0; idx2 < thisPMU.SignalNames.Count; idx2++)
                {
                    var aSignal = new SignalSignatureViewModel();
                    aSignal.SamplingRate = aFileInfo.SamplingRate;
                    aSignal.PMUName = thisPMUName;
                    aSignal.SignalName = thisPMU.SignalNames[idx2];
                    aSignal.Unit = thisPMU.SignalUnits[idx2];
                    aSignal.TypeAbbreviation = thisPMU.SignalTypes[idx2];
                    aSignal.OldSignalName = aSignal.SignalName;
                    aSignal.OldTypeAbbreviation = aSignal.TypeAbbreviation;
                    aSignal.OldUnit = aSignal.Unit;
                    newSignalList.Add(aSignal);
                }
            }
            aFileInfo.TaggedSignals = newSignalList;
            var newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
            newSig.SamplingRate = aFileInfo.SamplingRate;
            var a = new SignalTypeHierachy(newSig);
            a.SignalList = SortSignalByPMU(newSignalList);
            GroupedRawSignalsByPMU.Add(a);
            var b = new SignalTypeHierachy(newSig);
            b.SignalList = SortSignalByType(newSignalList);
            GroupedRawSignalsByType.Add(b);
            ReGroupedRawSignalsByType = GroupedRawSignalsByType;
        }
        public ObservableCollection<InputFileInfoViewModel> FileInfo { get; set; }
        public ObservableCollection<SignalTypeHierachy> SortSignalByType(ObservableCollection<SignalSignatureViewModel> signalList)
        {
            ObservableCollection<SignalTypeHierachy> signalTypeTreeGroupedBySamplingRate = new ObservableCollection<SignalTypeHierachy>();
            var signalTypeGroupBySamplingRate = signalList.GroupBy(x => x.SamplingRate);
            foreach (var rateGroup in signalTypeGroupBySamplingRate)
            {
                var rate = rateGroup.Key;
                var subSignalGroup = rateGroup.ToList();
                ObservableCollection<SignalTypeHierachy> signalTypeTree = new ObservableCollection<SignalTypeHierachy>();
                var signalTypeDictionary = subSignalGroup.GroupBy(x => x.TypeAbbreviation.ToArray()[0].ToString())/*.OrderBy(x=>x.Key)*/.ToDictionary(x => x.Key, x => new ObservableCollection<SignalSignatureViewModel>(x.ToList()));
                foreach (var signalType in signalTypeDictionary)
                {
                    switch (signalType.Key)
                    {
                        case "S":
                            {
                                var groups = signalType.Value.GroupBy(x => x.TypeAbbreviation);
                                foreach (var group in groups)
                                {
                                    switch (group.Key)
                                    {
                                        case "S":
                                            {
                                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Apparent"));
                                                newHierachy.SignalSignature.TypeAbbreviation = "S";
                                                foreach (var signal in group)
                                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                                signalTypeTree.Add(newHierachy);
                                                break;
                                            }

                                        case "SC":
                                            {
                                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Scalar"));
                                                newHierachy.SignalSignature.TypeAbbreviation = "SC";
                                                foreach (var signal in group)
                                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                                signalTypeTree.Add(newHierachy);
                                                break;
                                            }

                                        default:
                                            {
                                                throw new Exception("Unknown signal type: " + group.Key + "found!");
                                            }
                                    }
                                }

                                break;
                            }

                        case "O":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Other"));
                                newHierachy.SignalSignature.TypeAbbreviation = "OTHER";
                                foreach (var signal in signalType.Value)
                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        case "C":
                            {
                                // Dim groups = signalType.Value.GroupBy(Function(x) x.TypeAbbreviation).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList))
                                var groups = signalType.Value.GroupBy(x => x.TypeAbbreviation);
                                foreach (var group in groups)
                                {
                                    switch (group.Key)
                                    {
                                        case "C":
                                            {
                                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("CustomizedSignal"));
                                                newHierachy.SignalSignature.TypeAbbreviation = "C";
                                                foreach (var signal in group)
                                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                                signalTypeTree.Add(newHierachy);
                                                break;
                                            }

                                        case "CP":
                                            {
                                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Complex"));
                                                newHierachy.SignalSignature.TypeAbbreviation = "CP";
                                                foreach (var signal in group)
                                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                                signalTypeTree.Add(newHierachy);
                                                break;
                                            }

                                        default:
                                            {
                                                throw new Exception("Unknown signal type: " + group.Key + "found!");
                                            }
                                    }
                                }

                                break;
                            }

                        case "D":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Digital"));
                                newHierachy.SignalSignature.TypeAbbreviation = "D";
                                foreach (var signal in signalType.Value)
                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        case "F":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Frequency"));
                                newHierachy.SignalSignature.TypeAbbreviation = "F";
                                foreach (var signal in signalType.Value)
                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        case "R":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Rate of Change of Frequency"));
                                newHierachy.SignalSignature.TypeAbbreviation = "R";
                                foreach (var signal in signalType.Value)
                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        case "Q":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Reactive Power"));
                                newHierachy.SignalSignature.TypeAbbreviation = "Q";
                                foreach (var signal in signalType.Value)
                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        case "P":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Active Power"));
                                newHierachy.SignalSignature.TypeAbbreviation = "P";
                                foreach (var signal in signalType.Value)
                                    newHierachy.SignalList.Add(new SignalTypeHierachy(signal));
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        case "V":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Voltage"));
                                newHierachy.SignalSignature.TypeAbbreviation = "V";
                                var voltageHierachy = signalType.Value.GroupBy(y => y.TypeAbbreviation.ToArray()[1].ToString());
                                foreach (var group in voltageHierachy)
                                {
                                    switch (group.Key)
                                    {
                                        case "M":
                                            {
                                                var mGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Magnitude"));
                                                mGroup.SignalSignature.TypeAbbreviation = "VM";
                                                var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                foreach (var phase in mGroupHierachky)
                                                {
                                                    switch (phase.Key)
                                                    {
                                                        case "P":
                                                            {
                                                                var positiveGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Positive Sequence"));
                                                                positiveGroup.SignalSignature.TypeAbbreviation = "VMP";
                                                                foreach (var signal in phase)
                                                                    positiveGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(positiveGroup);
                                                                break;
                                                            }

                                                        case "A":
                                                            {
                                                                var AGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                AGroup.SignalSignature.TypeAbbreviation = "VMA";
                                                                foreach (var signal in phase)
                                                                    AGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(AGroup);
                                                                break;
                                                            }

                                                        case "B":
                                                            {
                                                                var BGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                BGroup.SignalSignature.TypeAbbreviation = "VMB";
                                                                foreach (var signal in phase)
                                                                    BGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(BGroup);
                                                                break;
                                                            }

                                                        case "C":
                                                            {
                                                                var CGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                CGroup.SignalSignature.TypeAbbreviation = "VMC";
                                                                foreach (var signal in phase)
                                                                    CGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(CGroup);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage magnitude!");
                                                            }
                                                    }
                                                }
                                                newHierachy.SignalList.Add(mGroup);
                                                break;
                                            }

                                        case "A":
                                            {
                                                var aGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Angle"));
                                                aGroup.SignalSignature.TypeAbbreviation = "VA";
                                                var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                foreach (var phase in aGroupHierachy)
                                                {
                                                    switch (phase.Key)
                                                    {
                                                        case "P":
                                                            {
                                                                var positiveGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Positive Sequence"));
                                                                positiveGroup.SignalSignature.TypeAbbreviation = "VAP";
                                                                foreach (var signal in phase)
                                                                    positiveGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(positiveGroup);
                                                                break;
                                                            }

                                                        case "A":
                                                            {
                                                                var GroupA = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                GroupA.SignalSignature.TypeAbbreviation = "VAA";
                                                                foreach (var signal in phase)
                                                                    GroupA.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupA);
                                                                break;
                                                            }

                                                        case "B":
                                                            {
                                                                var GroupB = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                GroupB.SignalSignature.TypeAbbreviation = "VAB";
                                                                foreach (var signal in phase)
                                                                    GroupB.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupB);
                                                                break;
                                                            }

                                                        case "C":
                                                            {
                                                                var GroupC = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                GroupC.SignalSignature.TypeAbbreviation = "VAC";
                                                                foreach (var signal in phase)
                                                                    GroupC.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupC);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                            }
                                                    }
                                                }
                                                newHierachy.SignalList.Add(aGroup);
                                                break;
                                            }

                                        case "P":
                                            {
                                                var aGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phasor"));
                                                aGroup.SignalSignature.TypeAbbreviation = "VP";
                                                var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                foreach (var phase in aGroupHierachy)
                                                {
                                                    switch (phase.Key)
                                                    {
                                                        case "P":
                                                            {
                                                                var positiveGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Positive Sequence"));
                                                                positiveGroup.SignalSignature.TypeAbbreviation = "VPP";
                                                                foreach (var signal in phase)
                                                                    positiveGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(positiveGroup);
                                                                break;
                                                            }

                                                        case "A":
                                                            {
                                                                var GroupA = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                GroupA.SignalSignature.TypeAbbreviation = "VPA";
                                                                foreach (var signal in phase)
                                                                    GroupA.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupA);
                                                                break;
                                                            }

                                                        case "B":
                                                            {
                                                                var GroupB = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                GroupB.SignalSignature.TypeAbbreviation = "VPB";
                                                                foreach (var signal in phase)
                                                                    GroupB.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupB);
                                                                break;
                                                            }

                                                        case "C":
                                                            {
                                                                var GroupC = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                GroupC.SignalSignature.TypeAbbreviation = "VPC";
                                                                foreach (var signal in phase)
                                                                    GroupC.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupC);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                            }
                                                    }
                                                }
                                                newHierachy.SignalList.Add(aGroup);
                                                break;
                                            }

                                        default:
                                            {
                                                throw new Exception("Error! Invalid voltage signal type found: " + group.Key);
                                            }
                                    }
                                }
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        case "I":
                            {
                                var newHierachy = new SignalTypeHierachy(new SignalSignatureViewModel("Current"));
                                newHierachy.SignalSignature.TypeAbbreviation = "I";
                                var currentHierachy = signalType.Value.GroupBy(y => y.TypeAbbreviation.ToArray()[1].ToString());
                                foreach (var group in currentHierachy)
                                {
                                    switch (group.Key)
                                    {
                                        case "M":
                                            {
                                                var mGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Magnitude"));
                                                mGroup.SignalSignature.TypeAbbreviation = "IM";
                                                var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                foreach (var phase in mGroupHierachky)
                                                {
                                                    switch (phase.Key)
                                                    {
                                                        case "P":
                                                            {
                                                                var positiveGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Positive Sequence"));
                                                                positiveGroup.SignalSignature.TypeAbbreviation = "IMP";
                                                                foreach (var signal in phase)
                                                                    positiveGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(positiveGroup);
                                                                break;
                                                            }

                                                        case "A":
                                                            {
                                                                var AGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                AGroup.SignalSignature.TypeAbbreviation = "IMA";
                                                                foreach (var signal in phase)
                                                                    AGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(AGroup);
                                                                break;
                                                            }

                                                        case "B":
                                                            {
                                                                var BGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                BGroup.SignalSignature.TypeAbbreviation = "IMB";
                                                                foreach (var signal in phase)
                                                                    BGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(BGroup);
                                                                break;
                                                            }

                                                        case "C":
                                                            {
                                                                var CGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                CGroup.SignalSignature.TypeAbbreviation = "IMC";
                                                                foreach (var signal in phase)
                                                                    CGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                mGroup.SignalList.Add(CGroup);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage magnitude!");
                                                            }
                                                    }
                                                }
                                                newHierachy.SignalList.Add(mGroup);
                                                break;
                                            }

                                        case "A":
                                            {
                                                var aGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Angle"));
                                                aGroup.SignalSignature.TypeAbbreviation = "IA";
                                                var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                foreach (var phase in aGroupHierachy)
                                                {
                                                    switch (phase.Key)
                                                    {
                                                        case "P":
                                                            {
                                                                var positiveGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Positive Sequence"));
                                                                positiveGroup.SignalSignature.TypeAbbreviation = "IAP";
                                                                foreach (var signal in phase)
                                                                    positiveGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(positiveGroup);
                                                                break;
                                                            }

                                                        case "A":
                                                            {
                                                                var GroupA = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                GroupA.SignalSignature.TypeAbbreviation = "IAA";
                                                                foreach (var signal in phase)
                                                                    GroupA.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupA);
                                                                break;
                                                            }

                                                        case "B":
                                                            {
                                                                var GroupB = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                GroupB.SignalSignature.TypeAbbreviation = "IAB";
                                                                foreach (var signal in phase)
                                                                    GroupB.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupB);
                                                                break;
                                                            }

                                                        case "C":
                                                            {
                                                                var GroupC = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                GroupC.SignalSignature.TypeAbbreviation = "IAC";
                                                                foreach (var signal in phase)
                                                                    GroupC.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupC);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                            }
                                                    }
                                                }
                                                newHierachy.SignalList.Add(aGroup);
                                                break;
                                            }

                                        case "P":
                                            {
                                                var aGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phasor"));
                                                aGroup.SignalSignature.TypeAbbreviation = "IP";
                                                var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                foreach (var phase in aGroupHierachy)
                                                {
                                                    switch (phase.Key)
                                                    {
                                                        case "P":
                                                            {
                                                                var positiveGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Positive Sequence"));
                                                                positiveGroup.SignalSignature.TypeAbbreviation = "IPP";
                                                                foreach (var signal in phase)
                                                                    positiveGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(positiveGroup);
                                                                break;
                                                            }

                                                        case "A":
                                                            {
                                                                var GroupA = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                GroupA.SignalSignature.TypeAbbreviation = "IPA";
                                                                foreach (var signal in phase)
                                                                    GroupA.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupA);
                                                                break;
                                                            }

                                                        case "B":
                                                            {
                                                                var GroupB = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                GroupB.SignalSignature.TypeAbbreviation = "IPB";
                                                                foreach (var signal in phase)
                                                                    GroupB.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupB);
                                                                break;
                                                            }

                                                        case "C":
                                                            {
                                                                var GroupC = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                GroupC.SignalSignature.TypeAbbreviation = "IPC";
                                                                foreach (var signal in phase)
                                                                    GroupC.SignalList.Add(new SignalTypeHierachy(signal));
                                                                aGroup.SignalList.Add(GroupC);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                            }
                                                    }
                                                }
                                                newHierachy.SignalList.Add(aGroup);
                                                break;
                                            }

                                        default:
                                            {
                                                throw new Exception("Error! Invalid voltage signal type found: " + group.Key);
                                            }
                                    }
                                }
                                signalTypeTree.Add(newHierachy);
                                break;
                            }

                        default:
                            {
                                throw new Exception("Error! Invalid signal type found: " + signalType.Key);
                            }
                    }
                }
                var newSig = new SignalSignatureViewModel("Sampling Rate: " + rate.ToString() + "/Second");
                newSig.SamplingRate = rate;
                var a = new SignalTypeHierachy(newSig);
                a.SignalList = signalTypeTree;
                signalTypeTreeGroupedBySamplingRate.Add(a);
            }
            return signalTypeTreeGroupedBySamplingRate;
        }
        public ObservableCollection<SignalTypeHierachy> SortSignalByPMU(ObservableCollection<SignalSignatureViewModel> signalList)
        {
            var groupBySamplingRate = signalList.GroupBy(x => x.SamplingRate);
            var pmuSignalTreeGroupedBySamplingRate = new ObservableCollection<SignalTypeHierachy>();
            foreach (var group in groupBySamplingRate)
            {
                var rate = group.Key;
                var subSignalList = group.ToList();
                var PMUSignalDictionary = new Dictionary<string, List<SignalSignatureViewModel>>();
                try
                {
                    var pairs = subSignalList.GroupBy(x => x.PMUName);
                    PMUSignalDictionary = pairs.ToDictionary(x => x.Key, x => x.ToList());
                }
                catch (Exception ex)
                {
                    throw new Exception("Missing PMU name. " + ex.Message);
                }
                var pmuSignalTree = new ObservableCollection<SignalTypeHierachy>();
                foreach (var subgroup in PMUSignalDictionary)
                {
                    var newPMUSignature = new SignalSignatureViewModel(subgroup.Key, subgroup.Key);
                    var newGroup = new SignalTypeHierachy(newPMUSignature);
                    foreach (var signal in subgroup.Value)
                        newGroup.SignalList.Add(new SignalTypeHierachy(signal));
                    newGroup.SignalSignature.SamplingRate = subgroup.Value.FirstOrDefault().SamplingRate;
                    pmuSignalTree.Add(newGroup);
                }
                var newSig = new SignalSignatureViewModel("Sampling Rate: " + rate.ToString() + "/Second");
                newSig.SamplingRate = rate;
                var a = new SignalTypeHierachy(newSig);
                a.SignalList = pmuSignalTree;
                pmuSignalTreeGroupedBySamplingRate.Add(a);
            }
            // Dim PMUSignalDictionary = subSignalList.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
            // Dim pmuSignalTree = New ObservableCollection(Of SignalTypeHierachy)
            // For Each group In PMUSignalDictionary
            // Dim newPMUSignature = New SignalSignatures(group.Key, group.Key)
            // Dim newGroup = New SignalTypeHierachy(newPMUSignature)
            // For Each signal In group.Value
            // newGroup.SignalList.Add(New SignalTypeHierachy(signal))
            // Next
            // newGroup.SignalSignature.SamplingRate = group.Value.FirstOrDefault.SamplingRate
            // pmuSignalTree.Add(newGroup)
            // Next
            return pmuSignalTreeGroupedBySamplingRate;
        }
        public void TagSignals(InputFileInfoViewModel fileInfo, List<string> signalList)
        {
            ObservableCollection<SignalSignatureViewModel> newSignalList = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var name in signalList)
            {
                SignalSignatureViewModel signal = new SignalSignatureViewModel();
                // signal.SignalName = name
                var nameParts = name.Split('.');
                signal.PMUName = nameParts[0];
                signal.SamplingRate = fileInfo.SamplingRate;
                if (nameParts.Length == 3)
                {
                    switch (nameParts[2])
                    {
                        case "F":
                            {
                                signal.TypeAbbreviation = "F";
                                signal.SignalName = nameParts[0] + ".frq";
                                signal.Unit = "Hz";
                                break;
                            }

                        case "R":
                            {
                                signal.TypeAbbreviation = "RCF";
                                signal.SignalName = nameParts[0] + ".rocof";
                                signal.Unit = "mHz/sec";
                                break;
                            }

                        case "A":
                            {
                                signal.SignalName = nameParts[0] + "." + nameParts[1] + ".ANG";
                                var channel = nameParts[1].Substring(nameParts[1].Length - 2).ToArray();
                                if (channel[0] == 'I' || channel[0] == 'V')
                                {
                                    signal.TypeAbbreviation = channel[0] + "A" + channel[1];
                                    signal.Unit = "DEG";
                                }
                                else
                                {
                                    signal.TypeAbbreviation = "OTHER";
                                    signal.Unit = "OTHER";
                                }

                                break;
                            }

                        case "M":
                            {
                                signal.SignalName = nameParts[0] + "." + nameParts[1] + ".MAG";
                                var channel = nameParts[1].Substring(nameParts[1].Length - 2).ToArray();
                                if (channel[0] == 'I')
                                {
                                    signal.TypeAbbreviation = channel[0] + "M" + channel[1];
                                    signal.Unit = "A";
                                }
                                else if (channel[0] == 'V')
                                {
                                    signal.TypeAbbreviation = channel[0] + "M" + channel[1];
                                    signal.Unit = "V";
                                }
                                else
                                {
                                    signal.TypeAbbreviation = "OTHER";
                                    signal.Unit = "OTHER";
                                }

                                break;
                            }
                        default:
                            {
                                throw new Exception("Error! Invalid signal name " + name + " found!");
                            }
                    }
                }
                else if (nameParts.Length == 2)
                {
                    if (nameParts[1].Substring(0, 1) == "D")
                    {
                        signal.TypeAbbreviation = "D";
                        signal.SignalName = nameParts[0] + ".dig" + nameParts[1].Substring(1);
                        signal.Unit = "D";
                    }
                    else
                    {
                        var lastLetter = nameParts[1].Last();
                        switch (lastLetter)
                        {
                            case 'V':
                                {
                                    signal.TypeAbbreviation = "Q";
                                    signal.SignalName = name;
                                    signal.Unit = "MVAR";
                                    break;
                                }

                            case 'W':
                                {
                                    signal.TypeAbbreviation = "P";
                                    signal.SignalName = name;
                                    signal.Unit = "MW";
                                    break;
                                }

                            default:
                                {
                                    throw new Exception("Error! Invalid signal name " + name + " found!");
                                }
                        }
                    }
                }
                else
                    throw new Exception("Error! Invalid signal name " + name + " found!");
                signal.OldSignalName = signal.SignalName;
                signal.OldTypeAbbreviation = signal.TypeAbbreviation;
                signal.OldUnit = signal.Unit;
                newSignalList.Add(signal);
            }
            fileInfo.TaggedSignals = newSignalList;
            var newSig = new SignalSignatureViewModel(fileInfo.FileDirectory + ", Sampling Rate: " + fileInfo.SamplingRate + "/Second");
            newSig.SamplingRate = fileInfo.SamplingRate;
            var a = new SignalTypeHierachy(newSig);
            a.SignalList = SortSignalByPMU(newSignalList);
            GroupedRawSignalsByPMU.Add(a);
            var b = new SignalTypeHierachy(newSig);
            b.SignalList = SortSignalByType(newSignalList);
            GroupedRawSignalsByType.Add(b);
            ReGroupedRawSignalsByType = GroupedRawSignalsByType;
        }

        public void AddRawSignalsFromADir(InputFileInfoViewModel model)
        {
            //var sampleFile = Utility.FindFirstInputFile(model.FileDirectory, model.Model.FileType);
            //var sampleFile = "";
            //if (!File.Exists(model.ExampleFile))
            //{
            //    model.ExampleFile = Utility.FindFirstInputFile(model.FileDirectory, model.Model.FileType);
            //}
            if (File.Exists(model.ExampleFile))
            {
                if (model.Model.FileType.ToLower() == "csv")
                {
                    try
                    {
                        _readCSVFile(model);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading .csv file. " + ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        _readPDATFile(model);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading .pdat file. " + ex.Message);
                    }
                }
                FileInfo.Add(model);
            }
            AllPMUs = _getAllPMU();
        }

        public ObservableCollection<SignalSignatureViewModel> FindSignalsEntirePMU(List<SignalSignatures> pMUElementList)
        {
            var newSignalList = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var signal in pMUElementList)
            {
                FindSignalsOfAPMU(newSignalList, signal.PMUName);

            }
            return newSignalList;
        }

        public void FindSignalsOfAPMU(ObservableCollection<SignalSignatureViewModel> newSignalList, string pmu)
        {
            foreach (var group in GroupedRawSignalsByPMU)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                newSignalList.Add(subsubgroup.SignalSignature);
                            }
                        }
                    }
                }
            }
            foreach (var group in GroupedSignalByDataConfigStepsOutput)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                newSignalList.Add(subsubgroup.SignalSignature);
                            }
                        }
                    }
                }
            }
            foreach (var group in GroupedSignalByProcessConfigStepsOutput)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                newSignalList.Add(subsubgroup.SignalSignature);
                            }
                        }
                    }
                }
            }
            foreach (var group in GroupedSignalByPostProcessConfigStepsOutput)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                newSignalList.Add(subsubgroup.SignalSignature);
                            }
                        }
                    }
                }
            }
        }

        public ObservableCollection<SignalSignatureViewModel> FindSignals(List<SignalSignatures> pMUElementList)
        {
            var newSignalList = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var signal in pMUElementList)
            {
                var foundSignal = SearchForSignalInTaggedSignals(signal.PMUName, signal.SignalName);
                if (!(foundSignal is null))
                {
                    newSignalList.Add(foundSignal);
                }
            }
            return newSignalList;
        }

        public SignalSignatureViewModel SearchForSignalInTaggedSignals(string pmu, string channel)
        {
            foreach (var group in GroupedRawSignalsByPMU)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                if (subsubgroup.SignalSignature.SignalName == channel)
                                    return subsubgroup.SignalSignature;
                            }
                        }
                    }
                }
            }
            foreach (var group in GroupedSignalByDataConfigStepsOutput)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                if (subsubgroup.SignalSignature.SignalName == channel)
                                    return subsubgroup.SignalSignature;
                            }
                        }
                    }
                }
            }
            foreach (var group in GroupedSignalByProcessConfigStepsOutput)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                if (subsubgroup.SignalSignature.SignalName == channel)
                                    return subsubgroup.SignalSignature;
                            }
                        }
                    }
                }
            }
            foreach (var group in GroupedSignalByPostProcessConfigStepsOutput)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.SignalSignature.PMUName == pmu)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                if (subsubgroup.SignalSignature.SignalName == channel)
                                    return subsubgroup.SignalSignature;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private ObservableCollection<PMUWithSamplingRate> _allPMU;
        public ObservableCollection<PMUWithSamplingRate> AllPMUs
        {
            set { _allPMU = value; OnPropertyChanged(); }
            get
            {
                return _getAllPMU();
            }
        }

        private ObservableCollection<PMUWithSamplingRate> _getAllPMU()
        {
            var allPMU = GroupedRawSignalsByPMU.SelectMany(x => x.SignalList).Distinct().SelectMany(r => r.SignalList).Distinct().Select(y => new PMUWithSamplingRate(y.SignalSignature.PMUName, y.SignalSignature.SamplingRate)).ToList();
            allPMU.AddRange(AllDataConfigOutputGroupedByPMU.SelectMany(x => x.SignalList).Distinct().SelectMany(r => r.SignalList).Distinct().Select(y => new PMUWithSamplingRate(y.SignalSignature.PMUName, y.SignalSignature.SamplingRate)).ToList());
            allPMU.AddRange(AllProcessConfigOutputGroupedByPMU.SelectMany(x => x.SignalList).Distinct().SelectMany(r => r.SignalList).Distinct().Select(y => new PMUWithSamplingRate(y.SignalSignature.PMUName, y.SignalSignature.SamplingRate)).ToList());
            allPMU.AddRange(AllPostProcessOutputGroupedByPMU.SelectMany(x => x.SignalList).Distinct().SelectMany(r => r.SignalList).Distinct().Select(y => new PMUWithSamplingRate(y.SignalSignature.PMUName, y.SignalSignature.SamplingRate)).ToList());
            return new ObservableCollection<PMUWithSamplingRate>(allPMU.Distinct());
        }

        #region Signal Manipulations, checking, unchecking, etc.

        public void DataConfigDetermineAllParentNodeStatus()
        {
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU);
            foreach (var stepInput in GroupedSignalByDataConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByDataConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
        }
        /// <summary>
        /// Go down a tree to determine nodes checking status
        /// </summary>
        /// <param name="groups"></param>
        private void _determineParentGroupedByTypeNodeStatus(ObservableCollection<SignalTypeHierachy> groups)
        {
            if (groups.Count > 0)
            {
                foreach (var group in groups)
                {
                    // if has children, then its status depends on children status
                    if (group.SignalList.Count > 0)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalList.Count > 0)
                            {
                                foreach (var subsubgroup in subgroup.SignalList)
                                {
                                    if (subsubgroup.SignalList.Count > 0)
                                    {
                                        foreach (var subsubsubgroup in subsubgroup.SignalList)
                                        {
                                            if (subsubsubgroup.SignalList.Count > 0)
                                            {
                                                foreach (var subsubsubsubgroup in subsubsubgroup.SignalList)
                                                {
                                                    if (subsubsubsubgroup.SignalList.Count > 0)
                                                        _determineParentCheckStatus(subsubsubsubgroup);
                                                }
                                                _determineParentCheckStatus(subsubsubgroup);
                                            }
                                        }
                                        _determineParentCheckStatus(subsubgroup);
                                    }
                                }
                                _determineParentCheckStatus(subgroup);
                            }
                        }
                        _determineParentCheckStatus(group);
                    }
                    else
                        // else, no children, status must be false, this only applies top level nodes, since leaf node won't have children at all
                        group.SignalSignature.IsChecked = false;
                }
            }
        }
        /// <summary>
        /// This sub loop through all children of a hierachy node to determine the node's status of checked/unchecked/indeterminate
        /// </summary>
        /// <param name="group"></param>
        private void _determineParentCheckStatus(SignalTypeHierachy group)
        {
            if (group.SignalList.Count > 0)
            {
                var hasCheckedItem = false;
                var hasUnCheckedItem = false;
                foreach (var subgroup in group.SignalList)
                {
                    if (subgroup.SignalSignature.IsChecked == null)
                    {
                        hasCheckedItem = true;
                        hasUnCheckedItem = true;
                        break;
                    }
                    if ((bool)subgroup.SignalSignature.IsChecked && !hasCheckedItem)
                    {
                        hasCheckedItem = true;
                        continue;
                    }
                    if (subgroup.SignalSignature.IsChecked == false && !hasUnCheckedItem)
                        hasUnCheckedItem = true;
                    if (hasCheckedItem & hasUnCheckedItem)
                        break;
                }
                if (hasCheckedItem & hasUnCheckedItem)
                    group.SignalSignature.IsChecked = null;
                else
                    group.SignalSignature.IsChecked = hasCheckedItem;
            }
        }
        public void DetermineAllParentNodeStatus()
        {
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU);
            _determineParentGroupedByTypeNodeStatus(ReGroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByPMU);
            _determineParentGroupedByTypeNodeStatus(AllPostProcessOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllPostProcessOutputGroupedByPMU);
            foreach (var stepInput in GroupedSignalByDataConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByDataConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
            foreach (var stepInput in GroupedSignalByProcessConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByProcessConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
            foreach (var stepInput in GroupedSignalByPostProcessConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByPostProcessConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
            foreach (var stepInput in GroupedSignalByDetectorInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
        }
        public void DetermineDataConfigPostProcessConfigAllParentNodeStatus()
        {
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByPMU);
            foreach (var stepInput in GroupedSignalByDataConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByDataConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
            foreach (var stepInput in GroupedSignalByPostProcessConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByPostProcessConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
        }
        public void DetectorConfigDetermineAllParentNodeStatus()
        {
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU);
            _determineParentGroupedByTypeNodeStatus(ReGroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByPMU);
            _determineParentGroupedByTypeNodeStatus(AllPostProcessOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllPostProcessOutputGroupedByPMU);
            foreach (var stepInput in GroupedSignalByDetectorInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
        }
        public void PostProcessDetermineAllParentNodeStatus()
        {
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU);
            _determineParentGroupedByTypeNodeStatus(ReGroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllProcessConfigOutputGroupedByPMU);
            foreach (var stepInput in GroupedSignalByPostProcessConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByPostProcessConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
        }
        public void ProcessConfigDetermineAllParentNodeStatus()
        {
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType);
            _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByType);
            _determineParentGroupedByTypeNodeStatus(AllDataConfigOutputGroupedByPMU);
            foreach (var stepInput in GroupedSignalByProcessConfigStepsInput)
            {
                if (stepInput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepInput.SignalList);
                    _determineParentCheckStatus(stepInput);
                }
                else
                    stepInput.SignalSignature.IsChecked = false;
            }
            foreach (var stepOutput in GroupedSignalByProcessConfigStepsOutput)
            {
                if (stepOutput.SignalList.Count > 0)
                {
                    _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList);
                    _determineParentCheckStatus(stepOutput);
                }
                else
                    stepOutput.SignalSignature.IsChecked = false;
            }
        }
        /// <summary>
        /// Check and decide if a file directory and its sub grouped signal is checkable or not depends on other file directory check status
        /// </summary>
        public void DetermineFileDirCheckableStatus()
        {
            var disableOthers = false;
            foreach (var group in GroupedRawSignalsByType)
            {
                //null or true
                if (group.SignalSignature.IsChecked == null || (bool)group.SignalSignature.IsChecked)
                {
                    disableOthers = true;
                    break;
                }
            }
            //null or true
            if (disableOthers)
            {
                foreach (var group in GroupedRawSignalsByType)
                {
                    if (group.SignalSignature.IsChecked == null || (bool)group.SignalSignature.IsChecked) //null or false
                        group.SignalSignature.IsEnabled = true;
                    else //true
                        group.SignalSignature.IsEnabled = false;
                }
                foreach (var group in GroupedRawSignalsByPMU)
                {
                    if (group.SignalSignature.IsChecked == null || (bool)group.SignalSignature.IsChecked)
                        group.SignalSignature.IsEnabled = true;
                    else
                        group.SignalSignature.IsEnabled = false;
                }
            }
            // false
            else
            {
                foreach (var group in GroupedRawSignalsByType)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in GroupedRawSignalsByPMU)
                    group.SignalSignature.IsEnabled = true;
            }
        }
        public void DetermineSamplingRateCheckableStatus(object _currentSelectedStep, int _currentTabIndex, int freq)
        {
            //var freq = -1;
            if (_currentSelectedStep != null && freq != -1)
            {
                //freq = _currentSelectedStep.InputChannels(0).SamplingRate;
                if (_currentTabIndex == 1)
                {
                    foreach (var group in GroupedRawSignalsByType)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedRawSignalsByPMU)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedSignalByDataConfigStepsInput)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedSignalByDataConfigStepsOutput)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                }
                else if (_currentTabIndex == 2)
                {
                    foreach (var group in GroupedRawSignalsByType)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedRawSignalsByPMU)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedSignalByProcessConfigStepsInput)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedSignalByProcessConfigStepsOutput)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in AllDataConfigOutputGroupedByPMU)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllDataConfigOutputGroupedByType)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                }
                else if (_currentTabIndex == 3)
                {
                    foreach (var group in ReGroupedRawSignalsByType)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedRawSignalsByPMU)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedSignalByPostProcessConfigStepsInput)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedSignalByPostProcessConfigStepsOutput)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in AllDataConfigOutputGroupedByPMU)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllDataConfigOutputGroupedByType)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllProcessConfigOutputGroupedByPMU)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllProcessConfigOutputGroupedByType)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                }
                else if (_currentTabIndex == 4)
                {
                    foreach (var group in ReGroupedRawSignalsByType)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedRawSignalsByPMU)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in GroupedSignalByDetectorInput)
                    {
                        foreach (var subgroup in group.SignalList)
                        {
                            if (subgroup.SignalSignature.SamplingRate != freq)
                                subgroup.SignalSignature.IsEnabled = false;
                            else
                                subgroup.SignalSignature.IsEnabled = true;
                        }
                    }
                    foreach (var group in AllDataConfigOutputGroupedByPMU)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllDataConfigOutputGroupedByType)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllProcessConfigOutputGroupedByPMU)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllProcessConfigOutputGroupedByType)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllPostProcessOutputGroupedByPMU)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                    foreach (var group in AllPostProcessOutputGroupedByType)
                    {
                        if (group.SignalSignature.SamplingRate != freq)
                            group.SignalSignature.IsEnabled = false;
                        else
                            group.SignalSignature.IsEnabled = true;
                    }
                }
            }
            else if (_currentTabIndex == 1)
            {
                foreach (var group in GroupedRawSignalsByType)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedRawSignalsByPMU)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedSignalByDataConfigStepsInput)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedSignalByDataConfigStepsOutput)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
            }
            else if (_currentTabIndex == 2)
            {
                foreach (var group in GroupedRawSignalsByType)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedRawSignalsByPMU)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedSignalByProcessConfigStepsInput)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedSignalByProcessConfigStepsOutput)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in AllDataConfigOutputGroupedByPMU)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllDataConfigOutputGroupedByType)
                    group.SignalSignature.IsEnabled = true;
            }
            else if (_currentTabIndex == 3)
            {
                foreach (var group in ReGroupedRawSignalsByType)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedRawSignalsByPMU)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedSignalByPostProcessConfigStepsInput)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedSignalByPostProcessConfigStepsOutput)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in AllDataConfigOutputGroupedByPMU)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllDataConfigOutputGroupedByType)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllProcessConfigOutputGroupedByPMU)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllProcessConfigOutputGroupedByType)
                    group.SignalSignature.IsEnabled = true;
            }
            else if (_currentTabIndex == 4)
            {
                foreach (var group in ReGroupedRawSignalsByType)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedRawSignalsByPMU)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in GroupedSignalByDetectorInput)
                {
                    foreach (var subgroup in group.SignalList)
                        subgroup.SignalSignature.IsEnabled = true;
                }
                foreach (var group in AllDataConfigOutputGroupedByPMU)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllDataConfigOutputGroupedByType)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllProcessConfigOutputGroupedByPMU)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllProcessConfigOutputGroupedByType)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllPostProcessOutputGroupedByPMU)
                    group.SignalSignature.IsEnabled = true;
                foreach (var group in AllPostProcessOutputGroupedByType)
                    group.SignalSignature.IsEnabled = true;
            }
        }
        /// <summary>
        /// This sub checks/unchecks of all children of a node in the signal grouped by type parent tree
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isChecked"></param>
        public void CheckAllChildren(SignalTypeHierachy node, bool isChecked)
        {
            if (node.SignalList.Count > 0)
            {
                // if not a leaf node, call itself recursively to check/uncheck all children
                foreach (var child in node.SignalList)
                {
                    if ((bool)child.SignalSignature.IsEnabled)
                    {
                        child.SignalSignature.IsChecked = isChecked;
                        CheckAllChildren(child, isChecked);
                    }
                }
            }
        }
        #endregion

        #region Raw signals
        private ObservableCollection<SignalTypeHierachy> _groupedRawSignalsByType;
        public ObservableCollection<SignalTypeHierachy> GroupedRawSignalsByType
        {
            get
            {
                return _groupedRawSignalsByType;
            }
            set
            {
                _groupedRawSignalsByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _reGroupedRawSignalsByType;
        public ObservableCollection<SignalTypeHierachy> ReGroupedRawSignalsByType
        {
            get
            {
                return _reGroupedRawSignalsByType;
            }
            set
            {
                _reGroupedRawSignalsByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _groupedRawSignalsByPMU;
        public ObservableCollection<SignalTypeHierachy> GroupedRawSignalsByPMU
        {
            get
            {
                return _groupedRawSignalsByPMU;
            }
            set
            {
                _groupedRawSignalsByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region data Config related
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByDataConfigStepsInput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByDataConfigStepsInput
        {
            get
            {
                return _groupedSignalByDataConfigStepsInput;
            }
            set
            {
                _groupedSignalByDataConfigStepsInput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByDataConfigStepsOutput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByDataConfigStepsOutput
        {
            get
            {
                return _groupedSignalByDataConfigStepsOutput;
            }
            set
            {
                _groupedSignalByDataConfigStepsOutput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _allDataConfigOutputGroupedByType;
        public ObservableCollection<SignalTypeHierachy> AllDataConfigOutputGroupedByType
        {
            get
            {
                return _allDataConfigOutputGroupedByType;
            }
            set
            {
                _allDataConfigOutputGroupedByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _allDataConfigOutputGroupedByPMU;
        public ObservableCollection<SignalTypeHierachy> AllDataConfigOutputGroupedByPMU
        {
            get
            {
                return _allDataConfigOutputGroupedByPMU;
            }
            set
            {
                _allDataConfigOutputGroupedByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region process config related
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByProcessConfigStepsInput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByProcessConfigStepsInput
        {
            get
            {
                return _groupedSignalByProcessConfigStepsInput;
            }
            set
            {
                _groupedSignalByProcessConfigStepsInput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByProcessConfigStepsOutput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByProcessConfigStepsOutput
        {
            get
            {
                return _groupedSignalByProcessConfigStepsOutput;
            }
            set
            {
                _groupedSignalByProcessConfigStepsOutput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _allProcessConfigOutputGroupedByType;
        public ObservableCollection<SignalTypeHierachy> AllProcessConfigOutputGroupedByType
        {
            get
            {
                return _allProcessConfigOutputGroupedByType;
            }
            set
            {
                _allProcessConfigOutputGroupedByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _allProcessConfigOutputGroupedByPMU;
        public ObservableCollection<SignalTypeHierachy> AllProcessConfigOutputGroupedByPMU
        {
            get
            {
                return _allProcessConfigOutputGroupedByPMU;
            }
            set
            {
                _allProcessConfigOutputGroupedByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region process config related
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByPostProcessConfigStepsInput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByPostProcessConfigStepsInput
        {
            get
            {
                return _groupedSignalByPostProcessConfigStepsInput;
            }
            set
            {
                _groupedSignalByPostProcessConfigStepsInput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByPostProcessConfigStepsOutput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByPostProcessConfigStepsOutput
        {
            get
            {
                return _groupedSignalByPostProcessConfigStepsOutput;
            }
            set
            {
                _groupedSignalByPostProcessConfigStepsOutput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _allPostProcessOutputGroupedByType;
        public ObservableCollection<SignalTypeHierachy> AllPostProcessOutputGroupedByType
        {
            get
            {
                return _allPostProcessOutputGroupedByType;
            }
            set
            {
                _allPostProcessOutputGroupedByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _allPostProcessOutputGroupedByPMU;
        public ObservableCollection<SignalTypeHierachy> AllPostProcessOutputGroupedByPMU
        {
            get
            {
                return _allPostProcessOutputGroupedByPMU;
            }
            set
            {
                _allPostProcessOutputGroupedByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region detector config related
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByDetectorInput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByDetectorInput
        {
            get
            {
                return _groupedSignalByDetectorInput;
            }
            set
            {
                _groupedSignalByDetectorInput = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
