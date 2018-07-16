using BAWGUI.MATLABRunResults.Models;
using BAWGUI.Results.Models;
using System.Collections.Generic;

namespace BAWGUI.Results.ViewModels
{
    public class RingdownEventViewModel
    {
        private RingDownEvent _model;

        public RingdownEventViewModel()
        {
        }

        public RingdownEventViewModel(RingDownEvent model)
        {
            this._model = model;
            _channels.Clear();
            foreach (var ch in _model.Channels)
            {
                _channels.Add(new RDChannelViewModel(ch));
            }
        }
        public string ID
        {
            get { return _model.ID; }
        }
        public string StartTime
        {
            get { return _model.StartTime; }
        }
        public string EndTime
        {
            get { return _model.EndTime; }
        }
        public string Duration
        {
            get { return _model.Duration; }
        }
        public string NumberOfChannels
        {
            get { return _model.Channels.Count.ToString(); }
        }
        private List<RDChannelViewModel> _channels = new List<RDChannelViewModel>();
        public List<RDChannelViewModel> Channels
        {
            get { return _channels; }
        }
    }
}