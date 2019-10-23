using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissipationEnergyFlowResults.ViewModels
{
    public class FOOccurrencePathViewModel
    {
        public FOOccurrencePathViewModel(ForcedOscillationTypeOccurrencePath model)
        {
            _model = model;
        }
        private readonly ForcedOscillationTypeOccurrencePath _model;
        public ForcedOscillationTypeOccurrencePath Model
        {
            get { return _model; }
        }
        public string From
        {
            get { return _model.From; }
        }
        public string To
        {
            get { return _model.To; }
        }
        public float DEF
        {
            get { return _model.DEF; }
        }
    }
}
