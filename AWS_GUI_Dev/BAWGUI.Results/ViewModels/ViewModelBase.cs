using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Raise property changed event
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event</param>
        //public event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged;
	    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName()] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected virtual bool CanExecute(object param)
        {
            return true;
        }
    }
}
