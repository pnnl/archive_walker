using BAWGUI.Results.Models;
using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class OccurrenceViewModel:ViewModelBase
    {
        public OccurrenceViewModel(DatedOccurrence model)
        {
            _model = model;
            _channels.Clear();
            foreach(var ch in _model.Channels)
            {
                _channels.Add(new ChannelViewModel(ch));
            }
        }
        private readonly DatedOccurrence _model;
        public DatedOccurrence Model
        {
            get { return _model; }
        }
        public string Start
        {
            get { return _model.Start; }
        }
        public string End
        {
            get { return _model.End; }
        }
        public string ID
        {
            get { return _model.ID; }
        }
        public string Alarm
        {
            get
            {
                if (_model.Alarm)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }
        //public string Frequency
        //{
        //    get { return _model.Frequency.ToString(); }
        //}
        //public string Persistence
        //{
        //    get { return _model.Persistence.ToString(); }
        //}
        ////private double _maxAmplitude;
        //public string MaxAmplitude
        //{
        //    get
        //    {
        //        if (float.IsNaN(_model.MaxAmplitude))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.MaxAmplitude.ToString();
        //        }
        //    }
        //}
        ////private double _maxSNR;
        //public string MaxSNR
        //{
        //    get
        //    {
        //        if (float.IsNaN(_model.MaxSNR))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.MaxSNR.ToString();
        //        }
        //    }
        //}
        ////private double _maxCoherence;
        //public string MaxCoherence
        //{
        //    get
        //    {
        //        if (float.IsNaN(_model.MaxCoherence))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.MaxCoherence.ToString();
        //        }
        //    }
        //}
        public float Frequency
        {
            get { return _model.Frequency; }
        }
        public float Persistence
        {
            get { return _model.Persistence; }
        }
        public float MaxAmplitude
        {
            get
            {
                return _model.MaxAmplitude;
            }
        }
        public float MaxSNR
        {
            get
            {
                return _model.MaxSNR;
            }
        }
        //private double _maxCoherence;
        public float MaxCoherence
        {
            get
            {
                return _model.MaxCoherence;
            }
        }
        private List<ChannelViewModel> _channels = new List<ChannelViewModel>();
        public List<ChannelViewModel> Channels
        {
            get { return _channels; }
        }
    }
}
