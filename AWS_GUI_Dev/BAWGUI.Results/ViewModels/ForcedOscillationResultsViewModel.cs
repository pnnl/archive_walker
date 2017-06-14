using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;

namespace BAWGUI.Results.ViewModels
{
    /// <summary>
    /// The ViewModel for the full Forced Oscillation results view.
    /// </summary>
    public class ForcedOscillationResultsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ForcedOscillationType[] _models;
        private ObservableCollection<ForcedOscillationResultViewModel> _results = new ObservableCollection<ForcedOscillationResultViewModel>();

        public ObservableCollection<ForcedOscillationResultViewModel> Results
        {
            get { return _results; }
            set { _results = value; }
        } 

        public ForcedOscillationType[] Models
        {
            get { return _models; }
            set
            {
                _models = value;

                _results.Clear();
                foreach (var model in value)
                {
                    _results.Add(new ForcedOscillationResultViewModel(model));
                }
            }
        }

    }
}
