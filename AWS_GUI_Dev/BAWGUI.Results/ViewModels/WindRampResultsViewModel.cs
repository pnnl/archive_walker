﻿using BAWGUI.RunMATLAB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class WindRampResultsViewModel:ViewModelBase
    {
        public WindRampResultsViewModel()
        {
            _configFilePath = "";
        }
        private string _configFilePath;
        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                _configFilePath = value;
                OnPropertyChanged();
            }
        }
    }
}
