using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Results.Models;

namespace BAWGUI.Results.ViewModels
{
    class ResultsViewModel
    {
        private ForcedOscillationResultsViewModel _forcedOscillationResultsViewModel = new ForcedOscillationResultsViewModel();
        private OutOfRangeResultsViewModel _outOfRangeResultsViewModel = new OutOfRangeResultsViewModel();
        private RingdownResultsViewModel _ringdownResultsViewModel = new RingdownResultsViewModel();
        private WindRampResultsViewModel _windRampResultsViewModel = new WindRampResultsViewModel();
        private ResultsModel _resultsModel = new ResultsModel();

        public ResultsViewModel()
        {

        }

        public void LoadResults(string filename)
        {
            this._resultsModel.LoadResults(filename);
        }
    }
}
