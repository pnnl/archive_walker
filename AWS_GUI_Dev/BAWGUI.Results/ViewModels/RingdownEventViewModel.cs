using BAWGUI.MATLABRunResults.Models;
using BAWGUI.Results.Models;
using System;
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
        public int NumberOfChannels
        {
            get { return _model.Channels.Count; }
        }
        private List<RDChannelViewModel> _channels = new List<RDChannelViewModel>();
        public List<RDChannelViewModel> Channels
        {
            get { return _channels; }
        }
    }
}