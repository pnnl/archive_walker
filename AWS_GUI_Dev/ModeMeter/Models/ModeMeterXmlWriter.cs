using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BAWGUI.Core;
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
                XElement baseliningSignals = _addBaseliningSignals(detector);
                mmElement.Add(baseliningSignals);
                foreach (var mode in detector.Modes)
                {
                    _writeAMode(mmElement, mode);
                }
                mmDetectors.AddBeforeSelf(mmElement);
            }
            configFile.Save(_configFilePath);
        }

        private void _writeAMode(XElement mmElement, ModeViewModel mode)
        {
            var modeElement = new XElement("Mode");
            modeElement.Add(new XElement("Name", mode.ModeName));
            _addModePMUSignals(modeElement, mode);
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
            if (mode.IsFODetecotrParametersVisible)
            {
                XElement foParameters = _writeFOParameterElement(mode.FODetectorParameters);
                modeElement.Add(foParameters);
            }
            mmElement.Add(modeElement);
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
                                                        new XElement("L", method.NumberOfEquations));
                    modeElement.Add(mth);
                    break;
                case ModeMethods.LSARMA:
                    mth = new XElement("AlgNames", new XElement("Name", "LS_ARMA"),
                                                        new XElement("na", method.ARModelOrder),
                                                        new XElement("nb", method.MAModelOrder),
                                                        new XElement("n_alpha", method.ExaggeratedARModelOrder));
                    modeElement.Add(mth);
                    break;
                case ModeMethods.YWARMAS:
                    mth = new XElement("AlgNames", new XElement("Name", "YW_ARMApS"),
                                                        new XElement("na", method.ARModelOrder),
                                                        new XElement("nb", method.MAModelOrder),
                                                        new XElement("L", method.NumberOfEquations),
                                                        new XElement("LFO", method.NumberOfEquationsWithFOpresent));
                    modeElement.Add(mth);
                    break;
                case ModeMethods.LSARMAS:
                    mth = new XElement("AlgNames", new XElement("Name", "LS_ARMApS"),
                                                        new XElement("na", method.ARModelOrder),
                                                        new XElement("nb", method.MAModelOrder),
                                                        new XElement("n_alpha", method.ExaggeratedARModelOrder));
                    modeElement.Add(mth);
                    break;
                default:
                    break;
            }
        }

        private static XElement _writeFOParameterElement(PeriodogramDetectorParametersViewModel parameters)
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
            if (!string.IsNullOrEmpty(parameters.FrequencyInterval))
            {
                foParameters.Add(new XElement("FrequencyInterval", parameters.FrequencyInterval));
            }
            if (!string.IsNullOrEmpty(parameters.FrequencyMax))
            {
                foParameters.Add(new XElement("FrequencyMax", parameters.FrequencyMax));
            }
            if (!string.IsNullOrEmpty(parameters.FrequencyMin))
            {
                foParameters.Add(new XElement("FrequencyMin", parameters.FrequencyMin));
            }
            if (!string.IsNullOrEmpty(parameters.FrequencyTolerance))
            {
                foParameters.Add(new XElement("FrequencyTolerance", parameters.FrequencyTolerance));
            }
            if (!string.IsNullOrEmpty(parameters.MedianFilterFrequencyWidth))
            {
                foParameters.Add(new XElement("MedianFilterFrequencyWidth", parameters.MedianFilterFrequencyWidth));
            }
            if (!string.IsNullOrEmpty(parameters.Pfa))
            {
                foParameters.Add(new XElement("Pfa", parameters.Pfa));
            }
            foParameters.Add(new XElement("WindowLength", parameters.WindowLength));
            foParameters.Add(new XElement("WindowOverlap", parameters.WindowOverlap));
            XElement type = null;
            switch (parameters.WindowType)
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
            return foParameters;
        }

        private void _addModePMUSignals(XElement modeElement, ModeViewModel mode)
        {
            var PMUSignalDictionary = mode.PMUs.GroupBy(x => x.PMUName).ToDictionary(x => x.Key, x => x.ToList());
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
