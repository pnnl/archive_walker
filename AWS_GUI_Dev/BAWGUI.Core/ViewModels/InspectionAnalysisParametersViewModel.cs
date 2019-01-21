using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core.ViewModels
{
    public class InspectionAnalysisParametersViewModel : ViewModelBase
    {
        public InspectionAnalysisParametersViewModel()
        {
            _model = new InspectionAnalysisParameters();
        }
        private InspectionAnalysisParameters _model;
        public int AnalysisLength
        {
            get { return _model.AnalysisLength; }
            set
            {
                _model.AnalysisLength = value;
                OnPropertyChanged();
            }
        }
        public int WindowLength
        {
            get { return _model.WindowLength; }
            set
            {
                _model.WindowLength = value;
                OnPropertyChanged();
            }
        }
        public DetectorWindowType WindowType
        {
            get { return _model.WindowType; }
            set
            {
                _model.WindowType = value;
                OnPropertyChanged();
            }
        }
        public int WindowOverlap
        {
            get { return _model.WindowOverlap; }
            set
            {
                _model.WindowOverlap = value;
                OnPropertyChanged();
            }
        }
        public int ZeroPadding
        {
            get { return _model.ZeroPadding; }
            set
            {
                _model.ZeroPadding = value;
                OnPropertyChanged();
            }
        }
        public int Fs
        {
            get { return _model.Fs; }
            set
            {
                _model.Fs = value;
                OnPropertyChanged();
            }
        }
        public bool LogScale
        {
            get { return _model.LogScale; }
            set
            {
                _model.LogScale = value;
                OnPropertyChanged();
            }
        }
        public int FreqMin
        {
            get { return _model.FreqMin; }
            set
            {
                _model.FreqMin = value;
                OnPropertyChanged();
            }
        }
        public int FreqMax
        {
            get { return _model.FreqMax; }
            set
            {
                _model.FreqMax = value;
                OnPropertyChanged();
            }
        }
    }
}
