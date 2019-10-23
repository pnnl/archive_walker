using BAWGUI.Results.Models;
using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Core;
using BAWGUI.Utilities;
using BAWGUI.MATLABRunResults.Models;
using DissipationEnergyFlowResults.ViewModels;

namespace BAWGUI.Results.ViewModels
{
    public class OccurrenceViewModel:ViewModelBase
    {
        //public OccurrenceViewModel()
        //{
        //    _channels = new List<ChannelViewModel>();
        //}
        public OccurrenceViewModel(DatedOccurrence model)
        {
            _model = model;
            _channels.Clear();
            _paths.Clear();
            foreach(var ch in _model.Channels)
            {
                _channels.Add(new FOOccurrenceChannelViewModel(ch));
            }
            _selectedChannel = _channels.FirstOrDefault();
            var paths = _model.Paths;
            if (paths != null)
            {
                foreach (var pth in _model.Paths)
                {
                    _paths.Add(new FOOccurrencePathViewModel(pth));
                }
                _selectedPath = _paths.FirstOrDefault();
            }
        }
        public OccurrenceViewModel()
        {

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
                    return "YES";
                }
                else
                {
                    return "NO";
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
        private List<FOOccurrenceChannelViewModel> _channels = new List<FOOccurrenceChannelViewModel>();
        //private List<ChannelViewModel> _channels;
        public List<FOOccurrenceChannelViewModel> Channels
        {
            get { return _channels; }
        }
        private List<FOOccurrencePathViewModel> _paths = new List<FOOccurrencePathViewModel>();
        //private List<ChannelViewModel> _channels;
        public List<FOOccurrencePathViewModel> Paths
        {
            get { return _paths; }
        }
        public int trackerKey { get; set; }
        private FOOccurrenceChannelViewModel _selectedChannel;
        public FOOccurrenceChannelViewModel SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                if (_selectedChannel != value)
                {
                    _selectedChannel = value;
                    OnPropertyChanged();
                    OnSelectedChannelChanged();
                }
            }
        }
        private FOOccurrencePathViewModel _selectedPath;
        public FOOccurrencePathViewModel SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (_selectedPath != value)
                {
                    _selectedPath = value;
                    OnPropertyChanged();
                    //OnSelectedChannelChanged();
                }
            }
        }
        public event EventHandler SelectedChannelChanged;
        public virtual void OnSelectedChannelChanged()
        {
            SelectedChannelChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
