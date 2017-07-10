using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class DateStruct:ViewModelBase
    {
        public DateStruct(string y, string m, string d)
        {
            _year = y;
            _month = m;
            _day = d;
        }
        private string _year;
        public string Year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }
        private string _month;
        public string Month
        {
            get { return _month; }
            set
            {
                _month = value;
                OnPropertyChanged();
            }
        }
        private string _day;
        public string Day
        {
            get { return _day; }
            set
            {
                _day = value;
                OnPropertyChanged();
            }
        }
    }
}
