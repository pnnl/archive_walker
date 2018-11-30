using BAWGUI.MATLABRunResults.Models;
using BAWGUI.Results.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class OutOfRangeEventViewModel
    {
        private OutOfRangeEvent _model;
        public OutOfRangeEventViewModel()
        {

        }
        public OutOfRangeEventViewModel(OutOfRangeEvent model)
        {
            _model = model;
            _channels.Clear();
            foreach (var ch in _model.Channels)
            {
                _channels.Add(new OORChannelViewModel(ch));
            }

        }
        public string ID
        {
            get { return _model.ID; }
        }
        public string StartTime
        {
            get { return _model.Start; }
        }
        public string EndTime
        {
            get { return _model.End; }
        }
        public double? Duration
        {
            get
            {
                try
                {
                    var s = Double.Parse(_model.Duration);
                    return s;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public double? Extrema
        {
            get
            {
                try
                {
                    var s = Double.Parse(_model.Extrema);
                    return s;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public double? ExtremaFactor
        {
            get
            {
                try
                {
                    var s = Double.Parse(_model.ExtremaFactor);
                    return s;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public int NumberOfChannels
        {
            get { return _model.Channels.Count; }
        }
        private List<OORChannelViewModel> _channels = new List<OORChannelViewModel>();
        public List<OORChannelViewModel> Channels
        {
            get { return _channels; }
        }
    }
}
