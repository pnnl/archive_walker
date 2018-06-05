using System;
using System.Drawing;
using BAWGUI.Results.Models;
using OxyPlot;

namespace BAWGUI.Results.ViewModels
{
    public class WindRampEventViewModel: Core.ViewModelBase
    {
        private WindRampEvent _model;
        public KnownColor RandColor { get; }
        public string SignalColorName
        {
            get { return RandColor.ToString(); }
        }
        public Color SignalSystemColor
        {
            get { return Color.FromKnownColor(RandColor); }
        }
        public WindRampEventViewModel()
        {
            _isSignalSelected = true;
        }
        public WindRampEventViewModel(WindRampEvent model)
        {
            _model = model;
            _isSignalSelected = true;
        }

        public WindRampEventViewModel(WindRampEvent model, KnownColor randColor) : this(model)
        {
            this.RandColor = randColor;
            var thisColor = Color.FromKnownColor(randColor);
            _signalColor = OxyColor.FromArgb(thisColor.A, thisColor.R, thisColor.G, thisColor.B);
        }

        public WindRampEventViewModel(WindRampEvent model, string selectedColor) : this(model)
        {
            var thisColor = Color.FromName(selectedColor);
            this.RandColor = (KnownColor)Enum.Parse(typeof(KnownColor), selectedColor);
            _signalColor = OxyColor.FromArgb(thisColor.A, thisColor.R, thisColor.G, thisColor.B);
        }

        public string ID
        {
            get { return _model.ID; }
        }
        public string TrendStart
        {
            get { return _model.TrendStart; }
        }
        public string TrendEnd
        {
            get { return _model.TrendEnd; }
        }
        public string TrendValue
        {
            get { return _model.TrendValue; }
        }
        public string PMU
        {
            get { return _model.PMU; }
        }
        public string Channel
        {
            get { return _model.Channel; }
        }
        public string Duration
        {
            get { return _model.Duration; }
        }
        public float ValueStart
        {
            get { return _model.ValueStart; }
        }
        public float ValueEnd
        {
            get { return _model.ValueEnd; }
        }

        public float GetHigherValue()
        {
            if (ValueStart > ValueEnd)
            {
                return ValueStart;
            }
            else
            {
                return ValueEnd;
            }
        }

        public float GetLowerValue()
        {
            if (ValueStart < ValueEnd)
            {
                return ValueStart;
            }
            else
            {
                return ValueEnd;
            }
        }
        private OxyColor _signalColor;
        public OxyColor SignalColor
        {
            get { return _signalColor; }
            set
            {
                _signalColor = value;
                OnPropertyChanged();
            }
        }
        private bool _isSignalSelected;
        public bool IsSignalSelected
        {
            get { return _isSignalSelected; }
            set
            {
                _isSignalSelected = value;
                OnPropertyChanged();
                //OnSignalSelectionChanged();
            }
        }

        //public event EventHandler SignalSelectionChanged;
        //private void OnSignalSelectionChanged() => SignalSelectionChanged?.Invoke(this, EventArgs.Empty);
    }
}