using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        public ChannelViewModel(ForcedOscillationTypeOccurrenceChannel model)
        {
            _model = model;
        }
        private readonly ForcedOscillationTypeOccurrenceChannel _model;
        public ForcedOscillationTypeOccurrenceChannel Model
        {
            get { return _model; }
        }
        public string Name
        {
            get { return _model.Name; }
        }
        //public string Amplitude
        //{
        //    get
        //    {
        //        if(float.IsNaN(_model.Amplitude))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.Amplitude.ToString();
        //        }
        //    }                
        //}
        //public string SNR
        //{
        //    get
        //    {
        //        if (float.IsNaN(_model.SNR))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.SNR.ToString();
        //        }
        //    }
        //}
        //public string Coherence
        //{
        //    get
        //    {
        //        if(float.IsNaN(_model.Coherence))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.Coherence.ToString();
        //        }
        //    }
        //}
        public float Amplitude
        {
            get
            {
                return _model.Amplitude;
            }
        }
        public float SNR
        {
            get
            {
                return _model.SNR;
            }
        }
        public float Coherence
        {
            get
            {
                return _model.Coherence;
            }
        }

    }
}
