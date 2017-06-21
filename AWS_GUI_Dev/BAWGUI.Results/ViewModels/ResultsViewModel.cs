using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Results.Models;
using BAWGUI.Xml;
using System.Globalization;

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

        public OutOfRangeResultsViewModel OutOfRangeResultsViewModel
        {
            get { return this._outOfRangeResultsViewModel; }
        }

        public RingdownResultsViewModel RingdownResultsViewModel
        {
            get { return this._ringdownResultsViewModel; }
        }

        public WindRampResultsViewModel WindRampResultsViewModel
        {
            get { return this._windRampResultsViewModel; }
        }

        public ResultsViewModel()
        {

        }

        public void LoadResults(List<string> filenames, string startDate, string endDate)
        {
            this._resultsModel.LoadResults(filenames, startDate, endDate);
            _forcedOscillationResultsViewModel.Models = _resultsModel.ForcedOscillationCombinedList;
            _forcedOscillationResultsViewModel.SelectedStartTime = DateTime.ParseExact(_resultsModel.SelectedStartTime, "yyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss");
            _forcedOscillationResultsViewModel.SelectedEndTime = DateTime.ParseExact(_resultsModel.SelectedEndTime, "yyMMdd", CultureInfo.InvariantCulture).AddDays(1).ToString("MM/dd/yyyy HH:mm:ss");
        }
    }
}
