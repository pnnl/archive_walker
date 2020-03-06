using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BAWGUI.Core.Models
{
    public class InputFileInfoModel
    {
        public InputFileInfoModel()
        {
            FileDirectory = "";
            FileType = DataFileType.pdat;
            Mnemonic = "";
            _exampleFile = "";
        }
        public string FileDirectory { get; set; }
        private DataFileType _fileType;
        public DataFileType FileType
        {
            get { return _fileType; }
            set
            {
                _fileType = value;
                if (File.Exists(ExampleFile) && CheckDataFileMatch())
                {
                    if (value == DataFileType.PI || value == DataFileType.OpenHistorian || value == DataFileType.OpenPDC)
                    {
                        try
                        {
                            FileDirectory = Path.GetDirectoryName(ExampleFile);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        try
                        {
                            var fullPath = Path.GetDirectoryName(ExampleFile);
                            var oneLevelUp = fullPath.Substring(0, fullPath.LastIndexOf(@"\"));
                            var twoLevelUp = oneLevelUp.Substring(0, oneLevelUp.LastIndexOf(@"\"));
                            FileDirectory = twoLevelUp;
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }
        public string Mnemonic { get; set; }
        private string _exampleFile;
        public string ExampleFile
        {
            get { return _exampleFile; }
            set
            {
                _exampleFile = value;
                if (File.Exists(value) && CheckDataFileMatch())
                {
                    //try
                    //{
                    //    FileType = Path.GetExtension(value).Substring(1);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Data file type not recognized. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    //}
                    //var filename = "";
                    //try
                    //{
                    //    filename = Path.GetFileNameWithoutExtension(value);
                    //}
                    //catch (ArgumentException ex)
                    //{
                    //    MessageBox.Show("Data file path contains one or more of the invalid characters. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    //}
                    //try
                    //{
                    //    Mnemonic = filename.Substring(0, filename.Length - 16);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Error extracting Mnemonic from selected data file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                    //}
                    if (FileType == DataFileType.PI || FileType == DataFileType.OpenHistorian || FileType == DataFileType.OpenPDC)
                    {
                        try
                        {
                            FileDirectory = Path.GetDirectoryName(value);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        try
                        {
                            var fullPath = Path.GetDirectoryName(value);
                            var oneLevelUp = fullPath.Substring(0, fullPath.LastIndexOf(@"\"));
                            var twoLevelUp = oneLevelUp.Substring(0, oneLevelUp.LastIndexOf(@"\"));
                            FileDirectory = twoLevelUp;
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                        }
                    }
                }
                //else
                //{
                //    MessageBox.Show("Example input data file does not exist!", "Warning!", MessageBoxButtons.OK);
                //}
            }
        }
        public bool CheckDataFileMatch()
        {
            var tp = "";
            try
            {
                tp = Path.GetExtension(ExampleFile).Substring(1).ToLower();
            }
            catch
            {
            }
            if (FileType.ToString().ToLower() == tp)
                return true;
            else if (FileType == DataFileType.powHQ && tp == "mat")
                return true;
            else if ((FileType == DataFileType.PI || FileType == DataFileType.OpenHistorian || FileType == DataFileType.OpenPDC) && tp == "xml")
                return true;
            else
                return false;
        }
        public List<string> GetPresets(string filename)
        {
            var newPresets = new List<string>();
            var doc = XDocument.Load(filename);
            var presets = doc.Element("Presets");
            if (presets != null)
            {
                var pts = presets.Elements("Preset");
                if (pts != null)
                {
                    foreach (var item in pts)
                    {
                        if (item.HasAttributes)
                        {
                            var nm = item.Attribute("name");
                            if (nm != null)
                            {
                                newPresets.Add(nm.Value.ToString());
                            }
                        }
                    }
                }
            }
            return newPresets;
        }
    }
}
