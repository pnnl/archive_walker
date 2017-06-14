using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;

namespace BAWGUI.Results.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Forced Oscillation event.
    /// TODO: Maybe change the name here. Too similar to ForcedOscillationResultsViewModel.
    /// </summary>
    public class ForcedOscillationResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ForcedOscillationType _model;

        public ForcedOscillationType Model
        {
            get { return this._model; }
        }

        public string Label
        {
            get { return "Event " + this._model.ID; }
        }

        public ForcedOscillationResultViewModel(ForcedOscillationType model)
        {
            this._model = model;
        }
    }
}
