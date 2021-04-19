using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Core.ViewModels;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using JSISCSVWriter;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

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
            _groupedSignalByDataWriterDetectorInput = new ObservableCollection<SignalTypeHierachy>();

            _engine = MatLabEngine.Instance;
            _dataViewGroupMethods = new List<string>(new string[] { "View Signal by Type", "View Signal by PMU" });
            AddPlot = new RelayCommand(_addAPlot);
            _signalPlots = new ObservableCollection<SignalPlotPanel>();
            UpdatePlot = new RelayCommand(_updatePlot);
            PlotSelected = new RelayCommand(_plotSelectedToEdit);
            DeleteAPlot = new RelayCommand(_deleteAPlot);
            AllPlotsDeSelected = new RelayCommand(DeSelectAllPlots);

            _inspectionAnalysisParams = new InspectionAnalysisParametersViewModel();
            SpectralInspection = new RelayCommand(_spectralInspection);
            ExportData = new RelayCommand(_exportData);
            MappingSignals = new ObservableCollection<SignalSignatureViewModel>();
        }
        public void cleanUp()
        {
            FileInfo = new ObservableCollection<InputFileInfoViewModel>();
            _groupedRawSignalsByType = new ObservableCollection<SignalTypeHierachy>();
            _reGroupedRawSignalsByType = new ObservableCollection<SignalTypeHierachy>();
            _groupedRawSignalsByPMU = new ObservableCollection<SignalTypeHierachy>();
            SingalWithDataList = new ObservableCollection<SignalSignatureViewModel>();
            //_timeStampNumber = new List<double>();
            GroupedSignalsWithDataByPMU = new ObservableCollection<SignalTypeHierachy>();
            GroupedSignalsWithDataByType = new ObservableCollection<SignalTypeHierachy>();
            SignalPlots.Clear();

            CleanUpSettingsSignals();

            MappingSignals = new ObservableCollection<SignalSignatureViewModel>();
        }

        public void CleanUpSettingsSignals()
        {
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
            _groupedSignalByDataWriterDetectorInput = new ObservableCollection<SignalTypeHierachy>();
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

        public Boolean AddRawSignals(List<InputFileInfoModel> inputFileInfos, string starttime)
        {
            var MissingExampleFile = new List<string>();
            bool ReadingSuccess = true;
            var dirs = new List<String>();
            var dirMnDict = new Dictionary<string, List<string>>();
            // go through each input file source to see if example file exist, if yes, add to file info, if not add to error message to tell user the file source is having problem
            // so all missing file source and input file reading errors will show up at the same time
            foreach (var item in inputFileInfos)
            {
                var aFileInfo = new InputFileInfoViewModel(item);
                FileInfo.Add(aFileInfo);
                if (!dirs.Contains(item.FileDirectory))
                {
                    dirs.Add(item.FileDirectory);
                }
                if (!dirMnDict.ContainsKey(item.FileDirectory))
                {
                    dirMnDict[item.FileDirectory] = new List<string>();
                }
                if (!dirMnDict[item.FileDirectory].Contains(item.Mnemonic))
                {
                    dirMnDict[item.FileDirectory].Add(item.Mnemonic);
                }
                else
                {
                    MissingExampleFile.Add("\nDuplicate file source not allowed! File source in directory: " + item.FileDirectory + " with Mnemonic " + item.Mnemonic);
                    continue;
                }
                if (!File.Exists(item.ExampleFile))
                {
                    //item.ExampleFile = Utility.FindFirstInputFile(item.FileDirectory, item.FileType);
                    //MessageBox.Show("Example input data file does not exist!", "Warning!", MessageBoxButtons.OK);           
                    MissingExampleFile.Add("The example file  " + Path.GetFileName(item.ExampleFile) + "  could not be found in the directory  " + Path.GetDirectoryName(item.ExampleFile) + ".");
                }
                else
                {
                    //var aFileInfo = new InputFileInfoViewModel(item);
                    if (item.FileType == DataFileType.csv)
                    {
                        try
                        {
                            _readExampleFile(aFileInfo, 2);
                        }
                        catch (Exception ex)
                        {
                            MissingExampleFile.Add("\nError reading .csv file:  " + Path.GetFileName(item.ExampleFile) + ". " + ex.Message + ".");
                            //MessageBox.Show("Error reading .csv file. " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                    }
                    else if(item.FileType == DataFileType.pdat)
                    {
                        try
                        {
                            _readExampleFile(aFileInfo, 1);
                        }
                        catch (Exception ex)
                        {
                            MissingExampleFile.Add("\nError reading .pdat file:  " + Path.GetFileName(item.ExampleFile) + ". " + ex.Message + ".");
                            //MessageBox.Show("Error reading .pdat file. " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                    }
                    else if(item.FileType == DataFileType.powHQ)
                    {
                        try
                        {
                            _readExampleFile(aFileInfo, 3);
                        }
                        catch (Exception ex)
                        {
                            MissingExampleFile.Add("\nError reading point on wave data:  " + Path.GetFileName(item.ExampleFile) + ". " + ex.Message + ".");
                            //MessageBox.Show("Error reading .pdat file. " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                    }
                    else if(item.FileType == DataFileType.PI)
                    {
                        try
                        {
                            aFileInfo.PresetList = item.GetPresets(item.ExampleFile);
                            _readDBExampleFile(aFileInfo, starttime, "PI");
                        }
                        catch (Exception ex)
                        {
                            MissingExampleFile.Add("\nError reading PI database:  " + Path.GetFileName(item.ExampleFile) + ". " + ex.Message + ".");
                        }
                    }
                    else if (item.FileType == DataFileType.OpenHistorian)
                    {
                        try
                        {
                            aFileInfo.PresetList = item.GetPresets(item.ExampleFile);
                            _readDBExampleFile(aFileInfo, starttime, "openHistorian");
                        }
                        catch (Exception ex)
                        {
                            MissingExampleFile.Add("\nError reading openHistorian database:  " + Path.GetFileName(item.ExampleFile) + ". " + ex.Message + ".");
                        }
                    }
                    else if (item.FileType == DataFileType.OpenPDC)
                    {
                        try
                        {
                            aFileInfo.PresetList = item.GetPresets(item.ExampleFile);
                            _readDBExampleFile(aFileInfo, starttime, "openPDC");
                        }
                        catch (Exception ex)
                        {
                            MissingExampleFile.Add("\nError reading openPDC database:  " + Path.GetFileName(item.ExampleFile) + ". " + ex.Message + ".");
                        }
                    }
                    else if (item.FileType == DataFileType.uPMUdat)
                    {
                        try
                        {
                            _readExampleFile(aFileInfo, 4);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    //FileInfo.Add(aFileInfo);
                }
            }
            if (MissingExampleFile.Count() > 0)
            {
                MessageBox.Show(string.Join("\n", MissingExampleFile) + "\nPlease go to the 'Data Source' tab, update the location of the example file, and click the 'Read File' button.", "Warning!", MessageBoxButtons.OK);
                ReadingSuccess = false;
            }
            AllPMUs = _getAllPMU();
            return ReadingSuccess;
        }

        public void AddRawSignalsFromADir(InputFileInfoViewModel model, string starttime)
        {
            //var sampleFile = Utility.FindFirstInputFile(model.FileDirectory, model.Model.FileType);
            //var sampleFile = "";
            //if (!File.Exists(model.ExampleFile))
            //{
            //    model.ExampleFile = Utility.FindFirstInputFile(model.FileDirectory, model.Model.FileType);
            //}
            if (File.Exists(model.ExampleFile))
            {
                if (model.Model.FileType == DataFileType.csv)
                {
                    try
                    {
                        _readExampleFile(model, 2);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading .csv file. " + ex.Message);
                    }
                }
                else if (model.Model.FileType == DataFileType.pdat)
                {
                    try
                    {
                        _readExampleFile(model, 1);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading .pdat file. " + ex.Message);
                    }
                }
                else if (model.Model.FileType == DataFileType.powHQ)
                {
                    try
                    {
                        _readExampleFile(model, 3);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading .mat file. " + ex.Message);
                    }
                }
                else if (model.Model.FileType == DataFileType.PI)
                {
                    try
                    {
                        model.PresetList = model.Model.GetPresets(model.ExampleFile);
                        _readDBExampleFile(model, starttime, "PI");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading PI database. " + ex.Message);
                    }
                }
                else if (model.Model.FileType == DataFileType.OpenHistorian)
                {
                    try
                    {
                        model.PresetList = model.Model.GetPresets(model.ExampleFile);
                        _readDBExampleFile(model, starttime, "openHistorian");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading openHistorian database. " + ex.Message);
                    }
                }
                else if (model.Model.FileType == DataFileType.OpenPDC)
                {
                    try
                    {
                        model.PresetList = model.Model.GetPresets(model.ExampleFile);
                        _readDBExampleFile(model, starttime, "openPDC");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading openPDC database. " + ex.Message);
                    }
                }
                else if (model.Model.FileType == DataFileType.uPMUdat)
                {
                    try
                    {
                        _readExampleFile(model, 4);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading .dat file. " + ex.Message);
                    }
                }
                //FileInfo.Add(model);
            }
            AllPMUs = _getAllPMU();
        }

        private void _readExampleFile(InputFileInfoViewModel aFileInfo, int fileType)
        {
            var signalInformation = _engine.GetFileExample(aFileInfo.ExampleFile, fileType);
            //aFileInfo.SamplingRate = signalInformation.SamplingRate;
            _organizeSignals(aFileInfo, signalInformation);
        }

        private void _readDBExampleFile(InputFileInfoViewModel model, string starttime, string dbtype)
        {
            var signalInformation = _engine.GetDBFileExample(starttime, model.Mnemonic, model.ExampleFile, dbtype);
            _organizeSignals(model, signalInformation);
        }

        private void _organizeSignals(InputFileInfoViewModel aFileInfo, ReadExampleFileResults signalInformation)
        {
            if (signalInformation != null && signalInformation.PMUSignalsList != null && signalInformation.PMUSignalsList.Count > 0)
            {
                ObservableCollection<SignalSignatureViewModel> newSignalList = new ObservableCollection<SignalSignatureViewModel>();
                for (int idx = 0; idx < signalInformation.PMUSignalsList.Count; idx++)
                {
                    var thisPMU = signalInformation.PMUSignalsList[idx];
                    var thisPMUName = signalInformation.PMUSignalsList[idx].PMUname;
                    for (int idx2 = 0; idx2 < thisPMU.SignalNames.Count; idx2++)
                    {
                        var aSignal = new SignalSignatureViewModel();
                        aSignal.SamplingRate = thisPMU.SamplingRate;
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
                aFileInfo.SamplingRate = signalInformation.PMUSignalsList[0].SamplingRate;
                aFileInfo.TaggedSignals = newSignalList;
                var newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", " + aFileInfo.Mnemonic + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
                newSig.SamplingRate = aFileInfo.SamplingRate;
                var a = new SignalTypeHierachy(newSig);
                a.SignalList = SortSignalByPMU(newSignalList);
                GroupedRawSignalsByPMU.Add(a);
                var b = new SignalTypeHierachy(newSig);
                b.SignalList = SortSignalByType(newSignalList);
                GroupedRawSignalsByType.Add(b);
                ReGroupedRawSignalsByType = GroupedRawSignalsByType;
            }
        }

        //private void _readCSVFile(InputFileInfoViewModel aFileInfo)
        //{
        //    var csvReader = new CSVReader(aFileInfo.ExampleFile);
        //    var pmuName = csvReader.pmuName;
        //    var SamplingRate = csvReader.SamplingRate;
        //    var signalNames = csvReader.signalNames;
        //    var signalTypes = csvReader.signalTypes;
        //    var signalUnits = csvReader.signalUnits;
        //    var signalList = new List<string>();
        //    var signalSignatureList = new ObservableCollection<SignalSignatureViewModel>();
        //    for (var index = 0; index <= signalNames.Count - 1; index++)
        //    {
        //        var newSignal = new SignalSignatureViewModel();
        //        newSignal.PMUName = pmuName;
        //        newSignal.Unit = signalUnits[index];
        //        newSignal.SignalName = signalNames[index];
        //        newSignal.SamplingRate = (int)SamplingRate;
        //        signalList.Add(signalNames[index]);
        //        switch (signalTypes[index])
        //        {
        //            case "VPM":
        //                {
        //                    // signalName = signalNames(index).Split(".")(0) & ".VMP"
        //                    // signalName = signalNames(index)
        //                    newSignal.TypeAbbreviation = "VMP";
        //                    break;
        //                }

        //            case "VPA":
        //                {
        //                    // signalName = signalNames(index).Split(".")(0) & ".VAP"
        //                    // signalName = signalNames(index)
        //                    newSignal.TypeAbbreviation = "VAP";
        //                    break;
        //                }

        //            case "IPM":
        //                {
        //                    // signalName = signalNames(index).Split(".")(0) & ".IMP"
        //                    // signalName = signalNames(index)
        //                    newSignal.TypeAbbreviation = "IMP";
        //                    break;
        //                }

        //            case "IPA":
        //                {
        //                    // signalName = signalNames(index).Split(".")(0) & ".IAP"
        //                    // signalName = signalNames(index)
        //                    newSignal.TypeAbbreviation = "IAP";
        //                    break;
        //                }

        //            case "F":
        //                {
        //                    // signalName = signalNames(index)
        //                    newSignal.TypeAbbreviation = "F";
        //                    break;
        //                }

        //            case "P":
        //                {
        //                    // signalName = signalNames(index)
        //                    newSignal.TypeAbbreviation = "P";
        //                    break;
        //                }

        //            case "Q":
        //                {
        //                    // signalName = signalNames(index)
        //                    newSignal.TypeAbbreviation = "Q";
        //                    break;
        //                }

        //            default:
        //                {
        //                    throw new Exception("Error! Invalid signal type " + signalTypes[index] + " found in file: " + aFileInfo.ExampleFile + " !");
        //                }
        //        }
        //        newSignal.OldSignalName = newSignal.SignalName;
        //        newSignal.OldTypeAbbreviation = newSignal.TypeAbbreviation;
        //        newSignal.OldUnit = newSignal.Unit;
        //        signalSignatureList.Add(newSignal);
        //    }
        //    aFileInfo.SignalList = signalList;
        //    aFileInfo.TaggedSignals = signalSignatureList;
        //    aFileInfo.SamplingRate = (int)SamplingRate;
        //    var newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
        //    newSig.SamplingRate = (int)SamplingRate;
        //    var a = new SignalTypeHierachy(newSig);
        //    a.SignalList = SortSignalByPMU(signalSignatureList);
        //    GroupedRawSignalsByPMU.Add(a);
        //    //newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
        //    //newSig.SamplingRate = (int)SamplingRate;
        //    var b = new SignalTypeHierachy(newSig);
        //    b.SignalList = SortSignalByType(signalSignatureList);
        //    GroupedRawSignalsByType.Add(b);
        //    ReGroupedRawSignalsByType = GroupedRawSignalsByType;
        //}
        //private void _readPDATFile(InputFileInfoViewModel aFileInfo)
        //{
        //    var signalInformation = _engine.GetFileExample(aFileInfo.ExampleFile, 1);
        //    aFileInfo.SamplingRate = signalInformation.SamplingRate;
        //    ObservableCollection<SignalSignatureViewModel> newSignalList = new ObservableCollection<SignalSignatureViewModel>();
        //    for (int idx = 0; idx < signalInformation.PMUSignalsList.Count; idx++)
        //    {
        //        var thisPMU = signalInformation.PMUSignalsList[idx];
        //        var thisPMUName = signalInformation.PMUSignalsList[idx].PMUname;
        //        for (int idx2 = 0; idx2 < thisPMU.SignalNames.Count; idx2++)
        //        {
        //            var aSignal = new SignalSignatureViewModel();
        //            aSignal.SamplingRate = aFileInfo.SamplingRate;
        //            aSignal.PMUName = thisPMUName;
        //            aSignal.SignalName = thisPMU.SignalNames[idx2];
        //            aSignal.Unit = thisPMU.SignalUnits[idx2];
        //            aSignal.TypeAbbreviation = thisPMU.SignalTypes[idx2];
        //            aSignal.OldSignalName = aSignal.SignalName;
        //            aSignal.OldTypeAbbreviation = aSignal.TypeAbbreviation;
        //            aSignal.OldUnit = aSignal.Unit;
        //            newSignalList.Add(aSignal);
        //        }
        //    }
        //    aFileInfo.TaggedSignals = newSignalList;
        //    var newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
        //    newSig.SamplingRate = aFileInfo.SamplingRate;
        //    var a = new SignalTypeHierachy(newSig);
        //    a.SignalList = SortSignalByPMU(newSignalList);
        //    GroupedRawSignalsByPMU.Add(a);
        //    var b = new SignalTypeHierachy(newSig);
        //    b.SignalList = SortSignalByType(newSignalList);
        //    GroupedRawSignalsByType.Add(b);
        //    ReGroupedRawSignalsByType = GroupedRawSignalsByType;
        //}
        public ObservableCollection<InputFileInfoViewModel> FileInfo { get; set; }
        public ObservableCollection<SignalTypeHierachy> SortSignalByType(ObservableCollection<SignalSignatureViewModel> signalList)
        {
            ObservableCollection<SignalTypeHierachy> signalTypeTreeGroupedBySamplingRate = new ObservableCollection<SignalTypeHierachy>();
            if (signalList != null && signalList.Count > 0)
            {
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

                                            case "W":
                                                {
                                                    var mGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Wave"));
                                                    mGroup.SignalSignature.TypeAbbreviation = "VM";
                                                    var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in mGroupHierachky)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            //case "P":
                                                            //    {
                                                            //        var positiveGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Positive Sequence"));
                                                            //        positiveGroup.SignalSignature.TypeAbbreviation = "VMP";
                                                            //        foreach (var signal in phase)
                                                            //            positiveGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                            //        mGroup.SignalList.Add(positiveGroup);
                                                            //        break;
                                                            //    }

                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                    AGroup.SignalSignature.TypeAbbreviation = "VWA";
                                                                    foreach (var signal in phase)
                                                                        AGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                    BGroup.SignalSignature.TypeAbbreviation = "VWB";
                                                                    foreach (var signal in phase)
                                                                        BGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                    CGroup.SignalSignature.TypeAbbreviation = "VWC";
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

                                            case "W":
                                                {
                                                    var mGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Wave"));
                                                    mGroup.SignalSignature.TypeAbbreviation = "IM";
                                                    var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in mGroupHierachky)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase A"));
                                                                    AGroup.SignalSignature.TypeAbbreviation = "IWA";
                                                                    foreach (var signal in phase)
                                                                        AGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase B"));
                                                                    BGroup.SignalSignature.TypeAbbreviation = "IWB";
                                                                    foreach (var signal in phase)
                                                                        BGroup.SignalList.Add(new SignalTypeHierachy(signal));
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTypeHierachy(new SignalSignatureViewModel("Phase C"));
                                                                    CGroup.SignalSignature.TypeAbbreviation = "IWC";
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
            }
            return signalTypeTreeGroupedBySamplingRate;
        }
        public ObservableCollection<SignalTypeHierachy> SortSignalByPMU(ObservableCollection<SignalSignatureViewModel> signalList)
        {
            var pmuSignalTreeGroupedBySamplingRate = new ObservableCollection<SignalTypeHierachy>();
            if (signalList != null && signalList.Count > 0)
            {
                var groupBySamplingRate = signalList.GroupBy(x => x.SamplingRate);
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
                                    signal.Unit = "O";
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
                                    signal.Unit = "O";
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
            //var newSig = new SignalSignatureViewModel(fileInfo.FileDirectory + ", Sampling Rate: " + fileInfo.SamplingRate + "/Second");
            var newSig = new SignalSignatureViewModel(fileInfo.FileDirectory + ", " + fileInfo.Mnemonic + ", Sampling Rate: " + fileInfo.SamplingRate + "/Second");
            newSig.SamplingRate = fileInfo.SamplingRate;
            var a = new SignalTypeHierachy(newSig);
            a.SignalList = SortSignalByPMU(newSignalList);
            GroupedRawSignalsByPMU.Add(a);
            var b = new SignalTypeHierachy(newSig);
            b.SignalList = SortSignalByType(newSignalList);
            GroupedRawSignalsByType.Add(b);
            ReGroupedRawSignalsByType = GroupedRawSignalsByType;
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
            if (groups != null && groups.Count > 0)
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
            _determineParentGroupedByTypeNodeStatus(ReGroupedRawSignalsByType);
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
            foreach (var stepInput in GroupedSignalByDataWriterDetectorInput)
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
                else if (_currentTabIndex == 4 || _currentTabIndex == 5)
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
            else if (_currentTabIndex == 4 || _currentTabIndex == 5)
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
        private ObservableCollection<SignalTypeHierachy> _groupedSignalByDataWriterDetectorInput;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalByDataWriterDetectorInput
        {
            get
            {
                return _groupedSignalByDataWriterDetectorInput;
            }
            set
            {
                _groupedSignalByDataWriterDetectorInput = value;
                OnPropertyChanged();
            }
        }
        #endregion



        #region DrawSignal
        public void GetSignalDataByTimeRange(ViewResolvingPlotModel pm, AWRunViewModel run)
        {
            SignalPlots.Clear();
            //SignalViewPlotModel = null;
            string start = null;
            string end = null;
            foreach (var ax in pm.Axes)
            {
                if (ax.IsHorizontal())
                {
                    start = DateTime.FromOADate(ax.ActualMinimum).ToString("MM/dd/yyyy HH:mm:ss.fff");
                    end = DateTime.FromOADate(ax.ActualMaximum).ToString("MM/dd/yyyy HH:mm:ss.fff");
                    break;
                }
            }
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                try
                {
                    _engine.RetrieveDataCompletedEvent += _retrieveDataCompleted;
                    _engine.RetrieveData(start, end, run);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
        }
        //private List<double> _timeStampNumber;
        public ObservableCollection<SignalSignatureViewModel> SingalWithDataList = new ObservableCollection<SignalSignatureViewModel>();
        private ObservableCollection<SignalTypeHierachy> _groupedSignalsWithDataByPMU;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalsWithDataByPMU
        {
            get
            {
                return _groupedSignalsWithDataByPMU;
            }
            set
            {
                _groupedSignalsWithDataByPMU = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _groupedSignalsWithDataByType;
        public ObservableCollection<SignalTypeHierachy> GroupedSignalsWithDataByType
        {
            get
            {
                return _groupedSignalsWithDataByType;
            }
            set
            {
                _groupedSignalsWithDataByType = value;
                OnPropertyChanged();
            }
        }
        private void _retrieveDataCompleted(object sender, ReadExampleFileResults e)
        {
            //_timeStampNumber = e.TimeStampNumber;
            SingalWithDataList = new ObservableCollection<SignalSignatureViewModel>();
            foreach (var pmu in e.PMUSignalsList)
            {
                for (int index = 0; index < pmu.SignalCount; index++)
                {
                    var aSignal = SearchForSignalInTaggedSignals(pmu.PMUname, pmu.SignalNames[index]);
                    if (aSignal != null && aSignal.TypeAbbreviation == pmu.SignalTypes[index] && aSignal.Unit == pmu.SignalUnits[index] && aSignal.SamplingRate == pmu.SamplingRate)
                    {
                        aSignal.Data = pmu.Data.GetRange(index * pmu.SignalLength, pmu.SignalLength);
                        aSignal.TimeStampNumber = pmu.TimeStampNumber;
                        aSignal.MATLABTimeStampNumber = pmu.MATLABTimeStampNumber;
                        SingalWithDataList.Add(aSignal);
                    }
                    else
                    {
                        var newSignal = new SignalSignatureViewModel(pmu.SignalNames[index], pmu.PMUname, pmu.SignalTypes[index]);
                        newSignal.SamplingRate = pmu.SamplingRate;
                        newSignal.Unit = pmu.SignalUnits[index];
                        newSignal.Data = pmu.Data.GetRange(index * pmu.SignalLength, pmu.SignalLength);
                        newSignal.TimeStampNumber = pmu.TimeStampNumber;
                        newSignal.MATLABTimeStampNumber = pmu.MATLABTimeStampNumber;
                        SingalWithDataList.Add(newSignal);
                    }
                }
            }
            GroupedSignalsWithDataByPMU = SortSignalByPMU(SingalWithDataList);
            GroupedSignalsWithDataByType = SortSignalByType(SingalWithDataList);
        }
        private string _selectedDataViewingGroupMethod;
        public string SelectedDataViewingGroupMethod
        {
            get { return _selectedDataViewingGroupMethod; }
            set
            {
                _selectedDataViewingGroupMethod = value;
                OnPropertyChanged();
            }
        }
        private List<string> _dataViewGroupMethods;
        public List<string> DataviewGroupMethods
        {
            get { return _dataViewGroupMethods; }
            set
            {
                _dataViewGroupMethods = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalPlotPanel> _signalPlots;
        public ObservableCollection<SignalPlotPanel> SignalPlots
        {
            set
            {
                _signalPlots = value;
                OnPropertyChanged();
            }
            get { return _signalPlots; }
        }
        public ICommand AddPlot { get; set; }
        private void _addAPlot(object obj)
        {
            var newPlot = new SignalPlotPanel();
            //newPlot.IsPlotSelected = true;
            SignalPlots.Add(newPlot);
            _plotSelectedToEdit(newPlot);
        }
        private SignalPlotPanel _selectedSignalPlotPanel;
        public SignalPlotPanel SelectedSignalPlotPanel
        {
            get { return _selectedSignalPlotPanel; }
            set
            {
                _selectedSignalPlotPanel = value;
                //figure out the sampling rate of the current plot selected, 
                //if no signals on this plot, do not change sampling rate of the inspection analysis parameter,
                //if there's any signals, reflect it in the inspection analysis parameter
                if (value != null && _selectedSignalPlotPanel.Signals.Any())
                {
                    InspectionAnalysisParams.Fs = _selectedSignalPlotPanel.Signals.FirstOrDefault().SamplingRate;
                }                
                OnPropertyChanged();
            }
        }
        public ICommand UpdatePlot { get; set; }
        private void _updatePlot(object obj)
        {
            if (SelectedSignalPlotPanel != null)
            {
                //var freqMatch = true;
                var hk = obj as SignalTypeHierachy;
                //if (SelectedSignalPlotPanel.Signals.Count > 0)
                //{
                //    freqMatch = _checkFreq(SelectedSignalPlotPanel.Signals[0].SamplingRate, hk);
                //}
                //SelectedSignalPlotPanel.Signals.Add();
                //if (freqMatch)
                //{
                    if ((bool)hk.SignalSignature.IsChecked)
                    {
                        _addSignalsToPlot(hk);
                    }
                    else
                    {
                        _removeSignalsFromPlot(hk);
                    }
                    CheckAllChildren(hk, (bool)hk.SignalSignature.IsChecked);
                    _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByPMU);
                    _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByType);
                    _drawSignals();
                    if (SelectedSignalPlotPanel.Signals.Count > 0 && InspectionAnalysisParams.Fs != SelectedSignalPlotPanel.Signals[0].SamplingRate)
                    {
                        InspectionAnalysisParams.Fs = SelectedSignalPlotPanel.Signals[0].SamplingRate;
                    }
                //}
                //else
                //{
                //    MessageBox.Show("Selected signal has a different sampling rate than the plotted ones.");
                //}
            }
            else
            {
                MessageBox.Show("No plot is selected to add signal.");
            }
        }

        private bool _checkFreq(int samplingRate, SignalTypeHierachy hk)
        {
            if (hk.SignalList.Count > 0)
            {
                return _checkFreq(samplingRate, hk.SignalList[0]);
            }
            else
            {
                return hk.SignalSignature.SamplingRate == samplingRate;
            }
        }

        private void _removeSignalsFromPlot(SignalTypeHierachy obj)
        {
            if (obj.SignalList.Count == 0 && SelectedSignalPlotPanel.Signals.Contains(obj.SignalSignature))
            {
                SelectedSignalPlotPanel.Signals.Remove(obj.SignalSignature);
            }
            else
            {
                foreach (var hk in obj.SignalList)
                {
                    _removeSignalsFromPlot(hk);
                }
            }
        }
        private void _addSignalsToPlot(SignalTypeHierachy obj)
        {
            if (obj.SignalList.Count == 0)
            {
                SelectedSignalPlotPanel.Signals.Add(obj.SignalSignature);
            }
            else
            {
                foreach (var hk in obj.SignalList)
                {
                    _addSignalsToPlot(hk);
                }
            }
        }
        //private SignalSignatureViewModel _selectedSignalToBeViewed;
        //public SignalSignatureViewModel SelectedSignalToBeViewed
        //{
        //    get { return _selectedSignalToBeViewed; }
        //    set
        //    {
        //        if (value != null && _selectedSignalToBeViewed != value)
        //        {
        //            _selectedSignalToBeViewed = value;
        //            _drawSignal();
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        private void _drawSignals()
        {
            var AsignalPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
            var legends = new ObservableCollection<Legend>();
            OxyPlot.Axes.DateTimeAxis timeXAxis = new OxyPlot.Axes.DateTimeAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Auto,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true,
            };
            timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
            AsignalPlot.Axes.Add(timeXAxis);
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = _getUnitFromSignals(SelectedSignalPlotPanel.Signals),
                //Unit = SelectedSignalToBeViewed.Unit,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            AsignalPlot.Axes.Add(yAxis);
            var signalCounter = 0;
            foreach (var signal in SelectedSignalPlotPanel.Signals)
            {
                var newSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
                for (int i = 0; i < signal.Data.Count; i++)
                {
                    newSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.Data[i]));
                }
                newSeries.Title = signal.SignalName;
                var c = string.Format("#{0:x6}", Color.FromName(Utility.SaturatedColors[signalCounter % 20]).ToArgb());
                newSeries.Color = OxyColor.Parse(c);
                legends.Add(new Legend(signal.SignalName, newSeries.Color));
                AsignalPlot.Series.Add(newSeries);
                signalCounter++;
            }
            AsignalPlot.LegendPlacement = LegendPlacement.Outside;
            AsignalPlot.LegendPosition = LegendPosition.RightMiddle;
            AsignalPlot.LegendPadding = 0.0;
            AsignalPlot.LegendSymbolMargin = 0.0;
            AsignalPlot.LegendMargin = 0;
            AsignalPlot.IsLegendVisible = false;
            //if (SelectedSignalPlotPanel.SignalViewPlotModel.Series.Count != 0)
            //{
            foreach (var ax in SelectedSignalPlotPanel.SignalViewPlotModel.Axes)
            {
                if (ax.IsHorizontal())
                {
                    foreach (var nax in AsignalPlot.Axes)
                    {
                        if (nax.IsHorizontal() && (ax.ActualMaximum != nax.ActualMaximum || ax.ActualMinimum != nax.ActualMinimum))
                        {
                            nax.Zoom(ax.ActualMinimum, ax.ActualMaximum);
                            break;
                        }
                    }
                }
                if (ax.IsVertical())
                {
                    foreach (var nax in AsignalPlot.Axes)
                    {
                        if (nax.IsVertical() && (ax.ActualMaximum != nax.ActualMaximum || ax.ActualMinimum != nax.ActualMinimum))
                        {
                            nax.Zoom(ax.ActualMinimum, ax.ActualMaximum);
                            break;
                        }
                    }
                }
            }
            //}

            SelectedSignalPlotPanel.SignalViewPlotModel = AsignalPlot;
            SelectedSignalPlotPanel.Legends = legends;
        }
        private void TimeXAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            foreach (var plot in SignalPlots)
            {
                foreach (var ax in plot.SignalViewPlotModel.Axes)
                {
                    if (ax.IsHorizontal() && (ax.ActualMaximum != xAxis.ActualMaximum || ax.ActualMinimum != xAxis.ActualMinimum))
                    {
                        ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                        plot.SignalViewPlotModel.InvalidatePlot(false);
                        break;
                    }
                }
            }
        }

        private string _getUnitFromSignals(ObservableCollection<SignalSignatureViewModel> signals)
        {
            var unit = "";
            foreach (var s in signals)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    unit = s.Unit;
                }
                else
                {
                    if (unit != s.Unit)
                    {
                        unit = "Mixed";
                        break;
                    }
                }
            }
            return unit;
        }
        public ICommand PlotSelected { get; set; }
        private void _plotSelectedToEdit(object obj)
        {
            var selection = obj as SignalPlotPanel;
            if (SelectedSignalPlotPanel != selection)
            {
                if (SelectedSignalPlotPanel != null)
                {
                    SelectedSignalPlotPanel.IsPlotSelected = false;
                    foreach (var s in SelectedSignalPlotPanel.Signals)
                    {
                        s.IsChecked = false;
                    }
                }
                SelectedSignalPlotPanel = selection;
                SelectedSignalPlotPanel.IsPlotSelected = true;
                foreach (var s in SelectedSignalPlotPanel.Signals)
                {
                    s.IsChecked = true;
                }
                _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByPMU);
                _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByType);
            }
        }
        public ICommand DeleteAPlot { set; get; }
        private void _deleteAPlot(object obj)
        {
            var toBeDeleted = obj as SignalPlotPanel;
            foreach (var s in toBeDeleted.Signals)
            {
                s.IsChecked = false;
            }
            _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByPMU);
            _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByType);
            toBeDeleted.IsPlotSelected = false;
            SelectedSignalPlotPanel = null;
            if (SignalPlots.Contains(toBeDeleted))
            {
                SignalPlots.Remove(toBeDeleted);
            }
        }
        public ICommand AllPlotsDeSelected { set; get; }
        public void DeSelectAllPlots(object obj)
        {
            if (SelectedSignalPlotPanel != null)
            {
                foreach (var s in SelectedSignalPlotPanel.Signals)
                {
                    s.IsChecked = false;
                }
                _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByPMU);
                _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByType);
                SelectedSignalPlotPanel.IsPlotSelected = false;
                SelectedSignalPlotPanel = null;
            }
        }
        public void GetRawSignalData(InputFileInfoViewModel info, string starttime)
        {
            //SignalViewPlotModel = null;
            if (info != null && File.Exists(info.ExampleFile) && Enum.IsDefined(typeof(DataFileType), info.FileType))
            {
                try
                {
                    _engine.RetrieveDataCompletedEvent += _retrieveDataCompleted;
                    if (info.FileType == DataFileType.csv)
                    {
                        _engine.GetFileExampleSignalData(info.ExampleFile, 2);
                    }
                    else if (info.FileType == DataFileType.pdat)
                    {
                        _engine.GetFileExampleSignalData(info.ExampleFile, 1);
                    }
                    else if (info.FileType == DataFileType.powHQ)
                    {
                        _engine.GetFileExampleSignalData(info.ExampleFile, 3);
                    }
                    else if (info.FileType == DataFileType.PI)
                    {
                        _engine.GetDBExampleSignals(starttime, info.Mnemonic, info.ExampleFile, "PI");
                    }
                    else if (info.FileType == DataFileType.OpenHistorian)
                    {
                        _engine.GetDBExampleSignals(starttime, info.Mnemonic, info.ExampleFile, "openHistorian");
                    }
                    else if (info.FileType == DataFileType.OpenPDC)
                    {
                        _engine.GetDBExampleSignals(starttime, info.Mnemonic, info.ExampleFile, "openPDC");
                    }
                    else if (info.FileType == DataFileType.uPMUdat)
                    {
                        _engine.GetFileExampleSignalData(info.ExampleFile, 4);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
        }
        #endregion
        private InspectionAnalysisParametersViewModel _inspectionAnalysisParams;
        public InspectionAnalysisParametersViewModel InspectionAnalysisParams
        {
            get { return _inspectionAnalysisParams; }
            set
            {
                _inspectionAnalysisParams = value;
                OnPropertyChanged();
            }
        }
        public ICommand SpectralInspection { get; set; }
        private void _spectralInspection(object obj)
        {
            var plot = obj as SignalPlotPanel;
            //string start = null;
            //string end = null;
            double startPoint = 0;
            double endPoint = 0;
            int startIndex = 0;
            int endIndex = 0;
            foreach (var ax in plot.SignalViewPlotModel.Axes)
            {
                if (ax.IsHorizontal())
                {
                    startPoint = ax.ActualMinimum;
                    endPoint = ax.ActualMaximum;
                    //start = DateTime.FromOADate(ax.ActualMinimum).ToString("MM/dd/yyyy HH:mm:ss.fff");
                    //end = DateTime.FromOADate(ax.ActualMaximum).ToString("MM/dd/yyyy HH:mm:ss.fff");
                    break;
                }
            }
            var timeStamp = plot.Signals[0].TimeStampNumber;
            startIndex = timeStamp.FindLastIndex(a => a <= startPoint);
            endIndex = timeStamp.FindIndex(a => a >= endPoint);
            if (startIndex == -1)
            {
                startIndex = 0;
            }
            if (endIndex == -1)
            {
                endIndex = timeStamp.Count - 1; 
            }
            double[][] allData = plot.Signals.Select(a => a.Data.GetRange(startIndex, endIndex - startIndex + 1).ToArray()).ToArray();
            double[] t = timeStamp.GetRange(startIndex, endIndex - startIndex + 1).ToArray();
            InspectionAnalysisResults iaResult = _engine.InspectionAnalysis("Spectral", allData, t, plot.Signals, InspectionAnalysisParams);
            SelectedSignalPlotPanel.AddATab = true;
            if (iaResult != null)
            {
                //plot it!
                _plotInspectionAnalysisResult(iaResult);
            }
        }
        private void _plotInspectionAnalysisResult(InspectionAnalysisResults iaResult)
        {
            var AsignalPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
            //var legends = new ObservableCollection<Legend>();
            OxyPlot.Axes.LinearAxis xAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = iaResult.Xlabel,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true,
            };
            //timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
            AsignalPlot.Axes.Add(xAxis);
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = iaResult.Ylabel,
                //Unit = SelectedSignalToBeViewed.Unit,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            AsignalPlot.Axes.Add(yAxis);
            //var signalCounter = 0;
            for (int index = 0; index < iaResult.Y.Count; index++)
            {
                var newSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
                for (int i = 0; i < iaResult.Y[index].Count; i++)
                {
                    newSeries.Points.Add(new DataPoint(iaResult.X[i], iaResult.Y[index][i]));
                }
                newSeries.Title = iaResult.Signalnames[index];
                foreach (var item in SelectedSignalPlotPanel.Legends)
                {
                    if (newSeries.Title == item.Name)
                    {
                        newSeries.Color = item.Color;
                    }
                }
                //var c = string.Format("#{0:x6}", Color.FromName(Utility.SaturatedColors[signalCounter % 20]).ToArgb());
                //newSeries.Color = OxyColor.Parse(c);
                //legends.Add(new Legend(signal.SignalName, newSeries.Color));
                AsignalPlot.Series.Add(newSeries);
                //signalCounter++;

            }
            AsignalPlot.LegendPlacement = LegendPlacement.Outside;
            AsignalPlot.LegendPosition = LegendPosition.RightMiddle;
            AsignalPlot.LegendPadding = 0.0;
            AsignalPlot.LegendSymbolMargin = 0.0;
            AsignalPlot.LegendMargin = 0;
            AsignalPlot.IsLegendVisible = false;
            SelectedSignalPlotPanel.SpectralInspectionPlotModel = AsignalPlot;
        }
        public ICommand ExportData { get; set; }
        private void _exportData(object obj)
        {
            var plot = obj as SignalPlotPanel;
            double startPoint = 0;
            double endPoint = 0;
            int startIndex = 0;
            int endIndex = 0;
            foreach (var ax in plot.SignalViewPlotModel.Axes)
            {
                if (ax.IsHorizontal())
                {
                    startPoint = ax.ActualMinimum;
                    endPoint = ax.ActualMaximum;
                    break;
                }
            }
            var timeStamp = plot.Signals[0].TimeStampNumber;
            startIndex = timeStamp.FindLastIndex(a => a <= startPoint);
            endIndex = timeStamp.FindIndex(a => a >= endPoint);
            if (startIndex == -1)
            {
                startIndex = 0;
            }
            if (endIndex == -1)
            {
                endIndex = timeStamp.Count - 1;
            }
            //double[][] allData = plot.Signals.Select(a => a.Data.GetRange(startIndex, endIndex - startIndex + 1).ToArray()).ToArray();
            var t = timeStamp.GetRange(startIndex, endIndex - startIndex + 1).ToList();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JSIS_CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.Title = "Export Result Data File";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    var timeStr = DateTime.FromOADate(t[0]).ToString(@"yyyyMMdd_HHmmss");
                    var path = Path.GetFullPath(saveFileDialog1.FileName);
                    var fullname = path.Split('.')[0] + "_" + timeStr + ".csv";
                    var index = 1;
                    while (File.Exists(fullname))
                    {
                        fullname = fullname.Split(new char[] { '(', '.' })[0] + "(" + index.ToString() + ").csv";
                        index++;
                    }
                    var writer = new JSIS_CSV_Writer();
                    writer.Signals = _convertDataToBeExported(plot.Signals, startIndex, endIndex, t);
                    writer.FileToBeSaved = fullname;
                    writer.WriteJSISCSV();
                }
            }
        }

        private List<Signal> _convertDataToBeExported(ObservableCollection<SignalSignatureViewModel> signals, int startIndex, int endIndex, List<double> t)
        {
            var newSignals = new List<Signal>();
            foreach (var item in signals)
            {
                var newSignal = new Signal();
                newSignal.Data = item.Data.GetRange(startIndex, endIndex - startIndex + 1);
                newSignal.PMUname = item.PMUName;
                newSignal.SamplingRate = item.SamplingRate;
                newSignal.SignalName = item.SignalName;
                //newSignal.TimeStampInSeconds = t;
                newSignal.TimeStampNumber = t;
                newSignal.Type = item.TypeAbbreviation;
                newSignal.Unit = item.Unit;
                newSignals.Add(newSignal);
            }
            return newSignals;
        }

        /// <summary>
        /// signals that are selected by forced oscillation need to be marked on map
        /// </summary>
        private ObservableCollection<SignalSignatureViewModel> _mappingSignals;
        public ObservableCollection<SignalSignatureViewModel> MappingSignals 
        {
            get { return _mappingSignals; }
            set 
            { 
                _mappingSignals = value;
                UniqueMappingSignals = new ObservableCollection<SignalSignatureViewModel>(MappingSignals.Distinct());
            } 
        }
        private ObservableCollection<SignalSignatureViewModel> _uniqueMappingSignals;
        public ObservableCollection<SignalSignatureViewModel> UniqueMappingSignals
        {
            get { return _uniqueMappingSignals; }
            set
            {
                _uniqueMappingSignals = value;
                OnPropertyChanged();
            }
        }

        public void DistinctMappingSignal()
        {
            UniqueMappingSignals = new ObservableCollection<SignalSignatureViewModel>(MappingSignals.Distinct());
            OnUniqueMappingSignalChanged(EventArgs.Empty);
        }
        public event EventHandler UniqueMappingSignalChanged;
        protected virtual void OnUniqueMappingSignalChanged(EventArgs e)
        {
            UniqueMappingSignalChanged?.Invoke(this, e);
        }
    }
}
