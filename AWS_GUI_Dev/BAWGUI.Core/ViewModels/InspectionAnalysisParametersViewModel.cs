using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
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
        public string AnalysisLengthStr
        {
            get { return _model.AnalysisLengthStr; }
            set
            {
                _model.AnalysisLengthStr = value;
                DataTable dt = new DataTable();
                AnalysisLength = (double)dt.Compute(value, "");
                OnPropertyChanged();
            }
        }
        public string WindowLengthStr
        {
            get { return _model.WindowLengthStr; }
            set
            {
                _model.WindowLengthStr = value;
                DataTable dt = new DataTable();
                WindowLength = (double)dt.Compute(value, "");
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
        public string WindowOverlapStr
        {
            get { return _model.WindowOverlapStr; }
            set
            {
                _model.WindowOverlapStr = value;
                DataTable dt = new DataTable();
                WindowOverlap = (double)dt.Compute(value, "");
                OnPropertyChanged();
            }
        }
        public int? ZeroPadding
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
                NumberOfSamplesInAnalysisLength = AnalysisLength * Fs;
                NumberOfSamplesInWindowLength = WindowLength * Fs;
                NumberOfSamplesInWindowOverlap = WindowOverlap * Fs;
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
        public double? FreqMin
        {
            get { return _model.FreqMin; }
            set
            {
                _model.FreqMin = value;
                OnPropertyChanged();
            }
        }
        public double? FreqMax
        {
            get { return _model.FreqMax; }
            set
            {
                _model.FreqMax = value;
                OnPropertyChanged();
            }
        }
        public double NumberOfSamplesInAnalysisLength
        {
            get { return _model.NumberOfSamplesInAnalysisLength; }
            set
            {
                _model.NumberOfSamplesInAnalysisLength = value;
                OnPropertyChanged();
            }
        }
        public double NumberOfSamplesInWindowLength
        {
            get { return _model.NumberOfSamplesInWindowLength; }
            set
            {
                _model.NumberOfSamplesInWindowLength = value;
                OnPropertyChanged();
            }
        }
        public double NumberOfSamplesInWindowOverlap
        {
            get { return _model.NumberOfSamplesInWindowOverlap; }
            set
            {
                _model.NumberOfSamplesInWindowOverlap = value;
                OnPropertyChanged();
            }
        }
        public double AnalysisLength
        {
            get { return _model.AnalysisLength; }
            set
            {
                _model.AnalysisLength = value;
                NumberOfSamplesInAnalysisLength = value * Fs;
                OnPropertyChanged();
            }
        }
        public double WindowLength
        {
            get { return _model.WindowLength; }
            set
            {
                _model.WindowLength = value;
                NumberOfSamplesInWindowLength = value * Fs;
                OnPropertyChanged();
            }
        }
        public double WindowOverlap
        {
            get { return _model.WindowOverlap; }
            set
            {
                _model.WindowOverlap = value;
                NumberOfSamplesInWindowOverlap = value * Fs;
                OnPropertyChanged();
            }
        }
    }
}
