using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Results.Models;

namespace BAWGUI.Results.ViewModels
{
    public class ResultsViewModel
    {
        private ForcedOscillationResultsViewModel _forcedOscillationResultsViewModel = new ForcedOscillationResultsViewModel();
        private OutOfRangeResultsViewModel _outOfRangeResultsViewModel = new OutOfRangeResultsViewModel();
        private RingdownResultsViewModel _ringdownResultsViewModel = new RingdownResultsViewModel();
        private WindRampResultsViewModel _windRampResultsViewModel = new WindRampResultsViewModel();
        private ResultsModel _resultsModel = new ResultsModel();

        public ForcedOscillationResultsViewModel ForcedOscillationResultsViewModel
        {
            get { return this._forcedOscillationResultsViewModel; }
        }

        public ResultsViewModel()
        {

        }

        public void LoadResults(string filename)
        {
            this._resultsModel.LoadResults(filename);
            this._forcedOscillationResultsViewModel.Models = this._resultsModel.Events.ForcedOscillation;
        }
    }
}
