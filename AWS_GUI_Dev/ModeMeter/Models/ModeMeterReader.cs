using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeMeter.Models
{
    public class ModeMeterReader
    {
        private string _configFilePath;

        public ModeMeterReader(string configFilePath)
        {
            this._configFilePath = configFilePath;
        }
    }
}
