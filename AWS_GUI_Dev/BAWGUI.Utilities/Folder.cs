using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BAWGUI.Utilities
{
        public class Folder : ViewModelBase
        {
            public Folder(string filename, string filetype, ref string firstFile)
            {
                _name = Path.GetFileName(filename);
                _fullName = filename;
                // what If type Is nothing?
                _type = filetype;
                // firstFile = Nothing
                // _buildDirTree(filename, firstFile)
                //_findFirstFile(filename, filetype, ref firstFile);
            }
            private string _type;
            private string _name;
            public string Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
            private string _fullName;
            public string FullName
            {
                get
                {
                    return _fullName;
                }
                set
                {
                    _fullName = value;
                    OnPropertyChanged();
                }
            }
            private ObservableCollection<Folder> _subFolders;
            public ObservableCollection<Folder> SubFolders
            {
                get
                {
                    return _subFolders;
                }
                set
                {
                    _subFolders = value;
                    OnPropertyChanged();
                }
            }
            private void _buildDirTree(string filename, ref string firstFile)
            {
                if (File.Exists(filename))
                {
                    if (string.IsNullOrEmpty(firstFile))
                        firstFile = filename;
                }
                else if (Directory.Exists(filename))
                {
                    _subFolders = new ObservableCollection<Folder>();
                    foreach (var path in Directory.GetDirectories(filename))
                        _subFolders.Add(new Folder(path, _type, ref firstFile));
                    foreach (var file in Directory.GetFiles(filename))
                    {
                        // what If type Is nothing?
                        if (Path.GetExtension(file).Substring(1) == _type)
                        {
                            if (string.IsNullOrEmpty(firstFile))
                                _subFolders.Add(new Folder(file, _type, ref firstFile));
                        }
                    }
                }
                else
                    throw new Exception("\nError: data file path \"" + filename + "\" does not exists!");
            }
            //private void _findFirstFile(string filename, string filetype, ref string firstFile)
            //{
            //    if (File.Exists(filename))
            //    {
            //        if (Path.GetExtension(filename).Substring(1).ToLower() == filetype)
            //            firstFile = filename;
            //    }
            //    else if (Directory.Exists(filename))
            //    {
            //        foreach (var file in Directory.GetFiles(filename))
            //        {
            //            _findFirstFile(file, filetype, ref firstFile);
            //            if (!string.IsNullOrEmpty(firstFile))
            //                return;
            //        }
            //        foreach (var path in Directory.GetDirectories(filename))
            //        {
            //            _findFirstFile(path, filetype, ref firstFile);
            //            if (!string.IsNullOrEmpty(firstFile))
            //                return;
            //        }
            //    }
            //    else
            //        throw new Exception("\nError: data file path \"" + filename + "\" does not exists!");
            //}
        }
}
