using BAWGUI.Core.Models;
using BAWGUI.ReadConfigXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModeMeter.Models
{
    public class ModeMeterReader
    {
        private string _configFilePath;

        public ModeMeterReader(string configFilePath)
        {
            this._configFilePath = configFilePath;
            var configFile = System.Xml.Linq.XDocument.Load(configFilePath);
            var modeMeters = configFile.Element("Config").Element("DetectorConfig").Element("Configuration").Elements("ModeMeter");
            var smallSignalStabilityTools = new List<SmallSignalStabilityTool>();
            foreach (var mm in modeMeters)
            {
                var smallSignalStabilityTool  = ReadSmallSignalStabilityTool(mm);
                smallSignalStabilityTools.Add(smallSignalStabilityTool);
            }
            _detectors = smallSignalStabilityTools;
        }

        public SmallSignalStabilityTool ReadSmallSignalStabilityTool(XElement mm)
        {
            var mmDetector = new SmallSignalStabilityTool();
            var path = mm.Element("ResultPath");
            if (path != null)
            {
                mmDetector.ResultPath = path.Value;
            }
            mmDetector.ModeMeterName = mmDetector.ResultPath.Substring(mmDetector.ResultPath.LastIndexOf("\\") + 1);
            var baseliningSigs = mm.Element("BaseliningSignals");
            if (baseliningSigs != null)
            {
                //var pmus = baseliningSigs.Elements("PMU");
                var pmus = PMUElementReader.ReadPMUElements(baseliningSigs);
                mmDetector.BaseliningSignals = pmus;
            }
            var modes = mm.Elements("Mode");
            if (modes != null)
            {
                foreach (var mode in modes)
                {
                    var m = ReadAMode(mode);
                    mmDetector.Modes.Add(m);
                }               
            }
            return mmDetector;
        }

        public Mode ReadAMode(XElement mode)
        {
            var newMode = new Mode();
            var name = mode.Element("Name");
            if (name != null)
            {
                newMode.Name = name.Value;
            }
            try
            {
                newMode.PMUs = PMUElementReader.ReadPMUElements(mode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            var dampRatioThreshold = mode.Element("DampRatioThreshold");
            if (dampRatioThreshold != null)
            {
                newMode.DampRatioThreashold = dampRatioThreshold.Value;
            }
            var al = mode.Element("AnalysisLength");
            if (al != null)
            {
                try
                {
                    newMode.AnalysisLength = Int32.Parse(al.Value);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            var rct = mode.Element("RetConTracking");
            if (rct != null)
            {
                newMode.RetConTracking = ReadRetroactiveContinuity(rct);
            }
            var dm = mode.Element("DesiredModes");
            if (dm != null)
            {
                newMode.DesiredModes = ReadDesiredModes(dm);
            }
            var algs = mode.Elements("AlgNames");
            if (algs != null)
            {
                var methods = ReadMethods(algs);
                newMode.AlgNames = methods;
            }
            var foDetectorParams = mode.Element("FOdetectorParam");
            if (foDetectorParams != null && foDetectorParams.HasElements)
            {
                var foDetectorParameters = ReadFODetectorParameters(foDetectorParams);
                newMode.FODetectorParameters = foDetectorParameters;
            }
            return newMode;
        }

        public PeriodogramDetectorModel ReadFODetectorParameters(XElement foDetectorParams)
        {
            var parameters = new PeriodogramDetectorModel();
            var par = foDetectorParams.Element("WindowType");
            if (par != null)
            {
                parameters.WindowType = (DetectorWindowType)Enum.Parse(typeof(DetectorWindowType), par.Value);
            }
            par = foDetectorParams.Element("FrequencyInterval");
            if (par != null)
            {
                parameters.FrequencyInterval = par.Value;
            }
            par = foDetectorParams.Element("WindowLength");
            if (par != null)
            {
                try
                {
                    parameters.WindowLength = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = foDetectorParams.Element("WindowOverlap");
            if (par != null)
            {
                try
                {
                    parameters.WindowOverlap = Int32.Parse(par.Value);
                }
                catch (Exception ex)
                {

                    throw new Exception("Integer expected. Original error: " + ex.Message);
                }
            }
            par = foDetectorParams.Element("MedianFilterFrequencyWidth");
            if (par != null)
            {
                parameters.MedianFilterFrequencyWidth = par.Value;
            }
            par = foDetectorParams.Element("Pfa");
            if (par != null)
            {
                parameters.Pfa = par.Value;
            }
            //par = foDetectorParams.Element("FrequencyMin");
            //if (par != null)
            //{
            //    parameters.FrequencyMin = par.Value;
            //}
            //par = foDetectorParams.Element("FrequencyMax");
            //if (par != null)
            //{
            //    parameters.FrequencyMax = par.Value;
            //}
            par = foDetectorParams.Element("FrequencyTolerance");
            if (par != null)
            {
                parameters.FrequencyTolerance = par.Value;
            }
            return parameters;
        }

        public List<ModeMethodBase> ReadMethods(IEnumerable<XElement> algs)
        {
            var methods = new List<ModeMethodBase>();
            foreach (var alg in algs)
            {
                string arModelOrder = null, maModelOrder = null, numberOfEquations = null, exaggeratedARmodelOrder = null, numberOfEquationsWithFOpresent = null;
                var na = alg.Element("na");
                if (na != null)
                {
                    arModelOrder = na.Value;
                }
                var nb = alg.Element("nb");
                if (nb != null)
                {
                    maModelOrder = nb.Value;
                }
                var l = alg.Element("L");
                if (l != null)
                {
                    numberOfEquations = l.Value;
                }
                var nalpha = alg.Element("n_alpha");
                if (nalpha != null)
                {
                    exaggeratedARmodelOrder = nalpha.Value;
                }
                var lfo = alg.Element("LFO");
                if (lfo != null)
                {
                    numberOfEquationsWithFOpresent = lfo.Value;
                }
                var mName = alg.Element("Name");
                if (mName != null)
                {
                    ModeMethodBase aNewMethod = null;
                    var methodName = mName.Value;
                    switch (methodName)
                    {
                        case "YW_ARMA":
                            aNewMethod = new YWARMA
                            {
                                ARModelOrder = arModelOrder,
                                MAModelOrder = maModelOrder,
                                NumberOfEquations = numberOfEquations
                            };
                            methods.Add(aNewMethod);
                            break;
                        case "LS_ARMA":
                            aNewMethod = new LSARMA
                            {
                                ARModelOrder = arModelOrder,
                                MAModelOrder = maModelOrder,
                                ExaggeratedARModelOrder = exaggeratedARmodelOrder
                            };
                            methods.Add(aNewMethod);
                            break;
                        case "YW_ARMApS":
                            aNewMethod = new YWARMAS()
                            {
                                ARModelOrder = arModelOrder,
                                MAModelOrder = maModelOrder,
                                NumberOfEquations = numberOfEquations,
                                NumberOfEquationsWithFOpresent = numberOfEquationsWithFOpresent
                            };
                            methods.Add(aNewMethod);
                            break;
                        case "LS_ARMApS":
                            aNewMethod = new LSARMAS()
                            {
                                ARModelOrder = arModelOrder,
                                MAModelOrder = maModelOrder,
                                ExaggeratedARModelOrder = exaggeratedARmodelOrder
                            };
                            methods.Add(aNewMethod);
                            break;
                        default:
                            throw new Exception("Method name not recognized!");
                    }
                }
            }
            return methods;
        }
        public DesiredModeAttributes ReadDesiredModes(XElement dm)
        {
            var newDesiredModeAttributes = new DesiredModeAttributes();
            var lf = dm.Element("LowF");
            if (lf != null)
            {
                newDesiredModeAttributes.LowF = lf.Value;
            }
            var hf = dm.Element("HighF");
            if (hf != null)
            {
                newDesiredModeAttributes.HighF = hf.Value;
            }
            var gf = dm.Element("GuessF");
            if (gf != null)
            {
                newDesiredModeAttributes.GuessF = gf.Value;
            }
            var dpm = dm.Element("DampMax");
            if (dpm != null)
            {
                newDesiredModeAttributes.DampMax = dpm.Value;
            }
            return newDesiredModeAttributes;
        }

        public RetroactiveContinuity ReadRetroactiveContinuity(XElement rct)
        {
            var newRct = new RetroactiveContinuity();
            var st = rct.Element("Status");
            if (st != null)
            {
                newRct.Status = (RetroactiveContinuityStatusType)Enum.Parse(typeof(RetroactiveContinuityStatusType), st.Value);
            }
            var ml = rct.Element("MaxLength");
            if (ml != null)
            {
                newRct.MaxLength = ml.Value;
            }
            return newRct;
        }

        private List<SmallSignalStabilityTool> _detectors = new List<SmallSignalStabilityTool>();
        public List<SmallSignalStabilityTool> GetDetectors()
        {
            return _detectors;
        }
    }
}
