using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BAWGUI.Core;
using BAWGUI.RunMATLAB.ViewModels;
using ModeMeter.ViewModels;

namespace ModeMeter.Models
{
    public class ModeMeterXmlWriter
    {
        public void WriteXmlCofigFile(AWRun run, List<SmallSignalStabilityToolViewModel> modeMeterList)
        {
            _configFilePath = run.ConfigFilePath;
            var configFile = XDocument.Load(_configFilePath);
            var mmDetectors = configFile.Element("Config").Element("DetectorConfig").Element("Configuration").Element("Alarming");
            foreach (var detector in modeMeterList)
            {
                var mmElement = new XElement("ModeMeter");
                var modeMeterDir = run.EventPath + "\\MM\\" + detector.ModeMeterName;
                if (!Directory.Exists(modeMeterDir))
                {
                    Directory.CreateDirectory(modeMeterDir);
                }
                mmElement.Add(new XElement("ResultPath", "\\MM\\" + detector.ModeMeterName));
                mmElement.Add(new XElement("CalcDEF", detector.CalcDEF.ToString().ToUpper()));
                XElement baseliningSignals = _addBaseliningSignals(detector);
                if (baseliningSignals.HasElements)
                {
                    mmElement.Add(baseliningSignals);
                }
                foreach (var mode in detector.Modes)
                {
                    _writeAMode(mmElement, mode);
                }
                mmDetectors.AddBeforeSelf(mmElement);
            }
            configFile.Save(_configFilePath);
        }

        public static void CheckMMDirsStatus(AWRun model, List<SmallSignalStabilityTool> modeMeters)
        {
            var eventPath = model.EventPath;
            var mm = eventPath + "\\MM";
            if (!Directory.Exists(mm))
            {
                Directory.CreateDirectory(mm);
                throw new Exception("Modemeter event subfolder MM was just created since it didn't exist.");
            }
            foreach (var meter in modeMeters)
            {
                var meterDir = mm + "\\" + meter.ModeMeterName;
                if (!Directory.Exists(meterDir))
                {
                    Directory.CreateDirectory(meterDir);
                    throw new Exception("Subfolder for mode meter " + meter.ModeMeterName + " was just created since it didn't exist.");
                }
            }
        }
        private void _writeAMode(XElement mmElement, ModeViewModel mode)
        {
            var modeElement = new XElement("Mode");
            modeElement.Add(new XElement("Name", mode.ModeName));
            _addModePMUSignals(modeElement, mode.PMUs);
            modeElement.Add(new XElement("AnalysisLength", mode.AnalysisLength));
            var retConTracking = new XElement("RetConTracking");
            switch (mode.Status)
            {
                case RetroactiveContinuityStatusType.ON:
                    var statusOn = new XElement("Status", "ON");
                    retConTracking.Add(statusOn);
                    var maxLength = new XElement("MaxLength", mode.MaxLength);
                    retConTracking.Add(maxLength);
                    break;
                case RetroactiveContinuityStatusType.OFF:
                    var statusOff = new XElement("Status", "OFF");
                    retConTracking.Add(statusOff);
                    break;
                default:
                    break;
            }
            modeElement.Add(retConTracking);
            //modeElement.Add(new XElement("DampRatioThreshold", mode.DampRatioThreshold));
            modeElement.Add(new XElement("DesiredModes", new XElement("LowF", mode.DesiredModes.LowF),
                                                         new XElement("HighF", mode.DesiredModes.HighF),
                                                         new XElement("GuessF", mode.DesiredModes.GuessF),
                                                         new XElement("DampMax", mode.DesiredModes.DampMax)));
            foreach (var method in mode.Methods)
            {
                _writeAMethod(modeElement, method);
            }
            //if (mode.IsFODetecotrParametersVisible)
            if (mode.ShowFOParameters)
            {
                XElement foParameters = _writeFOParameterElement(mode.FODetectorParameters);
                modeElement.Add(foParameters);
                XElement foTimeLocParameters = _writeFOTimeLocParameterElement(mode.FODetectorParameters);
                modeElement.Add(foTimeLocParameters);
            }
            if (mode.ShowRMSEnergyTransientParameters)
            {
                XElement eventDetectionparams = _writeEventDetectionParameterElement(mode.EventDetectionParameters);
                modeElement.Add(eventDetectionparams);
            }
            mmElement.Add(modeElement);
        }
        private XElement _writeFOTimeLocParameterElement(FOdetectorParametersViewModel fODetectorParameters)
        {
            throw new NotImplementedException();
        }
        private XElement _writeEventDetectionParameterElement(EventDetectionParametersViewModel parameters)
        {
            var edParameters = new XElement("EventDetectorParam");
            //if (!string.IsNullOrEmpty(parameters.RMSlength))
            //{
            //    edParameters.Add(new XElement("RMSlength", parameters.RMSlength));
            //}
            //if (!string.IsNullOrEmpty(parameters.RMSmedianFilterTime))
            //{
            //    edParameters.Add(new XElement("RMSmedianFilterTime", parameters.RMSmedianFilterTime));
            //}
            //if (!string.IsNullOrEmpty(parameters.RingThresholdScale))
            //{
            //    edParameters.Add(new XElement("RingThresholdScale", parameters.RingThresholdScale));
            //}
            if (!string.IsNullOrEmpty(parameters.MinAnalysisLength))
            {
                edParameters.Add(new XElement("MinAnalysisLength", parameters.MinAnalysisLength));
            }
            if (!string.IsNullOrEmpty(parameters.Threshold))
            {
                edParameters.Add(new XElement("Threshold", parameters.Threshold));
            }
            if (parameters.RingdownID)
            {
                edParameters.Add(new XElement("RingdownID", "TRUE"));
            }
            else
            {
                edParameters.Add(new XElement("RingdownID", "FALSE"));
            }
            edParameters.Add(new XElement("ForgetFactor1", parameters.ForgetFactor1));
            return edParameters;
        }

        private static void _writeAMethod(XElement modeElement, ModeMethodViewModel method)
        {
            XElement mth = null;
            switch (method.Name)
            {
                case ModeMethods.YWARMA:
                    mth = new XElement("AlgNames", new XElement("Name", "YW_ARMA"),
                                                        new XElement("na", method.ARModelOrder),
                                                        new XElement("nb", method.MAModelOrder),
                                                        new XElement("L", method.NumberOfEquations),
                                                        new XElement("ng", method.ExaggeratedARModelOrder),
                                                        new XElement("NaNomitLimit", method.NaNomitLimit));
                    modeElement.Add(mth);
                    break;
                case ModeMethods.LSARMA:
                    mth = new XElement("AlgNames", new XElement("Name", "LS_ARMA"),
                                                        new XElement("na", method.ARModelOrder),
                                                        new XElement("nb", method.MAModelOrder),
                                                        new XElement("n_alpha", method.ExaggeratedARModelOrder),
                                                        new XElement("NaNomitLimit", method.NaNomitLimit));
                    modeElement.Add(mth);
                    break;
                case ModeMethods.YWARMAS:
                    mth = new XElement("AlgNames", new XElement("Name", "YW_ARMApS"),
                                                        new XElement("na", method.ARModelOrder),
                                                        new XElement("nb", method.MAModelOrder),
                                                        new XElement("L", method.NumberOfEquations),
                                                        new XElement("LFO", method.NumberOfEquationsWithFOpresent),
                                                        new XElement("ng", method.ExaggeratedARModelOrder),
                                                        new XElement("NaNomitLimit", method.NaNomitLimit));
                    modeElement.Add(mth);
                    break;
                case ModeMethods.LSARMAS:
                    mth = new XElement("AlgNames", new XElement("Name", "LS_ARMApS"),
                                                        new XElement("na", method.ARModelOrder),
                                                        new XElement("nb", method.MAModelOrder),
                                                        new XElement("n_alpha", method.ExaggeratedARModelOrder),
                                                        new XElement("NaNomitLimit", method.NaNomitLimit));
                    modeElement.Add(mth);
                    break;
                default:
                    break;
            }
        }

        private XElement _writeFOParameterElement(FOdetectorParametersViewModel parameters)
        {
            var foParameters = new XElement("FOdetectorParam");//, new XElement("FrequencyInterval", parameters.FrequencyInterval),
                                                                //new XElement("FrequencyMax", parameters.FrequencyMax),
                                                                //new XElement("FrequencyMin", parameters.FrequencyMin),
                                                                //new XElement("FrequencyTolerance", parameters.FrequencyTolerance),
                                                                //new XElement("MedianFilterFrequencyWidth", parameters.MedianFilterFrequencyWidth),
                                                                //new XElement("Pfa", parameters.Pfa),
                                                                //new XElement("WindowLength", parameters.WindowLength),
                                                                //new XElement("WindowOverlap", parameters.WindowOverlap)
                                                                //);
            if (!string.IsNullOrEmpty(parameters.FODetectorParams.FrequencyInterval))
            {
                foParameters.Add(new XElement("FrequencyInterval", parameters.FODetectorParams.FrequencyInterval));
            }
            if (!string.IsNullOrEmpty(parameters.FODetectorParams.FrequencyMax))
            {
                foParameters.Add(new XElement("FrequencyMax", parameters.FODetectorParams.FrequencyMax));
            }
            if (!string.IsNullOrEmpty(parameters.FODetectorParams.FrequencyMin))
            {
                foParameters.Add(new XElement("FrequencyMin", parameters.FODetectorParams.FrequencyMin));
            }
            if (!string.IsNullOrEmpty(parameters.FODetectorParams.FrequencyTolerance))
            {
                foParameters.Add(new XElement("FrequencyTolerance", parameters.FODetectorParams.FrequencyTolerance));
            }
            if (!string.IsNullOrEmpty(parameters.FODetectorParams.MedianFilterFrequencyWidth))
            {
                foParameters.Add(new XElement("MedianFilterFrequencyWidth", parameters.FODetectorParams.MedianFilterFrequencyWidth));
            }
            if (!string.IsNullOrEmpty(parameters.FODetectorParams.Pfa))
            {
                foParameters.Add(new XElement("Pfa", parameters.FODetectorParams.Pfa));
            }
            foParameters.Add(new XElement("WindowLength", parameters.FODetectorParams.WindowLength));
            foParameters.Add(new XElement("WindowOverlap", parameters.FODetectorParams.WindowOverlap));
            XElement type = null;
            switch (parameters.FODetectorParams.WindowType)
            {
                case BAWGUI.Core.Models.DetectorWindowType.hann:
                    type = new XElement("WindowType", "hann");
                    break;
                case BAWGUI.Core.Models.DetectorWindowType.rectwin:
                    type = new XElement("WindowType", "rectwin");
                    break;
                case BAWGUI.Core.Models.DetectorWindowType.bartlett:
                    type = new XElement("WindowType", "bartlett");
                    break;
                case BAWGUI.Core.Models.DetectorWindowType.hamming:
                    type = new XElement("WindowType", "hamming");
                    break;
                case BAWGUI.Core.Models.DetectorWindowType.blackman:
                    type = new XElement("WindowType", "blackman");
                    break;
                default:
                    break;
            }
            foParameters.AddFirst(type);
            if (!string.IsNullOrEmpty(parameters.MinTestStatWinLength))
            {
                foParameters.Add(new XElement("MinTestStatWinLength", parameters.MinTestStatWinLength));
            }
            _addModePMUSignals(foParameters, parameters.PMUs);
            return foParameters;
        }

        private void _addModePMUSignals(XElement modeElement, ObservableCollection<SignalSignatureViewModel> signals)
        {
            var PMUSignalDictionary = signals.GroupBy(x => x.PMUName).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var pmuGroup in PMUSignalDictionary)
            {
                var pmu = new XElement("PMU");
                pmu.Add(new XElement("Name", pmuGroup.Key));
                foreach (var signal in pmuGroup.Value)
                {
                    var channel = new XElement("Channel");
                    channel.Add(new XElement("Name", signal.SignalName));
                    pmu.Add(channel);
                }
                modeElement.Add(pmu);
            }
        }

        private static XElement _addBaseliningSignals(SmallSignalStabilityToolViewModel detector)
        {
            var baseliningSignals = new XElement("BaseliningSignals");
            var PMUSignalDictionary = detector.BaseliningSignals.GroupBy(x => x.PMUName).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var pmuGroup in PMUSignalDictionary)
            {
                var pmu = new XElement("PMU");
                pmu.Add(new XElement("Name", pmuGroup.Key));
                foreach (var signal in pmuGroup.Value)
                {
                    var channel = new XElement("Channel");
                    channel.Add(new XElement("Name", signal.SignalName));
                    pmu.Add(channel);
                }
                baseliningSignals.Add(pmu);
                //baseliningSignals.Add(new XElement("PMU", detector.EventMergeWindow));
            }
            return baseliningSignals;
        }
        private string _configFilePath;
    }
}
