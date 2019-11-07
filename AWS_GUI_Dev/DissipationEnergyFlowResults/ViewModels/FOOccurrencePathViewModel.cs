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
            get 
            {
                if (_model.DEF >= 0)
                {
                    return _model.From;
                }
                else
                {
                    return _model.To;
                }
            }
        }
        public string To
        {
            get
            {
                if (_model.DEF >= 0)
                {
                    return _model.To;
                }
                else
                {
                    return _model.From;
                }
            }
        }
        public float DEF
        {
            get { return _model.DEF; }
        }
        public float ABSDEF
        {
            get { return Math.Abs(_model.DEF); }
        }
        public string FromActual
        {
            get { return _model.From; }
        }
        public string ToActual
        {
            get { return _model.To; }
        }
    }
}
