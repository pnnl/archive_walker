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
            var ef = mm.Element("CalcDEF");
            if (ef != null)
            {
                if (ef.Value.ToUpper() == "TRUE")
                {
                    mmDetector.CalcDEF = true;
                }
                else
                {
                    mmDetector.CalcDEF = false;
                }
            }
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
                newMode.ModeName = name.Value;
            }
            try
            {
                newMode.PMUs = PMUElementReader.ReadPMUElements(mode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //var dampRatioThreshold = mode.Element("DampRatioThreshold");
            //if (dampRatioThreshold != null)
            //{
            //    newMode.DampRatioThreshold = dampRatioThreshold.Value;
            //}
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
            FOdetectorParameters FOparams = null;
            var foDetectorParams = mode.Element("FOdetectorParam");
            if (foDetectorParams != null && foDetectorParams.HasElements)
            {
                if (FOparams == null)
                {
                    FOparams = new FOdetectorParameters();
                }
                ReadFODetectorParameters(FOparams, foDetectorParams);
            }
            var FOtimeLocParam = mode.Element("FOtimeLocParam");
            if (FOtimeLocParam != null && FOtimeLocParam.HasElements)
            {
                if (FOparams == null)
                {
                    FOparams = new FOdetectorParameters();
                }
                ReadFOtimeLocParam(FOparams, FOtimeLocParam);
            }
            // if both FOdetectorParam and FOtimeLocParam do not exist, FOparams would be null
            if (FOparams != null)
            {
                newMode.ShowFOParameters = true;
                newMode.FODetectorParas = FOparams;
            }
            var eventDetectionParameters = mode.Element("EventDetectorParam");
            if (eventDetectionParameters != null && eventDetectionParameters.HasElements)
            {
                newMode.ShowRMSEnergyTransientParameters = true;
                var eventDetectionParams = ReadEventDetectionParameters(eventDetectionParameters);
                newMode.EventDetectionPara = eventDetectionParams;
            }    
            return newMode;
        }
        private EventDetectionParameters ReadEventDetectionParameters(XElement eventDetectionParameters)
        {
            var parameters = new EventDetectionParameters();
            //var par = eventDetectionParameters.Element("RMSlength");
            //if (par != null)
            //{
            //    parameters.RMSlength = par.Value;
            //}
            //par = eventDetectionParameters.Element("RMSmedianFilterTime");
            //if (par != null)
            //{
            //    parameters.RMSmedianFilterTime = par.Value;
            //}
            //par = eventDetectionParameters.Element("RingThresholdScale");
            //if (par != null)
            //{
            //    parameters.RingThresholdScale = par.Value;
            //}
            var par = eventDetectionParameters.Element("MinAnalysisLength");
            if (par != null)
            {
                parameters.MinAnalysisLength = par.Value;
            }
            par = eventDetectionParameters.Element("Threshold");
            if (par != null)
            {
                parameters.Threshold = par.Value;
            }
            par = eventDetectionParameters.Element("RingdownID");
            if (par != null)
            {
                if (par.Value.ToUpper() == "TRUE")
                {
                    parameters.RingdownID = true;
                }
                else
                {
                    parameters.RingdownID = false;
                }
            }
            par = eventDetectionParameters.Element("ForgetFactor1");
            if (par != null)
            {
                parameters.ForgetFactor1 = (ForgetFactor1Type)Enum.Parse(typeof(ForgetFactor1Type), par.Value);
            }
            par = eventDetectionParameters.Element("ForgetFactor2");
            if (par != null)
            {
                parameters.ForgetFactor2 = (ForgetFactor2Type)Enum.Parse(typeof(ForgetFactor2Type), par.Value);
            }
            par = eventDetectionParameters.Element("PostEventWinAdj");
            if (par != null)
            {
                parameters.PostEventWinAdj = (PostEventWinAdjType)Enum.Parse(typeof(PostEventWinAdjType), par.Value);
            }
            try
            {
                parameters.PMUs = PMUElementReader.ReadPMUElements(eventDetectionParameters);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return parameters;
        }

        public void ReadFODetectorParameters(FOdetectorParameters fOparams, XElement foDetectorParams)
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
            par = foDetectorParams.Element("FrequencyMin");
            if (par != null)
            {
                parameters.FrequencyMin = par.Value;
            }
            par = foDetectorParams.Element("FrequencyMax");
            if (par != null)
            {
                parameters.FrequencyMax = par.Value;
            }
            par = foDetectorParams.Element("FrequencyTolerance");
            if (par != null)
            {
                parameters.FrequencyTolerance = par.Value;
            }
            par = foDetectorParams.Element("CalcDEF");
            if (par != null)
            {
                if (par.Value.ToUpper() == "TRUE")
                {
                    parameters.CalcDEF = true;
                }
                else
                {
                    parameters.CalcDEF = false;
                }
            }
            fOparams.FODetectorParams = parameters;
            par = foDetectorParams.Element("MinTestStatWinLength");
            if (par != null)
            {
                fOparams.MinTestStatWinLength = par.Value;
            }
            try
            {
                fOparams.PMUs = PMUElementReader.ReadPMUElements(foDetectorParams);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void ReadFOtimeLocParam(FOdetectorParameters fOparams, XElement fOtimeLocParam)
        {
            var parameters = new FOtimeLocParameters();
            var par = fOtimeLocParam.Element("PerformTimeLoc");
            if (par != null)
            {
                if (par.Value.ToUpper() == "TRUE")
                {
                    parameters.PerformTimeLoc = true;
                }
                else
                {
                    parameters.PerformTimeLoc = false;
                }
            }
            par = fOtimeLocParam.Element("LocMinLength");
            if (par != null)
            {
                parameters.LocMinLength = par.Value;
            }
            par = fOtimeLocParam.Element("LocLengthStep");
            if (par != null)
            {
                parameters.LocLengthStep = par.Value;
            }
            par = fOtimeLocParam.Element("LocRes");
            if (par != null)
            {
                parameters.LocRes = par.Value;
            }
            fOparams.FOtimeLocParams = parameters;
        }
        public List<ModeMethod> ReadMethods(IEnumerable<XElement> algs)
        {
            var methods = new List<ModeMethod>();
            foreach (var alg in algs)
            {
                ModeMethod aNewMethod = new ModeMethod();
                //string arModelOrder = null, maModelOrder = null, numberOfEquations = null, exaggeratedARmodelOrder = null, numberOfEquationsWithFOpresent = null;
                var na = alg.Element("na");
                if (na != null)
                {
                    //arModelOrder = na.Value;
                    aNewMethod.ARModelOrder = na.Value;
                }
                var nb = alg.Element("nb");
                if (nb != null)
                {
                    //maModelOrder = nb.Value;
                    aNewMethod.MAModelOrder = nb.Value;
                }
                var l = alg.Element("L");
                if (l != null)
                {
                    //numberOfEquations = l.Value;
                    aNewMethod.NumberOfEquations = l.Value;
                }
                var lfo = alg.Element("LFO");
                if (lfo != null)
                {
                    //numberOfEquationsWithFOpresent = lfo.Value;
                    aNewMethod.NumberOfEquationsWithFOpresent = lfo.Value;
                }
                var nan = alg.Element("NaNomitLimit");
                if (nan != null)
                {
                    aNewMethod.NaNomitLimit = nan.Value;
                }
                var niter = alg.Element("NumIteration");
                if (niter != null)
                {
                    //numberOfEquationsWithFOpresent = lfo.Value;
                    aNewMethod.MaximumIterations = niter.Value;
                }
                var thresh = alg.Element("thresh");
                if (thresh != null)
                {
                    aNewMethod.SVThreshold = thresh.Value;
                }
                var enabletl = alg.Element("EnableTimeLoc");
                if (enabletl != null)
                {
                    if (enabletl.Value.ToUpper() == "TRUE")
                    {
                        aNewMethod.EnableTimeLoc = true;
                    }
                    else
                    {
                        aNewMethod.EnableTimeLoc = false;
                    }
                }
                var userf = alg.Element("UseRefinedFreq");
                if (userf != null)
                {
                    if (userf.Value.ToUpper() == "TRUE")
                    {
                        aNewMethod.UseRefinedFreq = true;
                    }
                    else
                    {
                        aNewMethod.UseRefinedFreq = false;
                    }
                }
                var mName = alg.Element("Name");
                if (mName != null)
                {
                    //aNewMethod.Name = (ModeMethods)Enum.Parse(typeof(ModeMethods), mName.Value);
                    //ModeMethod aNewMethod = null;
                    var methodName = mName.Value;
                    switch (methodName)
                    {
                        case "YW_ARMA":
                            aNewMethod.Name = ModeMethods.YWARMA;
                            var ng = alg.Element("ng");
                            if (ng != null)
                            {
                                aNewMethod.ExaggeratedARModelOrder = ng.Value;
                            }
                            //aNewMethod = new YWARMA
                            //{
                            //    ARModelOrder = arModelOrder,
                            //    MAModelOrder = maModelOrder,
                            //    NumberOfEquations = numberOfEquations
                            //};
                            //methods.Add(aNewMethod);
                            break;
                        case "LS_ARMA":
                            aNewMethod.Name = ModeMethods.LSARMA;
                            var nalpha = alg.Element("n_alpha");
                            if (nalpha != null)
                            {
                                //exaggeratedARmodelOrder = nalpha.Value;
                                aNewMethod.ExaggeratedARModelOrder = nalpha.Value;
                            }
                            //aNewMethod = new LSARMA
                            //{
                            //    ARModelOrder = arModelOrder,
                            //    MAModelOrder = maModelOrder,
                            //    ExaggeratedARModelOrder = exaggeratedARmodelOrder
                            //};
                            //methods.Add(aNewMethod);
                            break;
                        case "YW_ARMApS":
                            aNewMethod.Name = ModeMethods.YWARMAS;
                            var ng2 = alg.Element("ng");
                            if (ng2 != null)
                            {
                                aNewMethod.ExaggeratedARModelOrder = ng2.Value;
                            }
                            //aNewMethod = new YWARMAS()
                            //{
                            //    ARModelOrder = arModelOrder,
                            //    MAModelOrder = maModelOrder,
                            //    NumberOfEquations = numberOfEquations,
                            //    NumberOfEquationsWithFOpresent = numberOfEquationsWithFOpresent
                            //};
                            //methods.Add(aNewMethod);
                            break;
                        case "LS_ARMApS":
                            aNewMethod.Name = ModeMethods.LSARMAS;
                            var nalpha2 = alg.Element("n_alpha");
                            if (nalpha2 != null)
                            {
                                //exaggeratedARmodelOrder = nalpha.Value;
                                aNewMethod.ExaggeratedARModelOrder = nalpha2.Value;
                            }
                            //aNewMethod = new LSARMAS()
                            //{
                            //    ARModelOrder = arModelOrder,
                            //    MAModelOrder = maModelOrder,
                            //    ExaggeratedARModelOrder = exaggeratedARmodelOrder
                            //};
                            //methods.Add(aNewMethod);
                            break;
                        case "STLS":
                            aNewMethod.Name = ModeMethods.STLS;
                            var nalpha3 = alg.Element("n_alpha");
                            if (nalpha3 != null)
                            {
                                aNewMethod.ExaggeratedARModelOrder = nalpha3.Value;
                            }
                            break;
                        case "STLSpS":
                            aNewMethod.Name = ModeMethods.STLSS;
                            var nalpha4 = alg.Element("n_alpha");
                            if (nalpha4 != null)
                            {
                                aNewMethod.ExaggeratedARModelOrder = nalpha4.Value;
                            }
                            break;
                        default:
                            throw new Exception("Method name not recognized!");
                    }
                    methods.Add(aNewMethod);
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
