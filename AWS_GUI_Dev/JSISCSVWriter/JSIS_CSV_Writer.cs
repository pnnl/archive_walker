using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSISCSVWriter
{
    public class JSIS_CSV_Writer
    {
        public List<Signal> Signals { get; set; }
        public string FileToBeSaved { get; set; }

        public void WriteJSISCSV()
        {
            var data = new DataToBeWritten(Signals);
            using (StreamWriter outputFile = new StreamWriter(FileToBeSaved))
            {
                outputFile.WriteLine(String.Join(",", data.NameRowList));
                outputFile.WriteLine(String.Join(",", data.TypeRowList));
                outputFile.WriteLine(String.Join(",", data.UnitRowList));
                outputFile.WriteLine(String.Join(",", data.PMUList));
                for (int index = 0; index < data.Data.RowCount - 1; index++)
                {
                    outputFile.WriteLine(string.Join(",", data.Data.Row(index).ToArray()));
                }
            }
        }
    }
}
