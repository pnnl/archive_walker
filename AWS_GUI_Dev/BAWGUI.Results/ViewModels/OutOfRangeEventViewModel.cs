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
        public string Duration
        {
            get { return _model.Duration; }
        }
        public string Extrema
        {
            get { return _model.Extrema; }
        }
        public string ExtremaFactor
        {
            get { return _model.ExtremaFactor; }
        }
        public string NumberOfChannels
        {
            get { return _model.Channels.Count.ToString(); }
        }
        private List<OORChannelViewModel> _channels = new List<OORChannelViewModel>();
        public List<OORChannelViewModel> Channels
        {
            get { return _channels; }
        }
    }
}
