using BAWGUI.Utilities;
using ModeMeter.Models;

namespace ModeMeter.ViewModels
{
    public class ModeViewModel : ViewModelBase
    {
        private Mode mode;

        public ModeViewModel(Mode mode)
        {
            this.mode = mode;
        }
    }
}