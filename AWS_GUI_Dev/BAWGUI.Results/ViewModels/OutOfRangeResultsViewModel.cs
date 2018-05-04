using BAWGUI.Results.Models;
using BAWGUI.RunMATLAB.Models;
using BAWGUI.RunMATLAB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class OutOfRangeResultsViewModel:ViewModelBase
    {
        public OutOfRangeResultsViewModel()
        {
            _configFilePath = "";
            _run = new AWRunViewModel();
        }
        private string _configFilePath;
        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                _configFilePath = value;
                OnPropertyChanged();
            }
        }
        private AWRunViewModel _run;
        public AWRunViewModel Run
        {
            get { return _run; }
            set
            {
                _run = value;
                if (System.IO.File.Exists(_run.Model.ConfigFilePath))
                {
                    ConfigFilePath = _run.Model.ConfigFilePath;
                }
                OnPropertyChanged();
            }
        }

        private List<OutOfRangeEvent> _models;
        public List<OutOfRangeEvent> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                //_results.Clear();
                //_filteredResults.Clear();
                //foreach (var model in value)
                //{
                //    _results.Add(new RingdownEventViewModel(model));
                //    _filteredResults.Add(new RingdownEventViewModel(model));
                //}
                OnPropertyChanged();
            }
        }
    }
}
